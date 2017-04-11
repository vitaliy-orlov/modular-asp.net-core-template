using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Features
{
    public class SubMenuItem : ISubMenuItem
    {
        private string id;
        private string action;
        private string controller;
        private string area;
        private string title;

        public string Id { get { return id; } }
        public string Action { get { return action; } }
        public string Controller { get { return controller; } }
        public string Area { get { return area; } }
        public string Title { get { return title; } }

        public SubMenuItem(string id, string action, string controller, string area, string title)
        {
            this.id = id;
            this.action = action;
            this.controller = controller;
            this.area = area;
            this.title = title;
        }
    }
}
