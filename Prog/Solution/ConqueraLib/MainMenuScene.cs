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

using Ale.Graphics;
using Microsoft.Xna.Framework;
using Ale.Input;
using System.Collections.Generic;
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics;
using Ale.Settings;
using Ale.Content;
using System;
using Ale.Gui;
using SimpleOrmFramework;
using Ale.Sound;
using System.IO;
using Ale.Scene;
using Conquera.Gui;
using Ale;


namespace Conquera
{
    public class MainMenuScene : OctreeScene
    {
        private static readonly string FireBallPsys = "FireBallPsys";
        private static float FireBallPsysSpeed = 2.5f;

        private Vector3 mLightDir = new Vector3(-0.3333333f, -0.5f, 1f);
        private List<ParticleSystemMissile> mMissiles = new List<ParticleSystemMissile>();
        private MainMenuGuiScene mGuiScene;

        public MainMenuScene(SceneManager sceneManager, ContentGroup content)
            : base(sceneManager, content, new BoundingBox(new Vector3(-100,-100,-100), new Vector3(100,100,100)))
        {
            SceneManager.KeyboardManager.KeyDown += new KeyboardManager.KeyEventHandler(KeyboardManager_KeyDown);
            SceneManager.MouseManager.MouseButtonUp += new MouseManager.MouseButtonEventHandler(MouseManager_MouseButtonUp);
            SceneManager.MouseManager.MouseButtonDown += new MouseManager.MouseButtonEventHandler(MouseManager_MouseButtonDown);

            for (int i = 0; i < 20; ++i)
            {
                mMissiles.Add(CreateMissile());
            }

            mGuiScene = new MainMenuGuiScene(this);
        }

        public override void Update(AleGameTime gameTime)
        {
            base.Update(gameTime);
            GuiManager.Instance.Update(gameTime);

            for (int i = 0; i < mMissiles.Count; ++i)
            {
                if (!mMissiles[i].Update(gameTime))
                {
                    mMissiles[i].Dispose();
                    mMissiles[i] = CreateMissile();
                }
            }
        }

        public override void Draw(AleGameTime gameTime)
        {
            base.Draw(gameTime);            
            GuiManager.Instance.Draw(gameTime);
        }

        protected override List<ScenePass> CreateScenePasses(GraphicsDeviceManager graphicsDeviceManager, RenderTargetManager renderTargetManager, ContentGroup content)
        {
            Camera mainCamera = new Camera(Vector3.Zero,10, new Vector2(-1.1f, 0), 20, 3, 1.55f, -1.57f);

            List<ScenePass> scenePasses = new List<ScenePass>();

            scenePasses.Add(new ShadowScenePass(mainCamera, this, mLightDir, new Plane(Vector3.UnitZ, HexTerrain.GroundHeight), renderTargetManager, content));
            scenePasses[0].RenderTarget.Clear(Color.White);

            Material skyPlaneMaterial = new Material(content.Load<MaterialEffect>("SkyPlaneFx"), 0);
            skyPlaneMaterial.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("Sky"));
            skyPlaneMaterial.Techniques["SkyPlaneScenePass"].Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("Sky"));
            scenePasses.Add(new SkyPlaneScenePass(mainCamera, this, content, skyPlaneMaterial));

            scenePasses.Add(new GameDefaultScenePass(this, mainCamera));

            return scenePasses;
        }

        protected override void UpdateSoundListener(SoundManager SoundManager)
        {
            Camera mainCamera = (Camera)MainCamera;

            Vector3 forward = mainCamera.TargetWorldPosition - mainCamera.WorldPosition;
            forward.Normalize();

            //todo - which is better??
            SoundManager.ListenerPosition = mainCamera.TargetWorldPosition;
            //            SoundManager.ListenerPosition = mainCamera.WorldPosition;

            SoundManager.ListenerUp = MainCamera.CameraUp;
            SoundManager.ListenerForward = forward;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                SceneManager.KeyboardManager.KeyDown -= KeyboardManager_KeyDown;

                for (int i = 0; i < mMissiles.Count; ++i)
                {
                    mMissiles[i].Dispose();
                }
            }
            base.Dispose(isDisposing);
        }

        protected override void OnActivatedImpl()
        {            
            GuiManager.Instance.ActiveScene = mGuiScene;
        }

        protected override void OnDeactivateImpl()
        {
            GuiManager.Instance.ActiveScene = DefaultGuiScene.Instance;
        }

        private ParticleSystemMissile CreateMissile()
        {
            Vector3 src = new Vector3(0.2f, 0.4f, 8);
            Vector3 srcVar = new Vector3(5, 5, 6);
            Vector3 dest = new Vector3(0.2f, 0.4f, -6);
            Vector3 destVar = new Vector3(8, 5, 0);

            return new ParticleSystemMissile(this, AleMathUtils.GetRandomVector3(ref src, ref srcVar), AleMathUtils.GetRandomVector3(ref dest, ref destVar), FireBallPsys, FireBallPsysSpeed);
        }

        private void KeyboardManager_KeyDown(Microsoft.Xna.Framework.Input.Keys key, KeyboardManager keyboardManager)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                var maps = HotseatGameScene.QueryMapFiles(Content);

                //var map = new HotseatGameScene("TestMap", SceneManager, 10,10, "Grass1Tile", Content);
                var map = HotseatGameScene.Load(maps[0], SceneManager, Content);
                SceneManager.ActivateScene(map);
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.Escape)
            {
                SceneManager.ExitApplication();
            }
        }

        private void MouseManager_MouseButtonUp(MouseButton button, MouseManager mouseManager)
        {
            GuiManager.Instance.HandleMouseUp(button);
        }

        private void MouseManager_MouseButtonDown(MouseButton button, MouseManager mouseManager)
        {
            GuiManager.Instance.HandleMouseDown(button);
        }
    }
}
