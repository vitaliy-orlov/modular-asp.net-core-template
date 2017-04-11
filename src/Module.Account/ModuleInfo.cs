using System.Collections.Generic;
using System.Reflection;
using Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Features;

namespace Module.Account
{
    public class ModuleInfo : IModuleBase
    {
        public const string AREA_NAME = "Account";

        private Assembly _info;
        public Assembly Assembly { get { return _info; } }
        public IEnumerable<ControllerInfo> Controllers { get; set; }
        public IFeatureCollection Features { get; set; }
        public string AreaName { get { return AREA_NAME; } }

        public ModuleInfo(Assembly assembly)
        {
            _info = assembly;

            Features = new FeatureCollection();

            Features.Set<Core.Features.ISideBarPanelFeature>(new Core.Features.SideBarPanelFeature("accounts", "/Account/", "Accounts", "fa-users", null));

            //example of module with several controllers
            //Features.Set<Core.Features.ISideBarPanelFeature>(new Core.Features.SideBarPanelFeature("catalog", "", "Catalog", "fa-table", new List<Core.Features.ISubMenuItem>()
            //{
            //    new Core.Features.SubMenuItem("good", "Index", "Good", AREA_NAME, "Goods"),
            //    new Core.Features.SubMenuItem("group", "Index", "Group", AREA_NAME, "Groups"),
            //}));
        }

        public void InitServices(IServiceCollection services, IConfigurationRoot configuration)
        {

        }

        public void InitConfigs(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IConfigurationRoot configuration)
        {
            
        }
    }
}
