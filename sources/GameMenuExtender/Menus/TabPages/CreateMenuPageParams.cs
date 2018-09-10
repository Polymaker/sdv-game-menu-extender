using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Menus
{
	public struct CreateMenuPageParams
	{
		public int X;
		public int Y;
		public int Width;
		public int Height;
		public bool UpperRightCloseButton;

		public CreateMenuPageParams(int x, int y, int width, int height, bool upperRightCloseButton)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			UpperRightCloseButton = upperRightCloseButton;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CreateMenuPageParams))
				return false;
			var other = (CreateMenuPageParams)obj;
			return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height && UpperRightCloseButton == other.UpperRightCloseButton;
		}

		public static bool operator == (CreateMenuPageParams a, CreateMenuPageParams b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(CreateMenuPageParams a, CreateMenuPageParams b)
		{
			return !a.Equals(b);
		}

		public static CreateMenuPageParams operator -(CreateMenuPageParams a, CreateMenuPageParams b)
		{
			return new CreateMenuPageParams(a.X - b.X, a.Y - b.Y, a.Width - b.Width, a.Height - b.Height, a.UpperRightCloseButton);
		}

		public static CreateMenuPageParams operator +(CreateMenuPageParams a, CreateMenuPageParams b)
		{
			return new CreateMenuPageParams(a.X + b.X, a.Y + b.Y, a.Width + b.Width, a.Height + b.Height, a.UpperRightCloseButton);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
