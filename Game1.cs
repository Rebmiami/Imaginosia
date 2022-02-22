using Imaginosia.Gameplay;
using Imaginosia.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Imaginosia
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatcher spriteBatch;

		public static Gamestate gamestate;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			getGame = this;
			gamestate = new Gamestate();


			gamestate.SpawnEnemy(Vector2.Zero, 2);
			// TODO: Add your initialization logic here
			Window.AllowUserResizing = true;

			base.Initialize();
		}

		public static int GameWidth = 240;
		public static int GameHeight = 160;

		public static bool hitboxView = true;

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatcher(GraphicsDevice, new Rectangle(0, 0, GameWidth, GameHeight));

			Assets.Tex2["player"] = SlicedSprite.Frameify(Content.Load<Texture2D>("PlayerSprite"), 0, 0);
			Assets.Tex2["tiles"] = SlicedSprite.Frameify(Content.Load<Texture2D>("TilesetReal"), 7, 15);
			Assets.Tex2["tilesImaginary"] = SlicedSprite.Frameify(Content.Load<Texture2D>("TilesetImaginary"), 7, 15);
			Assets.Tex2["items"] = SlicedSprite.Frameify(Content.Load<Texture2D>("ItemSprites"), 13, 1);
			Assets.Tex2["projectile"] = SlicedSprite.Frameify(Content.Load<Texture2D>("ProjectileSprites"), 4, 0);
			Assets.Tex2["inventory"] = SlicedSprite.Frameify(Content.Load<Texture2D>("InventorySquares"), 1, 1);

			Assets.Tex2["wolfReal"] = SlicedSprite.Frameify(Content.Load<Texture2D>("WolfReal"), 6, 0);
			// Assets.Tex2["wolfImaginary"] = SlicedSprite.Frameify(Content.Load<Texture2D>("BearImaginary"), 6, 0);
			Assets.Tex2["ratReal"] = SlicedSprite.Frameify(Content.Load<Texture2D>("RatReal"), 6, 0);
			// Assets.Tex2["ratImaginary"] = SlicedSprite.Frameify(Content.Load<Texture2D>("BearImaginary"), 6, 0);
			Assets.Tex2["bearReal"] = SlicedSprite.Frameify(Content.Load<Texture2D>("BearReal"), 6, 0);
			Assets.Tex2["bearImaginary"] = SlicedSprite.Frameify(Content.Load<Texture2D>("BearImaginary"), 6, 0);


			Assets.Tex["special"] = new Texture2D(GraphicsDevice, 1, 1);
			Assets.Tex["special"].SetData(new Color[] { Color.White });

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			gamestate.Update();

			// TODO: Add your update logic here
			KeyHelper.Update();
			MouseHelper.Update();
			GamePadHelper.Update();
			base.Update(gameTime);
		}

		public static int ScreenScalingFactor = 1;
		public static Point ScreenOriginOffset = Point.Zero;

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			gamestate.Draw(spriteBatch);

			// spriteBatch.LayerBegin("player", blendState: BlendState.Additive);
			// spriteBatch.LayerEnd();

			Rectangle destination = Window.ClientBounds;
			destination.Location -= Window.Position;

			Point a = destination.Size / new Point(GameWidth, GameHeight);

			int scale = Math.Min(a.X, a.Y);
			ScreenScalingFactor = scale;

			destination.Size = new Point(GameWidth * ScreenScalingFactor, GameHeight * ScreenScalingFactor);

			spriteBatch.FrameBegin(samplerState: SamplerState.PointClamp);
			spriteBatch.Draw(spriteBatch.GetLayer("tiles"), destination, Color.White);
			// spriteBatch.Draw(spriteBatch.GetLayer("player"), destination, Color.White);
			spriteBatch.FrameEnd();

			spriteBatch.DisposeLayer("tiles");
			// spriteBatch.DisposeLayer("player");


			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}

		public static Game1 getGame;
	}
}
