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
		//private bool _AutoSize;

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

		public SpriteFont Font { get; set; }

		public Color ForeColor { get; set; }

		public Vector2 ScrollOffset { get; set; }

		public Vector2 ScreenLocation => new Vector2(Bounds.X + ScrollOffset.X, Bounds.Y + ScrollOffset.Y);

		public Rectangle ScreenBounds => new Rectangle((int)ScreenLocation.X, (int)ScreenLocation.Y, Width, height);

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

		public int Width
		{
			get => Bounds.Width;
			set => Bounds = new Rectangle(Bounds.X, Bounds.Y, value, Bounds.Height);
		}

		public int Height
		{
			get => Bounds.Height;
			set => Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, value);
		}

		public UIControl()
		{

		}

		public UIControl(int x, int y, int w, int h) : base(x, y, w, h)
		{

		}

		protected Vector2 CalculateTextSize()
		{
			if (!string.IsNullOrEmpty(Text))
				return Font.MeasureString(Text);
			else
				return Vector2.Zero;
		}

		protected virtual void OnTextChanged() { }

		protected virtual void OnBoundsChanged()
		{

		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (Bounds.Contains(x, y))
				OnLeftClick(x, y);
		}

		protected virtual void OnLeftClick(int x, int y)
		{

		}

		//public virtual Vector2 GetAutoSize()
		//{
		//	return Vector2.One;
		//}
	}
}
