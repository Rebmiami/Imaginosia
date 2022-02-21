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

		public Gamestate()
		{
			world = World.GenerateSeeded(0);
			player = new Player();
			enemies = new List<Enemy>();
		}

		public void SpawnEnemy(Vector2 position, int type)
		{
			Enemy enemy = new Enemy();
			enemy.Spawn(position, type);
		}

		public void Update()
		{
			if (KeyHelper.Pressed(Keys.T))
			{
				ImaginationHandler.SwitchImagination();
			}


			player.Update();
		}

		public void Draw(SpriteBatcher spriteBatcher)
		{
			PositionHelper.CameraPos = player.position - PositionHelper.ToGamePosition(new Vector2(Game1.GameWidth, Game1.GameHeight) / 2, true);

			spriteBatcher.LayerBegin("tiles", blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
			world.Draw(spriteBatcher);
			spriteBatcher.End();
			spriteBatcher.Begin(samplerState: SamplerState.PointClamp);
			player.Draw(spriteBatcher);

			HitboxVisualization.DrawEntityHitbox(player, spriteBatcher);

			foreach (var item in enemies)
			{
				item.Draw(spriteBatcher);
				HitboxVisualization.DrawEntityHitbox(item, spriteBatcher);
			}

			spriteBatcher.End();
			spriteBatcher.Begin(samplerState: SamplerState.PointClamp);
			UIHandler.Draw(spriteBatcher);
			spriteBatcher.End();
		}
	}
}
