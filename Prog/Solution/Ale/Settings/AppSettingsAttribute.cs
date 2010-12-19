using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    /// <summary>
    /// Defines AppSettings
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppSettingsAttribute : Attribute
    {
        /// <summary>
        /// Name of the file that contains the settings.
        /// It is possible to contaion multiple AppSettings in one file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Name of the section in the ini file.
        /// If null, then the class name is used
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// Can't be commited.
        /// Default = false
        /// </summary>
        public bool Readonly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">- Name of the file that contains the settings. It is possible to contaion multiple AppSettings in one file. </param>
        public AppSettingsAttribute(string fileName)
        {
            FileName = fileName;
            Readonly = false;
        }
    }
}
