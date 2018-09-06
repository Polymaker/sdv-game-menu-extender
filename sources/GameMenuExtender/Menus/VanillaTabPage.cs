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
        private DefaultPageParams CtorsParams;
        private Type PageType;

        internal struct DefaultPageParams
        {
            public int x;
            public int y;
            public int width;
            public int height;
            public bool upperRightCloseButton;
        }

        internal VanillaTabPage(VanillaTab tab, IClickableMenu window) : base(tab)
        {
            PageWindow = window;
            Name = tab.Name;
            Label = tab.Label;
            PageType = window.GetType();

            CtorsParams = new DefaultPageParams()
            {
                x = window.xPositionOnScreen,
                y = window.yPositionOnScreen,
                width = window.width,
                height = window.height,
                upperRightCloseButton = window.upperRightCloseButton != null
            };
        }

        internal void InstanciateNewPage()
        {

        }
    }
}
