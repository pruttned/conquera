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
using System.Text;
using Ale.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera.Gui
{
    public class ListBox : Control
    {
        public class SelectedItemChangedEventArgs : EventArgs
        {
            public string Item { get; private set; }

            public SelectedItemChangedEventArgs(string item)
            {
                Item = item;
            }
        }

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        private TextElement mItemsTextElement;
        private GraphicElementContainer mItemsTextElementContainer;
        private IList<string> mItems;
        private GraphicButton mNextPageButton;
        private GraphicButton mPreviousPageButton;
        private int mMaxItemLength;
        private int mCurrentPageNumber;
        private int mMaxItemsPerPage;
        private int mPageCount;
        private string mSelectedItem = null;
        private int mSelectedItemIndex = -1;
        private TextElement mPageTextElement;
        private GraphicElementContainer mPageTextElementContainer;

        public override System.Drawing.SizeF Size
        {
            get { return ConqueraPalette.ListBoxBackground.Size; }
        }

        public string SelectedItem
        {
            get { return mSelectedItem; }
            private set
            {
                if (mSelectedItem != value)
                {
                    mSelectedItem = value;

                    if (SelectedItemChanged != null)
                    {
                        SelectedItemChanged(this, new SelectedItemChangedEventArgs(mSelectedItem));
                    }
                }
            }
        }

        public ListBox(IList<string> items)
        {
            Rectangle itemsRectangle = ConqueraPalette.ListBoxItemsRectangle;
            mItemsTextElement = new TextElement(itemsRectangle.Width, itemsRectangle.Height, ConqueraFonts.SpriteFont1, true, Color.White);
            mItemsTextElementContainer = new GraphicElementContainer(mItemsTextElement, itemsRectangle.Location);            
            
            mItems = items;
            mMaxItemLength = ConqueraPalette.ListBoxItemsRectangle.Width / mItemsTextElement.Font.AverageCharWidth - 1;
            mMaxItemsPerPage = (ConqueraPalette.ListBoxItemsRectangle.Height - mItemsTextElement.Font.FirstLineMargin) / mItemsTextElement.Font.InnerFont.LineSpacing;
            LoadItemsToTextElement();
            mPageCount = (int)Math.Ceiling((double)mItemsTextElement.LineCount / (double)mMaxItemsPerPage);
            mCurrentPageNumber = mItems.Count > 0 ? 1 : 0;

            mNextPageButton = new GraphicButton(ConqueraPalette.ListBoxNextPageButtonDefault, ConqueraPalette.ListBoxNextPageButtonOver, ConqueraPalette.ListBoxNextPageButtonDisabled);
            mPreviousPageButton = new GraphicButton(ConqueraPalette.ListBoxPreviousPageButtonDefault, ConqueraPalette.ListBoxPreviousPageButtonOver, ConqueraPalette.ListBoxPreviousPageButtonDisabled);
            mNextPageButton.Location = ConqueraPalette.ListBoxNextPageButtonLocation;
            mPreviousPageButton.Location = ConqueraPalette.ListBoxPreviousPageButtonLocation;
            mNextPageButton.Click += new EventHandler<ControlEventArgs>(mNextPageButton_Click);
            mPreviousPageButton.Click += new EventHandler<ControlEventArgs>(mPreviousPageButton_Click);
            UpdatePageButtonsHitTest();
            ChildControls.Add(mNextPageButton);
            ChildControls.Add(mPreviousPageButton);

            mPageTextElement = new TextElement(ConqueraFonts.SpriteFont1, Color.White);
            mPageTextElementContainer = new GraphicElementContainer(mPageTextElement, ConqueraPalette.ListBoxPageTextElementRectangle);
            UpdatePageTextElement();

            MouseDown += new EventHandler<MouseEventArgs>(ListBox_MouseDown);
        }

        protected override void OnDrawBackground()
        {
            ConqueraPalette.ListBoxBackground.Draw(ScreenLocation);
        }

        protected override void OnDrawForeground()
        {
            //Selected item.
            if (mSelectedItemIndex != -1 && mSelectedItemIndex >= mItemsTextElement.StartLine && mSelectedItemIndex <= mItemsTextElement.LastVisibleLine)
            {
                ConqueraPalette.ListBoxSelectedItem.Draw(GetItemScreenLocation(mSelectedItemIndex));
            }

            //Item under mouse.
            int itemUnderMouseIndex = GetItemIndexAtPoint(PointToClient(GuiManager.Instance.MouseManager.CursorPosition));
            if (itemUnderMouseIndex != -1 && itemUnderMouseIndex != mSelectedItemIndex)
            {
                ConqueraPalette.ListBoxOverItem.Draw(GetItemScreenLocation(itemUnderMouseIndex));
            }

            //Items.
            mItemsTextElementContainer.Draw(this);

            //Page text element.
            mPageTextElementContainer.Draw(this);
        }

        private void LoadItemsToTextElement()
        {
            StringBuilder builder = new StringBuilder();

            foreach (string item in mItems)
            {
                if (item.Length > mMaxItemLength)
                {
                    builder.AppendLine(string.Format("{0}...", item.Substring(0, mMaxItemLength - 3)));
                }
                else
                {
                    builder.AppendLine(item);
                }
            }
            mItemsTextElement.Text = builder.ToString();
        }

        private void mNextPageButton_Click(object sender, ControlEventArgs e)
        {
            mItemsTextElement.StartLine = mItemsTextElement.LastVisibleLine + 1;
            mCurrentPageNumber++;
            UpdatePageButtonsHitTest();
            UpdatePageTextElement();
        }

        private void mPreviousPageButton_Click(object sender, ControlEventArgs e)
        {
            mItemsTextElement.StartLine = mItemsTextElement.StartLine - mMaxItemsPerPage;
            mCurrentPageNumber--;
            UpdatePageButtonsHitTest();
            UpdatePageTextElement();
        }

        private void UpdatePageButtonsHitTest()
        {
            mNextPageButton.IsHitTestEnabled = mItemsTextElement.LastVisibleLine + 1 < mItemsTextElement.LineCount;
            mPreviousPageButton.IsHitTestEnabled = mItemsTextElement.StartLine > 0;
        }

        private void UpdatePageTextElement()
        {
            mPageTextElement.Text = string.Format("{0}/{1}", mCurrentPageNumber, mPageCount);
        }

        private void ListBox_MouseDown(object sender, MouseEventArgs e)
        {
            int itemIndex = GetItemIndexAtPoint(e.Location);
            if (itemIndex != -1)
            {
                mSelectedItemIndex = itemIndex;
                SelectedItem = mItems[mSelectedItemIndex];
            }
        }

        private int GetItemIndexAtPoint(Point point) //point relative to this control
        {
            if (point.X >= ConqueraPalette.ListBoxItemsRectangle.Left && point.X <= ConqueraPalette.ListBoxItemsRectangle.Right &&
                point.Y >= ConqueraPalette.ListBoxItemsRectangle.Top && point.Y <= ConqueraPalette.ListBoxItemsRectangle.Bottom)
            {
                int yInTextElement = point.Y - ConqueraPalette.ListBoxItemsRectangle.Top;
                int itemIndex = mItemsTextElement.StartLine + (yInTextElement - mItemsTextElement.Font.FirstLineMargin) / mItemsTextElement.Font.InnerFont.LineSpacing;

                if (itemIndex <= mItemsTextElement.LastVisibleLine)
                {
                    return itemIndex;
                }
            }
            return -1;
        }

        private Point GetItemScreenLocation(int itemIndex) //itemIndex must be a valid index of an item - there is no check
        {
            return new Point(ScreenLocation.X + ConqueraPalette.ListBoxItemsRectangle.X,
                             ScreenLocation.Y + ConqueraPalette.ListBoxItemsRectangle.Y + mItemsTextElement.Font.FirstLineMargin +
                                (itemIndex - mItemsTextElement.StartLine) * mItemsTextElement.Font.InnerFont.LineSpacing);
        }
    }
}