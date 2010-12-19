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


using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Ale.Content;
using System;
using Microsoft.Xna.Framework;
namespace Ale.Graphics
{
    /// <summary>
    /// Material pass that is binded to a specific effect pass. It has a given list of parameters and their values which
    /// are applied to a coresponding effect parameters during Apply.
    /// </summary>
    public class MaterialPass
    {
        #region Fields

        /// <summary>
        /// Next texture Id (0 reserved for no texture).
        /// </summary>
        private static ushort mNextTextureId = 1;

        /// <summary>
        /// Texture's ids
        /// </summary>
        private static Dictionary<Texture, ushort> mTextureIds = new Dictionary<Texture, ushort>();

        /// <summary>
        /// Number of this pass that is used for renderable unit batching. It consists of effect, texture and pass number
        /// </summary>
        private uint mRenderBatchNumber;

        /// <summary>
        /// Id of the main texture (0 - no texture)
        /// </summary>
        private uint mMainTextureId;

        /// <summary>
        /// Effect pass to which is this material pass binded
        /// </summary>
        private MaterialEffectPass mMaterialEffectPass;

        /// <summary>
        /// Material parameters that are applied by this pass
        /// </summary>
        private MaterialParamCollection mParameters;
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the name of this pass
        /// </summary>
        public string Name
        {
            get { return MaterialEffectPass.Name; }
        }
        
        /// <summary>
        /// Gets the effect pass to which is this material pass binded
        /// </summary>
        public MaterialEffectPass MaterialEffectPass
        {
            get { return mMaterialEffectPass; }
        }

        /// <summary>
        /// Gets the material parameters that are applied by this pass
        /// </summary>
        internal MaterialParamCollection Parameters
        {
            get { return mParameters; }
        }

        /// <summary>
        /// Gets the number of this pass that is used for renderable unit batching. It consists of effect, texture and pass number
        /// </summary>
        public uint RenderBatchNumber
        {
            get { return mRenderBatchNumber; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="contentGroup">- Content group that should be used to load effects and textures referenced in material</param>
        /// <param name="materialEffect"></param>
        /// <param name="materialEffectTechnique"></param>
        /// <param name="paramsFromTechnique">- Params defined in parent technique and material</param>
        internal MaterialPass(MaterialPassSettings settings, ContentGroup contentGroup, MaterialEffect materialEffect, MaterialEffectTechnique materialEffectTechnique, HashSet<MaterialParam> paramsFromTechnique)
        {
            mMaterialEffectPass = materialEffectTechnique.Passes[settings.Name];
            if (null == mMaterialEffectPass)
            {
                throw new ArgumentException(string.Format("Pass with name '{0}' couldn't be found in effect's technique'", mMaterialEffectPass.Name));
            }

            //load material params defined in pass
            HashSet<MaterialParam> paramsFromPass = new HashSet<MaterialParam>(MaterialParamByNameEqualityComparer.Instance);
            foreach (MaterialParamSettings paramDesc in settings.Params)
            {
                paramsFromPass.Add(paramDesc.CreateMaterialParam(materialEffect, contentGroup));
            }
            //add parameters from technique that are not present in passes's parameters
            if (null != paramsFromTechnique)
            {
                paramsFromPass.UnionWith(paramsFromTechnique);
            } 
            List<MaterialParam> paramsList = new List<MaterialParam>(paramsFromPass.Count);
            foreach (MaterialParam param in paramsFromPass)
            {
                paramsList.Add(param);
            }
            mParameters = new MaterialParamCollection(paramsList);

            //Main texture parameter
            InitMainTextureParamId();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="materialEffect"></param>
        /// <param name="materialEffectTechnique"></param>
        /// <param name="paramsFromTechnique">- Params defined in parent technique and material</param>
        internal MaterialPass(MaterialEffectPass pass, MaterialEffect materialEffect, MaterialEffectTechnique materialEffectTechnique, HashSet<MaterialParam> paramsFromTechnique)
        {
            mMaterialEffectPass = pass;

            if (null != paramsFromTechnique)
            {
                mParameters = new MaterialParamCollection(paramsFromTechnique.Count);
                foreach (MaterialParam param in paramsFromTechnique)
                {
                    mParameters.Add(param);
                }
            }
            else
            {
                mParameters = new MaterialParamCollection();
            }

            //Main texture parameter
            InitMainTextureParamId();
        }

        /// <summary>
        /// Applies all material parameters. This means that the each material 
        /// parameter updates its value in a coresponding effect parameter.
        /// You must call MaterialEffectPass.Apply after calling this method in order to commit changes
        /// to the effect's parameters.
        /// </summary>
        /// <example> This sample shows how to use MaterialPass.Apply
        /// <code>
        /// someMaterialPass.Apply(...);
        /// foreach(geometry that should be rendered with someMaterialPass)
        /// {
        ///     someMaterialPass.MaterialEffectPass.Apply(...);
        ///     //render geometry
        ///  }
        ///  
        ///  //....
        ///  //We are done with rendering using Material and MaterialEffect in this frame
        /// MaterialEffect.Finish(); 
        /// //render something for example using a Microsoft.Xna.Framework.Graphics.Effect
        /// </code>
        /// </example>        
        public void Apply()
        {
            Parameters.Apply();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mMaterialEffectPass.Name;
        }

        /// <summary>
        /// Updates render batch number
        /// </summary>
        private void UpdateRenderBatchNumber()
        {
            mRenderBatchNumber = (uint)(((uint)(mMaterialEffectPass.ParentTechnique.ParentMaterialEffect.Id & 0xFFF) << 20) | ((uint)(mMainTextureId & 0xFFFF) << 4) | (uint)(mMaterialEffectPass.OrderInTechnique & 0xF));
        }

        /// <summary>
        /// Gets the Id of a given texture
        /// </summary>
        /// <param name="texture">- Texture whose id should be returned. If eq to null then 0 is returned</param>
        /// <returns>Id of a give texture</returns>
        static private ushort GetTextureId(Texture texture)
        {
            if (null == texture)
            {
                return 0;
            }

            ushort id;
            if (!mTextureIds.TryGetValue(texture, out id))
            {
                id = mNextTextureId++;
                mTextureIds.Add(texture, id);
            }
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitMainTextureParamId()
        {
            if (null != MaterialEffectPass.MainTextureParameter)
            {
                MaterialParam mainTextureParam = Parameters[MaterialEffectPass.MainTextureParameter.Name];
                if (null != mainTextureParam) //found it
                {
                    Texture texture = null;
                    if (mainTextureParam is Texture2DMaterialParam)
                    {
                        texture = ((Texture2DMaterialParam)mainTextureParam).Value;
                    }
                    else
                    {
                        if (mainTextureParam is Texture3DMaterialParam)
                        {
                            texture = ((Texture3DMaterialParam)mainTextureParam).Value;
                        }
                        else
                        {
                            if (mainTextureParam is TextureCubeMaterialParam)
                            {
                                texture = ((TextureCubeMaterialParam)mainTextureParam).Value;
                            }
                        }
                    }

                    mMainTextureId = GetTextureId(texture);
                }
                else
                {
                    mMainTextureId = 0;
                }
            }
            else
            {
                mMainTextureId = 0;
            }

            UpdateRenderBatchNumber();
        }

        #endregion Methods

        public void SetParam(string paramName, int value)
        {
            mParameters.SetParam(new IntMaterialParam((IntMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, float value)
        {
            mParameters.SetParam(new FloatMaterialParam((FloatMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, bool value)
        {
            mParameters.SetParam(new BoolMaterialParam((BoolMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Vector2 value)
        {
            mParameters.SetParam(new Vector2MaterialParam((Vector2MaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Vector3 value)
        {
            mParameters.SetParam(new Vector3MaterialParam((Vector3MaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Vector4 value)
        {
            mParameters.SetParam(new Vector4MaterialParam((Vector4MaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Matrix value)
        {
            mParameters.SetParam(new MatrixMaterialParam((MatrixMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Texture2D value)
        {
            mParameters.SetParam(new Texture2DMaterialParam((Texture2DMaterialEffectParam)GetFxParam(paramName), value));
            InitMainTextureParamId();
        }
        public void SetParam(string paramName, Texture3D value)
        {
            mParameters.SetParam(new Texture3DMaterialParam((Texture3DMaterialEffectParam)GetFxParam(paramName), value));
            InitMainTextureParamId();
        }
        public void SetParam(string paramName, TextureCube value)
        {
            mParameters.SetParam(new TextureCubeMaterialParam((TextureCubeMaterialEffectParam)GetFxParam(paramName), value));
            InitMainTextureParamId();
        }


        public void SetParam(string paramName, int[] value)
        {
            mParameters.SetParam(new IntArrayMaterialParam((IntArrayMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, float[] value)
        {
            mParameters.SetParam(new FloatArrayMaterialParam((FloatArrayMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, bool[] value)
        {
            mParameters.SetParam(new BoolArrayMaterialParam((BoolArrayMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Vector2[] value)
        {
            mParameters.SetParam(new Vector2ArrayMaterialParam((Vector2ArrayMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Vector3[] value)
        {
            mParameters.SetParam(new Vector3ArrayMaterialParam((Vector3ArrayMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Vector4[] value)
        {
            mParameters.SetParam(new Vector4ArrayMaterialParam((Vector4ArrayMaterialEffectParam)GetFxParam(paramName), value));
        }
        public void SetParam(string paramName, Matrix[] value)
        {
            mParameters.SetParam(new MatrixArrayMaterialParam((MatrixArrayMaterialEffectParam)GetFxParam(paramName), value));
        }

        private MaterialEffectParam GetFxParam(string paramName)
        {
            MaterialEffectParam fxParam = (MaterialEffectParam)mMaterialEffectPass.ParentTechnique.ParentMaterialEffect.ManualParameters[paramName];
            if(null == fxParam)
            {
                throw new ArgumentException(string.Format("Manual parameter '{0}' couldn't be found in effect", paramName));
            }
            return fxParam;
        }

    }
}
