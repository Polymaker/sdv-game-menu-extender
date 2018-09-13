using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.UI
{
	public class UIScrollBar : UIControl
	{
		private ClickableTextureComponent upArrow;
		private ClickableTextureComponent downArrow;
		private ClickableTextureComponent scrollBar;
		private Rectangle scrollBarRunner;
		private bool scrolling;

		public UIScrollBar(int x, int y, int w, int h) : base(x, y, w, h)
		{

			
		}

		protected override void OnBoundsChanged()
		{
			InitializeElements();
		}

		private void InitializeElements()
		{
			/*
			upArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
			downArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
			scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 12, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
			scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, height - 128 - upArrow.bounds.Height - 8);
			*/
		}
	}
}
