using AutoMapper;
using E_Commerce.BLL.Abstraction;
using E_Commerce.BLL.Authentication;
using E_Commerce.BLL.DTOs.Authentication;
using E_Commerce.BLL.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;
using ProductService = E_Commerce.BLL.Services.ProductService;

namespace ECommerce_API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:DefaultConnection"] ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddControllers();

        services.AddSwaggerServices();

        services.AddFluentValidationConfigs();

        services.AddAutoMapperConfigs();

        services.AddAuthConfig(configuration);

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWishlistService, WishlistService>();

        // Stripe
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        services.AddScoped<IPaymentService, PaymentService>();
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

        return services;
    }

    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection AddFluentValidationConfigs(this IServiceCollection services)
    {
        //services.AddValidatorsFromAssembly(typeof(RegirsterRequestDtoValidator).Assembly);
        //services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssembly(typeof(BllAssemblyMarker).Assembly);
        services.AddFluentValidationAutoValidation();

        return services;
    }

    private static IServiceCollection AddAutoMapperConfigs(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.ShouldMapMethod = _ => false;
        });

        services.AddAutoMapper(typeof(ProductMap));
        services.AddAutoMapper(typeof(CategoryMap));
        services.AddAutoMapper(typeof(OrderMap));
        services.AddAutoMapper(typeof(RegisterUserMap));
        services.AddAutoMapper(typeof(UserMap));

        return services;
    }
    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();


        services.AddSingleton<IJwtProvider, JwtProvider>();


        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                };
            });

        // Microsoft Documentation: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-9.0
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }
}
