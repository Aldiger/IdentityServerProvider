using Microsoft.AspNetCore.Rewrite;
using IdentityServerProvider.Api.Helpers;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net;

namespace IdentityServerProvider.Api;

public static class ConfigureApplication
{
    public static void UseServer(
      this IApplicationBuilder app,
      IConfiguration configuration,
      string environmentName
    )
    {
        //if (Helpers.Constants.IsDevelopmentEnvironment(environmentName))
        //{
        app.UseCors(builder =>
        {
            var allowCors = configuration.GetValue("AllowCors", "");
            if (!(string.IsNullOrEmpty(allowCors) || allowCors == "*"))
                builder.WithOrigins(allowCors);
            else
                builder.SetIsOriginAllowed(x => true);


            builder.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });

        //if (!Helpers.Constants.IsTestingEnvironment(environmentName))
        //{
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server API V1");
            c.DocExpansion(DocExpansion.None);
        });
        //}

        app.UseDeveloperExceptionPage();
        //}
        //else
        //{
        //  app.UseExceptionHandler("/Error");
        //}


        app.UseDefaultFiles();
        app.UseStaticFiles();


        app.UseWhen(context => MiddlewareExtension.ApplyQueryStringDecode(context),
            appBuilder => MiddlewareExtension.DecodeRequestQueryString(app));


        app.UseStatusCodePagesWithReExecute("/error");

        app.UseRequestLocalization();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(builder =>
        {
            builder.MapControllers();
            builder.MapRazorPages();
            builder.MapDefaultControllerRoute();
        });
    }
}
