using GameMenuExtender.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public class GameMenuTabPageConfig : ConfigBase
    {
        private bool _Visible;
        private string _Title;
        private int _Index;
        private bool _IsNonAPI;

        public string TabName { get; private set; }
        public string Name { get; private set; }
        public bool Visible { get => _Visible; set { _Visible = value; OnPropertyChanged(nameof(Visible)); } }
        public string Title { get => _Title; set { _Title = value; OnPropertyChanged(nameof(Title)); } }
        public string DefaultTitle { get; private set; }
        public int Index { get => _Index; set { _Index = value; OnPropertyChanged(nameof(Index)); } }
        public bool IsNonAPI { get => _IsNonAPI; set { _Visible = value; OnPropertyChanged(nameof(IsNonAPI)); } }

        public GameMenuTabPageConfig()
        {

        }

        public GameMenuTabPageConfig(GameMenuTabPage tabPage)
        {
            IsNew = true;
            TabName = tabPage.Tab.Name;
            Name = tabPage.Name;
            _Visible = true;
            _Title = tabPage.Label;
            DefaultTitle = tabPage.Label;
            _IsNonAPI = (tabPage is CustomTabPage tp && tp.IsNonAPI);
        }

        public GameMenuTabPageConfig(string tabName, GameMenuExtenderConfig.CustomTabPageConfig pageConfig)
        {
            IsNew = false;
            TabName = tabName;
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
