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
        private SdvComboBox VanillaOverrideCbo;
        private List<TabPageConfigControl> PageControls;
        private bool IsLoading;

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

            VanillaOverrideCbo = new SdvComboBox()
            {
                X = EditNameBtn.Bounds.Right + 16,
                Y = 0,
                Width = 200,
                Visible = TabConfig.TabPages.Count > 1
            };
            Controls.Add(VanillaOverrideCbo);
            VanillaOverrideCbo.DataSource = TabConfig.TabPages;
            VanillaOverrideCbo.DisplayMember = "Title";

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
                
                pageCtrl.MoveUpClicked += PageCtrl_MoveUpClicked;
                pageCtrl.MoveDownClicked += PageCtrl_MoveDownClicked;
                pageCtrl.VisibilityChanged += PageCtrl_VisibilityChanged;
            }
        }

        #region TabPage Controls Events

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

        private void PageCtrl_VisibilityChanged(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        #endregion

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
       

        private void VisibleCheckbox_CheckChanged(object sender, EventArgs e)
        {
            VisibleCheckbox.TooltipText = TabConfig.Visible ? "Enabled" : "Hidden";
            if (!IsLoading)
            {
                TabConfig.Visible = VisibleCheckbox.Checked;
                RefreshInfo();
            }
        }

        public void RefreshInfo()
        {
            IsLoading = true;

            var cleanTitle = TabConfig.Title.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");

            TabNameLabel.Text = $"Tab: {cleanTitle}";

            VisibleCheckbox.Checked = TabConfig.Visible;

            ReorderTabPages();
            foreach (var pageCtrl in PageControls)
            {
                pageCtrl.Enabled = TabConfig.Visible;
                pageCtrl.RefreshInfo();
            }

            IsLoading = false;
        }

        protected override void OnDraw(SdvGraphics g)
        {
            EditNameBtn.Visible = MouseOver && CursorPosition.Y < 40;
            base.OnDraw(g);
        }
    }
}
