using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMenuTest
{
	public class CustomMenuTestMod : Mod
	{
		

		public override void Entry(IModHelper helper)
		{
			GameEvents.FirstUpdateTick += GameEvents_FirstUpdateTick;
		}

		private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
		{
			GameEvents.FirstUpdateTick -= GameEvents_FirstUpdateTick;
			var menuAPI = Helper.ModRegistry.GetApi<IGameMenuExtenderApi>("Polymaker.GameMenuExtender");
			menuAPI.RegisterGameMenuExtension("Social", typeof(MyCustomMenuPage), "test page");
		}
	}

	public interface IGameMenuExtenderApi
	{
		void RegisterGameMenuExtension(string targetTab, Type customPageType, string label);
	}
}
