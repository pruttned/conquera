using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class AppSettingsParamAttribute : Attribute
    {
        /// <summary>
        /// Name of the param in the ini file.
        /// If null, then the name of the property is used
        /// </summary>
        public string Name { get; set; }

        public AppSettingsParamAttribute()
        {
        }
    }
}
