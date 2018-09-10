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
        }

        public void RegisterCustomTabPage(string tabName, string label, Type pageMenuClass)
        {
			Monitor.Log($"RegisterCustomTabPage(\"{tabName}\", \"{label}\", typeof({pageMenuClass.FullName}))", LogLevel.Debug);

            if (!ValidateParameters(pageMenuClass, out IManifest sourceMod))
                return;

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
                return;
            }

            Mod.MenuManager.RegisterCustomTabPage(sourceMod, tabName, label, pageMenuClass);
        }

        public void RegisterTabPageExtension(string tabName, string pageName, string pageLabel, Type pageMenuClass)
        {
			Monitor.Log($"RegisterTabPageExtension(\"{tabName}\", \"{pageName}\", \"{pageLabel}\", typeof({pageMenuClass.FullName}))", LogLevel.Debug);

			if (!ValidateParameters(pageMenuClass, out IManifest sourceMod))
                return;

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
                return;
            }

            Mod.MenuManager.RegisterTabPageExtension(sourceMod, tabName, pageName, pageLabel, pageMenuClass);
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
	}
}
