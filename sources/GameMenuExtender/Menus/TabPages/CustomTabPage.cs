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


		public CustomTabPage(GameMenuTab tab, IManifest mod, Type pageClass, string name) : base(tab)
		{
			PageType = pageClass;
			OwnerMod = mod;
			Name = name;
		}
	}
}
