using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Repository.Contracts;

namespace ServerLibrary.Repository.Implementation
{
    public class UserAccountRepository(IOptions<JwtSection> section, EMSDbContext context) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user == null) 
            {
                return new GeneralResponse(false, "Model is Empty");
            }
            var userCheck = await FindUserByEmail(user.Email!);
            if (userCheck != null)
            {
                return new GeneralResponse(false, "User Is Already Registered");
            }
        }

        public Task<LoginResponse> SignInAsync(Login user)
        {
            throw new NotImplementedException();
        }

        private async Task<ApplicationUser?> FindUserByEmail(string email)
        {
            return await context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Email != null && x.Email.ToLower()! == email!.ToLower());
        }
    }
}
