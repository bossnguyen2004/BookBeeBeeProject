using Microsoft.AspNetCore.Cors.Infrastructure;

namespace BookBee.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
        //    services.AddMemoryCache();
        //    services.AddScoped<IAuthService, AuthService>();
        //    services.AddSingleton<ICacheService, InMemoryCacheService>();
        //    services.AddScoped<IAddressService, AddressService>();
        //    services.AddScoped<IAuthorService, AuthorService>();
        //    services.AddScoped<IBookService, BookService>();
        //    services.AddScoped<ICartService, CartService>();
        //    services.AddScoped<IOrderService, OrderService>();
        //    services.AddScoped<IPublisherService, PublisherService>();
        //    services.AddScoped<IStatisticalService, StatisticalService>();
        //    services.AddScoped<ITagService, TagService>();
        //    services.AddScoped<IUserService, UserService>();
        //    services.AddScoped<ITokenService, TokenService>();
        //    services.AddScoped<IVNPayService, VNPayService>();
        //    services.AddScoped<IFileStorageService, FileStorageService>();
        //    services.AddScoped<IMailService, MailService>();
        //    services.AddScoped<IShippingModeService, ShippingModeService>();
        //    services.AddScoped<IVoucherService, VoucherService>();
        }

        public static void AddUtilities(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<UserAccessor>();
        }
    }
}
