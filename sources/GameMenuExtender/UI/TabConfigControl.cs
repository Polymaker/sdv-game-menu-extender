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
        public MenuTabConfig TabConfig { get; /*set;*/ }

        private SdvLabel TabNameLabel;
        private SdvCheckbox VisibleCheckbox;
        private SdvButton EditNameBtn;
        private List<TabPageConfigControl> PageControls;

        private const int TabPageStartY = 40;

        public TabConfigControl(MenuTabConfig tabConfig)
        {
            TabConfig = tabConfig;
            PageControls = new List<TabPageConfigControl>();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TabNameLabel = new SdvLabel()
            {
                X = 52,
                Y = 2,
                Font = new SdvFont(StardewValley.Game1.smallFont, false, true)
            };

            Controls.Add(TabNameLabel);

            EditNameBtn = new SdvButton()
            {
                X = 400,
                Y = 0,
                Text = "Rename",
                Padding = new Polymaker.SdvUI.Padding(8, 4, 8, 0),
                Visible = false
            };
            Controls.Add(EditNameBtn);

            VisibleCheckbox = new SdvCheckbox()
            {
                TooltipText = "Enabled",
                X = 8,
                Y = 2,
                //Visible = !TabConfig.IsVanilla,
                Enabled = TabConfig.IsCustom,
                Checked = TabConfig.Visible
            };
            VisibleCheckbox.CheckChanged += VisibleCheckbox_CheckChanged;
            //VisibleCheckbox.X = ClientRectangle.Width - VisibleCheckbox.Width - 8;
            Controls.Add(VisibleCheckbox);

            CreateTabPageControls();

            Height = GetPreferredSize().Y;
            RefreshInfo();
        }

        private void CreateTabPageControls()
        {
            var currentY = TabPageStartY;

            foreach (var tabPage in TabConfig.TabPages.OrderBy(p => p.Index))
            {
                var pageCtrl = new TabPageConfigControl(TabConfig, tabPage)
                {
                    X = 0,
                    Y = currentY,
                    Width = Width
                };
                PageControls.Add(pageCtrl);
                Controls.Add(pageCtrl);

                currentY += pageCtrl.Height;

                pageCtrl.ConfigChanged += PageCtrl_ConfigChanged;
                pageCtrl.MoveUpClicked += PageCtrl_MoveUpClicked;
                pageCtrl.MoveDownClicked += PageCtrl_MoveDownClicked;
            }
        }

        private void PageCtrl_MoveDownClicked(object sender, EventArgs e)
        {
            var pageCtrl = (TabPageConfigControl)sender;
            if (pageCtrl.PageConfig.Index < PageControls.Count - 1)
            {
                var otherPage = PageControls[pageCtrl.PageConfig.Index + 1];
                pageCtrl.PageConfig.Index += 1;
                otherPage.PageConfig.Index -= 1;
                
                ReorderTabPages();

                pageCtrl.RefreshInfo();
                otherPage.RefreshInfo();
            }
        }

        private void PageCtrl_MoveUpClicked(object sender, EventArgs e)
        {
            var pageCtrl = (TabPageConfigControl)sender;
            if (pageCtrl.PageConfig.Index > 0)
            {
                var otherPage = PageControls[pageCtrl.PageConfig.Index - 1];
                pageCtrl.PageConfig.Index -= 1;
                otherPage.PageConfig.Index += 1;

                ReorderTabPages();

                pageCtrl.RefreshInfo();
                otherPage.RefreshInfo();
            }
        }

        private void ReorderTabPages()
        {
            var currentY = TabPageStartY;
            PageControls = PageControls.OrderBy(c => c.PageConfig.Index).ToList();

            foreach (var pageCtrl in PageControls)
            {
                pageCtrl.Y = currentY;
                currentY += pageCtrl.Height;
            }
        }

        private void PageCtrl_ConfigChanged(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        private void VisibleCheckbox_CheckChanged(object sender, EventArgs e)
        {
            TabConfig.Visible = VisibleCheckbox.Checked;
            VisibleCheckbox.TooltipText = TabConfig.Visible ? "Enabled" : "Hidden";
            PageControls.ForEach(c => c.Enabled = TabConfig.Visible);
        }

        public void RefreshInfo()
        {
            var cleanTitle = TabConfig.Title.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            TabNameLabel.Text = $"Tab: {cleanTitle}";
            VisibleCheckbox.Checked = TabConfig.Visible;

            ReorderTabPages();
            foreach (var pageCtrl in PageControls)
                pageCtrl.RefreshInfo();
        }

        protected override void OnDraw(SdvGraphics g)
        {
            EditNameBtn.Visible = MouseOver && CursorPosition.Y < 40;
            base.OnDraw(g);
        }
    }
}
