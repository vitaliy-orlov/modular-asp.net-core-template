using System;

namespace Core
{
    /// <summary>
    /// Specifies title for navigation panel of website
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuTitleAttribute : Attribute
    {
        public string Name { get; }
        public MenuTitleAttribute(string name)
        {
            Name = name;
        }
    }
}
