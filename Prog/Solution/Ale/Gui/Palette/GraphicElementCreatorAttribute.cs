using System;

namespace Ale.Gui
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class GraphicElementCreatorAttribute : Attribute
    {
        /// <summary>
        /// Id of graphic element type (image, animation...) in .xnb file.
        /// </summary>
        private int mGraphicElementTypeId;

        /// <summary>
        /// Gets the id of graphic element type (image, animation...) in .xnb file.
        /// </summary>
        public int GraphicElementTypeId
        {
            get { return mGraphicElementTypeId; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicElementTypeId">Id of graphic element type (image, animation...) in .xnb file.</param>
        public GraphicElementCreatorAttribute(int graphicElementTypeId)
        {
            mGraphicElementTypeId = graphicElementTypeId;
        }
    }
}