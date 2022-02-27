using Imaginosia.Audio;
using Imaginosia.Graphics;
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

		public void Damage(Point point, bool drops = true)
		{
			if (floorObjectType == FloorObjectType.Tree)
			{
				Item wood = new Item();
				wood.itemID = ItemType.Wood;
				wood.SetDefaults();
				wood.stackCount = RNG.rand.Next(3) + 1;
				if (drops)
				Game1.gamestate.world.PlaceItemNearest(point, ref wood);

				floorObjectHealth--;

				SoundSystem.PlayAtPosition(Game1.gamestate.player.position, point.ToVector2(), "treeHit", true);

				DustManager.CreateDustPuff(new FloatRectangle(point.ToVector2(), Vector2.One), Vector2.Zero, 0.1f, 10, 8);
			}
			if (floorObjectType == FloorObjectType.Fence)
			{

				floorObjectHealth--;

				SoundSystem.PlayAtPosition(Game1.gamestate.player.position, point.ToVector2(), "treeHit", true);
				DustManager.CreateDustPuff(new FloatRectangle(point.ToVector2(), Vector2.One), Vector2.Zero, 0.1f, 10, 8);
			}


			if (floorObjectHealth <= 0)
			{
				if (floorObjectType == FloorObjectType.Tree)
				{
					SoundSystem.PlayAtPosition(Game1.gamestate.player.position, point.ToVector2(), "treeBreak", true);
					floorObjectType = FloorObjectType.None;
				}	
				if (floorObjectType == FloorObjectType.Fence)
				{
					floorObjectType = FloorObjectType.None;
					Item stake = new Item();
					stake.itemID = ItemType.WoodStake;
					stake.SetDefaults();
					stake.stackCount = 1;
					if (drops)
					Game1.gamestate.world.PlaceItemNearest(point, ref stake);
				}
			}
		}

		public void Ignite(Point point)
		{
			Game1.gamestate.world.fires.Add(point);

			if (floorItem != null && (floorItem.itemID == ItemType.Wood || floorItem.itemID == ItemType.WoodStake))
			{
				floorObjectHealth = 400 * floorItem.stackCount;
				floorItem = null;
			}

			if (floorObjectType == FloorObjectType.Tree)
			{
				floorObjectHealth = 600;
			}

			floorObjectType = FloorObjectType.Campfire;
		}

		public void UpdateFire(Point point)
		{
			floorObjectHealth--;

			for (int i = 0; i < 9; i++)
			{
				int x = i % 3 - 1 + point.X;
				int y = i / 3 - 1 + point.Y;

				if (World.InBounds(new Point(x, y)))
				{
					Item item = Game1.gamestate.world.tiles[x, y].floorItem;
					if (item != null && item.itemID == ItemType.MeatRaw)
					{
						item.hiddenValue--;
						if (item.hiddenValue <= 0)
						{
							item.itemID = ItemType.MeatCooked;
						}
					}
				}
			}

			if (floorObjectHealth <= 0)
			{
				Game1.gamestate.world.fires.Remove(point);
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
