using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    /// <summary>
    /// Represents a control containing an empty GraphicElementRepositoryGroup, which always covers the whole client area of the control.
    /// </summary>
    public class SimpleControl : Control
    {
        private GraphicElementRepositoryGroup mMainGraphicElementRepository;

        protected GraphicElementRepositoryGroup MainGraphicElementRepository
        {
            get { return mMainGraphicElementRepository; }
        }

        public SimpleControl()
        {
            mMainGraphicElementRepository = new GraphicElementRepositoryGroup(this);
            MainGraphicElementRepository.Autosize = false;
        }

        internal override void SetClientArea(Rectangle value)
        {
            base.SetClientArea(value);

            MainGraphicElementRepository.Location = ClientArea.Location;
            MainGraphicElementRepository.Size = new System.Drawing.Size(ClientArea.Width, ClientArea.Height);
        }
    }
}
