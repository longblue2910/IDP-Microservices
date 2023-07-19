namespace RimMicroservices.IDP.Extensions
{
    public static class CookiePolicyExtensions
    {
        public static void ConfigureCookiePolicy(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = (SameSiteMode)(-1);
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });
        }

        static void CheckSameSite(HttpContext htppContext, CookieOptions options)
        {
            if (options.SameSite != SameSiteMode.None && options.SameSite != SameSiteMode.Unspecified) return;
            var userAgent = htppContext.Request.Headers["User-Agent"].ToString();

            if (DisallowsSameSiteNone(userAgent))
            {
                options.SameSite = (SameSiteMode)(-1);
            }
        }

        static bool DisallowsSameSiteNone(string userAgent)
        {
            if(userAgent.Contains("CPU iPhone OS 12") ||
                userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            if (userAgent.Contains("Chrome")) return true;

            return false;
        }
    }
}
