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
        public event EventHandler GoldChanged;
        public event EventHandler MaxUnitCntChanged;

        private List<GameUnit> mUnits = new List<GameUnit>();
        private List<HexCell> mCells = new List<HexCell>();
        private Dictionary<GameCard, int> mAllowedGameCards = new Dictionary<GameCard, int>();

        private GameScene mScene;
        private Material mActiveBorderMaterial;
        private Material mInactiveBorderMaterial;
        private int mGold;
        private int mMaxUnitCnt;

        public ReadOnlyCollection<HexCell> Cells { get; private set; }

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
        public int Gold
        {
            get { return mGold; }
            set
            {
                if (value != mGold)
                {
                    mGold = value;

                    if (GoldChanged != null)
                    {
                        GoldChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        [DataProperty(NotNull = true)]
        public Vector3 Color { get; private set; }
        [DataListProperty(NotNull=true)]
        private List<GameUnit> Units
        {
            get { return mUnits; }
            set { mUnits = value; }
        }

        internal Material ActiveBorderMaterial
        {
            get
            {
                CheckInit();
                return mActiveBorderMaterial;
            }
        }
        internal Material InactiveBorderMaterial
        {
            get
            {
                CheckInit();
                return mInactiveBorderMaterial;
            }
        }


        public GamePlayer(Vector3 color)
            :this()
        {
            Color = color;
        }

        public void AddCard(GameCard card)
        {
            int cardCnt = 0;
            mAllowedGameCards.TryGetValue(card, out cardCnt);
            mAllowedGameCards[card] = cardCnt+1;
        }

        public void RemoveCard(GameCard card)
        {
            int cardCnt = 0;
            if (mAllowedGameCards.TryGetValue(card, out cardCnt))
            {
                cardCnt--;
                if (0 == cardCnt)
                {
                    mAllowedGameCards.Remove(card);
                }
                else
                {
                    mAllowedGameCards[card] = cardCnt;
                }
            }
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
            mUnits.Add(unit);
        }

        internal bool RemoveGameUnit(GameUnit unit)
        {
            return mUnits.Remove(unit);
        }

        public abstract void OnBegineTurn();
        public abstract void OnEndTurn();

        internal void Init(GameScene scene, ContentGroup content)
        {
            mScene = scene;

            MaterialSettings ms = new MaterialSettings("LineMat", "ActiveBorderFx");
            ms.RenderLayer = DefaultRenderLayers.GroundLyingObjects;
            ms.Params.Add(new Texture2DMaterialParamSettings("gDiffuseMap", "lineText"));
            ms.Params.Add(new Vector3MaterialParamSettings("gColor", Color));
            mActiveBorderMaterial = new Material(ms, content);
            ms.EffectName = "InactiveBorderFx";
            mInactiveBorderMaterial = new Material(ms, content);

            foreach (var unit in Units)
            {
                unit.OwningPlayer = this;
                scene.AddGameUnit(unit);
            }
        }

        /// <summary>
        /// Whether has enough gold
        /// </summary>
        /// <param name="descName"></param>
        public bool HasEnoughGoldForUnit(string descName)
        {
            CheckInit();

            var desc = Scene.Content.Load<GameUnitDesc>(descName);
            return (desc.Cost <= Gold);
        }

        public GameUnit BuyUnit(string descName, Point cell)
        {
            CheckInit();

            var desc = Scene.Content.Load<GameUnitDesc>(descName);
            if (desc.Cost > Gold)
            {
                throw new ArgumentException("Not enough gold");
            }

            Gold -= desc.Cost;

            return Scene.AddGameUnit(this, descName, cell);
        }

        /// <summary>
        /// Ctor only for Sof
        /// </summary>
        protected GamePlayer()
        {
            Cells = new ReadOnlyCollection<HexCell>(mCells);
        }

        private void CheckInit()
        {
            if (null == mScene) { throw new InvalidOperationException("Init has not yet been called"); }
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

        public HumanPlayer(Vector3 color)
            :base(color)
        {
            CameraTargetPos = new Vector3();
        }

        public override void OnBegineTurn()
        {
            if(!Scene.GameCamera.IsInSight(new BoundingSphere(CameraTargetPos, 0.5f)))
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
    }
}
