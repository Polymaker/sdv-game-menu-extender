using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public abstract class GameMenuTabPage : GameMenuElement
    {
        public override MenuType Type => MenuType.TabPage;

        public GameMenuTab Tab { get; internal set; }

        public virtual string Tooltip { get; set; }

		public ClickableComponent TabPageButton { get; internal set; }

		public Type PageType { get; protected set; }

		public IClickableMenu PageWindow { get; internal set; }

        public int VisibleIndex => Visible ? Tab.TabPages.Where(t => t.Visible).ToList().IndexOf(this) : -1;

        internal GameMenuTabPage(GameMenuTab tab, string name) : base(tab.Manager, name)
        {
            Tab = tab;
            Tab.AddTabPage(this);
            UniqueID = $"{Tab.UniqueID}::{Name}";
        }

		internal CreateMenuPageParams GetMenuPageParams()
		{
			if (PageWindow != null)
				return new CreateMenuPageParams {
					x = PageWindow.xPositionOnScreen,
					y = PageWindow.yPositionOnScreen,
					width = PageWindow.width,
					height = PageWindow.height,
					upperRightCloseButton = (PageWindow.upperRightCloseButton != null)
				};
			return default(CreateMenuPageParams);
		}
		
		public static IClickableMenu CreatePageInstance(Type pageType, CreateMenuPageParams ctorParams)
		{
			try
			{
				if (pageType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool) }) != null)
				{
					return (IClickableMenu)Activator.CreateInstance(pageType,
						new object[] { ctorParams.x, ctorParams.y, ctorParams.width, ctorParams.height, ctorParams.upperRightCloseButton });
				}
				else if (pageType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }) != null)
				{
					return (IClickableMenu)Activator.CreateInstance(pageType,
						new object[] { ctorParams.x, ctorParams.y, ctorParams.width, ctorParams.height });
				}
				else if (pageType.GetConstructor(new Type[0]) != null)
				{
					var newPage = (IClickableMenu)Activator.CreateInstance(pageType);
					newPage.initialize(ctorParams.x, ctorParams.y, ctorParams.width, ctorParams.height, ctorParams.upperRightCloseButton);
					return newPage;
				}
			}
			catch /*(Exception ex)*/
			{
				
			}

			return null;
		}

        internal virtual void InstanciatePageWindow()
        {

        }
    }
}
