using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public class Projectile : Entity
	{
		public Projectile(Vector2 position, Vector2 velocity, int damage, int id)
		{
			this.position = position;
			this.velocity = velocity;
			this.damage = damage;
			this.id = id;
			SetDefaults();


			Game1.gamestate.projectiles.Add(this);
		}

		public void SetDefaults()
		{
			dimensions = new Vector2(1, 1);
		}

		public int timeLeft;
		public int damage;
		public float knockback;
		public int updateRes = 1;
		public int id;

		public bool kill = false;

		public override void Draw(SpriteBatcher spriteBatcher)
		{
			spriteBatcher.Draw(Assets.Tex2["projectile"].texture, ScreenPosition, Assets.Tex2["projectile"].frames[id], Color.White, rotation, dimensions / 2, 1, SpriteEffects.None, 0);
		}

		public override void Update()
		{
			if (kill)
				return;

			timeLeft--;
			if (timeLeft < 0)
			{
				kill = true;
			}
			rotation = (float)Math.Atan2(velocity.Y, velocity.X) + MathHelper.Pi / 2;

			foreach (Enemy enemy in Game1.gamestate.enemies)
			{
				if (!enemy.dead && enemy.Hitbox.Intersects(Hitbox))
				{
					OnHitEnemy(enemy);
					break;
				}
			}

			base.Update();
		}

		public virtual void OnHitEnemy(Enemy enemy)
		{
			enemy.TakeDamage(damage * (int)(RNG.rand.NextDouble() + 0.5f), Vector2.Normalize(velocity) * knockback);
			kill = true;
		}

		public override void WallCollide()
		{
			kill = true;
			timeLeft = 0;
		}

		public virtual void OnKill()
		{

		}
	}
}
