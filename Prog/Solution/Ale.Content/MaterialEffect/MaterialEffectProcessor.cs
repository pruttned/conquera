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
