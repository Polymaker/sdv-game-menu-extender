using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.UI
{
    class MenuExtenderConfigPage : IClickableMenu
    {
        private ClickableTextureComponent upArrow;
        private ClickableTextureComponent downArrow;
        private ClickableTextureComponent scrollBar;
        private Rectangle scrollBarRunner;

        public MenuExtenderConfigPage(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {
            upArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            downArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
            scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 12, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, height - 128 - upArrow.bounds.Height - 8);
        }

        public override void draw(SpriteBatch b)
        {
            SpriteText.drawString(b, "Menu Extender Settings:", xPositionOnScreen + 48, yPositionOnScreen + 112, 999, -1, 999, 1f, 0.1f);
            //Utility.drawTextWithShadow(b, "Menu Extender Settings", Game1.dialogueFont, new Vector2(xPositionOnScreen + 45, yPositionOnScreen + 120), Game1.textColor);

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            if (!GameMenu.forcePreventClose)
            {
                upArrow.draw(b);
                downArrow.draw(b);
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f, drawShadow: false);
                scrollBar.draw(b);
            }
        }
    }
}
