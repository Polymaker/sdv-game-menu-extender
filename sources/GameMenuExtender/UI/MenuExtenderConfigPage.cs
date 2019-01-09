using GameMenuExtender.Configs;
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
        private ConfigManager CurrentConfigs;
        private SdvButton SaveButton;
        private SdvButton ReloadButton;

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
            CurrentConfigs = ConfigManager.Load();
            var mainLabel = new SdvLabel()
            {
                Text = "Menu Extender Settings:",
                X = GAME_MENU_BORDER + 8,
                Y = GAME_MENU_BORDER + 8,
                Font = new SdvFont(Game1.dialogueFont, false, true)
            };
            Controls.Add(mainLabel);

            ReloadButton = new SdvButton() { Text = "Reload", Font = Game1.smallFont };
            ReloadButton.X = ClientRectangle.Width - ReloadButton.Width - GAME_MENU_BORDER - 8;
            ReloadButton.Y = GAME_MENU_BORDER + 8;
            ReloadButton.Padding = new Polymaker.SdvUI.Padding(16, 4, 16, 4);
            Controls.Add(ReloadButton);
            ReloadButton.MouseClick += ReloadButton_MouseClick;

            SaveButton = new SdvButton() { Text = "Save", Font = Game1.smallFont };
            SaveButton.X = ReloadButton.X - SaveButton.Width - 16;
            SaveButton.Y = GAME_MENU_BORDER + 8;
            SaveButton.Padding = new Polymaker.SdvUI.Padding(16, 4, 16, 4);
            Controls.Add(SaveButton);
            SaveButton.MouseClick += SaveButton_MouseClick;

            ConfigListPanel = new SdvScrollableControl()
            {
                X = 16,
                Y = SaveButton.Bounds.Bottom + 8,
                Width = Width - 32 - Padding.Horizontal,
                Height = Height - GAME_MENU_BORDER - SaveButton.Bounds.Bottom - Padding.Vertical - 16
            };
            ConfigListPanel.VScrollBar.SmallChange = 40;
            ConfigListPanel.VScrollBar.LargeChange = 40;

            Controls.Add(ConfigListPanel);

            ReloadConfigs();
        }

        private void ReloadButton_MouseClick(object sender, Polymaker.SdvUI.MouseEventArgs e)
        {
            CurrentConfigs.Reload();
            ReloadConfigs();
        }

        private void SaveButton_MouseClick(object sender, Polymaker.SdvUI.MouseEventArgs e)
        {
            CurrentConfigs.Save();
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
                    Width = ConfigListPanel.ClientRectangle.Width - ConfigListPanel.VScrollBar.Width
                };
                ConfigListPanel.Controls.Add(tabConfigCtrl);
                TabConfigControls.Add(tabConfigCtrl);
                currentY += tabConfigCtrl.Height;
            }
        }
    }
}
