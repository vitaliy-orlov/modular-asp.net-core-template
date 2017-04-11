using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.AspNetCore.Http.Features;

namespace Core
{
    /// <summary>
    /// Represents a base properties and methods for module
    /// </summary>
    public interface IModuleBase
    {
        /// <summary>
        /// Gets an assembly of module.
        /// </summary>
        Assembly Assembly { get; }
        /// <summary>
        /// Gets a name of Area for controllers.
        /// </summary>
        string AreaName { get; }
        /// <summary>
        /// Gets or sets a list of controllers to generate titles of submenu for navigation panel.
        /// </summary>
        IEnumerable<ControllerInfo> Controllers { get; set; }
        /// <summary>
        /// Gets or sets a list of features
        /// </summary>
        IFeatureCollection Features { get; set; }
        /// <summary>
        /// Add module services to the container.
        /// </summary>
        /// <param name="services"></param>
        void InitServices(IServiceCollection services, IConfigurationRoot configuration);
        /// <summary>
        /// Configure the HTTP request pipeline by module.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        void InitConfigs(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IConfigurationRoot configuration);
    }
}
