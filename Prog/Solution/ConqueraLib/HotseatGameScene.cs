﻿//////////////////////////////////////////////////////////////////////
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
    public class HotseatGameScene : BattleScene
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
        public HotseatGameScene(SceneManager sceneManager, ContentGroup content, OrmManager ormManager, BattleSceneHeader settings, HexTerrain terrain)
            : base(sceneManager, content, ormManager, settings, terrain)
        {
        }

        public new static HotseatGameScene Load(string mapName, SceneManager sceneManager, ContentGroup content)
        {
            return (HotseatGameScene)BattleScene.Load(mapName, "Hotseat", sceneManager, content);
        }

        public static IList<string> QueryMapFiles()
        {
            return BattleScene.QueryMapFiles("Hotseat");
        }

        protected override void CreatePlayers()
        {
            //todo
        }

        protected override BattleSceneHeader CreateGameSettings()
        {
            return new HotseatGameSceneSettings();
        }
    }
}
