using GameMenuExtender.Helpers;
using GameMenuExtender.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender
{
	internal /*sealed*/ class GameMenuPageExtender : IClickableMenu
	{
		public GameMenuTab MenuTab { get; private set; }

		public IClickableMenu OriginalPage { get; internal set; }

		public IClickableMenu CurrentOverride => CurrentPage?.Page ?? OriginalPage;

		public bool SuppressOriginal { get; protected set; }

		private List<PageExtensionTab> PageExtensions;

		private int currentPageIndex;

		private PageExtensionTab CurrentPage => PageExtensions[currentPageIndex];

		private const int TabPaddingX = 32;
		private const int TabPaddingY = 24;
        private const int LeftSideStartOffsetY = 20;

		static GameMenuPageExtender()
		{
			InitializeRedirections();
		}

		public GameMenuPageExtender(GameMenuTab tab)
		{
			MenuTab = tab;
			PageExtensions = new List<PageExtensionTab>();
		}

		public void Initialize(IClickableMenu sourePage)
		{
			initialize(sourePage.xPositionOnScreen, sourePage.yPositionOnScreen,
				sourePage.width, sourePage.height, sourePage.upperRightCloseButton != null);
		}

		public void InitializePageExtensions(IEnumerable<MenuPageEntry> entries)
		{

			PageExtensions.Clear();
            if (OriginalPage != null)
            {
                PageExtensions.Add(new PageExtensionTab()
                {
                    Index = 0,
                    Page = OriginalPage,
                    Label = MenuTab.Label
                });
            }
			

			foreach (var pageEntry in entries)
			{
				var customPage = pageEntry.MenuInstance ?? InstanciateCustomPage(pageEntry.PageClass);
				if(customPage != null)
				{
					PageExtensions.Add(new PageExtensionTab()
					{
						Index = PageExtensions.Count,
						Page = customPage,
						Label = pageEntry.Label,
						PageInfo = pageEntry
					});
				}
				else
				{
					
				}
			}
			UpdateCustomTabs();
			UpdateSubPages();
		}

        public IClickableMenu InstanciateCustomPage(Type customPageType)
        {
            var ctors = customPageType.GetConstructors();
            var getParamTypes = (Func<ConstructorInfo, Type[]>)(c =>
            {
                return c.GetParameters().Select(p => p.ParameterType).ToArray();
            });
            
            try
			{
                if (ctors.Any(c => getParamTypes(c) == new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool) }))
                {
                    return (IClickableMenu)Activator.CreateInstance(customPageType,
                        new object[] { xPositionOnScreen, yPositionOnScreen, width, height, upperRightCloseButton != null });
                }
                else if (ctors.Any(c => getParamTypes(c) == new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }))
                {
                    return (IClickableMenu)Activator.CreateInstance(customPageType,
                        new object[] { xPositionOnScreen, yPositionOnScreen, width, height, upperRightCloseButton != null });
                }
                else if (ctors.Any(c => getParamTypes(c).Length == 0))
                {
                    var newPage = (IClickableMenu)Activator.CreateInstance(customPageType);
                    newPage.initialize(xPositionOnScreen, yPositionOnScreen, width, height, upperRightCloseButton != null);
                    return newPage;
                }
            }
			catch
			{
				
			}

			return null;
		}

		private void UpdateCustomTabs()
		{

		}

		private void UpdateSubPages()
		{
			int currentPosY = yPositionOnScreen + 80;
            int maxLabelWidth = 0;

			foreach (var pageTab in PageExtensions)
			{
				if (!pageTab.Visible)
					continue;

				var labelSize = Game1.smallFont.MeasureString(pageTab.Label);
				int tabWidth = (int)labelSize.X + TabPaddingX;
				int tabHeight = (int)labelSize.Y + TabPaddingY;
                if (tabWidth > maxLabelWidth)
                    maxLabelWidth = tabWidth;
				var tabRect = new Rectangle(xPositionOnScreen - tabWidth + 26, currentPosY + LeftSideStartOffsetY, tabWidth, tabHeight);
				currentPosY += tabHeight + 10;

				var subPageName = pageTab.PageInfo != null ? $"{MenuTab.Name}_{pageTab.PageInfo.ID}" : $"{MenuTab.Name}_Default";
				pageTab.TabButton = new ClickableComponent(tabRect, subPageName, pageTab.Label);
			}

            if (maxLabelWidth > 0)
            {
                foreach (var pageTab in PageExtensions)
                {
                    if (pageTab.TabButton != null)
                    {
                        pageTab.TabButton.bounds.Width = maxLabelWidth;
                        pageTab.TabButton.bounds.X = xPositionOnScreen - maxLabelWidth + 26;
                    }
                }
            }

        }


		#region IClickableMenu Redirection

		private static MethodInfo actionOnRegionChangeMethod;
		private static MethodInfo cleanupBeforeExitMethod;
		private static MethodInfo customSnapBehaviorMethod;
		private static MethodInfo noSnappedComponentFoundMethod;

		private static void InitializeRedirections()
		{
			actionOnRegionChangeMethod = typeof(IClickableMenu).GetMethod("actionOnRegionChange",
				BindingFlags.Instance | BindingFlags.NonPublic);
			cleanupBeforeExitMethod = typeof(IClickableMenu).GetMethod("cleanupBeforeExit",
				BindingFlags.Instance | BindingFlags.NonPublic);
			customSnapBehaviorMethod = typeof(IClickableMenu).GetMethod("customSnapBehavior",
				BindingFlags.Instance | BindingFlags.NonPublic);
			noSnappedComponentFoundMethod = typeof(IClickableMenu).GetMethod("noSnappedComponent",
				BindingFlags.Instance | BindingFlags.NonPublic);
		}

		protected override void actionOnRegionChange(int oldRegion, int newRegion)
		{
			if (CurrentOverride != null)
				actionOnRegionChangeMethod.Invoke(CurrentOverride, new object[] { oldRegion, newRegion });
			else
				base.actionOnRegionChange(oldRegion, newRegion);
		}

		public override void applyMovementKey(int direction)
		{
			if (CurrentOverride != null)
				CurrentOverride.applyMovementKey(direction);
			else
				base.applyMovementKey(direction);
		}

		public override bool areGamePadControlsImplemented()
		{
			if (CurrentOverride != null)
				return CurrentOverride.areGamePadControlsImplemented();
			else
				return base.areGamePadControlsImplemented();
		}

		public override bool autoCenterMouseCursorForGamepad()
		{
			if (CurrentOverride != null)
				return CurrentOverride.autoCenterMouseCursorForGamepad();
			else
				return base.autoCenterMouseCursorForGamepad();
		}

		protected override void cleanupBeforeExit()
		{
			if (CurrentOverride != null)
				cleanupBeforeExitMethod.Invoke(CurrentOverride, null);
			else
				base.cleanupBeforeExit();
		}

		public override void clickAway()
		{
			if (CurrentOverride != null)
				CurrentOverride.clickAway();
			else
				base.clickAway();
		}

		protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
		{
			if (CurrentOverride != null)
				customSnapBehaviorMethod.Invoke(CurrentOverride, new object[] { direction, oldRegion, oldID });
			else
				base.customSnapBehavior(direction, oldRegion, oldID);
		}

		public override void drawBackground(SpriteBatch b)
		{
			if (CurrentOverride != null)
				CurrentOverride.drawBackground(b);
			else
				base.drawBackground(b);
		}

		public override void emergencyShutDown()
		{
			if (CurrentOverride != null)
				CurrentOverride.emergencyShutDown();
			else
				base.emergencyShutDown();
		}

		public override void gamePadButtonHeld(Buttons b)
		{
			if (CurrentOverride != null)
				CurrentOverride.gamePadButtonHeld(b);
			else
				base.gamePadButtonHeld(b);
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			if (CurrentOverride != null)
				CurrentOverride.gameWindowSizeChanged(oldBounds, newBounds);
			else
				base.gameWindowSizeChanged(oldBounds, newBounds);
		}

		public override ClickableComponent getCurrentlySnappedComponent()
		{
			if (CurrentOverride != null)
				return CurrentOverride.getCurrentlySnappedComponent();
			else
				return base.getCurrentlySnappedComponent();
		}

		public override bool isWithinBounds(int x, int y)
		{
			if (CurrentOverride != null)
				return CurrentOverride.isWithinBounds(x, y);
			else
				return base.isWithinBounds(x, y);
		}

		public override void leftClickHeld(int x, int y)
		{
			if (CurrentOverride != null)
				CurrentOverride.leftClickHeld(x, y);
			else
				base.leftClickHeld(x, y);
		}

		protected override void noSnappedComponentFound(int direction, int oldRegion, int oldID)
		{
            if (CurrentOverride != null)
                noSnappedComponentFoundMethod.Invoke(CurrentOverride, new object[] { direction, oldRegion, oldID });
            else
                base.noSnappedComponentFound(direction, oldRegion, oldID);
		}

		public override bool overrideSnappyMenuCursorMovementBan()
		{
			if (CurrentOverride != null)
				return CurrentOverride.overrideSnappyMenuCursorMovementBan();
			else
				return base.overrideSnappyMenuCursorMovementBan();
		}

		public override void performHoverAction(int x, int y)
		{
			if (CurrentOverride != null)
				CurrentOverride.performHoverAction(x, y);
			else
				base.performHoverAction(x, y);
		}

		public override bool readyToClose()
		{
			if (CurrentOverride != null)
				return CurrentOverride.readyToClose();
			else
				return base.readyToClose();
		}

		public override void receiveGamePadButton(Buttons b)
		{
			if (CurrentOverride != null)
				CurrentOverride.receiveGamePadButton(b);
			else
				base.receiveGamePadButton(b);
		}

		public override void receiveKeyPress(Keys key)
		{
			if (CurrentOverride != null)
				CurrentOverride.receiveKeyPress(key);
			else
				base.receiveKeyPress(key);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (HandleReceiveLeftClick(x, y, playSound))
				return;

			if (CurrentOverride != null)
				CurrentOverride.receiveLeftClick(x, y, playSound);
			else
				base.receiveLeftClick(x, y, playSound);
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			if (CurrentOverride != null)
				CurrentOverride.receiveRightClick(x, y, playSound);
			else
				base.receiveRightClick(x, y, playSound);
		}

		public override void receiveScrollWheelAction(int direction)
		{
			if (CurrentOverride != null)
				CurrentOverride.receiveScrollWheelAction(direction);
			else
				base.receiveScrollWheelAction(direction);
		}

		public override void releaseLeftClick(int x, int y)
		{
			if (CurrentOverride != null)
				CurrentOverride.releaseLeftClick(x, y);
			else
				base.releaseLeftClick(x, y);
		}

		public override void setCurrentlySnappedComponentTo(int id)
		{
			if (CurrentOverride != null)
				CurrentOverride.setCurrentlySnappedComponentTo(id);
			else
				base.setCurrentlySnappedComponentTo(id);
		}

		public override void setUpForGamePadMode()
		{
			if (CurrentOverride != null)
				CurrentOverride.setUpForGamePadMode();
			else
				base.setUpForGamePadMode();
		}

		public override bool showWithoutTransparencyIfOptionIsSet()
		{
			if (CurrentOverride != null)
				return CurrentOverride.showWithoutTransparencyIfOptionIsSet();
			else
				return base.showWithoutTransparencyIfOptionIsSet();
		}

		public override void snapCursorToCurrentSnappedComponent()
		{
			if (CurrentOverride != null)
				CurrentOverride.snapCursorToCurrentSnappedComponent();
			else
				base.snapCursorToCurrentSnappedComponent();
		}

		public override void snapToDefaultClickableComponent()
		{
			if (CurrentOverride != null)
				CurrentOverride.snapToDefaultClickableComponent();
			else
				base.snapToDefaultClickableComponent();
		}

		public override void update(GameTime time)
		{
			if (CurrentOverride != null)
				CurrentOverride.update(time);
			else
				base.update(time);
		}

		#endregion

		#region Custom tabs handling

		public class PageExtensionTab
		{
			public int Index { get; set; }

			public string Label { get; set; }

			public IClickableMenu Page { get; set; }

			public ClickableComponent TabButton { get; set; }

			public MenuPageEntry PageInfo { get; set; }

			public bool Visible => PageInfo?.Visible ?? true;

			public bool Enabled => PageInfo?.Enabled ?? true;

			public Rectangle Bounds => TabButton?.bounds ?? new Rectangle();

			//public PageExtensionTab(IClickableMenu page)
			//{
			//	Page = page;
			//	Label = "default";
			//}
		}

		public class CustomMenuTab
		{
			public int Index { get; set; }

			public string Label { get; set; }

			public GameMenuPageExtender Page { get; set; }

			public ClickableComponent TabButton { get; set; }

			public MenuPageEntry TabInfo { get; set; }

			public bool Visible => TabInfo?.Visible ?? true;

			public bool Enabled => TabInfo?.Enabled ?? true;

			public Rectangle Bounds => TabButton?.bounds ?? new Rectangle();
		}

        public override void draw(SpriteBatch b)
        {
            if (PageExtensions.Count(p => p.Visible) > 1)
                DrawPageExtensionTabs(b);

            if (CurrentOverride != null)
                CurrentOverride.draw(b);
            else
                base.draw(b);
        }

        private void DrawPageExtensionTabs(SpriteBatch b)
		{
            foreach (var pageTab in PageExtensions)
			{
                if (pageTab.Visible && pageTab.TabButton != null)
					DrawPageExtensionTab(b, pageTab);
            }
        }

		private void DrawPageExtensionTab(SpriteBatch b, PageExtensionTab pageTab)
		{
            bool isCurrent = (pageTab == CurrentPage);

            var buttonColor = isCurrent ? Color.White : Color.LightGray;

            var clipRect = new Rectangle(0, pageTab.Bounds.Y - 10, xPositionOnScreen + 16, pageTab.Bounds.Height + 20);

            if (isCurrent)
            {
                clipRect.Width += 8;
            }

            using (new GraphicClip(b, clipRect))
            {
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                pageTab.Bounds.X, pageTab.Bounds.Y, pageTab.Bounds.Width + 30, pageTab.Bounds.Height, buttonColor, 1f, isCurrent);

                b.DrawString(Game1.smallFont, pageTab.Label,
                        new Vector2(
                            pageTab.Bounds.X + (TabPaddingX / 2),
                            pageTab.Bounds.Y + (TabPaddingY / 2) + 3),
                        Game1.textColor);
            }
		}

		private bool HandleReceiveLeftClick(int x, int y, bool playSound)
		{
            if (PageExtensions.Count(p => p.Visible) == 1)
                return false;

            foreach (var pageTab in PageExtensions)
			{
				if (pageTab.Visible && pageTab.Enabled && pageTab.TabButton != null && pageTab.TabButton.containsPoint(x, y))
				{
					currentPageIndex = pageTab.Index;
                    Game1.playSound("smallSelect");
                    return true;
				}
			}
			return false;
		}

		#endregion
	}
}
