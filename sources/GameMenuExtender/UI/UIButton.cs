using GameMenuExtender.Data;
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
	public class UIButton : UIControl
	{
		public Icon Image { get; set; }

		public UIButton() { }

		public UIButton(int x, int y, int w, int h) : base(x, y, w, h)
		{
			
		}

		public override void draw(SpriteBatch b)
		{
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), (int)ScreenLocation.X, (int)ScreenLocation.Y, Width, Height, Color.White, 4f, true);

			if (!string.IsNullOrEmpty(Text))
			{
				Utility.drawTextWithShadow(b, Text, Font, ScreenLocation, ForeColor, 1f, 1f, -1, -1, 0f, 3);
			}
			else if(Image != null)
			{
				var aspectRatio = (float)Image.SourceRect.Width / (float)Image.SourceRect.Width;
				var scale = (float)(Width - 8) / Image.SourceRect.Width;
				var imgRect = new Rectangle(0, 0, (int)(Image.SourceRect.Width * scale), (int)(Image.SourceRect.Height * scale));
				imgRect.X = ScreenBounds.X + (Width - imgRect.Width) / 2;
				imgRect.Y = ScreenBounds.Y + (Height - imgRect.Height) / 2;
				b.Draw(Image.Texture, new Vector2(imgRect.X, imgRect.Y), new Rectangle?(Image.SourceRect),
					Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
			}
		}

		protected override void OnTextChanged()
		{
			var textSize = CalculateTextSize();

			Size = new Microsoft.Xna.Framework.Vector2(textSize.X + 64, 80);
		}

		protected override void OnLeftClick(int x, int y)
		{
			
		}
	}
}
