using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public class CustomTabPage : GameMenuTabPage
    {
        public override bool IsCustom => true;

		public bool IsNonAPI { get; internal set; }

		public CustomTabPage(GameMenuTab tab, IManifest mod, string name, string label, Type pageClass) : base(tab, name)
		{
			PageType = pageClass;
			SourceMod = mod;
            Label = label;
		}
    }
}
