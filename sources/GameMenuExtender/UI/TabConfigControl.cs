using GameMenuExtender.Configs;
using GameMenuExtender.Menus;
using Polymaker.SdvUI.Controls;
using Polymaker.SdvUI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.UI
{
    public class TabConfigControl : SdvContainerControl
    {
        public GameMenuTab MenuTab { get; set; }
        public MenuTabConfig TabConfig { get; /*set;*/ }

        private SdvLabel TabNameLabel;
        private SdvCheckbox VisibleCheckbox;
        private List<TabPageConfigControl> PageControls;

        public TabConfigControl(GameMenuTab tab)
        {
            MenuTab = tab;
            TabConfig = (MenuTabConfig)tab.Configuration;

            PageControls = new List<TabPageConfigControl>();
            Padding = new Polymaker.SdvUI.Padding(8, 2, 0, 2);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TabNameLabel = new SdvLabel()
            {
                Font = new SdvFont(StardewValley.Game1.smallFont, false, true)
            };

            Controls.Add(TabNameLabel);

            VisibleCheckbox = new SdvCheckbox()
            {
                Text = "Visible",
                Visible = !TabConfig.IsVanilla,
                Checked = TabConfig.Visible
            };
            VisibleCheckbox.CheckChanged += VisibleCheckbox_CheckChanged;
            VisibleCheckbox.X = ClientRectangle.Width - VisibleCheckbox.Width - 8;
            Controls.Add(VisibleCheckbox);

            var currentY = VisibleCheckbox.Bounds.Bottom + 2;

            foreach (var tabPage in MenuTab.TabPages.OrderBy(p => p.DisplayIndex))
            {
                var pageCtrl = new TabPageConfigControl(tabPage)
                {
                    X = 0,
                    Y = currentY,
                    Width = Width
                };
                
                PageControls.Add(pageCtrl);
                Controls.Add(pageCtrl);

                currentY += pageCtrl.Height;
                pageCtrl.ConfigChanged += PageCtrl_ConfigChanged;
            }

            Height = GetPreferredSize().Y - (PageControls.Count > 0 ? 2 : 0);
            RefreshInfo();
        }

        private void PageCtrl_ConfigChanged(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        private void VisibleCheckbox_CheckChanged(object sender, EventArgs e)
        {
            TabConfig.Visible = VisibleCheckbox.Checked;
            PageControls.ForEach(c => c.Enabled = VisibleCheckbox.Checked);
        }

        public void RefreshInfo()
        {
            var cleanTitle = TabConfig.Title.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            TabNameLabel.Text = $"Tab: {cleanTitle}";
            VisibleCheckbox.Checked = TabConfig.Visible;

            //TabConfig.TabPages = GameMenuExtenderMod.Instance.Configs.GetTabPagesConfig(TabConfig);
            var ctrlHeight = PageControls[0].Height;

            foreach (var pageCtrl in PageControls)
            {
                pageCtrl.RefreshInfo();
                pageCtrl.Y = VisibleCheckbox.Bounds.Bottom + 2 + (pageCtrl.TabPage.DisplayIndex * ctrlHeight);
            }
        }

        protected override void OnDraw(SdvGraphics g)
        {
            base.OnDraw(g);
        }
    }
}
