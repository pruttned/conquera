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

//using System;
//using System.Xml;
//using Microsoft.Xna.Framework;
//using Ale.Content.Tools;
//using Ale.Graphics;
//using System.Collections;

//namespace Ale.Content
//{
//    /// <summary>
//    /// Compiled material param
//    /// </summary>
//    public class AleCompiledMaterialParam
//    {
//        #region Fields

//        /// <summary>
//        /// Name of the parameter
//        /// </summary>
//        private string mName;

//        /// <summary>
//        /// Type of the parameter
//        /// </summary>
//        private MaterialEffectParam.ParamType mParamType;

//        /// <summary>
//        /// Parameter's value
//        /// </summary>
//        private object mValue;

//        #endregion Fields

//        #region Properties

//        /// <summary>
//        /// Gets the parameter's name
//        /// </summary>
//        public string Name
//        {
//            get { return mName; }
//        }

//        /// <summary>
//        /// Gets the parameter's type
//        /// </summary>
//        public MaterialEffectParam.ParamType ParamType
//        {
//            get { return mParamType; }
//        }

//        /// <summary>
//        /// Gets the parameter's value
//        /// </summary>
//        public object Value
//        {
//            get { return mValue; }
//        }

//        #endregion Properties

//        #region Methods

//        /// <summary>
//        /// Ctor
//        /// </summary>
//        /// <param name="paramNode">- Xml param node that describes this parameter</param>
//        public AleCompiledMaterialParam(XmlNode paramNode)
//        {
//            mName = paramNode.Attributes["name"].Value;

//            XmlNode firstChild = paramNode.FirstChild;
//            MaterialEffectParam.ParamType valueType = (MaterialEffectParam.ParamType)Enum.Parse(typeof(MaterialEffectParam.ParamType), paramNode.FirstChild.Name, true);


//            if (0 == paramNode.ChildNodes.Count) //error
//            {
//                throw new ArgumentException(string.Format("Parameter '{0}' has no value specified - zero child nodes", Name));
//            }
//            try
//            {
//                if (1 == paramNode.ChildNodes.Count) //single value
//                {
//                    mParamType = valueType;
//                    mValue = ParseValue(firstChild.InnerText, mParamType);
//                }
//                else //array
//                {
//                    int childCnt = paramNode.ChildNodes.Count;
//                    Type itemSysType = MaterialEffectParam.GetSystemTypeFromParamType(valueType);
//                    mParamType = MaterialEffectParam.ParamTypeToArray(valueType);
//                    mValue = Array.CreateInstance(itemSysType, childCnt);
//                    for (int i = 0; i < childCnt; ++i)
//                    {
//                        object itemValue = ParseValue(paramNode.ChildNodes[i].InnerText, valueType);
//                        if (itemValue.GetType() != itemSysType)
//                        {
//                            throw new ArgumentException(string.Format("Parameter '{0}' contains values with different types", Name));
//                        }
//                        ((IList)Value)[i] = itemValue;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new AleInvalidContentException(string.Format("Value of the material parameter '{0}' couldn't be parsed.", Name), ex);
//            }
//        }

//        /// <summary>
//        /// Whether are two parameters equal - they have same name
//        /// </summary>
//        /// <param name="obj">- Other param</param>
//        /// <returns>Whether are two parameters equal - they have same name</returns>
//        public override bool Equals(object obj)
//        {
//            return (Name == ((AleCompiledMaterialParam)obj).Name);
//        }

//        /// <summary>
//        /// Gets tha hash code
//        /// </summary>
//        /// <returns>Hash code</returns>
//        public override int GetHashCode()
//        {
//            return Name.GetHashCode();
//        }

//        /// <summary>
//        /// Parses objects value from a given string
//        /// </summary>
//        /// <param name="value">- String value</param>
//        /// <param name="paramType">- Type of the parameter</param>
//        /// <returns>Parsed value</returns>
//        private object ParseValue(string value, MaterialEffectParam.ParamType paramType)
//        {
//            switch (paramType)
//            {
//                case MaterialEffectParam.ParamType.Float:
//                    return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

//                case MaterialEffectParam.ParamType.Int:
//                    return int.Parse(value);

//                case MaterialEffectParam.ParamType.Bool:
//                    return bool.Parse(value);

//                case MaterialEffectParam.ParamType.Vector2:
//                    {
//                        float[] elements = ParseSpaceDelimitedFloats(value);
//                        if (2 != elements.Length)
//                        {
//                            throw new ArgumentException("Invalid number of Vector2 elements. Vector2 consists of 2 space separated floats");
//                        }
//                        return new Vector2(elements[0], elements[1]);
//                    }

//                case MaterialEffectParam.ParamType.Vector3:
//                    {
//                        float[] elements = ParseSpaceDelimitedFloats(value);
//                        if (3 != elements.Length)
//                        {
//                            throw new ArgumentException("Invalid number of Vector3 elements. Vector3 consists of 3 space separated floats");
//                        }
//                        return new Vector3(elements[0], elements[1], elements[2]);
//                    }

//                case MaterialEffectParam.ParamType.Vector4:
//                    {
//                        float[] elements = ParseSpaceDelimitedFloats(value);
//                        if (4 != elements.Length)
//                        {
//                            throw new ArgumentException("Invalid number of Vector4 elements. Vector4 consists of 4 space separated floats");
//                        }
//                        return new Vector4(elements[0], elements[1], elements[2], elements[3]);
//                    }

//                case MaterialEffectParam.ParamType.Matrix:
//                    {
//                        float[] elements = ParseSpaceDelimitedFloats(value);
//                        if (16 != elements.Length)
//                        {
//                            throw new ArgumentException("Invalid number of Matrix elements. Matrix consists of 16 space separated floats");
//                        }
//                        return new Matrix(elements[0], elements[1], elements[2], elements[3],
//                            elements[4], elements[5], elements[6], elements[7],
//                            elements[8], elements[9], elements[10], elements[11],
//                            elements[12], elements[13], elements[14], elements[15]);
//                    }

//                case MaterialEffectParam.ParamType.Texture2D:
//                    return value;

//                case MaterialEffectParam.ParamType.Texture3D:
//                    return value;

//                case MaterialEffectParam.ParamType.TextureCube:
//                    return value;
//            }
//            throw new ArgumentException("Unsuported parameter type");
//        }

//        /// <summary>
//        /// Parses space delimited floats to array of floats
//        /// </summary>
//        /// <param name="value">- String that contains space delimited floats</param>
//        /// <returns>Array of parsed floats</returns>
//        private float[] ParseSpaceDelimitedFloats(string value)
//        {

//            string[] stringValues = value.Split(' ');
//            float[] floatValues = new float[stringValues.Length];

//            for (int i = 0; i < stringValues.Length; ++i)
//            {
//                try
//                {
//                    floatValues[i] = float.Parse(stringValues[i], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
//                }
//                catch
//                {
//                    throw new ArgumentException("(" + value + ") ---" + stringValues[i]);
//                }
//            }
//            return floatValues;
//        }

//        #endregion Methods

//    }
//}
