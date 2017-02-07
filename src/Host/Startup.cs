using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;

namespace Host
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string path = Configuration.GetValue<string>(Extensions.ModuleLoader.CONFIG_MODULES_PATH);

            // Load assemblies
            Extensions.ModuleLoader.LoadAssemblies(path);

            // Add custom locations for searching Views in modules
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Add("{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("{2}/Views/Shared/{0}.cshtml");
            });

            var mvc = services.AddMvc();

            // Load each module to MVC service
            foreach (var module in Extensions.ModuleLoader.ModulesList)
            {
                module.InitServices(services);

                mvc.AddApplicationPart(module.Assembly).AddRazorOptions(o => {
                    o.AdditionalCompilationReferences.Add(MetadataReference.CreateFromFile(module.Assembly.Location));
                    o.FileProviders.Add(new EmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name.Replace("." + module.AreaName, "")));
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Add modules assemblies for static files serving
            foreach (var module in Extensions.ModuleLoader.ModulesList)
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new EmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name + ".wwwroot"),
                    RequestPath = new PathString("/" + module.AreaName)
                });
            }

            app.UseMvc(routes =>
            {
                // Special route for handling Area request
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
