using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI.Controls;
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

    class MyCustomMenuPage2 : SdvGameMenuForm
    {

        public MyCustomMenuPage2(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {
            var cboTest = new SdvComboBox()
            {
                Width = 150,
                Height = 32
            };
            Controls.Add(cboTest);
            cboTest.DataSource = new string[] { "Item #1", "Item #2" };
            
        }

        //public override void draw(SpriteBatch b)
        //{
        //    base.draw(b);
        //    Utility.drawTextWithShadow(b, "my custom page 2", Game1.dialogueFont, new Vector2(xPositionOnScreen + 40, yPositionOnScreen + 100), Game1.textColor);
        //}
    }
}
