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
    public abstract class BattlePlayer
    {
        public event EventHandler ManaChanged;
        public event EventHandler UnitsChanged;

        private List<BattleUnit> mUnits = new List<BattleUnit>();

        private int mMana;

        private Dictionary<string, IGameSceneState> mGameSceneStates = new Dictionary<string, IGameSceneState>();

        public ReadOnlyCollection<HexTerrainTile> Tiles { get; private set; }

        /// <summary>
        /// Changed by CastleTileDesc
        /// </summary>
        public int CastleCnt { get; set; }

        public BattleScene Scene {get; private set;}

        public abstract bool IsHuman { get; }

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

        public string Name { get; set; }

        public Vector3 Color { get; private set; }
        internal List<BattleUnit> Units
        {
            get { return mUnits; }
            set { mUnits = value; }
        }

        public BattlePlayer(BattleScene scene, string name, Vector3 color)
        {
            Scene = scene;
            Name = name;
            Color = color;

            CreateGameSceneStates(scene, mGameSceneStates);
        }

        public IGameSceneState GetGameSceneState(string name)
        {
            return mGameSceneStates[name];
        }

        internal void AddGameUnit(BattleUnit unit)
        {
            mUnits.Add(unit);
            RaiseUnitsChanged();
        }

        internal bool RemoveGameUnit(BattleUnit unit)
        {
            bool removed = mUnits.Remove(unit);
            RaiseUnitsChanged();
            return removed;
        }

        public virtual void OnBeginTurn()
        {
        }

        public abstract void OnEndTurn();

        /// <summary>
        /// Whether has enough mana
        /// </summary>
        /// <param name="descName"></param>
        public bool HasEnoughManaForUnit(string descName)
        {
            var desc = Scene.Content.Load<GameUnitDesc>(descName);
            return (desc.Cost <= Mana);
        }

        public BattleUnit BuyUnit(string descName, Point cell)
        {
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

        protected abstract void CreateGameSceneStates(BattleScene scene, Dictionary<string, IGameSceneState> gameSceneStates);
        
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
    public class HumanBattlePlayer : BattlePlayer
    {
        [DataProperty(NotNull = true)]
        public Vector3 CameraTargetPos { get; set; }

        public override bool IsHuman
        {
            get { return true; }
        }

        public HumanBattlePlayer(BattleScene scene, string name, Vector3 color)
            : base(scene, name, color)
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
                    CameraTargetPos = Units[0].Tile.CenterPos;
                }
                else
                {
                    if (0 < Tiles.Count)
                    {
                        CameraTargetPos = Tiles[0].CenterPos;
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

        protected override void CreateGameSceneStates(BattleScene scene, Dictionary<string, IGameSceneState> gameSceneStates)
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
