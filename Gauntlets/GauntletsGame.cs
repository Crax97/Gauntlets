using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using CraxAwesomeEngine.Core;
using CraxAwesomeEngine.Core.Physics;
using CraxAwesomeEngine.Core.GUI;
using CraxAwesomeEngine.Core.Scripting;
using CraxAwesomeEngine.Core.Debugging;
using System.ComponentModel;
using System.Threading;

/// <summary>
/// TODO: 
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

        World w;
        Camera firstCamera = null;
        Entity fakePlayer = null;
        Entity testCollider = null;
  
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

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
            RasterizerState customState = new RasterizerState();
            graphics.GraphicsDevice.RasterizerState = customState;

            graphics.ApplyChanges();

            ComponentRecord.RegisterAttribute<Sprite>("Sprite");
            ComponentRecord.RegisterAttribute<AnimatedSprite>("AnimatedSprite");
            ComponentRecord.RegisterAttribute<Transform>("Transform");
            ComponentRecord.RegisterAttribute<GUIButton>("GUIButton");
            ComponentRecord.RegisterAttribute<GUILabel>("GUILabel");
            ComponentRecord.RegisterAttribute<GUITextBox>("GUITextBox");
            ComponentRecord.RegisterAttribute<GUIImage>("GUIImage");
            ComponentRecord.RegisterAttribute<AABBCollider>("AABBCollider");
            ComponentRecord.RegisterAttribute<SATCollider>("SATCollider");
            ComponentRecord.RegisterAttribute<CharacterController>("CharacterController");

            Debug.InitializeDebug(GraphicsDevice, this);
            DebugConsole.DebugConsoleInit(this, graphics);
            GameSerializer.Init(this);
            GameScript.InitGameScript();
            Transform.UpdateGraphicsSize(graphics);
            InputManager.InitInputManager(this);
            Camera.InitCamera(graphics);
            GUILabel.Initialize(this);

            Exiting += new EventHandler<EventArgs>((object obj, EventArgs args) =>
            {
                Debug.CloseDebug();
            });

            DebugConsole.RegisterCommand("reset", (string[] args) =>
            {
                reset();
            });

            w = new World();

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            //Then initialize entities
            Entity.InitializeEntities();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            guiBatch = new SpriteBatch(GraphicsDevice);

            fakePlayer = Entity.Instantiate(1);
            testCollider = Entity.Instantiate(0);

            firstCamera = new Camera();

            testCollider.Transform.Position = Transform.WindowHalfSize;
            
            World.Current.BeginSimulation();
        }

        private void reset()
        {

            fakePlayer.Transform.Position = Vector2.Zero;
            GameScript.ResetScripts();

            //Console.Clear();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputManager.UpdateInputBegin();

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!DebugConsole.Enabled)
            {

                if (InputManager.ScrollWheel != 0.0f)
                {
                    Camera.Main.Zoom += InputManager.ScrollWheel * delta;
                }

                Camera.Main.Transform.Position = fakePlayer.Transform.Position;

                World.Current.Update(delta);
                Collider.CalculateCollisions();
            }


            base.Update(gameTime);
            InputManager.UpdateInputEnd();
            this.Window.Title = "Gauntlets - FPS " + 1.0f / delta;

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Camera.Main.BackgroundColor);
            Matrix viewMatrix = Camera.Main.GetMatrix();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, viewMatrix);
            guiBatch.Begin();

            World.Current.DrawSprites(spriteBatch, gameTime);
            spriteBatch.End();

            World.Current.DrawGUI(guiBatch);
            guiBatch.End();

            Debug.DebugDraw();

            base.Draw(gameTime);
        }
    }
}