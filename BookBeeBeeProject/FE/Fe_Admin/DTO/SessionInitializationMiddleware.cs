namespace Fe_Admin.DTO
{
    public class SessionInitializationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionInitializationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Nếu session chưa có AccessToken, kiểm tra trong cookie
            if (string.IsNullOrEmpty(context.Session.GetString("AccessToken")))
            {
                var cookieToken = context.Request.Cookies["AccessToken"];
                var cookieRole = context.Request.Cookies["UserRole"];
                var cookieUserId = context.Request.Cookies["UserAccountId"]; // 🔥 Thêm dòng này

                if (!string.IsNullOrEmpty(cookieToken) && !string.IsNullOrEmpty(cookieRole) && !string.IsNullOrEmpty(cookieUserId))
                {
                    context.Session.SetString("AccessToken", cookieToken);
                    context.Session.SetString("UserRole", cookieRole);
                    context.Session.SetString("UserAccountId", cookieUserId); // 🔥 Thêm dòng này
                }
            }

            await _next(context);
        }
    }
}
