using System.Net;

namespace IdentityServerProvider.Api.Helpers
{
    public static class MiddlewareExtension
    {
        public static void DecodeRequestQueryString(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Request.QueryString = new QueryString(WebUtility.UrlDecode(context.Request.QueryString.Value));

                // Do work that doesn't write to the Response.
                await next();
                // Do other work that doesn't write to the Response.
            });
        }
        
        public static bool ApplyQueryStringDecode(HttpContext context)
        {
            return context.Request.Path.Value.ToLower().Contains("/account/login") && context.Request.Method.ToLower() == "get";
        }
    }
}
