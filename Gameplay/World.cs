using Imaginosia.Graphics;
using Imaginosia.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginosia.Gameplay
{
	public class World
	{
		public const int WorldWidth = 100;
		public const int WorldHeight = 100;

		public WorldTile[,] tiles;

		public static World GenerateNew()
		{
			World world = new World();
			world.tiles = new WorldTile[WorldWidth, WorldHeight];

			Texture2D noiseTexture = new Texture2D(Game1.getGame.GraphicsDevice, WorldWidth, WorldHeight);

			GenerateNoiseMap(WorldWidth, WorldHeight, ref noiseTexture, 32);

			Color[] colors = new Color[WorldWidth * WorldHeight];
			noiseTexture.GetData(colors);

			for (int i = 0; i < colors.Length; i++)
			{
				int x = i % WorldWidth;
				int y = i / WorldWidth;

				world.tiles[x, y] = new WorldTile
				{
					sprite = Math.Clamp(colors[i].R / 50, 0, 4) + RNG.rand.Next(2) * 5
				};
			}

			return world;
		}

		public void Draw(SpriteBatcher spriteBatcher)
		{
			SlicedSprite groundTex = Assets.Tex2[ImaginationHandler.IsImagination ? "tilesImaginary" : "tiles"];

			for (int i = 0; i < WorldWidth; i++)
			{
				for (int j = 0; j < WorldHeight; j++)
				{
					WorldTile tile = tiles[i, j];

					spriteBatcher.Draw(groundTex.texture, PositionHelper.ToScreenPosition(new Vector2(i, j)), groundTex.frames[tile.sprite], Color.White);
					if (tile.floorItem != null)
					{
						tile.floorItem.Draw(PositionHelper.ToScreenPosition(new Vector2(i, j - 1)), spriteBatcher);
					}
				}
			}
		}

		private static void GenerateNoiseMap(int width, int height, ref Texture2D noiseTexture, int octaves)
		{
			var data = new float[width * height];

			/// track min and max noise value. Used to normalize the result to the 0 to 1.0 range.
			var min = float.MaxValue;
			var max = float.MinValue;

			/// rebuild the permutation table to get a different noise pattern. 
			/// Leave this out if you want to play with changing the number of octaves while 
			/// maintaining the same overall pattern.
			Perlin.Reseed();

			var frequency = 0.5f;
			var amplitude = 1f;
			var persistence = 0.25f;

			for (var octave = 0; octave < octaves; octave++)
			{
				/// parallel loop - easy and fast.
				Parallel.For(0
					, width * height
					, (offset) =>
					{
						var i = offset % width;
						var j = offset / width;
						var noise = Perlin.Noise(i * frequency * 1f / width, j * frequency * 1f / height);
						noise = data[j * width + i] += noise * amplitude;

						min = Math.Min(min, noise);
						max = Math.Max(max, noise);

					}
				);

				frequency *= 2;
				amplitude /= 2;
			}


			if (noiseTexture != null && (noiseTexture.Width != width || noiseTexture.Height != height))
			{
				noiseTexture.Dispose();
				noiseTexture = null;
			}
			if (noiseTexture == null)
			{
				noiseTexture = new Texture2D(Game1.getGame.GraphicsDevice, width, height, false, SurfaceFormat.Color);
			}

			var colors = data.Select(
				(f) =>
				{
					var norm = (f - min) / (max - min);
					return new Color(norm, norm, norm, 1);
				}
			).ToArray();

			noiseTexture.SetData(colors);
		}
	}
}
