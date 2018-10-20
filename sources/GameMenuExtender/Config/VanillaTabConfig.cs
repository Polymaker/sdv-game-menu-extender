using GameMenuExtender.Menus;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public class VanillaTabConfig : GameMenuTabConfig
    {
        private bool _VanillaPageVisible;
        private int _VanillaPageIndex;
        private string _VanillaPageTitle;
        private GameMenuTabs _Tab;

        public override bool PageVisible { get => _VanillaPageVisible; set => SetPropertyValue(ref _VanillaPageVisible, value); }

        public override int PageIndex { get => _VanillaPageIndex; set => SetPropertyValue(ref _VanillaPageIndex, value); }

        public override string PageTitle { get => string.IsNullOrEmpty(_VanillaPageTitle) ? DefaultPageTitle : _VanillaPageTitle; set => SetPropertyValue(ref _VanillaPageTitle, value); }

        public override string DefaultPageTitle { get; set; }

        public override int Index { get => (int)Tab; set { } }

        public override bool Visible { get => true; set  { } }

        public override bool IsVanilla => true;

        public override GameMenuTabs Tab => _Tab;

        public VanillaTabConfig(GameMenuExtenderConfig.VanillaTabConfig tabConfig)
            : base(tabConfig)
        {
            _VanillaPageVisible = tabConfig.VanillaPageVisible;
            _VanillaPageIndex = tabConfig.VanillaPageIndex;
            _VanillaPageTitle = tabConfig.VanillaPageTitle;
            _Tab = tabConfig.MenuTab;
        }

        public VanillaTabConfig(VanillaTab tab)
            : base(tab)
        {
            _DefaultPage = tab.VanillaPage.Name;
            _VanillaPageVisible = true;
            _VanillaPageTitle = tab.Label;
            _VanillaPageIndex = 0;
            _Tab = tab.TabName;
        }

        public override GameMenuExtenderConfig.TabConfig GetConfigObject()
        {
            return new GameMenuExtenderConfig.VanillaTabConfig()
            {
                DefaultPage = DefaultPage,
                Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null : Title,
                VanillaPageVisible = PageVisible,
                VanillaPageTitle = (!string.IsNullOrEmpty(DefaultPageTitle) && DefaultPageTitle == PageTitle) ? null : PageTitle
            };
        }
    }
}
