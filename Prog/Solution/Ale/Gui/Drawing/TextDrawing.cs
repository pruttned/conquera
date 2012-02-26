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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    public class TextDrawing
    {
        public event PropertyChangedHandler<TextDrawing, int> WidthChanged;
        public event PropertyChangedHandler<TextDrawing, int> HeightChanged;

        /// <summary>
        /// Base unchanged text.
        /// -> mText --(warp)--> mDrawSourceText --(substring)--> mDrawText --> (draw)
        /// </summary>
        private string mText;

        /// <summary>
        /// Source string for 'mDrawText'. 'mDrawText' is a substring of this string. When warpping is enabled, this string represents warpped 'mText'.
        /// -> mText --(warp)--> mDrawSourceText --(substring)--> mDrawText --> (draw)
        /// </summary>
        private string mDrawSourceText;

        /// <summary>
        /// Text being drawn. It is a substring of 'mDrawSourceText'.
        /// -> mText --(warp)--> mDrawSourceText --(substring)--> mDrawText --> (draw)
        /// </summary>
        private string mDrawText;

        /// <summary>
        /// Width of this TextElement.
        /// </summary>
        private int mWidth = 0;

        /// <summary>
        /// Height of this TextElement.
        /// </summary>
        private int mHeight;

        /// <summary>
        /// Width of last mDrawSourceText row.
        /// Used for warpping.
        /// </summary>
        private int mWarpLastRowWidth = 0;

        /// <summary>
        /// Separator of words.
        /// Used for warpping.
        /// </summary>
        private char[] mWordsSeparator = new char[] { ' ' };

        /// <summary>
        /// Font of this TextElement.
        /// </summary>
        private GuiFont mFont;

        /// <summary>
        /// True, warp is enabled.
        /// </summary>
        private bool mWarp;

        /// <summary>
        /// Text color.
        /// </summary>
        private Color mColor = Color.White;

        /// <summary>
        /// True, 'RefreshDrawSourceText()' does nothing.
        /// </summary>
        private bool mSuspendRefreshDrawSourceText = false;

        /// <summary>
        /// Indexes of characters that represent beginnings of lines.
        /// </summary>
        private List<int> mLineBeginnings = new List<int>();

        /// <summary>
        /// Index of first line to draw.
        /// </summary>
        private int mStartLine = 0;

        /// <summary>
        /// If true, text is not warped.
        /// </summary>
        private bool mAutoSize;

        /// <summary>
        /// Gets or sets the text of this TextElement.
        /// </summary>
        public string Text
        {
            get { return mText; }
            set 
            {
                if (value != Text)
                {
                    mText = value;
                    RefreshDrawSourceText();
                }
            }
        }

        /// <summary>
        /// Gets or sets, whether text warp is enabled. If true, text is warped.
        /// </summary>
        public bool Warp
        {
            get { return mWarp; }
            set 
            {
                if (value != Warp)
                {
                    mWarp = value;
                    RefreshDrawSourceText();
                }
            }
        }

        /// <summary>
        /// Gets or sets the font of this TextElement.
        /// ! Once, the font is set, the TextElement will not reflect any changes of its properties. !
        /// </summary>
        public GuiFont Font
        {
            get { return mFont; }
            set 
            {
                if (value != Font)
                {
                    if (null == value)
                    {
                        throw new NullReferenceException("Value cannot be null.");
                    }

                    mFont = value;
                    RefreshDrawSourceText();

                    if (AutoSize)
                    {
                        SetHeight(Font.FirstLineHeight);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color Color
        {
            get { return mColor; }
            set { mColor = value; }
        }

        /// <summary>
        /// Gets or sets, whether text autosize is enabled. If true, text is not warped.
        /// </summary>
        public bool AutoSize
        {
            get { return mAutoSize; }
            set 
            {
                if (value != AutoSize)
                {
                    mAutoSize = value;

                    if (AutoSize)
                    {
                        RefreshDrawSourceText(); //also sets 'Width'
                        SetHeight(Font.FirstLineHeight);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of this TextElement.
        /// </summary>
        public int Width
        {
            get { return mWidth; }
            set
            {
                if (!AutoSize)
                {
                    SetWidth(value, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of this TextElement.
        /// </summary>
        public int Height
        {
            get { return mHeight; }
            set
            {
                if (!AutoSize)
                {
                    SetHeight(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of first line to draw.
        /// </summary>
        public int StartLine
        {
            get { return mStartLine; }
            set 
            {
                if (value != StartLine)
                {
                    if (value >= LineCount || value <= -1)
                    {
                        throw new IndexOutOfRangeException("Value out of interval <0, LineCount - 1>.");
                    }

                    mStartLine = value;
                    RefreshDrawText();
                }
            }
        }

        /// <summary>
        /// Gets the index of last visible line.
        /// </summary>
        public int LastVisibleLine
        {
            get
            {
                return Math.Min(LineCount - 1, (Height - Font.FirstLineMargin) / Font.InnerFont.LineSpacing + StartLine - 1);
            }
        }

        /// <summary>
        /// Gets the total count of lines in the text.
        /// </summary>
        public int LineCount
        {
            get { return mLineBeginnings.Count; }
        }

        /// <summary>
        /// Constructs a new instance of TextElement with an empty text.
        /// </summary>
        /// <param name="width">Width of this TextElement.</param>
        /// <param name="height">Height of this TextElement.</param>
        /// <param name="font">Font of this TextElement.</param>
        /// <param name="warp">Whether text warp is enabled. If true, text is warped.</param>
        /// <param name="color">Text color.</param>
        public TextDrawing(int width, int height, GuiFont font, bool warp, Color color)
        {
            Color = color;

            mSuspendRefreshDrawSourceText = true;            
            Font = font;
            Width = width;
            Warp = warp;
            mSuspendRefreshDrawSourceText = false;            

            Height = height;
        }

        /// <summary>
        /// Constructs a new instance of TextElement with an empty text. Autosize is set to true and Warp to false.
        /// </summary>
        /// <param name="font">Font of this TextElement.</param>
        /// <param name="color">Text color.</param>
        public TextDrawing(GuiFont font, Color color)
        {
            Color = color;

            mSuspendRefreshDrawSourceText = true;
            Font = font;
            AutoSize = true;
            Warp = false;
            mSuspendRefreshDrawSourceText = false;
        }

        public void Draw(SpriteBatch spriteBatch, Point positionOnScreen)
        {
            Draw(spriteBatch, GuiHelper.ConvertPointToVector(positionOnScreen));
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 positionOnScreen)
        {
            if (!string.IsNullOrEmpty(mDrawText))
            {
                spriteBatch.DrawString(Font.InnerFont, mDrawText, positionOnScreen, Color);
            }
        }

        /// <summary>
        /// Appends a text into this TextElement.
        /// </summary>
        /// <param name="text">Text to append.</param>
        public void Append(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                mSuspendRefreshDrawSourceText = true;
                Text += text;
                mSuspendRefreshDrawSourceText = false;

                int oldDrawSourceTextLength = mDrawSourceText.Length;
                mDrawSourceText += Warp && !AutoSize ? WarpText(text) : text;

                if (AutoSize)
                {
                    SetAutosizeWidth();
                }

                AddLineBeginnings(oldDrawSourceTextLength);
                RefreshDrawText();
            }
        }

        /// <summary>
        /// Appends a line of text into this TextElement.
        /// </summary>
        /// <param name="text">Text line to append.</param>
        public void AppendLine(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Append("\n" + text);
            }
        }

        /// <summary>
        /// If the line with the specified index is not currently visible in the TextElement, the TextElement is scrolled down or up to make the line visible.
        /// </summary>
        /// <param name="lineIndex">Index of line, which to make visible.</param>
        public void MakeLineVisible(int lineIndex)
        {
            if (lineIndex >= LineCount || lineIndex <= -1)
            {
                throw new IndexOutOfRangeException("Parameter 'lineIndex' out of interval <0, LineCount - 1>.");
            }

            if (lineIndex < StartLine)
            {
                StartLine = lineIndex;
            }
            else if(lineIndex > LastVisibleLine)
            {
                StartLine += lineIndex - LastVisibleLine;
            }
        }

        /// <summary>
        /// Generates 'mDrawSourceText'.
        /// </summary>
        private void RefreshDrawSourceText()
        {
            if (!mSuspendRefreshDrawSourceText)
            {
                mWarpLastRowWidth = 0;
                mDrawSourceText = Warp && !AutoSize ? WarpText(Text) : Text;
                
                if (AutoSize)
                {
                    SetAutosizeWidth();
                }

                FillLineBeginningsList();
                RefreshDrawText();
            }
        }

        /// <summary>
        /// Creates a substring of 'mDrawSourceText' and sets it to 'mDrawText'.
        /// </summary>
        private void RefreshDrawText()
        {
            if (string.IsNullOrEmpty(mDrawSourceText))
            {
                mDrawText = string.Empty;
            }
            else if (Warp && !AutoSize)
            {
                int toCharIndex;

                if (LastVisibleLine == LineCount - 1)
                {
                    toCharIndex = mDrawSourceText.Length - 1;
                }
                else
                {
                    toCharIndex = mLineBeginnings[LastVisibleLine + 1] - 1;
                }

                mDrawText = mDrawSourceText.Substring(mLineBeginnings[StartLine], toCharIndex - mLineBeginnings[StartLine] + 1);
            }
            else
            {
                mDrawText = CutNotWarpedText(mDrawSourceText);
            }
        }

        private string CutNotWarpedText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return text.Substring(0, GetLastVisibleCharIndex(text) + 1);
        }

        /// <summary>
        /// Warps the specified text.
        /// </summary>
        /// <param name="text">Text to warp.</param>
        /// <returns>Warpped text.</returns>
        private string WarpText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = text.Replace("\n", " \n ");
            string[] words = text.Split(mWordsSeparator);
            StringBuilder builder = new StringBuilder(text.Length);
            int currentWordWidth;

            for (int i = 0; i < words.Length; i++)
            {
                if ("\n" == words[i])
                {
                    NewLine(builder);
                }
                else
                {
                    currentWordWidth = (int)mFont.InnerFont.MeasureString(words[i]).X;

                    if (mWarpLastRowWidth + currentWordWidth <= mWidth || (0 == mWarpLastRowWidth && currentWordWidth > mWidth))
                    {
                        InsertWord(builder, words[i], currentWordWidth, i == words.Length - 1);
                    }
                    else
                    {
                        NewLine(builder);
                        InsertWord(builder, words[i], currentWordWidth, i == words.Length - 1);
                    }
                }
            }

            return builder.ToString();
        }

        #region WarpText

        /// <summary>
        /// Inserts a word to the specified StringBuilder.
        /// </summary>
        /// <param name="builder">Where to add the word.</param>
        /// <param name="word">Word to add.</param>
        /// <param name="wordWidth">Width of the word to add.</param>
        /// <param name="isLastWord">Whether it is the last word of the text.</param>
        private void InsertWord(StringBuilder builder, string word, int wordWidth, bool isLastWord)
        {
            if (wordWidth <= mWidth)
            {
                builder.Append(word);

                if (isLastWord)
                {
                    mWarpLastRowWidth += wordWidth;
                }
                else
                {
                    builder.Append(" ");
                    mWarpLastRowWidth += wordWidth + Font.SpaceWidth;
                }
            }
            else
            {
                int lastVisibleCharIndex = GetLastVisibleCharIndex(word);
                builder.Append(word.Substring(0, lastVisibleCharIndex + 1));
                NewLine(builder);

                string wordRest = word.Substring(lastVisibleCharIndex + 1);
                InsertWord(builder, wordRest, (int)mFont.InnerFont.MeasureString(wordRest).X, isLastWord);
            }
        }

        /// <summary>
        /// Gets the index of last visible char based on the width of this TextElement.
        /// </summary>
        /// <param name="word">Word which char index to find.</param>
        /// <returns>Index of last visible char based on the width of this TextElement.</returns>
        private int GetLastVisibleCharIndex(string word)
        {
            int lastVisibleCharIndex = Math.Min(word.Length - 1, mWidth / Font.AverageCharWidth - 1);
            int subWordWidth = (int)Font.InnerFont.MeasureString(word.Substring(0, lastVisibleCharIndex + 1)).X;
            int nextCharWidth;

            while (true)
            {
                if (subWordWidth > mWidth)
                {
                    subWordWidth -= (int)Font.InnerFont.MeasureString(word[lastVisibleCharIndex].ToString()).X;
                    lastVisibleCharIndex--;
                }
                else if(lastVisibleCharIndex + 1 != word.Length)
                {
                    nextCharWidth = (int)Font.InnerFont.MeasureString(word[lastVisibleCharIndex + 1].ToString()).X;

                    if (subWordWidth + nextCharWidth <= mWidth)
                    {
                        subWordWidth += nextCharWidth;
                        lastVisibleCharIndex++;
                    }
                    else
                    {
                        return lastVisibleCharIndex;
                    }
                }
                else
                {
                    return lastVisibleCharIndex;
                }
            }
        }

        /// <summary>
        /// Appends a new line to the StringBuilder.
        /// </summary>
        /// <param name="builder">StringBuilder, where to append the new line.</param>
        private void NewLine(StringBuilder builder)
        {
            builder.Append('\n');
            mWarpLastRowWidth = 0;
        }

        #endregion WarpText

        /// <summary>
        /// Clears and fills 'mLineBeginnings' list.
        /// </summary>
        private void FillLineBeginningsList()
        {
            mLineBeginnings.Clear();

            if (!string.IsNullOrEmpty(mDrawSourceText))
            {
                mLineBeginnings.Add(0);
                GetLineBeginnings(0);

                if (StartLine >= mLineBeginnings.Count)
                {
                    StartLine = mLineBeginnings.Count - 1;
                }
            }
            else
            {
                StartLine = 0;
            }
        }

        /// <summary>
        /// Adds new line beginnings to 'mLineBeginnings' list. 'mDrawSourceText' is processed from the specified index.
        /// </summary>
        /// <param name="startIndex">Char index, from which to start process 'mDrawSourceText'.</param>
        private void AddLineBeginnings(int startIndex)
        {
            if (mDrawSourceText[startIndex - 1] == '\n')
            {
                mLineBeginnings.Add(startIndex);
            }

            GetLineBeginnings(startIndex);
        }

        /// <summary>
        /// Adds new line beginnings to 'mLineBeginnings' list. 'mDrawSourceText' is processed from the specified index.
        /// </summary>
        /// <param name="startIndex">Char index, from which to start process 'mDrawSourceText'.</param>
        private void GetLineBeginnings(int startIndex)
        {
            int index = startIndex;

            while (true)
            {
                index = mDrawSourceText.IndexOf('\n', index);

                if (-1 != index && mDrawSourceText.Length - 1 != index)
                {
                    index++;
                    mLineBeginnings.Add(index);
                }
                else
                {
                    break;
                }
            }
        }

        private void SetAutosizeWidth()
        {
            if (string.IsNullOrEmpty(mDrawSourceText))
            {
                SetWidth(0, false);
            }
            else
            {
                SetWidth((int)Font.InnerFont.MeasureString(mDrawSourceText).X, false);
            }
        }

        private void SetWidth(int width, bool refreshDrawSourceText)
        {
            int newWidth = Math.Max(Font.MaxCharWidth, width);
            if (newWidth != mWidth)
            {
                int oldWidth = mWidth;
                mWidth = newWidth;

                if (refreshDrawSourceText)
                {
                    RefreshDrawSourceText();
                }
                EventHelper.RaiseEvent<TextDrawing, int>(WidthChanged, this, oldWidth);
            }
        }

        private void SetHeight(int height)
        {
            int newHeight = Math.Max(0, height);
            if (newHeight != mHeight)
            {
                int oldHeight = mHeight;
                mHeight = newHeight;
                EventHelper.RaiseEvent<TextDrawing, int>(HeightChanged, this, oldHeight);
            }
        }
    }
}