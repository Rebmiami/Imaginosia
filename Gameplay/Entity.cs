using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public class Entity
	{
		public Entity()
		{
		}

		public Vector2 position;
		public Vector2 velocity;
		public Vector2 dimensions;
		public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
		public float rotation;

		public Vector2 hitboxOffset;

		public Vector2 GetFoot()
		{
			return position + new Vector2(dimensions.X / 2, dimensions.Y);
		}

		public Vector2 Center
		{
			get
			{
				return Hitbox.Center;
			}
		}

		public FloatRectangle Hitbox
		{
			get
			{
				return new FloatRectangle(position + hitboxOffset, dimensions);
			}
		}

		public virtual void Update()
		{
			position += velocity;
			if (!new FloatRectangle(0, 0, World.WorldWidth, World.WorldHeight).Contains(Hitbox))
			{
				WallCollide();
			}
			if (!(this is Projectile) && velocity != Vector2.Zero)
			FenceCollide();
		}

		public virtual void WallCollide()
		{
			FloatRectangle windowBox = new FloatRectangle(0, 0, World.WorldWidth, World.WorldHeight);

			if (Hitbox.Left < windowBox.Left)
			{
				position.X = 0 - hitboxOffset.X;
				velocity.X = 0;
			}
			else if (Hitbox.Right > windowBox.Right)
			{
				position.X = World.WorldWidth - dimensions.X - hitboxOffset.X;
				velocity.X = 0;
			}

			if (Hitbox.Top < windowBox.Top)
			{
				position.Y = 0 - hitboxOffset.Y;
				velocity.Y = 0;
			}
			else if (Hitbox.Bottom > windowBox.Bottom)
			{
				position.Y = World.WorldHeight - dimensions.Y - hitboxOffset.Y;
				velocity.Y = 0;
			}
		}

		public void FenceCollide()
		{
			Point corner1 = new Vector2(Hitbox.Left + 0.1f, Hitbox.Top + 0.1f).ToPoint();
			Point corner4 = new Vector2(Hitbox.Right - 0.1f, Hitbox.Bottom - 0.1f).ToPoint();

			Rectangle possibleTouching = new Rectangle(corner1, corner4 - corner1);

			Vector2 displacement = Vector2.Zero;

			for (int i = possibleTouching.Left; i <= possibleTouching.Right; i++)
			{
				for (int j = possibleTouching.Top; j <= possibleTouching.Bottom; j++)
				{
					if (World.InBounds(new Point(i, j)) && Game1.gamestate.world.tiles[i, j].floorObjectType == FloorObjectType.Fence)
					{
						OnTouchFence(new Point(i, j));
						Vector2 tileCenter = new Vector2(i, j) + new Vector2(0.5f, 0);
						Vector2 directionToTile = Vector2.Normalize(tileCenter - Center);

						float dot = Vector2.Dot(Vector2.Normalize(velocity), directionToTile);


						if (dot > 0)
						displacement += directionToTile * -dot;
					}
				}
			}
			if (displacement.Length() > 0)
			position += Vector2.Normalize(displacement) * velocity.Length();
		}

		public virtual void OnTouchFence(Point point)
		{

		}

		public Vector2 GamePosition
		{
			get => position;
			set => position = value;
		}

		public Vector2 ScreenPosition
		{
			get => Graphics.PositionHelper.ToScreenPosition(position);
			set => position = Graphics.PositionHelper.ToGamePosition(value);
		}

		public virtual void Draw(SpriteBatcher spriteBatcher)
		{
			spriteBatcher.Draw(textures[GetType().Name], position, null, Color.White, rotation, dimensions / 2, 0, SpriteEffects.None, 0);
		}

		public Entity Clone()
		{
			return (Entity)MemberwiseClone();
		}
	}
}
