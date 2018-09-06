using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public abstract class GameMenuElement
    {
        internal GameMenuManager Manager { get; set; }

        public int ID { get; internal set; }

        public virtual bool Visible { get; set; }

        public virtual bool Enabled { get; set; }

        public string Name { get; protected set; }

        public string Label { get; set; }

        public abstract MenuType Type { get; }

        public abstract bool IsCustom { get; }

        public bool IsVanilla => !IsCustom;

        public bool IsSelected => Type == MenuType.Tab ? Manager.CurrentTab == this : Manager.CurrentTabPage == this;

        public IManifest OwnerMod { get; internal set; }
    }
}
