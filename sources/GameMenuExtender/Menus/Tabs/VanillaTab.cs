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

		public override bool Enabled { get => true; set => throw new NotSupportedException(); }

        public override bool Visible { get => true; set => throw new NotSupportedException(); }

        public GameMenuTabs TabName => (GameMenuTabs)TabIndex;

        public VanillaTabPage VanillaPage => TabPages.OfType<VanillaTabPage>().FirstOrDefault();

		public VanillaTab(int index, ClickableComponent tab)// : base(index)
        {
			TabIndex = index;
            TabButton = tab;
            Name = tab.name;
		}

		internal void RemoveAllCustomPages()
		{
			PageList.RemoveAll(p => p.IsCustom);
		}

		internal void InitializeLayout()
		{
			var curPage = CurrentTabPage.PageWindow;
			PageExtender.initialize(curPage.xPositionOnScreen, curPage.yPositionOnScreen,
				curPage.width, curPage.height, curPage.upperRightCloseButton != null);
			PageExtender.BuildTabButtons();
		}
	}
}
