﻿using Imaginosia;
using Imaginosia.Gameplay;
using Imaginosia.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Imaginosia.Graphics
{
	public static class UIHandler
	{
		static UIHandler()
		{
			player = Game1.gamestate.player;
		}

		public static void Draw(SpriteBatcher spriteBatcher)
		{
			// Draw HUD bars
			float bar1 = player.health / Player.MaxHealth;
			float bar2 = ImaginationHandler.IsImagination ? player.magic / Player.MaxMagic : player.hunger / Player.MaxHunger;

			Vector2 hudBarOrigin = new Vector2(10, 10);

			int imagination = ImaginationHandler.IsImagination ? 4 : 0;

			Rectangle bar1Rect = Assets.Tex2["hudBars"].frames[1 + imagination];
			bar1Rect.Width = (int)(bar1Rect.Width * bar1);

			Rectangle bar2Rect = Assets.Tex2["hudBars"].frames[3 + imagination];
			bar2Rect.Width = (int)(bar2Rect.Width * bar2);

			spriteBatcher.Draw(Assets.Tex2["hudBars"].texture, hudBarOrigin, Assets.Tex2["hudBars"].frames[0 + imagination], Color.White);
			spriteBatcher.Draw(Assets.Tex2["hudBars"].texture, hudBarOrigin, bar1Rect, Color.White);
			spriteBatcher.Draw(Assets.Tex2["hudBars"].texture, hudBarOrigin + new Vector2(0, 8), Assets.Tex2["hudBars"].frames[2 + imagination], Color.White);
			spriteBatcher.Draw(Assets.Tex2["hudBars"].texture, hudBarOrigin + new Vector2(0, 8), bar2Rect, Color.White);

			// Draw inventory
			for (int i = 0; i < player.inventory.Length; i++)
			{
				Vector2 position = new Vector2(10, 140);
				position += new Vector2(18, 0) * i;
				if (i == player.itemSlot)
					position += new Vector2(0, -10);
				spriteBatcher.Draw(Assets.Tex2["inventory"].texture, position, Assets.Tex2["inventory"].frames[0], Color.White);
				if (player.inventory[i] == null)
					continue;
				player.inventory[i].Draw(position, spriteBatcher);
			}

			// Draw gun name and reloading text/ammo counter
			if (player.HeldItem != null)
			{
				string text = player.HeldItem.usesLeft.ToString();
				TextPrinter.Print(player.HeldItem.GetName(), new Vector2(20, 100), spriteBatcher);
				TextPrinter.Print(text, new Vector2(20, 120), spriteBatcher);
			}
		}

		public static Texture2D heart;
		public static SlicedSprite activeBox;
		public static Player player;
		public static int hitFlash;
	}
}