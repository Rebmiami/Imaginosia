using Imaginosia.Gameplay;
using Imaginosia.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Imaginosia
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		internal SpriteBatcher spriteBatch;

		public static Gamestate gamestate;
		public static GameScreen gameScreen;
		public static Stopwatch gameTimer;

		public static int gameStartTimer;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			getGame = this;
			gameScreen = GameScreen.Menu;
			gameTimer = new Stopwatch();

			// TODO: Add your initialization logic here
			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += Window_ClientSizeChanged;

			base.Initialize();
		}

		private void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			if (Window.ClientBounds.Width < GameWidth)
			{
				_graphics.PreferredBackBufferWidth = GameWidth;
			}
			if (Window.ClientBounds.Height < GameHeight)
			{
				_graphics.PreferredBackBufferHeight = GameHeight;
			}
			_graphics.ApplyChanges();
		}

		public static int GameWidth = 360;
		public static int GameHeight = 240;

		public static bool hitboxView = true;

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatcher(GraphicsDevice, new Rectangle(0, 0, GameWidth, GameHeight));

			Assets.Tex2["player"] = SlicedSprite.Frameify(Content.Load<Texture2D>("PlayerSprite"), 5, 3);
			Assets.Tex2["tiles"] = SlicedSprite.Frameify(Content.Load<Texture2D>("TilesetReal"), 7, 15);
			Assets.Tex2["tilesImaginary"] = SlicedSprite.Frameify(Content.Load<Texture2D>("TilesetImaginary"), 7, 15);
			Assets.Tex2["items"] = SlicedSprite.Frameify(Content.Load<Texture2D>("ItemSprites"), 13, 1);
			Assets.Tex2["projectile"] = SlicedSprite.Frameify(Content.Load<Texture2D>("ProjectileSprites"), 4, 0);
			Assets.Tex2["inventory"] = SlicedSprite.Frameify(Content.Load<Texture2D>("InventorySquares"), 1, 1);

			Assets.Tex2["wolfReal"] = SlicedSprite.Frameify(Content.Load<Texture2D>("WolfReal"), 6, 0);
			Assets.Tex2["wolfImaginary"] = SlicedSprite.Frameify(Content.Load<Texture2D>("WolfImaginary"), 6, 0);
			Assets.Tex2["ratReal"] = SlicedSprite.Frameify(Content.Load<Texture2D>("RatReal"), 6, 0);
			Assets.Tex2["ratImaginary"] = SlicedSprite.Frameify(Content.Load<Texture2D>("RatImaginary"), 6, 0);
			Assets.Tex2["bearReal"] = SlicedSprite.Frameify(Content.Load<Texture2D>("BearReal"), 6, 0);
			Assets.Tex2["bearImaginary"] = SlicedSprite.Frameify(Content.Load<Texture2D>("BearImaginary"), 6, 0);
			Assets.Tex2["hudBars"] = SlicedSprite.Frameify(Content.Load<Texture2D>("HudBars"), 0, 7);
			Assets.Tex2["dust"] = SlicedSprite.Frameify(Content.Load<Texture2D>("DustSprites"), 1, 11);


			Assets.Tex["shroomIcon"] = Content.Load<Texture2D>("MushroomIcon");
			Assets.Tex["imaginosia"] = Content.Load<Texture2D>("Imaginosia");

			Assets.Tex["flashlightMask"] = Content.Load<Texture2D>("FlashlightMask");
			Assets.Tex["vignetteMask"] = Content.Load<Texture2D>("VignetteMask");
			Assets.Tex["vignetteOverlay"] = Content.Load<Texture2D>("VignetteOverlay");
			Assets.Tex["fireMask"] = Content.Load<Texture2D>("FireLight");
			Assets.Tex["special"] = new Texture2D(GraphicsDevice, 1, 1);
			Assets.Tex["special"].SetData(new Color[] { Color.White });

			Assets.Sfx["ambience1"] = Content.Load<SoundEffect>("SFX/AmbienceWarble");
			Assets.Sfx["ambience2"] = Content.Load<SoundEffect>("SFX/AmbienceBell");
			Assets.Sfx["ambience3"] = Content.Load<SoundEffect>("SFX/AmbiencePiano");
			Assets.Sfx["ambience4"] = Content.Load<SoundEffect>("SFX/AmbienceScream");
			Assets.Sfx["ambience5"] = Content.Load<SoundEffect>("SFX/AmbienceChanting");
			Assets.Sfx["ambience6"] = Content.Load<SoundEffect>("SFX/AmbienceCymbals");

			Assets.Sfx["treeHit"] = Content.Load<SoundEffect>("SFX/TreeHit");
			Assets.Sfx["treeBreak"] = Content.Load<SoundEffect>("SFX/TreeBreak");
			Assets.Sfx["gunshot"] = Content.Load<SoundEffect>("SFX/PlayerGunshot");
			Assets.Sfx["footstepReal"] = Content.Load<SoundEffect>("SFX/PlayerFootstepReal");

			Assets.Sfx["wandShotImaginary"] = Content.Load<SoundEffect>("SFX/WandShotImaginary");
			Assets.Sfx["foodEatImaginary"] = Content.Load<SoundEffect>("SFX/FoodEatImaginary");
			Assets.Sfx["boneEatImaginary"] = Content.Load<SoundEffect>("SFX/BoneEatImaginary");

			Assets.Sfx["wolfGrowlReal"] = Content.Load<SoundEffect>("SFX/WolfGrowlReal");
			Assets.Sfx["wolfBarkReal"] = Content.Load<SoundEffect>("SFX/WolfBarkReal");
			Assets.Sfx["wolfWhimperReal"] = Content.Load<SoundEffect>("SFX/WolfWhimperReal");
			Assets.Sfx["wolfDeathReal"] = Content.Load<SoundEffect>("SFX/WolfDeathReal");
			Assets.Sfx["wolfPingImaginary"] = Content.Load<SoundEffect>("SFX/WolfPingImaginary");

			Assets.Sfx["ratDeathReal"] = Content.Load<SoundEffect>("SFX/RatDeathReal");
			Assets.Sfx["ratHurtReal"] = Content.Load<SoundEffect>("SFX/RatHurtReal");
			Assets.Sfx["ratLaserImaginary"] = Content.Load<SoundEffect>("SFX/RatLaserImaginary");
			Assets.Sfx["ratLaughImaginary"] = Content.Load<SoundEffect>("SFX/RatLaughImaginary");

			Assets.Sfx["bearSleepReal"] = Content.Load<SoundEffect>("SFX/BearSleepReal");
			Assets.Sfx["bearStirReal"] = Content.Load<SoundEffect>("SFX/BearStirReal");
			Assets.Sfx["bearGruntReal"] = Content.Load<SoundEffect>("SFX/BearGruntReal");

			Assets.Sfx["imaginationPrelude"] = Content.Load<SoundEffect>("SFX/ImaginationPrelude");
			Assets.Sfx["imaginationJingle"] = Content.Load<SoundEffect>("SFX/ImaginationTransition");
			Assets.Sfx["realWorldJingle"] = Content.Load<SoundEffect>("SFX/RealTransition");

			Assets.Sfx["enemyHurtReal"] = Content.Load<SoundEffect>("SFX/AnimalHitReal");
			Assets.Sfx["playerHurtImaginary"] = Content.Load<SoundEffect>("SFX/PlayerHurtImaginary");
			Assets.Sfx["enemyHurtImaginary"] = Content.Load<SoundEffect>("SFX/AnimalHitImaginary");
			Assets.Sfx["enemyPopImaginary"] = Content.Load<SoundEffect>("SFX/EnemyPopImaginary");
			Assets.Sfx["swoosh"] = Content.Load<SoundEffect>("SFX/Swoosh");
			Assets.Sfx["death"] = Content.Load<SoundEffect>("SFX/Death");

			Assets.Sfx["mushroom"] = Content.Load<SoundEffect>("SFX/Mushroom");

			Assets.Sfx["menuClick"] = Content.Load<SoundEffect>("SFX/MenuClick");

			Assets.Darkness = Content.Load<Effect>("DarknessShader");

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (gameStartTimer > 0)
			{
				gameStartTimer--;
				if (gameStartTimer == 0)
				{
					gameTimer.Restart();
					gameScreen = GameScreen.InGame;
				}
			}
			else
			{
				if (KeyHelper.Pressed(Keys.Escape) && (gameScreen == GameScreen.Menu || gameScreen == GameScreen.Menu))
					Exit();

				if (gameScreen == GameScreen.InGame)
					gamestate.Update();

				if (gameScreen == GameScreen.Paused)
				{
					int pageSwap = Keybinds.SwapItem(ControllerType.Keyboard, PlayerIndex.One);
					if (pageSwap != 0)
					{
						PauseScreen.guidePage += pageSwap;
						PauseScreen.guidePage %= PauseScreen.guidePageCount;
						if (PauseScreen.guidePage < 0)
						{
							PauseScreen.guidePage = PauseScreen.guidePageCount - 1;
						}
						Assets.Sfx["menuClick"].Play(0.1f, 0, 0);
					}
				}

				if (KeyHelper.Pressed(Keys.Space))
				{
					if (gameScreen == GameScreen.InGame)
					{
						gameTimer.Stop();
						gameScreen = GameScreen.Paused;
					}
					else if (gameScreen == GameScreen.Paused)
					{
						gameTimer.Start();
						gameScreen = GameScreen.InGame;
					}
				}

				if (gameScreen == GameScreen.Menu)
				{
					if (KeyHelper.Pressed(Keys.Enter))
					{
						gamestate = new Gamestate();
						UIHandler.Reset();
						gameStartTimer = 60;
						Assets.Sfx["imaginationPrelude"].Play();
					}
				}

				if (gameScreen == GameScreen.Paused || gameScreen == GameScreen.Death)
				{
					if (KeyHelper.Pressed(Keys.Escape))
					{
						gameTimer.Reset();
						gameScreen = GameScreen.Menu;
					}
				}
			}

			

			// TODO: Add your update logic here
			KeyHelper.Update();
			MouseHelper.Update();
			GamePadHelper.Update();
			base.Update(gameTime);
		}

		public static int ScreenScalingFactor = 1;
		public static Point ScreenOriginOffset = Point.Zero;

		public static int ScreenShake;

		protected override void Draw(GameTime gameTime)
		{
			Rectangle destination = Window.ClientBounds;
			destination.Location -= Window.Position;

			Point a = destination.Size / new Point(GameWidth, GameHeight);

			int scale = Math.Min(a.X, a.Y);
			ScreenScalingFactor = scale;

			destination.Size = new Point(GameWidth * ScreenScalingFactor, GameHeight * ScreenScalingFactor);

			ScreenOriginOffset.X = (Window.ClientBounds.Width - destination.Width) / 2;
			ScreenOriginOffset.Y = (Window.ClientBounds.Height - destination.Height) / 2;

			destination.Location = ScreenOriginOffset;

			GraphicsDevice.Clear(Color.Black);

			switch (gameScreen)
			{
				case GameScreen.Menu:

					spriteBatch.LayerBegin("final");
					spriteBatch.Draw(Assets.Tex["special"], new Rectangle(0, 0, GameWidth, GameHeight), Color.CadetBlue * 0.2f);

					spriteBatch.Draw(Assets.Tex["imaginosia"], new Vector2(GameWidth / 2 - Assets.Tex["imaginosia"].Width / 2, 40), Color.White);
					TextPrinter.Print("by Rebmiami, submission for the 2022.1 Brackeys game jam 'It Is Not Real'", new Vector2(GameWidth / 2, 70), spriteBatch, center: true);

					TextPrinter.Print(gameStartTimer > 0 ? "It begins..." : "Press enter to play", new Vector2(GameWidth / 2, 100), spriteBatch, center: true);
					TextPrinter.Print("Press space to pause and see game guide", new Vector2(GameWidth / 2, 180), spriteBatch, center: true);
					TextPrinter.Print("Press escape to exit", new Vector2(GameWidth / 2, 200), spriteBatch, center: true);

					if (gameStartTimer > 0)
					{
						spriteBatch.Draw(Assets.Tex["special"], new Rectangle(0, 0, GameWidth, GameHeight), Color.White * ((60 - gameStartTimer) / 60f));
					}

					break;
				case GameScreen.Paused:
				case GameScreen.InGame:
					if (ScreenShake > 0)
					{
						destination.Location += (RNG.RotateRandom(Vector2.UnitX, 360) * ScreenShake).ToPoint();
						ScreenShake--;
					}

					gamestate.Draw(spriteBatch);

					// spriteBatch.LayerBegin("player", blendState: BlendState.Additive);
					// spriteBatch.LayerEnd();

					spriteBatch.LayerBegin("light", blendState: BlendState.Additive);

					spriteBatch.Draw(Assets.Tex["flashlightMask"], gamestate.player.ScreenPosition, null, Color.White * 0.8f, (float)Math.Atan2(gamestate.player.direction.Y, gamestate.player.direction.X) - MathHelper.PiOver2, new Vector2(50, 0), 1, SpriteEffects.None, 0);
					spriteBatch.Draw(Assets.Tex["fireMask"], gamestate.player.ScreenPosition, null, Color.White, 0, new Vector2(540), new Vector2(0.2f, 0.13f), SpriteEffects.None, 0);

					foreach (var item in gamestate.world.fires)
					{
						float intensity = (float)Math.Sqrt(gamestate.world.tiles[item.X, item.Y].floorObjectHealth * 0.001f);


						spriteBatch.Draw(Assets.Tex["fireMask"], PositionHelper.ToScreenPosition(item.ToVector2() + new Vector2(0.5f)), null, Color.White, 0, new Vector2(540), new Vector2(0.2f, 0.13f) * intensity, SpriteEffects.None, 0);
					}

					spriteBatch.LayerEnd();


					Assets.Darkness.Parameters["LightMask"].SetValue(spriteBatch.GetLayer("light"));

					bool alwaysLight = false;

					if (ImaginationHandler.IsImagination || alwaysLight)
					{
						spriteBatch.LayerBegin("final");
					}
					else
					{
						spriteBatch.LayerBegin("final", effect: Assets.Darkness);
					}

					spriteBatch.DrawLayer("tiles");
					spriteBatch.LayerEnd();

					spriteBatch.Begin(samplerState: SamplerState.PointClamp);

					DustManager.Draw(spriteBatch, 1);

					if (gamestate.player.MouseText != null)
						TextPrinter.Print(gamestate.player.MouseText, MouseHelper.Position, spriteBatch, background: true);

					UIHandler.Draw(spriteBatch);

					if (gamestate.player.hunger < 2f && ImaginationHandler.IsImagination)
					{
						spriteBatch.Draw(Assets.Tex["vignetteOverlay"], new Rectangle(0, 0, GameWidth, GameHeight), Color.Black * (2 - gamestate.player.hunger));
					}
					else if (gamestate.player.hunger < 1f)
					{
						spriteBatch.Draw(Assets.Tex["vignetteOverlay"], new Rectangle(0, 0, GameWidth, GameHeight), Color.DarkGreen * (1 - gamestate.player.hunger) * 0.6f);
					}

					if (gamestate.player.invFrames > 0 && !ImaginationHandler.IsImagination)
					{
						spriteBatch.Draw(Assets.Tex["vignetteOverlay"], new Rectangle(0, 0, GameWidth, GameHeight), Color.Red * (gamestate.player.invFrames / 30f));
					}

					if (ImaginationHandler.TransitionTimer > 0)
					{
						spriteBatch.Draw(Assets.Tex["special"], new Rectangle(0, 0, GameWidth, GameHeight), Color.White * ((60 - ImaginationHandler.TransitionTimer) / 60f));
					}

					if (gameScreen == GameScreen.Paused)
					{
						PauseScreen.DrawHelp();
					}

					break;
				case GameScreen.Death:
					spriteBatch.LayerBegin("final");
					TextPrinter.Print("GAME OVER", new Vector2(GameWidth / 2, 70), spriteBatch, center: true);

					TextPrinter.Print(deathMessage, new Vector2(GameWidth / 2, 80), spriteBatch, center: true);

					string deathTimer = "";


					if (gameTimer.Elapsed.TotalHours > 1)
					{
						deathTimer = $"{(int)gameTimer.Elapsed.TotalHours} hours, {gameTimer.Elapsed.Minutes} minutes, and {gameTimer.Elapsed.Seconds} seconds.";
					}
					else if (gameTimer.Elapsed.TotalMinutes > 1)
					{
						deathTimer = $"{(int)gameTimer.Elapsed.TotalMinutes} minutes and {gameTimer.Elapsed.Seconds} seconds.";
					}
					else
					{
						deathTimer = $"{gameTimer.Elapsed.Seconds} seconds.";
					}

					deathTimer = "You survived for " + deathTimer;
					TextPrinter.Print(deathTimer, new Vector2(GameWidth / 2, 90), spriteBatch, center: true);


					TextPrinter.Print("Press Escape to return to the main menu.", new Vector2(GameWidth / 2, 120), spriteBatch, center: true);
					break;
				default:
					break;
			}

			spriteBatch.End();

			spriteBatch.FrameBegin(samplerState: SamplerState.PointClamp);
			spriteBatch.Draw(spriteBatch.GetLayer("final"), destination, Color.White);
			spriteBatch.FrameEnd();

			spriteBatch.DisposeLayer("tiles");
			spriteBatch.DisposeLayer("light");
			spriteBatch.DisposeLayer("final");
			// spriteBatch.DisposeLayer("player");


			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}

		public static Game1 getGame;

		public static string deathMessage;

		internal void GameOver(Entity source)
		{
			gameTimer.Stop();
			gameScreen = GameScreen.Death;

			if (source == null)
			{
				deathMessage = "You died of starvation.";
			}
			else
			{
				switch (((Enemy)source).type)
				{
					case 0:
						deathMessage = "You were killed by a wolf.";
						break;
					case 1:
						deathMessage = "You were killed by a rat.";
						break;
					case 2:
						deathMessage = "You were killed by a bear.";
						break;
				}
			}
		}
	}

	public enum GameScreen
	{
		Menu,
		InGame,
		Paused,
		Death
	}
}
