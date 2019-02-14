﻿using GameMenuExtender.Configs;
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
            //CurrentConfigs.LoadDefaultTitles();
            ConfigManager.ValidateAndAdjustTabsConfigs(CurrentConfigs, false);

            var mainLabel = new SdvLabel()
            {
                Text = "Menu Extender Settings:",
                X = GAME_MENU_BORDER + 8,
                Y = GAME_MENU_BORDER + 8,
                Font = new SdvFont(Game1.dialogueFont, false, true)
            };
            Controls.Add(mainLabel);

            ReloadButton = new SdvButton() { Text = "Reload" };
            ReloadButton.X = ClientRectangle.Width - ReloadButton.Width - GAME_MENU_BORDER - 8;
            ReloadButton.Y = GAME_MENU_BORDER + 16;
            ReloadButton.Padding = new Polymaker.SdvUI.Padding(16, 4, 16, 4);
            Controls.Add(ReloadButton);
            ReloadButton.MouseClick += ReloadButton_MouseClick;

            SaveButton = new SdvButton() { Text = "Save"};
            SaveButton.X = ReloadButton.X - SaveButton.Width - 8;
            SaveButton.Y = GAME_MENU_BORDER + 16;
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
            //CurrentConfigs.LoadDefaultTitles();
            ConfigManager.ValidateAndAdjustTabsConfigs(CurrentConfigs, false);
            ReloadConfigs();
        }

        private void SaveButton_MouseClick(object sender, Polymaker.SdvUI.MouseEventArgs e)
        {
            ConfigManager.ValidateAndAdjustTabsConfigs(CurrentConfigs, true);
            GameMenuExtenderMod.Instance.MenuManager.ReloadMenu();
            ReloadConfigs();
        }

        private void ReloadConfigs()
        {
            ConfigListPanel.Controls.Clear();
            TabConfigControls.Clear();
            var currentY = 0;

            var vanillaTabLbl = new SdvLabel()
            {
                Font = SdvFont.OptionFont,
                Text = "Default Tabs",
                Y = currentY,
                AutoSize = true
            };

            vanillaTabLbl.Padding = new Polymaker.SdvUI.Padding(6, 0, 0, 0);
            ConfigListPanel.Controls.Add(vanillaTabLbl);
            currentY += vanillaTabLbl.Height;

            bool customLblCreated = false;

            foreach (var tab in CurrentConfigs.TabConfigs)
            {
                if (tab.IsCustom && !customLblCreated)
                {
                    var customTabLbl = new SdvLabel()
                    {
                        Font = SdvFont.OptionFont,
                        Text = "Custom Tabs",
                        Y = currentY,
                        AutoSize = true
                    };

                    customTabLbl.Padding = new Polymaker.SdvUI.Padding(6, 0, 0, 0);
                    ConfigListPanel.Controls.Add(customTabLbl);
                    currentY += customTabLbl.Height;
                    customLblCreated = true;
                }

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
