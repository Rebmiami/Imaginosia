using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Audio
{
	public static class SoundSystem
	{
		public static void PlayAtPosition(Vector2 listener, Vector2 source, string soundName, bool randomPitch = true)
		{
			float distance = Vector2.Distance(listener, source);
			float pan = (float)Math.Tanh(listener.X - source.Y);

			float pitch = 0;
			if (randomPitch)
			{
				pitch += (float)(RNG.rand.NextDouble() - 0.5f) * 0.3f;
			}

			Assets.Sfx[soundName].Play(Math.Clamp(1 / distance, 0, 1), pitch, pan);
		}
	}
}
