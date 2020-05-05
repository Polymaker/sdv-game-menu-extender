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
                X = 30,
                Y = 30,
                Width = 180,
            };
            Controls.Add(cboTest);
            cboTest.Height = cboTest.GetPreferredSize().Y;
            cboTest.DataSource = new string[] { "Item #1", "Item #2", "Item #3" };
            cboTest.SelectedIndex = 1;

            var listView = new SdvListView()
            {
                X = 30,
                Y = 100,
                Width = width - 30 - GameMenuPadding.Horizontal,
                Height = height - 100 - GameMenuPadding.Vertical
            };
            Controls.Add(listView);

            listView.Columns.Add(new ListViewColumn()
            {
                 Text = "Col 1",
                 Width = 200,
                 HeaderAlignment = Polymaker.SdvUI.HorizontalAlignment.Right
            });

            listView.Columns.Add(new ListViewColumn()
            {
                Text = "Column #2",
                Width = 0.4f,
                TextAlignment = Polymaker.SdvUI.HorizontalAlignment.Center
            });

            listView.Columns.Add(new ListViewColumn()
            {
                Text = "Column #3",
                Width = 0.4f,
                HeaderAlignment = Polymaker.SdvUI.HorizontalAlignment.Center
            });



            var lvi = new ListViewItem("Hello");
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem("World"));
            lvi.SubItems.Add(new ListViewItem.ListViewSubItem("!!!!"));
            listView.Items.Add(lvi);
            var rng = new Random();
            for (int i = 0; i < 20; i++)
            {
                listView.Items.Add("Item #" + (i + 1), rng.Next(0,100).ToString(), "asdf");
            }
        }

        //public override void draw(SpriteBatch b)
        //{
        //    base.draw(b);
        //    Utility.drawTextWithShadow(b, "my custom page 2", Game1.dialogueFont, new Vector2(xPositionOnScreen + 40, yPositionOnScreen + 100), Game1.textColor);
        //}
    }
}
