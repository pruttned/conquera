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

using Microsoft.Xna.Framework.Graphics;

namespace Ale.Content
{
    /// <summary>
    /// Compiled material effect. Just a wrapper arround CompiledEffect because it is not possible to have
    /// two ContentTypeWriters that use a same content type
    /// </summary>
    public struct CompiledMaterialEffect
    {        
        #region Fields
        
        /// <summary>
        /// Internal CompiledEffect
        /// </summary>
        private CompiledEffect mCompiledEffect;
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the internal CompiledEffect
        /// </summary>
        public CompiledEffect CompiledEffect
        {
            get { return mCompiledEffect; }
        }

        #endregion Properties

        #region Methods
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="compiledEffect">- Internal CompiledEffect</param>
        public CompiledMaterialEffect(CompiledEffect compiledEffect)
        {
            mCompiledEffect = compiledEffect;
        }

        #endregion Methods
    }
}
