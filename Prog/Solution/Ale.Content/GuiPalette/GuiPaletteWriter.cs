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
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Ale.Content
{
    [ContentTypeWriter]
    public class GuiPaletteWriter : ContentTypeWriter<GuiPaletteContent>
    {
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Ale.Gui.Palette).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(GuiPaletteReader).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, GuiPaletteContent input)
        {
            WriteRectangles(input.PaletteRoot.SelectNodes("rectangles/rectangle"), output);
            WriteGraphicElements(input.PaletteRoot.SelectNodes("graphicElements/*"), output);
        }

        private void WriteRectangles(XmlNodeList rectangles, ContentWriter output)
        {
            output.Write(rectangles.Count);

            foreach (XmlElement rectangle in rectangles)
            {
                XmlAttributeCollection attributes = rectangle.Attributes;

                output.Write(attributes["name"].Value);
                output.Write(int.Parse(attributes["locationX"].Value));
                output.Write(int.Parse(attributes["locationY"].Value));
                output.Write(int.Parse(attributes["width"].Value));
                output.Write(int.Parse(attributes["height"].Value));
            }
        }

        private void WriteGraphicElements(XmlNodeList graphicElements, ContentWriter output)
        {
            output.Write(graphicElements.Count);

            foreach (XmlElement graphicElement in graphicElements)
            {
                switch (graphicElement.Name)
                {
                    case "image":
                        WriteImage(graphicElement, output);
                        break;
                    
                    case "animation":
                        WriteAnimation(graphicElement, output);
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Type of graphic element was not recognized. Xml element name: '{0}'.", 
                            graphicElement.Name));
                }
            }
        }

        private void WriteImage(XmlElement graphicElement, ContentWriter output)
        {
            XmlAttributeCollection attributes = graphicElement.Attributes;

            output.Write(0);
            output.Write(attributes["name"].Value);
            output.Write(attributes["textureFileName"].Value);
            output.Write(int.Parse(attributes["locationX"].Value));
            output.Write(int.Parse(attributes["locationY"].Value));
            output.Write(int.Parse(attributes["width"].Value));
            output.Write(int.Parse(attributes["height"].Value));
        }

        private void WriteAnimation(XmlElement graphicElement, ContentWriter output)
        {
            XmlAttributeCollection attributes = graphicElement.Attributes;

            output.Write(1);
            output.Write(attributes["name"].Value);
            output.Write(attributes["textureFileName"].Value);
            output.Write(int.Parse(attributes["frameDuration"].Value));
            output.Write(bool.Parse(attributes["loop"].Value));
            output.Write(int.Parse(attributes["width"].Value));
            output.Write(int.Parse(attributes["height"].Value));

            //Frames.
            XmlNodeList frames = graphicElement.SelectNodes("frame");
            output.Write(frames.Count);
            foreach (XmlElement frame in frames)
            {
                output.Write(int.Parse(frame.Attributes["locationX"].Value));
                output.Write(int.Parse(frame.Attributes["locationY"].Value));
            }
        }
    }
}
