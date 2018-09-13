using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.UI
{
	public class UIControl : IClickableMenu
	{
		protected string _Text;

		public string Text
		{
			get { return _Text; }
			set
			{
				if(value != _Text)
				{
					_Text = value;
					OnTextChanged();
				}
			}
		}

		public Vector2 ScrollOffset { get; set; }

		public Vector2 ScreenLocation => new Vector2(Bounds.X + ScrollOffset.X, Bounds.Y + ScrollOffset.Y);

		public Rectangle Bounds
		{
			get { return new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height); }
			set
			{
				initialize(value.X, value.Y, value.Width, value.Height);
				OnBoundsChanged();
			}
		}

		public Vector2 Size
		{
			get => new Vector2(width, height);
			set => Bounds = new Rectangle(Bounds.X, Bounds.Y, (int)value.X, (int)value.Y);
		}

		public int X
		{
			get => Bounds.X;
			set => Bounds = new Rectangle(value, Bounds.Y, Bounds.Width, Bounds.Height);
		}

		public int Y
		{
			get => Bounds.Y;
			set => Bounds = new Rectangle(Bounds.X, value, Bounds.Width, Bounds.Height);
		}

		public UIControl()
		{

		}

		public UIControl(int x, int y, int w, int h) : base(x, y, w, h)
		{

		}

		protected virtual void OnTextChanged() { }

		protected virtual void OnBoundsChanged()
		{

		}
	}
}
