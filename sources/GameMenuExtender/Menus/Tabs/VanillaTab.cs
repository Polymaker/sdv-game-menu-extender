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

		public bool IsTabButtonOffseted { get; private set; }

        public GameMenuTabs TabName => (GameMenuTabs)TabIndex;

        public VanillaTabPage VanillaPage => TabPages.OfType<VanillaTabPage>().FirstOrDefault();

        internal GameMenuPageExtender PageExtender { get; private set; }

        internal VanillaTab(GameMenuManager manager, int index, ClickableComponent tab) : base(manager, tab.name)
        {
			TabIndex = index;
            TabButton = tab;
            Label = tab.label;
            UniqueID = $"StardewValley::{tab.name}";
            PageExtender = new GameMenuPageExtender(this);
        }

		internal VanillaTab(GameMenuManager manager, int index, GameMenuTabs tab) : base(manager, tab.ToString().ToLower())
		{
			TabIndex = index;
			//TabButton = tab;
			//Label = tab.label;
			UniqueID = $"StardewValley::{tab.ToString().ToLower()}";
			//PageExtender = new GameMenuPageExtender(this);
		}

		internal void Initialize(ClickableComponent tab, IClickableMenu page)
		{
			TabButton = tab;
			Label = tab.label;
			PageExtender = new GameMenuPageExtender(this);

		}

		//internal void RemoveAllCustomPages()
		//{
		//	PageList.RemoveAll(p => p.IsCustom);
		//}

		internal void InitializeLayout()
		{
			if(CurrentTabPage != null && CurrentTabPage.PageWindow != null)
			{
				PageExtender.Initialize(CurrentTabPage.PageWindow);
				PageExtender.RebuildTabPagesButtons(this);
			}
			IsTabButtonOffseted = false;
		}

		internal void RebuildLayoutForCurrentTab()
		{
			if (Manager.CurrentTab != null && Manager.CurrentTabPage.PageWindow != null)
			{
				PageExtender.Initialize(Manager.CurrentTabPage.PageWindow);
				PageExtender.RebuildTabPagesButtons(Manager.CurrentTab);
			}
		}

		internal void OverrideSelectedTabOffset()
		{
			if(TabButton != null && !IsTabButtonOffseted)
			{
				TabButton.bounds.Y -= 8;
				IsTabButtonOffseted = true;
			}
		}

		internal void RemoveTabOffsetOverride()
		{
			if (TabButton != null && IsTabButtonOffseted)
			{
				TabButton.bounds.Y += 8;
				IsTabButtonOffseted = false;
			}
		}

		internal static Type GetDefaultTabPageType(GameMenuTabs tab)
		{
			switch (tab)
			{
				case GameMenuTabs.Inventory:
					return typeof(InventoryPage);
				case GameMenuTabs.Skills:
					return typeof(SkillsPage);
				case GameMenuTabs.Social:
					return typeof(SocialPage);
				case GameMenuTabs.Map:
					return typeof(MapPage);
				case GameMenuTabs.Crafting:
					return typeof(CraftingPage);
				case GameMenuTabs.Collections:
					return typeof(CollectionsPage);
				case GameMenuTabs.Options:
					return typeof(OptionsPage);
				case GameMenuTabs.Exit:
					return typeof(ExitPage);
			}
			return null;
		}
	}
}
