using GameMenuExtender.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public class CustomTabConfig : GameMenuTabConfig
    {
        private int _Index;
        private bool _Visible;

        public override int Index { get => _Index; set => SetPropertyValue(ref _Index, value); }
        public override bool Visible { get => _Visible; set => SetPropertyValue(ref _Visible, value); }

        public override bool IsVanilla => false;
        public override GameMenuTabs Tab => GameMenuTabs.Custom;

        public override string PageTitle { get { return null; } set { } }
        public override string DefaultPageTitle { get { return null; } set { } }
        public override bool PageVisible { get { return false; } set { } }
        public override int PageIndex { get { return -1; } set { } }
        public string ModID => Name.Split(':')[0];

        public CustomTabConfig(GameMenuExtenderConfig.CustomTabConfig tabConfig)
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

        public override GameMenuExtenderConfig.TabConfig GetConfigObject()
        {
            return new GameMenuExtenderConfig.CustomTabConfig()
            {
                Index = Index,
                Name = Name,
                Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null : Title,
                Visible = Visible,
                DefaultPage = DefaultPage
            };
        }
    }
}
