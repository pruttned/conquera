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

using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    public class Dialog : Control
    {
        public event EventHandler Closed;

        private Control mOldModalControl;
        private bool mModal;
        private bool mShown = false;

        public bool Shown
        {
            get { return mShown; }
        }

        public bool Show(bool modal)
        {
            if (Shown)
            {
                return false;
            }

            System.Drawing.SizeF screenSize = GuiManager.Instance.ScreenSize;
            Location = new Point((int)(screenSize.Width / 2 - Size.Width / 2), (int)(screenSize.Height / 2 - Size.Height / 2));

            GuiManager.Instance.ActiveScene.RootControls.Add(this);
            
            if (modal)
            {
                mOldModalControl = GuiManager.Instance.ActiveScene.ModalControl;
                GuiManager.Instance.ActiveScene.ModalControl = this;
            }
            mModal = modal;

            mShown = true;
            return true;
        }

        public bool Hide()
        {
            if (!Shown)
            {
                return false;
            }

            if (mModal)
            {
                GuiManager.Instance.ActiveScene.ModalControl = mOldModalControl;
            }

            SiblingColleciton.Remove(this);
            mShown = false;

            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }

            return true;
        }
    }
}
