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
