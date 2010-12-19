using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOrmFramework;

namespace Ale.Graphics
{
    /// <summary>
    /// Desc of the material technique
    /// </summary>
    [DataObject]
    public class MaterialTechniqueSettings : BaseDataObject
    {
        private string mName;
        private List<MaterialParamSettings> mParams;
        private List<MaterialPassSettings> mPasses;

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

        [DataListProperty(NotNull = true)]
        public List<MaterialPassSettings> Passes
        {
            get { return mPasses; }
            private set { mPasses = value; }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        public MaterialTechniqueSettings(string name)
        {
            mName = name;
            mParams = new List<MaterialParamSettings>();
            mPasses= new List<MaterialPassSettings>();
        }

        /// <summary>
        /// ctor (Name = 'Default')
        /// </summary>
        public MaterialTechniqueSettings()
            :this("Default")
        {
        }
    }
}
