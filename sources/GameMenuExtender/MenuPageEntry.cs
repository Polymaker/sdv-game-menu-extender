using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender
{
	public class MenuPageEntry
	{
		public int ID { get; set; }

		public string UniqueID { get; set; }

		public MenuType Type { get; set; }

		public string TabName { get; set; }

		public GameMenuTabs MenuTab { get; set; }

		public Type PageClass { get; set; }

		public string Label { get; set; }

		public bool Visible { get; set; }

		public bool Enabled { get; set; }

		internal ClickableComponent TabButton { get; set; }

		public IManifest OwnerMod { get; internal set; }

		public bool IsNonApiOverride { get; internal set; }

		internal IClickableMenu MenuInstance { get; set; }

		public MenuPageEntry(string tabName, Type pageType, IManifest owner)
		{
			Type = MenuType.CustomTab;
			MenuTab = GameMenuTabs.Custom;
			OwnerMod = owner;
			UniqueID = $"{OwnerMod.UniqueID}.{tabName}";
			Visible = true;
			Enabled = true;
		}

		public MenuPageEntry()
		{

		}
	}
}
