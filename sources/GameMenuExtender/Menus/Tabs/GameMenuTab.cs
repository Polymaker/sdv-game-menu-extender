using GameMenuExtender.Config;
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

		public ClickableComponent TabButton { get; internal set; }

        MenuPageTabsLocation PageTabsLocation { get; set; }

        public abstract GameMenuTabs TabName { get; }

        public IList<GameMenuTabPage> TabPages
        {
            get { return _TabPages.AsReadOnly(); }
        }

		protected List<GameMenuTabPage> PageList => _TabPages;

		internal int CurrentTabPageIndex;

        public GameMenuTabPage CurrentTabPage => (CurrentTabPageIndex >= 0 && CurrentTabPageIndex < TabPages.Count) ? TabPages[CurrentTabPageIndex] : null;

        public GameMenuTabConfig Config { get; private set; }

        internal GameMenuTab(GameMenuManager manager, string name) : base(manager, name)
        {
            _TabPages = new List<GameMenuTabPage>();
            UniqueID = name;
        }

        public void AddTabPage(GameMenuTabPage page)
        {
            if (!_TabPages.Contains(page))
                _TabPages.Add(page);
            page.Tab = this;
        }

		internal void SelectFirstPage()
		{
			CurrentTabPageIndex = 0;

			for (int i = 0; i < TabPages.Count; i++)
			{
				if (TabPages[i].Visible)
				{
					CurrentTabPageIndex = i;
					break;
				}
			}
		}

        internal void SelectDefaultPage()
        {
            CurrentTabPageIndex = 0;

            for (int i = 0; i < TabPages.Count; i++)
            {
                if (TabPages[i].NameEquals(Config.DefaultPage))
                {
                    CurrentTabPageIndex = i;
                    break;
                }
            }
        }

        public void SelectTabPage(GameMenuTabPage page)
        {
            SelectTabPage(TabPages.IndexOf(page));
        }

		public void SelectTabPage(int index)
		{
			CurrentTabPageIndex = index;

            if (IsSelected)
            {
                Manager.CurrentTabReal.RebuildLayoutForCurrentTab();
                Manager.OnCurrentTabPageChanged();
            }
        }

        public void LoadConfig()
        {
            Config = Manager.Mod.Configs.LoadOrCreateConfig(this);
            Label = Config.Title;
        }

        public void OrganizeTabPages()
        {
            int currentIndex = 0;

            foreach(var page in TabPages.OrderByDescending(p => p.NameEquals(Config.DefaultPage)).ThenBy(p => p.DisplayIndex))
            {
                page.DisplayIndex = currentIndex++;
            }

            _TabPages = _TabPages.OrderBy(p => p.DisplayIndex).ToList();
        }
	}
}
