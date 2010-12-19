using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Content;
using Microsoft.Xna.Framework.Graphics;
using Ale.Graphics;
using Ale.Gui;
using Ale.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Ale.Tools;

namespace Ale.Editor
{
    public class DefaultScene : OctreeScene
    {
        Renderer mRenderer = new Renderer();
        PostProcessEffectManager mPostProcessEffectManager;

        AleContentManager mContentManager;

        public DefaultScene(SceneManager sceneManager, ContentGroup content)
            :base(sceneManager, content, new BoundingBox(new Vector3(-100, -100, -100), new Vector3(100, 100, 100)))
        {
            mContentManager = content.ParentContentManager;




            //GraphicModel dom = new GraphicModel(Content.Load<GraphicModelDesc>("domGm"), Content);
            GraphicModel gm = new GraphicModel(Content.Load<Mesh>("Sphere"), Content.Load<Material>("DomMat"));
//            GraphicModel gm = new GraphicModel(Content.Load<Mesh>("Sphere"), new Material(null, Content));
            //gm.SetMaterials(
            Octree.AddObject(gm);
            //gm.SetMaterials(new Material(
        }

        protected override List<ScenePass> CreateScenePasses(GraphicsDeviceManager graphicsDeviceManager, RenderTargetManager renderTargetManager, ContentGroup content)
        {
            Camera mainCamera = new Camera(Vector3.Zero, 100, new Vector2(-0.8f, 0), 20000, 3, 1.55f, -1.57f);
            mainCamera.DistanceToTarget = 10;

            //return null;
            List<ScenePass> scenePasses = new List<ScenePass>();
            scenePasses.Add(new ScenePass("Default", this, mainCamera, null));

            //scenePasses[0].RenderTarget.Clear(Color.White);

            //scenePasses[0].IsEnabled = false;

            return scenePasses;
        }

        public override void Update(AleGameTime gameTime)
        {
            Camera camera = (Camera)MainCamera;

            Vector2 curPos = SceneManager.MouseManager.CursorPosition;
            Vector3 mouseMovement = SceneManager.MouseManager.CursorPositionDelta;
            if (SceneManager.MouseManager.IsButtonDown(MouseButton.Right)) // zoom
            {
                camera.DistanceToTarget += mouseMovement.Y / 1.0f;

                //  MouseManager.ClipRealCursor = true;
            }
            else
            {
                if (SceneManager.MouseManager.IsButtonDown(MouseButton.Middle))
                {//movement
                    camera.RotationArroundTarget -= new Vector2(mouseMovement.Y / 200.0f, mouseMovement.X / 200.0f);
                }
            }
        }

        protected override void UpdateSoundListener(Ale.Sound.SoundManager SoundManager)
        {
        }
    }
}
