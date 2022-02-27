using Imaginosia;
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

		public static void Reset()
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

			if (player.clothing > 0)
			{
				new Item() { itemID = ItemType.Clothes }.Draw(hudBarOrigin + new Vector2(64, 0), spriteBatcher);
				TextPrinter.Print((player.clothing) * 5 + "%", hudBarOrigin + new Vector2(66, 12), spriteBatcher);
			}

			// Draw inventory
			for (int i = 0; i < player.inventory.Length - (player.equippedBag ? 0 : 2); i++)
			{
				Vector2 position = new Vector2(10, 220);
				position += new Vector2(18, 0) * i;
				if (i == player.itemSlot)
					position += new Vector2(0, -10);

				if (i > 3)
					position += new Vector2(180, 0);

				spriteBatcher.Draw(Assets.Tex2["inventory"].texture, position, Assets.Tex2["inventory"].frames[0], Color.White);
				
				
				if (player.inventory[i] == null)
					continue;
				player.inventory[i].Draw(position, spriteBatcher);


				if (!player.inventory[i].CanUseItem())
				{
					Rectangle reuseShade = Assets.Tex2["inventory"].frames[3];
					reuseShade.Height = (int)(reuseShade.Height * (float)player.inventory[i].useTime / player.inventory[i].useCooldown);

					spriteBatcher.Draw(Assets.Tex2["inventory"].texture, position, reuseShade, new Color(255, 255, 255, 0));
				}

				

				int number = 0;

				if (player.inventory[i].stackable)
				{
					number = player.inventory[i].stackCount;
				}

				if (player.inventory[i].usesLeft > 0)
				{
					// Multi-use items in the imagination use magic instead
					if (ImaginationHandler.IsImagination)
					{
						continue;
					}
					number = player.inventory[i].usesLeft;
				}

				if (number > 0)
				{
					TextPrinter.Print(number.ToString(), position + new Vector2(1), spriteBatcher);
				}
			}

			if (player.HeldItem != null)
			{
				string text = player.HeldItem.usesLeft.ToString();
				TextPrinter.Print(player.HeldItem.GetName(), new Vector2(20, 200), spriteBatcher, background: true);
			}

			if (player.hallucinogen > 0 && !ImaginationHandler.IsImagination)
			{
				if (player.hunger > 5f)
				{
					TextPrinter.Print("Press T to imagine", new Vector2(290, 26), spriteBatcher, background: true);
				}
				else
				{
					TextPrinter.Print("Too hungry to imagine", new Vector2(280, 26), spriteBatcher, background: true);
				}
			}

			for (int i = 0; i < player.hallucinogen; i++)
			{
				Vector2 position = new Vector2(340 - i * 18, 10);
				spriteBatcher.Draw(Assets.Tex["shroomIcon"], position, null, Color.White);
			}
		}

		public static Texture2D heart;
		public static SlicedSprite activeBox;
		public static Player player;
		public static int hitFlash;
	}
}