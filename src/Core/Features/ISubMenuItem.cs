using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Features
{
    public interface ISubMenuItem
    {
        string Id { get; }
        string Action { get; }
        string Controller { get; }
        string Area { get; }
        string Title { get; }
    }
}
