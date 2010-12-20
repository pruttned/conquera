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
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using Ale.Tools;
using Ale.Content;
using Ale.Graphics;
using System.Collections.ObjectModel;
using Ale.Scene;
using Ale;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 0)]
    [CustomBasicTypeProvider(typeof(Point), typeof(FieldCustomBasicTypeProvider<Point>))]
    public class GameUnit : OctreeSceneObject, IDataObject
    {
        public delegate void CellIndexChangedHandler(GameUnit obj, Point oldValue);
        public event CellIndexChangedHandler CellIndexChanged;

        private Point mCellIndex;
        private IGameUnitState mState;
        private Dictionary<string, IGameUnitState> mStates = new Dictionary<string, IGameUnitState>();
        private List<GameCard> mCardsPrivate = new List<GameCard>();
        
        public GamePlayer OwningPlayer { get; internal set; }

        [DataProperty(NotNull = true)]
        public Point CellIndex
        {
            get { return mCellIndex; }
            set
            {
                if (mCellIndex != value)
                {
                    Point oldValue = mCellIndex;
                    mCellIndex = value;

                    if (IsInScene)
                    {
                        UpdatePositionFromIndex();
                    }

                    OnCellIndexChanged(oldValue);

                    if (null != CellIndexChanged)
                    {
                        CellIndexChanged.Invoke(this, oldValue);
                    }
                }
            }
        }

        public GameScene GameScene
        {
            get { return (GameScene)Scene; }
        }

        public GameUnitDesc GameUnitDesc
        {
            get { return (GameUnitDesc)Desc; }
        }

        public SkeletalAnimationPlayer AnimationPlayer
        {
            get { return GraphicModel.AnimationPlayer; }
        }

        public bool IsIdle { get; private set; }

        [DataProperty(NotNull = true)]
        public int Hp { get; private set; }
        
        public int MaxHp 
        {//add modifiers?
            get { return GameUnitDesc.MaxHp; } 
        }

        public ReadOnlyCollection<GameCard> Cards { get; private set; }
        public int AttackPurple { get; private set; }
        public int AttackGreen { get; private set; }
        public int AttackBlack { get; private set; }
        public int DefensePurple { get; private set; }
        public int DefenseGreen { get; private set; }
        public int DefenseBlack { get; private set; }        

        public bool HasMovedThisTurn
        {
            get { return (LastMovedTurn == GameScene.GameSceneContextState.TurnNum); }
        }

        public bool HasAttackedThisTurn
        {
            get { return (LastAttackedTurn == GameScene.GameSceneContextState.TurnNum); }
        }

        public IGameUnitState State
        {
            get { return mState; }
            set
            {
                mState = value;
                mState.OnStart();
                IsIdle = mState.IsIdle;
            }
        }

        public Dictionary<string, IGameUnitState> States
        {
            get { return mStates; }
        }

        long IDataObject.Id { get; set; }

        [DataListProperty(NotNull = true)] //todo
        private List<long> CardsDb { get; set; }

        [DataProperty(NotNull = true)]
        private long DescId { get; set; }

        [DataProperty(NotNull = true)]
        private int LastMovedTurn { get; set; }

        [DataProperty(NotNull = true)]
        private int LastAttackedTurn { get; set; }

        public GameUnit(long descId, GamePlayer owningPlayer)
            :this()
        {
            DescId = descId;
            OwningPlayer = owningPlayer;
            Cards = new ReadOnlyCollection<GameCard>(mCardsPrivate);
        }

        public override void Update(AleGameTime gameTime)
        {
            mState.Update(gameTime);
        }

        public bool CanMoveTo(Point cell)
        {
            if (cell == CellIndex)
            {
                return false;
            }

            HexCell srcCell = GameScene.GetCell(CellIndex);
            HexCell targetCell = GameScene.GetCell(cell);

            return targetCell.IsPassable && (srcCell.Region == targetCell.Region || srcCell.IsSiblingTo(targetCell));
        }

        public bool CanAttackTo(Point cell)
        {
            if (cell == CellIndex)
            {
                return false;
            }

            HexCell srcCell = GameScene.GetCell(CellIndex);
            HexCell targetCell = GameScene.GetCell(cell);

            if (null == targetCell.GameUnit)
            {
                return false;
            }

            return null != targetCell.GameUnit && OwningPlayer != targetCell.GameUnit.OwningPlayer &&  srcCell.IsSiblingTo(targetCell);
        }

        public bool MoveTo(Point cell)
        {
            CheckIdle();

            if (cell != CellIndex)
            {
                if (!HasMovedThisTurn && CanMoveTo(cell))
                {
                    LastMovedTurn = GameScene.GameSceneContextState.TurnNum;

                    var state = (MovingGameUnitState)States["Move"];
                    state.TargetCell = cell;
                    State = state;

                    return true;
                }
            }
            return false;
        }

        public bool Attack(Point cell)
        {
            CheckIdle();

            if (!HasAttackedThisTurn && CanAttackTo(cell))
            {
                LastAttackedTurn = GameScene.GameSceneContextState.TurnNum;

                var state = (AttackingGameUnitState)States["Attack"];
                state.TargetUnit = GameScene.GetCell(cell).GameUnit;
                State = state;
                
                return true;
            }
            return false;
        }


        public void RotateTo(Point cell)
        {
            Vector3 targetPos = GetPositionFromIndex(cell);

            Vector3 movementVect = targetPos - Position;
            movementVect.Normalize();

            float angle = (float)Math.Atan2(movementVect.Y, movementVect.X) - 1.570f;
            Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, angle);
        }


        public void AddCard(GameCard card)
        {
            mCardsPrivate.Add(card);
            RecalculateAttackDefense();
        }

        public void RemoveCard(GameCard card)
        {
            mCardsPrivate.Remove(card);
            RecalculateAttackDefense();
        }

        public int ComputeDamageTo(GameUnit unit)
        {
            return (Math.Max(AttackBlack - unit.DefenseBlack, 0) +
                    Math.Max(AttackPurple - unit.DefensePurple, 0) +
                    Math.Max(AttackGreen - unit.DefenseGreen, 0));
        }

        //todo jednotka moze v jednom tahu ista a bojovat v lubovolnom poradi. Ked vsak zabije zlu jednotku, tak to neznamena, ze na to policko aj prejde.

        public void Heal(int amount)
        {
            GameScene.FireCellNotificationLabel(string.Format("+{0}", amount), CellNotificationIcons.Hearth, Color.Red, CellIndex);
            Hp = Math.Min(Hp + amount, MaxHp);
        }

        public void ReceiveDamage(int damage)
        {
            Hp -= damage;
            GameScene.FireCellNotificationLabel(string.Format("-{0}", Math.Abs(damage)), CellNotificationIcons.BrokenHearth, Color.Red, CellIndex);
            if (Hp <= 0)
            {
                GameScene.KillUnit(this);
            }
            else
            {
                GameScene.ParticleSystemManager.CreateFireAndforgetParticleSystem(GameUnitDesc.BloodParticleSystem, Position);
            }
        }

        public Vector3 GetPositionFromIndex(Point index)
        {
            HexCell cell = GameScene.GetCell(index);
            if (!cell.IsGap)
            {
                return cell.CenterPos + cell.HexTerrainTile.UnitPosition;
            }
            else
            {
                return cell.CenterPos;
            }
        }

        protected GameUnit()
        {
            Cards = new ReadOnlyCollection<GameCard>(mCardsPrivate);

            IsIdle = true;
            LastMovedTurn = -1;
            LastAttackedTurn = -1;

            States.Add("Idle", new IdleGameUnitState(this));
            States.Add("Move", new MovingGameUnitState(this));
            States.Add("Attack", new AttackingGameUnitState(this));
        }

        protected override bool IsSceneValid(BaseScene scene)
        {
            return (scene is GameScene);
        }

        protected override void OnAddToSceneImpl(BaseScene scene)
        {
            base.OnAddToSceneImpl(scene);

            State = States["Idle"];

            UpdatePositionFromIndex();

            foreach(var part in GraphicModel.GraphicModelParts)
            {
                var colorParam = PlayerColorMaterialEffectParametersUpdater.TryGetPlayerColorParam(part.Material.MaterialEffect);
                if (null != colorParam)
                {
                    part.CustomMaterialEffectParametersUpdater = new PlayerColorMaterialEffectParametersUpdater(colorParam, OwningPlayer.Color);
                }
            }

            GraphicModel.AnimationPlayer.Animation = GameUnitDesc.IdleAnimation;
            GraphicModel.AnimationPlayer.Play(true, true);
            GraphicModel.AnimationPlayer.AnimStopped += new SkeletalAnimationPlayer.AnimStoppedHandler(AnimationPlayer_AnimStopped);

            if (((IDataObject)this).Id <= 0) //not loaded
            {
                Hp = GameUnitDesc.MaxHp;
            }
            RecalculateAttackDefense();
        }

        protected virtual void OnCellIndexChanged(Point oldValue)
        {
        }

        protected override OctreeSceneObjectDesc LoadDesc(ContentGroup content)
        {
            return content.Load<GameUnitDesc>(DescId);
        }    

        private void UpdatePositionFromIndex()
        {
            Position = GetPositionFromIndex(mCellIndex);
        }

        void IDataObject.AfterLoad(OrmManager ormManager)
        {
        }

        void IDataObject.BeforeSave(OrmManager ormManager)
        {
        }

        void IDataObject.AfterSave(OrmManager ormManager, bool isNew)
        {
        }

        void IDataObject.BeforeDelete(OrmManager ormManager)
        {
        }

        private void RecalculateAttackDefense()
        {
            AttackPurple = GameUnitDesc.BaseRedAttack;
            AttackGreen = GameUnitDesc.BaseGreenAttack;
            AttackBlack = GameUnitDesc.BaseBlackAttack;

            DefensePurple = GameUnitDesc.BaseRedDefense;
            DefenseGreen = GameUnitDesc.BaseGreenDefense;
            DefenseBlack = GameUnitDesc.BaseBlackDefense;

            foreach (GameCard card in Cards)
            {
                AttackPurple += card.AttackPurple;
                AttackGreen += card.AttackGreen;
                AttackBlack += card.AttackBlack;

                DefensePurple += card.DefensePurple;
                DefenseGreen += card.DefenseGreen;
                DefenseBlack += card.DefenseBlack;
            }
        }

        private void CheckIdle()
        {
            if(!IsIdle) throw new InvalidOperationException("Operation can be called only in idle state");
        }

        private void AnimationPlayer_AnimStopped(SkeletalAnimationPlayer animation)
        {
            GraphicModel.AnimationPlayer.Animation = GameUnitDesc.IdleAnimation;
            GraphicModel.AnimationPlayer.Play(true);
        }
    }

    class PlayerColorMaterialEffectParametersUpdater : IMaterialEffectParametersUpdater
    {
        private Vector3MaterialEffectParam mColorParam;
        private Vector3 mColor;

        public PlayerColorMaterialEffectParametersUpdater(Vector3MaterialEffectParam colorParam, Vector3 color)
        {
            mColor = color;
            mColorParam = colorParam;
        }

        public static Vector3MaterialEffectParam TryGetPlayerColorParam(MaterialEffect effect)
        {
            return (Vector3MaterialEffectParam)effect.ManualParameters["gPlayerColor"];
        }

        public void UpdateMaterialEffectParameters()
        {
            mColorParam.Value = mColor;
        }
    }
}
