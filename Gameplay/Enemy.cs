using Imaginosia.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public class Enemy : Entity
	{
		public int type;

		public int health;
		public int maxHealth;

		public float fear; // Increased by player actions such as making noise, attacking, or using the flashlight
		public float fight; // Enemy's desire to fight the player when afraid. Enemies are more likely to fight if they are healthy.
		public float flight; // Enemy's desire to run away from the player when afraid. Enemies are more likely to flee if hurt or very scared

		public Vector2 interest; // The position of the enemy's interest, or what it wants to get. May be the player or an item on the ground
		public float attention; // How willing the enemy is to follow its interest. It will lose interest if it continually fails

		public Item carryingItem;

		public int hitTimer; // How long to show the "hurt" animation after taking damage
		public int animationTimer; // Used for animations
		public int animFrame;

		public bool dead; // If the enemy is dead, this will be true.

		public Vector2 direction = Vector2.UnitX;

		public virtual void Spawn(Vector2 position, int type)
		{
			this.type = type;
			Game1.gamestate.enemies.Add(this);
			this.position = position;
			this.position = position - (GetFoot() - position);

			SetDefaults();
		}

		public void SetDefaults()
		{
			dimensions = PositionHelper.ToGamePosition(new Vector2(16, 18), true);
			switch (type)
			{
				case 0:
					maxHealth = 10;
					break;
				case 1:
					maxHealth = 5;
					break;
				case 2:
					maxHealth = 20;
					break;
				default:
					break;
			}
			health = maxHealth;
		}

		public override void Update()
		{
			fear *= 0.995f;
		}

		public override void Draw(SpriteBatcher spriteBatcher)
		{
			SlicedSprite texture;

			if (ImaginationHandler.IsImagination)
			{
				switch (type)
				{
					case 0:
						texture = Assets.Tex2["wolfImaginary"];
						break;
					case 1:
						texture = Assets.Tex2["ratImaginary"];
						break;
					case 2:
						texture = Assets.Tex2["bearImaginary"];
						break;
					default:
						throw new InvalidOperationException("No enemy with this texture");
				}
			}
			else
			{
				switch (type)
				{
					case 0:
						texture = Assets.Tex2["wolfReal"];
						break;
					case 1:
						texture = Assets.Tex2["ratReal"];
						break;
					case 2:
						texture = Assets.Tex2["bearReal"];
						break;
					default:
						throw new InvalidOperationException("No enemy with this texture");
				}
			}


			spriteBatcher.Draw(texture.texture, ScreenPosition - new Vector2(8, 16), texture.frames[animFrame], Color.White, 0, dimensions / 2, 1, direction.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
	}
}
