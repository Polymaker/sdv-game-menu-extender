using GameMenuExtender.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.UI
{
    class MenuExtenderConfigPage : IClickableMenu
    {
        private ClickableTextureComponent upArrow;
        private ClickableTextureComponent downArrow;
        private ClickableTextureComponent scrollBar;
        private Rectangle scrollBarRunner;
		private bool scrolling;
		private string hoverText;
		private int currentItemIndex;
		private int ItemHeight;
		private List<UIControl> ConfigControls;

		public MenuExtenderConfigPage(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {
            upArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            downArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
            scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 12, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, height - 128 - upArrow.bounds.Height - 8);
			ItemHeight = (height - 128) / 7;
			ConfigControls = new List<UIControl>();
		}

		internal void LoadConfigs(GameMenuManager manager)
		{
			ConfigControls.Clear();
			var currentY = yPositionOnScreen + 112 + ItemHeight;

			foreach (var tab in manager.AllTabs)
			{
				var tabLabel = new UILabel(Game1.smallFont, "Tab: " + tab.Label);
				tabLabel.X = xPositionOnScreen + 48;
				tabLabel.Y = currentY;
				ConfigControls.Add(tabLabel);
				currentY += tabLabel.height + 3;

				foreach(var page in tab.TabPages)
				{
					var pageLabel = new UILabel(Game1.smallFont, "Page : " + page.Label);
					pageLabel.X = xPositionOnScreen + 48 + 30;
					pageLabel.Y = currentY;
					ConfigControls.Add(pageLabel);
					currentY += pageLabel.height + 3;
				}
			}
		}

        public override void draw(SpriteBatch b)
        {
			b.End();
			b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
			SpriteText.drawString(b, "Menu Extender Settings:", xPositionOnScreen + 48, yPositionOnScreen + 112, 999, -1, 999, 1f, 0.1f);

			foreach(var option in ConfigControls)
			{
				option.ScrollOffset = new Vector2(0, currentItemIndex * ItemHeight * -1);
				option.draw(b);
			}

            //Utility.drawTextWithShadow(b, "Menu Extender Settings", Game1.dialogueFont, new Vector2(xPositionOnScreen + 45, yPositionOnScreen + 120), Game1.textColor);

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            if (!GameMenu.forcePreventClose)
            {
                upArrow.draw(b);
                downArrow.draw(b);
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f, drawShadow: false);
                scrollBar.draw(b);
            }
        }

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (!GameMenu.forcePreventClose)
			{
				if (downArrow.containsPoint(x, y) && currentItemIndex < Math.Max(0, ConfigControls.Count - 7))
				{
					downArrowPressed();
					Game1.playSound("shwip");
				}
				else if (upArrow.containsPoint(x, y) && currentItemIndex > 0)
				{
					upArrowPressed();
					Game1.playSound("shwip");
				}
				else if (scrollBar.containsPoint(x, y))
				{
					scrolling = true;
				}
				else if (!downArrow.containsPoint(x, y) && x > xPositionOnScreen + width && x < xPositionOnScreen + width + 128 && y > yPositionOnScreen && y < yPositionOnScreen + height)
				{
					scrolling = true;
					leftClickHeld(x, y);
					releaseLeftClick(x, y);
				}
				//currentItemIndex = Math.Max(0, Math.Min(ConfigControls.Count - 7, currentItemIndex));
				//int i = 0;
				//while (true)
				//{
				//	if (i < optionSlots.Count)
				//	{
				//		if (optionSlots[i].bounds.Contains(x, y) && currentItemIndex + i < options.Count && options[currentItemIndex + i].bounds.Contains(x - optionSlots[i].bounds.X, y - optionSlots[i].bounds.Y))
				//		{
				//			break;
				//		}
				//		i++;
				//		continue;
				//	}
				//	return;
				//}
				//options[currentItemIndex + i].receiveLeftClick(x - optionSlots[i].bounds.X, y - optionSlots[i].bounds.Y);
				//optionsSlotHeld = i;
			}
		}

		private void downArrowPressed()
		{
			downArrow.scale = downArrow.baseScale;
			currentItemIndex++;
			setScrollBarToCurrentIndex();
		}

		private void upArrowPressed()
		{
			upArrow.scale = upArrow.baseScale;
			currentItemIndex--;
			setScrollBarToCurrentIndex();
		}

		private void setScrollBarToCurrentIndex()
		{
			if (ConfigControls.Count > 0)
			{
				scrollBar.bounds.Y = scrollBarRunner.Height / Math.Max(1, ConfigControls.Count - 7 + 1) * currentItemIndex + upArrow.bounds.Bottom + 4;
				if (currentItemIndex == ConfigControls.Count - 7)
				{
					scrollBar.bounds.Y = downArrow.bounds.Y - scrollBar.bounds.Height - 4;
				}
			}
		}

	}
}
