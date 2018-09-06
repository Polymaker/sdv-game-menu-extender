using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    internal class GameMenuManager
    {
        public IModHelper Helper { get; private set; }
        public IMonitor Monitor { get; private set; }

        private bool HasInitialized;

        internal GameMenu Menu { get; private set; }

		public IEnumerable<GameMenuTab> AllTabs => VanillaTabs.Select(t => (GameMenuTab)t).Concat(CustomTabs);

		//public List<GameMenuTabPage> MenuTabPages { get; private set; }

		public List<CustomTab> CustomTabs { get; private set; }

		public List<VanillaTab> VanillaTabs { get; private set; }

		private IEnumerable<CustomTabPage> CustomTabPages => AllTabs.SelectMany(t => t.TabPages).OfType<CustomTabPage>();

		private CustomTab CurrentTabOverride;

		public GameMenuTab CurrentTab => (GameMenuTab)CurrentTabOverride ?? RealCurrentTab;

		public GameMenuTabPage CurrentTabPage => CurrentTab?.CurrentTabPage;

		public VanillaTab RealCurrentTab => VanillaTabs[Menu.currentTab];

		//public VanillaTabPage CurrentVanillaTabPage => CurrentVanillaTab?.VanillaPage;

		internal GameMenuManager(IMod mod)
        {
            Helper = mod.Helper;
            Monitor = mod.Monitor;
            //AllTabs = new List<GameMenuTab>();
			VanillaTabs = new List<VanillaTab>();
			CustomTabs = new List<CustomTab>();

		}

		/// <summary>
		/// Populate the vanilla Tabs and Pages
		/// </summary>
        internal void InitializeVanillaMenus()
        {
			VanillaTabs.Clear();
			var gameMenu = new GameMenu();
            var vanillaTabs = Helper.Reflection.GetField<List<ClickableComponent>>(gameMenu, "tabs").GetValue();
            var vanillaPages = Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();

            for (int i = 0; i < vanillaTabs.Count; i++)
            {
                if (Enum.TryParse(vanillaTabs[i].name, true, out GameMenuTabs tabName))
                {
					var menuTab = new VanillaTab(i, vanillaTabs[i]) { Manager = this };
                    var tabPage = new VanillaTabPage(menuTab, vanillaPages[i]) { Manager = this };
					VanillaTabs.Add(menuTab);
					//MenuTabPages.Add(tabPage);
					menuTab.AssignUniqueID();
					tabPage.AssignUniqueID();
					menuTab.InitializeLayout();
				}
            }

            HasInitialized = true;
        }

		public void InitializeCustomMenus(IEnumerable<MenuPageEntry> customMenus)
		{
			CustomTabs.Clear();
			VanillaTabs.ForEach(v => v.RemoveAllCustomPages());
			
			foreach(var customTabDef in customMenus.Where(m=>m.Type == MenuType.Tab))
			{
				var customTab = new CustomTab(customTabDef.TabName)
				{
					Label = customTabDef.Label,
					OwnerMod = customTabDef.OwnerMod,
					Manager = this
				};
				customTab.AssignUniqueID();
				CustomTabs.Add(customTab);
			}

			foreach (var customPageDef in customMenus.Where(m => m.Type == MenuType.TabPage))
			{
				var parentTab = AllTabs.FirstOrDefault(t => t.Name == customPageDef.TabName);
				if(parentTab != null)
				{
					var customPage = new CustomTabPage(parentTab, customPageDef.OwnerMod, customPageDef.PageClass, customPageDef.Label) { Manager = this };
					customPage.AssignUniqueID();
				}
			}
		}

		internal void SetCurrentMenu(GameMenu menu)
		{
			Menu = menu;
		}

        internal void ExtendGameMenu()
        {
			if (!HasInitialized)
				InitializeVanillaMenus();

			CurrentTabOverride = null;

            var currentTabs = Helper.Reflection.GetField<List<ClickableComponent>>(Menu, "tabs").GetValue();
            var currentPages = Helper.Reflection.GetField<List<IClickableMenu>>(Menu, "pages").GetValue();
            
            for (int i = 0; i < VanillaTabs.Count; i++)
            {
				var currentTab = VanillaTabs.ElementAt(i);

				var currentPage = currentPages[i];
                var vanillaPageMenuType = GetDefaultTabPageType(currentTab.TabName);

                if (currentPage.GetType() == vanillaPageMenuType)
                {
					currentTab.VanillaPage.PageWindow = currentPage;
                }
                else
                {
					currentTab.VanillaPage.InstanciateNewPage();
                    var customPageMod = Helper.ModRegistry.GetModByType(currentPage.GetType());
					
					if (customPageMod != null)
                    {
                        var customTabPage = CustomTabPages.FirstOrDefault(p => p.Tab == currentTab && p.OwnerMod == customPageMod && p.IsNonAPI);

						if (customTabPage == null)
                        {
							Monitor.Log($"The tab page '{currentTab.Name}' is overrided by another mod ({customPageMod?.Name ?? "unknown"})");

							customTabPage = new CustomTabPage(currentTab, customPageMod, currentPage.GetType(), currentTab.Name)
							{
								IsNonAPI = true,
								Manager = this
							};
							customTabPage.AssignUniqueID();
						}
						customTabPage.PageWindow = currentPage;
					}
                }

                currentPages[i] = currentTab.PageExtender;
            }

			foreach(var tab in AllTabs)
				tab.SelectFirstPage();

			foreach(var realTab in VanillaTabs)
				realTab.InitializeLayout();
		}

        public void ChangeTab(GameMenuTab newTab)
        {
            if (CurrentTab.IsCustom && !newTab.IsCustom)
                RealCurrentTab.TabButton.bounds.Y += 8;

            if (newTab.IsVanilla)
			{
				CurrentTabOverride = null;
				Menu.changeTab((newTab as VanillaTab).TabIndex);
			}
            else
			{
				RealCurrentTab.TabButton.bounds.Y -= 8;
				CurrentTabOverride = (CustomTab)newTab;
			}

			RealCurrentTab.InitializeLayout();
		}

        internal void DrawCustomTabs(SpriteBatch b)
        {

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
	}
}
