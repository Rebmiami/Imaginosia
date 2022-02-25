using Imaginosia.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public static class ImaginationHandler
	{
		public static bool IsImagination { get; private set; }

		public static int TimeSinceSwitched { get; set; }

		static ImaginationHandler()
		{
			IsImagination = false;
		}

		public static void EnterImagination()
		{
			IsImagination = true;
			ImaginationChanged();
		}

		public static void LeaveImagination()
		{
			IsImagination = false;
			ImaginationChanged();
		}

		private static void ImaginationChanged()
		{
			DustManager.CreateDustPuff(new FloatRectangle(0, 0, Game1.GameWidth, Game1.GameHeight), Microsoft.Xna.Framework.Vector2.Zero, 1, 5, 1000);
			TimeSinceSwitched = 0;
		}

		public static void SwitchImagination()
		{
			if (IsImagination)
			{
				LeaveImagination();
			}
			else
			{
				EnterImagination();
			}
		}

		public static void Reset()
		{
			IsImagination = false;
			TimeSinceSwitched = 0;
		}
	}
}
