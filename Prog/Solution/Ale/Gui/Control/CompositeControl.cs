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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    /// <summary>
    /// Represents a base class for all controls, which can contain child controls.
    /// </summary>
    public abstract class CompositeControl : Control
    {
        #region Fields

        /// <summary>
        /// Child controls.
        /// </summary>
        private ControlList mControls;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the child controls collection.
        /// </summary>
        public ControlList Controls
        {
            get { return mControls; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new CompositeControl instance with an empty child controls collection.
        /// </summary>
        public CompositeControl()
        {
            mControls = new ControlList(this);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Overriden. Gets a child control of this control (on any level), which occupies the specified screen location. 
        /// If there is no such child control, this control is returned. If this control does not occupy the specifeid screen location, null is returned.
        /// </summary>
        /// <param name="screenLocation">Screen location.</param>
        /// <returns>Null, if this control does not occupy the specified screen location. Otherwise a child control (on any level), which occupies
        /// the specified screen location. If there is no such child control, but this control occupies the specified screen location, this control
        /// is returned.</returns>
        public override Control GetControl(Point screenLocation)
        {
            if (OccupiesScreenLocation(screenLocation))
            {
                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    Control control = Controls[i].GetControl(screenLocation);
                    if (null != control)
                    {
                        return control;
                    }
                }

                return this;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Overriden. Draws also child controls.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public override void DrawForeground(SpriteBatch spriteBatch, AleGameTime gameTime)
        {
            base.DrawForeground(spriteBatch, gameTime);

            foreach (Control childControl in Controls)
            {
                if (childControl.X + childControl.Width >= 0 && childControl.X <= ClientArea.Width &&
                    childControl.Y + childControl.Height >= 0 && childControl.Y <= ClientArea.Height &&
                    childControl.Visible)
                {
                    childControl.Draw(spriteBatch, gameTime);
                }
            }
        }

        /// <summary>
        /// Determines, whether the specified control is this control or is one of the child controls (recursive) of this control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public override bool ContainsRecursive(Control control)
        {
            if (base.ContainsRecursive(control))
            {
                return true;
            }

            foreach (Control childControl in Controls)
            {
                if (childControl.ContainsRecursive(control))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Overriden. Checks parenting cycle before setting.
        /// </summary>
        /// <param name="value">New property value.</param>
        internal override void SetParentControl(CompositeControl value)
        {
            CheckParentingCycle(value);
            base.SetParentControl(value);
        }

        /// <summary>
        /// Overriden. 'ScreenLocation' of all child controls are also recalculated.
        /// </summary>
        internal override void RecalculateScreenLocation()
        {
            base.RecalculateScreenLocation();
            RecalculateChildControlsScreenLocation();
        }

        /// <summary>
        /// Overriden. Sets GuiManager to all child controls.
        /// </summary>
        internal override void SetGuiManager(GuiManager value)
        {
            base.SetGuiManager(value);

            foreach (Control childControl in Controls)
            {
                childControl.GuiManager = GuiManager;
            }
        }

        /// <summary>
        /// Recalculates 'ScreenLocation' of all child controls.
        /// </summary>
        protected void RecalculateChildControlsScreenLocation()
        {
            foreach (Control childControl in Controls)
            {
                childControl.RecalculateScreenLocation();
            }
        }

        /// <summary>
        /// Checks, whether setting the specified control as a parent control for this control will cause parenting cycle. Parenting cycle ocurs, when
        /// the control itself or some of its child controls (on any level) is going to be set as its parent control. When a parenting cycle detected, 
        /// ArgumentException is thrown.
        /// </summary>
        /// <param name="newParent">Control to check, whether setting it as parent would cause a parenting cycle.</param>
        private void CheckParentingCycle(CompositeControl newParent)
        {
            if (this == newParent || (newParent != null && newParent.HasParentRecursive(this)))
            {
                throw new ArgumentException("Parenting cycle. A control`s parent cannot be the control itself or some of its child controls.");
            }
        }

        /// <summary>
        /// Returns true, whether this control has a the specified parent control on any level up to the root control.
        /// </summary>
        /// <param name="parent">Parent to find.</param>
        /// <returns>True, whether this control has a the specified parent control on any level to the root control.</returns>
        private bool HasParentRecursive(CompositeControl parent)
        {
            return null != ParentControl && (parent == ParentControl || ParentControl.HasParentRecursive(parent));
        }

        #endregion Methods
    }
}
