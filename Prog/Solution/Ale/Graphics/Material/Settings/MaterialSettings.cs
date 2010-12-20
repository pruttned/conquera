//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
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
    /// Desc of the material
    /// </summary>
    [DataObject(MaxCachedCnt=5)]
    public class MaterialSettings : BaseDataObject
    {
        private string mName;
        private string mEffectName;
        private int mRenderLayer; 
        private List<MaterialParamSettings> mParams;
        private List<MaterialTechniqueSettings> mTechniques;

        /// <summary>
        /// Name of the material
        /// </summary>
        [DataProperty(CaseSensitive=false, Unique=true, NotNull=true)]
        public string Name
        {
            get { return mName; }
            set 
            {
                if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException("value"); }
                mName = value; 
            }
        }

        /// <summary>
        /// Name of the effect on which is this material based
        /// </summary>
        [DataProperty(NotNull = true)]
        public string EffectName
        {
            get { return mEffectName; }
            set 
            { 
                mEffectName = value;
                if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException("value"); }
            }
        }

        /// <summary>
        /// Render layer on which is a renderable unit rendered
        /// Layers are render first from greater to lower for opaque objects and then from lower to
        /// greater for transparent objects. Lower layer number means that is is further from the camera.
        /// </summary>
        [DataProperty(NotNull = true)]
        public int RenderLayer
        {
            get { return mRenderLayer; }
            set
            {
                mRenderLayer = value;
            }
        }

        [DataListProperty(NotNull = true)]
        public List<MaterialParamSettings> Params
        {
            get { return mParams; }
            private set { mParams = value; }
        }

        [DataListProperty(NotNull = true)]
        public List<MaterialTechniqueSettings> Techniques
        {
            get { return mTechniques; }
            private set { mTechniques = value; }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="effectName">- Name of the effect on which is this material based</param>
        /// <param name="renderLayer">- Render layer on which is a renderable unit rendered</param>
        public MaterialSettings(string name, string effectName, int renderLayer)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }
            if (string.IsNullOrEmpty(effectName)) { throw new ArgumentNullException("effectName"); }

            mName = name;
            mEffectName = effectName;
            mRenderLayer = renderLayer;
            mParams = new List<MaterialParamSettings>();
            mTechniques = new List<MaterialTechniqueSettings>();
        }

        /// <summary>
        /// ctor (renderLayer = DefaultRenderLayers.GroundStandingObjects)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="effectName">- Name of the effect on which is this material based</param>
        public MaterialSettings(string name, string effectName)
            : this(name, effectName, DefaultRenderLayers.GroundStandingObjects)
        {}

        /// <summary>
        /// For sof
        /// </summary>
        private MaterialSettings()
        {
        }
    }
}
