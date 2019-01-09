using GameMenuExtender.Configs;
using GameMenuExtender.Menus;
using Polymaker.SdvUI;
using Polymaker.SdvUI.Controls;
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

        public event EventHandler ConfigChanged;

        public TabPageConfigControl(MenuTabConfig tabConfig, MenuTabPageConfig pageConfig)
        {
            TabConfig = tabConfig;
            PageConfig = pageConfig;
            Padding = new Polymaker.SdvUI.Padding(8, 2, 0, 2);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            PageNameLabel = new SdvLabel() {
                X = 32,
                Font = new SdvFont(StardewValley.Game1.smallFont, false, true)
            };
            Controls.Add(PageNameLabel);

            EditNameBtn = new SdvButton()
            {
                X = 300,
                Text = "Edit",
                Font = new SdvFont(StardewValley.Game1.smallFont, false, false, 0.8f),
                Padding = new Polymaker.SdvUI.Padding(8)
            };
            Controls.Add(EditNameBtn);


            DownArrowBtn = new SdvButton()
            {
                Padding = new Polymaker.SdvUI.Padding(8),
                X = 10
            };
            var img = SdvImages.DownArrow;
            img.Scale = 2f;
            DownArrowBtn.Image = img;

            Controls.Add(DownArrowBtn);
            DownArrowBtn.X = ClientRectangle.Width - DownArrowBtn.Width - 8;

            UpArrowBtn = new SdvButton()
            {
                Padding = new Polymaker.SdvUI.Padding(8),
                X = DownArrowBtn.X - DownArrowBtn.Width - 8
            };
            img = SdvImages.UpArrow;
            img.Scale = 2f;
            UpArrowBtn.Image = img;
            
            Controls.Add(UpArrowBtn);

            VisibleCheckbox = new SdvCheckbox()
            {
                Text = "Visible",
                Checked = PageConfig.Visible
            };

            VisibleCheckbox.CheckChanged += VisibleCheckbox_CheckChanged;
            VisibleCheckbox.X = UpArrowBtn.X - VisibleCheckbox.Width - 16;
            Controls.Add(VisibleCheckbox);

            Height = GetPreferredSize().Y;
            RefreshInfo();
        }

        private void VisibleCheckbox_CheckChanged(object sender, EventArgs e)
        {
            PageConfig.Visible = VisibleCheckbox.Checked;

            //if (!PageConfig.Visible && GameMenuElement.NameEquals(MenuTab.Configuration.DefaultPage, PageConfig.Name))
            //{
            //    MenuTab.Configuration.DefaultPage = MenuTab.TabPages.FirstOrDefault(p=>p.Visible).Name;
            //}

            OnConfigChanged();
        }

        public void RefreshInfo()
        {
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

            UpdateArrowsVisibillity();

            UpArrowBtn.Enabled = PageConfig.Index > 0;
            DownArrowBtn.Enabled = PageConfig.Index < visiblePageCount - 1;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateArrowsVisibillity();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateArrowsVisibillity();
        }

        private void UpdateArrowsVisibillity()
        {
            var visiblePageCount = TabConfig.TabPages.Count(p => p.Visible);
            UpArrowBtn.Visible = DownArrowBtn.Visible = visiblePageCount > 1;// && (/*Focused ||*/ DisplayRectangle.Contains(CursorPosition));
        }

        private void OnConfigChanged()
        {
            ConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
