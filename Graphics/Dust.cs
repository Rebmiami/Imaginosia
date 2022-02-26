using Imaginosia.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Imaginosia.Graphics
{
	public class Dust
	{
		public Vector2 position;
		public Vector2 velocity;
		public int type;
		public Color color;

		public float opacity;
		public float rotation;
		public float scale;

		public int timeLeft;
		public int maxTime;
		public int layer;

		public bool noRotate = false;
		public bool noGravity = false;
		public bool noFade = false;
		public bool gameLayer = false;

		public Dust(Vector2 position, Vector2 velocity, float randomVel, int type)
		{
			this.position = position;
			this.velocity = velocity;
			this.velocity += RNG.RotateRandom(new Vector2(randomVel / 2, 0), 360, randomVel);
			this.type = type;

			color = Color.White;
			opacity = 1;
			scale = 1;

			switch (type)
			{
				case 0:
					gameLayer = true;
					maxTime = 20;
					noFade = true;
					break;
				case 1:
				case 2:
					gameLayer = true;
					color = new Color(255, 255, 255, 0);
					noGravity = true;
					maxTime = 20;
					break;
				case 3:
					gameLayer = true;
					color = new Color(255, 255, 255, 0);
					noGravity = true;
					maxTime = 60;
					noFade = true;
					break;
				case 4:
					gameLayer = true;
					maxTime = 180;
					noGravity = true;
					scale = 2;
					break;
				case 5:
					maxTime = 40;
					noFade = true;
					layer = 1;
					noGravity = true;
					break;
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
					gameLayer = true;
					maxTime = 20;
					noFade = true;
					break;
				default:
					break;
			}

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
				velocity.Y += 0.01f;
			}
			if (!noRotate)
			{
				rotation += velocity.X / 10f;
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

			Vector2 drawPosition = position;

			if (gameLayer)
			{
				drawPosition = PositionHelper.ToScreenPosition(position);
			}


			spriteBatcher.Draw(Assets.Tex2["dust"].texture, drawPosition, Assets.Tex2["dust"].frames[type + (ImaginationHandler.IsImagination ? 12 : 0)], color, rotation, new Vector2(4), effectiveScale, SpriteEffects.None, 0);
		}
	}
}