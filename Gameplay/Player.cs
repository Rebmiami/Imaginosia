﻿using Imaginosia.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public class Player : Entity
	{
		public Player()
		{
			dimensions = PositionHelper.ToGamePosition(new Vector2(16, 18), true);

			inventory = new Item[] { new Item() { itemID = ItemType.Gun }, new Item() { itemID = ItemType.Knife }, new Item() { itemID = ItemType.Axe }, new Item() { itemID = ItemType.Matchbox }, null, null, null, null, null };
			HeldItem = inventory[0];

			foreach (var item in inventory)
			{
				if (item != null)
				item.SetDefaults();
			}

			health = MaxHealth;
		}

		public Item[] inventory = new Item[5];
		public int itemSlot = 0;
		public ControllerType controller = ControllerType.Keyboard;
		public PlayerIndex gamepadNumber = PlayerIndex.One;
		public Vector2 direction = Vector2.UnitX;
		public Vector2 gameDirection = Vector2.UnitX;

		public Item HeldItem
		{
			get { return inventory[itemSlot]; }
			set { inventory[itemSlot] = value; }
		}

		public float health;
		public float hunger;
		public float magic;

		public const int MaxHealth = 10;
		public const int MaxHunger = 10;
		public const int MaxMagic = 10;

		public int invFrames;

		public bool equippedBag;

		public int noise;

		public float directionChange;

		public string MouseText;

		public const int MaxTileReach = 4;

		public override void Update()
		{
			bool canInteractTile = Vector2.Distance(GamePosition, MouseHelper.MouseTileHover.ToVector2()) < MaxTileReach;
			MouseText = null;

			noise = 1;

			int inventorySize = 7;

			if (inventory[itemSlot] == null || inventory[itemSlot].useTime == 0)
			{
				int itemSwap = Keybinds.SwapItem(controller, gamepadNumber);
				if (Math.Abs(itemSwap) >= 1)
				{
					itemSlot += itemSwap;
					itemSlot %= inventorySize;
					if (itemSlot < 0)
					{
						itemSlot = inventorySize - 1;
					}
				}
			}

			velocity *= 0.8f;



			float speed = 0.2f;

			
			if (KeyHelper.Down(Keys.W) && !KeyHelper.Down(Keys.S))
			{
				velocity.Y = Math.Max(-1, velocity.Y - 1);
			}
			else if (KeyHelper.Down(Keys.S) && !KeyHelper.Down(Keys.W))
			{
				velocity.Y = Math.Min(1, velocity.Y + 1);
			}

			if (KeyHelper.Down(Keys.A) && !KeyHelper.Down(Keys.D))
			{
				velocity.X = Math.Max(-1, velocity.X - 1);
			}
			else if (KeyHelper.Down(Keys.D) && !KeyHelper.Down(Keys.A))
			{
				velocity.X = Math.Min(1, velocity.X + 1);
			}

			if (velocity.Length() > speed)
			velocity = Vector2.Normalize(velocity) * speed;

			noise += (int)(velocity.Length() * 50);

			Vector2 oldDirection = gameDirection;
			direction = Vector2.Normalize(MouseHelper.Position - ScreenPosition);
			gameDirection = Vector2.Normalize(PositionHelper.ToGamePosition(direction, true));
			directionChange = -Vector2.Dot(gameDirection, oldDirection) + 1;

			foreach (Item item in inventory)
			{
				item?.Update();
			}

			WorldTile focusTile = Game1.gamestate.world.tiles[MouseHelper.MouseTileHover.X, MouseHelper.MouseTileHover.Y];

			if (canInteractTile && focusTile.floorItem != null)
			{
				MouseText = focusTile.floorItem.GetName();
			}


			if (KeyHelper.Down(Keys.LeftShift) || KeyHelper.Down(Keys.RightShift))
			{
				if (itemSlot <= 3)
				{
					MouseText = "Cannot drop this item";
				}
				else
				{
					if (canInteractTile)
					{
						if (inventory[itemSlot] != null && focusTile.floorItem != null && inventory[itemSlot].itemID == focusTile.floorItem.itemID)
						{
							MouseText = "Right click to take " + focusTile.floorItem.GetName() + "/Click to drop " + inventory[itemSlot].GetName();

							if (MouseHelper.Pressed(MouseButton.Left))
							{
								Item.MergeStacks(ref focusTile.floorItem, ref inventory[itemSlot]);
							}
							if (MouseHelper.Pressed(MouseButton.Right))
							{
								Item.MergeStacks(ref inventory[itemSlot], ref focusTile.floorItem);
							}
						}
						else
						{
							if (inventory[itemSlot] != null)
							{
								MouseText = "Click to drop " + inventory[itemSlot].GetName();
								if (MouseHelper.Pressed(MouseButton.Left))
									focusTile.PlaceItem(ref inventory[itemSlot]);
							}
							if (inventory[itemSlot] == null && focusTile.floorItem != null)
							{
								MouseText = "Right click to take " + focusTile.floorItem.GetName();
								if (MouseHelper.Pressed(MouseButton.Right))
									inventory[itemSlot] = focusTile.TakeItem();
							}
						}
					}
					else
					{
						MouseText = "Cannot reach";
					}
				}
			}
			else
			{
				if (focusTile.floorObjectType == FloorObjectType.Tree && HeldItem != null && HeldItem.itemID == ItemType.Axe)
				{
					if (canInteractTile)
					{
						MouseText = "Click to chop tree";
						if (MouseHelper.Pressed(MouseButton.Left))
							focusTile.Damage(MouseHelper.MouseTileHover);
					}
					else
					{
						MouseText = "Cannot reach";
					}
				}
					




				if (MouseHelper.Pressed(MouseButton.Right) && inventory[0].CanUseItem())
				{
					inventory[0].UseItem();
					noise += 1000;
					new Projectile(position, gameDirection, 10, 0)
					{
						updateRes = 4,
						timeLeft = 16,
						knockback = 1
					};
				}
			}


			if (HeldItem != null && HeldItem.CanUseItem())
			{
				if (HeldItem.usesLeft > 0 && Keybinds.UseItem(controller, gamepadNumber))
				{
					// inventory[itemSlot].UseItem(GetFoot() + new Vector2(0, -32), direction, this);
				}
			}

			if (invFrames > 0)
			{
				invFrames--;
			}

			base.Update();
		}

		public void Hit(int damage, Entity source, Vector2 direction)
		{
			if (invFrames > 0)
				return;
			health -= damage;
			// DrawHandler.screenShake += 4;
			invFrames = 90;
			// UIHandler.hitFlash = 10;
			CheckDead();
		}

		public void CheckDead()
		{
			if (health <= 0)
			{
				// Game1.getGame.GameOver();
			}
		}

		public void PickUpItem(Item item)
		{
			if (item is Item newItem)
			{
				for (int i = 0; i < inventory.Length; i++)
				{
					if (inventory[i] == null)
					{
						inventory[i] = newItem;
						itemSlot = i;
						return;
					}
				}
				// new FloorItem(HeldItem, GetFoot());
				HeldItem = newItem;
			}
		}

		public override void Draw(SpriteBatcher spriteBatcher)
		{
			// float rotation = (float)Math.Atan2(direction.Y, direction.X);
			// 
			// float invTransparency = 1;
			// if (invFrames > 0)
			// {
			// 	invTransparency = (float)Math.Sin((double)invFrames * MathHelper.TwoPi / 8);
			// }

			spriteBatcher.Draw(Assets.Tex2["player"].texture, ScreenPosition - new Vector2(8, 16), null, Color.White, 0, dimensions / 2, 1, direction.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

			if (HeldItem != null && HeldItem.itemID == ItemType.Gun)
			{
				float rotation = (float)Math.Atan2(direction.Y, direction.X);
				int holdDir = direction.X > 0 ? 1 : -1;
				SlicedSprite texture = Assets.Tex2["items"];
				if (!HeldItem.CanUseItem())
				{
					rotation += MathHelper.PiOver4 * holdDir;
				}
				spriteBatcher.Draw(texture.texture, ScreenPosition + new Vector2(0, -4) + direction * 4, texture.frames[HeldItem.GetSlice()], Color.White, rotation, texture.frames[0].Size.ToVector2() / 2, 1, holdDir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
			}
		}
	}
}
