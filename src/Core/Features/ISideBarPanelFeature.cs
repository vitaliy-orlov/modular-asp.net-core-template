using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Features
{
    public interface ISideBarPanelFeature
    {
        string Id { get; }
        string Url { get; }
        string Title { get; }
        string FAIcon { get; }
        ICollection<ISubMenuItem> SubMenu { get; }
    }
}
