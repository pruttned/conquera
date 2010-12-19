//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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


using Ale.Gui;
namespace Conquera.Gui
{
    public static class AlCursors
    {
        public static readonly CursorInfo Default = new CursorInfo(GuiManager.Instance.Palette.CreateGraphicElement("CursorDefault"),
                                                                   GuiManager.Instance.Palette.CreateRectangle("CursorDefaultHotSpot").Location);

        public static readonly CursorInfo Attack = new CursorInfo(GuiManager.Instance.Palette.CreateGraphicElement("CursorAttack"),
                                                                  GuiManager.Instance.Palette.CreateRectangle("CursorAttackHotSpot").Location);

        public static readonly CursorInfo Move = new CursorInfo(GuiManager.Instance.Palette.CreateGraphicElement("CursorMove"),
                                                                GuiManager.Instance.Palette.CreateRectangle("CursorMoveHotSpot").Location);

        public static readonly CursorInfo MoveDisabled = new CursorInfo(GuiManager.Instance.Palette.CreateGraphicElement("CursorMoveDisabled"),
                                                                        GuiManager.Instance.Palette.CreateRectangle("CursorMoveDisabledHotSpot").Location);
    }
}
