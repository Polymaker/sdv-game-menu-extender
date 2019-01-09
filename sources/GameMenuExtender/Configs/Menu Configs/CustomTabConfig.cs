using GameMenuExtender.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public class CustomTabConfig : MenuTabConfig
    {
        private int _Index;
        private bool _Visible;

        public override int Index { get => _Index; set => SetPropertyValue(ref _Index, value); }
        public override bool Visible { get => _Visible; set => SetPropertyValue(ref _Visible, value); }
        public override bool IsVanilla => false;
        public override GameMenuTabs Tab => GameMenuTabs.Custom;

        public CustomTabConfig(Serialization.CustomTabCfg tabConfig)
            : base(tabConfig)
        {
            _Index = tabConfig.Index;
            _Visible = tabConfig.Visible;
        }

        public CustomTabConfig(CustomTab tab)
            : base(tab)
        {
            _DefaultPage = tab.TabPages.FirstOrDefault()?.Name;
            IsNew = true;
        }

        public override Serialization.TabCfgBase GetJsonObject()
        {
            return new Serialization.CustomTabCfg()
            {
                Index = Index,
                Name = Name,
                Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null : Title,
                Visible = Visible
            };
        }
    }
}
