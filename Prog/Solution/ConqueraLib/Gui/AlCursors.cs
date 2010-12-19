
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