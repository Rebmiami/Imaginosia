using Imaginosia.Graphics;
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

			health = 10;
			maxHealth = (int)Math.Ceiling(health);
		}

		public Item[] inventory = new Item[5];
		public int itemSlot = 0;
		public ControllerType controller = ControllerType.Keyboard;
		public PlayerIndex gamepadNumber = PlayerIndex.One;
		public Vector2 direction = Vector2.UnitX;

		public Item HeldItem
		{
			get { return inventory[itemSlot]; }
			set { inventory[itemSlot] = value; }
		}

		public float health;
		public int maxHealth;
		public int invFrames;

		public override void Update()
		{
			if (inventory[itemSlot] == null || inventory[itemSlot].useTime == 0)
			{
				int itemSwap = Keybinds.SwapItem(controller, gamepadNumber);
				if (Math.Abs(itemSwap) >= 1)
				{
					itemSlot += itemSwap;
					itemSlot %= 5;
					if (itemSlot < 0)
					{
						itemSlot = 4;
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


			direction = Vector2.Normalize(MouseHelper.Position - ScreenPosition);

			foreach (Item item in inventory)
			{
				item?.Update();
			}

			if (HeldItem != null && HeldItem.CanUseItem())
			{
				if (HeldItem.usesLeft > 0 && Keybinds.UseItem(controller, gamepadNumber))
				{
					// inventory[itemSlot].FireGun(GetFoot() + new Vector2(0, -32), direction, this);
				}
			}

			if (invFrames > 0)
			{
				invFrames--;
			}

			if (HeldItem == null || HeldItem.CanUseItem())
			{
				// FloorItem toPick = null;
				// foreach (FloorItem item in Game1.gameState.items)
				// {
				// 	if (Vector2.Distance(item.GetFoot(), GetFoot()) < 40)
				// 	{
				// 		toPick = item;
				// 		break;
				// 	}
				// }
				// if (toPick != null)
				// {
				// 	toPick.lightUp = true;
				// 	if (Keybinds.PickUpItem(controller, gamepadNumber))
				// 	{
				// 		PickUpItem(toPick.item);
				// 		Game1.gameState.items.Remove(toPick);
				// 	}
				// }
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
