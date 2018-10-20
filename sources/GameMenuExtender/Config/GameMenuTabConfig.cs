using GameMenuExtender.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Config
{
    public abstract class GameMenuTabConfig : ConfigBase, IMenuTabConfig
    {
        protected string _Title;
        protected string _DefaultPage;

        public abstract GameMenuTabs Tab { get; }

        public string Name { get; private set; }

        public string Title { get => string.IsNullOrEmpty(_Title) ? DefaultTitle : _Title; set => SetPropertyValue(ref _Title, value); }

        public string DefaultPage { get => _DefaultPage; set => SetPropertyValue(ref _DefaultPage, value); }

        public abstract bool IsVanilla { get; }

        public string DefaultTitle { get; set; }

        public abstract bool Visible { get; set; }
        public abstract int Index { get; set; }
        public abstract string PageTitle { get; set; }
        public abstract string DefaultPageTitle { get; set; }
        public abstract bool PageVisible { get; set; }
        public abstract int PageIndex { get; set; }

        protected GameMenuTabConfig(string name)
        {
            Name = name;
        }

        protected GameMenuTabConfig(GameMenuExtenderConfig.TabConfig config)
        {
            var vtc = config as GameMenuExtenderConfig.VanillaTabConfig;
            var ctc = config as GameMenuExtenderConfig.CustomTabConfig;
            Name = vtc?.Name.ToString() ?? ctc.Name;
            _Title = config.Title;
            _DefaultPage = config.DefaultPage;
            IsNew = false;
        }

        protected GameMenuTabConfig(GameMenuTab tab)
        {
            Name = tab.Name;
            _Title = tab.Label;
            _DefaultPage = tab.TabPages.FirstOrDefault()?.Name;
            DefaultTitle = tab.Label;
            IsNew = true;
        }

        public abstract GameMenuExtenderConfig.TabConfig GetConfigObject();
        //{
        //    if (IsVanilla)
        //    {
        //        return new GameMenuExtenderConfig.VanillaTabConfig()
        //        {
        //            DefaultPage = DefaultPage,
        //            Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null: Title,
        //            VanillaPageVisible = VanillaPageVisible,
        //            VanillaPageTitle = (!string.IsNullOrEmpty(DefaultPageTitle) && DefaultPageTitle == VanillaPageTitle) ? null : VanillaPageTitle
        //        };
        //    }

        //    return new GameMenuExtenderConfig.CustomTabConfig()
        //    {
        //        Index = Index,
        //        Name = Name,
        //        Title = (!string.IsNullOrEmpty(DefaultTitle) && DefaultTitle == Title) ? null : Title,
        //        Visible = Visible,
        //        DefaultPage = DefaultPage
        //    };
        //}
    }
}
