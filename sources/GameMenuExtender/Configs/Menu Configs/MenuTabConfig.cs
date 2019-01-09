using GameMenuExtender.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public abstract class MenuTabConfig : GameMenuElementConfig
    {
        protected string _Title;
        protected string _DefaultPage;

        public abstract GameMenuTabs Tab { get; }

        public string Name { get; private set; }

        public string Title { get => string.IsNullOrEmpty(_Title) ? DefaultTitle : _Title; set => SetPropertyValue(ref _Title, value); }

        [Obsolete("The TabPage with index 0 is the default page.")]
        public string DefaultPage { get => _DefaultPage; set => SetPropertyValue(ref _DefaultPage, value); }

        public string DefaultTitle { get; set; }

        public abstract bool Visible { get; set; }

        public abstract int Index { get; set; }

        public List<MenuTabPageConfig> TabPages { get; private set; }

        protected MenuTabConfig(string name)
            : base(name)
        {
            Name = name;
            TabPages = new List<MenuTabPageConfig>();
        }

        protected MenuTabConfig(Serialization.TabCfgBase config) 
            : this(config.Name)
        {
            Name = config.Name;
            _Title = config.Title;
            IsNew = false;
        }

        protected MenuTabConfig(GameMenuTab tab)
            : this(tab.Name)
        {
            Name = tab.Name;
            _Title = tab.Label;
            DefaultTitle = tab.Label;
            IsNew = true;
        }

        public abstract Serialization.TabCfgBase GetJsonObject();

    }
}
