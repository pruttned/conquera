using Ale.Gui;
using Microsoft.Xna.Framework;

namespace Conquera.Gui
{
    public class SpellPanel : Control
    {
        private SpellSlotButton[] mButtons;
        private SpellSlotCollection mSpellSlotCollection;

        public SpellSlotCollection SpellSlotCollection
        {
            get { return mSpellSlotCollection; }
            set
            {
                if (value != mSpellSlotCollection)
                {
                    mSpellSlotCollection = value;

                    if (mSpellSlotCollection == null)
                    {
                        for (int i = 0; i < mButtons.Length; i++)
                        {
                            mButtons[i].SpellSlot = null;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < mButtons.Length; i++)
                        {
                            mButtons[i].SpellSlot = mSpellSlotCollection[i];
                        }
                    }
                }
            }
        }

        public override System.Drawing.SizeF Size
        {
            get
            {
                //TODO: from palette
                return new System.Drawing.SizeF(800, 70);
            }
        }

        public SpellPanel(GameScene gameScene)
        {
            mButtons = new SpellSlotButton[SpellSlotCollection.Spells.Length];
            for (int i = 0; i < mButtons.Length; i++)
            {
                mButtons[i] = new SpellSlotButton(gameScene);
                mButtons[i].Location = new Point(i * 72, 0);
                ChildControls.Add(mButtons[i]);
            }
        }
    }
}