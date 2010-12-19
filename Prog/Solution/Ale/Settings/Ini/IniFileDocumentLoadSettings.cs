using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Settings
{
    public class IniFileDocumentLoadSettings
    {
        /// <summary>
        /// Default = true
        /// </summary>
        public bool IncludeComments { get; set; }

        /// <summary>
        /// Regex for section names that should be loaded. 
        /// Nullable
        /// Default = null
        /// </summary>
        public string SectionNameFilter { get; set; }

        public IniFileDocumentLoadSettings()
        {
            IncludeComments = true;
            SectionNameFilter = null;
        }
    }

}
