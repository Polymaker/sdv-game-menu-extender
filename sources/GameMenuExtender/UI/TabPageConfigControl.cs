using GameMenuExtender.Configs;
using GameMenuExtender.Menus;
using Polymaker.SdvUI;
using Polymaker.SdvUI.Controls;
using Polymaker.SdvUI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.UI
{
    public class TabPageConfigControl : SdvContainerControl
    {
        public MenuTabConfig TabConfig { get; }
        public MenuTabPageConfig PageConfig { get; /*set;*/ }


        private SdvLabel PageNameLabel;
        private SdvCheckbox VisibleCheckbox;
        private SdvButton EditNameBtn;
        private SdvButton UpArrowBtn;
        private SdvButton DownArrowBtn;
        private bool CanShowArrows;
        private bool IsLoading;

        public event EventHandler ConfigChanged;
        public event EventHandler MoveUpClicked;
        public event EventHandler MoveDownClicked;
        public event EventHandler VisibilityChanged;

        public TabPageConfigControl(MenuTabConfig tabConfig, MenuTabPageConfig pageConfig)
        {
            TabConfig = tabConfig;
            PageConfig = pageConfig;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            PageNameLabel = new SdvLabel() {
                X = 76,
                Y = 3,
                Font = new SdvFont(StardewValley.Game1.smallFont, false, true)
            };
            Controls.Add(PageNameLabel);

            EditNameBtn = new SdvButton()
            {
                X = 400,
                Y = 0,
                Text = "Rename",
                Padding = new Polymaker.SdvUI.Padding(8, 4, 8, 0),
                Visible = false
            };
            Controls.Add(EditNameBtn);


            DownArrowBtn = new SdvButton()
            {
                Y = 0,
                Padding = new Polymaker.SdvUI.Padding(8),
                Image = SdvImages.DownArrow,
                ImageScale = 2f,
            };
            DownArrowBtn.Click += DownArrowBtn_Click;
            Controls.Add(DownArrowBtn);
            
            UpArrowBtn = new SdvButton()
            {
                Y = 0,
                Padding = new Polymaker.SdvUI.Padding(8),
                Image = SdvImages.UpArrow,
                ImageScale = 2f,
            };
            UpArrowBtn.Click += UpArrowBtn_Click;
            Controls.Add(UpArrowBtn);

            //DownArrowBtn.X = ClientRectangle.Width - DownArrowBtn.Width - 8;
            //UpArrowBtn.X = DownArrowBtn.X - UpArrowBtn.Width - 8;

            UpArrowBtn.X = ClientRectangle.Width - UpArrowBtn.Width - 8;
            DownArrowBtn.X = UpArrowBtn.X - DownArrowBtn.Width - 8;

            VisibleCheckbox = new SdvCheckbox()
            {
                //Text = "Hide",
                Y = 2,
                TooltipText = "Enabled",
                Checked = PageConfig.Visible
            };
            Controls.Add(VisibleCheckbox);

            VisibleCheckbox.CheckChanged += VisibleCheckbox_CheckChanged;
            //VisibleCheckbox.X = DownArrowBtn.X - VisibleCheckbox.Width - 16;
            VisibleCheckbox.X = 32;

            Height = 40;
            RefreshInfo();
        }

        private void UpArrowBtn_Click(object sender, EventArgs e)
        {
            MoveUpClicked?.Invoke(this, e);
        }

        private void DownArrowBtn_Click(object sender, EventArgs e)
        {
            MoveDownClicked?.Invoke(this, e);
        }

        private void VisibleCheckbox_CheckChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                PageConfig.Visible = VisibleCheckbox.Checked;
                OnVisibilityChanged();
            }

            VisibleCheckbox.TooltipText = PageConfig.Visible ? "Enabled" : "Hidden";
        }

        public void RefreshInfo()
        {
            IsLoading = true;
            var cleanTitle = (PageConfig.Title ?? "Not set").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            PageNameLabel.Text = $"Page: {cleanTitle}";

            var visiblePageCount = TabConfig.TabPages.Count(p => p.Visible);
            bool canHidePage = true;

            if (visiblePageCount <= 1)
                canHidePage = false;

            if(PageConfig.IsVanilla && TabConfig is VanillaTabConfig vTab && string.IsNullOrEmpty(vTab.VanillaPageOverride))
                canHidePage = false;

            if (PageConfig.ModID == "Polymaker.GameMenuExtender")
                canHidePage = false;

            VisibleCheckbox.Enabled = !PageConfig.Visible || canHidePage;
            VisibleCheckbox.Checked = PageConfig.Visible;

            if (TabConfig is VanillaTabConfig vanillaTab && !string.IsNullOrEmpty(vanillaTab.VanillaPageOverride))
            {
                VisibleCheckbox.Enabled = false;
            }

            CanShowArrows = TabConfig.TabPages.Count(p => p.Visible) > 1;
      
            UpArrowBtn.Enabled = PageConfig.Index > 0;
            DownArrowBtn.Enabled = PageConfig.Index < visiblePageCount - 1;

            IsLoading = false;
        }


        protected override void OnDraw(SdvGraphics g)
        {
            UpArrowBtn.Visible = DownArrowBtn.Visible = CanShowArrows && (/*ContainsFocus || */MouseOver);
            //EditNameBtn.Visible = MouseOver;
            base.OnDraw(g);
        }

        private void OnConfigChanged()
        {
            ConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnVisibilityChanged()
        {
            VisibilityChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
