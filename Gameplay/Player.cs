using Imaginosia.Audio;
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
			position = new Vector2(50) + MathTools.RotateVector(new Vector2(20, 0), RNG.rand.NextDouble() * MathHelper.TwoPi);
			dimensions = PositionHelper.ToGamePosition(new Vector2(16, 18), true);

			inventory = new Item[] { new Item() { itemID = ItemType.Gun }, new Item() { itemID = ItemType.Knife }, new Item() { itemID = ItemType.Axe }, new Item() { itemID = ItemType.Matchbox }, null, null, null, null, null };
			HeldItem = inventory[0];

			foreach (var item in inventory)
			{
				if (item != null)
				item.SetDefaults();
			}

			health = MaxHealth;
			hunger = MaxHunger;
			magic = MaxMagic;
			magicRechargeTimer = 0;
			hitboxOffset = new Vector2(-0.5f, -1);
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

		public int walkTimer;

		public int clothing;

		public int magicRechargeTimer;

		public int hallucinogen;

		public override void Update()
		{
			bool canUseKnife = false;

			bool canInteractTile = Vector2.Distance(GamePosition, MouseHelper.MouseTileHover.ToVector2()) < MaxTileReach;
			MouseText = null;

			noise = 1;

			int inventorySize = 7;

			if (equippedBag)
				inventorySize += 2;

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
					Assets.Sfx["menuClick"].Play(0.1f, 0, 0);
				}
			}

			velocity *= 0.8f;

			float speed = 0.1f;
			if (KeyHelper.Down(Keys.LeftShift) || KeyHelper.Down(Keys.RightShift))
			{
				speed = 0.05f;
			}
			if ((KeyHelper.Down(Keys.LeftControl) || KeyHelper.Down(Keys.RightControl)) && hunger > 2.5f)
			{
				speed = 0.2f;
			}


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

			if (velocity.Length() > 0.01f)
			{
				walkTimer++;

				if (walkTimer % 30 == 29)
				{
					Assets.Sfx["footstepReal"].Play(0.4f, (float)(RNG.rand.NextDouble() - 0.5f) * 0.3f, 0);
				}
			}
			else
			{
				walkTimer = 0;
			}

			Vector2 oldDirection = gameDirection;
			direction = Vector2.Normalize(MouseHelper.Position - ScreenPosition);
			gameDirection = Vector2.Normalize(PositionHelper.ToGamePosition(direction, true));
			directionChange = -Vector2.Dot(gameDirection, oldDirection) + 1;

			foreach (Item item in inventory)
			{
				item?.Update();
			}

			WorldTile hoverTile = Game1.gamestate.world.tiles[MouseHelper.MouseTileHover.X, MouseHelper.MouseTileHover.Y];

			if (canInteractTile && hoverTile.floorItem != null)
			{
				MouseText = hoverTile.floorItem.GetName();
			}

			// This has got to be the single worst block of code I have ever written in my life
			// Reusing code is for dummies
			if (KeyHelper.Down(Keys.LeftShift) || KeyHelper.Down(Keys.RightShift))
			{
				if (canInteractTile)
				{
					if (inventory[itemSlot] != null && hoverTile.floorItem != null && inventory[itemSlot].itemID == hoverTile.floorItem.itemID)
					{
						MouseText = "Right click to take " + hoverTile.floorItem.GetName() + "/Click to drop " + inventory[itemSlot].GetName();

						if (MouseHelper.Pressed(MouseButton.Left))
						{
							Item.MergeStacks(ref hoverTile.floorItem, ref inventory[itemSlot]);
						}
						if (MouseHelper.Pressed(MouseButton.Right))
						{
							Item.MergeStacks(ref inventory[itemSlot], ref hoverTile.floorItem);
						}
						goto SingleAction;
					}
					else
					{
						int firstOpenSlot = 3;
						for (int i = firstOpenSlot; i < inventorySize; i++)
						{
							if (inventory[i] != null && hoverTile.floorItem != null && inventory[i].itemID == hoverTile.floorItem.itemID && inventory[i].stackable && inventory[i].stackCount < inventory[i].maxStack)
							{
								break;
							}
							firstOpenSlot++;
						}

						if (firstOpenSlot >= inventorySize)
						{
							firstOpenSlot = 3;
							for (int i = firstOpenSlot; i < inventorySize; i++)
							{
								if (inventory[i] == null)
								{
									break;
								}
								firstOpenSlot++;
							}
						}


						if (hoverTile.floorItem != null && firstOpenSlot < inventorySize)
						{
							MouseText = "Right click to take " + hoverTile.floorItem.GetName();
							if (MouseHelper.Pressed(MouseButton.Right))
							{
								if (inventory[firstOpenSlot] == null)
								{
									inventory[firstOpenSlot] = hoverTile.TakeItem();
								}
								else
								{
									Item.MergeStacks(ref inventory[firstOpenSlot], ref hoverTile.floorItem);
								}
							}
							goto SingleAction;
						}
						if (inventory[itemSlot] != null)
						{
							if (itemSlot <= 3)
							{
								MouseText = "Cannot drop this item";
								goto SingleAction;
							}

							MouseText = "Click to drop " + inventory[itemSlot].GetName();
							if (MouseHelper.Pressed(MouseButton.Left))
								hoverTile.PlaceItem(ref inventory[itemSlot]);
							goto SingleAction;
						}
					}
				}
				else
				{
					MouseText = "Cannot reach";
					goto SingleAction;
				}

				SingleAction:;
			}
			else
			{
				if (!ImaginationHandler.IsImagination && canInteractTile && inventory[1].CanUseItem())
				{
					Enemy toScavenge = null;
					foreach (var item in Game1.gamestate.enemies)
					{
						if (item.dead && item.Hitbox.Contains(PositionHelper.ToGamePosition(MouseHelper.Position)))
						{
							toScavenge = item;
						}
					}

					if (toScavenge != null)
					{
						MouseText = "Click to scavenge";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							SoundSystem.PlayAtPosition(Center, toScavenge.Center, "enemyHurtReal", true);

							inventory[1].UseItem();
							toScavenge.remove = true;
							Game1.gamestate.world.PlaceItemNearest(toScavenge.Center.ToPoint(), ref toScavenge.carryingItem);

							List<Item> potentialDrops = new List<Item>();

							int items = 0;

							if (toScavenge.type == 0)
							{
								Item item1 = new Item();
								item1.itemID = ItemType.MeatRaw;
								item1.SetDefaults();
								item1.stackCount = RNG.rand.Next(2) + 1;

								Item item2 = new Item();
								item2.itemID = ItemType.Fur;
								item2.SetDefaults();
								item2.stackCount = RNG.rand.Next(2) + 1;

								Item item3 = new Item();
								item3.itemID = ItemType.Bone;
								item3.SetDefaults();
								item3.stackCount = RNG.rand.Next(2) + 1;

								potentialDrops = new List<Item> { item1, item2, item3 };
								items = 2;
							}
							if (toScavenge.type == 1)
							{
								Item item1 = new Item();
								item1.itemID = ItemType.MeatRaw;
								item1.SetDefaults();
								item1.stackCount = 1;

								Item item2 = new Item();
								item2.itemID = ItemType.Fur;
								item2.SetDefaults();
								item2.stackCount = 1;

								Item item3 = new Item();
								item3.itemID = ItemType.Bone;
								item3.SetDefaults();
								item3.stackCount = 1;

								potentialDrops = new List<Item> { item1, item2, item3 };
								items = 1;
							}
							if (toScavenge.type == 2)
							{
								Item item1 = new Item();
								item1.itemID = ItemType.MeatRaw;
								item1.SetDefaults();
								item1.stackCount = RNG.rand.Next(3) + 2;

								Item item2 = new Item();
								item2.itemID = ItemType.Fur;
								item2.SetDefaults();
								item2.stackCount = RNG.rand.Next(3) + 2;

								Item item3 = new Item();
								item3.itemID = ItemType.Bone;
								item3.SetDefaults();
								item3.stackCount = RNG.rand.Next(3) + 2;

								potentialDrops = new List<Item> { item1, item2, item3 };
								items = 3;
							}

							for (int i = 0; i < items; i++)
							{
								int j = RNG.rand.Next(potentialDrops.Count);

								Item toDrop = potentialDrops[j];
								Game1.gamestate.world.PlaceItemNearest(toScavenge.Center.ToPoint(), ref toDrop);
								potentialDrops.RemoveAt(j);
							}
						}
						goto SingleAction;
					}
				}

				if (hoverTile.floorObjectType == FloorObjectType.Mushroom)
				{
					if (canInteractTile)
					{
						MouseText = "Click to obtain hallucinogen";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							hoverTile.floorObjectType = FloorObjectType.None;
							hallucinogen += 3;
							Assets.Sfx["mushroom"].Play();
						}
					}
					else
					{
						MouseText = "Cannot reach";
					}
					goto SingleAction;
				}

				if (hoverTile.floorObjectType == FloorObjectType.Tree && HeldItem != null && HeldItem.itemID == ItemType.Axe && HeldItem.CanUseItem())
				{
					if (canInteractTile)
					{
						MouseText = "Click to chop tree";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							HeldItem.UseItem();
							hoverTile.Damage(MouseHelper.MouseTileHover);
						}
					}
					else
					{
						MouseText = "Cannot reach";
					}
					goto SingleAction;
				}

				if (hoverTile.floorObjectType == FloorObjectType.Fence && HeldItem != null && HeldItem.itemID == ItemType.Axe && HeldItem.CanUseItem())
				{
					if (canInteractTile)
					{
						MouseText = "Click to chop fence";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							HeldItem.UseItem();
							hoverTile.Damage(MouseHelper.MouseTileHover);
						}
					}
					else
					{
						MouseText = "Cannot reach";
					}
					goto SingleAction;
				}

				if (HeldItem != null)
				{
					if (canInteractTile)
					{
						if (HeldItem.itemID == ItemType.Matchbox && (hoverTile.floorObjectType == FloorObjectType.Tree || (hoverTile.floorItem != null && (hoverTile.floorItem.itemID == ItemType.Wood || hoverTile.floorItem.itemID == ItemType.WoodStake))))
						{
							if (HeldItem.usesLeft > 0)
							{
								MouseText = "Click to start a fire";
								if (MouseHelper.Pressed(MouseButton.Left))
								{
									HeldItem.UseItem();
									hoverTile.Ignite(MouseHelper.MouseTileHover);
								}
								goto SingleAction;
							}
							else
							{
								MouseText = "No matches left";
							}
						}
					}
					if (canInteractTile)
					{
						if ((HeldItem.itemID == ItemType.Wood || HeldItem.itemID == ItemType.WoodStake) && (hoverTile.floorObjectType == FloorObjectType.Campfire))
						{
							MouseText = "Click to fuel the fire";
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								if (HeldItem.stackCount <= 0)
								{
									HeldItem = null;
								}
								hoverTile.floorObjectHealth += 400;
							}
							goto SingleAction;
						}
					}
				}

				if (hoverTile.floorItem != null && HeldItem != null && HeldItem.CanUseItem())
				{
					if (canInteractTile)
					{
						if (HeldItem.itemID == ItemType.Matchbox && (hoverTile.floorObjectType == FloorObjectType.Tree || hoverTile.floorItem.itemID == ItemType.Wood || hoverTile.floorItem.itemID == ItemType.WoodStake))
						{
							Item item = new Item();
							item.itemID = ItemType.Clothes;
							item.SetDefaults();

							MouseText = "Click to make " + item.GetName();
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								hoverTile.floorItem.stackCount -= 5;
								if (hoverTile.floorItem.stackCount <= 0)
								{
									hoverTile.floorItem = null;
								}
								Game1.gamestate.world.PlaceItemNearest(MouseHelper.MouseTileHover, ref item);
							}
							goto SingleAction;
						}


						if (HeldItem.itemID == ItemType.Knife && hoverTile.floorItem.itemID == ItemType.Fur && hoverTile.floorItem.stackCount >= 5)
						{
							Item item = new Item();
							item.itemID = ItemType.Clothes;
							item.SetDefaults();

							MouseText = "Click to make " + item.GetName();
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								hoverTile.floorItem.stackCount -= 5;
								if (hoverTile.floorItem.stackCount <= 0)
								{
									hoverTile.floorItem = null;
								}
								Game1.gamestate.world.PlaceItemNearest(MouseHelper.MouseTileHover, ref item);
							}
							goto SingleAction;
						}
						if (HeldItem.itemID == ItemType.BoneKnife && hoverTile.floorItem.itemID == ItemType.Fur && hoverTile.floorItem.stackCount >= 8)
						{
							Item item = new Item();
							item.itemID = ItemType.Bag;
							item.SetDefaults();

							MouseText = "Click to make " + item.GetName();
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								hoverTile.floorItem.stackCount -= 8;
								if (hoverTile.floorItem.stackCount <= 0)
								{
									hoverTile.floorItem = null;
								}
								Game1.gamestate.world.PlaceItemNearest(MouseHelper.MouseTileHover, ref item);
							}
							goto SingleAction;
						}
						if (HeldItem.itemID == ItemType.Knife && hoverTile.floorItem.itemID == ItemType.Bone)
						{
							Item item = new Item();
							item.itemID = ItemType.BoneKnife;
							item.stackCount = 2;
							item.SetDefaults();

							MouseText = "Click to make " + item.GetName();
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								hoverTile.floorItem.stackCount--;
								if (hoverTile.floorItem.stackCount <= 0)
								{
									hoverTile.floorItem = null;
								}
								Game1.gamestate.world.PlaceItemNearest(MouseHelper.MouseTileHover, ref item);
							}
							goto SingleAction;
						}
						if (HeldItem.itemID == ItemType.Axe && hoverTile.floorItem.itemID == ItemType.Bone)
						{
							Item item = new Item();
							item.itemID = ItemType.BoneTrap;
							item.stackCount = 1;
							item.SetDefaults();

							MouseText = "Click to make " + item.GetName();
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								hoverTile.floorItem.stackCount--;
								if (hoverTile.floorItem.stackCount <= 0)
								{
									hoverTile.floorItem = null;
								}
								Game1.gamestate.world.PlaceItemNearest(MouseHelper.MouseTileHover, ref item);
							}
							goto SingleAction;
						}
						if (HeldItem.itemID == ItemType.Axe && hoverTile.floorItem.itemID == ItemType.Wood)
						{
							Item item = new Item();
							item.itemID = ItemType.WoodStake;
							item.SetDefaults();

							MouseText = "Click to make " + item.GetName();
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								hoverTile.floorItem.stackCount--;
								if (hoverTile.floorItem.stackCount <= 0)
								{
									hoverTile.floorItem = null;
								}
								Game1.gamestate.world.PlaceItemNearest(MouseHelper.MouseTileHover, ref item);
							}
							goto SingleAction;
						}
					}
					else
					{
						MouseText = "Cannot reach";
					}
					goto SingleAction;
				}

				if (HeldItem != null)
				{
					if (HeldItem.itemID == ItemType.MeatRaw)
					{
						MouseText = "Click to eat";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							HeldItem.UseItem();
							if (HeldItem.stackCount <= 0)
							{
								HeldItem = null;
							}
						}
						goto SingleAction;
					}
					else if (HeldItem.itemID == ItemType.MeatCooked)
					{
						MouseText = "Click to eat";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							HeldItem.UseItem();
							if (HeldItem.stackCount <= 0)
							{
								HeldItem = null;
							}
						}
						goto SingleAction;
					}
					else if (HeldItem.itemID == ItemType.Bone && ImaginationHandler.IsImagination)
					{
						MouseText = "Click to eat";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							HeldItem.UseItem();
							if (HeldItem.stackCount <= 0)
							{
								HeldItem = null;
							}
						}
						goto SingleAction;
					}
					else if (HeldItem.itemID == ItemType.Bag && !equippedBag)
					{
						MouseText = "Click to equip";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							HeldItem = null;
							equippedBag = true;
						}
						goto SingleAction;
					}
					else if (HeldItem.itemID == ItemType.Clothes)
					{
						MouseText = "Click to equip";
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							HeldItem = null;
							clothing = 20;
						}
						goto SingleAction;
					}
					else if (HeldItem.itemID == ItemType.BoneKnife)
					{
						if (MouseHelper.Pressed(MouseButton.Left))
						{
							HeldItem.UseItem();
							if (HeldItem.stackCount <= 0)
							{
								HeldItem = null;
							}

							if (ImaginationHandler.IsImagination)
							{
								new Projectile(position, gameDirection * 0.4f, 8, 4)
								{
									updateRes = 1,
									timeLeft = 24,
									knockback = 0.5f
								};
							}
							else
							{
								new Projectile(position, gameDirection * 0.6f, 2, 1)
								{
									updateRes = 1,
									timeLeft = 16,
									knockback = 0.5f
								};
							}
						}
						goto SingleAction;
					}
					else if (HeldItem.itemID == ItemType.BoneTrap && hoverTile.floorItem == null && hoverTile.floorObjectType == FloorObjectType.None)
					{
						if (canInteractTile)
						{
							MouseText = "Click to place";
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								if (HeldItem.stackCount <= 0)
								{
									HeldItem = null;
								}

								hoverTile.floorObjectType = FloorObjectType.BoneTrap;
							}
						}
						else
						{
							MouseText = "Cannot reach";
						}

						goto SingleAction;
					}
					else if (HeldItem.itemID == ItemType.WoodStake && hoverTile.floorItem == null && hoverTile.floorObjectType == FloorObjectType.None)
					{
						if (canInteractTile)
						{
							MouseText = "Click to place";
							if (MouseHelper.Pressed(MouseButton.Left))
							{
								HeldItem.UseItem();
								if (HeldItem.stackCount <= 0)
								{
									HeldItem = null;
								}

								hoverTile.floorObjectType = FloorObjectType.Fence;
								hoverTile.floorObjectHealth = 3;
							}
						}
						else
						{
							MouseText = "Cannot reach";
						}
						goto SingleAction;
					}
				}

				canUseKnife = true;

				SingleAction:

				if (MouseHelper.Pressed(MouseButton.Right))
				{
					if ((inventory[0].CanUseItem() && inventory[0].usesLeft > 0 && !ImaginationHandler.IsImagination) || (inventory[0].CanUseItem() && ImaginationHandler.IsImagination))
					{
						inventory[0].UseItem();

						if (ImaginationHandler.IsImagination)
						{
							new Projectile(position, gameDirection * 0.2f, 3, 2)
							{
								updateRes = 1,
								timeLeft = 64,
								knockback = 0.5f
							};
						}
						else
						{
							Assets.Sfx["gunshot"].Play();
							noise += 1000;
							new Projectile(position, gameDirection, 10, 0)
							{
								updateRes = 4,
								timeLeft = 16,
								knockback = 1
							};
							Game1.ScreenShake = 4;
						}
					}
					else
					{
						int firstBoneKnife = 3;
						for (int i = firstBoneKnife; i < inventorySize; i++)
						{
							if (inventory[i] != null && inventory[i].itemID == ItemType.BoneKnife && inventory[i].CanUseItem())
							{
								break;
							}
							firstBoneKnife++;
						}

						if (firstBoneKnife < inventorySize)
						{
							Assets.Sfx["swoosh"].Play(0.3f, 0, 0);
							Item knife = inventory[firstBoneKnife];
							knife.UseItem();
							if (knife.stackCount <= 0)
							{
								inventory[firstBoneKnife] = null;
							}

							if (ImaginationHandler.IsImagination)
							{
								new Projectile(position, gameDirection * 0.4f, 8, 4)
								{
									updateRes = 1,
									timeLeft = 24,
									knockback = 0.5f
								};
							}
							else
							{
								new Projectile(position, gameDirection * 0.6f, 2, 1)
								{
									updateRes = 1,
									timeLeft = 16,
									knockback = 0.5f
								};
							}
						}
					}
				}


				if (MouseHelper.Pressed(MouseButton.Left) && inventory[1].CanUseItem() && canUseKnife)
				{
					Assets.Sfx["swoosh"].Play(0.3f, 0, 0);
					inventory[1].UseItem();
					if (ImaginationHandler.IsImagination && health == 10)
					{
						new Projectile(position, gameDirection * 0.2f, 2, 3)
						{
							updateRes = 1,
							timeLeft = 64,
							knockback = 0.5f
						};
					}
				}

			}

			if (KeyHelper.Pressed(Keys.C))
			{
				int firstConsumable = 3;
				for (int i = firstConsumable; i < inventorySize; i++)
				{
					if (inventory[i] != null && (inventory[i].itemID == ItemType.MeatCooked || inventory[i].itemID == ItemType.MeatRaw))
					{
						if (ImaginationHandler.IsImagination)
						{
							if (magic < MaxMagic)
								break;
						}
						else
						{
							break;
						}
					}
					if (inventory[i] != null && inventory[i].itemID == ItemType.Bone && ImaginationHandler.IsImagination)
					{
						if (ImaginationHandler.IsImagination)
						{
							if (health < MaxHealth)
								break;
						}
						else
						{
							break;
						}
					}
					firstConsumable++;
				}

				if (firstConsumable < inventorySize)
				{
					Item consumable = inventory[firstConsumable];
					consumable.UseItem();
					if (consumable.stackCount <= 0)
					{
						inventory[firstConsumable] = null;
					}
				}
			}

			if (inventory[1].useTime > 0)
			{
				FloatRectangle hitbox = new FloatRectangle(0, 0, 1, 1);
				hitbox.Location = Center - new Vector2(0.5f);
				hitbox.Location += direction * 1.2f;
				foreach (var item in Game1.gamestate.enemies)
				{
					if (hitbox.Intersects(item.Hitbox) && !item.dead && item.hitTimer == 0)
					{
						item.TakeDamage(4, direction * 0.5f);
					}
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

			hunger -= 0.0003f + 0.007f * velocity.Length();

			if (health < MaxHealth && hunger > 1)
			{
				float healRate = 1;
				healRate -= (health / MaxHealth) * 0.9f;
				healRate *= hunger / MaxHunger;
				healRate *= 0.8f;

				hunger -= 0.01f * healRate;
				health += 0.02f * healRate;
			}

			if (hunger <= 0)
			{
				health -= 0.01f;
				CheckDead(null);
			}

			if (ImaginationHandler.IsImagination)
			{
				magic += 0.01f * (float)Math.Tanh(magicRechargeTimer / 30f);
				magicRechargeTimer++;
			}

			health = Math.Clamp(health, 0, MaxHealth);
			hunger = Math.Clamp(hunger, 0, MaxHunger);
			magic = Math.Clamp(magic, 0, MaxMagic);

			base.Update();
		}

		public void Hit(float damage, Entity source, Vector2 direction)
		{
			if (invFrames > 0)
				return;

			if (clothing > 0)
			{
				damage /= 2;
				clothing -= (int)(damage + 1);
			}
			Game1.ScreenShake = (int)damage * 3;

			health -= damage;
			// DrawHandler.screenShake += 4;
			invFrames = 30;
			// UIHandler.hitFlash = 10;
			if (ImaginationHandler.IsImagination)
			{
				Assets.Sfx["playerHurtImaginary"].Play();
			}

			CheckDead(source);
		}

		public void CheckDead(Entity source)
		{
			if (health <= 0)
			{
				Assets.Sfx["death"].Play();
				Game1.getGame.GameOver(source);
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

			int baseFrame = 0;

			if (walkTimer % 30 > 15)
			{
				baseFrame++;
			}
			if (direction.Y < 0)
			{
				baseFrame += 2;
			}
			if (ImaginationHandler.IsImagination)
			{
				baseFrame += 12;
			}

			spriteBatcher.Draw(Assets.Tex2["player"].texture, ScreenPosition - new Vector2(8.5f, 8), Assets.Tex2["player"].frames[baseFrame], Color.White, 0, dimensions / 2, 1, direction.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			if (clothing > 0)
			spriteBatcher.Draw(Assets.Tex2["player"].texture, ScreenPosition - new Vector2(8.5f, 8), Assets.Tex2["player"].frames[baseFrame + 4], Color.White, 0, dimensions / 2, 1, direction.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			if (equippedBag)
			spriteBatcher.Draw(Assets.Tex2["player"].texture, ScreenPosition - new Vector2(8.5f, 8), Assets.Tex2["player"].frames[baseFrame + 8], Color.White, 0, dimensions / 2, 1, direction.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

			if ((HeldItem != null && HeldItem.itemID == ItemType.Gun || (inventory[0].useTime > 0)) && !(inventory[1].useTime > 0))
			{
				float rotation = (float)Math.Atan2(direction.Y, direction.X);
				int holdDir = direction.X > 0 ? 1 : -1;
				SlicedSprite texture = Assets.Tex2["items"];
				if (ImaginationHandler.IsImagination)
				{
					rotation += MathHelper.PiOver4 * holdDir;
				}
				else
				{
					if (!inventory[0].CanUseItem())
					{
						rotation -= MathHelper.PiOver4 * holdDir * ((float)inventory[0].useTime / inventory[0].useCooldown);
					}
				}
				spriteBatcher.Draw(texture.texture, ScreenPosition + new Vector2(0, -0) + direction * 16, texture.frames[inventory[0].GetSlice()], Color.White, rotation, texture.frames[0].Size.ToVector2() / 2, 1, holdDir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
			}
			else if (HeldItem != null && HeldItem.itemID == ItemType.Knife || (inventory[1].useTime > 0))
			{
				float rotation = (float)Math.Atan2(direction.Y, direction.X);
				int holdDir = direction.X > 0 ? 1 : -1;
				SlicedSprite texture = Assets.Tex2["items"];
				if (ImaginationHandler.IsImagination)
				{
					rotation += MathHelper.PiOver4 * holdDir;
				}
				else
				{
					rotation += MathHelper.PiOver2 * holdDir;
				}
				spriteBatcher.Draw(texture.texture, ScreenPosition + new Vector2(0, -0) + direction * (8 + inventory[1].useTime), texture.frames[inventory[1].GetSlice()], Color.White, rotation, texture.frames[0].Size.ToVector2() / 2, 1, holdDir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
			}
		}
	}
}
