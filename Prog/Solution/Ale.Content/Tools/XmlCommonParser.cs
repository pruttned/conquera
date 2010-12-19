using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Globalization;

namespace Ale.Content.Tools
{
    /// <summary>
    /// Provides methods for parsing common xml elements
    /// </summary>
    internal static class XmlCommonParser
    {
        /// <summary>
        /// Parses Vector2
        /// </summary>
        /// <param name="vector2XmlNode">requiered attributes: x,y</param>
        /// <returns></returns>
        public static Vector2 ParseVector2(XmlNode vector2XmlNode)
        {
            return new Vector2(float.Parse(vector2XmlNode.Attributes["x"].Value, CultureInfo.InvariantCulture),
                float.Parse(vector2XmlNode.Attributes["y"].Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Parses Vector3
        /// </summary>
        /// <param name="vector3XmlNode">requiered attributes: x,y,z</param>
        /// <returns></returns>
        public static Vector3 ParseVector3(XmlNode vector3XmlNode)
        {
            return new Vector3(float.Parse(vector3XmlNode.Attributes["x"].Value, CultureInfo.InvariantCulture),
                float.Parse(vector3XmlNode.Attributes["y"].Value, CultureInfo.InvariantCulture),
                float.Parse(vector3XmlNode.Attributes["z"].Value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Parses Quaternion
        /// </summary>
        /// <param name="quaternionXmlNode">requiered attributes: x,y,z,w</param>
        /// <returns></returns>
        public static Quaternion ParseQuaternion(XmlNode quaternionXmlNode)
        {
            return new Quaternion(float.Parse(quaternionXmlNode.Attributes["x"].Value, CultureInfo.InvariantCulture),
                float.Parse(quaternionXmlNode.Attributes["y"].Value, CultureInfo.InvariantCulture),
                float.Parse(quaternionXmlNode.Attributes["z"].Value, CultureInfo.InvariantCulture),
                float.Parse(quaternionXmlNode.Attributes["w"].Value, CultureInfo.InvariantCulture));
        }

    }
}
