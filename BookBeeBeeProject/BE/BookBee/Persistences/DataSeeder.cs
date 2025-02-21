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
            if (!dbContext.Roles.Any())
            {
                await dbContext.Roles.AddRangeAsync(new Role
                {
                    Name = "admin"
                }, new Role
                {
                    Name = "user"
                }, new Role
                {
                    Name = "nhanvien"
                }

                );
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedUsers(this DataContext dbContext)
        {
            if (!dbContext.UserAccounts.Any())
            {
                const string defaultPassword = "1234@Abcd";
                PasswordHelper.CreatePasswordHash(defaultPassword, out var passwordHash, out var passwordSalt);

                var adminRoleId = dbContext.Roles.First(r => r.Name == "admin").Id;
                var userRoleId = dbContext.Roles.First(r => r.Name == "user").Id;
                var staffRoleId = dbContext.Roles.First(r => r.Name == "nhanvien").Id;

                var adminAccount = new UserAccount
                {
                    Username = "admin",
                    PasswordSalt = passwordSalt,
                    PasswordHash = passwordHash,
                    RoleId = adminRoleId,
                    IsVerified = true
                };

                var userAccount = new UserAccount
                {
                    Username = "user",
                    PasswordSalt = passwordSalt,
                    PasswordHash = passwordHash,
                    RoleId = userRoleId,
                    IsVerified = true
                };

                var guestAccount = new UserAccount
                {
                    Username = "guest",
                    PasswordSalt = passwordSalt,
                    PasswordHash = passwordHash,
                    RoleId = userRoleId,
                    IsVerified = true
                };

                var staffAccount = new UserAccount
                {
                    Username = "nhanvien",
                    PasswordSalt = passwordSalt,
                    PasswordHash = passwordHash,
                    RoleId = staffRoleId,
                    IsVerified = true
                };

                await dbContext.UserAccounts.AddRangeAsync(adminAccount, userAccount, guestAccount, staffAccount);
                await dbContext.SaveChangesAsync();

                // Tạo UserProfile tương ứng
                var adminProfile = new UserProfile
                {
                    FirstName = "Admin",
                    LastName = "",
                    Email = "admin@test.com",
                    Phone = "0123456789",
                    UserAccountId = adminAccount.Id
                };

                var userProfile = new UserProfile
                {
                    FirstName = "User",
                    LastName = "",
                    Email = "user@test.com",
                    Phone = "0123456789",
                    UserAccountId = userAccount.Id
                };

                var guestProfile = new UserProfile
                {
                    FirstName = "Khách",
                    LastName = "Vãng lai",
                    Email = "guest@test.com",
                    Phone = "0123456789",
                    UserAccountId = guestAccount.Id
                };

                var staffProfile = new UserProfile
                {
                    FirstName = "Nhân",
                    LastName = "Viên",
                    Email = "nhanvien@test.com",
                    Phone = "0123456789",
                    UserAccountId = staffAccount.Id
                };

                await dbContext.UserProfiles.AddRangeAsync(adminProfile, userProfile, guestProfile, staffProfile);
                await dbContext.SaveChangesAsync();

                // Tạo Employee tương ứng với staff
                var employee = new Employee
                {
                    LastName = "Nhân viên",
                    Gender = 1, // 1: Nam, 0: Nữ
                    BirthYear = new DateTime(1990, 1, 1),
                    Phone = "0987654321",
                    Hometown = "Hà Nội",
                    UserAccountId = staffAccount.Id
                };

                await dbContext.Employees.AddAsync(employee);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedAddresses(this DataContext dbContext)
        {
            if (!dbContext.Addresses.Any())
            {
                await dbContext.Addresses.AddRangeAsync(
                    new Address
                    {
                        UserProfileId = 1, // Admin
                        Name = "Admin",
                        Phone = "0123456789",
                        Street = "Admin Street",
                        City = "Admin City",
                        State = "Admin State",
                        Create = DateTime.Now,
                        Update = DateTime.Now
                    },
                    new Address
                    {
                        UserProfileId = 2, // User
                        Name = "User",
                        Phone = "0123456789",
                        Street = "User Street",
                        City = "User City",
                        State = "User State",
                        Create = DateTime.Now,
                        Update = DateTime.Now
                    },
                    new Address
                    {
                        UserProfileId = 3, // Khách vãng lai (ẩn danh)
                        Name = string.Empty,
                        Phone = string.Empty,
                        Street = string.Empty,
                        City = string.Empty,
                        State = "Mua hàng tại quầy",
                        IsDeleted = true,
                        Create = DateTime.Now,
                        Update = DateTime.Now
                    },
                    new Address
                    {
                        UserProfileId = 4, // Nhân viên
                        Name = "NhanVien",
                        Phone = "0987654321",
                        Street = "Đường Số 1",
                        City = "Thành phố A",
                        State = "Tỉnh B",
                        Create = DateTime.Now,
                        Update = DateTime.Now
                    }
                );

                await dbContext.SaveChangesAsync();
            }
        }

    }
}
