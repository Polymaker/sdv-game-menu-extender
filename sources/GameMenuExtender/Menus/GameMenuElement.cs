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

		public string UniqueID { get; set; }

        public virtual bool Visible { get; set; }

        public virtual bool Enabled { get; set; }

        public string Name { get; protected set; }

        public string Label { get; set; }

        public abstract MenuType Type { get; }

        public abstract bool IsCustom { get; }

        public bool IsVanilla => !IsCustom;

        public bool IsSelected => Type == MenuType.Tab ? Manager.CurrentTab == this : Manager.CurrentTabPage == this;

        public IManifest OwnerMod { get; internal set; }

		internal static int NextID;

		public GameMenuElement()
		{
			ID = ++NextID;
		}

		public static string GenerateUniqueID(GameMenuElement element)
		{
			string ownerName = element.OwnerMod?.Name ?? "StardewValley";

			switch (element.Type)
			{
				case MenuType.Tab:
					return $"Tab_{element.Name}::{ownerName}";
				case MenuType.TabPage:
					return $"Tab_{(element as GameMenuTabPage).Tab.Name}:Page_{element.Name}::{ownerName}";
			}
			return string.Empty;
		}

		public static string GenerateUniqueID(string ownerName, string tabName, string pageName)
		{
			if(!string.IsNullOrWhiteSpace(pageName))
				return $"Tab_{tabName}:Page_{pageName}::{ownerName}";
			else
				return $"Tab_{tabName}::{ownerName}";
		}


		internal void AssignUniqueID()
		{
			UniqueID = GenerateUniqueID(this);
		}
	}
}
