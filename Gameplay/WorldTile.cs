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

		public Item floorItem;

		public bool PlaceItem(ref Item item)
		{
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
				// TODO: Stack the items together
				return true;
			}
			return false;
		}

		public Item TakeItem()
		{
			Item y = floorItem;
			floorItem = null;
			return y;
		}
	}
}
