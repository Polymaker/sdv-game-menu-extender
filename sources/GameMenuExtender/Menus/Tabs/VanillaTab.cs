using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
    public class VanillaTab : GameMenuTab
    {
        public override bool IsCustom => false;

		public int TabIndex { get; protected set; }

		public override bool Enabled { get => true; }

        public override bool Visible { get => true; }

        public GameMenuTabs TabName => (GameMenuTabs)TabIndex;

        public VanillaTabPage VanillaPage => TabPages.OfType<VanillaTabPage>().FirstOrDefault();

        internal GameMenuPageExtender PageExtender { get; private set; }

        internal VanillaTab(GameMenuManager manager, int index, ClickableComponent tab) : base(manager, tab.name)
        {
			TabIndex = index;
            TabButton = tab;
            Label = tab.label;
            UniqueID = $"StardewValley:{tab.name}";
            PageExtender = new GameMenuPageExtender(this);
        }

		internal void RemoveAllCustomPages()
		{
			PageList.RemoveAll(p => p.IsCustom);
		}

		internal void InitializeLayout()
		{
			var curPage = Manager.CurrentTabPage.PageWindow;
            if (curPage != null)
            {
                PageExtender.initialize(curPage.xPositionOnScreen, curPage.yPositionOnScreen,
                                curPage.width, curPage.height, curPage.upperRightCloseButton != null);

                PageExtender.RebuildTabPagesButtons(this);
            }
		}
	}
}
