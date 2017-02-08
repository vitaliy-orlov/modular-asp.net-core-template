using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;

namespace Core
{
    public static class CoreMvcModulesExtensions
    {
        public static IApplicationBuilder UseMvcModulesStaticFiles(this IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IConfigurationRoot configuration, string staticCommonPrefix = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (CoreMvcBuilderExtensions.ModulesList == null)
                return app;

            bool useCustomPrefix = !string.IsNullOrEmpty(staticCommonPrefix);

            foreach (var module in CoreMvcBuilderExtensions.ModulesList)
            {
                module.InitConfigs(app, env, loggerFactory, configuration);

                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new EmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name + ".wwwroot"),
                    RequestPath = new PathString("/" + (useCustomPrefix ? staticCommonPrefix : module.AreaName))
                });
            }

            return app;
        }

        public static IRouteBuilder UseMvcModulesRoute(this IRouteBuilder routes)
        {
            if (routes == null)
            {
                throw new ArgumentNullException(nameof(routes));
            }

            return routes.MapRoute(name: "areaRoute", template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        }
    }
}
