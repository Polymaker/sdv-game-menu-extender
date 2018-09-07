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

		public int DisplayIndex { get; set; }

        public bool DrawText { get; set; }

        public Icon TabIcon { get; set; }

        internal CustomTab(GameMenuManager manager, IManifest mod, string name, string label) : base(manager, name)
        {
            Label = label;
        }
    }
}
