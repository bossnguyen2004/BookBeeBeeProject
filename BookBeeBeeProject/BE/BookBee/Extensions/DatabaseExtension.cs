using BookBee.Persistences;
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
        //    services.AddScoped<IUserRepository, UserRepository>();
        //    services.AddScoped<IAuthorRepository, AuthorRepository>();
        //    services.AddScoped<IPublisherRepository, PublisherRepository>();
        //    services.AddScoped<IAddressRepository, AddressRepository>();
        //    services.AddScoped<ITagRepository, TagRepository>();
        //    services.AddScoped<IBookRepository, BookRepository>();
        //    services.AddScoped<IOrderRepository, OrderRepository>();
        //    services.AddScoped<IQuantityRepository, QuantityRepository>();
        //    services.AddScoped<ICartRepository, CartRepository>();
        //    services.AddScoped<IShippingModeRepository, ShippingModeRepository>();
        //    services.AddScoped<IVoucherRepository, VoucherRepository>();
        }

    }
}
