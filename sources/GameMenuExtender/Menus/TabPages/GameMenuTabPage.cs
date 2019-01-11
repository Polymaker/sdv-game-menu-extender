using GameMenuExtender.API;
using GameMenuExtender.Configs;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public abstract class GameMenuTabPage : GameMenuElement, API.ITabPageInfo
    {
        public override MenuType Type => MenuType.TabPage;

        public GameMenuTab Tab { get; internal set; }

        ITabInfo ITabPageInfo.Tab => Tab;

        public virtual string Tooltip { get; set; }

        public Polymaker.SdvUI.SdvImage TabIcon { get; set; }

        public ClickableComponent TabPageButton { get; internal set; }

		public Type PageType { get; protected set; }

		public IClickableMenu PageWindow { get; internal set; }

        public MenuTabPageConfig Configuration { get; private set; }

        public bool IsVanillaOverride => Tab is VanillaTab vTab ? NameEquals(vTab.Configuration.VanillaPageOverride) : false;

        public int DisplayIndex
        {
            get => Configuration.Index;
            set => Configuration.Index = value;
        }

		public CreateMenuPageParams GameWindowOffset { get; protected set; }

        public string TabName => Tab.Name;

        public bool Suppressed => !(Configuration?.Visible ?? true);

        bool API.ITabPageInfo.Visible { get => Visible; set => SetVisibleFromAPI(value); }

        bool API.ITabPageInfo.Enabled { get => Enabled; set => SetEnabledFromAPI(value); }

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
					X = PageWindow.xPositionOnScreen,
					Y = PageWindow.yPositionOnScreen,
					Width = PageWindow.width,
					Height = PageWindow.height,
					UpperRightCloseButton = (PageWindow.upperRightCloseButton != null)
				};
			return default(CreateMenuPageParams);
		}

		internal void CalculateGameMenuOffset(GameMenu menu)
		{
			var pageBounds = GetMenuPageParams();
			if (pageBounds != default(CreateMenuPageParams))
			{
				GameWindowOffset = new CreateMenuPageParams
					(
					pageBounds.X - menu.xPositionOnScreen,
					pageBounds.Y - menu.yPositionOnScreen,
					pageBounds.Width - menu.width,
					pageBounds.Height - menu.height,
					pageBounds.UpperRightCloseButton
					);
			}
			else
				GameWindowOffset = default(CreateMenuPageParams);
		}

		internal void InitializeWindow(bool forceRecreate = false)
		{
            var menuBounds = Manager.GameWindowBounds;
            var finalBounds = menuBounds + GameWindowOffset;

            if (PageWindow != null && !forceRecreate)
                PageWindow.initialize(finalBounds.X, finalBounds.Y, finalBounds.Width, finalBounds.Height, PageWindow.upperRightCloseButton != null);
            else
			{
                var oldPage = PageWindow;

                PageWindow = Manager.CreatePageInstance(PageType, IsVanilla ? finalBounds : menuBounds);

                if(PageWindow == null && oldPage != null)
                {
                    PageWindow = oldPage;
                }
			}
        }

        public void LoadConfig()
        {
            Configuration = Manager.Configuration.LoadOrCreateConfig(this);
            Label = Configuration.Title;
        }

        public bool IsVisible()
        {
            return Configuration.Visible && Visible;
        }

        internal void SetVisibleFromAPI(bool value)
        {
            if (IsVanillaOverride || (Manager.IsGameMenuOpen && Manager.CurrentTabPage == this))
                return;

            if (!value && Tab.VisibleTabPages.Count(p => p.Enabled) <= 1)
                return;

            Visible = value;
        }

        internal void SetEnabledFromAPI(bool value)
        {
            if (IsVanillaOverride || (Manager.IsGameMenuOpen && Manager.CurrentTabPage == this))
                return;

            if (!value && Tab.VisibleTabPages.Count(p => p.Enabled) <= 1)
                return;

            Enabled = value;
        }
    }
}
