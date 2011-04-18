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
    public enum EditMode
    {
        TileEdit,
        RegionEdit,
        UnitEdit
    }

    class EditorScene : BaseScene
    {
        GameSettings mGameSettings;
        private GameScene mGameScene;
        private TileBrush mTileBrush;
        private HexCell mLastSetTileCell = null;

        public EditMode EditMode { get; set; }
        public string UnitType { get; set; }
        public GamePlayer Player { get; set; }


        public IList<GamePlayer> Players { get { return mGameScene.GameSceneContextState.Players; } }


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

        public TileBrush TileBrush
        {
            get { return mTileBrush; }
            set
            {
                mTileBrush = value;
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
            EditMode = EditMode.TileEdit;

            Player = GameScene.CurrentPlayer;
        }

        public override void Draw(AleGameTime gameTime)
        {
            GameScene.Draw(gameTime);
        }

        public override void Update(AleGameTime gameTime)
        {
            GameScene.Update3dCursor();
            HandleCamera();

            var cellUnderCur = mGameScene.GetCellUnderCur();

            if (null != cellUnderCur)
            {
                var cellUnderCurIndex = cellUnderCur.Index;
                switch (EditMode)
                {
                    case EditMode.TileEdit:
                        if (SceneManager.MouseManager.IsButtonDown(MouseButton.Left))
                        {
                            if (null == TileBrush)
                            {
                                ClearCell(cellUnderCur); 
                                cellUnderCur.SetTile(null);
                            }
                            else
                            {
                                if (cellUnderCur.IsGap || mLastSetTileCell != cellUnderCur || !string.Equals(cellUnderCur.HexTerrainTile.DisplayName, TileBrush.Name))
                                {
                                    string tile = TileBrush.GetTile();
                                    if (!Content.Load<HexTerrainTileDesc>(tile).IsPassable)
                                    {
                                        ClearCell(cellUnderCur);
                                    }

                                    cellUnderCur.SetTile(tile);
                                    mLastSetTileCell = cellUnderCur;
                                }
                            }
                        }
                        else
                        {
                            if (SceneManager.MouseManager.IsButtonDown(MouseButton.Right))
                            {
                                ClearCell(cellUnderCur); 
                                cellUnderCur.SetTile(null);
                            }
                        }

                        break;
                    case EditMode.RegionEdit:
                        if (SceneManager.MouseManager.IsButtonDown(MouseButton.Left))
                        {
                            if (!cellUnderCur.IsGap && cellUnderCur.IsPassable)
                            {
                                GameScene.SetCellOwner(cellUnderCurIndex, Player);
                            }
                        }
                        else
                        {
                            if (SceneManager.MouseManager.IsButtonDown(MouseButton.Right))
                            {
                                GameScene.SetCellOwner(cellUnderCurIndex, null);
                            }
                        }
                        break;
                    case EditMode.UnitEdit:
                        if (SceneManager.MouseManager.IsButtonDown(MouseButton.Left))
                        {
                            if (null == cellUnderCur.GameUnit && !cellUnderCur.IsGap && cellUnderCur.IsPassable)
                            {
                                GameScene.AddGameUnit(Player, UnitType, cellUnderCurIndex);
                            }
                        }
                        else
                        {
                            if (SceneManager.MouseManager.IsButtonDown(MouseButton.Right))
                            {
                                if (null != cellUnderCur.GameUnit)
                                {
                                    GameScene.RemoveUnit(cellUnderCur.GameUnit);
                                }
                            }
                        }
                        break;
                }

            }
        }

        private void ClearCell(HexCell cellUnderCur)
        {
            var cellUnderCurIndex = cellUnderCur.Index;
            GameScene.SetCellOwner(cellUnderCurIndex, null);
            if (null != cellUnderCur.GameUnit)
            {
                GameScene.RemoveUnit(cellUnderCur.GameUnit);
            }
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
