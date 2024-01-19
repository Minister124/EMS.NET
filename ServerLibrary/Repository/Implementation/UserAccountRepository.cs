using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Repository.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServerLibrary.Repository.Implementation
{
    public class UserAccountRepository(IOptions<JwtSection> section, EMSDbContext context) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user == null)
            {
                return new GeneralResponse(false, "User is Empty");
            }
            var userCheck = await FindUserByEmailAsync(user.Email!);
            if (userCheck != null)
            {
                return new GeneralResponse(false, "User Is Already Registered");
            }

            //Add User to the Database
            var addUser = await AddToDatabase(new ApplicationUser
            {
                Name = user.FullName,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });

            //check the role and create Admin Role and Assign it to the First User Registered
            var adminRoleCheck = await context.Roles.FirstOrDefaultAsync(x => x.RoleName! == Constants.Admin);
            if (adminRoleCheck == null)
            {
                var createAdminRole = await AddToDatabase(new Role
                {
                    RoleName = Constants.Admin
                });
                await AddToDatabase(new UserRole
                {
                    RoleId = createAdminRole.Id,
                    UserId = addUser.Id
                });
                return new GeneralResponse(true, "Account has been Successfullyf Created");
            }
            //checks user role if it is User, if not creates user role and assigns it to User.
            var userRoleCheck = await context.Roles.FirstOrDefaultAsync(x => x.RoleName! == Constants.User);
            Role response = new();
            if (userRoleCheck == null)
            {
                response = await AddToDatabase(new Role { RoleName = Constants.User });
                await AddToDatabase(new UserRole { RoleId = response.Id, UserId = addUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole { RoleId = userRoleCheck.Id, UserId = addUser.Id }); //if it is not null add User Role to the user
            }
            return new GeneralResponse(true, "Account Created Successfully");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            if (user == null)
            {
                return new LoginResponse(false, "User Cannot be Null");
            }
            var addUser = await FindUserByEmailAsync(user.Email!);
            if (addUser == null)
            {
                return new LoginResponse(false, "User Not Found");
            }
            //verify password by comparing
            if (!BCrypt.Net.BCrypt.Verify(user.Password, addUser.Password))
            {
                return new LoginResponse(false, "Password Does not Match");
            }
            var getUserRole = await FindUserRole(addUser.Id);
            if (getUserRole == null)
            {
                return new LoginResponse(false, "User Role Not Found");
            }
            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName == null)
            {
                return new LoginResponse(false, "RoleName Not Found");
            }

            string jwtToken = GenerateToken(addUser, getRoleName!.RoleName!);
            string refreshToken = GenerateRefereshToken();

            var findUser = await context.RefreshTokenInfos
                .FirstOrDefaultAsync(x => x.UserId == addUser.Id);
            if (findUser != null)
            {
                findUser!.Token = refreshToken;
                await context.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo
                {
                    Token = refreshToken,
                    UserId = addUser.Id
                });
            }
            return new LoginResponse(true, "Login Successfull", jwtToken, refreshToken);
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var secureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section.Value.Key!));
            var credentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!),
            };
            var token = new JwtSecurityToken(
                issuer: section.Value.Issuer,
                audience: section.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserRole?> FindUserRole(Guid uId)
        {
            return await context.UserRoles
                .FirstOrDefaultAsync(c => c.UserId == uId);
        }

        private async Task<Role?> FindRoleName(Guid rId)
        {
            return await context.Roles
                .FirstOrDefaultAsync(c => c.Id == rId);
        }

        private static string GenerateRefereshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private async Task<ApplicationUser?> FindUserByEmailAsync(string email)
        {
            return await context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Email != null && x.Email.ToLower()! == email.ToLower());
        }

        private async Task<T> AddToDatabase<T>(T model)
        {
            var add = context.Add(model!);
            await context.SaveChangesAsync();
            return (T)add.Entity;
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null)
            {
                return new LoginResponse(false, "Refresh Token is Null");
            }
            var tokenSearch = await context.RefreshTokenInfos
                .FirstOrDefaultAsync(c => c.Token! == refreshToken.Token);
            if (tokenSearch == null)
            {
                return new LoginResponse(false, "Refresh Token Is Required");
            }
            //get user Details
            var userDetails = await context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Id == tokenSearch.UserId);
            if (userDetails == null)
            {
                return new LoginResponse(false, "Refresh Token Generation Failed Because User Not Found");
            }

            var userRole = await FindUserRole(userDetails.Id);
            var roleName = await FindRoleName(userRole!.RoleId);
            string jwtToken = GenerateToken(userDetails, roleName!.RoleName!);
            string refToken = GenerateRefereshToken();

            var updateRefToken = await context.RefreshTokenInfos
                .FirstOrDefaultAsync(c => c.UserId == userDetails.Id);
            if (updateRefToken == null)
            {
                return new LoginResponse(false, "Refresh Token Could Not Be Generated Because User Has Not Signed In");
            }

            updateRefToken.Token = refToken;
            await context.SaveChangesAsync();
            return new LoginResponse(true, "Token Refreshed Successfully", jwtToken, refToken);
        }
    }
}
