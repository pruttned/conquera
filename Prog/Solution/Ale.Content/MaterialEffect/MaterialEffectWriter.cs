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

using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace Ale.Content
{
    /// <summary>
    /// ContentTypeWriter for MaterialEffect
    /// </summary>
    [ContentTypeWriter]
    public class MaterialEffectWriter : ContentTypeWriter<CompiledMaterialEffect>
    {
        /// <summary>
        /// See ContentTypeReader
        /// </summary>
        /// <param name="output"></param>
        /// <param name="value"></param>
        protected override void Write(ContentWriter output, CompiledMaterialEffect value)
        {
            byte[] effectCode = value.CompiledEffect.GetEffectCode();
            output.Write(effectCode.Length);
            output.Write(effectCode);
        }

        /// <summary>
        /// See ContentTypeReader
        /// </summary>
        /// <param name="targetPlatform"></param>
        /// <returns></returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(MaterialEffectReader).AssemblyQualifiedName;
        }
    }
}
