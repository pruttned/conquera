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
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
//using Ale.Graphics;
//using System.Collections;

//namespace Ale.Content
//{
//    /// <summary>
//    /// Content writer for AleCompiledMaterial
//    /// </summary>
//    [ContentTypeWriter]
//    public class AleMaterialWriter : ContentTypeWriter<AleCompiledMaterial>
//    {
//        /// <summary>
//        /// See ContentTypeWriter
//        /// </summary>
//        /// <param name="output"></param>
//        /// <param name="compiledMaterial"></param>
//        protected override void Write(ContentWriter output, AleCompiledMaterial compiledMaterial)
//        {
//            output.Write(compiledMaterial.Effect);

//            //techniques
//            output.Write(compiledMaterial.Techniques.Count);
//            foreach (AleCompiledMaterialTechnique technique in compiledMaterial.Techniques)
//            {
//                output.Write(technique.Name);

//                //pases
//                output.Write(technique.Passes.Count);
//                foreach (AleCompiledMaterialPass pass in technique.Passes)
//                {
//                    output.Write(pass.Name);

//                    //parameters
//                    output.Write(pass.Parameters.Count);
//                    foreach (AleCompiledMaterialParam param in pass.Parameters)
//                    {
//                        WriteParam(output, param);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// See ContentTypeWriter
//        /// </summary>
//        /// <param name="targetPlatform"></param>
//        /// <returns></returns>
//        public override string GetRuntimeReader(TargetPlatform targetPlatform)
//        {
//            return typeof(MaterialReader).AssemblyQualifiedName;
//        }

//        /// <summary>
//        /// Writes one material param
//        /// </summary>
//        /// <param name="output">- Output</param>
//        /// <param name="param">- Param that should be written</param>
//        private void WriteParam(ContentWriter output, AleCompiledMaterialParam param)
//        {
//            output.Write(param.Name);
//            output.Write((byte)param.ParamType);

//            switch (param.ParamType)
//            {
//                case MaterialEffectParam.ParamType.Float:
//                    output.Write((float)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.Int:
//                    output.Write((int)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.Bool:
//                    output.Write((bool)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.Vector2:
//                    output.Write((Vector2)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.Vector3:
//                    output.Write((Vector3)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.Vector4:
//                    output.Write((Vector4)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.Matrix:
//                    output.Write((Matrix)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.Texture2D:
//                    output.Write((string)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.Texture3D:
//                    output.Write((string)param.Value);
//                    break;
//                case MaterialEffectParam.ParamType.TextureCube:
//                    output.Write((string)param.Value);
//                    break;
//                default:
//                    WriteArrayParamValue(output, param);
//                    break;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="output"></param>
//        /// <param name="param"></param>
//        private void WriteArrayParamValue(ContentWriter output, AleCompiledMaterialParam param)
//        {
//            IList listValue = (IList)param.Value;
//            output.Write(listValue.Count);
//            for (int i = 0; i < listValue.Count; ++i)
//            { //I now that the "for" should be in the "switch" for performance purpose but this is more cleaner
//                switch (param.ParamType)
//                {
//                    case MaterialEffectParam.ParamType.FloatArray:
//                        output.Write((float)listValue[i]);
//                        break;
//                    case MaterialEffectParam.ParamType.IntArray:
//                        output.Write((int)listValue[i]);
//                        break;
//                    case MaterialEffectParam.ParamType.BoolArray:
//                        output.Write((bool)listValue[i]);
//                        break;
//                    case MaterialEffectParam.ParamType.Vector2Array:
//                        output.Write((Vector2)listValue[i]);
//                        break;
//                    case MaterialEffectParam.ParamType.Vector3Array:
//                        output.Write((Vector3)listValue[i]);
//                        break;
//                    case MaterialEffectParam.ParamType.Vector4Array:
//                        output.Write((Vector4)listValue[i]);
//                        break;
//                    case MaterialEffectParam.ParamType.MatrixArray:
//                        output.Write((Matrix)listValue[i]);
//                        break;
//                    default:
//                        throw new ArgumentException("Unsuported parameter type");
//                }
//            }
//        }
//    }
//}
