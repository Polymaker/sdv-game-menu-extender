using GameMenuExtender.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.API
{
    public class GameMenuExtenderAPI : IGameMenuExtenderAPI
    {
        private GameMenuExtenderMod Mod;
        private IModHelper Helper => Mod.Helper;
        private IMonitor Monitor => Mod.Monitor;
        private GameMenuManager MenuManager => Mod.MenuManager;

        private List<CustomMenuEntry> RegisterQueue;

        public event EventHandler CurrentTabPageChanged
        {
            add
            {
                MenuManager.CurrentTabPageChanged += value;
            }
            remove
            {
                MenuManager.CurrentTabPageChanged -= value;
            }
        }

        private class CustomMenuEntry
        {
            public MenuType Type;
            public IManifest Source;
            public string TabName;
            public string PageName;
            public string Label;
            public Type PageMenuClass;
			public string DependsOn;
		}

        internal GameMenuExtenderAPI(GameMenuExtenderMod mod)
        {
            Mod = mod;
            RegisterQueue = new List<CustomMenuEntry>();
        }

        internal void PerformRegistration()
        {
			RegisterQueue = RegisterQueue
				.OrderBy(m => m.Type == MenuType.TabPage)
				.ThenBy(m => !string.IsNullOrEmpty(m.DependsOn)).ToList();

			while (RegisterQueue.Count > 0)
            {
				var entry = RegisterQueue[0];
				RegisterQueue.RemoveAt(0);

				if (entry.Type == MenuType.Tab)
                    MenuManager.RegisterCustomTabPage(entry.Source, entry.TabName, entry.Label, entry.PageMenuClass);
                else
                    MenuManager.RegisterTabPageExtension(entry.Source, entry.TabName, entry.PageName, entry.Label, entry.PageMenuClass);
			}

            if (Mod.Configs.AllConfigs.Any(c => c.HasChanged))
                Mod.Configs.Save();
        }

        /// <summary>
        /// Registers a custom tab (and page) in the game menu.
        /// </summary>
        /// <param name="tabName">The tab's identifier.</param>
        /// <param name="label">The tab's tooltip text and the tab's main page label.</param>
        /// <param name="pageMenuClass">The type (class) of the page UI. It must descend from IClickableMenu.</param>
        /// <returns>Returns the created tab page's ID.</returns>
        public string RegisterCustomTabPage(string tabName, string label, Type pageMenuClass)
        {
			Monitor.Log($"RegisterCustomTabPage(\"{tabName}\", \"{label}\", typeof({pageMenuClass.FullName}))", LogLevel.Debug);

            if (!ValidateParameters(pageMenuClass, out IManifest sourceMod))
                return string.Empty;

            if (!MenuManager.HasInitialized)
            {
                RegisterQueue.Add(new CustomMenuEntry
                {
                    Type = MenuType.Tab,
                    TabName = tabName,
                    Label = label,
                    PageMenuClass = pageMenuClass,
                    Source = sourceMod
                });
                return $"{sourceMod.UniqueID}:{tabName}";
            }

            var customTab = Mod.MenuManager.RegisterCustomTabPage(sourceMod, tabName, label, pageMenuClass);
            Mod.Configs.Save();

            return customTab?.Name;
        }

        /// <summary>
        /// Registers an additional page for an existing tab. It's possible to extend both custom and vanilla tabs.
        /// <para>The standard (vanilla) tab names are: Inventory, Skills, Social, Map, Crafting, Collections, Options, Exit</para>
        /// <para>To extend a custom tab, use the following format (without quotes): "ModUniqueID:TabName"</para>
        /// </summary>
        /// <param name="tabName">The tab's identifier on which to add a custom page.</param>
        /// <param name="pageName">The page's identifier.</param>
        /// <param name="pageLabel">The page label.</param>
        /// <param name="pageMenuClass">The type (class) of the page UI. It must descend from IClickableMenu.</param>
        /// <returns>Returns the created tab page ID.</returns>
        public string RegisterTabPageExtension(string tabName, string pageName, string pageLabel, Type pageMenuClass)
        {
			Monitor.Log($"RegisterTabPageExtension(\"{tabName}\", \"{pageName}\", \"{pageLabel}\", typeof({pageMenuClass.FullName}))", LogLevel.Debug);

			if (!ValidateParameters(pageMenuClass, out IManifest sourceMod))
                return string.Empty;

            if (!MenuManager.HasInitialized)
            {
				RegisterQueue.Add(new CustomMenuEntry
				{
					Type = MenuType.TabPage,
					TabName = tabName,
					PageName = pageName,
					Label = pageLabel,
					PageMenuClass = pageMenuClass,
					Source = sourceMod,
					DependsOn = tabName.Contains(':') ? tabName.Split(':')[0] : string.Empty
				});
                return $"{sourceMod.UniqueID}:{pageName}";
            }

            var customPage = Mod.MenuManager.RegisterTabPageExtension(sourceMod, tabName, pageName, pageLabel, pageMenuClass);
            Mod.Configs.Save();

            return customPage?.Name;
        }

        private bool ValidateParameters(Type pageMenuClass, out IManifest sourceMod)
        {
            sourceMod = null;

            try
            {
                sourceMod = Helper.ModRegistry.GetModByType(pageMenuClass);
                if (sourceMod == null)
                    sourceMod = Helper.ModRegistry.GetCallingMod();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Critical Error: An exception occured while tracing the calling mod: {ex.ToString()}", LogLevel.Error);
                return false;
            }

            if (sourceMod == null)
            {
                Monitor.Log("Critical Error: Could not trace which mod tried to register a tab or page.", LogLevel.Warn);
                return false;
            }

            if (pageMenuClass == null || !pageMenuClass.IsSubclassOf(typeof(IClickableMenu)))
            {
                Monitor.Log($"Warning, the mod '{sourceMod.Name}' tried to register a tab page which type is either undefined or does not inherit from IClickableMenu.", LogLevel.Error);
                return false;
            }

            return true;
        }

		public IClickableMenu GetCurrentTabPage()
		{
			return MenuManager.CurrentTabPage?.PageWindow;
		}

        public string GetCurrentTabPageName()
        {
            return MenuManager.CurrentTabPage?.Name;
        }

        public void SetPageVisibillity(string pageID, bool visible)
        {
            var foundPage = MenuManager.AllTabPages.FirstOrDefault(p => p.NameEquals(pageID));
            if (foundPage != null && 
                MenuManager.CurrentTabPage != foundPage &&
                !foundPage.IsVanillaOverride &&
                (visible || foundPage.Tab.VisibleTabPages.Count() > 1))
            {
                foundPage.Visible = visible;
                if (MenuManager.IsGameMenuOpen)
                {

                }
            }
        }
    }
}
