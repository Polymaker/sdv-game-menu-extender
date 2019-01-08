using Polymaker.SdvUI.Controls;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.UI
{
    public class MenuExtenderConfigPage : SdvGameMenuForm
    {
        private SdvScrollableControl ConfigListPanel;
        private List<TabConfigControl> TabConfigControls;

        private SdvButton SaveButton;

        public MenuExtenderConfigPage()
        {
            TabConfigControls = new List<TabConfigControl>();
            //IntiliazeMenu();
        }

        public MenuExtenderConfigPage(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {
            TabConfigControls = new List<TabConfigControl>();
            //IntiliazeMenu();
        }

        internal void IntiliazeMenu()
        {
            var mainLabel = new SdvLabel()
            {
                Text = "Menu Extender Settings:",
                X = GAME_MENU_BORDER + 8,
                Y = GAME_MENU_BORDER + 8,
                Font = Game1.dialogueFont
            };
            Controls.Add(mainLabel);

            SaveButton = new SdvButton() { Text = "Save", Font = Game1.smallFont };
            SaveButton.X = Width - SaveButton.Width - Padding.Horizontal - GAME_MENU_BORDER - 8;
            SaveButton.Y = GAME_MENU_BORDER + 8;
            SaveButton.MouseClick += SaveButton_MouseClick;
            Controls.Add(SaveButton);

            ConfigListPanel = new SdvScrollableControl()
            {
                X = 16,
                Y = SaveButton.Bounds.Bottom + 16,
                Width = Width - 32 - Padding.Horizontal,
                Height = Height - GAME_MENU_BORDER - SaveButton.Bounds.Bottom - Padding.Vertical - 16
            };
            ConfigListPanel.VScrollBar.SmallChange = 40;
            ConfigListPanel.VScrollBar.LargeChange = 40;

            Controls.Add(ConfigListPanel);

            ReloadConfigs();
        }

        private void SaveButton_MouseClick(object sender, Polymaker.SdvUI.MouseEventArgs e)
        {
            GameMenuExtenderMod.Instance.MenuManager.ValidateTabConfigs();
            GameMenuExtenderMod.Instance.MenuManager.ReloadMenu();
            ReloadConfigs();

        }

        private void ReloadConfigs()
        {
            ConfigListPanel.Controls.Clear();
            TabConfigControls.Clear();
            var currentY = 0;
            foreach (var tab in GameMenuExtenderMod.Instance.MenuManager.AllTabs)
            {

                var tabConfigCtrl = new TabConfigControl(tab)
                {
                    Y = currentY,
                    Width = ConfigListPanel.Width - ConfigListPanel.VScrollBar.Width
                };
                ConfigListPanel.Controls.Add(tabConfigCtrl);
                TabConfigControls.Add(tabConfigCtrl);
                currentY += tabConfigCtrl.Height;
            }
        }
    }
}
