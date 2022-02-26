using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Imaginosia.Gameplay
{
	public class Item
	{
		public Item()
		{
			stackCount = 1;
		}

		public int useCooldown = 0;
		public int useTime;

		public int stackCount;
		public int usesLeft;

		public bool stackable;
		public bool multiUse;
		public int maxStack;
		public bool consumable;
		public float magicCost;

		public ItemType itemID;

		public int hiddenValue;

		public void SetDefaults()
		{
			switch (itemID)
			{
				case ItemType.Gun:
					usesLeft = 20;
					useCooldown = 10;
					stackable = false;
					consumable = false;
					multiUse = true;
					magicCost = 1;
					break;
				case ItemType.Knife:
					useCooldown = 20;
					stackable = false;
					consumable = false;
					break;
				case ItemType.Axe:
					useCooldown = 20;
					stackable = false;
					consumable = false;
					break;
				case ItemType.Matchbox:
					usesLeft = 10;
					useCooldown = 10;
					stackable = false;
					consumable = false;
					multiUse = true;
					magicCost = 2;
					break;
				case ItemType.MeatRaw:
					stackable = true;
					consumable = true;
					maxStack = 15;
					hiddenValue = 1800; // 30 seconds to cook
					break;
				case ItemType.MeatCooked:
					stackable = true;
					consumable = true;
					maxStack = 15;
					break;
				case ItemType.Fur:
					stackable = true;
					maxStack = 15;
					break;
				case ItemType.Clothes:
					stackable = false;
					hiddenValue = 20;
					break;
				case ItemType.Bag:
					stackable = false;
					break;
				case ItemType.Bone:
					stackable = true;
					maxStack = 15;
					break;
				case ItemType.BoneKnife:
					stackable = true;
					consumable = true;
					maxStack = 15;
					break;
				case ItemType.BoneTrap:
					stackable = true;
					maxStack = 15;
					break;
				case ItemType.Wood:
					stackable = true;
					maxStack = 15;
					break;
				case ItemType.WoodStake:
					stackable = true;
					consumable = true;
					maxStack = 15;
					break;
				default:
					break;
			}
		}

		public virtual bool CanUseItem()
		{
			if (ImaginationHandler.IsImagination)
			{
				if (Game1.gamestate.player.magic < magicCost * (Game1.gamestate.player.clothing > 0 ? 0.5f : 1f))
				{
					return false;
				}

				return useTime == 0;
			}
			else
			{
				if (multiUse && usesLeft <= 0)
				{
					return false;
				}

				return useTime == 0;
			}
		}

		public virtual bool UseItem()
		{
			useTime = useCooldown;
			if (ImaginationHandler.IsImagination)
			{
				if (multiUse)
				{
					Game1.gamestate.player.magicRechargeTimer = 0;
					Game1.gamestate.player.magic -= magicCost * (Game1.gamestate.player.clothing > 0 ? 0.5f : 1f);
				}
			}
			else
			{
				if (multiUse)
				{
					usesLeft--;
				}
			}

			
			if (stackable)
			{
				stackCount--;
				if (stackCount <= 0)
					return false;
			}
			return true;
		}

		public virtual void Update()
		{
			if (useTime > 0)
				useTime--;
		}

		public static void MergeStacks(ref Item stackTo, ref Item stackFrom)
		{
			stackTo.stackCount += stackFrom.stackCount;
			if (stackTo.stackCount > stackTo.maxStack)
			{
				stackFrom.stackCount = stackTo.stackCount - stackTo.maxStack;
				stackTo.stackCount = stackTo.maxStack;
			}
			else
			{
				stackFrom = null;
			}
		}

		public virtual string GetName()
		{
			string name = "Not An Item";
			if (ImaginationHandler.IsImagination)
			{
				switch (itemID)
				{
					case ItemType.Gun: name = "Magic Wand";				  break;
					case ItemType.Knife: name = "Sacred Sword";			  break;
					case ItemType.Axe: name = "Super Hammer";			  break;
					case ItemType.Matchbox: name = "Baby Dragon";		  break;
					case ItemType.MeatRaw: name = "Cookie Dough";		  break;
					case ItemType.MeatCooked: name = "Baked Cookies";	  break;
					case ItemType.Fur: name = "Fluffy Cotton";			  break;
					case ItemType.Clothes: name = "Enchanted Robes";	  break;
					case ItemType.Bag: name = "Magic Pouch";			  break;
					case ItemType.Bone: name = "Candy Cane";			  break;
					case ItemType.BoneKnife: name = "Spearmint";		  break;
					case ItemType.BoneTrap: name = "Sugar Bomb";		  break;
					case ItemType.Wood: name = "Wacky Wood";			  break;
					case ItemType.WoodStake: name = "Fancy Fence";        break;
					default:
						break;
				}
			}
			else
			{
				switch (itemID)
				{
					case ItemType.Gun: name = "Gun";			    break;
					case ItemType.Knife: name = "Survival Knife";   break;
					case ItemType.Axe: name = "Survival Axe";	    break;
					case ItemType.Matchbox: name = "Matchbox";	    break;
					case ItemType.MeatRaw: name = "Raw Meat";	    break;
					case ItemType.MeatCooked: name = "Cooked Meat"; break;
					case ItemType.Fur: name = "Fur";			    break;
					case ItemType.Clothes: name = "Clothing";	    break;
					case ItemType.Bag: name = "Bag";			    break;
					case ItemType.Bone: name = "Bone";			    break;
					case ItemType.BoneKnife: name = "Bone Knife";   break;
					case ItemType.BoneTrap: name = "Bone Trap";	    break;
					case ItemType.Wood: name = "Wood";			    break;
					case ItemType.WoodStake: name = "Wooden Stake"; break;
					default:
						break;
				}
			}
			return name + (stackCount > 1 ? " x" + stackCount : "");
		}

		public int GetSlice()
		{
			return (int)itemID * 2 + (ImaginationHandler.IsImagination ? 1 : 0);
		}

		public void Draw(Vector2 position, SpriteBatcher batcher)
		{
			batcher.Draw(Assets.Tex2["items"].texture, position, Assets.Tex2["items"].frames[GetSlice()], Color.White);
		}
	}
}
