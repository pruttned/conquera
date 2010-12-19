using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    /// <summary>
    /// Defines comment that should be stored in a ini file on a item
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class AppSettingsCommentAttribute : Attribute
    {
        public string Value { get; private set; }

        public AppSettingsCommentAttribute(string value)
        {
            Value = value;
        }
    }
}
