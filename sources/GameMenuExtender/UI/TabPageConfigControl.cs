using GameMenuExtender.Configs;
using GameMenuExtender.Menus;
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
        public GameMenuTabPage TabPage { get; }
        public GameMenuTab MenuTab => TabPage.Tab;
        public IMenuTabPageConfig PageConfig { get; /*set;*/ }

        private SdvLabel PageNameLabel;
        private SdvCheckbox VisibleCheckbox;

        public event EventHandler ConfigChanged;

        public TabPageConfigControl(GameMenuTabPage tabPage)
        {
            TabPage = tabPage;
            PageConfig = tabPage.Configuration;
            Padding = new Polymaker.SdvUI.Padding(2);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            PageNameLabel = new SdvLabel() {
                X = 16
            };
            Controls.Add(PageNameLabel);

            VisibleCheckbox = new SdvCheckbox()
            {
                Text = "Visible",
                Checked = PageConfig.Visible
            };
            VisibleCheckbox.CheckChanged += VisibleCheckbox_CheckChanged;
            VisibleCheckbox.X = Width - VisibleCheckbox.Width - Padding.Right - 8;
            Controls.Add(VisibleCheckbox);

            Height = GetPreferredSize().Y;
            RefreshInfo();
        }

        private void VisibleCheckbox_CheckChanged(object sender, EventArgs e)
        {
            PageConfig.Visible = VisibleCheckbox.Checked;

            if (!PageConfig.Visible && GameMenuElement.NameEquals(MenuTab.Configuration.DefaultPage, PageConfig.Name))
            {
                MenuTab.Configuration.DefaultPage = MenuTab.TabPages.FirstOrDefault(p=>p.Visible).Name;
            }

            OnConfigChanged();
        }

        public void RefreshInfo()
        {
            var cleanTitle = (PageConfig.Title ?? "Not set").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            PageNameLabel.Text = $"Page: {cleanTitle}";
            VisibleCheckbox.Checked = PageConfig.Visible;

            VisibleCheckbox.Enabled = !PageConfig.Visible || MenuTab.TabPages.Count(p => p.Visible) > 1;
            if (TabPage.PageType == typeof(MenuExtenderConfigPage))
                VisibleCheckbox.Enabled = false;
        }

        private void OnConfigChanged()
        {
            ConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
