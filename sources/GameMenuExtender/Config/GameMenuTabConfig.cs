using GameMenuExtender.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public class GameMenuTabConfig : ConfigBase
    {
        private bool _Visible;
        private string _Title;
        private int _Index;
        private string _DefaultPage;
        private bool _VanillaPageVisible;
        private int _VanillaPageIndex;
        private string _VanillaPageTitle;

        public GameMenuTabs Tab { get; private set; }

        public string Name { get; private set; }
        public bool Visible { get => _Visible; set { _Visible = value; OnPropertyChanged(nameof(Visible)); } }
        public string Title { get => _Title; set { _Title = value; OnPropertyChanged(nameof(Title)); } }
        public int Index { get => _Index; set { _Index = value; OnPropertyChanged(nameof(Index)); } }
        public string DefaultPage { get => _DefaultPage; set  { _DefaultPage = value; OnPropertyChanged(nameof(DefaultPage)); } }
        public bool IsVanilla { get; private set; }
        public bool VanillaPageVisible { get => _VanillaPageVisible; set { _VanillaPageVisible = value; OnPropertyChanged(nameof(VanillaPageVisible)); } }
        public int VanillaPageIndex { get => _VanillaPageIndex; set { _VanillaPageIndex = value; OnPropertyChanged(nameof(VanillaPageIndex)); } }
        public string VanillaPageTitle { get => _VanillaPageTitle; set { _VanillaPageTitle = value; OnPropertyChanged(nameof(VanillaPageTitle)); } }
        public string DefaultTitle { get; private set; }

        public GameMenuTabConfig()
        {
        }

        public GameMenuTabConfig(GameMenuExtenderConfig.VanillaTabConfig tabConfig, string name)
        {
            Name = name;
            _Visible = true;
            _Title = tabConfig.Title;
            _DefaultPage = tabConfig.DefaultPage;
            IsVanilla = true;
            _VanillaPageVisible = tabConfig.VanillaPageVisible;
            _VanillaPageIndex = tabConfig.VanillaPageIndex;
            _VanillaPageTitle = tabConfig.VanillaPageTitle;
            Tab = (GameMenuTabs)Enum.Parse(typeof(GameMenuTabs), name, true);
            IsNew = false;
        }

        public GameMenuTabConfig(VanillaTab tab)
        {
            Name = tab.Name;
            _Visible = true;
            _Title = tab.Label;
            _DefaultPage = tab.VanillaPage.Name;
            IsVanilla = true;
            _VanillaPageVisible = true;
            _VanillaPageTitle = tab.VanillaPage.Label;
            DefaultTitle = tab.VanillaPage.Label;
            Tab = (GameMenuTabs)Enum.Parse(typeof(GameMenuTabs), tab.Name, true);
            IsNew = true;
        }

        public GameMenuTabConfig(GameMenuExtenderConfig.CustomTabConfig tabConfig)
        {
            Name = tabConfig.Name;
            _Visible = tabConfig.Visible;
            _Title = tabConfig.Title;
            _DefaultPage = tabConfig.DefaultPage;
            _Index = tabConfig.Index;
            Tab = GameMenuTabs.Custom;
            IsNew = false;
        }

        public GameMenuTabConfig(CustomTab tab)
        {
            Name = tab.Name;
            _Title = tab.Label;
            _Visible = true;
            _DefaultPage = tab.TabPages.FirstOrDefault()?.Name;
            DefaultTitle = tab.Label;
            Tab = GameMenuTabs.Custom;
            IsNew = true;
        }

        public GameMenuExtenderConfig.TabConfig GetConfigObject()
        {
            if (IsVanilla)
            {
                return new GameMenuExtenderConfig.VanillaTabConfig()
                {
                    DefaultPage = DefaultPage,
                    Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null: Title,
                    VanillaPageVisible = VanillaPageVisible
                };
            }

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
