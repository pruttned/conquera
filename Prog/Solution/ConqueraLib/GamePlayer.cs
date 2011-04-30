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
using Ale.Graphics;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ale.Content;
using SimpleOrmFramework;
using Ale.Tools;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 0)]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    public abstract class GamePlayer : BaseDataObject
    {
        public event EventHandler ManaChanged;
        public event EventHandler MaxUnitCntChanged;
        public event EventHandler UnitsChanged;

        private List<GameUnit> mUnits = new List<GameUnit>();
        private List<HexCell> mCells = new List<HexCell>();

        private GameScene mScene;
        private int mMana;
        private int mMaxUnitCnt;

        private Dictionary<string, IGameSceneState> mGameSceneStates = new Dictionary<string, IGameSceneState>();

        public ReadOnlyCollection<HexCell> Cells { get; private set; }

        /// <summary>
        /// Changed by CastleTileDesc
        /// </summary>
        public int CastleCnt { get; set; }

        //Computed - not saved
        public int MaxUnitCnt
        {
            get { return mMaxUnitCnt; }
            set
            {
                if (value != mMaxUnitCnt)
                {
                    mMaxUnitCnt = value;

                    if (MaxUnitCntChanged != null)
                    {
                        MaxUnitCntChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public GameScene Scene
        {
            get
            {
                CheckInit();
                return mScene;
            }
        }

        public abstract bool IsHuman { get; }

        [DataProperty(NotNull = true)]
        public int Mana
        {
            get { return mMana; }
            set
            {
                if (value != mMana)
                {
                    mMana = value;

                    if (ManaChanged != null)
                    {
                        ManaChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        [DataProperty(NotNull = true, Unique=true)]
        public string Name { get; set; }

        [DataProperty(NotNull = true)]
        public Vector3 Color { get; private set; }
        [DataListProperty(NotNull = true)]
        internal List<GameUnit> Units
        {
            get { return mUnits; }
            set { mUnits = value; }
        }

        public GamePlayer(string name, Vector3 color)
            : this()
        {
            Name = name;
            Color = color;
        }

        public IGameSceneState GetGameSceneState(string name)
        {
            return mGameSceneStates[name];
        }

        internal void AddCell(HexCell cell)
        {
            mCells.Add(cell);
        }

        internal bool RemoveCell(HexCell cell)
        {
            return mCells.Remove(cell);
        }

        internal void AddGameUnit(GameUnit unit)
        {
            //if (mUnits.Count >= MaxUnitCnt)
            //{
            //    throw new InvalidOperationException("MaxUnitCnt already reached.");
            //}

            mUnits.Add(unit);
            RaiseUnitsChanged();
        }

        internal bool RemoveGameUnit(GameUnit unit)
        {
            bool removed = mUnits.Remove(unit);
            RaiseUnitsChanged();
            return removed;
        }

        public virtual void OnBeginTurn()
        {
            Mana = 0;
        }

        public abstract void OnEndTurn();

        internal void Init(GameScene scene, ContentGroup content)
        {
            mScene = scene;

            foreach (var unit in Units)
            {
                unit.OwningPlayer = this;
                scene.AddGameUnit(unit);
            }

            CreateGameSceneStates(mScene, mGameSceneStates);
        }

        /// <summary>
        /// Whether has enough mana
        /// </summary>
        /// <param name="descName"></param>
        public bool HasEnoughManaForUnit(string descName)
        {
            CheckInit();

            var desc = Scene.Content.Load<GameUnitDesc>(descName);
            return (desc.Cost <= Mana);
        }

        public GameUnit BuyUnit(string descName, Point cell)
        {
            CheckInit();

            var desc = Scene.Content.Load<GameUnitDesc>(descName);
            if (desc.Cost > Mana)
            {
                throw new ArgumentException("Not enough mana");
            }

            Mana -= desc.Cost;

            var unit =  Scene.AddGameUnit(this, descName, cell, false);
                                    
            return unit;
        }

        public override string ToString()
        {
            return Name;
        }

        protected GamePlayer()
        {
            Cells = new ReadOnlyCollection<HexCell>(mCells);
            MaxUnitCnt = 3;
        }

        protected abstract void CreateGameSceneStates(GameScene scene, Dictionary<string, IGameSceneState> gameSceneStates);

        private void CheckInit()
        {
            if (null == mScene) { throw new InvalidOperationException("Init has not yet been called"); }
        }
        private void RaiseUnitsChanged()
        {
            if (UnitsChanged != null)
            {
                UnitsChanged(this, EventArgs.Empty);
            }
        }
    }

    [DataObject(MaxCachedCnt = 0)]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    public class HumanPlayer : GamePlayer
    {
        [DataProperty(NotNull = true)]
        public Vector3 CameraTargetPos { get; set; }

        public override bool IsHuman
        {
            get { return true; }
        }

        public HumanPlayer(string name, Vector3 color)
            : base(name, color)
        {
            CameraTargetPos = new Vector3(-10000, -10000, -10000);
        }

        public override void OnBeginTurn()
        {
            base.OnBeginTurn();

            if (CameraTargetPos.X < -1000)
            {
                if (0 < Units.Count)
                {
                    CameraTargetPos = Units[0].Cell.CenterPos;
                }
                else
                {
                    if (0 < Cells.Count)
                    {
                        CameraTargetPos = Cells[0].CenterPos;
                    }
                    else
                    {
                        CameraTargetPos = new Vector3();
                    }
                }
            }
                if (!Scene.GameCamera.IsInSight(new BoundingSphere(CameraTargetPos, 0.5f)))
                {
                    //              Scene.GameCamera.MoveCameraTo(CameraTargetPos);
                    Scene.GameCamera.TargetWorldPosition = CameraTargetPos;
                }
        }

        public override void OnEndTurn()
        {
            CameraTargetPos = Scene.GameCamera.TargetWorldPosition;
        }

        /// <summary>
        /// Ctor only for Sof
        /// </summary>
        protected HumanPlayer()
        {
        }

        protected override void CreateGameSceneStates(GameScene scene, Dictionary<string, IGameSceneState> gameSceneStates)
        {
            gameSceneStates.Add(GameSceneStates.Idle, new IdleGameSceneState(scene));
            gameSceneStates.Add(GameSceneStates.UnitMoving, new UnitMovingGameSceneState(scene));
            gameSceneStates.Add(GameSceneStates.CameraAnimation, new CameraAnimationGameSceneState(scene));
            gameSceneStates.Add(GameSceneStates.VictoryEvaluation, new VictoryEvaluationGameSceneState(scene));
            gameSceneStates.Add(GameSceneStates.BeginTurn, new BeginTurnGameSceneState(scene));
            gameSceneStates.Add(GameSceneStates.ReadyGameUnitSelected, new ReadyGameUnitSelectedGameSceneState(scene));
            gameSceneStates.Add(GameSceneStates.Battle, new BattleGameSceneState(scene));
        }
    }
}
