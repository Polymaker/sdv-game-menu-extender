using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public abstract class GameMenuTabPage : GameMenuElement
    {
        public override MenuType Type => MenuType.TabPage;

        public GameMenuTab Tab { get; internal set; }

        public virtual string Tooltip { get; set; }

        public IClickableMenu PageWindow { get; internal set; }

        public int VisibleIndex => Visible ? Tab.TabPages.Where(t => t.Visible).ToList().IndexOf(this) : -1;

        internal GameMenuTabPage(GameMenuTab tab)
        {
            Tab = tab;
            if (!tab.TabPages.Contains(this))
                tab.TabPages.Add(this);
        }

        internal GameMenuTabPage()
        {
        }

       
    }
}
