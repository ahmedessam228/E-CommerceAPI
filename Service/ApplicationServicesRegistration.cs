using Domain.Interfaces;
using Domain.Interfaces.Service;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Service
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICartItemsService, CartItemService>();
            services.AddScoped<IShippingAddress, ShippingAddressService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            return services;
        }
    }
}
