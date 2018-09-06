using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    }
}
