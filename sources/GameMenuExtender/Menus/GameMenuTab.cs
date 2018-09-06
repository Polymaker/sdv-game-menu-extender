using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public abstract class GameMenuTab : GameMenuElement
    {
        private List<GameMenuTabPage> _TabPages;

        public override MenuType Type => MenuType.Tab;

        public int TabIndex { get; protected set; }

        public int VisibleIndex => Visible ? Manager.MenuTabs.Where(t => t.Visible).ToList().IndexOf(this) : -1;

        public ClickableComponent TabButton { get; internal set; }

        MenuPageTabsLocation PageTabsLocation { get; set; }

        public IList<GameMenuTabPage> TabPages
        {
            get { return _TabPages.AsReadOnly(); }
        }

        internal int CurrentTabPageIndex;

        public GameMenuTabPage CurrentTabPage => CurrentTabPageIndex >= 0 && CurrentTabPageIndex < TabPages.Count ? TabPages[CurrentTabPageIndex] : null;//TabPages.Where(p=>p.Visible).ToList().el

        internal GameMenuPageExtender PageExtender;

        internal GameMenuTab(int index)
        {
            TabIndex = index;
            _TabPages = new List<GameMenuTabPage>();
            PageExtender = new GameMenuPageExtender(this);
        }

        public void AddTabPage(GameMenuTabPage page)
        {
            if (!_TabPages.Contains(page))
                _TabPages.Add(page);
            page.Tab = this;
        }

        public void SelectTabPage(int index)
        {

        }
    }
}
