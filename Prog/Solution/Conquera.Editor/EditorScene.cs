using System;
using System.Collections.Generic;
using System.Text;
using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale;
using Ale.Content;
using Ale.Input;
using Ale.Settings;

namespace Conquera.Editor
{
    class EditorScene : BaseScene
    {
        GameSettings mGameSettings;

        private GameScene mGameScene;
        public GameScene GameScene 
        { 
            get {return mGameScene;}
            set 
            {
                if (null == value) throw new ArgumentNullException("GameScene");

                if (value != mGameScene)
                {
                    if (null != mGameScene)
                    {
                        mGameScene.Dispose();
                    }
                    mGameScene = value;
                }
            }
        }
        protected GameCamera GameCamera
        {
            get { return GameScene.GameCamera; }
        }

        public EditorScene(GameScene gameScene)
            : base(gameScene.SceneManager, gameScene.Content)
        {
            GameScene = gameScene;
            mGameSettings = AppSettingsManager.Default.GetSettings<GameSettings>();
        }

        public override void Draw(AleGameTime gameTime)
        {
            GameScene.Draw(gameTime);
        }
        public override void Update(AleGameTime gameTime)
        {
            GameScene.Update3dCursor();

            HandleCamera();
        }

        protected override List<ScenePass> CreateScenePasses(GraphicsDeviceManager graphicsDeviceManager, RenderTargetManager renderTargetManager, ContentGroup content)
        {
            //dummy
            Camera mainCamera = new Camera(Vector3.Zero, 10, new Vector2(-1.1f, 0), 20, 3, 1.55f, -1.57f);
            List<ScenePass> scenePasses = new List<ScenePass>();
            scenePasses.Add(new GameDefaultScenePass(this, mainCamera));

            return scenePasses;
        }

        protected override void UpdateSoundListener(Ale.Sound.SoundManager SoundManager)
        {
        }


        private void HandleCamera()
        {
            Vector3 mouseMovement = SceneManager.MouseManager.CursorPositionDelta;
            //zoom
            if (Math.Abs(mouseMovement.Z) > 0.00001f)
            {
                if (mouseMovement.Z > 0)
                {
                    GameCamera.IncZoomLevel(false);
                }
                else
                {
                    GameCamera.DecZoomLevel(false);
                }
            }
            else
            {
                if (SceneManager.MouseManager.IsButtonDown(MouseButton.Middle))
                {//movement
                    float scrollSpeed = mGameSettings.CameraScrollSpeed;
                    GameScene.PanCamera(new Vector2(mouseMovement.X * scrollSpeed, mouseMovement.Y * scrollSpeed));
                }
            }
        }
    }
}
