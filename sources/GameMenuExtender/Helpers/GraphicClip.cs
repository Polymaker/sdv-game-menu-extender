using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMenuExtender.Helpers
{
    public class GraphicClip : IDisposable
    {
        private Rectangle OriginalClipRect;
        private SpriteBatch SB;

        public GraphicClip(SpriteBatch b, Rectangle clipRect)
        {
            OriginalClipRect = b.GraphicsDevice.ScissorRectangle;
            SB = b;
            b.End();
            b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState
            {
                ScissorTestEnable = true
            });

            clipRect.X = Math.Max(clipRect.X, 0);
            clipRect.Y = Math.Max(clipRect.Y, 0);

            clipRect.Height = Math.Min(clipRect.Bottom, b.GraphicsDevice.Viewport.Height) - clipRect.Y;
            clipRect.Width = Math.Min(clipRect.Right, b.GraphicsDevice.Viewport.Width) - clipRect.X;
            

            b.GraphicsDevice.ScissorRectangle = clipRect;
        }

        public void Dispose()
        {
            SB.GraphicsDevice.ScissorRectangle = OriginalClipRect;
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }
    }
}
