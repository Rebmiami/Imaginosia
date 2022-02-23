using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Graphics
{
	public static class TextPrinter
	{
		public static SlicedSprite characterSheet;

		static TextPrinter()
		{
			characterSheet = SlicedSprite.Frameify(Game1.getGame.Content.Load<Texture2D>("SpriteFont"), 15, 5);
			Rectangle[] newFrame = new Rectangle[characterSheet.frames.Length];
			for (int i = 0; i < characterSheet.frames.Length; i++)
			{
				newFrame[i] = characterSheet.frames[(i / 16) + (i % 16 * 6)];
			}

			/* 
			Dimensions of characters for kerning
			 *-1 = 0x0 (character does not exist)
			 * 0 = 3x5  (most letters, all numbers, some symbols)
			 * 1 = 2x5  (some letters, some symbols)
			 * 2 = 1x5  (some symbols, most punctuation)
			 * 3 = 3x8 (some letters)
			 * 4 = 5x5  (some letters)
			*/
			int[] types = new int[]
			{
				2, 2, 0, 4, 4, 4, 4, 2, 1, 1, 0, 0, 2, 0, 2, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 0,
				4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 1, 0, 1, 0, 0,
				-1, 0, 0, 0, 0, 0, 0, 3, 0, 2, 1, 0, 2, 4, 0, 0,
				3, 3, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 2, 0, 4, -1
			};

			for (int i = 0; i < characterSheet.frames.Length; i++)
			{
				newFrame[i].Location += new Point(2);
				switch (types[i])
				{
					case 0:
						newFrame[i].Size = new Point(3, 5);
						break;
					case 1:
						newFrame[i].Size = new Point(2, 5);
						break;
					case 2:
						newFrame[i].Size = new Point(1, 5);
						break;
					case 3:
						newFrame[i].Size = new Point(3, 8);
						break;
					case 4:
						newFrame[i].Size = new Point(5, 5);
						break;
					default:
						newFrame[i].Size = new Point(0);
						break;
				}
				newFrame[i].Y -= 1;
				newFrame[i].Size = (newFrame[i].Size.ToVector2() + new Vector2(1, 1)).ToPoint();
			}
			characterSheet.frames = newFrame;
		}

		public static void Print(string text, Vector2 position, SpriteBatcher spriteBatcher, bool center = false, bool background = false)
		{
			position.Floor();

			Rectangle bounds = new Rectangle(position.ToPoint(), new Point(0, 8));
			bounds.Y--;

			int xOffset = 0;
			if (center)
			{
				foreach (char character in text)
				{
					xOffset -= characterSheet.frames[character - 32].Width;
				}
				xOffset /= 2;
			}
			foreach (char character in text)
			{
				bounds.X = (int)(position.X + xOffset) - 1;
				bounds.Width = characterSheet.frames[character - 32].Width + 1;
				if (background)
				{
					spriteBatcher.Draw(Assets.Tex["special"], bounds, Color.Black);
				}

				spriteBatcher.Draw(characterSheet.texture, position + new Vector2(xOffset, 0), characterSheet.frames[character - ' '], Color.White);
				xOffset += characterSheet.frames[character - 32].Width;
			}
			bounds.Width = xOffset;
			// TODO: Option to center text
		}
	}
}
