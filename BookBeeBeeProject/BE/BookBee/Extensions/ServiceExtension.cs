using BookBee.Services.AddressService;
using BookBee.Services.AuthorService;
using BookBee.Services.AuthService;
using BookBee.Services.CacheService;
using BookBee.Services.CategoryService;
using BookBee.Services.MailService;
using BookBee.Services.OrderVoucherService;
using BookBee.Services.PaymentMethodService;
using BookBee.Services.PublisherService;
using BookBee.Services.StatisticalService;
using BookBee.Services.SupplierService;
using BookBee.Services.TokenService;
using BookBee.Services.UserService;
using BookBee.Services.VNPayService;
using BookBee.Services.VoucherService;
using BookBee.Utilities;
using BookStack.Services.BookService;
using BookStack.Services.CartService;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace BookBee.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<ICacheService, InMemoryCacheService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ICartService, CartService>();
            //services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPublisherService, PublisherService>();
            services.AddScoped<IStatisticalService, StatisticalService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVNPayService, VNPayService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IVoucherService, VoucherService>();
			services.AddScoped<IOrderVoucherService, OrderVoucherService>();
		}

        public static void AddUtilities(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<UserAccessor>();
        }
    }
}
