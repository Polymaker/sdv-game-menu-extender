using GameMenuExtender.API;
using GameMenuExtender.Configs;
using GameMenuExtender.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameMenuExtender
{
	public class GameMenuExtenderMod : Mod
	{
        internal GameMenuManager MenuManager;
		internal GameMenuExtenderAPI ApiInstance;

        internal static GameMenuExtenderMod Instance { get; private set; }

        public override void Entry(IModHelper helper)
		{
            MenuManager = new GameMenuManager(this);
            ApiInstance = new GameMenuExtenderAPI(this);
            Helper.Events.GameLoop.SaveLoaded += SaveEvents_AfterLoad;
            Instance = this;
        }

		private void SaveEvents_AfterLoad(object sender, SaveLoadedEventArgs e)
		{
            MenuManager.Initialize();
        }

		public override object GetApi()
		{
			return ApiInstance;
		}
    }
}
