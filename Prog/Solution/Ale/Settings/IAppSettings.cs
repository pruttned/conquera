using System;
using System.Collections.Generic;
using System.Text;
using Ale.Tools;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Ale.Settings
{
    /// <summary>
    /// AppSettings. AppSettings must have also AppSettingsAttribute
    /// </summary>
    public interface IAppSettings : ICloneable
    {
    }
}
