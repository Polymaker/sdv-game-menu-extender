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

namespace GameMenuExtender.Menus
{
	internal sealed class GameMenuPageExtender : IClickableMenu
	{
		public VanillaTab MenuTab { get; private set; }

		public GameMenuManager Manager => MenuTab.Manager;

		public IClickableMenu CurrentOverride => Manager.CurrentTabPage?.PageWindow;

		private const int TabPaddingX = 32;
		private const int TabPaddingY = 24;
        private const int LeftSideStartOffsetY = 128;
        private const int LeftSideTabOffsetX = 24;

		public GameMenuPageExtender(VanillaTab tab)
		{
			MenuTab = tab;
		}

		public void Initialize(IClickableMenu sourePage)
		{
			initialize(sourePage.xPositionOnScreen, sourePage.yPositionOnScreen,
				sourePage.width, sourePage.height, sourePage.upperRightCloseButton != null);
		}

		//public void Initialize(CreateMenuPageParams windowParams)
		//{
		//	initialize(windowParams.X, windowParams.Y, windowParams.Width, windowParams.Height, windowParams.UpperRightCloseButton);
		//}

		internal void RebuildTabPageButtons(GameMenuTab tab)
		{
			int currentPosY = yPositionOnScreen + LeftSideStartOffsetY;
            int maxLabelWidth = 0;

			foreach(var tabPage in tab.TabPages)
			{
				if (tabPage.Visible)
				{
					var labelSize = Game1.smallFont.MeasureString(tabPage.Label);
					int tabWidth = (int)labelSize.X + TabPaddingX;
					int tabHeight = (int)labelSize.Y + 30;
					if (tabWidth > maxLabelWidth)
						maxLabelWidth = tabWidth;
					var tabRect = new Rectangle(xPositionOnScreen - tabWidth + LeftSideTabOffsetX, currentPosY, tabWidth, tabHeight);
					currentPosY += tabHeight + 8;
					
					tabPage.TabPageButton = new ClickableComponent(tabRect, tabPage.UniqueID, tabPage.Label);
				}
				else
				{
                    tabPage.TabPageButton = null;
				}
			}

            if (maxLabelWidth > 0)
            {
				foreach (var tabPage in tab.TabPages)
				{
					if(tabPage.TabPageButton != null)
					{
						tabPage.TabPageButton.bounds.Width = maxLabelWidth;
						tabPage.TabPageButton.bounds.X = xPositionOnScreen - maxLabelWidth + LeftSideTabOffsetX;
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

        public override void draw(SpriteBatch b)
        {
			//Draw custom tabs (at the top)
			Manager.DrawCustomTabs(b);

			//Draw sub-pages tab buttons
			DrawPagesTabButtons(b);

			//Draw the page
			if (CurrentOverride != null)
				CurrentOverride.draw(b);

			if (!string.IsNullOrWhiteSpace(HoverText))
                IClickableMenu.drawHoverText(b, HoverText, Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
        }

        private string HoverText;

        public override void performHoverAction(int x, int y)
        {
            HoverText = string.Empty;

            foreach (var tab in Manager.CustomTabs.Where(t => t.Visible && t.TabButton != null && !t.DrawText))
            {
                if (tab.TabButton.containsPoint(x, y))
                {
                    HoverText = tab.Label;
                    break;
                }
            }

            if (CurrentOverride != null)
                CurrentOverride.performHoverAction(x, y);
            else
                base.performHoverAction(x, y);
        }

        private void DrawPagesTabButtons(SpriteBatch b)
		{
			if (Manager.CurrentTab == null || Manager.CurrentTab.TabPages.Count(p => p.Visible) == 1)
				return;

			foreach (var tabPage in Manager.CurrentTab.TabPages)
			{
				if (tabPage.Visible && tabPage.TabPageButton != null)
					DrawPageExtensionTab(b, tabPage);
			}
		}

		private void DrawPageExtensionTab(SpriteBatch b, GameMenuTabPage pageTab)
		{
            bool isCurrent = pageTab.IsSelected;

            var buttonColor = pageTab.IsSelected ? Color.White : Color.LightGray;
			var buttonBounds = pageTab.TabPageButton.bounds;
            var clipRect = new Rectangle(0, buttonBounds.Y - 10, xPositionOnScreen + 16, buttonBounds.Height + 20);

            if (isCurrent)
            {
                clipRect.Width += 8;
            }

            using (new GraphicClip(b, clipRect))
            {
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
				    buttonBounds.X, buttonBounds.Y, buttonBounds.Width + 16, buttonBounds.Height, buttonColor, 1f, pageTab.IsSelected);

                Utility.drawTextWithShadow(b, pageTab.Label, Game1.smallFont,
                        new Vector2(
                            buttonBounds.X + (TabPaddingX / 2),
                            buttonBounds.Y + 18),
                        Game1.textColor);
            }
		}

		private bool HandleReceiveLeftClick(int x, int y, bool playSound)
		{
            foreach (var tab in Manager.AllTabs)
            {
                if (tab.Visible && tab.TabButton != null && tab.TabButton.containsPoint(x, y))
                {
					if ((tab.IsCustom && !tab.IsSelected) || (Manager.CurrentTab.IsCustom && !tab.IsCustom))
					{
						Manager.ChangeTab(tab);
						Game1.playSound("smallSelect");
						return true;
					}
				}
            }

			if (Manager.CurrentTab == null || Manager.CurrentTab.TabPages.Count(p => p.Visible) == 1)
				return false;

			foreach (var tabPage in Manager.CurrentTab.TabPages)
			{
				if (tabPage.Visible && tabPage.TabPageButton != null && tabPage.TabPageButton.containsPoint(x, y))
				{
                    if (!tabPage.IsSelected)
                    {
                        Manager.CurrentTab.SelectTabPage(tabPage);
                        Game1.playSound("smallSelect");
                        return true;
                    }
				}
			}
			return false;
		}

		#endregion
	}
}
