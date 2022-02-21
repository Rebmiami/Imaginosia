using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Graphics
{
	public static class PositionHelper
	{
		public const int TileWidth = 16;
		public const int TileHeight = 8;

		public static Vector2 CameraPos;

		public static Vector2 ToScreenPosition(Vector2 vector, bool noCamera = false)
		{
			return (vector - (noCamera ? Vector2.Zero : CameraPos)) * new Vector2(TileWidth, TileHeight);
		}

		public static Vector2 ToGamePosition(Vector2 vector, bool noCamera = false)
		{
			return (vector + (noCamera ? Vector2.Zero : CameraPos)) / new Vector2(TileWidth, TileHeight);
		}
	}
}
