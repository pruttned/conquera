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

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Ale.Content
{
    /// <summary>
    /// ContentProcessor for MaterialEffect
    /// </summary>
    [ContentProcessor(DisplayName = "MaterialEffect - Ale")]
    public class MaterialEffectProcessor : ContentProcessor<EffectContent, CompiledMaterialEffect>
    {
        /// <summary>
        /// See ContentProcessor
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override CompiledMaterialEffect Process(EffectContent input, ContentProcessorContext context)
        {
            return new CompiledMaterialEffect((new EffectProcessor()).Process(input, context));
        }
    }
}
