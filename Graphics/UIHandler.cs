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
			// Draw health bar
			int heartSlices = (int)player.health;
			int hearts = player.maxHealth / 2;
			for (int i = 0; i < hearts; i++)
			{
				int frame = 2;
				if (heartSlices == 1)
				{
					frame = 1;
				}
				else if (heartSlices > 1)
				{
					frame = 0;
				}
				// if (hitFlash > 0 && heartSlices == 0 || heartSlices == 1)
				// {
				// 	Effect effect = Game1.flash.Clone();
				// 	effect.Parameters["flash"].SetValue(hitFlash / 5f);
				// 	Effect[] effects = new Effect[] { effect };
				// 	DrawHandler.DrawSliced(SlicedSprite.Frameify(heart, 0, 2), frame, new Vector2(120 + 20 * i, 360 - hitFlash), Color.White, shaders: effects);
				// }
				// else
				// {
				// 	DrawHandler.DrawSliced(SlicedSprite.Frameify(heart, 0, 2), frame, new Vector2(120 + 20 * i, 360), Color.White);
				// }
				heartSlices -= 2;
			}
			if (hitFlash > 0)
			{
				hitFlash--;
			}

			// Draw inventory
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (player.inventory[i] == null)
					continue;
				Vector2 position = new Vector2(60, 400) + MathTools.RotateVector(new Vector2(0, -30), MathHelper.TwoPi / 5 * (-i + player.itemSlot));
				if (player.inventory[i] == player.HeldItem)
					position += new Vector2(0, -30);
				// DrawHandler.Draw(Item.textures[player.guns[i].GetType().Name], position, null, Color.White, centerBottom: true);
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