using Imaginosia.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public static class ImaginationHandler
	{
		public static bool IsImagination { get; private set; }
		public static bool Next { get; set; }

		public static int TimeSinceSwitched { get; set; }

		public static int TransitionTimer;

		static ImaginationHandler()
		{
			IsImagination = false;
		}

		public static void EnterImagination()
		{
			IsImagination = true;
			ImaginationChanged();

			// Revealing all enemies at the start helps with the steady increase in difficulty
			foreach (var item in Game1.gamestate.enemies)
			{
				item.spotted = true;
			}
			Assets.Sfx["imaginationJingle"].Play();
		}

		public static void LeaveImagination()
		{
			IsImagination = false;
			ImaginationChanged();
			Assets.Sfx["realWorldJingle"].Play();
		}

		private static void ImaginationChanged()
		{
			DustManager.CreateDustPuff(new FloatRectangle(0, 0, Game1.GameWidth, Game1.GameHeight), Microsoft.Xna.Framework.Vector2.Zero, 1, 5, 1000);
			TimeSinceSwitched = 0;
			Game1.ScreenShake = 8;
		}

		public static void StartTransition()
		{
			Assets.Sfx["imaginationPrelude"].Play();
			TransitionTimer = 60;
		}

		public static void SwitchImagination()
		{
			if (Next)
			{
				EnterImagination();
			}
			else
			{
				LeaveImagination();
			}
		}

		public static void Reset()
		{
			IsImagination = false;
			TimeSinceSwitched = 0;
		}
	}
}
