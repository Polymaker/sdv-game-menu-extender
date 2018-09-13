using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.UI
{
	public class UILabel : UIControl
	{
		public SpriteFont Font { get; set; }

		public Color ForeColor { get; set; }

		private Vector2 TextSize;

		private bool _AutoSize;

		public bool AutoSize
		{
			get { return _AutoSize; }
			set
			{
				if(value != _AutoSize)
				{
					_AutoSize = value;
					if (value)
						Size = TextSize;
				}
			}
		}

		public UILabel(SpriteFont font, string text)
		{
			Font = font;
			_Text = text;
			ForeColor = Game1.textColor;
			CalculateTextSize();
			_AutoSize = true;
			Size = TextSize;
		}

		public override void draw(SpriteBatch b)
		{
			b.DrawString(Font, Text, ScreenLocation, ForeColor);
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged();
			CalculateTextSize();
		}

		private void CalculateTextSize()
		{
			if (!string.IsNullOrEmpty(Text))
				TextSize = Font.MeasureString(Text);
			else
				TextSize = Vector2.Zero;
		}
	}
}
