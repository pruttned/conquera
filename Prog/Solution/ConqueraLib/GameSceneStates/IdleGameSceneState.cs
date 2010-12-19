using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Input;
using Ale.Gui;
using Microsoft.Xna.Framework;
using Ale;
using Conquera.Gui;

namespace Conquera
{
    public class IdleGameSceneState : IGameSceneState
    {
        private GameScene mScene;

        public IdleGameSceneState(GameScene scene)
        {
            mScene = scene;
        }

        public void OnStart()
        {
            if (IsSelectedUnitReady())
            {
                mScene.State = mScene.States[typeof(ReadyGameUnitSelectedGameSceneState)];
            }
            else
            {
                Ale.Gui.GuiManager.Instance.Cursor = AlCursors.Default;
                mScene.EnableMouseCameraControl = true;
            }
        }

        public void OnEnd()
        {
        }

        public void OnClickOnCell(HexCell cellUnderCur, MouseButton button)
        {
            if (MouseButton.Left == button)
            {
                mScene.SelectedCell = cellUnderCur;
                if (IsSelectedUnitReady())
                {
                    mScene.State = mScene.States[typeof(ReadyGameUnitSelectedGameSceneState)];
                }
            }
        }

        public void Update(AleGameTime gameTime)
        {
            GameCamera camera = mScene.GameCamera;

            var cellUnderCur = mScene.GetCellUnderCur();
            if (null != cellUnderCur)
            {
                Point index = cellUnderCur.Index;
                // mCursor3d.Position = cellUnderCur.CenterPos;
                // mCursor3d.IsVisible = true;

                if (mScene.SceneManager.MouseManager.IsButtonDown(MouseButton.Left))
                {
                    if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
                    {
                        //Terrain.SetTile(index, null);
                        mScene.SetCellOwner(index, null);
                    }
                    else
                    {
                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F))
                        {
                            mScene.GetCell(index).SetTile(null);
                        }
                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C))
                        {
                            mScene.GetCell(index).SetTile("CastleTile");
                        }
                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.G))
                        {
                            mScene.GetCell(index).SetTile("Grass1Tile");
                        }
                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.M))
                        {
                            mScene.GetCell(index).SetTile("GoldMineLv1Tile");
                        }
                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                        {
                            mScene.GetCell(index).SetTile("Dirt1Tile");
                        }
                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
                        {
                            mScene.GetCell(index).SetTile(null);
                        }
                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.H))
                        {
                            mScene.GetCell(index).SetTile("MountainsTile");
                        }

                        //  Terrain.SetTile(index, "Grass1Tile");
                        // mHexTerrain.SetTile(index.X, index.Y, "Grass2Tile");
                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1))
                        {
                            if (cellUnderCur.IsPassable)
                            {
                                mScene.SetCellOwner(index, mScene.CurrentPlayer);
                            }
                        }

                        if (mScene.SceneManager.KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V))
                        {
                            camera.MoveCameraToCell(cellUnderCur);
                        }
                    }
                }
            }
            else
            {
                //    mCursor3d.IsVisible = false;
            }




            //}
        }

        private bool IsSelectedUnitReady()
        {
            var selectedUnit = mScene.SelectedUnit;
            return null != selectedUnit && (!selectedUnit.HasMovedThisTurn || !selectedUnit.HasAttackedThisTurn) && selectedUnit.OwningPlayer == mScene.CurrentPlayer;
        }
    }

}
