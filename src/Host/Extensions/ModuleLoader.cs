using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc;

namespace Host.Extensions
{
    /// <summary>
    /// Represents an error occurs when a required attribute is missing
    /// </summary>
    public class MissingAttributeException : Exception
    {
        public MissingAttributeException(string message) : base(message) { }
    }

    /// <summary>
    /// Represents a helper for searching and parsing modules
    /// </summary>
    public static class ModuleLoader
    {
        public static readonly string CONFIG_MODULES_PATH = "ModulesPath";
        public static readonly string BASE_DIR_NAME_FOR_MODULES = "Modules";
        public static IEnumerable<IModuleBase> ModulesList { get; private set; }
        public static void LoadAssemblies(string path)
        {
            List<IModuleBase> modulesList = new List<IModuleBase>();

            if (string.IsNullOrEmpty(path))
                path = Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, BASE_DIR_NAME_FOR_MODULES);

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

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
                MenuTitleAttribute sub;

                foreach (Type controller in moduleAssembly.GetTypes().Where(x => typeof(Controller).IsAssignableFrom(x)))
                {
                    area = controller.GetTypeInfo().GetCustomAttribute<AreaAttribute>();
                    sub = controller.GetTypeInfo().GetCustomAttribute<MenuTitleAttribute>();

                    if (area == null)
                        throw new MissingAttributeException($"Missing '{nameof(AreaAttribute)}' for controller {controller.FullName}");

                    list.Add(new ControllerInfo(area.RouteValue, controller.Name.Replace("Controller", ""), sub?.Name));
                }

                IModuleBase module = (IModuleBase)Activator.CreateInstance(moduleType, moduleAssembly);
                
                module.Controllers = list.ToArray().AsEnumerable();

                modulesList.Add(module);
            }

            ModulesList = modulesList.ToArray().AsEnumerable();

            modulesList.Clear();
        }
    }
}
