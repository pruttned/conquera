using System.Xml;

namespace Ale.Content
{
    public class GuiPaletteContent
    {
        private XmlElement mPaletteRoot;

        public XmlElement PaletteRoot
        {
            get { return mPaletteRoot; }
        }

        public GuiPaletteContent(XmlElement paletteRoot)
        {
            mPaletteRoot = paletteRoot;
        }
    }
}
