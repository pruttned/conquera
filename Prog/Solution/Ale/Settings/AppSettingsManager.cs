using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Ale.Settings;
using System.Runtime.Serialization;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;

namespace Ale.Settings
{
    /// <summary>
    /// Settings manager
    /// </summary>
    public class AppSettingsManager
    {
        public delegate void CommittedHandler(IAppSettings settings);

        /// <summary>
        /// Executed whenever has been app settings commited
        /// </summary>
        public event CommittedHandler AppSettingsCommitted;

        private static AppSettingsManager mInstance;
        private Dictionary<Type, IAppSettings> mSettings = new Dictionary<Type, IAppSettings>();
        private string mSettingsDirectory;

        public static AppSettingsManager Default
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = new AppSettingsManager();
                }
                return mInstance;
            }
        }

        public AppSettingsManager(string settingsDirectory)
        {
            mSettingsDirectory = settingsDirectory;
            if (!Path.IsPathRooted(mSettingsDirectory))
            {
                mSettingsDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), mSettingsDirectory);
            }
        }

        /// <summary>
        /// Get settings of a given type (even if they where not committed jet)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSettings<T>() where T : class, IAppSettings
        {
            return (T)GetSettings(typeof(T));
        }

        /// <summary>
        /// Get settings of a given type, but only if they where already committed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        public bool TryGetSettings<T>(out T appSettings) where T : class, IAppSettings
        {
            IAppSettings appSettings2;
            if (TryGetSettings(typeof(T), out appSettings2))
            {
                appSettings = (T)appSettings2;
                return true;
            }
            appSettings = null;
            return false;
        }

        /// <summary>
        /// Get settings of a given type (even if they where not committed yet)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IAppSettings GetSettings(Type type)
        {
            IAppSettings appSettings;
            if (!TryGetSettings(type, out appSettings))
            {
                appSettings = (IAppSettings)Activator.CreateInstance(type, true);
                CommitSettings(appSettings, false, false);
            }
            return (IAppSettings)appSettings.Clone();
        }

        /// <summary>
        /// Get settings of a given type, but only if they where already commited
        /// </summary>
        /// <param name="type"></param>
        /// <param name="appSettings"></param>
        /// <returns></returns>
        public bool TryGetSettings(Type type, out IAppSettings appSettings)
        {
            //check cache
            IAppSettings settings;
            if (!mSettings.TryGetValue(type, out settings))
            {
                //try to load from file
                settings = LoadSettings(type);
                if (null == settings)
                {
                    appSettings = null;
                    return false;
                }
                mSettings.Add(type, settings);
            }

            appSettings = (IAppSettings)settings.Clone();
            return true;
        }

        /// <summary>
        /// Commit settings to file
        /// </summary>
        /// <param name="appSettings"></param>
        public void CommitSettings(IAppSettings appSettings)
        {
            CommitSettings(appSettings, true, true);
        }

        private AppSettingsManager()
        {
            mSettingsDirectory = MainSettings.Instance.UserDir;
        }

        /// <summary>
        /// Commit settings to file
        /// </summary>
        /// <param name="appSettings"></param>
        private void CommitSettings(IAppSettings appSettings, bool fireEvent, bool throwOnReadonly)
        {
            Type type = appSettings.GetType();
            string iniFile;
            string sectionName;
            bool readonlySettings;
            GetSettingsInfo(type, out iniFile, out sectionName, out readonlySettings);

            if (throwOnReadonly && readonlySettings)
            {
                throw new InvalidOperationException("App settings are readonly and can't be commited");
            }

            appSettings = (IAppSettings)((ICloneable)appSettings).Clone();

            IniFileDocument doc = new IniFileDocument();
            if (File.Exists(iniFile))
            {
                doc.Load(iniFile);
            }

            //section
            IniFileSectionNode section;
            if (!doc.Sections.TryGetNode(sectionName, out section))
            {
                section = doc.Sections.Add(sectionName);

                //comments - only if section doesn't exists
                object[] appSettingsCommentAttributes = type.GetCustomAttributes(typeof(AppSettingsCommentAttribute), false);
                if (null != appSettingsCommentAttributes)
                {
                    foreach (AppSettingsCommentAttribute appSettingsCommentAttribute in appSettingsCommentAttributes)
                    {
                        section.Comments.Add(appSettingsCommentAttribute.Value);
                    }
                }
            }

            //params
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                object[] appSettingsParamAttributes = property.GetCustomAttributes(typeof(AppSettingsParamAttribute), false);
                if (null != appSettingsParamAttributes && 0 != appSettingsParamAttributes.Length)
                {
                    AppSettingsParamAttribute appSettingsParamAttribute = (AppSettingsParamAttribute)appSettingsParamAttributes[0];
                    string paramName = !string.IsNullOrEmpty(appSettingsParamAttribute.Name) ? appSettingsParamAttribute.Name : property.Name;
                    IniFileParameterNode param;
                    if (!section.Parameters.TryGetNode(paramName, out param))
                    {
                        param = section.Parameters.Add(paramName, null);

                        //comments - only if param doesn't exists
                        object[] appSettingsCommentAttributes = property.GetCustomAttributes(typeof(AppSettingsCommentAttribute), false);
                        if (null != appSettingsCommentAttributes)
                        {
                            foreach (AppSettingsCommentAttribute appSettingsCommentAttribute in appSettingsCommentAttributes)
                            {
                                param.Comments.Add(appSettingsCommentAttribute.Value);
                            }
                        }
                    }

                    Type propertyType = property.PropertyType;
                    var converter = TypeDescriptor.GetConverter(propertyType);
                    param.Value = converter.ConvertToString(null, CultureInfo.InvariantCulture, property.GetValue(appSettings, null));
                }
            }


            //ensure dir
            string filePathDir = Path.GetDirectoryName(iniFile);
            if (!Directory.Exists(filePathDir))
            {
                Directory.CreateDirectory(filePathDir);
            }

            doc.Save(iniFile);

            mSettings[type] = appSettings;

            if (fireEvent && null != AppSettingsCommitted)
            {
                AppSettingsCommitted.Invoke(appSettings);
            }
        }

        private void GetSettingsInfo(Type type, out string iniFile, out string sectionName, out bool readonlySettings)
        {
            object[] appSettingsAttributes = type.GetCustomAttributes(typeof(AppSettingsAttribute), false);
            if (null == appSettingsAttributes || 0 == appSettingsAttributes.Length)
            {
                throw new ArgumentException(string.Format("Type '{0}' is missing requiered attribute {1}", type.FullName, typeof(AppSettingsAttribute).Name));
            }
            AppSettingsAttribute appSettingsAttribute = (AppSettingsAttribute)appSettingsAttributes[0];

            iniFile = Path.Combine(mSettingsDirectory, appSettingsAttribute.FileName);
            sectionName = appSettingsAttribute.SectionName ?? type.Name;
            readonlySettings = appSettingsAttribute.Readonly;
        }

        private IAppSettings LoadSettings(Type type)
        {
            string iniFile;
            string sectionName;
            bool readonlySettings;
            GetSettingsInfo(type, out iniFile, out sectionName, out readonlySettings);

            if (File.Exists(iniFile))
            {
                IniFileDocumentLoadSettings loadSettings = new IniFileDocumentLoadSettings()
                {
                    IncludeComments = false,
                    SectionNameFilter = string.Format("^{0}$", sectionName)
                };
                IniFileDocument doc = new IniFileDocument();
                doc.Load(iniFile, loadSettings);

                if (0 == doc.Sections.Count)
                {
                    return null;
                }
                IniFileSectionNode section = doc.Sections[0];

                IAppSettings appSettings = (IAppSettings)Activator.CreateInstance(type, true);

                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    object[] appSettingsParamAttributes = property.GetCustomAttributes(typeof(AppSettingsParamAttribute), false);
                    if (null != appSettingsParamAttributes && 0 != appSettingsParamAttributes.Length) //load property
                    {
                        AppSettingsParamAttribute appSettingsParamAttribute = (AppSettingsParamAttribute)appSettingsParamAttributes[0];
                        string paramName = !string.IsNullOrEmpty(appSettingsParamAttribute.Name) ? appSettingsParamAttribute.Name : property.Name;
                        IniFileParameterNode param;
                        if (section.Parameters.TryGetNode(paramName, out param))
                        {
                            Type propertyType = property.PropertyType;
                            var converter = TypeDescriptor.GetConverter(propertyType);
                            object value = converter.ConvertFromString(null, CultureInfo.InvariantCulture, param.Value);
                            property.SetValue(appSettings, value, null);
                        }
                    }
                }

                return appSettings;
            }
            return null;
        }
    }
}
