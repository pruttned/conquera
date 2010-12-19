using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOrmFramework;

namespace Ale.Graphics
{
    /// <summary>
    /// Desc of the material pass
    /// </summary>
    [DataObject]
    public class MaterialPassSettings : BaseDataObject
    {
        private string mName;
        private List<MaterialParamSettings> mParams;

        /// <summary>
        /// Name of the pass in effect file 
        /// </summary>
        [DataProperty(NotNull = true)]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        [DataListProperty(NotNull = true)]
        public List<MaterialParamSettings> Params
        {
            get { return mParams; }
            private set { mParams = value; }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        public MaterialPassSettings(string name)
        {
            mName = name;
            mParams = new List<MaterialParamSettings>();
        }

        /// <summary>
        /// For sof
        /// </summary>
        private MaterialPassSettings()
        {
        }
    }
}
