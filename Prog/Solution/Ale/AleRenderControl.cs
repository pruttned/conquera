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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ale
{
    /// <summary>
    /// Control for rendering Ale content
    /// </summary>
    public partial class AleRenderControl : UserControl
    {
        #region Fields
        
        /// <summary>
        /// Ale game
        /// </summary>
        private AleGame mAleGame = null;

        /// <summary>
        /// Red pen
        /// </summary>
        private Pen mRedPen;

        #endregion Fields

        /// <summary>
        /// Ctor
        /// </summary>
        public AleRenderControl()
        {
            mRedPen = new Pen(Color.Red);

            InitializeComponent();
        }

        /// <summary>
        /// Start the game loop by enabling timer. Called from AleGame.Run
        /// </summary>
        internal void Start(AleGame aleGame)
        {
            mAleGame = aleGame;

            mTimer.Enabled = true;

            mAleGame.OnRenderControlResize(Width, Height);
        }

        /// <summary>
        /// Pause the rendering loop
        /// </summary>
        internal void Pause()
        {
            if (null != mAleGame) //already started
            {
                mTimer.Enabled = false;
            }
        }

        /// <summary>
        /// Resumes paused rendering loop
        /// </summary>
        internal void Resume()
        {
            if (null != mAleGame) //already started
            {
                mTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Draws empty content if Start was not yet called
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (null == mAleGame)
            {
                e.Graphics.DrawRectangle(mRedPen, 0, 0, Width - 1, Height - 1);
                e.Graphics.DrawLine(mRedPen, 0, 0, Width, Height);
                e.Graphics.DrawLine(mRedPen, Width, 0, 0, Height);
            }
        }

        /// <summary>
        /// On size changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (null != mAleGame)
            {
                mAleGame.OnRenderControlResize(Width, Height);
            }
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Updates and renders the game content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mTimer_Tick(object sender, EventArgs e)
        {
            mAleGame.Tick();
        }
    }
}
