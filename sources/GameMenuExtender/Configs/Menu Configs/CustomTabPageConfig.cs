using GameMenuExtender.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public class CustomTabPageConfig : ConfigBase, IMenuTabPageConfig
    {
        private bool _Visible;
        private string _Title;
        private int _Index;
        private bool _IsNonAPI;

        public string TabName { get; private set; }
        public string Name { get; private set; }
        public bool Visible { get => _Visible; set => SetPropertyValue(ref _Visible, value); }
        public string Title { get => string.IsNullOrEmpty(_Title) ? DefaultTitle  : _Title; set => SetPropertyValue(ref _Title, value); }
        public int Index { get => _Index; set => SetPropertyValue(ref _Index, value); }

        public string DefaultTitle { get; set; }
        public bool IsNonAPI { get => _IsNonAPI; set { _Visible = value; OnPropertyChanged(nameof(IsNonAPI)); } }

        public bool IsVanilla => false;

        public CustomTabPageConfig(GameMenuTabPage tabPage)
        {
            IsNew = true;
            TabName = tabPage.Tab.Name;
            Name = tabPage.Name;
            _Visible = true;
            _Title = tabPage.Label;
            DefaultTitle = tabPage.Label;
            _IsNonAPI = (tabPage is CustomTabPage tp && tp.IsNonAPI);
        }

        public CustomTabPageConfig(GameMenuExtenderConfig.CustomTabPageConfig pageConfig)
        {
            IsNew = false;
            TabName = pageConfig.TabName;
            Name = pageConfig.Name;
            _Visible = pageConfig.Visible;
            _Title = pageConfig.Title;
            _Index = pageConfig.Index;
            _IsNonAPI = pageConfig.IsNonAPI;
        }

        public GameMenuExtenderConfig.CustomTabPageConfig GetConfigObject()
        {
            return new GameMenuExtenderConfig.CustomTabPageConfig()
            {
                Name = Name,
                Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null : Title,
                Index = Index,
                Visible = Visible,
                IsNonAPI = IsNonAPI
            };
        }
    }
}
