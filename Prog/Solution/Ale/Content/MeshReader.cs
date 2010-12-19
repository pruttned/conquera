using Microsoft.Xna.Framework.Content;
using Ale.Graphics;

namespace Ale.Content
{
    /// <summary>
    /// Reads the Mesh from xnb in the content pipeline
    /// </summary>
    public class MeshReader : ContentTypeReader<Mesh>
    {
        /// <summary>
        /// Reads the mesh (see ContentTypeReader)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="existingInstance"></param>
        /// <returns></returns>
        protected override Mesh Read(ContentReader input, Mesh existingInstance)
        {
            return new Mesh(input);
        }
    }
}
