﻿using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuTest
{
	public class CustomMenuTestMod : Mod
	{
        private IGameMenuExtenderAPI MenuAPI;
        public override void Entry(IModHelper helper)
		{
			GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
		}

		private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
		{
			GameEvents.FirstUpdateTick -= GameEvents_FirstUpdateTick;
            MenuAPI = Helper.ModRegistry.GetApi<IGameMenuExtenderAPI>("Polymaker.GameMenuExtender");

            MenuAPI.RegisterTabPageExtension("Social", "MyPage", "My Page", typeof(MyCustomMenuPage));
            MenuAPI.RegisterCustomTabPage("MyTab","I'm the best", typeof(MyCustomMenuPage));
            MenuAPI.RegisterTabPageExtension($"{ModManifest.UniqueID}:MyTab", "MyPage2", "My Page 2", typeof(MyCustomMenuPage2));
            //MenuAPI.CurrentTabPageChanged += MenuAPI_CurrentTabPageChanged;
            //MenuAPI.RegisterGameMenuTab("MyTab", typeof(MyCustomMenuPage));
        }

        private void MenuAPI_CurrentTabPageChanged(object sender, EventArgs e)
        {
            Monitor.Log($"Current Tab Page Changed: {MenuAPI.GetCurrentTabPageName()}");
        }
    }

    public interface IGameMenuExtenderAPI
    {
        /// <summary>
        /// 
        /// </summary>
        event EventHandler CurrentTabPageChanged;

        /// <summary>
        /// Registers a custom tab in the game menu.
        /// </summary>
        /// <param name="tabName">The tab's identifier.</param>
        /// <param name="label">The tab's tooltip text and the tab's main page label.</param>
        /// <param name="pageMenuClass">The class of the page menu IClickableMenu.</param>
        void RegisterCustomTabPage(string tabName, string label, Type pageMenuClass);

		/// <summary>
		/// Registers an additional page for an existing tab. It is possible to extend both custom and vanilla tabs.
		/// <para>The standard (vanilla) tab names are: Inventory, Skills, Social, Map, Crafting, Collections, Options, Exit</para>
		/// <para>To extend a custom tab, use the following format: ModUniqueID.TabName</para>
		/// </summary>
		/// <param name="tabName">The tab's name.</param>
		/// <param name="pageName">The page's identifier.</param>
		/// <param name="pageLabel">The page label.</param>
		/// <param name="pageMenuClass">A type desce</param>
		void RegisterTabPageExtension(string tabName, string pageName, string pageLabel, Type pageMenuClass);

        /// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IClickableMenu GetCurrentTabPage();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetCurrentTabPageName();
    }
}
