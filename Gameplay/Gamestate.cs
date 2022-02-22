using Imaginosia.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public class Gamestate
	{
		public World world;
		public Player player;
		public List<Enemy> enemies;
		public List<Projectile> projectiles;

		public Gamestate()
		{
			world = World.GenerateSeeded(0);
			player = new Player();
			enemies = new List<Enemy>();
			projectiles = new List<Projectile>();
		}

		public void SpawnEnemy(Vector2 position, int type)
		{
			Enemy enemy = new Enemy();
			enemy.Spawn(position, type);
		}

		public void Update()
		{
			if (RNG.rand.Next(100) == 0)
			{
				SpawnEnemy(new Vector2((float)RNG.rand.NextDouble() * World.WorldWidth, (float)RNG.rand.NextDouble() * World.WorldHeight), RNG.rand.Next(3));
			}


			if (KeyHelper.Pressed(Keys.T))
			{
				ImaginationHandler.SwitchImagination();
			}

			player.Update();

			foreach (var item in enemies)
			{
				item.Update();
			}

			foreach (Projectile projectile in projectiles)
				for (int i = 0; i < projectile.updateRes; i++)
					projectile.Update();
			ListCleaner.CleanList(projectiles, delegate (Projectile obj)
			{
				if (obj.kill)
				{
					obj.OnKill();
					return true;
				}
				return false;
			});
		}

		public void Draw(SpriteBatcher spriteBatcher)
		{
			PositionHelper.CameraPos = player.position - PositionHelper.ToGamePosition(new Vector2(Game1.GameWidth, Game1.GameHeight) / 2, true);

			spriteBatcher.LayerBegin("tiles", blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
			world.Draw(spriteBatcher);
			spriteBatcher.End();
			spriteBatcher.Begin(samplerState: SamplerState.PointClamp);
			player.Draw(spriteBatcher);

			bool debug = true;

			if (debug)
				HitboxVisualization.DrawEntityHitbox(player, spriteBatcher);

			foreach (var item in enemies)
			{
				item.Draw(spriteBatcher);
				if (debug)
					HitboxVisualization.DrawEntityHitbox(item, spriteBatcher);

				Vector2 corner = PositionHelper.ToScreenPosition(new Vector2(item.Hitbox.Right, item.Hitbox.Bottom));
				if (debug)
					TextPrinter.Print($"Fear: {item.fear}", corner, spriteBatcher);
			}

			foreach (var item in projectiles)
			{
				item.Draw(spriteBatcher);
				if (debug)
					HitboxVisualization.DrawEntityHitbox(item, spriteBatcher);
			}
			if (debug)
				TextPrinter.Print("Special Tile", PositionHelper.ToScreenPosition(MouseHelper.MouseTileHover.ToVector2()), spriteBatcher);

			spriteBatcher.End();
			spriteBatcher.Begin(samplerState: SamplerState.PointClamp);
			UIHandler.Draw(spriteBatcher);
			spriteBatcher.End();
		}
	}
}
