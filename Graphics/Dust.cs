using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Graphics
{
	public class Dust
	{
		public Vector2 position;
		public Vector2 velocity;
		public int type;
		public int frame;
		public Color color;

		public float opacity;
		public float rotation;
		public float scale;

		public int timeLeft;
		public int maxTime;

		public bool noRotate = false;
		public bool noGravity = false;
		public bool noFade = false;
		public bool backGround = false;
		public int flash = 0;

		public Dust(Vector2 position, Vector2 velocity, float randomVel, int type)
		{
			this.position = position;
			this.velocity = velocity;
			this.velocity += RNG.RotateRandom(new Vector2(randomVel / 2, 0), 360, randomVel);
			this.type = type;
			frame = RNG.rand.Next(3);

			color = Color.White;
			opacity = 1;
			scale = 1;

			timeLeft = maxTime;
		}

		public void Update()
		{
			if (timeLeft > maxTime)
			{
				maxTime = timeLeft;
			}

			if (!noGravity)
			{
				velocity.Y += 0.1f;
			}
			if (!noRotate)
			{
				rotation += velocity.X / 10f;
			}

			if (flash > 0)
			{
				flash--;
			}

			position += velocity;

			//switch (type)
			//{
			//	case 2:
			//        {
			//			velocity = RNG.RotateRandom(velocity, 5);
			//        }
			//		break;
			//}

			timeLeft--;
		}

		public void Draw(SpriteBatcher spriteBatcher)
		{
			float effectiveScale;
			if (noFade)
			{
				effectiveScale = (float)timeLeft / maxTime;
			}
			else
			{
				effectiveScale = 1 + (float)timeLeft / maxTime / 2f;
				opacity = (float)timeLeft / maxTime;
			}
			effectiveScale *= scale;
			Effect[] effects = null;
			spriteBatcher.Draw(Assets.Tex2["dust"].texture, PositionHelper.ToScreenPosition(position), Assets.Tex2["dust"].frames[type], color, rotation, new Vector2(5), 1, SpriteEffects.None, 0);
		}
	}
}