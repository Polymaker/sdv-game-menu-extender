using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Data
{
    public class Icon
    {
        public Texture2D Texture { get; private set; }
        public Rectangle SourceRect { get; private set; }

        public Icon(Texture2D texture, Rectangle sourceRect)
        {
            Texture = texture;
            SourceRect = sourceRect;
        }

		public static Icon UpArrow => new Icon(Game1.mouseCursors, new Rectangle(421, 459, 11, 12));

		public static Icon DownArrow => new Icon(Game1.mouseCursors, new Rectangle(421, 472, 11, 12));


	}
}
