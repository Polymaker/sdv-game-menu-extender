using GameMenuExtender.API;
using GameMenuExtender.Compatibility;
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
		internal GameMenuExtenderMod Mod { get; private set; }

		public IModHelper Helper => Mod.Helper;

		public IMonitor Monitor => Mod.Monitor;

		public bool HasInitialized { get; private set; }

        internal GameMenu ActiveGameMenu { get; private set; }
        internal List<CompatibilityPatch> CompatibilityPatches { get; } = new List<CompatibilityPatch>();

        #region Tabs & Tab Pages Properties

        public IEnumerable<GameMenuTab> AllTabs => VanillaTabs.Select(t => (GameMenuTab)t).Concat(CustomTabs);

		public List<CustomTab> CustomTabs { get; private set; }

		public List<VanillaTab> VanillaTabs { get; private set; }

		public IEnumerable<GameMenuTabPage> AllTabPages => AllTabs.SelectMany(t => t.TabPages);

		public IEnumerable<CustomTabPage> CustomTabPages => AllTabs.SelectMany(t => t.TabPages).OfType<CustomTabPage>();

		public GameMenuTab CurrentTab => IsGameMenuExtended ? ((GameMenuTab)CurrentTabOverride ?? CurrentTabReal) : null;

		public GameMenuTabPage CurrentTabPage => CurrentTab?.CurrentTabPage;

		/// <summary>
		/// Gets the current vanilla tab that is displayed or that displays the current custom tab.
		/// </summary>
		public VanillaTab CurrentTabReal => IsGameMenuExtended ? VanillaTabs[ActiveGameMenu.currentTab] : null;

		#endregion

		public bool IsGameMenuOpen => StardewValley.Game1.activeClickableMenu is GameMenu;

		public bool IsGameMenuExtended { get; private set; }

        internal CreateMenuPageParams GameWindowBounds;
        internal Rectangle JuminoIconDefaultBounds;

        public event EventHandler CurrentTabPageChanged;
        public event EventHandler GameMenuOpened;
        public event EventHandler GameMenuClosed;

        internal GameMenuManager(GameMenuExtenderMod mod)
        {
			Mod = mod;
			VanillaTabs = new List<VanillaTab>();
			CustomTabs = new List<CustomTab>();
            GameMenuTabList = new List<ClickableComponent>();
            GameMenuPageList = new List<IClickableMenu>();

            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
			MenuEvents.MenuClosed += MenuEvents_MenuClosed;
        }

		#region Game Menu Events Management

		private bool WaitingForGameMenu;
		private int SelectedOptionFix = -1;

		private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
		{
            //Monitor.Log($"Menu Changing to {(e.NewMenu != null ? e.NewMenu.GetType().Name : "null")}");

            if (e.NewMenu is GameMenu && !WaitingForGameMenu)
			{
				var currentMenu = (GameMenu)e.NewMenu;

				var optionPage = currentMenu.GetPage<OptionsPage>();
				if (optionPage != null && optionPage.currentItemIndex > 0)
					SelectedOptionFix = optionPage.currentItemIndex;
				else
					SelectedOptionFix = -1;

				WaitingForGameMenu = true;
				GameEvents.SecondUpdateTick += MenuEvents_AfterMenuChanged;
			}
		}

		private void MenuEvents_AfterMenuChanged(object sender, EventArgs e)
		{
			WaitingForGameMenu = false;
			GameEvents.SecondUpdateTick -= MenuEvents_AfterMenuChanged;
			GameEvents.UpdateTick += GameEvents_UpdateTick;

			if(Game1.activeClickableMenu is GameMenu)
			{
				if (ActiveGameMenu != null)
				{
					if (ActiveGameMenu == Game1.activeClickableMenu)
						return;
					OnGameMenuClosed();
				}
				ActiveGameMenu = (GameMenu)Game1.activeClickableMenu;
				OnGameMenuOpened();
			}
		}

		private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
		{
			if (e.PriorMenu is GameMenu)
			{
				GameEvents.UpdateTick -= GameEvents_UpdateTick;
				OnGameMenuClosed();
            }
		}

		private void GameEvents_UpdateTick(object sender, EventArgs e)
		{
			if (!IsGameMenuOpen)
			{
				GameEvents.UpdateTick -= GameEvents_UpdateTick;
				return;
			}
			if (IsGameMenuExtended)
				OnGameMenuUpdate();
		}

		#endregion

		public void Initialize()
		{
			if (!HasInitialized)
			{
				InitializeVanillaMenus();
				Mod.ApiInstance.PerformRegistration();
				HasInitialized = true;
            }
		}

        public void InitializeCompatibilityFixes()
        {
            var patchClasses = typeof(GameMenuManager).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CompatibilityPatch)) && t != typeof(CompatibilityPatch)).ToList();
            foreach(var patchClass in patchClasses)
            {
                if(!CompatibilityPatches.Any(p=>p.GetType() == patchClass))
                {
                    var patch = (CompatibilityPatch)Activator.CreateInstance(patchClass, this);
                    if (patch.IsAppliable() && patch.InitializePatch())
                    {
                        patch.SuscribeToManagerEvents();
                        CompatibilityPatches.Add(patch);
                    }
                }
            }
        }

        #region Game Menu Tracking

        private List<ClickableComponent> GameMenuTabList;
        private List<IClickableMenu> GameMenuPageList;

        private void OnGameMenuOpened()
		{
			if (!HasInitialized)
				Initialize();
			ExtendGameMenu();
            GameMenuOpened?.Invoke(null, EventArgs.Empty);
            //Monitor.Log("Game menu opened");
		}

		private void OnGameMenuClosed()
		{
			CurrentTabOverride = null;
			CustomTabHost = null;
			CurrentTabIndex = 0;
            ActiveGameMenu = null;
            IsGameMenuExtended = false;
            GameMenuTabList.Clear();
            GameMenuPageList.Clear();
            GameMenuClosed?.Invoke(null, EventArgs.Empty);
            //Monitor.Log("Game menu closed");
        }

		private void OnGameMenuUpdate()
		{
			if (IsGameMenuExtended)
			{
                if(ActiveGameMenu.currentTab != CurrentTabIndex && !ChangingTab)
                {
                    if (CurrentTabOverride != null)
                        ReturnToVanillaTab();
                    CurrentTabIndex = ActiveGameMenu.currentTab;
                    CurrentTabReal.SelectDefaultPage();
                    CurrentTabReal.InitializeLayout();
                    OnCurrentTabPageChanged();
                }
				else if(!(GameMenuPageList[ActiveGameMenu.currentTab] is GameMenuPageExtender))
                {
                    //Monitor.Log($"A mod has overrided a tab page late!");
                    RegisterNonApiTabPage(CurrentTabReal, GameMenuPageList[ActiveGameMenu.currentTab]);
                    GameMenuPageList[ActiveGameMenu.currentTab] = CurrentTabReal.PageExtender;
                }
            }
		}

		#endregion

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
					tabPage.CalculateGameMenuOffset(gameMenu);
					VanillaTabs.Add(menuTab);
                    menuTab.LoadConfig();
                    tabPage.LoadConfig();
                }
            }

            if (Mod.Configs.AllConfigs.Any(c => c.HasChanged))
                Mod.Configs.SaveConfigs();
        }

        /// <summary>
        /// Replaces the GameMenu Tabs with GameMenuPageExtender
        /// </summary>
		internal void ExtendGameMenu()
        {
            //Init & reset stuff
			CurrentTabOverride = null;
			CustomTabHost = null;
			CurrentTabIndex = ActiveGameMenu.currentTab;

			GameWindowBounds = new CreateMenuPageParams { X = ActiveGameMenu.xPositionOnScreen, Y = ActiveGameMenu.yPositionOnScreen, Width = ActiveGameMenu.width, Height = ActiveGameMenu.height };
            if (ActiveGameMenu.junimoNoteIcon != null)
                JuminoIconDefaultBounds = ActiveGameMenu.junimoNoteIcon.bounds;
            else
                JuminoIconDefaultBounds = Rectangle.Empty;
            
            //Override pages
            GameMenuTabList = Helper.Reflection.GetField<List<ClickableComponent>>(ActiveGameMenu, "tabs").GetValue();
            GameMenuPageList = Helper.Reflection.GetField<List<IClickableMenu>>(ActiveGameMenu, "pages").GetValue();

			bool alreadyExtended = false;

            for (int i = 0; i < VanillaTabs.Count; i++)
            {
				var currentTab = VanillaTabs.ElementAt(i);
                var currentPage = GameMenuPageList[i];

				if(currentPage is GameMenuPageExtender)
				{
					alreadyExtended = true;
					break;
				}

                currentTab.TabButton = GameMenuTabList[i];
                
                var vanillaPageMenuType = VanillaTab.GetDefaultTabPageType(currentTab.TabName);
                
                //if the current page is not vanilla (overrided by another mod)
                if (currentPage.GetType() != vanillaPageMenuType)
                {
                    currentTab.VanillaPage.PageWindow = null; //force recreate Vanilla Tab Page
                    RegisterNonApiTabPage(currentTab, currentPage);
                }
                else
                {
                    currentTab.VanillaPage.PageWindow = currentPage;
                }

                GameMenuPageList[i] = currentTab.PageExtender;
            }

			if (alreadyExtended)
			{
				Monitor.Log("ExtendGameMenu was called twice on the same instance");
				return;
			}

            //Ensure that all tabs are instanciated and initialized. 
            //Re -instanciate any custom menus that were previously loaded following game logic (the menu and tabs always get re-instanciated)
            foreach (var tabPage in AllTabPages)
            {
                if (tabPage.IsCustom && tabPage.PageWindow == null)
				{
					tabPage.InitializeWindow();
					tabPage.CalculateGameMenuOffset(ActiveGameMenu);
				}
                else
                {
                    tabPage.InitializeWindow(tabPage.IsCustom && !(tabPage is CustomTabPage ctp && ctp.IsNonAPI));
                }
            }

            //fix UI scaling bug in option page. 
            //there is code in the game to scroll back to the scaling option but the menu gets instanciated twice for unknown reason and the second time the parameter is not set
            if (SelectedOptionFix != -1)
			{
				var optionsPage = ActiveGameMenu.GetPage<OptionsPage>();
                if (optionsPage != null)
                    optionsPage.currentItemIndex = SelectedOptionFix;
			}

            ValidateTabConfigs();

            RebuildCustomTabButtons();

            foreach (var tab in AllTabs)
			{
				tab.SelectDefaultPage();
				if (tab.IsVanilla)
					(tab as VanillaTab).InitializeLayout();
			}

			IsGameMenuExtended = true;
		}

        private CustomTabPage RegisterNonApiTabPage(VanillaTab tab, IClickableMenu customPage)
        {
            CustomTabPage customTabPage = null;
            var customPageMod = Helper.ModRegistry.GetModByType(customPage.GetType());

            if (customPageMod != null)
            {
                customTabPage = CustomTabPages.FirstOrDefault(p => p.Tab == tab && p.SourceMod == customPageMod && p.IsNonAPI);

                if (customTabPage == null)
                {
                    Monitor.Log($"The tab page '{tab.Name}' is overrided by another mod ({customPageMod.Name})", LogLevel.Info);
                    customTabPage = RegisterTabPageExtension(customPageMod, tab.Name, tab.Name, customPageMod.Name, customPage.GetType(), false);
                    customTabPage.PageWindow = customPage;
                    customTabPage.CalculateGameMenuOffset(ActiveGameMenu);

                    if (customTabPage.Tab.TabPages.Count(p => p.IsCustom && p.Visible) == 1 && customTabPage.Visible && customTabPage.Configuration.IsNew)
                    {
                        tab.Configuration.DefaultPage = customTabPage.Name;
                        tab.Configuration.PageVisible = false;
                        Mod.Configs.SaveConfigs();
                    }
                }
                else
                    customTabPage.PageWindow = customPage;
            }
            else
            {
                Monitor.Log($"The tab page '{tab.Name}' seems to be overrided by another mod but could not be identified.", LogLevel.Warn);
            }

            return customTabPage;
        }

		#region Tab Handling

		private CustomTab CurrentTabOverride;
		private VanillaTab CustomTabHost;
		private int CurrentTabIndex;
		private bool ChangingTab = false;

		public void ChangeTab(GameMenuTab newTab)
		{
			if (newTab == CurrentTab || newTab == null)
				return;

			ChangingTab = true;

			if (newTab.IsVanilla)
			{
				ReturnToVanillaTab();
				var vanillaTab = (VanillaTab)newTab;
				if (ActiveGameMenu.currentTab != vanillaTab.TabIndex)
					ActiveGameMenu.changeTab(vanillaTab.TabIndex);
			}
			else
			{
				CurrentTabOverride = (CustomTab)newTab;
				CustomTabHost = CurrentTabReal;
				CustomTabHost.OverrideSelectedTabOffset();
			}

			CurrentTabIndex = CurrentTabReal.TabIndex;

			newTab.SelectDefaultPage();
			CurrentTabReal.RebuildLayoutForCurrentTab();
			ChangingTab = false;

            OnCurrentTabPageChanged();
        }

		internal void ReturnToVanillaTab()
		{
			if(CurrentTabOverride != null)
			{
				CustomTabHost.RemoveTabOffsetOverride();
				CustomTabHost = null;
				CurrentTabOverride = null;
				CurrentTabReal.InitializeLayout();
			}
		}

        internal void OnCurrentTabPageChanged()
        {
            CurrentTabPageChanged?.Invoke(null, EventArgs.Empty);
            //Monitor.Log($"Current Tab Page is {(CurrentTabPage != null ? CurrentTabPage.Name : "null")}");
        }

        private void ValidateTabConfigs()
        {
            bool isWaitingForMapToLoad = false;

            foreach (var tab in AllTabs)
            {
                if (!tab.TabPages.Any(p => p.NameEquals(tab.Configuration.DefaultPage)))
                {
                    if(tab.TabName == GameMenuTabs.Map)
                    {
                        var config = Mod.Configs.TabPagesConfigs.FirstOrDefault(c => tab.NameEquals(c.TabName) && c.Name.ToLower() == tab.Configuration.DefaultPage.ToLower());
                        if (config != null && config.IsNonAPI)
                        {
                            var modName = config.Name.Split(':')[0];
                            if (Helper.ModRegistry.IsLoaded(modName))
                            {
                                isWaitingForMapToLoad = true;
                                continue;
                            }
                        }
                    }

                    if (tab is VanillaTab vtab)
                        tab.Configuration.DefaultPage = vtab.VanillaPage.Name;
                    else
                        tab.Configuration.DefaultPage = tab.TabPages.OrderByDescending(p => p.Visible).FirstOrDefault()?.Name;
                }

                var defaultPage = tab.TabPages.FirstOrDefault(p => p.NameEquals(tab.Configuration.DefaultPage));
                if (defaultPage != null && !defaultPage.Visible)
                    defaultPage.Visible = true;
            }

            foreach(var tab in VanillaTabs)
            {
                if (!tab.Configuration.PageVisible 
                    && (!tab.TabPages.Any(p=> !p.IsVanilla && p.Visible) || tab.VanillaPage.NameEquals(tab.Configuration.DefaultPage)))
                {
                    if (tab.TabName != GameMenuTabs.Map || !isWaitingForMapToLoad)
                        tab.Configuration.PageVisible = true;
                }

                tab.VanillaPage.Visible = tab.Configuration.PageVisible;
                tab.TabButton.label = tab.Label;
                tab.VanillaPage.Label = tab.Configuration.PageTitle;
            }

            int curIndex = 0;
            foreach (var tab in CustomTabs.OrderBy(t => t.Configuration.Index))
                tab.Configuration.Index = curIndex++;

            foreach (var tab in AllTabs)
                tab.OrganizeTabPages();

            if (Mod.Configs.AllConfigs.Any(c => c.HasChanged))
                Mod.Configs.SaveConfigs();
        }

        #endregion

        private void RebuildCustomTabButtons()
        {
            var lastTabButton = VanillaTabs.Last().TabButton;
            int currentX = lastTabButton.bounds.Right;
            int currentY = lastTabButton.bounds.Top;

            foreach (var tab in CustomTabs.OrderBy(t => t.DisplayIndex))
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

            foreach (var tab in CustomTabs.OrderBy(t => t.DisplayIndex))
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

        #region Tab Page

        internal IClickableMenu CreatePageInstance(Type pageType, CreateMenuPageParams ctorParams)
        {
            try
            {
                if (pageType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool) }) != null)
                {
                    return (IClickableMenu)Activator.CreateInstance(pageType,
                        new object[] { ctorParams.X, ctorParams.Y, ctorParams.Width, ctorParams.Height, ctorParams.UpperRightCloseButton });
                }
                else if (pageType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }) != null)
                {
                    return (IClickableMenu)Activator.CreateInstance(pageType,
                        new object[] { ctorParams.X, ctorParams.Y, ctorParams.Width, ctorParams.Height });
                }
                else if (pageType.GetConstructor(new Type[0]) != null)
                {
                    var newPage = (IClickableMenu)Activator.CreateInstance(pageType);
                    newPage.initialize(ctorParams.X, ctorParams.Y, ctorParams.Width, ctorParams.Height, ctorParams.UpperRightCloseButton);
                    return newPage;
                }
                else
                {
                    Monitor.Log($"[CreatePageInstance] No constructor found for page of type {pageType?.Name ?? "null"}", LogLevel.Error);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"[CreatePageInstance] Could not create page of type {pageType?.Name ?? "null"}:\r\n{ex.ToString()}", LogLevel.Error);
            }

            return null;
        }

        #endregion

        #region API

        public CustomTab RegisterCustomTabPage(IManifest source, string tabName, string label, Type pageMenuClass)
        {
            Monitor.Log($"Registering a custom tab page '{tabName}' for mod {source.Name}");

            string uniqueName = $"{source.UniqueID}:{tabName}";

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

            newTab.LoadConfig();

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

            string uniqueName = $"{source.UniqueID}:{pageName}";
            if (CustomTabPages.Any(t => t.NameEquals(uniqueName)))
            {
                Monitor.Log($"Could not register a page extension for the mod '{source.Name}'. There is alreay a page extension named '{uniqueName}'.", LogLevel.Warn);
                return null;
            }

            var newTabPage = new CustomTabPage(foundTab, source, uniqueName, pageLabel, pageMenuClass) { IsNonAPI = !byAPI };

            newTabPage.LoadConfig();

            return newTabPage;
        }

        #endregion
    }
}
