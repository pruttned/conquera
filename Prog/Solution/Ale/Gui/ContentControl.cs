using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    public class ContentControl : Control
    {
        private Control mContent = null;

        public Control Content
        {
            get { return mContent; }
            set
            {
                if (value != mContent)
                {
                    if (mContent != null) //old
                    {
                        ChildControls.Remove(mContent);
                    }

                    mContent = value;

                    if (mContent != null) //new
                    {
                        ChildControls.Add(mContent);
                    }
                }
            }
        }
    }
}
