using GameMenuExtender.Menus;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public class VanillaTabConfig : MenuTabConfig
    {
        //private bool _HideVanillaPage;
        private int _VanillaPageIndex;
        private string _VanillaPageTitle;
        public string _VanillaPageOverride;
        private GameMenuTabs _Tab;

        public bool HideVanillaPage => !string.IsNullOrEmpty(VanillaPageOverride);

        public int VanillaPageIndex { get => _VanillaPageIndex; set => SetPropertyValue(ref _VanillaPageIndex, value); }

        public string VanillaPageTitle { get => string.IsNullOrEmpty(_VanillaPageTitle) ? DefaultVanillaTitle : _VanillaPageTitle; set => SetPropertyValue(ref _VanillaPageTitle, value); }

        public string DefaultVanillaTitle { get; set; }

        public override int Index { get => (int)Tab; set { } }

        public override bool Visible { get => true; set  { } }

        public string VanillaPageOverride { get => _VanillaPageOverride; set => SetPropertyValue(ref _VanillaPageOverride, value); }

        public override bool IsVanilla => true;

        public override GameMenuTabs Tab => _Tab;

        public VanillaTabConfig(Serialization.VanillaTabCfg tabConfig)
            : base(tabConfig)
        {
            //_HideVanillaPage = !tabConfig.VanillaPageVisible;
            _VanillaPageIndex = tabConfig.VanillaPageIndex;
            _VanillaPageTitle = tabConfig.VanillaPageTitle;
            _Tab = tabConfig.MenuTab;
        }

        public VanillaTabConfig(VanillaTab tab)
            : base(tab)
        {
            //_DefaultPage = tab.VanillaPage.Name;
            //_HideVanillaPage = false;
            _VanillaPageTitle = tab.Label;
            _VanillaPageIndex = 0;
            _Tab = tab.TabName;
        }

        public override Serialization.TabCfgBase GetJsonObject()
        {
            return new Serialization.VanillaTabCfg()
            {
                VanillaPageOverride = VanillaPageOverride,
                Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null : Title,
                //VanillaPageVisible = !HideVanillaPage,
                VanillaPageTitle = (!string.IsNullOrEmpty(DefaultVanillaTitle) && DefaultVanillaTitle == VanillaPageTitle) ? null : VanillaPageTitle,
                VanillaPageIndex = VanillaPageIndex
            };
        }
    }
}
