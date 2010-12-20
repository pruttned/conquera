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


using Ale.Content;
using System.Collections.Generic;
using System;
using Ale.Tools;
namespace Ale.Graphics
{
    /// <summary>
    /// Material's technique
    /// </summary>
    public class MaterialTechnique
    {
        #region Fields

        /// <summary>
        /// Effect technique to which is this material technique binded
        /// </summary>
        private MaterialEffectTechnique mMaterialEffectTechnique;

        /// <summary>
        /// Passes of this technique
        /// </summary>
        private MaterialPassCollection mPasses;
       
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the name of this technique
        /// </summary>
        public string Name
        {
            get { return mMaterialEffectTechnique.Name; }
        }

        /// <summary>
        /// Gets the effect technique to which is this material technique binded
        /// </summary>
        public MaterialEffectTechnique MaterialEffectTechnique
        {
            get { return mMaterialEffectTechnique; }
        }

        /// <summary>
        /// Gets the scene pass in which should be this technique used
        /// </summary>
        public NameId ScenePass
        {
            get { return mMaterialEffectTechnique.ScenePass; }
        }

        /// <summary>
        /// Gets all passes of this technique
        /// </summary>
        public MaterialPassCollection Passes
        {
            get { return mPasses; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="contentGroup">- Content group that should be used to load effects and textures referenced in material</param>
        /// <param name="materialEffect"></param>
        /// <param name="paramsFromMaterial">- Params defined in a parent material</param>
        internal MaterialTechnique(MaterialTechniqueSettings settings, ContentGroup contentGroup, MaterialEffect materialEffect, HashSet<MaterialParam> paramsFromMaterial)
        {
            mMaterialEffectTechnique = materialEffect.Techniques[settings.Name];

            //load material params defined in technique
            HashSet<MaterialParam> paramsFromTechnique = new HashSet<MaterialParam>(MaterialParamByNameEqualityComparer.Instance);
            foreach (MaterialParamSettings paramDesc in settings.Params)
            {
                MaterialParam param = paramDesc.CreateMaterialParam(materialEffect, contentGroup);
                if (mMaterialEffectTechnique.IsParameterUsed(param.MaterialEffectParam))
                {
                    paramsFromTechnique.Add(paramDesc.CreateMaterialParam(materialEffect, contentGroup));
                }
            }
            //add parameters from material
            if (null != paramsFromMaterial)
            {
                foreach (MaterialParam param in paramsFromMaterial)
                {
                    if (mMaterialEffectTechnique.IsParameterUsed(param.MaterialEffectParam))
                    {
                        paramsFromTechnique.Add(param);  //AddIfNotPresent
                    }
                }
            }

            //load passes from settings
            List<MaterialPass> passes = new List<MaterialPass>(settings.Passes.Count);
            HashSet<string> passesLoadedFromSettings = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (MaterialPassSettings passSettings in settings.Passes)
            {
                passes.Add(new MaterialPass(passSettings, contentGroup, materialEffect, mMaterialEffectTechnique, paramsFromTechnique));
                passesLoadedFromSettings.Add(passSettings.Name);
            }
            //load rest of the passes without settings
            foreach (MaterialEffectPass effectPass in mMaterialEffectTechnique.Passes)
            {
                if (!passesLoadedFromSettings.Contains(effectPass.Name))
                {
                    passes.Add(new MaterialPass(effectPass, materialEffect, mMaterialEffectTechnique, paramsFromTechnique));
                }
            }

            mPasses = new MaterialPassCollection(passes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="techniqueName"></param>
        /// <param name="materialEffect"></param>
        /// <param name="paramsFromMaterial">- Params defined in a parent material</param>
        internal MaterialTechnique(MaterialEffectTechnique technique, MaterialEffect materialEffect, HashSet<MaterialParam> paramsFromMaterial)
        {
            mMaterialEffectTechnique = technique;

            //load material params defined in technique
            HashSet<MaterialParam> paramsFromTechnique = new HashSet<MaterialParam>(MaterialParamByNameEqualityComparer.Instance);
            //add parameters from material
            if (null != paramsFromMaterial)
            {
                foreach (MaterialParam param in paramsFromMaterial)
                {
                    if (mMaterialEffectTechnique.IsParameterUsed(param.MaterialEffectParam))
                    {
                        paramsFromTechnique.Add(param);
                    }
                }
            }

            //load passes
            List<MaterialPass> passes = new List<MaterialPass>(mMaterialEffectTechnique.Passes.Count);
            foreach (MaterialEffectPass pass in mMaterialEffectTechnique.Passes)
            {
                passes.Add(new MaterialPass(pass, materialEffect, mMaterialEffectTechnique, paramsFromMaterial));
            }
            mPasses = new MaterialPassCollection(passes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mMaterialEffectTechnique.Name;
        }



        #endregion Methods
    }
}
