using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public class WorldTile
	{
		public const int TileWidth = 16;
		public const int TileHeight = 8;

		public int sprite;

		public FloorObjectType floorObjectType;
		public int floorObjectHealth;
		public int floorObjectStyle;

		public Item floorItem;

		public bool PlaceItem(ref Item item)
		{
			if (floorObjectType != FloorObjectType.None)
			{
				return false;
			}

			if (floorItem == null)
			{
				floorItem = item;
				item = null;
				return true;
			}
			else if (floorItem.itemID == ItemType.Bag)
			{
				// TODO: Place items in the bag
				return true;
			}
			else if (floorItem.itemID == item.itemID)
			{
				Item.MergeStacks(ref floorItem, ref item);
				return true;
			}
			return false;
		}

		public void Damage(Point point)
		{
			if (floorObjectType == FloorObjectType.Tree)
			{
				Item wood = new Item();
				wood.itemID = ItemType.Wood;
				wood.SetDefaults();
				wood.stackCount = RNG.rand.Next(3) + 1;
				Game1.gamestate.world.PlaceItemNearest(point, ref wood);

				floorObjectHealth--;
			}


			if (floorObjectHealth <= 0)
			{
				floorObjectType = FloorObjectType.None;
			}
		}

		public Item TakeItem()
		{
			Item y = floorItem;
			floorItem = null;
			return y;
		}
	}
}
