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
    public class ParticleSystemScene : OctreeScene
    {
        class MainScenePass : ScenePass
        {
            public MainScenePass(BaseScene scene, ICamera camera)
                : base("Default", scene, camera, false)
            {
            }
        }


        Vector3LinearAnimator mPosAnim = new Vector3LinearAnimator();
        Vector3[] mPosKeyFrames = new Vector3[]
            {
                new Vector3(-1,-1,0),
                new Vector3(1,-1,0),
                new Vector3(1,1,0),
                new Vector3(-1,1,0)
            };
        int mKeyFrame = 0;

        float mAnimSpeed = 0.0f;

        ParticleSystem mParticleSystem;

        public ParticleSystemScene(SceneManager sceneManager, ContentGroup content)
            : base(sceneManager, content, new BoundingBox(new Vector3(-100, -100, -100), new Vector3(100, 100, 100)))
        {
            //GraphicModel dom = new GraphicModel(Content.Load<GraphicModelDesc>("domGm"), Content);
            //mGraphicModel = new GraphicModel(Content.Load<Mesh>("Sphere"), Content.Load<Material>("DomMat"));
            //            GraphicModel gm = new GraphicModel(Content.Load<Mesh>("Sphere"), new Material(null, Content));
            //gm.SetMaterials(
            //Octree.AddObject(mGraphicModel);
            

        }

        protected override List<ScenePass> CreateScenePasses(GraphicsDeviceManager graphicsDeviceManager, IRenderTargetManager renderTargetManager, ContentGroup content)
        {
            Camera mainCamera = new Camera(Vector3.Zero, 100, new Vector2(-0.8f, 0), 20000, 3, 1.55f, -1.57f);
            mainCamera.DistanceToTarget = 10;

            //return null;
            List<ScenePass> scenePasses = new List<ScenePass>();
            scenePasses.Add(new MainScenePass(this, mainCamera));

            //scenePasses[0].RenderTarget.Clear(Color.White);

            //scenePasses[0].IsEnabled = false;

            return scenePasses;
        }

        public override void Update(AleGameTime gameTime)
        {
            if (!mPosAnim.Update(gameTime))
            {
                Vector3 p1 = mPosKeyFrames[mKeyFrame];
                mKeyFrame++;
                if (mKeyFrame >= mPosKeyFrames.Length)
                {
                    mKeyFrame = 0;
                }
                mPosAnim.Animate(mAnimSpeed, p1, mPosKeyFrames[mKeyFrame]);
            }
            else
            {
                mParticleSystem.Position = mPosAnim.CurrentValue;
            }

            Camera camera = (Camera)MainCamera;

            Vector2 curPos = SceneManager.MouseManager.CursorPosition;
            Vector3 mouseMovement = SceneManager.MouseManager.CursorPositionDelta;
            if (SceneManager.MouseManager.IsButtonDown(MouseButton.Left)) // zoom
            {
                camera.DistanceToTarget += mouseMovement.Y / 1.0f;

                //  MouseManager.ClipRealCursor = true;
            }
            else
            {
                if (SceneManager.MouseManager.IsButtonDown(MouseButton.Right))
                {//movement
                    camera.RotationArroundTarget -= new Vector2(mouseMovement.Y / 200.0f, mouseMovement.X / 200.0f);
                }
            }
        }

        internal void SetParticleSystemSettings(ParticleSystemSettings settings)
        {
            if (null != mParticleSystem)
            {
                Octree.DestroyObject(mParticleSystem);
                mParticleSystem.Position = mPosKeyFrames[0];
                mKeyFrame = 1;
                mParticleSystem = null;
                mPosAnim.Animate(mAnimSpeed, mPosKeyFrames[0], mPosKeyFrames[1]);
            }

            mParticleSystem = ParticleSystemManager.CreateParticleSystem(new ParticleSystemDesc(this.SceneManager.GraphicsDeviceManager.GraphicsDevice,
                settings, Content));

            mParticleSystem.ShowWorldBounds = true;
            Octree.AddObject(mParticleSystem);
        }

        protected override void UpdateSoundListener(Ale.Sound.SoundManager SoundManager)
        {
        }


    }

}
