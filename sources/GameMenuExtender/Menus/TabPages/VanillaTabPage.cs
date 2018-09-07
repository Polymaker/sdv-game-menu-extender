using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public class VanillaTabPage : GameMenuTabPage
    {
        public override bool IsCustom => false;
		internal CreateMenuPageParams DefaultCtorParams;

        internal VanillaTabPage(VanillaTab tab, IClickableMenu window) : base(tab, tab.Name)
        {
            PageWindow = window;
            Label = tab.Label;
            PageType = window.GetType();

            DefaultCtorParams = new CreateMenuPageParams()
            {
                x = window.xPositionOnScreen,
                y = window.yPositionOnScreen,
                width = window.width,
                height = window.height,
                upperRightCloseButton = window.upperRightCloseButton != null
            };
        }

        internal override void InstanciatePageWindow()
        {
            PageWindow = CreatePageInstance(PageType, DefaultCtorParams);
        }
    }
}
