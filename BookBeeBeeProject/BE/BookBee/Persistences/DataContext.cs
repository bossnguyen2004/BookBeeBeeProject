using BookBee.Model;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences
{
    public class DataContext:DbContext
    {
        public DataContext(){}
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Image> Images { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Model.Address> Addresses { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<DetailedPayment> DetailedPayments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<OrderVoucher> OrderVouchers { get; set; }
        public DbSet<VoucherDetail> VoucherDetails { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne(o => o.UserAccount)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                       .HasOne(o => o.Employee)
                       .WithMany()
                       .HasForeignKey(o => o.EmployeeId)
                       .OnDelete(DeleteBehavior.NoAction); // Ngăn xóa Employee gây lỗi

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ lại Cascade nếu cần

            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderVoucher)
                .WithMany()
                .HasForeignKey(o => o.OrderVoucherId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
