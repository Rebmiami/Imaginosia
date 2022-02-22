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
		public int useCooldown = 0;
		public int useTime;

		public int stackCount;
		public int usesLeft;

		public bool stackable;
		public int maxStack;
		public bool consumable;

		public ItemType itemID;

		public void SetDefaults()
		{
			switch (itemID)
			{
				case ItemType.Gun:
					usesLeft = 20;
					stackable = false;
					consumable = false;
					break;
				case ItemType.Knife:
					break;
				case ItemType.Axe:
					break;
				case ItemType.MeatRaw:
					consumable = true;
					break;
				case ItemType.MeatCooked:
					consumable = true;
					break;
				case ItemType.Fur:
					break;
				case ItemType.Clothes:
					break;
				case ItemType.Bag:
					break;
				case ItemType.Bone:
					break;
				case ItemType.BoneKnife:
					break;
				case ItemType.BoneTrap:
					break;
				case ItemType.Wood:
					break;
				case ItemType.WoodStake:
					break;
				default:
					break;
			}
		}

		public virtual bool CanUseItem()
		{
			return useTime == 0;
		}

		public virtual bool UseItem()
		{
			useTime = useCooldown;
			if (usesLeft > 0)
			{
				usesLeft--;
				if (usesLeft <= 0 && consumable)
				{
					return false;
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

		public virtual string GetName()
		{
			if (ImaginationHandler.IsImagination)
			{
				switch (itemID)
				{
					case ItemType.Gun: return "Magic Wand";
					case ItemType.Knife: return "Sacred Sword";
					case ItemType.Axe: return "Super Hammer";
					case ItemType.Matchbox: return "Baby Dragon";
					case ItemType.MeatRaw: return "Cookie Dough";
					case ItemType.MeatCooked: return "Baked Cookies";
					case ItemType.Fur: return "Fluffy Cotton";
					case ItemType.Clothes: return "Enchanted Robes";
					case ItemType.Bag: return "Magic Pouch";
					case ItemType.Bone: return "Candy Cane";
					case ItemType.BoneKnife: return "Spearmint";
					case ItemType.BoneTrap: return "Sugar Bomb";
					case ItemType.Wood: return "Wacky Wood";
					case ItemType.WoodStake: return "Fancy Fence";
					default:
						break;
				}
			}
			else
			{
				switch (itemID)
				{
					case ItemType.Gun: return "Gun";
					case ItemType.Knife: return "Survival Knife";
					case ItemType.Axe: return "Survival Axe";
					case ItemType.Matchbox: return "Matchbox";
					case ItemType.MeatRaw: return "Raw Meat";
					case ItemType.MeatCooked: return "Cooked Meat";
					case ItemType.Fur: return "Fur";
					case ItemType.Clothes: return "Clothing";
					case ItemType.Bag: return "Bag";
					case ItemType.Bone: return "Bone";
					case ItemType.BoneKnife: return "Bone Knife";
					case ItemType.BoneTrap: return "Bone Trap";
					case ItemType.Wood: return "Wood";
					case ItemType.WoodStake: return "Wooden Stake";
					default:
						break;
				}
			}
			return "Not An Item";
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
