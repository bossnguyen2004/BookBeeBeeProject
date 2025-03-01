namespace Fe_Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

            builder.Services.AddHttpClient("BackendApi", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });
            builder.Services.AddSession();

            builder.Services.AddDistributedMemoryCache(); 
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; 
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.Use(async (context, next) =>
            {
                if (context.Session.GetString("AccessToken") == null &&
                    context.Request.Cookies.TryGetValue("AccessToken", out var token))
                {
                    // ? L?y token t? Cookie v? l?u v?o Session
                    context.Session.SetString("AccessToken", token);
                    context.Session.SetString("UserRole", context.Request.Cookies["UserRole"]);
                }

                await next();
            });






            app.UseAuthorization();

            app.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
