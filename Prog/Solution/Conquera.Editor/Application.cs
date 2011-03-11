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
using Ale;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using Ale.Graphics;
using Ale.Input;
using System.Diagnostics;
using Ale.Tools;
using Ale.Scene;
using Microsoft.Xna.Framework.Content;
using Ale.Content;
using Ale.Gui;
using Conquera.Gui;
using System.IO;
using Ale.Settings;

namespace Conquera.Editor
{
    public class EditorApplication : BaseApplication
    {
        private ToolBarForm mToolBarForm = new ToolBarForm();
        private EditorScene mEditorScene;
        private NewMapInfo mDefaultNewMapInfo;
        Dictionary<string, TileBrush> mTileBrushes = new Dictionary<string, TileBrush>(StringComparer.InvariantCultureIgnoreCase);

        public CommandQueue CommandQueue { get; private set; }
        public ICollection<string> TileBrushes { get { return mTileBrushes.Keys; } }

        public GameScene GameScene
        {
            get
            {
                return mEditorScene.GameScene;
            }
            set
            {
                mEditorScene.GameScene = value;
                GuiManager.Instance.Cursor = null;

            }
        }

        public string TileBrush
        {
            get
            {
                if (null == mEditorScene.TileBrush)
                {
                    return null;
                }
                return mEditorScene.TileBrush.Name;
            }
            set
            {
                mEditorScene.TileBrush = null == value ? null : mTileBrushes[value];
            }
        }

        public IList<GamePlayer> Players { get { return mEditorScene.Players; } }
        public List<string> UnitTypes { get; private set; }

        public EditMode EditMode
        {
            get
            {
                return mEditorScene.EditMode;
            }
            set
            {
                mEditorScene.EditMode = value;
            }
        }

        public string UnitType
        {
            get
            {
                return mEditorScene.UnitType;
            }
            set
            {
                mEditorScene.UnitType = value;
            }
        }

        public GamePlayer Player
        {
            get
            {
                return mEditorScene.Player;
            }
            set
            {
                mEditorScene.Player = value;
            }
        }

        protected override string GuiPaletteName
        {
            get { return "PaletteDef"; }
        }

        protected override CursorInfo DefaultCursor
        {
            get { return null; }
        }

        public EditorApplication(NewMapInfo newMapInfo)
            : base()
        {
            CommandQueue = new CommandQueue();
            mDefaultNewMapInfo = newMapInfo;

            InitTileBrushes();
            InitUnitTypes();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                mToolBarForm.Dispose();
            }

            base.Dispose(isDisposing);
        }

        protected override BaseScene CreateDefaultScene(SceneManager sceneManager)
        {
            mEditorScene = new EditorScene(new HotseatGameScene(mDefaultNewMapInfo.Name, SceneManager, mDefaultNewMapInfo.Width, mDefaultNewMapInfo.Height,
                "Grass1Tile", Content.DefaultContentGroup));
            return mEditorScene;
        }

        protected override void OnInit()
        {
            mToolBarForm = new ToolBarForm();
            mToolBarForm.EditorApplication = this;
            mToolBarForm.RenderWindow = RenderWindow;
            mToolBarForm.Show();
            ShowSysCursor = true;

            base.OnInit();

            GuiManager.Instance.Cursor = null;
        }

        protected override void Update(AleGameTime gameTime)
        {
            ICommand command = null;
            while(null != (command = CommandQueue.Dequeue()))
            {
                command.Execute(this);
            }

            base.Update(gameTime);
        }

        private void InitTileBrushes()
        {
            var ormManager = Content.OrmManager;
            foreach (var tile in ormManager.LoadObjects<HexTerrainTileSettings>())
            {
                string displName = tile.DisplayName;
                TileBrush brush;
                if (!mTileBrushes.TryGetValue(displName, out brush))
                {
                    brush = new TileBrush(displName);
                    mTileBrushes.Add(displName, brush);
                }
                brush.AddTile(tile.Name);
            }
        }

        private void InitUnitTypes()
        {
            UnitTypes = new List<string>();
            var ormManager = Content.OrmManager;
            foreach (var unit in ormManager.LoadObjects<GameUnitSettings>())
            {
                UnitTypes.Add(unit.Name);
            }
        }
    }

    public class TileBrush
    {
        private List<string> mTiles = new List<string>();

        public string Name {get; private set;}

        public TileBrush(string name)
        {
            Name = name;
        }

        public void AddTile(string tile)
        {
            mTiles.Add(tile);
        }

        public string GetTile()
        {
            if (0 == mTiles.Count)
            {
                throw new InvalidOperationException("Brush is empty");
            }
            return mTiles[AleMathUtils.Random.Next(mTiles.Count)];
        }



    }
}
