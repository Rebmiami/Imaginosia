using Imaginosia.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia
{
	public static class Assets
	{
		public static Dictionary<string, Texture2D> Tex = new Dictionary<string, Texture2D>();
		public static Dictionary<string, SlicedSprite> Tex2 = new Dictionary<string, SlicedSprite>();
		public static Dictionary<string, SoundEffect> Sfx = new Dictionary<string, SoundEffect>();
		public static Effect Darkness;
	}
}
