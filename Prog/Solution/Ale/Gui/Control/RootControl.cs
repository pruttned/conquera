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

namespace Ale.Gui
{
    /// <summary>
    /// Represents the root control of the whole controls hierarchy (of the whole gui).
    /// </summary>
    public class RootControl : CompositeControl
    {
        /// <summary>
        /// Constructs a new RootControl instance.
        /// </summary>
        /// <param name="guiManager">GuiManager for the whole controls hierarchy.</param>
        internal RootControl(GuiManager guiManager)
        {
            GuiManager = guiManager;
            SizeEqualsToBackgroundSize = false;
        }

        /// <summary>
        /// Overriden. Thorws 'InvalidOperationException', becouse a RootControl cannot have a parent control.
        /// </summary>
        /// <param name="value">New property value.</param>
        internal override void SetParentControl(CompositeControl value)
        {
            throw new InvalidOperationException("RootControl cannot have a parent control.");
        }

        internal void SetSizeProperty(System.Drawing.Size size)
        {
            Size = size;
        }

        internal override void SetSize(System.Drawing.Size size)
        {
            base.SetSize(size);
            RecalculateChildControlsScreenLocation();
        }
    }
}
