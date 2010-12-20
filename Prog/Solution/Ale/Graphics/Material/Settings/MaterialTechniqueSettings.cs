//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

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
