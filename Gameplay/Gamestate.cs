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
			world = World.GenerateNew();
			player = new Player();
			enemies = new List<Enemy>();
			projectiles = new List<Projectile>();

			DustManager.dusts.Clear();
			ImaginationHandler.Reset();
		}

		public void SpawnEnemy(Vector2 position, int type)
		{
			Enemy enemy = new Enemy();
			enemy.Spawn(position, type);
		}

		public void Update()
		{
			ImaginationHandler.TimeSinceSwitched++;

			DustManager.Update();

			if (RNG.rand.Next(100) == 0 && enemies.Count < 20)
			{
				Vector2 spawnPosition = new Vector2((float)RNG.rand.NextDouble() * World.WorldWidth, (float)RNG.rand.NextDouble() * World.WorldHeight);

				if (Vector2.Distance(spawnPosition, player.position) > 30)
				SpawnEnemy(spawnPosition, RNG.rand.Next(3));
			}

			for (int i = world.fires.Count - 1; i >= 0; i--)
			{
				world.tiles[world.fires[i].X, world.fires[i].Y].UpdateFire(world.fires[i]);
			}


			if (KeyHelper.Pressed(Keys.T) && ((player.hallucinogen > 0 && player.hunger > 1f) || ImaginationHandler.IsImagination))
			{
				if (!ImaginationHandler.IsImagination)
				{
					player.hallucinogen--;
				}	
				ImaginationHandler.SwitchImagination();
			}

			if (ImaginationHandler.IsImagination && player.hunger < 1f)
			{
				ImaginationHandler.LeaveImagination();
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
			ListCleaner.CleanList(enemies, delegate (Enemy obj)
			{
				if (obj.remove)
				{
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

			bool debug = false;

			if (debug)
				HitboxVisualization.DrawEntityHitbox(player, spriteBatcher);

			foreach (var item in enemies)
			{
				item.Draw(spriteBatcher);
				if (debug)
					HitboxVisualization.DrawEntityHitbox(item, spriteBatcher);

				Vector2 corner = PositionHelper.ToScreenPosition(new Vector2(item.Hitbox.Right, item.Hitbox.Bottom));
				if (debug)
				{
					TextPrinter.Print($"Fear: {item.fear}", corner, spriteBatcher);
					TextPrinter.Print($"Alert: {item.alertness}", corner + new Vector2(0, 6), spriteBatcher);
					TextPrinter.Print($"Drive: {item.drive}", corner + new Vector2(0, 12), spriteBatcher);
					TextPrinter.Print($"Attention: {item.attention}", corner + new Vector2(0, 18), spriteBatcher);
					TextPrinter.Print($"State: {item.state}", corner + new Vector2(0, 24), spriteBatcher);
				}
			}

			foreach (var item in projectiles)
			{
				item.Draw(spriteBatcher);
				if (debug)
					HitboxVisualization.DrawEntityHitbox(item, spriteBatcher);
			}
			if (debug)
				TextPrinter.Print("Special Tile", PositionHelper.ToScreenPosition(MouseHelper.MouseTileHover.ToVector2()), spriteBatcher);

			DustManager.Draw(spriteBatcher, 0);

			spriteBatcher.End();
		}
	}
}
