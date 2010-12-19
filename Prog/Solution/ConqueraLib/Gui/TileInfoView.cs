using Ale.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ale.Gui;

namespace Conquera.Gui
{
    public abstract class TileInfoView : Control
    {
        private System.Drawing.SizeF mSize;
        private GraphicElementContainer mIconContainer;
        private GraphicElementContainer mNameContainer;
        private GraphicElementContainer mDescriptionContainer;
        private Rectangle mNameRectangle;

        public override System.Drawing.SizeF Size
        {
            get { return mSize; }
        }

        public TileInfoView()
        {
            Rectangle iconRectangle = GuiManager.Instance.Palette.CreateRectangle("TileInfoViewIcon");
            mIconContainer = new GraphicElementContainer(null, new Point(iconRectangle.Left, iconRectangle.Top));

            mNameRectangle = GuiManager.Instance.Palette.CreateRectangle("TileInfoViewName");
            mNameContainer = new GraphicElementContainer(new TextElement(GuiManager.Instance.GetGuiFont("TileInfo/TileTypeName"), Color.Black), Point.Zero);

            Rectangle descriptionRectangle = GuiManager.Instance.Palette.CreateRectangle("TileInfoViewDescription");
            TextElement descriptionLabel = new TextElement(GuiManager.Instance.GetGuiFont("TileInfo/TileDescription"), Color.Black);
            descriptionLabel.Warp = true;
            descriptionLabel.AutoSize = false;
            descriptionLabel.Width = descriptionRectangle.Width;
            descriptionLabel.Height = descriptionRectangle.Height;
            mDescriptionContainer = new GraphicElementContainer(descriptionLabel, new Point(descriptionRectangle.Left, descriptionRectangle.Top));
        }

        public void SetSize(System.Drawing.SizeF size)
        {
            mSize = size;
        }

        public abstract void Update(HexCell cell);

        public void Setup(string name, GraphicElement icon, string description)
        {
            mIconContainer.GraphicElement = icon;
            ((TextElement)mDescriptionContainer.GraphicElement).Text = description;

            TextElement nameLabel = (TextElement)mNameContainer.GraphicElement;
            nameLabel.Text = name;
            mNameContainer.Location = new Point(mNameRectangle.Left + mNameRectangle.Width / 2 - nameLabel.Width / 2,
                                                mNameRectangle.Top + mNameRectangle.Height / 2 - nameLabel.Height / 2);
        }

        protected override void OnDrawForeground()
        {
            mIconContainer.Draw(this);
            mNameContainer.Draw(this);
            mDescriptionContainer.Draw(this);
        }
    }
}
