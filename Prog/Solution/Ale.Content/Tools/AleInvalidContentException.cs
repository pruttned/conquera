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
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Ale.Content.Tools
{
    class AleInvalidContentException : InvalidContentException
    {
        #region Methods

        public AleInvalidContentException(string message)
            : base(message)
        {
        }

        public AleInvalidContentException(string message, Exception innerException)
            : base(BuildFullMessage(message, innerException))
        {
        }

        private static string BuildFullMessage(string message, Exception innerException)
        {
            return string.Format("{0}\n==========Inner error============\n{1}", message, innerException.ToString());
        }

        #endregion Methods
    }
}
