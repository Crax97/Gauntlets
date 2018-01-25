using System;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Gauntlets.Core;

/// <summary>
/// TODO: Add GUIImage class.
/// After it's done, start working on Collider class.
/// Then, start developing a base platformer character
/// </summary>

namespace Gauntlets.Simulation
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
        SpriteBatch guiBatch;

        World w = new World();
        Entity test = null;

        public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

		}

		void HandleButtonCallback()
		{

			Console.WriteLine("Click!");

		}

		void HandleButtonCallback1()
		{
			Console.WriteLine("Release!");
		}

		void HandleButtonCallback2()
		{
			Console.WriteLine("Double click!");
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			graphics.ApplyChanges();

            ComponentRecord.RegisterAttribute<Sprite>("Sprite");
            ComponentRecord.RegisterAttribute<AnimatedSprite>("AnimatedSprite");
            ComponentRecord.RegisterAttribute<Transform>("Transform");
            ComponentRecord.RegisterAttribute<GUIButton>("GUIButton");
            ComponentRecord.RegisterAttribute<GUILabel>("GUILabel");
            ComponentRecord.RegisterAttribute<GUITextBox>("GUITextBox");

			Transform.UpdateGraphicsSize(graphics);

			this.IsMouseVisible = true;

			World.Current.BeginSimulation();

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
        {
            //Initialize components first!
            GUILabel.Initialize(this);

            //Then initialize entities
            Entity.InitializeEntities(Path.Combine(Content.RootDirectory, "XML", "EntityList.xml"), this);

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
            guiBatch = new SpriteBatch(GraphicsDevice);

            test = Entity.Instantiate(0);
             
            GUIButton buttonComponent = test.GetComponent<GUIButton>();
            buttonComponent.AddOnReleaseCallback(HandleButtonCallback2);
            buttonComponent.Transform.Translate(new Vector2(150, 0));

            GUITextBox textBox = test.GetComponent<GUITextBox>();
            textBox.Offset = new Vector2(10, 10);
            textBox.Transform.LocalPosition += new Vector2(0, graphics.PreferredBackBufferHeight - textBox.Sprite.Height) * 0.5f;

            GUILabel label = test.GetComponent<GUILabel>();
            label.Transform.Translate (new Vector2(400, 0));
            label.Label = "There are some\nGUI Elements under me!";
            label.Center = label.Font.MeasureString("There are some\nGUI Elements under me!") * new Vector2(0.5f, 0);
			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                test.Transform.LocalPosition += new Vector2(10, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {

                test.Transform.LocalPosition += new Vector2(-10, 0);
            }

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			World.Current.Update(delta);

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
            guiBatch.Begin();

			World.Current.DrawSprites(spriteBatch, gameTime);
			spriteBatch.End();

            World.Current.DrawGUI(guiBatch);               
            guiBatch.End();

			base.Draw(gameTime);
		}
	}
}
