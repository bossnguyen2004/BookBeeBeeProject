using BookBee.Persistences;
using BookBee.Persistences.Repositories.AddressRepository;
using BookBee.Persistences.Repositories.AuthorRepository;
using BookBee.Persistences.Repositories.CartDetailsRepository;
using BookBee.Persistences.Repositories.CartRepository;
using BookBee.Persistences.Repositories.CategoryRepository;
using BookBee.Persistences.Repositories.DetailedPaymentRepository;
using BookBee.Persistences.Repositories.EmployeeRepository;
using BookBee.Persistences.Repositories.OrderDetailRepository;
using BookBee.Persistences.Repositories.OrderVoucherRepository;
using BookBee.Persistences.Repositories.PaymentMethodRepository;
using BookBee.Persistences.Repositories.PublisherRepository;
using BookBee.Persistences.Repositories.SupplierRepository;
using BookBee.Persistences.Repositories.TaiQuayRepository;
using BookBee.Persistences.Repositories.UserRepository;
using BookBee.Persistences.Repositories.VoucherRepository;
using BookStack.Persistence.Repositories.BookRepository;
using BookStack.Persistence.Repositories.OrderRepository;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Extensions
{
    public static class DatabaseExtension
    {
        public static async Task ApplyMigrations(this IApplicationBuilder app)
        {
            try
            {
                using var scope = app.ApplicationServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
            }
        }

        public static async Task SeedData(this IApplicationBuilder app)
        {
            try
            {
                using var scope = app.ApplicationServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                await dbContext.SeedData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding data: {ex.Message}");
            }
        }

        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
			services.AddScoped<IAuthorRepository, AuthorRepository>();
			services.AddScoped<IPublisherRepository, PublisherRepository>();
			services.AddScoped<ISupplierRepository, SupplierRepository>();
			services.AddScoped<ITagRepository, TagRepository>();
			services.AddScoped<IVoucherRepository, VoucherRepository>();
			services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
			services.AddScoped<IOrderVoucherRepository, OrderVoucherRepository>();
			services.AddScoped<IBookRepository, BookRepository>();
			services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<ITaiQuayRepository, TaiQuayRepository>();
            services.AddScoped<IDetailedPaymentRepository, DetailedPaymentRepository>();
            services.AddScoped<ICartDetailsRepository, CartDetailsRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }

    }
}
