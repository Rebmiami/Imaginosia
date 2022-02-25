using Imaginosia.Gameplay;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Graphics
{
	public static class DustManager
	{
		public static List<Dust> dusts;

		static DustManager()
		{
			dusts = new List<Dust>();
		}

		public static Dust NewDust(Vector2 position, Vector2 velocity, float randomVel, int type)
		{
			Dust dust = new Dust(position, velocity, randomVel, type);
			dusts.Add(dust);
			return dust;
		}

		public static void CreateDustPuff(FloatRectangle rectangle, Vector2 velocity, float randomVel, int type, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Vector2 position = new Vector2((float)RNG.rand.NextDouble() * rectangle.Width + rectangle.X, (float)RNG.rand.NextDouble() * rectangle.Height + rectangle.Y);

				NewDust(position, velocity, randomVel, type);
			}
		}

		public static void Update()
		{
			foreach (Dust dust in dusts)
			{
				dust.Update();
			}

			for (int i = 0; i < dusts.Count;)
			{
				if (dusts[i].timeLeft <= 0)
				{
					dusts.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		public static void Draw(SpriteBatcher spriteBatcher, int layer)
		{
			foreach (Dust dust in dusts)
			{
				if (dust.layer == layer)
				dust.Draw(spriteBatcher);
			}
		}
	}
}
