using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
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
		//private bool scrolling;
		//public Orientation Orientation { get; private set; }

		public UIScrollBar(int x, int y/*, int w*/, int h) : base(x, y, 44, h)
		{
			InitializeElements();
		}

		protected override void OnBoundsChanged()
		{
			InitializeElements();
		}

		private void InitializeElements()
		{
			upArrow = new ClickableTextureComponent(new Rectangle(X, Y, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
			downArrow = new ClickableTextureComponent(new Rectangle(X, Y + Height - 48, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
			scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 12, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
			scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, downArrow.bounds.Y - scrollBar.bounds.Y - 4);
		}

		public override void draw(SpriteBatch b)
		{
			upArrow.draw(b);
			downArrow.draw(b);
			drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f, drawShadow: false);
			scrollBar.draw(b);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			
		}
	}
}
