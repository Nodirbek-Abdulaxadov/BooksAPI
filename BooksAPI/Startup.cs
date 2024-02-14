using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Service;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BooksAPI.DTOs;
using System.Security.Claims;
using Asp.Versioning;
using AspNetCoreRateLimit;
using StackExchange.Redis;

namespace BooksAPI;

public static class Startup
{
    readonly static string cors = "AllowAll";
    public static void AddDependencyInjectionServices(
        this WebApplicationBuilder builder)
    {
        #region API Versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        #endregion

        #region Default services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        #endregion

        #region Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(cors,
                       builder =>
                       {
                           builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader();
                       });
        });
        #endregion

        #region Add DB Context to DI Container
        builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB")));

        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddTransient<ICategoryInterface, CategoryRepository>();
        builder.Services.AddTransient<IBookInterface, BookRepository>();
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.AddTransient<ICategoryService, CategoryService>();
        builder.Services.AddTransient<IBookService, BookService>();
        builder.Services.AddTransient<IUserService, UserService>();
        #endregion

        #region Add JWT
        var key = Encoding.ASCII.GetBytes("BuVapwemSecretKey");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role
            };
        });
        #endregion

        #region Add Automapper
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AutoMapperProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);
        #endregion

        #region Add rate limiting to DI services

        builder.Services.Configure<IpRateLimitOptions>
            (builder.Configuration.GetSection("IpRateLimit"));  //load the configuration from app settings 
        builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>(); //inject counter
        builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        builder.Services.AddMemoryCache();   //store the rate limit counters and ip rules 
        builder.Services.AddHttpContextAccessor();
        #endregion

        #region Add Redis to DI services
        builder.Services.Configure<ConfigurationOptions>(
            builder.Configuration.GetSection("RedisCacheOptions"));

        builder.Services.AddStackExchangeRedisCache(setupAction =>
        {
            setupAction.Configuration = builder.Configuration
                                               .GetConnectionString("RedisConnectionString");
        });
        #endregion
    }

    public static void AddApplicationMiddlewares(
        this WebApplication app)
    {
        app.UseIpRateLimiting();
       
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors(cors);

        app.UseAuthentication();
        app.UseAuthorization();

        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .Build();

        app.MapControllers().WithApiVersionSet(versionSet);
        app.SeedRolesAndUsers().Wait();
        app.Run();
    }

    private static async Task SeedRolesAndUsers(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        //add default admin
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        const string adminEmail = "super@admin.uz";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            var superAdmin = new User
            {
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true,
                FullName = "Super Admin"
            };

            var createAdmin = await userManager.CreateAsync(superAdmin, "Admin.123$");
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "Admin");
            }
        }
    }
}