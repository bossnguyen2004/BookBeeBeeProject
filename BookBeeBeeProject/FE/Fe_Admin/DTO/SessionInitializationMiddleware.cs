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
			// Kiểm tra session có khả dụng không
			if (context.Session == null || !context.Session.IsAvailable)
			{
				Console.WriteLine("⚠ Session is not available!");
			}
			else
			{
				Console.WriteLine("✅ Session is available.");
			}

			// Nếu session chưa có AccessToken, kiểm tra trong cookie
			if (context.Session != null && string.IsNullOrEmpty(context.Session.GetString("AccessToken")))
			{
				var cookieToken = context.Request.Cookies["AccessToken"];
				var cookieRole = context.Request.Cookies["UserRole"];
				var cookieUserId = context.Request.Cookies["UserAccountId"];

				if (!string.IsNullOrEmpty(cookieToken) && !string.IsNullOrEmpty(cookieRole) && !string.IsNullOrEmpty(cookieUserId))
				{
					context.Session.SetString("AccessToken", cookieToken);
					context.Session.SetString("UserRole", cookieRole);
					context.Session.SetString("UserAccountId", cookieUserId);
				}
			}

			await _next(context);
		}
	}
}
