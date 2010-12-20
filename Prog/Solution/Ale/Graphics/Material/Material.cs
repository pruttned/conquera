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
using SimpleOrmFramework;
using Ale.Content;
using System.Collections.Generic;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Each material is binded to exactly one effect. It is responsible for defining values 
    /// of effect's manual parameters.
    /// </summary>
    [NonContentPipelineAsset(typeof(MaterialLoader))]
    public class Material
    {
        #region Fields

        private string mName;

        /// <summary>
        /// Effect to which is this material binded
        /// </summary>
        private MaterialEffect mMaterialEffect;

        /// <summary>
        /// Techniques of this material
        /// </summary>
        private MaterialTechniqueCollection mTechniques;

        /// <summary>
        /// Default technique of this material
        /// </summary>
        private MaterialTechnique mDefaultTechnique;

        /// <summary>
        /// Render layer on which is a renderable unit rendered
        /// Layers are render first from greater to lower for opaque objects and then from lower to
        /// greater for transparent objects. Lower layer number means that is is further from the camera.
        /// </summary>
        private int mRenderLayer;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets the effect to which is this material binded
        /// </summary>
        public MaterialEffect MaterialEffect
        {
            get { return mMaterialEffect; }
        }

        /// <summary>
        /// Gets/Sets the render layer on which is a renderable unit rendered
        /// Layers are render first from greater to lower for opaque objects and then from lower to
        /// greater for transparent objects. Lower layer number means that is is further from the camera.
        /// </summary>
        public int RenderLayer
        {
            get { return mRenderLayer; }
            set { mRenderLayer = value; }
        }

        /// <summary>
        /// Gets all techniques of this material
        /// </summary>
        public MaterialTechniqueCollection Techniques
        {
            get { return mTechniques; }
        }

        /// <summary>
        /// Gets the default technique of this material
        /// </summary>
        public MaterialTechnique DefaultTechnique
        {
            get { return mDefaultTechnique; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="materialDesc"></param>
        /// <param name="contentGroup">- Content group that should be used to load effects and textures referenced in material</param>
        public Material(MaterialSettings settings, ContentGroup contentGroup)
        {
            mName = settings.Name;
            mMaterialEffect = contentGroup.Load<MaterialEffect>(settings.EffectName);
            mRenderLayer = settings.RenderLayer;

            //load root material params
            HashSet<MaterialParam> paramsFromMaterial = new HashSet<MaterialParam>(MaterialParamByNameEqualityComparer.Instance);
            foreach (MaterialParamSettings paramDesc in settings.Params)
            {
                paramsFromMaterial.Add(paramDesc.CreateMaterialParam(mMaterialEffect, contentGroup));
            }

            //load techniques
            Dictionary<NameId, MaterialTechnique> techniques = new Dictionary<NameId,MaterialTechnique>();
            foreach (MaterialTechniqueSettings techniqueSettings in settings.Techniques)
            {
                MaterialTechnique technique = new MaterialTechnique(techniqueSettings, contentGroup, mMaterialEffect, paramsFromMaterial);
                techniques.Add(technique.ScenePass, technique);
            }

            //load rest of the techniques without settings
            foreach (MaterialEffectTechnique effectTechnique in mMaterialEffect.Techniques.Values)
            {
                if (!techniques.ContainsKey(effectTechnique.ScenePass))
                {
                    MaterialTechnique technique = new MaterialTechnique(effectTechnique, mMaterialEffect, paramsFromMaterial);
                    techniques.Add(technique.ScenePass, technique);
                }
            }
            
            mTechniques = new MaterialTechniqueCollection(techniques);

            //default technique
            mDefaultTechnique = mTechniques["Default"]; //now it will throw if it isn't found
            //if (null == mDefaultTechnique) //Default technique is required
            //{
            //    throw new ArgumentException(string.Format("Unable to find material's requered technique with name 'Default'in effect '{1}'", settings.EffectName));
            //}
        }

        /// <summary>
        /// Ctor  - without settings
        /// </summary>
        /// <param name="materialEffect"></param>
        /// <param name="renderLayer"></param>
        public Material(MaterialEffect materialEffect, int renderLayer)
        {
            mName = null;
            mMaterialEffect = materialEffect;

            Dictionary<NameId, MaterialTechnique> techniques = new Dictionary<NameId, MaterialTechnique>();
            mTechniques = new MaterialTechniqueCollection(techniques);
            foreach (MaterialEffectTechnique effectTechnique in mMaterialEffect.Techniques.Values)
            {
                MaterialTechnique technique = new MaterialTechnique(effectTechnique, mMaterialEffect, null);
                techniques.Add(technique.ScenePass, technique);
            }

            //default technique
            mDefaultTechnique = mTechniques["Default"]; //now it will throw if it isn't found
            //if (null == mDefaultTechnique) //Default technique is required
            //{
            //    throw new ArgumentException("Unable to find material's requered technique with name 'Default'");
            //}
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mName;
        }

        #endregion Methods
    }
}
