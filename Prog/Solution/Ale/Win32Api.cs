using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Ale
{
    /// <summary>
    /// Provides Win32 Api interoperability.
    /// Based on http://www.pinvoke.net/index.aspx
    /// </summary>
    internal class Win32Api
    {
        #region Enums

        #endregion Enums

        #region Structures

        /// <summary>
        /// Rect
        /// </summary>
        public struct RECT
        {
            #region Fields

            /// <summary>
            /// Left position of the rectangle.
            /// </summary>
            public int Left;
            /// <summary>
            /// Top position of the rectangle.
            /// </summary>
            public int Top;
            /// <summary>
            /// Right position of the rectangle.
            /// </summary>
            public int Right;
            /// <summary>
            /// Bottom position of the rectangle.
            /// </summary>
            public int Bottom;

            #endregion Fields

            #region Properties

            /// <summary>
            /// 
            /// </summary>
            public int Width
            {
                get { return Right - Left; }
            }

            /// <summary>
            /// 
            /// </summary>
            public int Height
            {
                get { return Bottom - Top; }
            }

            #endregion Properties

            #region Operators

            /// <summary>
            /// Operator to convert a RECT to Drawing.Rectangle.
            /// </summary>
            /// <param name="rect">Rectangle to convert.</param>
            /// <returns>A Drawing.Rectangle</returns>
            public static implicit operator System.Drawing.Rectangle(RECT rect)
            {
                return System.Drawing.Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            /// <summary>
            /// Operator to convert Drawing.Rectangle to a RECT.
            /// </summary>
            /// <param name="rect">Rectangle to convert.</param>
            /// <returns>RECT rectangle.</returns>
            public static implicit operator RECT(System.Drawing.Rectangle rect)
            {
                return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            #endregion Operators

            #region Methods

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="left">Horizontal position.</param>
            /// <param name="top">Vertical position.</param>
            /// <param name="right">Right most side.</param>
            /// <param name="bottom">Bottom most side.</param>
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            #endregion Methods
        }
        
        #endregion Structures

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool ClipCursor(IntPtr lpRect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool ClipCursor(ref RECT lpRect);

        /// <summary>
        /// Gets the lower part of the dword
        /// </summary>
        /// <param name="dword"></param>
        /// <returns></returns>
        public static UInt16 LoWord(UInt32 dword)
        {
            return (UInt16)(dword);
        }

        /// <summary>
        /// Gets the higher part of the dword
        /// </summary>
        /// <param name="dword"></param>
        /// <returns></returns>
        public static UInt16 HiWord(UInt32 dword)
        {
            return ((UInt16)(((UInt32)(dword) >> 16) & 0xFFFF));
        }

        /// <summary>
        /// Gets the lower part of the word
        /// </summary>
        /// <param name="dword"></param>
        /// <returns></returns>
        public static byte LoWord(UInt16 word)
        {
            return (byte)(word);
        }

        /// <summary>
        /// Gets the higher part of the word
        /// </summary>
        /// <param name="dword"></param>
        /// <returns></returns>
        public static byte HiWord(UInt16 word)
        {
            return ((byte)(((UInt16)(word) >> 8) & 0xFF));
        }

        #endregion Methods
    }

}
