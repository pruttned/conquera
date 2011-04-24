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
    public class HotseatGameScene : GameScene
    {
        public override string GameType
        {
            get { return "Hotseat"; }
        }
        public HotseatGameScene(string name, SceneManager sceneManager, int width, int height, string defaultTile, ContentGroup content)
            :base(name, sceneManager, width, height, defaultTile, content)
        {
        }

        /// <summary>
        /// Use only in GameSceneSettings
        /// </summary>
        public HotseatGameScene(SceneManager sceneManager, ContentGroup content, OrmManager ormManager, GameSceneSettings settings, HexTerrain terrain, GameSceneContextState gameSceneState)
            : base(sceneManager, content, ormManager, settings, terrain, gameSceneState)
        {
        }

        public new static HotseatGameScene Load(string mapName, SceneManager sceneManager, ContentGroup content)
        {
            return (HotseatGameScene)GameScene.Load(mapName, "Hotseat", sceneManager, content);
        }

        public static IList<string> QueryMapFiles()
        {
            return GameScene.QueryMapFiles("Hotseat");
        }

        protected override void CreatePlayers()
        {
            GameSceneContextState.Players.Add(new HumanPlayer("Blue", Color.Blue.ToVector3()));
            GameSceneContextState.Players.Add(new HumanPlayer("Red", Color.Red.ToVector3()));
            GameSceneContextState.Players[0].Mana = 1000;
            GameSceneContextState.Players[1].Mana = 1000;
        }

        protected override GameSceneSettings CreateGameSettings()
        {
            return new HotseatGameSceneSettings();
        }
    }
}
