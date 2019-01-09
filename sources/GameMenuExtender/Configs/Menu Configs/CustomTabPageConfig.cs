using GameMenuExtender.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public class CustomTabPageConfig : MenuTabPageConfig
    {
        private bool _Visible;
        private string _Title;
        private int _Index;
        private bool _IsNonAPI;
        private bool _IsVanillaReplacement;

        public override bool Visible { get => _Visible; set => SetPropertyValue(ref _Visible, value); }

        public override string Title { get => string.IsNullOrEmpty(_Title) ? DefaultTitle  : _Title; set => SetPropertyValue(ref _Title, value); }

        public override int Index { get => _Index; set => SetPropertyValue(ref _Index, value); }

        public bool IsNonAPI
        {
            get => _IsNonAPI;
            set => SetPropertyValue(ref _IsNonAPI, value);
        }

        public bool IsVanillaReplacement
        {
            get => _IsVanillaReplacement;
            set => SetPropertyValue(ref _IsVanillaReplacement, value);
        }

        public override bool IsVanilla => false;

        public CustomTabPageConfig(GameMenuTabPage tabPage)
            : base(tabPage.Name, tabPage.Tab.Name)
        {
            IsNew = true;
            _Visible = true;
            _Title = tabPage.Label;
            DefaultTitle = tabPage.Label;
            _IsNonAPI = (tabPage is CustomTabPage tp && tp.IsNonAPI);
        }

        public CustomTabPageConfig(Serialization.CustomPageCfg pageConfig)
            : base(pageConfig.Name, pageConfig.TabName)
        {
            IsNew = false;
            _Visible = pageConfig.Visible;
            _Title = pageConfig.Title;
            _Index = pageConfig.Index;
            _IsNonAPI = pageConfig.IsNonAPI;
        }

        public Serialization.CustomPageCfg GetJsonObject()
        {
            return new Serialization.CustomPageCfg()
            {
                Name = Name,
                Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null : Title,
                Index = Index,
                Visible = Visible,
                IsNonAPI = IsNonAPI,
                VanillaReplacement = IsVanillaReplacement
            };
        }
    }
}
