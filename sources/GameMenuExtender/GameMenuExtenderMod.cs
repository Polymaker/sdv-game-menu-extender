using GameMenuExtender.API;
using GameMenuExtender.Config;
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
        internal ConfigManager Configs { get; private set; }

        public override void Entry(IModHelper helper)
		{
            MenuManager = new GameMenuManager(this);
            ApiInstance = new GameMenuExtenderAPI(this);
            Configs = new ConfigManager(this);

            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
		}

		private void SaveEvents_AfterLoad(object sender, System.EventArgs e)
		{
            Configs.LoadConfigs();
            MenuManager.Initialize();
            MenuManager.InitializeCompatibilityFixes();

        }

		public override object GetApi()
		{
			return ApiInstance;
		}

    }
}
