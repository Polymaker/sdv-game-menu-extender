using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Configs
{
    public abstract class MenuTabPageConfig : GameMenuElementConfig
    {
        public string Name { get; private set; }

        public string TabName { get; protected set; }

        public abstract string Title { get; set; }

        //public virtual string DefaultTitle { get; set; }

        public string DefaultTitle => ConfigManager.DefaultPageTitles.ContainsKey(Name) ? ConfigManager.DefaultPageTitles[Name] : string.Empty;

        public abstract bool Visible { get; set; }

        public abstract int Index { get; set; }

        protected MenuTabPageConfig(string name, string tabName)
            : base(name)
        {
            Name = name;
            TabName = tabName;
        }
    }
}
