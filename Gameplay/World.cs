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

		public List<Point> fires;

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
					sprite = Math.Clamp(colors[i].R / 50, 0, 4) + RNG.rand.Next(2) * 5,
					floorObjectStyle = RNG.rand.Next(4),
				};

				if (RNG.rand.NextDouble() * 50 < world.tiles[x, y].sprite % 5)
				{
					world.tiles[x, y].floorObjectType = FloorObjectType.Tree;
					world.tiles[x, y].floorObjectHealth = 3;
				}
			}
			world.fires = new List<Point>();


			int mushrooms = 3;
			for (int i = 0; i < mushrooms; i++)
			{
				world.tiles[RNG.rand.Next(WorldWidth), RNG.rand.Next(WorldHeight)].floorObjectType = FloorObjectType.Mushroom;
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
					

					if (tile.floorObjectType != FloorObjectType.None)
					{
						int floorObjectOffset = 0;
						int floorObjectSize = 0;

						switch (tile.floorObjectType)
						{
							case FloorObjectType.None:
								break;
							case FloorObjectType.Tree:
								floorObjectOffset = 10;
								floorObjectSize = 3;
								break;
							case FloorObjectType.Campfire:
								floorObjectOffset = 39;
								floorObjectSize = 3;
								if (RNG.rand.Next(8) == 0)
								{
									tile.floorObjectStyle = RNG.rand.Next(4);
								}

								break;
							case FloorObjectType.Fence:
								floorObjectOffset = 30;
								floorObjectSize = 2;
								break;
							case FloorObjectType.BoneTrap:
								floorObjectOffset = 22;
								floorObjectSize = 2;
								break;
							case FloorObjectType.Mushroom:
								floorObjectOffset = 51;
								floorObjectSize = 2;
								break;
							default:
								break;
						}

						Rectangle box = groundTex.frames[floorObjectOffset + floorObjectSize * tile.floorObjectStyle];
						box.Height *= floorObjectSize;
						spriteBatcher.Draw(groundTex.texture, PositionHelper.ToScreenPosition(new Vector2(i, j - floorObjectSize + 1)), box, Color.White);

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

		public static bool InBounds(Point point)
		{
			return new Rectangle(0, 0, WorldHeight, WorldWidth).Contains(point);
		}

		public void PlaceItemNearest(Point point, ref Item item)
		{
			Rectangle range = new Rectangle(point - new Point(5), new Point(10));
			range = Rectangle.Intersect(range, new Rectangle(0, 0, WorldHeight, WorldWidth));

			float distance = 10000f;
			Point closest = Point.Zero;

			for (int i = range.Left; i < range.Right; i++)
			{
				for (int j = range.Top; j < range.Bottom; j++)
				{
					WorldTile tile = tiles[i, j];

					if (tile.floorItem == null && tile.floorObjectType == FloorObjectType.None && distance > Vector2.Distance(point.ToVector2(), new Vector2(i, j)))
					{
						distance = Vector2.Distance(point.ToVector2(), new Vector2(i, j));
						closest = new Point(i, j);
					}
				}
			}

			tiles[closest.X, closest.Y].PlaceItem(ref item);
		}
	}
}
