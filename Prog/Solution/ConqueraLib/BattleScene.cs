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
using Ale;

namespace Conquera
{
    public abstract class BattleScene : OctreeScene
    {
        //TODO !!! Doplnit co treba do Dispose
        
        public event EventHandler ActiveSpellChanged;

        private BattleSceneHeader mHeader;
        private Vector3 mLightDir = new Vector3(-0.3333333f, -0.5f, 1f);
        private BattleUnit mSelectedUnit = null;
        private ParticleSystemDesc mCaptureParticleSystemDesc;
        private ParticleSystemDesc mUnitDeathParticleSystemDesc;

        private GameSettings mGameSettings;

        private GraphicModel mCursor3d;
        private GraphicModel mCursor3dCellSel;
        private MovementArrow mMovementArrow;

        private bool mShow3dCursor = true;

        private HexTerrainTile mSelectedTile = null;

        SoundEmitter2d mSoundEmitter = new SoundEmitter2d();        

        private IGameSceneState mState;

        private CellLabelManager mCellLabelManager;
        private SpellSlot mActiveSpell = null;        

        private LavaRenderable mLavaRenderable;

        public string Name 
        {
            get { return mHeader.Name; }
        }

        public bool Show3dCursor
        {
            get { return mShow3dCursor; }
            set 
            {
                mShow3dCursor = value;
                mCursor3d.IsVisible = mShow3dCursor; 
            }
        }

        //todo
        public int TurnNum { get; private set; }

        //public SpellSlotCollection Spells
        //{
        //    get { return mHeader.Spells; }
        //}

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

        public UnitAttackAreaRenderable UnitAttackAreaRenderable { get; private set; }
        public UnitActionAreaRenderable UnitActionAreaRenderable { get; private set; }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return SceneManager.GraphicsDeviceManager; }
        }

        public MovementArrow MovementArrow
        {
            get { return mMovementArrow; }
        }

        public HexTerrain Terrain { get; private set; }

        //temp
        public BattlePlayer CurrentPlayer { get; private set; }

        public GameCamera GameCamera
        {
            get { return (GameCamera)MainCamera; }
        }

        public HexTerrainTile SelectedTile
        {
            get { return mSelectedTile; }
            set
            {
                if (mSelectedTile != value)
                {
                    mSelectedTile = value;                    
                    RefreshSelectedCell();
                }
            }
        }

        public BattleUnit SelectedUnit
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

        //public SpellSlot ActiveSpellSlot
        //{
        //    get { return mActiveSpell; }
        //    set
        //    {
        //        if (value != mActiveSpell)
        //        {
        //            mActiveSpell = value;
        //            EventHelper.RaiseEvent(ActiveSpellChanged, this);
        //        }
        //    }
        //}

        public Viewport Viewport { get; private set; }

        /// <summary>
        /// For map folder name and saves folder name
        /// </summary>
        public abstract string GameType { get; }


        public BattleScene(string name, SceneManager sceneManager, int width, int height, string defaultTile, ContentGroup content)
            : base(sceneManager, content, GetBoundsFromSize(width, height))
        {
            Terrain = new HexTerrain(width, height, defaultTile, this);
            mHeader = CreateGameSettings();
            mHeader.Name = name;
            //mHeader.Spells = new SpellSlotCollection();
            //GameSceneContextState = new GameSceneContextState();
            //GameSceneContextState.GameMap = name;

            CreatePlayers();

            Init();
        }

        public static IList<string> QueryMapFiles(string gameType)
        {
            string mapDir = GetMapDirName(gameType);
            if(!Directory.Exists(mapDir))
            {
                return new string[0];
            }
            List<string> maps = new List<string>();
            foreach (string file in Directory.GetFiles(mapDir, "*.map"))
            {
                // Directory.GetFiles will return all files whose ext starts with "map"
                if (string.Equals(Path.GetExtension(file), ".map", StringComparison.InvariantCultureIgnoreCase))
                {
                    maps.Add(file);
                }
            }
            return maps;
        }

        public static BattleScene Load(string mapName, string gameType, SceneManager sceneManager, ContentGroup content)
        {
            string mapFile = GetMapFileName(mapName, gameType);

            return Load(mapFile, sceneManager, content);
        }

        public static BattleScene Load(string mapFile, SceneManager sceneManager, ContentGroup content)
        {
            if (!File.Exists(mapFile))
            {
                throw new ArgumentException(string.Format("Map '{0}' doesn't exists", mapFile));
            }

            BattleScene scene;

            using (OrmManager ormManager = new OrmManager(OrmManager.CreateDefaultConnectionString(mapFile)))
            {
                var settings = ormManager.LoadObjects<BattleSceneHeader>()[0];
                var terrain = ormManager.LoadObject<HexTerrain>(settings.TerrainId);
                //var gameSceneState = ormManager.LoadObjects<GameSceneContextState>()[0];
                
                scene = settings.CreateScene(sceneManager, content, ormManager, settings, terrain);
            }

            return scene;
        }

        public void ExitToMainMenu()
        {
            SceneManager.ActivateScene(new MainMenuScene(SceneManager, Content));
        }

        public IGameSceneState GetGameSceneState(string name)
        {
            return CurrentPlayer.GetGameSceneState(name);
        }

        public void SaveMap()
        {
            string mapDir = GetMapDirName();
            string mapFile = GetMapFileName();

            if (!Directory.Exists(mapDir))
            {
                Directory.CreateDirectory(mapDir);
            }

            if (0 >= mHeader.Id) //not loaded
            {
                File.Delete(mapFile);
            }

            using (OrmManager ormManager = new OrmManager(OrmManager.CreateDefaultConnectionString(mapFile)))
            {
                using (SofTransaction transaction = ormManager.BeginTransaction())
                {
                    mHeader.TerrainId = ormManager.SaveObject(Terrain);
                    ormManager.SaveObject(mHeader);

                    transaction.Commit();
                }
            }
        }

        public BattleUnit AddGameUnit(BattlePlayer gamePlayer, string desc, Point index, bool isReady)
        {
            throw new NotImplementedException();
            //var tile = Terrain[index];
            //if (null != tile.GameUnit)
            //{
            //    throw new ArgumentException(string.Format("Tile {0} already contains a game unit", index));
            //}

            //tile.OwningPlayer = gamePlayer;
            //long descId = Content.ParentContentManager.OrmManager.FindObject(typeof(GameUnitSettings), string.Format("Name='{0}'", desc));
            //if (0 >= descId)
            //{
            //    throw new ArgumentException(string.Format("Unit desc '{0}' doesn't exists", desc));
            //}
            //BattleUnit unit = new BattleUnit(descId, gamePlayer, isReady);
            //unit.CellIndex = index;
            //unit.CellIndexChanged += new BattleUnit.CellIndexChangedHandler(unit_CellIndexChanged);

            //tile.GameUnit = unit;
            //gamePlayer.AddGameUnit(unit);
            //AddSceneObject(unit);

            //RefreshSelectedCell();

            //if (SelectedUnit == unit && SelectedUnit.OwningPlayer == CurrentPlayer)
            //{
            //    State = GetGameSceneState(GameSceneStates.ReadyGameUnitSelected);
            //}

            //return unit;
        }

        public void EndTurn()
        {
            //todo

            //SelectedTile = null;
            //ActiveSpellSlot = null;
            //BattlePlayer oldPlayer = CurrentPlayer;
            //CurrentPlayer.OnEndTurn();

            //GameSceneContextState.EndTurn();

            //CurrentPlayer.OnBeginTurn();

            ////todo: store capured cells in player?
            //for (int i = 0; i < Terrain.Width; ++i)
            //{
            //    for (int j = 0; j < Terrain.Height; ++j)
            //    {
            //        if (mCells[i, j].BelongsToCurrentPlayer)
            //        {
            //            mCells[i, j].OnBeginTurn();
            //        }
            //    }
            //}
            
            //mGuiScene.HandleEndTurn(oldPlayer);
        }


        public override void Update(AleGameTime gameTime)
        {
            base.Update(gameTime);            
            State.Update(gameTime);

            if (EnableMouseCameraControl)
            {
                HandleCameraControl();
            }

            Update3dCursor();

            //todo
            //mGuiScene.DebugText = State.GetType().ToString();
        }
        public void Update3dCursor()
        {
            var cellUnderCur = GetTileUnderCur();
            //3d cursor
            if (null != cellUnderCur)
            {
                mCursor3d.Position = cellUnderCur.CenterPos;


                if (Show3dCursor)
                {
                    mCursor3d.IsVisible = true;
                }



                Plane plane = new Plane(Vector3.UnitZ, 0);

                Ray ray;
                MainCamera.CameraToViewport(SceneManager.MouseManager.CursorPosition, SceneManager.GraphicsDeviceManager.GraphicsDevice.Viewport, out ray);

                float? intersection = ray.Intersects(plane);
                if (null != intersection)
                {
                    Vector3 intPoint = ray.Position + intersection.Value * ray.Direction;

                    cursorLight.Position = new Vector3(intPoint.X, intPoint.Y, 0.5f);
                }



            }
            else
            {
                mCursor3d.IsVisible = false;
            }
        }

        public override void Draw(AleGameTime gameTime)
        {
            base.Draw(gameTime);
            mCellLabelManager.Draw(gameTime);            
            

        }

        public HexTerrainTile GetTileUnderCur()
        {
            Plane plane = new Plane(Vector3.UnitZ, 0);

            Ray ray;
            MainCamera.CameraToViewport(SceneManager.MouseManager.CursorPosition, SceneManager.GraphicsDeviceManager.GraphicsDevice.Viewport, out ray);

            float? intersection = ray.Intersects(plane);
            if (null != intersection)
            {
                Vector3 intPoint = ray.Position + intersection.Value * ray.Direction;

                Point index;
                if (HexHelper.GetIndexFromPos(new Vector2(intPoint.X, intPoint.Y), Terrain.Width, Terrain.Height, out index))
                {
                    return Terrain[index];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="icon">Use CellNotificationIcons</param>
        /// <param name="textColor"></param>
        /// <param name="cell"></param>
        public void FireTileNotificationLabel(string text, ImageDrawing icon, Color textColor, Point tileIndex)
        {
            mCellLabelManager.AddLabel(text, icon, textColor, Terrain[tileIndex].CenterPos);
        }

        public void FireCellNotificationLabel(string text, ImageDrawing icon, Color textColor, Vector3 pos)
        {
            mCellLabelManager.AddLabel(text, icon, textColor, pos);
        }

        public virtual void OnVictory(BattlePlayer player)
        {            
        }

        public void PanCamera(Vector2 pan)
        {
            Vector2 dirVec;
            Vector2 perpDir;
            GetCameraPerpDirVec(out dirVec, out perpDir);

            GameCamera.TargetWorldPosition += new Vector3((perpDir.X + dirVec.X) * pan.X, (perpDir.Y + dirVec.Y) * pan.Y, 0);
        }

        void msg_Closed(object sender, EventArgs e)
        {
            SceneManager.ExitApplication();
        }

        protected abstract void CreatePlayers();
        protected abstract BattleSceneHeader CreateGameSettings();

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                SceneManager.KeyboardManager.KeyDown -= KeyboardManager_KeyDown;
                SceneManager.MouseManager.MouseButtonUp -= MouseManager_MouseButtonUp;
                SceneManager.MouseManager.MouseButtonDown -= MouseManager_MouseButtonDown;
                AppSettingsManager.Default.AppSettingsCommitted -= Default_AppSettingsCommitted;

                Terrain.Dispose();
                mMovementArrow.Dispose();
                mCellLabelManager.Dispose();
                Octree.DestroyObject(mLavaRenderable);
                UnitAttackAreaRenderable.Dispose();
                UnitActionAreaRenderable.Dispose();
                GameCamera.Dispose();
            }
            base.Dispose(isDisposing);
        }

        protected override List<ScenePass> CreateScenePasses(GraphicsDeviceManager graphicsDeviceManager, IRenderTargetManager renderTargetManager, ContentGroup content)
        {
            GameCamera mainCamera = new GameCamera(this);

            //return null;
            List<ScenePass> scenePasses = new List<ScenePass>();

            scenePasses.Add(new ShadowScenePass(mainCamera, this, mLightDir, new Plane(Vector3.UnitZ, HexTerrain.GroundHeight)));
            
            //scenePasses.Add(new WaterReflectionPass(mainCamera, this, renderTargetManager, content));

            //todo - nacitavaj material z content podla mena
            //Material skyPlaneMaterial = new Material(content.Load<MaterialEffect>("SkyPlaneFx"), 0);
            //skyPlaneMaterial.DefaultTechnique.Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("Sky"));
            //skyPlaneMaterial.Techniques["SkyPlaneScenePass"].Passes[0].SetParam("gDiffuseMap", content.Load<Texture2D>("Sky"));
            //scenePasses.Add(new SkyPlaneScenePass(mainCamera.RealCamera, this, content, skyPlaneMaterial));

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
            SceneManager.KeyboardManager.KeyDown += new KeyEventHandler(KeyboardManager_KeyDown);
            SceneManager.KeyboardManager.KeyUp += new KeyEventHandler(KeyboardManager_KeyUp);
            SceneManager.MouseManager.MouseButtonUp += new MouseButtonEventHandler(MouseManager_MouseButtonUp);
            SceneManager.MouseManager.MouseButtonDown += new MouseButtonEventHandler(MouseManager_MouseButtonDown);
            

            
        }

        protected override void OnDeactivateImpl()
        {
            SceneManager.KeyboardManager.KeyDown -= KeyboardManager_KeyDown;
            SceneManager.KeyboardManager.KeyUp -= KeyboardManager_KeyUp;
            SceneManager.MouseManager.MouseButtonUp -= MouseManager_MouseButtonUp;
            SceneManager.MouseManager.MouseButtonDown -= MouseManager_MouseButtonDown;

           
        }

        protected BattleScene(SceneManager sceneManager, ContentGroup content, OrmManager ormManager, BattleSceneHeader settings, HexTerrain terrain)
            : base(sceneManager, content, GetBoundsFromSize(terrain.Width, terrain.Height))
        {
            mHeader = settings;
            Terrain = terrain;

            terrain.InitAfterLoad(this, ormManager);

            Init();
        }

        private void RefreshSelectedCell()
        {
            if (null == mSelectedTile)
            {
                mCursor3dCellSel.IsVisible = false;
                SelectedUnit = null;
            }
            else
            {
                mCursor3dCellSel.IsVisible = true;
                mCursor3dCellSel.Position = mSelectedTile.CenterPos;
                //todo !! 
              //  SelectedUnit = mSelectedTile.GameUnit;
            }
            //todo
          //  mGuiScene.UpdateHexTile(mSelectedTile);
        }


        private void HandleCameraControl()
        {
            Vector2 curPos = SceneManager.MouseManager.CursorPosition;

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
                if (SceneManager.MouseManager.IsButtonDown(MouseButton.Middle))
                {//movement
                    float scrollSpeed = mGameSettings.CameraScrollSpeed;
                    PanCamera(new Vector2(mouseMovement.X * scrollSpeed, mouseMovement.Y * scrollSpeed));
                }
            }

            if (!SceneManager.MouseManager.IsButtonDown(MouseButton.Left) && !SceneManager.MouseManager.IsButtonDown(MouseButton.Right) &&
                !SceneManager.MouseManager.IsButtonDown(MouseButton.Middle))
            {
                float scrollSpeed = mGameSettings.CameraCornerScrollSpeed;
                if (curPos.X <= 0)
                {
                    PanCamera(new Vector2(scrollSpeed, 0));
                }
                else
                {
                    if (curPos.X >= Viewport.Width - 1)
                    {
                        PanCamera(new Vector2(-scrollSpeed, 0));
                    }
                }
                if (curPos.Y <= 0)
                {
                    PanCamera(new Vector2(0, scrollSpeed));
                }
                else
                {
                    if (curPos.Y >= Viewport.Height - 1)
                    {
                        PanCamera(new Vector2(0, -scrollSpeed));
                    }
                }
            }

        }

 

        private void GetCameraPerpDirVec(out Vector2 dirVec, out Vector2 perpDir)
        {
            dirVec = new Vector2(GameCamera.TargetWorldPosition.X - MainCamera.WorldPosition.X,
                GameCamera.TargetWorldPosition.Y - MainCamera.WorldPosition.Y);
            dirVec.Normalize();

            AleMathUtils.GetPerpVector(ref dirVec, out perpDir);
        }

        private void KeyboardManager_KeyDown(Microsoft.Xna.Framework.Input.Keys key, IKeyboardManager keyboardManager)
        {            
            if (key == Microsoft.Xna.Framework.Input.Keys.S)
            {
                ScenePass shadowPass = GetScenePass(ShadowScenePass.Name);
                shadowPass.IsEnabled = !shadowPass.IsEnabled;
                if (!shadowPass.IsEnabled)
                {
                    shadowPass.RenderTarget.Clear();
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
                ExitToMainMenu();
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                EndTurn();
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.F6)
            {
                SaveMap();
            }
            //if (key == Microsoft.Xna.Framework.Input.Keys.F1)
            //{
            //    if (null != SelectedUnit)
            //    {
            //        KillUnit(SelectedUnit);
            //    }
            //}

            //if (key == Microsoft.Xna.Framework.Input.Keys.U)
            //{
            //    if (null != SelectedCell && null == SelectedCell.GameUnit)
            //    {
            //        AddGameUnit(CurrentPlayer, "GameUnit1", SelectedCell.Index, true);
            //    }
            //}
            //if (key == Microsoft.Xna.Framework.Input.Keys.B)
            //{
            //    if (null != SelectedCell && null == SelectedCell.GameUnit)
            //    {
            //        if (CurrentPlayer.HasEnoughManaForUnit("GameUnit1"))
            //        {
            //            CurrentPlayer.BuyUnit("GameUnit1", SelectedCell.Index);
            //        }
            //        Console.WriteLine(CurrentPlayer.Mana);
            //    }
            //}

            if (key == Microsoft.Xna.Framework.Input.Keys.O)
            {

                SpecialEffectManager.FireSpecialEffect("TestSpellAnim", HexHelper.Get3DPosFromIndex(new Point(1, 1), 0));
            }
        }

        private void KeyboardManager_KeyUp(Microsoft.Xna.Framework.Input.Keys key, IKeyboardManager keyboardManager)
        {            
        }

        public void KillUnit(BattleUnit unit)
        {
            if (null == unit) throw new ArgumentNullException("unit");

            ParticleSystemManager.CreateFireAndForgetParticleSystem(mUnitDeathParticleSystemDesc, unit.Position);

            //   GetCell(unit.CellIndex).GameUnit = null;
            unit.OwningPlayer.RemoveGameUnit(unit);
            DestroySceneObject(unit);
            RefreshSelectedCell();
        }

        public string GetMapDirName()
        {
            return GetMapDirName(GameType);
        }

        public string GetMapFileName()
        {
            return GetMapFileName(Name, GameType);
        }

        public static string GetMapFileName(string mapName, string gameType)
        {
            return Path.Combine(GetMapDirName(gameType), mapName + ".map");
        }

        public static string GetMapDirName(string gameType)
        {
            string modFile = MainSettings.Instance.Mod;
            if (!Path.IsPathRooted(modFile))
            {
                modFile = Path.Combine(MainSettings.Instance.DataDirectory, modFile);
            }

            return Path.Combine(Path.Combine(Path.GetDirectoryName(modFile), Path.GetFileNameWithoutExtension(modFile)), string.Format("Maps\\{0}", gameType));
        }

        protected override Ale.SpecialEffects.ISpecialEffectManager CreateSpecialEffectManager()
        {
            var specialEffectManager =  base.CreateSpecialEffectManager();
            specialEffectManager.RegisterTriggerAction(new CameraShakeTimeTriggerAction(GameCamera));

            return specialEffectManager;
        }


        private void MouseManager_MouseButtonUp(MouseButton button, IMouseManager mouseManager)
        {
            State.OnClickOnTile(GetTileUnderCur(), button);            
        }

        private void MouseManager_MouseButtonDown(MouseButton button, IMouseManager mouseManager)
        {            
        }

        private void unit_CellIndexChanged(BattleUnit obj, Point oldValue)
        {
            //todo
            //HexCell cell = GetCell(obj.CellIndex);
            //if (cell.OwningPlayer != obj.OwningPlayer) //has captured
            //{
            //    ParticleSystemManager.CreateFireAndForgetParticleSystem(mCaptureParticleSystemDesc, cell.CenterPos);
            //    cell.OwningPlayer = obj.OwningPlayer;
            //}
            //GetCell(oldValue).GameUnit = null;
            //cell.GameUnit = obj;
        }

        private void Init()
        {
            //temp!!!!!!!!!!!!!!
            Terrain.SetTile(new Point(0, 0), "LavaHexTile");
            Terrain.SetTile(new Point(0, 1), "LavaHexTile");
            Terrain.SetTile(new Point(1, 2), "LavaHexTile");
            Terrain.SetTile(new Point(1, 1), "LavaHexTile");
            Terrain.SetTile(new Point(2, 2), "LavaHexTile");
            Terrain.SetTile(new Point(1, 3), "StoneTile");
            Terrain.SetTile(new Point(3, 2), "StoneTile");
            Terrain.SetTile(new Point(3, 1), "StoneTile2");
            //for (int i = 1; i < 30; ++i)
            //{
            //    Terrain.SetTile(new Point(3, i), "LavaHexTile");
            //}
            //for (int i = 1; i < 30; ++i)
            //{
            //    Terrain.SetTile(new Point(5, i), "LavaHexTile");
            //}
            //for (int i = 1; i < 30; ++i)
            //{
            //    Terrain.SetTile(new Point(8, i), "LavaHexTile");
            //}
            //temp!!!!!!!!!!!!!!
            CurrentPlayer = new HumanBattlePlayer(this, "ja", Color.Blue.ToVector3());



            AppSettingsManager.Default.AppSettingsCommitted += new AppSettingsManager.CommittedHandler(Default_AppSettingsCommitted);

            Viewport = GraphicsDeviceManager.GraphicsDevice.Viewport;
            mGameSettings = AppSettingsManager.Default.GetSettings<GameSettings>();


            PostProcessEffectManager.PostProcessEffects.Add(new BloomProcessEffect(GraphicsDeviceManager, RenderTargetManager, Content));
            //PostProcessEffectManager.PostProcessEffects.Add(new InvertProcessEffect(GraphicsDeviceManager, RenderTargetManager, Content));
            PostProcessEffectManager.PostProcessEffects[0].Enabled = false;

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

            //gui
            //todo
         //   mGuiScene = new GameGuiScene(this);
            State = GetGameSceneState(GameSceneStates.Idle);

            mCursor3dCellSel.IsVisible = false;

            mMovementArrow.IsVisible = false;

            mLavaRenderable = new LavaRenderable(GraphicsDeviceManager.GraphicsDevice, Content, Octree.Bounds);
            mLavaRenderable.IsVisible = false;
            Octree.AddObject(mLavaRenderable);

            Material lightMat = Content.Load<Material>("PointLightMat");
            //int cnt = 15;
            //for (int i = 0; i < cnt; ++i)
            //{
            //    Vector3 v = new Vector3(0.5f, 0.5f, 0.5f);
            //    Vector3 c = AleMathUtils.GetRandomVector3(ref v, 0.5f);
            //    //  c = new Vector3(1,0.1f,0.1f);
            //    if (i < (float)cnt / 3.0f)
            //    {
            //        c = new Vector3(2, 0, 0);
            //    }
            //    else
            //    {
            //        if (i < 2.0f * (float)cnt / 3.0f)
            //        {
            //            c = new Vector3(0.0f, 2.0f, 0.0f);
            //        }
            //        else
            //        {
            //            c = new Vector3(0.0f, 0.0f, 2.0f);
            //        }
            //    }

            //    PointLight light = new PointLight(Content, lightMat);
            //    light.Color = c;
            //    light.Scale = 2.0f;
            //    var sss = HexHelper.Get3DPosFromIndex(new Point(Terrain.Height, Terrain.Width), HexTerrain.GroundHeight);
            //    light.Position = new Vector3((float)AleMathUtils.Random.NextDouble() * sss.X
            //        , (float)AleMathUtils.Random.NextDouble() * sss.Y, 0.5f);

            //    //light.Position = new Vector3(10, 10, 1);



            //    Octree.AddObject(light);


            //    //GraphicModel gapWall = new GraphicModel(Content.Load<Mesh>("LavaGapWall"), new List<Material>()
            //    //{
            //    //    Content.Load<Material>("WallMat"),
            //    //    Content.Load<Material>("WallLavaGlowMat")
            //    //});
            //    //for (int j = 0; j < 6; ++j)
            //    //{
            //    //    gapWall.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(60 * j));
            //    //    StaticGeomtery.AddGraphicModel(gapWall);
            //    //}
            //}

            cursorLight = new PointLight(Content, lightMat);
            cursorLight.Color = new Vector3(2.0f, 2.0f, 2.0f);
            cursorLight.Scale = 1.5f;
            cursorLight.Position = new Vector3(0, 0, 0.1f);
            //cursorLight.IsVisible = false;
            Octree.AddObject(cursorLight);

            UnitAttackAreaRenderable = new UnitAttackAreaRenderable(this);
            UnitActionAreaRenderable = new UnitActionAreaRenderable(this);
        }
        PointLight cursorLight;


        private static BoundingBox GetBoundsFromSize(int width, int height)
        {
            var v1 = HexHelper.Get3DPosFromIndex(new Point(0, 0), HexTerrain.GroundHeight);
            v1.X -= 50;
            v1.Y -= 50;
            v1.Z = -100;
            var v2 = HexHelper.Get3DPosFromIndex(new Point(width, height), HexTerrain.GroundHeight);
            v2.X += 50;
            v2.Y += 50;
            v2.Z = 100;
            return new BoundingBox(v1, v2);
        }

        private void Default_AppSettingsCommitted(IAppSettings settings)
        {
            if (settings is VideoSettings)
            {
                Viewport = GraphicsDeviceManager.GraphicsDevice.Viewport;
            }
            else
            {
                if (settings is GameSettings)
                {
                    mGameSettings = ((GameSettings)settings);
                }
            }
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




    internal class LavaRenderable : GraphicModel
    {
        const float ZPos = -5.00f;
        private bool mIsDisposed = false;

        public LavaRenderable(GraphicsDevice graphicsDevice, ContentGroup content, BoundingBox sceneBounds)
            : base(BuildMesh(graphicsDevice, sceneBounds), GetMaterial(content))
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

        private static Mesh BuildMesh(GraphicsDevice graphicsDevice, BoundingBox sceneBounds)
        {
            MeshBuilder mb = new MeshBuilder(graphicsDevice);
            mb.SetCurrentSubMesh("m");

            Vector3 min = sceneBounds.Min;
            Vector3 max = sceneBounds.Max;
            int v1 = mb.AddVertex(new SimpleVertex(new Vector3(min.X, max.Y, ZPos), new Vector3(0, 0, 1), new Vector2(0.0f, 0.0f)));
            int v2 = mb.AddVertex(new SimpleVertex(new Vector3(max.X, max.Y, ZPos), new Vector3(0, 0, 1), new Vector2(1.0f, 0.0f)));
            int v3 = mb.AddVertex(new SimpleVertex(new Vector3(max.X, min.Y, ZPos), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f)));
            int v4 = mb.AddVertex(new SimpleVertex(new Vector3(min.X, min.Y, ZPos), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));

            mb.AddFace(v1, v2, v4);
            mb.AddFace(v3, v4, v2);

            return mb.BuildMesh(true);
        }

        private static Material GetMaterial(ContentGroup content)
        {
           // return content.Load<Material>("SurfaceMat");
            MaterialSettings settings= new MaterialSettings("LavaMat", "LavaFx", DefaultRenderLayers.Water);
            settings.Params.Add(new Texture2DMaterialParamSettings("gLavaNoiseMap", "LavaNoiseTex"));
            settings.Params.Add(new Texture2DMaterialParamSettings("gLavaColdMap", "LavaColdTex"));
            settings.Params.Add(new Texture2DMaterialParamSettings("gNoiseMap", "bwNoise"));
            return new Material(settings, content);
        }
    }
}
