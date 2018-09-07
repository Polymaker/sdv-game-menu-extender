using GameMenuExtender.API;
using GameMenuExtender.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
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

        public bool HasInitialized { get; private set; }

        internal GameMenu Menu { get; private set; }

		public IEnumerable<GameMenuTab> AllTabs => VanillaTabs.Select(t => (GameMenuTab)t).Concat(CustomTabs);

		//public List<GameMenuTabPage> MenuTabPages { get; private set; }

		public List<CustomTab> CustomTabs { get; private set; }

		public List<VanillaTab> VanillaTabs { get; private set; }

		private IEnumerable<CustomTabPage> CustomTabPages => AllTabs.SelectMany(t => t.TabPages).OfType<CustomTabPage>();

		private CustomTab CurrentTabOverride;
        private VanillaTab CustomTabHost;

		public GameMenuTab CurrentTab => (GameMenuTab)CurrentTabOverride ?? RealCurrentTab;

		public GameMenuTabPage CurrentTabPage => CurrentTab?.CurrentTabPage;

		public VanillaTab RealCurrentTab => VanillaTabs[Menu.currentTab];

        public bool IsMenuOpen => StardewValley.Game1.activeClickableMenu is GameMenu;

        internal CreateMenuPageParams GameWindowBounds;

        internal GameMenuManager(IMod mod)
        {
            Helper = mod.Helper;
            Monitor = mod.Monitor;
			VanillaTabs = new List<VanillaTab>();
			CustomTabs = new List<CustomTab>();
            MenuEvents.MenuClosed += MenuEvents_MenuClosed;
        }

        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            if (e.PriorMenu is GameMenu)
                StopWatchingForVanillaTabChange();
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
					var menuTab = new VanillaTab(this, i, vanillaTabs[i]);
                    var tabPage = new VanillaTabPage(menuTab, vanillaPages[i]);
					VanillaTabs.Add(menuTab);
				}
            }

            HasInitialized = true;
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
            GameWindowBounds = new CreateMenuPageParams { x = Menu.xPositionOnScreen, y = Menu.yPositionOnScreen, width = Menu.width, height = Menu.height };

            var currentTabs = Helper.Reflection.GetField<List<ClickableComponent>>(Menu, "tabs").GetValue();
            var currentPages = Helper.Reflection.GetField<List<IClickableMenu>>(Menu, "pages").GetValue();
            
            for (int i = 0; i < VanillaTabs.Count; i++)
            {
				var currentTab = VanillaTabs.ElementAt(i);
                currentTab.TabButton = currentTabs[i];

                var currentPage = currentPages[i];
                var vanillaPageMenuType = GetDefaultTabPageType(currentTab.TabName);

                if (currentPage.GetType() == vanillaPageMenuType)
                {
					currentTab.VanillaPage.PageWindow = currentPage;
                }
                else
                {
					currentTab.VanillaPage.InstanciatePageWindow();
                    var customPageMod = Helper.ModRegistry.GetModByType(currentPage.GetType());
					
					if (customPageMod != null)
                    {
                        var customTabPage = CustomTabPages.FirstOrDefault(p => p.Tab == currentTab && p.SourceMod == customPageMod && p.IsNonAPI);

						if (customTabPage == null)
                        {
							Monitor.Log($"The tab page '{currentTab.Name}' is overrided by another mod ({customPageMod.Name})");
                            customTabPage = RegisterTabPageExtension(customPageMod, currentTab.Name, currentTab.Name, currentTab.Label, currentPage.GetType(), false);
                        }

                        if (customTabPage != null)
                            customTabPage.PageWindow = currentPage;
					}
                }

                currentPages[i] = currentTab.PageExtender;
            }

            foreach (var customPage in CustomTabPages)
            {
                if (customPage.PageWindow == null)
                    customPage.InstanciatePageWindow();
            }

            UpdateCustomTabs();

            foreach (var tab in AllTabs)
				tab.SelectFirstPage();

			foreach(var realTab in VanillaTabs)
				realTab.InitializeLayout();
		}

        public void ChangeTab(GameMenuTab newTab)
        {
            if (newTab == CurrentTab)
                return;

            if (newTab.IsVanilla)
			{
				CurrentTabOverride = null;
				Menu.changeTab((newTab as VanillaTab).TabIndex);
			}
            else
			{
                RealCurrentTab.TabButton.bounds.Y -= 8;
                CurrentTabOverride = (CustomTab)newTab;
                CustomTabHost = RealCurrentTab;
                StartWatchingForVanillaTabChange();
            }

			RealCurrentTab.InitializeLayout();
        }

        private bool WatchTimerAttached;

        private void StartWatchingForVanillaTabChange()
        {
            if (!WatchTimerAttached)
            {
                GameEvents.UpdateTick += WatchForVanillaTabChange;
                WatchTimerAttached = true;
            }
        }

        private void StopWatchingForVanillaTabChange()
        {
            if (WatchTimerAttached)
            {
                GameEvents.UpdateTick -= WatchForVanillaTabChange;
                WatchTimerAttached = false;
            }
        }

        private void WatchForVanillaTabChange(object sender, EventArgs e)
        {
            if (CustomTabHost == null)
            {
                StopWatchingForVanillaTabChange();
                return;
            }

            if (Menu.currentTab != CustomTabHost.TabIndex)
            {
                ReturnToVanillaTab();
            }
        }

        internal void ReturnToVanillaTab()
        {
            CurrentTabOverride = null;
            CustomTabHost.TabButton.bounds.Y += 8;
            CustomTabHost = null;
            RealCurrentTab.InitializeLayout();
            StopWatchingForVanillaTabChange();
        }

        private void UpdateCustomTabs()
        {
            var lastTabButton = VanillaTabs.Last().TabButton;
            int currentX = lastTabButton.bounds.Right;
            int currentY = lastTabButton.bounds.Top; ;

            foreach (var tab in CustomTabs)
            {
                if (tab.Visible)
                {
                    tab.DrawText = (tab.TabIcon == null);
                    var buttonBounds = new Rectangle(currentX, currentY, 64, 64);
                    if (tab.DrawText)
                    {
                        var labelSize = Game1.smallFont.MeasureString(tab.Label);
                        buttonBounds.Width += (int)labelSize.X - 32;
                    }
                    tab.TabButton = new ClickableComponent(buttonBounds, tab.Name, tab.Label);
                    currentX += buttonBounds.Width;
                }
                else
                    tab.TabButton = null;
            }
        }

        internal void DrawCustomTabs(SpriteBatch b)
        {
            //b.End();
            //b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

            foreach (var tab in CustomTabs)
            {
                if (tab.Visible && tab.TabButton != null)
                {
                    var tabBounds = tab.TabButton.bounds;

                    if (tab.DrawText)
                    {
                        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(16, 368, 16, 16),
                            tabBounds.X, tabBounds.Y + (tab.IsSelected ? 8 : 0), tabBounds.Width, tabBounds.Height, Color.White, 4f, false);

                        Utility.drawTextWithShadow(b, tab.Label, Game1.smallFont, new Vector2(tabBounds.X + 16, tabBounds.Y + 24 + (tab.IsSelected ? 8 : 0)), Game1.textColor);
                    }
                    else
                    {
                        b.Draw(Game1.mouseCursors, new Vector2(tabBounds.X, tabBounds.Y + (tab.IsSelected ? 8 : 0)),
                            new Rectangle?(new Rectangle(16, 368, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
                    }
                }
            }

            //b.End();
            //b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
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

        #region API

        public CustomTab RegisterCustomTabPage(IManifest source, string tabName, string label, Type pageMenuClass)
        {
            Monitor.Log($"Registering a custom tab page '{tabName}' for mod {source.Name}");

            string uniqueName = $"{source.Name}.{tabName}";

            if (AllTabs.Any(t => t.NameEquals(uniqueName)))
            {
                Monitor.Log($"Could not register a custom tab for the mod '{source.Name}'. There is already a custom tab named '{uniqueName}'.", LogLevel.Warn);
                return null;
            }

            var newTab = new CustomTab(this, source, uniqueName, label);
            CustomTabs.Add(newTab);

            var newTabPage = RegisterTabPageExtension(source, uniqueName, tabName, label, pageMenuClass);

            if (newTabPage == null)
            {
                CustomTabs.Remove(newTab);
                Monitor.Log($"Error: A tab page could not be created for custom tab {uniqueName}", LogLevel.Warn);
                return null;
            }

            return newTab;
        }

        public CustomTabPage RegisterTabPageExtension(IManifest source, string tabName, string pageName, string pageLabel, Type pageMenuClass, bool byAPI = true)
        {
            Monitor.Log($"Registering a page extension '{pageName}' on tab '{tabName}' for mod {source.Name}");

            var foundTab = AllTabs.FirstOrDefault(t => t.NameEquals(tabName));
            if (foundTab == null)
            {
                Monitor.Log($"Could not register a page extension for the mod '{source.Name}'. Could not find a tab named '{tabName}'.", LogLevel.Warn);
                return null;
            }

            string uniqueName = $"{source.Name}.{pageName}";
            if (CustomTabPages.Any(t => t.NameEquals(uniqueName)))
            {
                Monitor.Log($"Could not register a page extension for the mod '{source.Name}'. There is alreay a page extension named '{uniqueName}'.", LogLevel.Warn);
                return null;
            }
            
            return new CustomTabPage(foundTab, source, uniqueName, pageLabel, pageMenuClass);
        }

        #endregion
    }
}
