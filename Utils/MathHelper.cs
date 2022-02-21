using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia
{
	public static class MathTools
	{
		public static Vector2 RotateVector(Vector2 v, double radians)
		{
			Vector2 result = Vector2.Zero;
			result.X = (float)(v.X * Math.Cos(radians) - v.Y * Math.Sin(radians));
			result.Y = (float)(v.X * Math.Sin(radians) + v.Y * Math.Cos(radians));
			return result;
		}
	}
}
