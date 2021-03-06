using Imaginosia.Audio;
using Imaginosia.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia.Gameplay
{
	public class Enemy : Entity
	{
		public int type;

		public int health;
		public int maxHealth;

		public int alertness; // How aware the enemy is of the player's location
		public int fear; // Increased by player actions such as making noise, attacking, or using the flashlight
		public int drive; // How persistent the enemy is

		public Vector2 interest; // The position of the enemy's interest, or what it wants to get. May be the player or an item on the ground
		public int attention;

		public Item carryingItem;

		public int hitTimer; // How long to show the "hurt" animation after taking damage
		public int animationTimer; // Used for animations
		public int animFrame;

		public bool dead; // If the enemy is dead, this will be true.

		public int despawnTimer;
		public bool remove = false;

		public EnemyState state;

		public Vector2 direction = Vector2.UnitX;

		public bool spotted;
		public int spotTimer;

		public int soundTimer;

		public int bunnyReloadTimer;

		public virtual void Spawn(Vector2 position, int type)
		{
			this.type = type;
			Game1.gamestate.enemies.Add(this);
			this.position = position;
			this.position = position - (GetFoot() - position);
			fear = 0;
			despawnTimer = 900;
			spotTimer = 40;
			soundTimer = 60 + RNG.rand.Next(30);
			hitboxOffset = new Vector2(0, -1);

			SetDefaults();
		}

		public void SetDefaults()
		{
			dimensions = PositionHelper.ToGamePosition(new Vector2(16, 18), true);
			switch (type)
			{
				case 0:
					maxHealth = 10;
					drive = 200;
					state = EnemyState.Wander;
					break;
				case 1:
					maxHealth = 5;
					drive = 100;
					state = EnemyState.Wander;
					break;
				case 2:
					maxHealth = 20;
					drive = 300;
					state = EnemyState.Sleep;
					break;
				default:
					break;
			}
			health = maxHealth;
		}

		public override void Update()
		{
			// TODO: The following actions should scare enemies:
			// - Moving in their direction
			// - Taking damage
			// - Shining the flashlight

			// Calculate fear and alertness changes:
			if (spotted && spotTimer > 0)
			{
				spotTimer--;
			}

			EnemyState actingState = state;

			if (ImaginationHandler.IsImagination && type != 2)
			{
				actingState = EnemyState.Wander;
			}

			float playerDistance = Vector2.Distance(position, Game1.gamestate.player.position);

			if (playerDistance < 6)
			{
				spotted = true;
			}

			WorldTile standTile = Game1.gamestate.world.tiles[Math.Clamp((int)Center.X, 0, World.WorldWidth - 1), Math.Clamp((int)Center.Y, 0, World.WorldHeight - 1)];

			if (standTile.floorObjectType == FloorObjectType.BoneTrap)
			{
				if (ImaginationHandler.IsImagination)
				{
					standTile.floorObjectType = FloorObjectType.None;

					foreach (var item in Game1.gamestate.enemies)
					{
						if (Vector2.Distance(Vector2.Floor(Center) + new Vector2(0.5f), item.Center) < 5f)
						{
							item.TakeDamage(20, Vector2.Normalize(item.Center - (Vector2.Floor(Center) + new Vector2(0.5f))));

							DustManager.CreateDustPuff(new FloatRectangle(Vector2.Floor(Center), Vector2.One), Vector2.Zero, 1, 10, 30);
						}
					}
				}
				else
				{
					if (RNG.rand.Next(10) == 0 && hitTimer == 0)
					{
						TakeDamage(1, Vector2.Zero); 
						if (RNG.rand.Next(10) == 0)
						{
							standTile.floorObjectType = FloorObjectType.None;
						}
					}
				}
			}


			float speed = 0.1f;

			if (!dead && hitTimer == 0)
			{
				if (!ImaginationHandler.IsImagination)
				{

					alertness += (int)(Math.Min(1 / playerDistance, 1) * Game1.gamestate.player.noise * 4);
					fear += (int)(Math.Min(1 / playerDistance, 1) * Game1.gamestate.player.noise * (5f / health));

					float velocityDot = Vector2.Dot(Vector2.Normalize(Game1.gamestate.player.velocity), Vector2.Normalize(ScreenPosition - Game1.gamestate.player.ScreenPosition));

					float flashlightDot = Vector2.Dot(Game1.gamestate.player.direction, Vector2.Normalize(ScreenPosition - Game1.gamestate.player.ScreenPosition));

					if (flashlightDot > 0.95f - Game1.gamestate.player.directionChange * 0.1f)
					{
						spotted = true;
						alertness += (int)((10 + Game1.gamestate.player.directionChange * 10) * (1 / (playerDistance * 0.1f)));
						fear += (int)(Game1.gamestate.player.directionChange * 500 * (1 / (playerDistance * 0.1f)) * (5f / health));
					}

					alertness += (int)(1 / (playerDistance / 5));

					if (velocityDot > 0)
						fear += (int)(velocityDot * 100 * Game1.gamestate.player.velocity.Length() * (alertness / 500f) * (1 / playerDistance) * (5f / health));

					alertness = Math.Clamp((int)(alertness * 0.99f), 0, 1000);

					if (state == EnemyState.Flee)
					{
						// Fleeing enemies should stay afraid for longer
						fear = Math.Clamp(fear - 1, 0, 1000);
					}
					else
					{
						fear = Math.Clamp((int)(fear * 0.99f), 0, 1000);
					}

					if (carryingItem != null)
					{
						state = EnemyState.Flee;
						fear = 1000;
					}



					switch (actingState)
					{
						case EnemyState.Sleep:
							if (alertness > 800 && drive > 0)
							{
								state = EnemyState.Attack;
							}

							break;
						case EnemyState.Wander:
							velocity = direction * speed * 0.5f;
							if (attention == 0)
							{
								direction = MathTools.RotateVector(Vector2.UnitX, RNG.rand.NextDouble() * MathHelper.TwoPi);
								attention = RNG.rand.Next(300) + 60;
							}
							if (alertness > 200)
							{
								if (type == 1)
								{
									state = EnemyState.Flee;
									direction *= -1;
									attention = RNG.rand.Next(60) + 30;
								}
								else if (drive > 0)
								{
									GetNewInterest();
									state = EnemyState.Sneak;
									attention = RNG.rand.Next(60) + 60;
								}
							}
							Sniff();

							break;
						case EnemyState.Sneak:
							velocity = direction * speed * 0.3f;
							if ((Vector2.Distance(position, interest) < 8 || alertness > 1000 - drive * 2) && drive > 0)
							{
								GetNewInterest();
								state = EnemyState.Attack;
							}

							if (fear > drive * 5)
							{
								state = EnemyState.Flee;
								direction *= -1;
								attention = RNG.rand.Next(60) + 30;
							}

							if (attention == 0)
							{
								GetNewInterest();
								attention = RNG.rand.Next(60) + 60;
							}

							if (type == 1 && alertness > 400)
							{
								state = EnemyState.Wander;
							}
							else if (drive + alertness < 50)
							{
								state = EnemyState.Wander;
							}


							if (RNG.rand.Next(60) == 1)
							{
								drive--;
							}

							if (standTile.floorItem != null)
							{
								carryingItem = standTile.TakeItem();
								state = EnemyState.Flee;
							}

							break;

						case EnemyState.Attack:
							velocity = direction * speed;

							if (fear > drive * 4)
							{
								state = EnemyState.Flee;
								GetNewInterest();
								direction *= -1;
								attention = RNG.rand.Next(60) + 30;
							}

							if (attention == 0)
							{
								GetNewInterest();
								attention = RNG.rand.Next(60) + 60;
							}

							if (RNG.rand.Next(30) == 1)
							{
								drive--;
							}

							if (standTile.floorItem != null)
							{
								carryingItem = standTile.TakeItem();
								state = EnemyState.Flee;
							}

							if (Game1.gamestate.player.Hitbox.Intersects(Hitbox))
							{
								int damage = 1;
								if (type == 2)
									damage = 2;

								Game1.gamestate.player.Hit(damage, this, velocity);
							}


							break;

						case EnemyState.Flee:
							velocity = direction * speed * 1.5f;

							if (fear < 100)
							{
								if (type == 2)
								{
									state = EnemyState.Sleep;
								}
								else if (type == 0)
								{
									state = EnemyState.Wander;
								}
							}

							if (attention == 0 && alertness > 200)
							{
								GetNewInterest();
								direction *= -1;
								attention = RNG.rand.Next(60) + 60;
							}

							if (velocityDot > 0)
							{
								attention--;
							}

							if (RNG.rand.Next(15) == 1)
							{
								drive--;
							}

							// Scared rats should still steal
							if (carryingItem == null && type == 1 && standTile.floorItem != null)
							{
								carryingItem = standTile.TakeItem();
								state = EnemyState.Flee;
							}

							break;

						default:
							break;
					}
				}
				else
				{
					switch (state)
					{
						case EnemyState.Sleep:
							if (playerDistance < 8)
							{
								state = EnemyState.Attack;
							}

							break;
						case EnemyState.Wander:
							velocity = direction * speed * 0.5f;

							if (playerDistance < ImaginationHandler.TimeSinceSwitched / 60f + 10)
							{
								GetNewInterest();
								state = EnemyState.Attack;
								attention = RNG.rand.Next(60) + 60;
								bunnyReloadTimer = 240;
							}

							break;
						case EnemyState.Sneak:
							GetNewInterest();
							state = EnemyState.Attack;

							break;

						case EnemyState.Attack:
							velocity = direction * speed;

							if (type == 1)
							{
								if (spotted)
								{
									if (bunnyReloadTimer < 180)
									{
										DustManager.CreateDustPuff(Hitbox, Vector2.Zero, 0.1f, 2, 1);
									}

									if (bunnyReloadTimer > 0)
									{
										bunnyReloadTimer--;
									}
									else if (Vector2.Distance(Center, Game1.gamestate.player.Center) < 6f)
									{
										bunnyReloadTimer = 360;
										int damage = 3;
										Game1.gamestate.player.Hit(damage, this, velocity);
										SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "ratLaserImaginary", true);


										for (int i = 0; i < Vector2.Distance(Center, Game1.gamestate.player.Center) * 8; i++)
										{
											DustManager.NewDust(Vector2.Lerp(Center, Game1.gamestate.player.Center, (float)RNG.rand.NextDouble()), Vector2.Zero, 0.05f, 3);
										}
									}
								}
							}
							else
							{
								if (Game1.gamestate.player.Hitbox.Intersects(Hitbox))
								{
									int damage = 1;
									if (type == 2)
										damage = 2;

									Game1.gamestate.player.Hit(damage, this, velocity);
								}
							}

							break;

						case EnemyState.Flee:
							GetNewInterest();
							state = EnemyState.Attack;
							break;

						default:
							break;
					}

					if (attention == 0)
					{
						GetNewInterest();
						attention = RNG.rand.Next(60) + 60;
					}
				}
			}

			if (attention > 0)
			{
				attention--;
			}

			if (dead)
			{
				if (animationTimer > 0)
				{
					animationTimer--;
				}
			}
			else
			{
				if (state != EnemyState.Sleep)
				{
					animationTimer++;

					animFrame = animationTimer / 10 % 2 + 1;

					if (direction.Y < 0)
					{
						animFrame += 2;
					}
				}
				else
				{
					animFrame = 0;
				}
			}

			velocity *= 0.8f;

			if (hitTimer > 0)
			{
				hitTimer--;
				animFrame = 5;
			}

			if (dead)
			{
				animFrame = 6;
			}

			if (!dead && Vector2.Distance(position, Game1.gamestate.player.position) < 40)
			{
				despawnTimer = 900;
			}

			if (despawnTimer > 0)
			{
				despawnTimer--;
				if (despawnTimer <= 0)
				{
					remove = true;
				}
			}

			if (ImaginationHandler.IsImagination && dead && animationTimer == 0)
			{
				Assets.Sfx["enemyPopImaginary"].Play();
				remove = true;
				Game1.gamestate.world.PlaceItemNearest(Center.ToPoint(), ref carryingItem);

				Item item = new Item();

				if (type == 0)
				{
					item.itemID = ItemType.MeatRaw;
				}
				if (type == 1)
				{
					item.itemID = ItemType.Bone;
				}
				if (type == 2)
				{
					item.itemID = ItemType.Fur;
				}
				item.SetDefaults();
				item.stackCount = 1;

				// Bears should always drop an item in the imagination because they're harder to find
				if (RNG.rand.Next(3) == 0 || type == 2)
				Game1.gamestate.world.PlaceItemNearest(Center.ToPoint(), ref item);
			}

			if (soundTimer > 0)
			{
				soundTimer--;
			}
			else if (!dead)
			{
				if (ImaginationHandler.IsImagination)
				{
					switch (type)
					{
						case 0:
							SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "wolfPingImaginary");
							soundTimer = 20 + (int)(Vector2.Distance(Game1.gamestate.player.Center, Center) * 10);
							break;
						case 1:
							SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "ratLaughImaginary", true);
							soundTimer = 240 + RNG.rand.Next(60);
							break;
						case 2:
							break;
					}
				}
				else
				{
					switch (type)
					{
						case 0:
							switch (state)
							{
								case EnemyState.Wander:
									break;
								case EnemyState.Sneak:
									SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "wolfGrowlReal", true);
									soundTimer = 240 + RNG.rand.Next(60);
									break;
								case EnemyState.Attack:
									SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "wolfBarkReal", true);
									soundTimer = 120 + RNG.rand.Next(60);
									break;
								case EnemyState.Flee:
									break;
								default:
									break;
							}
							break;
						case 1:
							break;
						case 2:
							switch (state)
							{
								case EnemyState.Sleep:
									if (alertness > 100)
									{
										SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "bearStirReal", true);
									}
									else
									{
										SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "bearSleepReal", true);
									}
									soundTimer = 240;

									break;
								case EnemyState.Wander:
									break;
								case EnemyState.Sneak:
									break;
								case EnemyState.Attack:
									break;
								case EnemyState.Flee:
									break;
								default:
									break;
							}

							break;
					}
				}
			}

			base.Update();
		}

		public void GetNewInterest()
		{
			if (ImaginationHandler.IsImagination)
			{
				interest = Game1.gamestate.player.position + MathTools.RotateVector(Vector2.UnitX * 1, RNG.rand.NextDouble() * MathHelper.TwoPi);
				direction = Vector2.Normalize(interest - Center);
			}
			else
			{
				if (new Rectangle(0, 0, World.WorldWidth, World.WorldHeight).Contains(interest) && Game1.gamestate.world.tiles[(int)interest.X, (int)interest.Y].floorItem != null)
				{
					return;
				}
				else if (type == 1 && state != EnemyState.Wander)
				{
					state = EnemyState.Flee;
				}

				interest = Game1.gamestate.player.position + MathTools.RotateVector(Vector2.UnitX * (1 - alertness) * 0.005f, RNG.rand.NextDouble() * MathHelper.TwoPi);
				direction = Vector2.Normalize(interest - Center);
			}
		}

		public virtual void TakeDamage(int damage, Vector2 knockback)
		{
			damage = (int)(damage * (RNG.rand.NextDouble() + 0.5f));
			hitTimer = 6;
			health -= damage;
			velocity += knockback;
			bunnyReloadTimer = 360;
			fear += damage * 50;
			if (ImaginationHandler.IsImagination)
			{
				SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "enemyHurtImaginary", true);
			}
			else
			{
			}
			if (health <= 0)
			{
				dead = true;
				animationTimer = 60;
				despawnTimer = 3600;
				if (ImaginationHandler.IsImagination)
				{
					Game1.gamestate.player.magic += 0.5f;
				}

				if (!ImaginationHandler.IsImagination)
				{
					switch (type)
					{
						case 0:
							SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "wolfDeathReal", true);
							break;
						case 1:
							SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "ratDeathReal", true);
							break;
						case 2: 
							break;
					}
				}
			}
			else
			{

				if (!ImaginationHandler.IsImagination)
				{
					switch (type)
					{
						case 0:
							SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "wolfWhimperReal", true);
							break;
						case 1:
							// SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "ratDeathReal", true);
							break;
						case 2:
							SoundSystem.PlayAtPosition(Game1.gamestate.player.Center, Center, "bearGruntReal", true);
							break;
					}
				}
			}
		}

		public void Sniff()
		{
			if (carryingItem != null)
				return;

			Vector2 sniffTarget = position + MathTools.RotateVector(new Vector2((float)RNG.rand.NextDouble() * 30), RNG.rand.NextDouble() * MathHelper.TwoPi);
			if (new Rectangle(0, 0, World.WorldWidth, World.WorldHeight).Contains(sniffTarget))
			{
				WorldTile toSniff = Game1.gamestate.world.tiles[(int)sniffTarget.X, (int)sniffTarget.Y];

				if (toSniff.floorItem != null)
				{
					Item item = toSniff.floorItem;


					if (item.itemID == ItemType.MeatRaw || item.itemID == ItemType.MeatCooked || RNG.rand.Next(3) == 0)
					{
						interest = sniffTarget;
						state = EnemyState.Sneak;
					}
				}
			}
		}

		public override void Draw(SpriteBatcher spriteBatcher)
		{

			if (ImaginationHandler.IsImagination && !spotted)
			{
				return;
			}

			Color color;

			color = Color.Lerp(Color.White, Color.Black, spotTimer / 40f);
			if (ImaginationHandler.IsImagination)
			{
				color = Color.Lerp(color, Color.Transparent, spotTimer / 40f);
			}


			SlicedSprite texture;

			if (ImaginationHandler.IsImagination)
			{
				texture = type switch
				{
					0 => Assets.Tex2["wolfImaginary"],
					1 => Assets.Tex2["ratImaginary"],
					2 => Assets.Tex2["bearImaginary"],
					_ => throw new InvalidOperationException("No enemy with this texture"),
				};
			}
			else
			{
				texture = type switch
				{
					0 => Assets.Tex2["wolfReal"],
					1 => Assets.Tex2["ratReal"],
					2 => Assets.Tex2["bearReal"],
					_ => throw new InvalidOperationException("No enemy with this texture"),
				};
			}

			spriteBatcher.Draw(texture.texture, ScreenPosition - new Vector2(8, 16), texture.frames[animFrame], color, 0, dimensions / 2, 1, direction.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}

		public override void OnTouchFence(Point point)
		{
			// Bears can destroy wooden fences to get at your stuff
			if (type == 2 && RNG.rand.Next(40) == 0)
			{
				Game1.gamestate.world.tiles[point.X, point.Y].Damage(point, false);
			}

			base.OnTouchFence(point);
		}
	}
}
