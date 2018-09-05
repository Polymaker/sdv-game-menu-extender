using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;

namespace GameMenuExtender
{
	public class GameMenuExtenderMod : Mod
	{
		public override void Entry(IModHelper helper)
		{
			
		}

		public override object GetApi()
		{
			return new GameMenuExtenderApi(this);
		}
	}
}
