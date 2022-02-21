using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia
{
	static class RNG
	{
		public static Random rand = new Random();

		public static Vector2 RotateRandom(Vector2 vector, float angle, float variation = 1)
		{
			vector = MathTools.RotateVector(vector, (float)MathHelper.ToRadians(angle) * (float)(rand.NextDouble() - 0.5f));
			vector *= variation * (float)(rand.NextDouble() - 0.5f) + 1f;
			return vector;
		}

		public static T WeightedRandom<T>((T, float)[] selection)
		{
			float totalWeight = 0;
			foreach ((T, float) item in selection)
            {
				totalWeight += item.Item2;
            }
			float rand = totalWeight * (float)RNG.rand.NextDouble();
			float accumWeight = 0;
			foreach ((T, float) item in selection)
			{
				if (item.Item2 + accumWeight > rand)
					return item.Item1;
				accumWeight += item.Item2;
			}
			throw new Exception("Random selection value in WeightedRandom exceeded total weight.");
		}

		public static void RotateRandom(ref Vector2 vector, float angle, float variation = 1)
		{
			vector = RotateRandom(vector, angle, variation);
		}
	}
}
