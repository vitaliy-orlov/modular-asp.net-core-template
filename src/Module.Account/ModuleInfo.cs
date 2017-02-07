using System.Collections.Generic;
using System.Reflection;
using Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Module.Account
{
    public class ModuleInfo : IModuleBase
    {
        public const string AREA_NAME = "Account";

        private Assembly _info;
        public Assembly Assembly { get { return _info; } }
        public IEnumerable<ControllerInfo> Controllers { get; set; }
        public string MainTabTitle { get { return "Account"; } }
        public string AreaName { get { return AREA_NAME; } }

        public ModuleInfo(Assembly assembly)
        {
            _info = assembly;
        }

        public void InitServices(IServiceCollection services)
        {

        }

        public void InitConfigs(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
        }
    }
}
