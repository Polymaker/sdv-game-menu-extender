using GameMenuExtender.Configs;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public abstract class GameMenuTab : GameMenuElement, API.ITabInfo
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

        public IEnumerable<GameMenuTabPage> VisibleTabPages
        {
            get { return _TabPages.Where(p => p.IsVisible()); }
        }

        protected List<GameMenuTabPage> PageList => _TabPages;

		internal int CurrentTabPageIndex;

        public GameMenuTabPage CurrentTabPage => (CurrentTabPageIndex >= 0 && CurrentTabPageIndex < TabPages.Count) ? TabPages[CurrentTabPageIndex] : null;

        public bool TabPageInitialized => CurrentTabPage != null && CurrentTabPage.IsVisible();

        public MenuTabConfig Configuration { get; private set; }

        public bool Suppressed => !(Configuration?.Visible ?? true);

        bool API.ITabInfo.Visible { get => Visible; set => SetVisibleFromAPI(value); }

        bool API.ITabInfo.Enabled { get => Enabled; set => SetEnabledFromAPI(value); }

        internal GameMenuTab(GameMenuManager manager, string name) : base(manager, name)
        {
            _TabPages = new List<GameMenuTabPage>();
            UniqueID = name;
            CurrentTabPageIndex = -1;
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
				if (TabPages[i].IsVisible())
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
            if (CurrentTabPageIndex != index)
            {
                CurrentTabPageIndex = index;

                if (IsSelected)
                {
                    Manager.CurrentTabReal.RebuildLayoutForCurrentTab();
                    Manager.OnCurrentTabPageChanged();
                }
            }
        }

        public virtual void LoadConfig()
        {
            Configuration = Manager.Configuration.LoadOrCreateConfig(this);
            Label = Configuration.Title;
        }

        public void OrganizeTabPages()
        {
            //int currentIndex = 0;

            //foreach (var page in TabPages.OrderBy(p => p.DisplayIndex))
            //{
            //    page.DisplayIndex = currentIndex++;
            //}

            _TabPages = _TabPages.OrderBy(p => p.DisplayIndex).ToList();
        }

        public bool IsVisible()
        {
            return Configuration.Visible && Visible;
        }

        internal void SetVisibleFromAPI(bool value)
        {
            if (IsVanilla || (Manager.IsGameMenuOpen && Manager.CurrentTab == this))
                return;

            Visible = value;

            if (Manager.IsGameMenuOpen)
                Manager.RefreshMenu();
        }

        internal void SetEnabledFromAPI(bool value)
        {
            if (IsVanilla || (Manager.IsGameMenuOpen && Manager.CurrentTab == this))
                return;

            Enabled = value;
        }
    }
}
