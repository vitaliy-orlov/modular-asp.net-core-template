using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Features
{
    public class SideBarPanelFeature : ISideBarPanelFeature
    {
        private string id;
        private string url;
        private string title;
        private string fAIcon;
        private ICollection<ISubMenuItem> subMenu;

        public string Id { get { return id; } }
        public string Url { get { return url; } }
        public string Title { get { return title; } }
        public string FAIcon { get { return fAIcon; } }
        public ICollection<ISubMenuItem> SubMenu { get { return subMenu; } }

        public SideBarPanelFeature(string id, string url, string title, string fAIcon, ICollection<ISubMenuItem> subMenu)
        {
            this.id = id;
            this.url = url;
            this.title = title;
            this.fAIcon = fAIcon;
            this.subMenu = subMenu;
        }
    }
}
