using GameMenuExtender.Configs;
using GameMenuExtender.Data;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public class CustomTab : GameMenuTab
    {
        public override bool IsCustom => true;

        public int DisplayIndex { get => Configuration?.Index ?? 0; set => Configuration.Index = value; }

        public bool DrawText { get; set; }

        public Icon TabIcon { get; set; }

        public override GameMenuTabs TabName => GameMenuTabs.Custom;

        public new CustomTabConfig Configuration => base.Configuration as CustomTabConfig;

        internal CustomTab(GameMenuManager manager, IManifest mod, string name, string label) : base(manager, name)
        {
            Label = label;
        }
    }
}
