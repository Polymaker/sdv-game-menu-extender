﻿using GameMenuExtender.Menus;
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

        private Queue<CustomMenuEntry> RegisterQueue;

        private struct CustomMenuEntry
        {
            public MenuType Type;
            public IManifest Source;
            public string TabName;
            public string PageName;
            public string Label;
            public Type PageMenuClass;
        }

        public GameMenuExtenderAPI(GameMenuExtenderMod mod)
        {
            Mod = mod;
            RegisterQueue = new Queue<CustomMenuEntry>();
        }

        internal void Foobar()
        {
            while (RegisterQueue.Count > 0)
            {
                var entry = RegisterQueue.Dequeue();
                if (entry.Type == MenuType.Tab)
                    MenuManager.RegisterCustomTabPage(entry.Source, entry.TabName, entry.Label, entry.PageMenuClass);
                else
                    MenuManager.RegisterTabPageExtension(entry.Source, entry.TabName, entry.PageName, entry.Label, entry.PageMenuClass);
            }
        }

        public void RegisterCustomTabPage(string tabName, string label, Type pageMenuClass)
        {
            if (!ValidateParameters(pageMenuClass, out IManifest sourceMod))
                return;

            if (!MenuManager.HasInitialized)
            {
                RegisterQueue.Enqueue(new CustomMenuEntry
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
            if (!ValidateParameters(pageMenuClass, out IManifest sourceMod))
                return;

            if (!MenuManager.HasInitialized)
            {
                RegisterQueue.Enqueue(new CustomMenuEntry
                {
                    Type = MenuType.TabPage,
                    TabName = tabName,
                    PageName = pageName,
                    Label = pageLabel,
                    PageMenuClass = pageMenuClass,
                    Source = sourceMod
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
    }
}
