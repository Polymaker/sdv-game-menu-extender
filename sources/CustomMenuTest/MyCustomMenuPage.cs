﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuTest
{
	class MyCustomMenuPage : IClickableMenu
	{

        public MyCustomMenuPage(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            Utility.drawTextWithShadow(b, "my custom page", Game1.dialogueFont, new Vector2(xPositionOnScreen + 40, yPositionOnScreen + 100), Game1.textColor);
        }
    }

    class MyCustomMenuPage2 : IClickableMenu
    {

        public MyCustomMenuPage2(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            Utility.drawTextWithShadow(b, "my custom page 2", Game1.dialogueFont, new Vector2(xPositionOnScreen + 40, yPositionOnScreen + 100), Game1.textColor);
        }
    }
}
