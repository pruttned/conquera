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

//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using Ale.Graphics;
//using System.Collections;

//namespace Ale.Content
//{
//    /// <summary>
//    /// Reads the Material from xnb in the content pipeline
//    /// </summary>
//    public class MaterialReader : ContentTypeReader<Material>
//    {
//        /// <summary>
//        /// Reads the material (see ContentTypeReader)
//        /// </summary>
//        /// <param name="input"></param>
//        /// <param name="existingInstance"></param>
//        /// <returns></returns>
//        protected override Material Read(ContentReader input, Material existingInstance)
//        {
//            //get material effect
//            string effect = input.ReadString();
//            MaterialEffect materialEffect = input.ContentManager.Load<MaterialEffect>(effect);

//            int techniqueCnt = input.ReadInt32();

//            //techniques
//            List<MaterialTechnique> materialTechniques = new List<MaterialTechnique>(techniqueCnt);
//            for (int i = 0; i < techniqueCnt; ++i)
//            {
//                materialTechniques.Add(ReadTechnique(input, materialEffect));
//            }

//            return new Material(materialEffect, new MaterialTechniqueCollection(materialTechniques));
//        }

//        /// <summary>
//        /// Reads one material technique
//        /// </summary>
//        /// <param name="input">- Input</param>
//        /// <param name="materialEffect">- Material effect that is used by currently loading material</param>
//        /// <returns>MaterialTechnique</returns>
//        private MaterialTechnique ReadTechnique(ContentReader input, MaterialEffect materialEffect)
//        {
//            string name = input.ReadString();
//            MaterialEffectTechnique materialEffectTechnique = materialEffect.Techniques[name];
//            if (null == materialEffectTechnique)
//            {
//                throw new ArgumentException(string.Format("Technique with name '{0}' couldn't be found in the material effect", name));
//            }

//            int passCnt = input.ReadInt32();

//            //passes
//            List<MaterialPass> materialPasses = new List<MaterialPass>(passCnt);
//            for (int i = 0; i < passCnt; ++i)
//            {
//                materialPasses.Add(ReadPass(input, materialEffectTechnique, materialEffect));
//            }

//            return new MaterialTechnique(materialEffectTechnique, new MaterialPassCollection(materialPasses));
//        }

//        /// <summary>
//        /// Reads one material pass
//        /// </summary>
//        /// <param name="input">- Input</param>
//        /// <param name="materialEffectTechnique">- Material effect technique to which belongs MaterialEffectPass that is used by material pass that is
//        /// going to be loaded</param>
//        /// <param name="materialEffect">- Material effect that is used by currently loading material</param>
//        /// <returns>MaterialPass</returns>
//        private MaterialPass ReadPass(ContentReader input, MaterialEffectTechnique materialEffectTechnique, MaterialEffect materialEffect)
//        {
//            string name = input.ReadString();
//            MaterialEffectPass materialEffectPass = materialEffectTechnique.Passes[name];
//            if (null == materialEffectPass)
//            {
//                throw new ArgumentException(string.Format("Pass with name '{0}' couldn't be found in the material effect technique '{1}'", name, materialEffectTechnique.Name));
//            }

//            int paramCnt = input.ReadInt32();

//            //params
//            List<MaterialParam> materialParams = new List<MaterialParam>(paramCnt);
//            for (int i = 0; i < paramCnt; ++i)
//            {
//                materialParams.Add(ReadParameter(input, materialEffect));
//            }

//            return new MaterialPass(materialEffectPass, new MaterialParamCollection(materialParams));
//        }

//        /// <summary>
//        /// Reads one material parameter
//        /// </summary>
//        /// <param name="input">- Input</param>
//        /// <param name="materialEffect">- Material effect that is used by currently loading material</param>
//        /// <returns>MaterialParam</returns>
//        private MaterialParam ReadParameter(ContentReader input, MaterialEffect materialEffect)
//        {
//            string name = input.ReadString();
//            MaterialEffectParam materialEffectParam = materialEffect.ManualParameters[name];
//            if (null == materialEffectParam)
//            {
//                throw new ArgumentException(string.Format("Param with name '{0}' couldn't be found in the material effect", name));
//            }
            
//            MaterialEffectParam.ParamType paramType = (MaterialEffectParam.ParamType)input.ReadByte();

//            if (materialEffectParam.ParameterType != paramType)
//            {
//                throw new ArgumentException(String.Format("Type of the parameter '{0}' that is specified in the material is different that the type specified in the effect", name));
//            }

//            //param value
//            object paramValue;
//            switch (paramType)
//            {
//                case MaterialEffectParam.ParamType.Float:
//                    paramValue = input.ReadSingle();
//                    break;
//                case MaterialEffectParam.ParamType.Int:
//                    paramValue = input.ReadInt32();
//                    break;
//                case MaterialEffectParam.ParamType.Bool:
//                    paramValue = input.ReadBoolean();
//                    break;
//                case MaterialEffectParam.ParamType.Vector2:
//                    paramValue = input.ReadVector2();
//                    break;
//                case MaterialEffectParam.ParamType.Vector3:
//                    paramValue = input.ReadVector3();
//                    break;
//                case MaterialEffectParam.ParamType.Vector4:
//                    paramValue = input.ReadVector4();
//                    break;
//                case MaterialEffectParam.ParamType.Matrix:
//                    paramValue = input.ReadMatrix();
//                    break;
//                case MaterialEffectParam.ParamType.Texture2D:
//                    {
//                        string textureName = input.ReadString();
//                        paramValue = input.ContentManager.Load<Texture2D>(textureName);
//                        break;
//                    }
//                case MaterialEffectParam.ParamType.Texture3D:
//                    {
//                        string textureName = input.ReadString();
//                        paramValue = input.ContentManager.Load<Texture3D>(textureName);
//                        break;
//                    }
//                case MaterialEffectParam.ParamType.TextureCube:
//                    {
//                        string textureName = input.ReadString();
//                        paramValue = input.ContentManager.Load<TextureCube>(textureName);
//                        break;
//                    }
//                default:
//                    paramValue = ReadArrayParamValue(input, paramType);
//                        break;
//            }

//            return new MaterialParam(materialEffectParam, paramValue);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="input"></param>
//        /// <param name="paramType"></param>
//        /// <returns></returns>
//        private object ReadArrayParamValue(ContentReader input, MaterialEffectParam.ParamType paramType)
//        {
//            int itemCnt = input.ReadInt32();
//            IList itemArray = Array.CreateInstance(MaterialEffectParam.GetSystemTypeFromParamType(MaterialEffectParam.ArrayParamTypeToBase(paramType)), itemCnt);
            
//            for (int i = 0; i < itemCnt; ++i)
//            { //I now that the "for" should be in the "switch" for performance purpose but this is more cleaner
//                switch (paramType)
//                {
//                    case MaterialEffectParam.ParamType.FloatArray:
//                        itemArray[i] = (object)input.ReadSingle();
//                        break;
//                    case MaterialEffectParam.ParamType.IntArray:
//                        itemArray[i] = (object)input.ReadInt32();
//                        break;
//                    case MaterialEffectParam.ParamType.BoolArray:
//                        itemArray[i] = (object)input.ReadBoolean();
//                        break;
//                    case MaterialEffectParam.ParamType.Vector2Array:
//                        itemArray[i] = (object)input.ReadVector2();
//                        break;
//                    case MaterialEffectParam.ParamType.Vector3Array:
//                        itemArray[i] = (object)input.ReadVector3();
//                        break;
//                    case MaterialEffectParam.ParamType.Vector4Array:
//                        itemArray[i] = (object)input.ReadVector4();
//                        break;
//                    case MaterialEffectParam.ParamType.MatrixArray:
//                        itemArray[i] = (object)input.ReadMatrix();
//                        break;
//                    default:
//                        throw new ArgumentException("Unsuported parameter type");
//                }
//            }

//            return itemArray;

//        }
//    }
//}
