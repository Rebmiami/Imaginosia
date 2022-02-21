using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Graphics
{
	public class SlicedSprite
	{
		public Texture2D texture;
		public Rectangle[] frames;

		public static SlicedSprite Frameify(Texture2D texture, int xCuts, int yCuts)
		/* X cuts are vertical, Y cuts are horizontal - do not confuse them
		Number of cuts you should use is frames - 1
		Frame numbers behave as follows:

		   X X
		  1|4|7
		Y -+-+-
		  2|5|8
		Y -+-+-
		  3|6|9

		Do not use sprites that cannot be perfectly divided

		*/
		{
			Rectangle bounds = texture.Bounds;
			Rectangle[] frames = new Rectangle[(xCuts + 1) * (yCuts + 1)];
			Vector2 cutSize = new Vector2(bounds.Size.X / (xCuts + 1), bounds.Size.Y / (yCuts + 1));
			for (int i = 0; i < (xCuts + 1); i++)
			{
				for (int j = 0; j < (yCuts + 1); j++)
				{
					frames[i * (yCuts + 1) + j] = new Rectangle((int)cutSize.X * i, (int)cutSize.Y * j, (int)cutSize.X, (int)cutSize.Y);
				}
			}
			return new SlicedSprite(texture, frames);
		}

		public SlicedSprite(Texture2D texture, Rectangle[] frames)
		{
			this.texture = texture;
			this.frames = frames;
		}
	}
}
