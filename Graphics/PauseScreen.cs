using Imaginosia.Gameplay;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Graphics
{
	public static class PauseScreen
	{
		public static int guidePage;
		public const int guidePageCount = 3;

		public static void DrawHelp()
		{
			SpriteBatcher.Draw(Assets.Tex["special"], new Rectangle(0, 0, Game1.GameWidth, Game1.GameHeight), Color.Black * 0.5f);

			Point pos = new Point(10);
			DrawText(Game1.GameWidth / 2, pos.Y, "Game paused.", true);
			pos.Y += 8;
			DrawText(Game1.GameWidth / 2, pos.Y, "Press space to resume.", true);
			pos.Y += 18;

			SpriteBatcher.Draw(Assets.Tex["special"], new Rectangle(pos, new Point(Game1.GameWidth - 10, 2)), Color.White);
			pos.Y += 8;

			string pagename;
			switch (guidePage)
			{
				case 0: pagename = "Controls"; break;
				case 1: pagename = "Items"; break; 
				case 2: pagename = "Recipes"; break;
				default:
					throw new NotImplementedException("The specified help page does not exist");
					break;
			}

			DrawText(pos.X, pos.Y, $"Game Guide (Page {guidePage + 1}/3: {pagename}, scroll to navigate)");
			pos.Y += 8;

			

			switch (guidePage)
			{
				case 0:
					DrawText(pos.X, pos.Y, "Press Escape to quit to the menu.");
					pos.Y += 12;
					DrawText(pos.X, pos.Y, "Shift+left click to drop an item.");
					pos.Y += 8;
					DrawText(pos.X, pos.Y, "Shift+right click to pick up an item.");
					pos.Y += 12;
					DrawText(pos.X, pos.Y, ImaginationHandler.IsImagination ? "Left click to thrust with your sword. At full health, it fires a sword beam." : "Left click to stab with your knife.");
					pos.Y += 8;
					DrawText(pos.X, pos.Y, ImaginationHandler.IsImagination ? "Right click to cast a fireball or throw a spearmint. Your magic wand requires magic!" : "Right click to fire your gun or throw a bone knife.");
					pos.Y += 8;
					DrawText(pos.X, pos.Y, "Left click while holding an item to use it.");
					pos.Y += 12;
					DrawText(pos.X, pos.Y, "Press C to quickly eat the first edible item in your inventory.");
					pos.Y += 12;
					DrawText(pos.X, pos.Y, ImaginationHandler.IsImagination ? "Enemies may drop items after you defeat them." : "After hunting an animal, you can click on its body to obtain resources. Larger animals yield more.");
					pos.Y += 12;
					if (ImaginationHandler.IsImagination)
					{
						DrawText(pos.X, pos.Y, "Press T to leave the imagination manually.");
						pos.Y += 8;
						DrawText(pos.X, pos.Y, "You will still lose hunger in the imagination. If you get too hungry, you will be kicked out.");
					}
					else
					{
						DrawText(pos.X, pos.Y, "You will find purple mushrooms scarcely spread around the world. Click on one to obtain hallucinogen.");
						pos.Y += 8;
						DrawText(pos.X, pos.Y, "Press T to use a hallucinogen and go to the imaginary world, as long as you aren't too hungry.");
					}

					
					break;
				case 1:
					DrawItem(pos.X, pos.Y, ItemType.Axe);
					DrawText(pos.X + 18, pos.Y, $"{drawItems[ItemType.Axe].GetName()}: Use it on a tree or fence to cut it down and obtain wood.");
					pos.Y += 8;
					pos.Y += 10;

					DrawItem(pos.X, pos.Y, ItemType.Matchbox);
					DrawText(pos.X + 18, pos.Y, $"{drawItems[ItemType.Matchbox].GetName()}: Use it on a pile of wood you dropped on the ground to create a fire.");
					pos.Y += 8;
					DrawText(pos.X + 18, pos.Y, "Click on the fire while holding wood to fuel it. If you don't, it will burn out.");
					pos.Y += 10;

					DrawItem(pos.X, pos.Y, ItemType.MeatRaw);
					DrawText(pos.X + 18, pos.Y, $"{drawItems[ItemType.MeatRaw].GetName()}: Use it to eat it, restoring a small amount of {(ImaginationHandler.IsImagination ? "magic" : "hunger")}.");
					pos.Y += 8;
					DrawText(pos.X + 18, pos.Y, "Drop it next to a fire and wait 30 seconds to cook it. This will increase its effects greatly.");
					pos.Y += 10;

					DrawItem(pos.X, pos.Y, ItemType.Clothes);
					DrawText(pos.X + 18, pos.Y, $"{drawItems[ItemType.Clothes].GetName()}: Use it to equip it, increasing your defense{(ImaginationHandler.IsImagination ? " and magical efficiency" : "")}.");
					pos.Y += 8;
					DrawText(pos.X + 18, pos.Y, "It will wear out eventually. Use a new one to replace your old clothes.");
					pos.Y += 10;

					DrawItem(pos.X, pos.Y, ItemType.Bag);
					DrawText(pos.X + 18, pos.Y, $"{drawItems[ItemType.Bag].GetName()}: Use it to equip it, increasing the number of items you can carry.");
					pos.Y += 8;
					DrawText(pos.X + 18, pos.Y, "You can only wear one bag! Don't waste resources by making more than one.");
					pos.Y += 10;

					DrawItem(pos.X, pos.Y, ItemType.BoneKnife);
					DrawText(pos.X + 18, pos.Y, $"{drawItems[ItemType.BoneKnife].GetName()}: Simple ranged weapon. Use it to throw it.");
					pos.Y += 8;
					pos.Y += 10;

					DrawItem(pos.X, pos.Y, ItemType.BoneTrap);
					DrawText(pos.X + 18, pos.Y, $"{drawItems[ItemType.BoneTrap].GetName()}: Use it on the ground to place it.");
					pos.Y += 8;
					DrawText(pos.X + 18, pos.Y, $"{(ImaginationHandler.IsImagination ? "The bomb will explode when an enemy touches it, damaging nearby enemies." : "The trap will slow and weaken animals that walk over it.")}");
					pos.Y += 10;

					DrawItem(pos.X, pos.Y, ItemType.WoodStake);
					DrawText(pos.X + 18, pos.Y, $"{drawItems[ItemType.WoodStake].GetName()}: Use it on the ground to place it. It will block {(ImaginationHandler.IsImagination ? "enemies" : "animals")} from crossing it.");
					pos.Y += 8;
					pos.Y += 10;
					break;
				case 2:
					DrawText(pos.X, pos.Y, "To craft an item, you place the material required on the ground and then click on it with a tool.");
					pos.Y += 8;
					DrawText(pos.X, pos.Y, "Here is a list of crafting recipes (tool + material = result)");
					pos.Y += 12;


					DrawText(pos.X, pos.Y, $"{drawItems[ItemType.BoneKnife].GetName()} recipe:");
					pos.Y += 8;
					DrawItem(pos.X, pos.Y, ItemType.Knife); 
					DrawText(pos.X + 16, pos.Y + 6, "+");
					DrawItem(pos.X + 20, pos.Y, ItemType.Bone);
					DrawText(pos.X + 36, pos.Y + 6, "=");
					DrawItem(pos.X + 40, pos.Y, ItemType.BoneKnife);
					DrawText(pos.X + 40, pos.Y, "2");
					pos.Y += 18;

					DrawText(pos.X, pos.Y, $"{drawItems[ItemType.BoneTrap].GetName()} recipe:");
					pos.Y += 8;
					DrawItem(pos.X, pos.Y, ItemType.Axe);
					DrawText(pos.X + 16, pos.Y + 6, "+");
					DrawItem(pos.X + 20, pos.Y, ItemType.Bone);
					DrawText(pos.X + 36, pos.Y + 6, "=");
					DrawItem(pos.X + 40, pos.Y, ItemType.BoneTrap);
					pos.Y += 18;

					DrawText(pos.X, pos.Y, $"{drawItems[ItemType.WoodStake].GetName()} recipe:");
					pos.Y += 8;
					DrawItem(pos.X, pos.Y, ItemType.Axe);
					DrawText(pos.X + 16, pos.Y + 6, "+");
					DrawItem(pos.X + 20, pos.Y, ItemType.Wood);
					DrawText(pos.X + 36, pos.Y + 6, "=");
					DrawItem(pos.X + 40, pos.Y, ItemType.WoodStake);
					pos.Y += 18;

					DrawText(pos.X, pos.Y, $"{drawItems[ItemType.Clothes].GetName()} recipe:");
					pos.Y += 8;
					DrawItem(pos.X, pos.Y, ItemType.Knife);
					DrawText(pos.X + 16, pos.Y + 6, "+");
					DrawItem(pos.X + 20, pos.Y, ItemType.Fur);
					DrawText(pos.X + 20, pos.Y, "5");
					DrawText(pos.X + 36, pos.Y + 6, "=");
					DrawItem(pos.X + 40, pos.Y, ItemType.Clothes);
					pos.Y += 18;

					DrawText(pos.X, pos.Y, $"{drawItems[ItemType.Bag].GetName()} recipe:");
					pos.Y += 8;
					DrawItem(pos.X, pos.Y, ItemType.BoneKnife);
					DrawText(pos.X + 16, pos.Y + 6, "+");
					DrawItem(pos.X + 20, pos.Y, ItemType.Fur);
					DrawText(pos.X + 20, pos.Y, "8");
					DrawText(pos.X + 36, pos.Y + 6, "=");
					DrawItem(pos.X + 40, pos.Y, ItemType.Bag);
					pos.Y += 18;


					break;
				default:
					break;
			}
		}

		static PauseScreen()
		{
			drawItems = new Dictionary<ItemType, Item>();
			for (int i = 0; i < Enum.GetValues(typeof(ItemType)).Length; i++)
			{
				drawItems[(ItemType)i] = new Item() { itemID = (ItemType)i };
			}
		}

		static SpriteBatcher SpriteBatcher { get => Game1.getGame.spriteBatch; }
		static Dictionary<ItemType, Item> drawItems;

		private static void DrawText(int x, int y, string text, bool center = false)
		{
			TextPrinter.Print(text, new Vector2(x, y), SpriteBatcher, center);
		}

		private static void DrawItem(int x, int y, ItemType type)
		{
			drawItems[type].Draw(new Vector2(x, y), SpriteBatcher);
		}


	}
}
