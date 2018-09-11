using GameMenuExtender.Menus;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StardewValley.Menus
{
	internal static class GameMenuHelper
	{
		private static FieldInfo PageField;

		static GameMenuHelper()
		{
			PageField = typeof(GameMenu).GetField("pages", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public static List<IClickableMenu> GetPages(this GameMenu menu)
		{
			return PageField.GetValue(menu) as List<IClickableMenu>;
		}

		public static List<IClickableMenu> GetVanillaPages(this GameMenu menu)
		{
			var pages = GetPages(menu);
			if (pages.OfType<GameMenuPageExtender>().Any())
			{
				return pages.OfType<GameMenuPageExtender>().Select(p => p.MenuTab.VanillaPage.PageWindow).ToList();
			}
			return PageField.GetValue(menu) as List<IClickableMenu>;
		}

		public static IClickableMenu GetPage(this GameMenu menu, GameMenuExtender.GameMenuTabs tab)
		{
			var pages = GetVanillaPages(menu);
			return pages?[(int)tab];
		}

		public static T GetPage<T>(this GameMenu menu) where T : IClickableMenu
		{
			var pages = GetVanillaPages(menu);

			return pages?.OfType<T>().FirstOrDefault();
		}
	}
}
