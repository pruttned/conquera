using System;
using Ale.Scene;
using Ale.Gui;

namespace Conquera.Gui
{
    public class GameCardSlot : Control
    {
        private GameCardInfoDialog mDialog = new GameCardInfoDialog();

        public GameCard Card { get; set; }

        public override System.Drawing.SizeF Size
        {
            get
            {
                if (Card == null)
                {
                    return System.Drawing.SizeF.Empty;
                }
                return Card.Icon.Size;
            }
        }

        public GameCardSlot()
        {
            MouseEnter += new EventHandler<ControlEventArgs>(GameCardInfoSlot_MouseEnter);
            MouseLeave += new EventHandler<ControlEventArgs>(GameCardInfoSlot_MouseLeave);
        }

        protected override void OnDrawBackground()
        {
            if (Card != null)
            {
                Card.Icon.Draw(ScreenLocation);
            }
        }

        private void GameCardInfoSlot_MouseEnter(object sender, ControlEventArgs e)
        {
            if (Card != null)
            {
                mDialog.SetGameCard(Card);
                mDialog.Show(false);
            }
        }

        private void GameCardInfoSlot_MouseLeave(object sender, ControlEventArgs e)
        {
            mDialog.Hide();
        }
    }
}
