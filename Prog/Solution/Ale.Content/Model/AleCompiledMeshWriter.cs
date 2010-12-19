using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace Ale.Content
{
    /// <summary>
    /// ContentTypeWriter for MaterialEffect
    /// </summary>
    [ContentTypeWriter]
    public class AleCompiledMeshWriter : ContentTypeWriter<AleCompiledMesh>
    {
        /// <summary>
        /// See ContentTypeReader
        /// </summary>
        /// <param name="output"></param>
        /// <param name="value"></param>
        protected override void Write(ContentWriter output, AleCompiledMesh value)
        {
            value.Write(output);
        }

        /// <summary>
        /// See ContentTypeReader
        /// </summary>
        /// <param name="targetPlatform"></param>
        /// <returns></returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(MeshReader).AssemblyQualifiedName;
        }
    }
}
