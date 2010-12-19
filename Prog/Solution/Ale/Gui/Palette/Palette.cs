using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    public class Palette
    {
        #region Fields

        /// <summary>
        /// Constructors of graphic element creators.
        /// (graphic element id in .xnb file x graphic element creator of this type)
        /// </summary>
        private static Dictionary<int, ConstructorInfo> mGraphicElementCreatorConstructors = new Dictionary<int, ConstructorInfo>();

        /// <summary>
        /// Rectangles defined in the palette.
        /// (rectangle name x rectangle)
        /// </summary>
        private Dictionary<string, Rectangle> mRectangles = new Dictionary<string, Rectangle>();

        /// <summary>
        /// Creators for graphic elements defined in the palette.
        /// (graphic element name x creator of this graphic element)
        /// </summary>
        private Dictionary<string, IGraphicElementCreator> mGraphicElementCreators = new Dictionary<string, IGraphicElementCreator>();

        /// <summary>
        /// Texture cach.
        /// (texture file name x texture)
        /// </summary>
        private Dictionary<string, Texture2D> mTextures = new Dictionary<string, Texture2D>();

        private ContentManager mContent;

        #endregion Fields

        #region Constructors

        static Palette()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsAbstract && typeof(IGraphicElementCreator).IsAssignableFrom(type))
                {
                    object[] attributes = type.GetCustomAttributes(typeof(GraphicElementCreatorAttribute), false);

                    if (0 != attributes.Length)
                    {
                        mGraphicElementCreatorConstructors.Add(
                            ((GraphicElementCreatorAttribute)attributes[0]).GraphicElementTypeId, type.GetConstructor(Type.EmptyTypes));
                    }
                }
            }
        }

        /// <summary>
        /// Internal - instances created by ContentManager.
        /// </summary>
        /// <param name="input"></param>
        internal Palette(ContentReader input)
        {
            mContent = input.ContentManager;
            ReadRectangles(input);
            ReadGraphicElements(input);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Reads 4 Int32 numbers and returns a Rectangle.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Rectangle ReadRectangle(ContentReader input)
        {
            return new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32());
        }

        /// <summary>
        /// Creates a Rectangle.
        /// </summary>
        /// <param name="name">Name of the Rectangle.</param>
        /// <returns></returns>
        public Rectangle CreateRectangle(string name)
        {
            Rectangle rectangle;
            if (mRectangles.TryGetValue(name, out rectangle))
            {
                return rectangle;
            }

            throw new ArgumentException(string.Format("There is no rectangle with name '{0}' in the palette.", name));
        }

        /// <summary>
        /// Creates a graphic element instance.
        /// </summary>
        /// <param name="name">Name of the graphic element.</param>
        /// <returns></returns>
        public GraphicElement CreateGraphicElement(string name)
        {
            IGraphicElementCreator creator;
            if (mGraphicElementCreators.TryGetValue(name, out creator))
            {
                return creator.CreateGraphicElement();
            }

            throw new ArgumentException(string.Format("There is no graphic element creator for graphic elements with name '{0}' in the palette.", name));
        }

        /// <summary>
        /// Gets a texture - textures are cached.
        /// </summary>
        /// <param name="fileName">Texture content file name without extension.</param>
        /// <returns></returns>
        public Texture2D GetTexture(string fileName)
        {
            Texture2D texture;
            if (!mTextures.TryGetValue(fileName, out texture))
            {
                texture = mContent.Load<Texture2D>(fileName);
                mTextures.Add(fileName, texture);
            }

            return texture;
        }

        private void ReadRectangles(ContentReader input)
        {
            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                string name = input.ReadString();
                Rectangle rectangle = ReadRectangle(input);
                mRectangles.Add(name, rectangle);
            }
        }

        private void ReadGraphicElements(ContentReader input)
        {
            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                int graphicElementType = input.ReadInt32();
                ConstructorInfo creatorConstructor;

                if (mGraphicElementCreatorConstructors.TryGetValue(graphicElementType, out creatorConstructor))
                {
                    string name = input.ReadString();
                    IGraphicElementCreator creator = (IGraphicElementCreator)creatorConstructor.Invoke(null);
                    creator.Initialize(input, this);
                    mGraphicElementCreators.Add(name, creator);
                }
                else
                {
                    throw new NotSupportedException(string.Format("There is no graphic element creator for graphic element with .xnb type id '{0}'.",
                        graphicElementType));
                }
            }
        }

        #endregion Methods
    }
}