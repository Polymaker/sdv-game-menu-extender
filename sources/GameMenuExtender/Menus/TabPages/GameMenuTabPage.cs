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

        internal GameMenuTabPage(GameMenuTab tab)
        {
            Tab = tab;
			Tab.AddTabPage(this);
            //if (!tab.TabPages.Contains(this))
            //    tab..Add(this);
        }

        internal GameMenuTabPage()
        {

        }
		
		public static IClickableMenu CreatePageInstance(Type pageType, CreateMenuPageParams ctorParams)
		{
			var ctors = pageType.GetConstructors();
			var getParamTypes = (Func<ConstructorInfo, Type[]>)(c =>
			{
				return c.GetParameters().Select(p => p.ParameterType).ToArray();
			});

			try
			{
				if (ctors.Any(c => getParamTypes(c) == new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool) }))
				{
					return (IClickableMenu)Activator.CreateInstance(pageType,
						new object[] { ctorParams.x, ctorParams.y, ctorParams.width, ctorParams.height, ctorParams.upperRightCloseButton });
				}
				else if (ctors.Any(c => getParamTypes(c) == new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }))
				{
					return (IClickableMenu)Activator.CreateInstance(pageType,
						new object[] { ctorParams.x, ctorParams.y, ctorParams.width, ctorParams.height });
				}
				else if (ctors.Any(c => getParamTypes(c).Length == 0))
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
    }
}
