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
    public class GameScene : OctreeScene
    {
        //TODO !!! Doplnit co treba do Dispose

        //promoted collections
        private static HashSet<HexRegion> NewRegions = new HashSet<HexRegion>();
        private static List<HexCell> Siblings = new List<HexCell>(6);

        private GameSceneSettings mSettings;
        private Vector3 mLightDir = new Vector3(-0.3333333f, -0.5f, 1f);
        private HexCell[,] mCells;
        private GameUnit mSelectedUnit = null;
        private ParticleSystemDesc mCaptureParticleSystemDesc;
        private ParticleSystemDesc mUnitDeathParticleSystemDesc;

        private GraphicModel mCursor3d;
        private GraphicModel mCursor3dCellSel;
        private MovementArrow mMovementArrow;

        private HexCell mSelectedCell = null;

        SoundEmitter mSoundEmitter = new SoundEmitter();
        private GameGuiScene mGuiScene = new GameGuiScene();

        private IGameSceneState mState;

        private CellLabelManager mCellLabelManager;

        public string Name { get; private set; }

        public GameSceneContextState GameSceneContextState { get; private set; }

        public IGameSceneState State
        {
            get { return mState; }
            set
            {
                if (null == value) { throw new ArgumentNullException("State"); }

                //refresh selected cell to update everything
                RefreshSelectedCell();

                if (mState != value)
                {
                    if (null != mState)
                    {
                        mState.OnEnd();
                    }
                    mState = value;
                    mState.OnStart();
                }
            }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return SceneManager.GraphicsDeviceManager; }
        }

        public MovementArrow MovementArrow
        {
            get { return mMovementArrow; }
        }

        public HexTerrain Terrain { get; private set; }

        public GamePlayer CurrentPlayer
        {
            get { return GameSceneContextState.CurrentPlayer; }
        }

        public GameCamera GameCamera
        {
            get { return (GameCamera)MainCamera; }
        }

        public HexCell SelectedCell
        {
            get { return mSelectedCell; }
            set
            {
                if (mSelectedCell != value)
                {
                    mSelectedCell = value;
                    RefreshSelectedCell();
                }
            }
        }

        public GameUnit SelectedUnit
        {
            get { return mSelectedUnit; }
            private set
            {
                //if (mSelectedUnit != value)
                //{
                mSelectedUnit = value;
                //}
            }
        }

        public bool EnableMouseCameraControl { get; set; }

        public GameScene(string name, SceneManager sceneManager, int width, int height, string defaultTile, ContentGroup content)
            : base(sceneManager, content, GetBoundsFromSize(width, height))
        {
            Name = name;

            Terrain = new HexTerrain(width, height, defaultTile, this);
            mSettings = new GameSceneSettings();
            GameSceneContextState = new GameSceneContextState();
            GameSceneContextState.GameMap = name;

            GameSceneContextState.Players.Add(new HumanPlayer(Color.Blue.ToVector3()));
            GameSceneContextState.Players.Add(new HumanPlayer(Color.Red.ToVector3()));
            GameSceneContextState.Players[0].Gold = 1000;
            GameSceneContextState.Players[1].Gold = 1000;

            Init();
        }

        public static GameScene Load(string mapName, SceneManager sceneManager, ContentGroup content)
        {
            string mapFile = GetMapFileName(content, mapName);

            if (!File.Exists(mapFile))
            {
                throw new ArgumentException(string.Format("Map '{0}' doesn't exists", mapFile));
            }

            GameScene scene;

            using (OrmManager ormManager = new OrmManager(OrmManager.CreateDefaultConnectionString(mapFile)))
            {
                long settingsId = ormManager.FindObject(typeof(GameSceneSettings), "Key=1");
                if (0 >= settingsId)
                {
                    throw new ArgumentException(string.Format("Map file '{0}' is corrupted", mapFile));
                }
                var settings = ormManager.LoadObject<GameSceneSettings>(settingsId);
                var terrain = ormManager.LoadObject<HexTerrain>(settings.TerrainId);
                var gameSceneState = ormManager.LoadObject<GameSceneContextState>(ormManager.FindObject(typeof(GameSceneContextState), "Key=1"));

                scene = new GameScene(sceneManager, content, ormManager, settings, terrain, gameSceneState);
            }

            return scene;
        }

        public IGameSceneState GetGameSceneState(string name)
        {
            return CurrentPlayer.GetGameSceneState(name);
        }

        public void Save()
        {
            string mapDir = GetMapDirName();
            string mapFile = GetMapFileName();

            if (!Directory.Exists(mapDir))
            {
                Directory.CreateDirectory(mapDir);
            }

            if (0 >= mSettings.Id) //not loaded
            {
                File.Delete(mapFile);
            }

            using (OrmManager ormManager = new OrmManager(OrmManager.CreateDefaultConnectionString(mapFile)))
            {
                using (SofTransaction transaction = ormManager.BeginTransaction())
                {
                    mSettings.TerrainId = ormManager.SaveObject(Terrain);
                    ormManager.SaveObject(mSettings);
                    ormManager.SaveObject(GameSceneContextState);


                    // cells ownership
                    Dictionary<GamePlayer, byte> playerNums = new Dictionary<GamePlayer, byte>();
                    for (byte i = 0; i < GameSceneContextState.Players.Count; ++i)
                    {
                        playerNums.Add(GameSceneContextState.Players[i], i);
                    }

                    byte[] data = new byte[Terrain.Width * Terrain.Height];
                    int k = 0;
                    for (int i = 0; i < Terrain.Width; ++i)
                    {
                        for (int j = 0; j < Terrain.Height; ++j)
                        {
                            var player = mCells[i, j].OwningPlayer;
                            data[k++] = (null != player ? playerNums[player] : (byte)255);
                        }
                    }

                    ormManager.SetBlobData("CellsOwnership", data);

                    transaction.Commit();
                }
            }
        }

        public GameUnit AddGameUnit(GamePlayer gamePlayer, string desc, Point index)
        {
            var cell = GetCell(index);
            if (null != cell.GameUnit)
            {
                throw new ArgumentException(string.Format("Cell {0} already contains a game unit", index));
            }

            SetCellOwner(index, gamePlayer);
            long descId = Content.ParentContentManager.OrmManager.FindObject(typeof(GameUnitSttings), string.Format("Name='{0}'", desc));
            if (0 >= descId)
            {
                throw new ArgumentException(string.Format("Unit desc '{0}' doesn't exists", desc));
            }
            GameUnit unit = new GameUnit(descId, gamePlayer);
            unit.CellIndex = index;
            unit.CellIndexChanged += new GameUnit.CellIndexChangedHandler(unit_CellIndexChanged);

            cell.GameUnit = unit;
            gamePlayer.AddGameUnit(unit);
            AddSceneObject(unit);

            return unit;
        }

        internal GameUnit AddGameUnit(GameUnit unit)
        {
            Point index = unit.CellIndex;
            GamePlayer gamePlayer = unit.OwningPlayer;

            var cell = GetCell(index);
            if (null != cell.GameUnit)
            {
                throw new ArgumentException(string.Format("Cell {0} already contains a game unit", index));
            }

            //SetCellOwner(index, gamePlayer); - robi problem s load ... je to tak blbe riesenie  a preto je to internal
            unit.CellIndexChanged += new GameUnit.CellIndexChangedHandler(unit_CellIndexChanged);

            cell.GameUnit = unit;
            AddSceneObject(unit);

            return unit;
        }

        public void EndTurn()
        {
            CurrentPlayer.OnEndTurn();

            GameSceneContextState.EndTurn();

            CurrentPlayer.OnBegineTurn();

            for (int i = 0; i < Terrain.Width; ++i)
            {
                for (int j = 0; j < Terrain.Height; ++j)
                {
                    if (mCells[i, j].BelongsToCurrentPlayer)
                    {
                        mCells[i, j].OnBeginTurn();
                    }
                }
            }
        }


        public override void Update(AleGameTime gameTime)
        {
            base.Update(gameTime);

            State.Update(gameTime);

            if (EnableMouseCameraControl)
            {
                HandleCameraControl();
            }

            var cellUnderCur = GetCellUnderCur();
            //3d cursor
            if (null != cellUnderCur)
            {
                Point index = cellUnderCur.Index;
                mCursor3d.Position = cellUnderCur.CenterPos;
                mCursor3d.IsVisible = true;
            }
            else
            {
                mCursor3d.IsVisible = false;
            }

            mGuiScene.DebugText = State.GetType().ToString();

            //Gui
            GuiManager.Instance.Update(gameTime);
        }

        public override void Draw(AleGameTime gameTime)
        {
            base.Draw(gameTime);
            mCellLabelManager.Draw(gameTime);
            GuiManager.Instance.Draw(gameTime);
        }

        public HexCell GetCellUnderCur()
        {
            Plane plane = new Plane(Vector3.UnitZ, 0);

            Ray ray;
            MainCamera.CameraToViewport(SceneManager.MouseManager.CursorPosition, SceneManager.GraphicsDeviceManager.GraphicsDevice.Viewport, out ray);

            float? intersection = ray.Intersects(plane);
            if (null != intersection)
            {
                Vector3 intPoint = ray.Position + intersection.Value * ray.Direction;

                Point index;
                if (Terrain.GetIndexFromPos(intPoint.X, intPoint.Y, out index))
                {
                    return GetCell(index);
                }
            }

            return null;
        }

        protected internal virtual void HexCellTileChanged(HexCell tile, HexTerrainTileDesc oldDesc)
        {
        }

        public HexCell GetCell(Point index)
        {
            return mCells[index.X, index.Y];
        }

        public void SetCellOwner(Point cellIndex, GamePlayer newOwner)
        {
            HexCell cell = GetCell(cellIndex);
            GamePlayer oldOwner = cell.OwningPlayer;

            if (oldOwner != newOwner)
            {
                HexRegion oldRegion = cell.Region;

                NewRegions.Clear();
                Siblings.Clear();

                cell.GetSiblings(Siblings);
                foreach (HexCell sibling in Siblings)
                {
                    if (null != sibling.Region && (sibling.OwningPlayer == oldOwner || sibling.OwningPlayer == newOwner))
                    {
                        sibling.Region.Dispose();
                    }
                }
                if (null != oldRegion)
                {
                    oldRegion.Dispose();
                }

                HexRegion newRegion;
                if (null != newOwner)
                {
                    newRegion = new HexRegion(newOwner, Octree);
                    NewRegions.Add(newRegion);

                    newRegion.PropagateRegion(cell, mCells, null, GraphicsDeviceManager.GraphicsDevice);
                }
                else
                {
                    cell.NewRegion = null;
                    cell.UpdateRegion();
                    newRegion = null;
                }

                foreach (HexCell sibling in Siblings)
                {
                    if (!NewRegions.Contains(sibling.Region) && null != sibling.OwningPlayer && (sibling.OwningPlayer == oldOwner || sibling.OwningPlayer == newOwner))
                    {
                        HexRegion region = new HexRegion(sibling.OwningPlayer, Octree);
                        region.PropagateRegion(sibling, mCells, null, GraphicsDeviceManager.GraphicsDevice);
                        NewRegions.Add(region);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="icon">Use CellNotificationIcons</param>
        /// <param name="textColor"></param>
        /// <param name="cell"></param>
        public void FireCellNotificationLabel(string text, string icon, Color textColor, Point cell)
        {
            mCellLabelManager.AddLabel(text, icon, textColor, GetCell(cell).CenterPos);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                SceneManager.KeyboardManager.KeyDown -= KeyboardManager_KeyDown;
                Terrain.Dispose();
                mMovementArrow.Dispose();
                mCellLabelManager.Dispose();

                GameCamera.Dispose();
            }
            base.Dispose(isDisposing);
        }

        protected override List<ScenePass> CreateScenePasses(GraphicsDeviceManager graphicsDeviceManager, RenderTargetManager renderTargetManager, ContentGroup content)
        {
            GameCamera mainCamera = new GameCamera(this);

            //return null;
            List<ScenePass> scenePasses = new List<ScenePass>();

            scenePasses.Add(new ShadowScenePass(mainCamera, this, mLightDir, new Plane(Vector3.UnitZ, HexTerrain.GroundHeight), renderTargetManager, content));
            scenePasses[0].RenderTarget.Clear(Color.White);

            //scenePasses.Add(new WaterReflectionPass(mainCamera, this, renderTargetManager, content));

            //todo - nacitavaj material z content podla mena
            Material skyPlaneMaterial = new Material(content.Load<MaterialEffect>("SkyPlaneFx"), 0);
            skyPlaneMaterial.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("Sky"));
            skyPlaneMaterial.Techniques["SkyPlaneScenePass"].Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("Sky"));
            scenePasses.Add(new SkyPlaneScenePass(mainCamera.RealCamera, this, content, skyPlaneMaterial));

            scenePasses.Add(new GameDefaultScenePass(this, mainCamera));

            //scenePasses[0].IsEnabled = false;

            return scenePasses;
        }

        protected override void UpdateSoundListener(SoundManager SoundManager)
        {
            GameCamera mainCamera = GameCamera;

            Vector3 forward = mainCamera.TargetWorldPosition - mainCamera.WorldPosition;
            forward.Normalize();

            //todo - which is better??
            SoundManager.ListenerPosition = mainCamera.TargetWorldPosition;
            //            SoundManager.ListenerPosition = mainCamera.WorldPosition;

            SoundManager.ListenerUp = MainCamera.CameraUp;
            SoundManager.ListenerForward = forward;
        }

        protected override void OnActivatedImpl()
        {
            GuiManager.Instance.ActiveScene = mGuiScene;
        }

        private GameScene(SceneManager sceneManager, ContentGroup content, OrmManager ormManager, GameSceneSettings settings, HexTerrain terrain, GameSceneContextState gameSceneState)
            : base(sceneManager, content, GetBoundsFromSize(terrain.Width, terrain.Height))
        {
            mSettings = settings;
            Terrain = terrain;
            GameSceneContextState = gameSceneState;

            Name = gameSceneState.GameMap;

            terrain.InitAfterLoad(this, ormManager);

            Init();

            // cells ownership
            byte[] data = ormManager.GetBlobData("CellsOwnership");
            if (data.Length != Terrain.Width * Terrain.Height)
            {
                throw new ArgumentOutOfRangeException("CellsOwnership blob data has a wrong size");
            }
            GamePlayer[,] cellsOwnerships = new GamePlayer[Terrain.Width, Terrain.Height];
            int k = 0;
            for (int i = 0; i < Terrain.Width; ++i)
            {
                for (int j = 0; j < Terrain.Height; ++j)
                {
                    byte owner = data[k++];
                    cellsOwnerships[i, j] = 255 > owner ? GameSceneContextState.Players[owner] : null;
                }
            }
            for (int i = 0; i < Terrain.Width; ++i)
            {
                for (int j = 0; j < Terrain.Height; ++j)
                {
                    HexCell cell = mCells[i, j];
                    GamePlayer owner = cellsOwnerships[i, j];
                    if (null != owner && null == cell.Region)
                    {
                        var newRegion = new HexRegion(owner, Octree);
                        newRegion.PropagateRegion(cell, mCells, cellsOwnerships, GraphicsDeviceManager.GraphicsDevice);
                    }
                }
            }
        }

        private void RefreshSelectedCell()
        {
            if (null == mSelectedCell || mSelectedCell.IsGap)
            {
                mCursor3dCellSel.IsVisible = false;
                SelectedUnit = null;
            }
            else
            {
                mCursor3dCellSel.IsVisible = true;
                mCursor3dCellSel.Position = mSelectedCell.CenterPos;
                SelectedUnit = mSelectedCell.GameUnit;
            }
            mGuiScene.UpdateHexCell(mSelectedCell);
        }

        private int DebugGetRegionCnt()
        {
            HashSet<HexRegion> regions = new HashSet<HexRegion>();
            for (int i = 0; i < Terrain.Width; ++i)
            {
                for (int j = 0; j < Terrain.Height; ++j)
                {
                    HexRegion region = mCells[i, j].Region;
                    if (null != region && !regions.Contains(region))
                    {
                        regions.Add(region);
                    }
                }
            }

            return regions.Count;
        }

        private void HandleCameraControl()
        {
            Vector3 mouseMovement = SceneManager.MouseManager.CursorPositionDelta;

            //zoom
            if (Math.Abs(mouseMovement.Z) > 0.00001f)
            {
                if (mouseMovement.Z > 0)
                {
                    GameCamera.IncZoomLevel();
                }
                else
                {
                    GameCamera.DecZoomLevel();
                }
            }
            else
            {
                if (SceneManager.MouseManager.IsButtonDown(MouseButton.Right))
                {//movement
                    Vector2 dirVec = new Vector2(GameCamera.TargetWorldPosition.X - MainCamera.WorldPosition.X,
                        GameCamera.TargetWorldPosition.Y - MainCamera.WorldPosition.Y);
                    dirVec.Normalize();

                    Vector2 perpDir;
                    AleMathUtils.GetPerpVector(ref dirVec, out perpDir);
                    perpDir *= mouseMovement.X / 10.0f;
                    dirVec *= mouseMovement.Y / 10.0f;

                    GameCamera.TargetWorldPosition += new Vector3(perpDir.X + dirVec.X, perpDir.Y + dirVec.Y, 0);
                }
            }
        }


        private void KeyboardManager_KeyDown(Microsoft.Xna.Framework.Input.Keys key, KeyboardManager keyboardManager)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.S)
            {
                ScenePass shadowPass = GetScenePass(ShadowScenePass.Name);
                shadowPass.IsEnabled = !shadowPass.IsEnabled;
                if (!shadowPass.IsEnabled)
                {
                    shadowPass.RenderTarget.Clear(Color.White);
                }

            }
            if (key == Microsoft.Xna.Framework.Input.Keys.P)
            {
                PostProcessEffectManager.PostProcessEffects[0].Enabled = !PostProcessEffectManager.PostProcessEffects[0].Enabled;
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.X)
            {
                VideoSettings videoSettings = AppSettingsManager.Default.GetSettings<VideoSettings>();

                if (1024 == videoSettings.ScreenWidth)
                {
                    videoSettings.ScreenWidth = 800;
                    videoSettings.ScreenHeight = 600;
                }
                else
                {
                    videoSettings.ScreenWidth = 1024;
                    videoSettings.ScreenHeight = 768;
                }

                AppSettingsManager.Default.CommitSettings(videoSettings);
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.Escape)
            {
                SceneManager.ExitApplication();
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.G)
            {
                mGuiScene.SidePanelsVisible = !mGuiScene.SidePanelsVisible;
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.T)
            {
                mGuiScene.DebugTextVisible = !mGuiScene.DebugTextVisible;
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                EndTurn();
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.F6)
            {
                Save();
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.F1)
            {
                if (null != SelectedUnit)
                {
                    KillUnit(SelectedUnit);
                }
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.U)
            {
                if (null != SelectedCell && null == SelectedCell.GameUnit)
                {
                    AddGameUnit(CurrentPlayer, "GameUnit1", SelectedCell.Index);
                }
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.B)
            {
                if (null != SelectedCell && null == SelectedCell.GameUnit)
                {
                    if (CurrentPlayer.HasEnoughGoldForUnit("GameUnit1"))
                    {
                        CurrentPlayer.BuyUnit("GameUnit1", SelectedCell.Index);
                    }
                    Console.WriteLine(CurrentPlayer.Gold);
                }
            }
        }

        public void KillUnit(GameUnit unit)
        {
            if (null == unit) throw new ArgumentNullException("unit");

            ParticleSystemManager.CreateFireAndforgetParticleSystem(mUnitDeathParticleSystemDesc, unit.Position);
            RemoveUnit(unit);
        }

        public void RemoveUnit(GameUnit unit)
        {
            GetCell(unit.CellIndex).GameUnit = null;
            unit.OwningPlayer.RemoveGameUnit(unit);
            DestroySceneObject(unit);
            RefreshSelectedCell();
        }

        private void MouseManager_MouseButtonUp(MouseButton button, MouseManager mouseManager)
        {
            State.OnClickOnCell(GetCellUnderCur(), button);
        }

        private void unit_CellIndexChanged(GameUnit obj, Point oldValue)
        {
            HexCell cell = GetCell(obj.CellIndex);
            if (cell.OwningPlayer != obj.OwningPlayer) //has captured
            {
                ParticleSystemManager.CreateFireAndforgetParticleSystem(mCaptureParticleSystemDesc, cell.CenterPos);
                SetCellOwner(obj.CellIndex, obj.OwningPlayer);
            }
            GetCell(oldValue).GameUnit = null;
            cell.GameUnit = obj;
        }

        private string GetMapDirName()
        {
            return GetMapDirName(Content);
        }

        private string GetMapFileName()
        {
            return GetMapFileName(Content, Name);
        }

        private static string GetMapFileName(ContentGroup content, string mapName)
        {
            return Path.Combine(GetMapDirName(content), mapName + ".map");
        }

        private static string GetMapDirName(ContentGroup content)
        {
            string modFile = content.ParentContentManager.ModFile;
            return Path.Combine(Path.Combine(Path.GetDirectoryName(modFile), Path.GetFileNameWithoutExtension(modFile)), "Maps");
        }

        private void Init()
        {
            SceneManager.KeyboardManager.KeyDown += new KeyboardManager.KeyEventHandler(KeyboardManager_KeyDown);
            SceneManager.MouseManager.MouseButtonUp += new MouseManager.MouseButtonEventHandler(MouseManager_MouseButtonUp);

            mCells = new HexCell[Terrain.Width, Terrain.Height];

            for (int i = 0; i < Terrain.Width; ++i)
            {
                for (int j = 0; j < Terrain.Height; ++j)
                {
                    mCells[i, j] = new HexCell(this, new Point(i, j));
                }
            }

            foreach (var player in GameSceneContextState.Players)
            {
                player.Init(this, Content);
            }

            PostProcessEffectManager.PostProcessEffects.Add(new BloomProcessEffect(GraphicsDeviceManager, Content));
            PostProcessEffectManager.PostProcessEffects[0].Enabled = true;

            mCaptureParticleSystemDesc = Content.Load<ParticleSystemDesc>("CellCaptureParticleSystem");
            mUnitDeathParticleSystemDesc = Content.Load<ParticleSystemDesc>("UnitDeathParticleSystem");

            mCursor3d = new GraphicModel(Content.Load<GraphicModelDesc>("Cursor3dGm"), Content);
            Octree.AddObject(mCursor3d);

            mCursor3dCellSel = new GraphicModel(Content.Load<GraphicModelDesc>("Cursor3dCellSelGm"), Content);
            Octree.AddObject(mCursor3dCellSel);
            mCursor3dCellSel.IsVisible = false;

            mMovementArrow = new MovementArrow(GraphicsDeviceManager.GraphicsDevice, Content);
            SceneDrawableComponents.Add(mMovementArrow);

            mCellLabelManager = new CellLabelManager(MainCamera, GraphicsDeviceManager, Content);

            RegisterFrameListener(GameCamera);

            State = GetGameSceneState(GameSceneStates.Idle);

            //temp
            mCursor3dCellSel.IsVisible = false;
            mMovementArrow.IsVisible = false;
        }

        private static BoundingBox GetBoundsFromSize(int width, int height)
        {
            var v1 = HexTerrain.GetPosFromIndex(new Point(0, 0));
            v1.X -= 50;
            v1.Y -= 50;
            v1.Z = -100;
            var v2 = HexTerrain.GetPosFromIndex(new Point(width, height));
            v2.X += 50;
            v2.Y += 50;
            v2.Z = 100;
            return new BoundingBox(v1, v2);
        }

    }

    public static class GameSceneStates
    {
        public static readonly string Idle = "Idle";
        public static readonly string UnitMoving = "UnitMoving";
        public static readonly string CameraAnimation = "CameraAnimation";
        public static readonly string VictoryEvaluation = "VictoryEvaluation";
        public static readonly string BeginTurn = "BeginTurn";
        public static readonly string ReadyGameUnitSelected = "ReadyGameUnitSelected";
        public static readonly string Battle = "Battle";
    }
}
