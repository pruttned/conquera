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
