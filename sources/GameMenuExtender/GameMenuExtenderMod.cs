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

        public override void Entry(IModHelper helper)
		{
            MenuManager = new GameMenuManager(this);
            ApiInstance = new GameMenuExtenderAPI(this);
			//MenuEvents.MenuChanged += MenuEvents_MenuChanged;
			SaveEvents.AfterLoad += SaveEvents_AfterLoad;
			//GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
		}

		//private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
		//{
		//	GameEvents.FirstUpdateTick -= GameEvents_FirstUpdateTick;
		//	//var otherMod = Helper.ModRegistry.GetMod("Polymaker.CustomMenuTest");
		//}

		private void SaveEvents_AfterLoad(object sender, System.EventArgs e)
		{
			MenuManager.Initialize();
		}

		//     private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
		//     {

		//if (e.NewMenu is GameMenu)
		//         {
		//             MenuManager.SetCurrentMenu((GameMenu)e.NewMenu);
		//             GameEvents.FourthUpdateTick += OnGameMenuShown;
		//         }
		//     }

		//     private void OnGameMenuShown(object sender, EventArgs e)
		//     {
		//         GameEvents.FourthUpdateTick -= OnGameMenuShown;
		//         MenuManager.ExtendGameMenu();
		//     }

		public override object GetApi()
		{
			return ApiInstance;
		}

    }
}
