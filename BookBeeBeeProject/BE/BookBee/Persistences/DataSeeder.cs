using BookBee.Model;
using BookBee.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BookBee.Persistences
{
    public static class DataSeeder
    {
        public static async Task SeedData(this DataContext dbContext)
        {
            await dbContext.SeedRoles();
            await dbContext.SeedUsers();
            await dbContext.SeedAddresses();
        }

        private static async Task SeedRoles(this DataContext dbContext)
        {
            if (!await dbContext.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new() { Name = "admin" },
                    new() { Name = "user" },
                    new() { Name = "nhanvien" }
                };

                await dbContext.Roles.AddRangeAsync(roles);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedUsers(this DataContext dbContext)
        {
            if (!await dbContext.UserAccounts.AnyAsync())
            {
                const string defaultPassword = "1234@Abcd";
                PasswordHelper.CreatePasswordHash(defaultPassword, out var passwordHash, out var passwordSalt);

                var adminRole = await dbContext.Roles.FirstAsync(r => r.Name == "admin");
                var userRole = await dbContext.Roles.FirstAsync(r => r.Name == "user");
                var staffRole = await dbContext.Roles.FirstAsync(r => r.Name == "nhanvien");

                var users = new List<UserAccount>
                {
                   new()
                   {
                       Username = "admin",
                       PasswordHash = passwordHash,
                       PasswordSalt = passwordSalt,
                       RoleId = (await dbContext.Roles.FirstAsync(r => r.Name == "admin")).Id,
                       FirstName = "Quản trị",
                       LastName = "Hệ Thống",
                       Email = "admin@store.com",
                       Phone = "0901112223"
                   },
                   new()
                   {
                       Username = "khachhang",
                       PasswordHash = passwordHash,
                       PasswordSalt = passwordSalt,
                       RoleId =(await dbContext.Roles.FirstAsync(r => r.Name == "user")).Id,
                       FirstName = "Khách",
                       LastName = "Hàng",
                       Email = "customer@store.com",
                       Phone = "0903334445",

                   }, new()
                    {
                        Username = "guest",
                        FirstName = "Khách vãng lai",
                        LastName = string.Empty,
                        Email = "guest@test.com",
                        Phone = "0000000000",
                        PasswordSalt = passwordSalt,
                        PasswordHash = passwordHash,
                        RoleId = (await dbContext.Roles.FirstAsync(r => r.Name == "user")).Id,
                    },
                   new()
                   {
                       Username = "nhanvien1",
                       PasswordHash = passwordHash,
                       PasswordSalt = passwordSalt,
                        RoleId = (await dbContext.Roles.FirstAsync(r => r.Name == "nhanvien")).Id,
                       FirstName = "Nhân",
                       LastName = "Viên 1",
                       Email = "staff1@store.com",
                       Phone = "0905556667",
                   }
                };

                await dbContext.UserAccounts.AddRangeAsync(users);
                await dbContext.SaveChangesAsync();

                // Tạo hồ sơ nhân viên
                var staffAccount = users.First(u => u.Username == "nhanvien1");
                var employee = new Employee
                {
                    LastName = "Nguyễn",
                    Gender = 1,
                    BirthYear = new DateTime(1995, 5, 15),
                    Phone = staffAccount.Phone,
                    Hometown = "TP.HCM",
                    UserAccountId = staffAccount.Id
                };

                await dbContext.Employees.AddAsync(employee);
                await dbContext.SaveChangesAsync();
            }

        }

        private static async Task SeedAddresses(this DataContext dbContext)
        {
            if (!await dbContext.Addresses.AnyAsync())
            {
                var admin = await dbContext.UserAccounts.FirstAsync(u => u.Username == "admin");
                var customer = await dbContext.UserAccounts.FirstAsync(u => u.Username == "khachhang");
                var staff = await dbContext.UserAccounts.FirstAsync(u => u.Username == "nhanvien1");

                var addresses = new List<Address>
        {
                      new()
                      {
                          UserAccountId = 1,
                          Name = "Trụ sở chính",
                          Street = "123 Đường Lê Lợi",
                          City = "Quận 1",
                          State = "TP.HCM",
                          Phone = admin.Phone,
                          Create = DateTime.Now
                      },
                      new()
                      {
                          UserAccountId = 2,
                          Name = "Nhà riêng",
                          Street = "45 Đường Nguyễn Huệ",
                          City = "Quận 1",
                          State = "TP.HCM",
                          Phone = customer.Phone,
                          Create = DateTime.Now
                      },new ()
                       {
                           UserAccountId = 3,
                           Name = string.Empty,
                           Phone = string.Empty,
                           Street = string.Empty,
                           City = string.Empty,
                           State = "Mua hàng tại quầy",
                           IsDeleted = true,
                           Create = DateTime.Now,
                           Update = DateTime.Now
                       },
                      new()
                      {
                          UserAccountId = 4,
                          Name = "Chi nhánh Q3",
                          Street = "78 Đường Võ Văn Tần",
                          City = "Quận 3",
                          State = "TP.HCM",
                          Phone = staff.Phone,
                          Create = DateTime.Now
                      }
                };

                await dbContext.Addresses.AddRangeAsync(addresses);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}
