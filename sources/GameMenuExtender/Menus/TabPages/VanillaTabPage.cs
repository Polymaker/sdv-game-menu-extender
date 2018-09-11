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

        internal VanillaTabPage(VanillaTab tab, IClickableMenu window) : base(tab, tab.Name)
        {
            PageWindow = window;
            Label = tab.Label;
            PageType = window.GetType();
        }
    }
}
