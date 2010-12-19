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