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
	public class GameMenuExtenderApi : IGameMenuExtenderApi
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
            MenuManager.Initialize();
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
		{
            if (e.NewMenu is GameMenu && MenuManager != null)
            {
                MenuManager.Menu = (GameMenu)e.NewMenu;
                GameEvents.FourthUpdateTick += OnGameMenuShown;
            }
		}

		private void OnGameMenuShown(object sender, EventArgs e)
		{
			GameEvents.FourthUpdateTick -= OnGameMenuShown;
            MenuManager.ExtendGameMenu();

            //InitializeMenuExtenders();
		}

		private void InitializeMenuExtenders()
		{
			if (!(Game1.activeClickableMenu is GameMenu))
				return;

			var gameMenu = (GameMenu)Game1.activeClickableMenu;
			var tabList = Helper.Reflection.GetField<List<ClickableComponent>>(gameMenu, "tabs").GetValue();
			var pageList = Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();

			CustomMenuEntries.RemoveAll(m => m.OwnerMod == null && m.IsNonApiOverride);
			CustomMenuEntries.ForEach(m => 
			{
				if (m.IsNonApiOverride && m.OwnerMod != null)
					m.Visible = false;
			});

			for (int i = 0; i < tabList.Count; i++)
			{
				if (Enum.TryParse(tabList[i].name, true, out GameMenuTabs menuTab))
				{
					//if (menuTab == GameMenuTabs.Map)
					//	continue;

					var currentPage = pageList[i];
					var pageExtender = new GameMenuPageExtender(null);
					pageExtender.Initialize(currentPage);
					
					var defaultPageType = GetDefaultTabPageType(menuTab);

					if (currentPage.GetType() != defaultPageType)
					{
						var pageOwnerMod = Helper.ModRegistry.GetModByType(currentPage.GetType());
						
						Monitor.Log($"The tab page '{menuTab}' has already been overrided by another mod ({pageOwnerMod?.Name ?? "unknown"})");

						bool customEntryExist = false;

						if (pageOwnerMod != null)
						{
							var existingPage = CustomMenuEntries.FirstOrDefault(p => 
								p.PageClass == currentPage.GetType() && p.OwnerMod == pageOwnerMod && p.IsNonApiOverride);

							if(existingPage != null)
							{
								existingPage.Visible = true;
								customEntryExist = true;
							}
						}

						if(!customEntryExist)
						{
							var newPage = RegisterGameMenuSubPage(tabList[i].name, pageOwnerMod?.Name ?? "Custom", currentPage.GetType(), pageOwnerMod);
							newPage.IsNonApiOverride = true;
							newPage.MenuInstance = currentPage;
						}

						pageExtender.OriginalPage = pageExtender.InstanciateCustomPage(defaultPageType);
					}
					else
						pageExtender.OriginalPage = currentPage;


					pageExtender.InitializePageExtensions(CustomMenuEntries.Where(m => m.MenuTab == menuTab && m.Type == MenuType.TabPage));

					pageList[i] = pageExtender;
				}
			}
		}

		private static Type GetDefaultTabPageType(GameMenuTabs tab)
		{
			switch (tab)
			{
				case GameMenuTabs.Inventory:
					return typeof(InventoryPage);
				case GameMenuTabs.Skills:
					return typeof(SkillsPage);
				case GameMenuTabs.Social:
					return typeof(SocialPage);
				case GameMenuTabs.Map:
					return typeof(MapPage);
				case GameMenuTabs.Crafting:
					return typeof(CraftingPage);
				case GameMenuTabs.Collections:
					return typeof(CollectionsPage);
				case GameMenuTabs.Options:
					return typeof(OptionsPage);
				case GameMenuTabs.Exit:
					return typeof(ExitPage);
			}
			return null;
		}

        public static string GetDefaultTabName(GameMenuTabs tab)
        {
            switch (tab)
            {
                case GameMenuTabs.Social:
                    return "Relationships";
                default:
                    return tab.ToString();
            }
        }

        public void RegisterGameMenuExtension(string targetTab, Type customPageType, string label)
		{
			var modInfo = Helper.ModRegistry.GetCallingMod();

			Monitor.Log($"Registering custom page extension for GameMenu '{targetTab}' tab");
			if (Enum.TryParse(targetTab, true, out GameMenuTabs menuTab))
			{
				CustomMenuEntries.Add(new MenuPageEntry()
				{
					ID = ++CurrentEntryID,
					Type = MenuType.TabPage,
					TabName = targetTab,
					MenuTab = menuTab,
					PageClass = customPageType,
					Visible = true,
					Enabled = true,
					Label = label
				});
			}
			else if (CustomMenuEntries.Any(m => m.Type == MenuType.Tab && m.Label == targetTab))
			{
				CustomMenuEntries.Add(new MenuPageEntry()
				{
					ID = ++CurrentEntryID,
					Type = MenuType.TabPage,
					TabName = targetTab,
					MenuTab = GameMenuTabs.Custom,
					PageClass = customPageType,
					Visible = true,
					Enabled = true,
					Label = label
				});
			}
			else
			{
				Monitor.Log($"Invalid tab name");
			}
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

        public void RegisterGameMenuTab(string tabName, Type customTabPageType)
        {
            RegisterGameMenuTab(tabName, customTabPageType, Helper.ModRegistry.GetCallingMod());
        }

        private MenuPageEntry RegisterGameMenuTab(string tabName, Type customTabPageType, IManifest ownerMod)
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
				Label = tabName,
				OwnerMod = ownerMod
			};

			CustomMenuEntries.Add(newMenuTab);
			return newMenuTab;
		}

	}
}
