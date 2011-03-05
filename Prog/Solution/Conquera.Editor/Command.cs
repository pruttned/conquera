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
using System.Windows.Forms;

namespace Conquera.Editor
{
    public interface ICommand
    {
        string Name { get; }
        /// <summary>
        /// Whether to remove duplicit enqued commands of a same name
        /// </summary>
        bool RemoveEnquedDuplicates { get; }
        void Execute(EditorApplication app);
    }



    public class NewMapInfo
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class NewMapCommand : ICommand
    {
        private NewMapInfo mNewMapInfo;
        ToolBarForm mToolBarForm;

        public string Name { get { return "NewMap"; } }
        public bool RemoveEnquedDuplicates
        {
            get { return true; }
        }

        public NewMapCommand(NewMapInfo newMapInfo, ToolBarForm toolBarForm)
        {
            mNewMapInfo = newMapInfo;
            mToolBarForm = toolBarForm;
        }

        public void Execute(EditorApplication app)
        {
            app.GameScene = new HotseatGameScene(mNewMapInfo.Name, app.SceneManager, mNewMapInfo.Width, mNewMapInfo.Height,
                "Grass1Tile", app.Content.DefaultContentGroup);
            mToolBarForm.Invoke(new InitPlayersDel(InitPlayers), mToolBarForm);
        }

        public override string ToString()
        {
            return Name;
        }

        delegate void InitPlayersDel(ToolBarForm form);
        private void InitPlayers(ToolBarForm form)
        {
            form.InitPlayers();
        }
    }

    public class SetTileBrushCommand : ICommand
    {
        private string mBrushName;

        public string Name { get { return "SetTileBrush"; } }
        public bool RemoveEnquedDuplicates
        {
            get { return true; }
        }

        public SetTileBrushCommand(string brushName)
        {
            mBrushName = brushName;
        }

        public void Execute(EditorApplication app)
        {
            app.TileBrush = mBrushName;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SetEditModeCommand : ICommand
    {
        private EditMode mMode;

        public string Name { get { return "SetEditMode"; } }
        public bool RemoveEnquedDuplicates
        {
            get { return true; }
        }

        public SetEditModeCommand(EditMode mode)
        {
            mMode = mode;
        }

        public void Execute(EditorApplication app)
        {
            app.EditMode = mMode;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SetPlayerCommand : ICommand
    {
        private GamePlayer mPlayer;

        public string Name { get { return "SetPlayer"; } }
        public bool RemoveEnquedDuplicates
        {
            get { return true; }
        }

        public SetPlayerCommand(GamePlayer player)
        {
            mPlayer = player;
        }

        public void Execute(EditorApplication app)
        {
            app.Player = mPlayer;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SetUnitTypeCommand : ICommand
    {
        private string mUnitType;

        public string Name { get { return "SetTileBrush"; } }
        public bool RemoveEnquedDuplicates
        {
            get { return true; }
        }

        public SetUnitTypeCommand(string unitType)
        {
            mUnitType = unitType;
        }

        public void Execute(EditorApplication app)
        {
            app.UnitType = mUnitType;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SaveMapCommand : ICommand
    {
        ToolBarForm mToolBarForm;
        public string Name { get { return "SaveMap"; } }
        public bool RemoveEnquedDuplicates
        {
            get { return false; }
        }

        public SaveMapCommand(ToolBarForm toolBarForm)
        {
            mToolBarForm = toolBarForm;
        }

        public void Execute(EditorApplication app)
        {
            app.GameScene.SaveMap();

            mToolBarForm.Invoke(new ShowMsgBoxDel(ShowMsgBox), mToolBarForm);
        }

        public override string ToString()
        {
            return Name;
        }

        delegate void ShowMsgBoxDel(ToolBarForm form);
        private void ShowMsgBox(ToolBarForm form)
        {
            MessageBox.Show(form, "Saved");
        }
    }


    public class LoadMapCommand : ICommand
    {
        private string mFileName;
        ToolBarForm mToolBarForm;

        public string Name { get { return "LoadMap"; } }
        public bool RemoveEnquedDuplicates
        {
            get { return true; }
        }

        public LoadMapCommand(string fileName, ToolBarForm toolBarForm)
        {
            mFileName = fileName;
            mToolBarForm = toolBarForm;
        }

        public void Execute(EditorApplication app)
        {
            app.GameScene = GameScene.Load(mFileName, app.SceneManager, app.Content.DefaultContentGroup);
            mToolBarForm.Invoke(new InitPlayersDel(InitPlayers), mToolBarForm);
        }

        public override string ToString()
        {
            return Name;
        }

        delegate void InitPlayersDel(ToolBarForm form);
        private void InitPlayers(ToolBarForm form)
        {
            form.InitPlayers();
        }
    }

}
