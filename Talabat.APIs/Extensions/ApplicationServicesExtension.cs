using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.MiddleWares;
using Talabat.Core;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Repository;
using Talabat.Repository.Basket_Repository;
using Talabat.Repository.Generic_Repository;
using Talabat.Repository.Identity;
using Talabat.Service;
using Talabat.Service.AuthService;
using Talabat.Service.CacheService;
using Talabat.Service.OrderService;
using Talabat.Service.PaymentService;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension 
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {

            services.AddSingleton(typeof(IResponseCacheService), typeof(ResponseCacheService));
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            services.AddScoped(typeof(IProductService), typeof(ProductService));
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
           
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<ExceptionMiddleWare>();
            #region MyErrorResponse
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (ActionContext) =>
                {
                    var errors = ActionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                    .SelectMany(P => P.Value.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToList();

                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response);
                };

            }

           );
            #endregion

            return services;
        }


        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options => { })
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

            services.AddAuthentication(options=>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = configuration["JWT:ValidIssuer"],
            ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AuthKey"] ?? string.Empty)), // lw rg3 null 5leh string fadi 
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

    });
            services.AddScoped(typeof(IAuthService), typeof(AuthService));

            return services;
        }
    }
}
