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

		public bool DrawShadow { get; set; }

		public UILabel(SpriteFont font, string text)
		{
			Font = font;
			_Text = text;
			ForeColor = Game1.textColor;
			TextSize = CalculateTextSize();
			_AutoSize = true;
			Size = TextSize;
		}

		public override void draw(SpriteBatch b)
		{
			if (DrawShadow)
				Utility.drawTextWithShadow(b, Text, Font, ScreenLocation, ForeColor);
			else
				b.DrawString(Font, Text, ScreenLocation, ForeColor);
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged();
			TextSize = CalculateTextSize();
		}

		
	}
}
