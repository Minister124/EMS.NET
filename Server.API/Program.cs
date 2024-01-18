using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementation;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EMSDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection") ??
        throw new InvalidOperationException("The Connection Was Not Established SuccessFully"));
});

builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
builder.Services.AddScoped<IUserAccount, UserAccountRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
