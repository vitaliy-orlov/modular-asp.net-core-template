using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents a common information about controller
    /// </summary>
    public class ControllerInfo
    {
        /// <summary>
        /// Area name
        /// </summary>
        public string area;
        /// <summary>
        /// Controller class name (without 'Controller')
        /// </summary>
        public string controller;

        public ControllerInfo(string area, string controller)
        {
            this.area = area;
            this.controller = controller;
        }
    }
}
