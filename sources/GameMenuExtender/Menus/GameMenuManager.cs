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
		internal GameMenuExtenderMod Mod { get; private set; }
		public IModHelper Helper => Mod.Helper;

		public IMonitor Monitor => Mod.Monitor;


		public bool HasInitialized { get; private set; }

        internal GameMenu ActiveGameMenu { get; private set; }

		#region Tabs & Tab Pages Properties

		public IEnumerable<GameMenuTab> AllTabs => VanillaTabs.Select(t => (GameMenuTab)t).Concat(CustomTabs);

		public List<CustomTab> CustomTabs { get; private set; }

		public List<VanillaTab> VanillaTabs { get; private set; }

		public IEnumerable<GameMenuTabPage> AllTabPages => AllTabs.SelectMany(t => t.TabPages);

		public IEnumerable<CustomTabPage> CustomTabPages => AllTabs.SelectMany(t => t.TabPages).OfType<CustomTabPage>();

		public GameMenuTab CurrentTab => IsGameMenuOpen ? ((GameMenuTab)CurrentTabOverride ?? CurrentTabReal) : null;

		public GameMenuTabPage CurrentTabPage => CurrentTab?.CurrentTabPage;

		/// <summary>
		/// Gets the current vanilla tab that is displayed or that displays the current custom tab.
		/// </summary>
		public VanillaTab CurrentTabReal => ActiveGameMenu != null ? VanillaTabs[ActiveGameMenu.currentTab] : null;

		#endregion

		public bool IsGameMenuOpen => StardewValley.Game1.activeClickableMenu is GameMenu;

		public bool IsGameMenuExtended { get; private set; }

        internal CreateMenuPageParams GameWindowBounds;

        internal GameMenuManager(GameMenuExtenderMod mod)
        {
			Mod = mod;
			VanillaTabs = new List<VanillaTab>();
			CustomTabs = new List<CustomTab>();
			MenuEvents.MenuChanged += MenuEvents_MenuChanged;
			MenuEvents.MenuClosed += MenuEvents_MenuClosed;
        }

		#region Game Menu Events Management

		private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
		{
			if (e.NewMenu is GameMenu)
			{
				ActiveGameMenu = (GameMenu)e.NewMenu;
				GameEvents.SecondUpdateTick += MenuEvents_AfterMenuChanged;
			}
		}

		private void MenuEvents_AfterMenuChanged(object sender, EventArgs e)
		{
			GameEvents.SecondUpdateTick -= MenuEvents_AfterMenuChanged;
			GameEvents.UpdateTick += GameEvents_UpdateTick;
			OnGameMenuOpened();
		}

		private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
		{
			if (e.PriorMenu is GameMenu)
			{
				GameEvents.UpdateTick -= GameEvents_UpdateTick;
				OnGameMenuClosed();
				ActiveGameMenu = null;
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

		#region Game Menu Tracking

		private void OnGameMenuOpened()
		{
			if (!HasInitialized)
				Initialize();
			ExtendGameMenu();
		}

		private void OnGameMenuClosed()
		{
			CurrentTabOverride = null;
			CustomTabHost = null;
			CurrentTabIndex = 0;
			IsGameMenuExtended = false;
		}

		private void OnGameMenuUpdate()
		{
			if (ActiveGameMenu != null && ActiveGameMenu.currentTab != CurrentTabIndex && !ChangingTab)
			{
				if (CurrentTabOverride != null)
					ReturnToVanillaTab();
				CurrentTabIndex = ActiveGameMenu.currentTab;
				CurrentTabReal.SelectFirstPage();
				CurrentTabReal.InitializeLayout();
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
				}
            }
        }

		//private CreateMenuPageParams GetGameMenuBounds()
		//{
		//	return new CreateMenuPageParams { X = ActiveGameMenu.xPositionOnScreen, Y = ActiveGameMenu.yPositionOnScreen, Width = ActiveGameMenu.width, Height = ActiveGameMenu.height };
		//}

		internal void ExtendGameMenu()
        {
			CurrentTabOverride = null;
			CustomTabHost = null;
			CurrentTabIndex = ActiveGameMenu.currentTab;

			GameWindowBounds = new CreateMenuPageParams { X = ActiveGameMenu.xPositionOnScreen, Y = ActiveGameMenu.yPositionOnScreen, Width = ActiveGameMenu.width, Height = ActiveGameMenu.height };

			var currentTabs = Helper.Reflection.GetField<List<ClickableComponent>>(ActiveGameMenu, "tabs").GetValue();
            var currentPages = Helper.Reflection.GetField<List<IClickableMenu>>(ActiveGameMenu, "pages").GetValue();
            
            for (int i = 0; i < VanillaTabs.Count; i++)
            {
				var currentTab = VanillaTabs.ElementAt(i);

				currentTab.TabButton = currentTabs[i];

                var currentPage = currentPages[i];
                var vanillaPageMenuType = VanillaTab.GetDefaultTabPageType(currentTab.TabName);

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
                            customTabPage = RegisterTabPageExtension(customPageMod, currentTab.Name, currentTab.Name, customPageMod.Name, currentPage.GetType(), false);
							customTabPage.CalculateGameMenuOffset(ActiveGameMenu);
						}

                        if (customTabPage != null)
                            customTabPage.PageWindow = currentPage;
					}
					else
					{
						Monitor.Log($"The tab page '{currentTab.Name}' seems to be overrided by another mod but could not be identified.");
					}
                }

                currentPages[i] = currentTab.PageExtender;
            }

            foreach (var tabPage in AllTabPages)
            {
                if (tabPage.IsCustom && tabPage.PageWindow == null)
				{
					tabPage.InstanciatePageWindow();
					tabPage.CalculateGameMenuOffset(ActiveGameMenu);
				}

				tabPage.InitializeWindow();
			}

            RebuildCustomTabButtons();

            foreach (var tab in AllTabs)
			{
				tab.SelectFirstPage();
				if (tab.IsVanilla)
					(tab as VanillaTab).InitializeLayout();
			}

			IsGameMenuExtended = true;
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

			newTab.SelectFirstPage();
			CurrentTabReal.RebuildLayoutForCurrentTab();
			ChangingTab = false;
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

		#endregion

		private void RebuildCustomTabButtons()
        {
            var lastTabButton = VanillaTabs.Last().TabButton;
            int currentX = lastTabButton.bounds.Right;
            int currentY = lastTabButton.bounds.Top;

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

			return new CustomTabPage(foundTab, source, uniqueName, pageLabel, pageMenuClass) { IsNonAPI = !byAPI };
        }

        #endregion
    }
}
