using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    /// <summary>
    /// Used by Palette to create concrete Animaiton instances - parametrized factory.
    /// </summary>
    [GraphicElementCreator(1)]
    public class AnimationCreator : IGraphicElementCreator
    {
        private Texture2D mSourceTexture;
        private int mFrameDuration;
        private bool mLoop;
        private System.Drawing.Size mSourceSize;
        private Point[] mSourceLocations;

        public GraphicElement CreateGraphicElement()
        {
            Animation animation = new Animation(mFrameDuration, mLoop);

            foreach (Point sourceLocation in mSourceLocations)
            {
                Image frame = new Image(new Rectangle(sourceLocation.X, sourceLocation.Y, mSourceSize.Width, mSourceSize.Height), mSourceTexture);
                animation.Frames.Add(frame);
            }

            return animation;
        }

        public void Initialize(ContentReader input, Palette palette)
        {
            mSourceTexture = palette.GetTexture(input.ReadString());
            mFrameDuration = input.ReadInt32();
            mLoop = input.ReadBoolean();
            mSourceSize = new System.Drawing.Size(input.ReadInt32(), input.ReadInt32());

            //Frames.
            int frameCount = input.ReadInt32();
            mSourceLocations = new Point[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                mSourceLocations[i] = new Point(input.ReadInt32(), input.ReadInt32());
            }
        }
    }
}