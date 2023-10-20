using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Settings;
using UserManagement_API.Data;
using UserManagement_API.Data.Models;
using UserManagement_API.Extensions;
using UserManagement_API.Service;
using UserManagement_API.Service.IService;

namespace UserManagement_API;

public static class IdentityServiceRegistration
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddEntityFrameworkStores<UserDbContext>().AddDefaultTokenProviders()
            .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));

        services.AddScoped<IAuthService, AuthService>();

        services.SetupAuthentication(configuration);

        return services;

    }
}