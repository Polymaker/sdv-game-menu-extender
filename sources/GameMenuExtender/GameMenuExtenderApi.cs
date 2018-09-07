using GameMenuExtender.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender
{
	public class GameMenuExtenderApi
	{
        private IMod Mod;
        public IModHelper Helper => Mod.Helper;
        public IMonitor Monitor => Mod.Monitor;
        private GameMenuManager MenuManager;
        private List<MenuPageEntry> CustomMenuEntries;
		private static int CurrentEntryID;

		public GameMenuExtenderApi(IMod mod)
		{
            Mod = mod;
            MenuManager = new GameMenuManager(Mod);
            CustomMenuEntries = new List<MenuPageEntry>();
			MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            MenuManager.InitializeVanillaMenus();
			MenuManager.InitializeCustomMenus(CustomMenuEntries);
		}

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
		{
            if (e.NewMenu is GameMenu && MenuManager != null)
            {
				MenuManager.SetCurrentMenu((GameMenu)e.NewMenu);
                GameEvents.FourthUpdateTick += OnGameMenuShown;
            }
		}

		private void OnGameMenuShown(object sender, EventArgs e)
		{
			GameEvents.FourthUpdateTick -= OnGameMenuShown;
            MenuManager.ExtendGameMenu();
		}

        public void RegisterGameMenuSubPage(string tabName, string pageName, Type customPageType)
        {
            RegisterGameMenuSubPage(tabName, pageName, customPageType, Helper.ModRegistry.GetCallingMod());
        }

        private MenuPageEntry RegisterGameMenuSubPage(string tabName, string pageName, Type customPageType, IManifest ownerMod)
		{
			Monitor.Log($"Registering custom page ({pageName}:{customPageType.Name}) extension for GameMenu '{tabName}' tab");

			GameMenuTabs targetTab;
			if (!Enum.TryParse(tabName, true, out targetTab))
				targetTab = GameMenuTabs.Custom;

			if(targetTab == GameMenuTabs.Custom && 
				!CustomMenuEntries.Any(m=>m.Type == MenuType.Tab 
				&& m.TabName.Equals(tabName, StringComparison.InvariantCultureIgnoreCase)))
			{
				Monitor.Log($"Invalid tab name");
				return null;
			}

			var newMenuPage = new MenuPageEntry()
			{
				ID = ++CurrentEntryID,
				Type = MenuType.TabPage,
				TabName = tabName,
				MenuTab = targetTab,
				PageClass = customPageType,
				Visible = true,
				Enabled = true,
				Label = pageName,
				OwnerMod = ownerMod
			};

			CustomMenuEntries.Add(newMenuPage);
			return newMenuPage;
		}

        public void RegisterGameMenuTab(string tabName, Type customTabPageType, string tabLabel)
        {
            RegisterGameMenuTab(tabName, customTabPageType, tabLabel, Helper.ModRegistry.GetCallingMod());
        }

        private MenuPageEntry RegisterGameMenuTab(string tabName, Type customTabPageType, string tabLabel, IManifest ownerMod)
		{
			Monitor.Log($"Registering custom GameMenu tab ({tabName}:{customTabPageType.Name})");

			GameMenuTabs targetTab;
			if (!Enum.TryParse(tabName, true, out targetTab))
				targetTab = GameMenuTabs.Custom;

			if (CustomMenuEntries.Any(m => m.Type == MenuType.Tab
				&& m.TabName.Equals(tabName, StringComparison.InvariantCultureIgnoreCase)))
			{
				Monitor.Log($"Invalid tab name");
				return null;
			}

			var newMenuTab = new MenuPageEntry()
			{
				ID = ++CurrentEntryID,
				Type = MenuType.Tab,
				TabName = tabName,
				MenuTab =  GameMenuTabs.Custom,
				PageClass = customTabPageType,
				Visible = true,
				Enabled = true,
				Label = tabLabel,
				OwnerMod = ownerMod
			};

			CustomMenuEntries.Add(newMenuTab);
			return newMenuTab;
		}
	}
}
