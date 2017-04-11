using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.FileProviders;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Core
{
    public static class CoreMvcBuilderExtensions
    {
        public static readonly string CONFIG_MODULES_PATH = "ModulesPath";
        public static readonly string BASE_DIR_NAME_FOR_MODULES = "Modules";
        public static IEnumerable<IModuleBase> ModulesList { get; private set; }

        public static IMvcBuilder AddMvcModules(this IMvcBuilder builder, IServiceCollection services, IConfigurationRoot configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            string path = configuration.GetValue<string>(CONFIG_MODULES_PATH);
            List<IModuleBase> modulesList = new List<IModuleBase>();

            if (string.IsNullOrEmpty(path))
                path = Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, BASE_DIR_NAME_FOR_MODULES);

            if (!Directory.Exists(path))
                return builder;

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (var file in di.GetFileSystemInfos("*.dll", SearchOption.TopDirectoryOnly))
            {
                Assembly moduleAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                Type moduleType = moduleAssembly.GetTypes().FirstOrDefault(x => typeof(IModuleBase).IsAssignableFrom(x));

                if (moduleType == null)
                    throw new MissingMemberException();

                if (moduleType.GetConstructors().FirstOrDefault(x =>
                        (x.Attributes & MethodAttributes.Public) != 0 &&
                        x.GetParameters().Length == 1 &&
                        x.GetParameters()[0].ParameterType == typeof(Assembly)) == null)
                    throw new MissingMethodException($"{moduleType.FullName} does not have a constructor with parameter of type '{nameof(Assembly)}'");


                List<ControllerInfo> list = new List<ControllerInfo>();
                AreaAttribute area;

                foreach (Type controller in moduleAssembly.GetTypes().Where(x => typeof(Controller).IsAssignableFrom(x)))
                {
                    area = controller.GetTypeInfo().GetCustomAttribute<AreaAttribute>();

                    if (area == null)
                        throw new MissingAttributeException($"Missing '{nameof(AreaAttribute)}' for controller {controller.FullName}");

                    list.Add(new ControllerInfo(area.RouteValue, controller.Name.Replace("Controller", "")));
                }

                IModuleBase module = (IModuleBase)Activator.CreateInstance(moduleType, moduleAssembly);

                module.Controllers = list.ToArray().AsEnumerable();

                module.InitServices(services, configuration);

                builder.AddApplicationPart(module.Assembly).AddRazorOptions(o => {
                    o.AdditionalCompilationReferences.Add(MetadataReference.CreateFromFile(module.Assembly.Location));
                    o.FileProviders.Add(new EmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name.Replace("." + module.AreaName, "")));
                });

                modulesList.Add(module);
            }

            ModulesList = modulesList.ToArray().AsEnumerable();

            modulesList.Clear();

            // Add custom locations for searching Views in modules
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Add("{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("{2}/Views/Shared/{0}.cshtml");
            });

            return builder;
        }
    }
}
