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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ale.Gui;
using Microsoft.Xna.Framework.Graphics;
using Ale.Graphics;
using Ale.Settings;
using Ale.Tools;
using Ale;
using Ale.Content;

namespace Conquera
{
    internal class CellNotificationLabel
    {
        private TextDrawing mTextDrawing;
        private ImageDrawing mImage;
        private Point mScreenPos;
        private Vector3 mWorldPos;
        private float mFadeStartTime = -1;

        private FloatLinearAnimator mAnimator = new FloatLinearAnimator();
        private PointLinearAnimator mPosAnimator = new PointLinearAnimator();

        public CellNotificationLabel(string text, ImageDrawing image, GuiFont font, Color color, Vector3 worldPos)
        {
            mTextDrawing = new TextDrawing(font, color);
            mTextDrawing.Text = text;
            mImage = image;
            mWorldPos = worldPos;

            mAnimator.Animate(1.5f, 1.0f, 0.0f);
            mPosAnimator.Animate(40, Point.Zero, new Point(10,10));
        }

        public bool Draw(AleGameTime gameTime, SpriteBatch spriteBatch)
        {
            if (0 > mFadeStartTime)
            {
                mFadeStartTime = gameTime.TotalTime + 1;
            }
            if (gameTime.TotalTime < mFadeStartTime || mAnimator.Update(gameTime))
            {
                mPosAnimator.Update(gameTime);
                mTextDrawing.Color = new Color(mTextDrawing.Color, mAnimator.CurrentValue);
                mTextDrawing.Draw(spriteBatch, mPosAnimator.CurrentValue);

                if (null != mImage)
                {
                    Point imagePos = mPosAnimator.CurrentValue;
                    imagePos.X -= mImage.Width;
                    mImage.Color = new Color(1, 1, 1, mAnimator.CurrentValue);
                    mImage.Draw(spriteBatch, imagePos);
                }

                return true;
            }

            return false;
        }


        public void UpdateViewProjTransformation(Viewport viewport, ICamera camera)
        {
            mScreenPos = ProjectPos(mWorldPos, viewport, camera);
            Point screenPosEnd = mScreenPos;
            screenPosEnd.Y -= 1000;

            mPosAnimator.ChangeStartEnd(mScreenPos, screenPosEnd);
        }

        private Point ProjectPos(Vector3 pos, Viewport viewport, ICamera camera)
        {
            Vector3 screenPos = viewport.Project(pos, camera.ProjectionTransformation, camera.ViewTransformation, Matrix.Identity);
            screenPos.X -= mTextDrawing.Width / 2.0f;
            screenPos.Y -= mTextDrawing.Height / 2.0f;

            if (null != mImage)
            {
                screenPos.X += mImage.Width / 2.0f;
            }
            return new Point((int)screenPos.X, (int)screenPos.Y);
        }
    }


    internal sealed class CellLabelManager : IDisposable
    {
        private SpriteBatch mSpriteBatch;
        private ICamera mCamera;
        private List<CellNotificationLabel> mLabels = new List<CellNotificationLabel>();
        private bool mIsDisposed = false;
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private Viewport mViewport;
        private GuiFont mGuiFont;        

        public CellLabelManager(ICamera camera, GraphicsDeviceManager graphicsDeviceManager, ContentGroup content)
        {
            mGuiFont = GuiFont.Get(content.Load<SpriteFont>("CellNotificationFont"));            

            mSpriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);
            mGraphicsDeviceManager = graphicsDeviceManager;
            mViewport = graphicsDeviceManager.GraphicsDevice.Viewport;

            mCamera = camera;
            mCamera.ViewProjectionTransformationChanged += new CameraTransformationChangedHandler(mCamera_ViewProjectionTransformationChanged);
            AppSettingsManager.Default.AppSettingsCommitted += new AppSettingsManager.CommittedHandler(Default_AppSettingsCommitted);
        }

        public void AddLabel(string text, ImageDrawing image, Color textColor, Vector3 worldPos)
        {
            var label = new CellNotificationLabel(text, image, mGuiFont, textColor, worldPos);
            label.UpdateViewProjTransformation(mViewport, mCamera);
            mLabels.Add(label);
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                mSpriteBatch.Dispose();
                mCamera.ViewProjectionTransformationChanged -= mCamera_ViewProjectionTransformationChanged;
                AppSettingsManager.Default.AppSettingsCommitted -= Default_AppSettingsCommitted;

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

        public void Draw(AleGameTime gameTime)
        {
            if (0 < mLabels.Count)
            {
                mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.None);

                for (int i = mLabels.Count - 1; i >= 0; --i)
                {
                    if (!mLabels[i].Draw(gameTime, mSpriteBatch))
                    {
                        mLabels.RemoveAt(i);
                    }
                }

                mSpriteBatch.End();
            }
        }

        private void Default_AppSettingsCommitted(IAppSettings settings)
        {
            if (settings is VideoSettings)
            {
                mViewport = mGraphicsDeviceManager.GraphicsDevice.Viewport;
                UpdateTransformations();
            }
        }

        private void mCamera_ViewProjectionTransformationChanged(ICamera camera)
        {
            UpdateTransformations();
        }

        private void UpdateTransformations()
        {
            foreach (var label in mLabels)
            {
                label.UpdateViewProjTransformation(mViewport, mCamera);
            }
        }

    }

    public class CellNotificationIcons
    {
        public static readonly string Coin = "Coin";
        public static readonly string Hearth = "Hearth";
        public static readonly string BrokenHearth = "BrokenHearth";
        public static readonly string FireStorm = "FireStorm";
        public static readonly string Slayer = "Slayer";
        public static readonly string Spikes = "Spikes";
        public static readonly string VampiricTouch = "VampiricTouch";
        public static readonly string PackReinforcement = "PackReinforcement";
        public static readonly string MindControl = "MindControl";
        public static readonly string Plague = "Plague";
        public static readonly string BloodMadness = "BloodMadness";
        public static readonly string LastSacrifice = "LastSacrifice";
    }
}
