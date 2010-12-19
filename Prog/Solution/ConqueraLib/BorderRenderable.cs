using System;
using System.Collections.Generic;
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ale.Tools;

namespace Conquera
{
    internal class BorderRenderable : GraphicModel
    {
        const float LineZPos = 0.02f;
        const float LineThickness =0.7f;
        private bool mIsDisposed = false;

        internal BorderRenderable(GraphicsDevice graphicsDevice ,IList<Vector2> points, Material material)
            : base(BuildBorderGm(graphicsDevice, points), material) 
        {
        }

        protected override void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    Mesh.Dispose();
                }
                mIsDisposed = true;
            }

            base.Dispose(isDisposing);
        }

        private static Mesh BuildBorderGm(GraphicsDevice graphicsDevice, IList<Vector2> points)
        {
            if (points.Count < 3) throw new ArgumentException("You must specify at least 3 points");

            MeshBuilder mb = new MeshBuilder(graphicsDevice);
            mb.SetCurrentSubMesh("m");

            Vector2 prevP1p2Perp;
            int prevV1;
            int prevV2;
            int firstV1;
            int firstV2;

            Vector2 lastP1p2Perp;
            SimpleVertex vertexUv0 = new SimpleVertex(Vector3.Zero, Vector3.Zero, new Vector2(0.0f, 0.0f));
            SimpleVertex vertexUv1 = new SimpleVertex(Vector3.Zero, Vector3.Zero, new Vector2(0.0f, 1.0f));

            {//last to first
                Vector2 p1p2Last = points[points.Count - 1] - points[points.Count - 2];
                p1p2Last.Normalize();

                Vector2 p1 = points[points.Count - 1];
                Vector2 p2 = points[0];
                Vector2 p1p2 = p2 - p1;
                p1p2.Normalize();
                lastP1p2Perp = prevP1p2Perp = AleMathUtils.GetPerpVector(p1p2);
                Vector2 p1p2Perp = (prevP1p2Perp + AleMathUtils.GetPerpVector(p1p2Last));
                p1p2Perp.Normalize();

                vertexUv0.Position = new Vector3(p1.X, p1.Y, LineZPos);
                vertexUv1.Position = new Vector3(p1.X - (p1p2Perp.X * LineThickness), p1.Y - (p1p2Perp.Y * LineThickness), LineZPos);

                firstV1 = prevV1 = mb.AddVertex(ref vertexUv0);
                firstV2 = prevV2 = mb.AddVertex(ref vertexUv1);
            }
            for (int i = 0; i < points.Count - 1; ++i)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[i + 1];
                Vector2 p1p2 = p2 - p1;
                p1p2.Normalize();
                Vector2 p1p2PerpReal = AleMathUtils.GetPerpVector(p1p2); ;
                Vector2 p1p2Perp = p1p2PerpReal + prevP1p2Perp;
                p1p2Perp.Normalize();

                vertexUv0.Position = new Vector3(p1.X, p1.Y, LineZPos);
                vertexUv1.Position = new Vector3(p1.X - (p1p2Perp.X * LineThickness), p1.Y - (p1p2Perp.Y * LineThickness), LineZPos);

                int v1 = mb.AddVertex(ref vertexUv0);
                int v2 = mb.AddVertex(ref vertexUv1);

                mb.AddFace(v1, prevV1, prevV2);
                mb.AddFace(v2, v1, prevV2);

                prevV1 = v1;
                prevV2 = v2;
                prevP1p2Perp = p1p2PerpReal;
            }
            { //last
                Vector2 p1 = points[points.Count - 1];
                Vector2 p1p2Perp = lastP1p2Perp + prevP1p2Perp;
                p1p2Perp.Normalize();

                vertexUv0.Position = new Vector3(p1.X, p1.Y, LineZPos);
                vertexUv1.Position = new Vector3(p1.X - (p1p2Perp.X * LineThickness), p1.Y - (p1p2Perp.Y * LineThickness), LineZPos);

                int v1 = mb.AddVertex(ref vertexUv0);
                int v2 = mb.AddVertex(ref vertexUv1);

                mb.AddFace(v1, prevV1, prevV2);
                mb.AddFace(v2, v1, prevV2);
            }

            return mb.BuildMesh(true);
        }
    }
}
