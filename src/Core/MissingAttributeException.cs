using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents an error occurs when a required attribute is missing
    /// </summary>
    public class MissingAttributeException : Exception
    {
        public MissingAttributeException(string message) : base(message) { }
    }
}
