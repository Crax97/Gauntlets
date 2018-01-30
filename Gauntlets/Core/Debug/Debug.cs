using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CraxAwesomeEngine.Core.Debugging
{

    /// <summary>
    /// A Debug class
    /// </summary>
    static class Debug
    {
        public static Texture2D SinglePointTexture { get; private set; } = null;
        private static Game runningGame = null;
        private static SpriteBatch debugBatch = null;
        private static GraphicsDevice graphicsDevice = null;
        private static Queue<Action> drawQueue = null;
        private static StreamWriter logFile = null;

        public static bool DebugEnabled { get; set; } = true;

        /// <summary>
        /// Initializes the Debug class.
        /// Call this in the Game.LoadContent() function
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="device"></param>
        public static void InitializeDebug(GraphicsDevice device, Game game)
        {
            debugBatch = new SpriteBatch(device);
            drawQueue = new Queue<Action>();
            graphicsDevice = device;
            SinglePointTexture = new Texture2D(device, 1, 1);
            SinglePointTexture.SetData<Color>(new Color[] { Color.White });
            runningGame = game;
            logFile = new StreamWriter(File.Open("log.txt", FileMode.OpenOrCreate));

        }

        /// <summary>
        /// Queues a draw of a debug line that goes from a begin to an end.
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        public static void DrawLine(Vector2 begin, Vector2 end, Color? color)
        {
            Vector2 direction = (end - begin);
            float rotation = (float)Math.Atan2(direction.Y, direction.X);

            drawQueue.Enqueue(() => {
                debugBatch.Draw(SinglePointTexture,
                new Rectangle((int)begin.X, (int)begin.Y, (int)direction.Length(), 1),
                null, (color.HasValue ? color.Value : Color.Yellow),
                rotation, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            );

        }

        /// <summary>
        /// Queues a point draw at a specified location
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void DrawPoint(Vector2 position, Color? color, float thickness = 1.0f)
        {

            Vector2 actualPosition = position - new Vector2(thickness * 0.5f, thickness * 0.5f);

            drawQueue.Enqueue(() =>
           {
               debugBatch.Draw(SinglePointTexture, actualPosition, null, (color.HasValue ? color.Value : Color.LightYellow), 0.0f, Vector2.Zero, thickness, SpriteEffects.None, 1.0f);
           });
        }

        /// <summary>
        /// Queues a filled rectangle draw.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        public static void DrawRectangleFilled(Rectangle rect, Color? color = null, float rotation = 0.0f)
        {

            drawQueue.Enqueue(() =>
            {
                debugBatch.Draw(SinglePointTexture, rect, null, 
                    (color.HasValue ? color.Value : Color.LightYellow), 
                    rotation, 
                    (Vector2.Zero),
                    SpriteEffects.None, 1.0f );
            });
        }

        /// <summary>
        /// Queue a rectangle borders draw.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        public static void DrawRectangleBounds(Rectangle rect, Color? color = null)
        {
            Vector2 position = rect.Center.ToVector2();
            Vector2 halfSize = rect.Size.ToVector2() *  0.5f;
            Vector2[] vectors = new Vector2[]
            {
                position + new Vector2(-halfSize.X, -halfSize.Y),
                position + new Vector2(+halfSize.X, -halfSize.Y),
                position + new Vector2(+halfSize.X, +halfSize.Y),
                position + new Vector2(-halfSize.X, +halfSize.Y),
            };

            for (int i = 0; i < 4; i++)
            {
                Vector2 begin = vectors[i];
                Vector2 end = (i < 3) ? vectors[i + 1] : vectors[0];
                Debug.DrawLine(begin, end, (color.HasValue ? color.Value : Color.LightYellow));
            }

        }

        public static void DrawShape(List<Vector2> shape, Color? color)
        {
            for (int i = 0; i < shape.Count; i++)
            {
                Vector2 begin = shape[i];
                Vector2 end = (i < shape.Count - 1) ? shape[i + 1] : shape[0];
                Debug.DrawLine(begin, end, (color.HasValue ? color.Value : Color.LightYellow));
            }
        }

        /// <summary>
        /// Draws all the enqueued shapes.
        /// Call this at the bottom of the Game.Draw() call
        /// </summary>
        public static void DebugDraw()
        {
            //if (drawQueue.Count != 0 && DebugEnabled)
            //{
                debugBatch.Begin();

                while (drawQueue.Count > 0)
                {
                    Action currentDraw = drawQueue.Dequeue();
                    currentDraw();
                }

                DebugConsole.Draw(debugBatch);
                debugBatch.End();
            //}


        }

        /// <summary>
        /// Prints a message to the console debug
        /// and to the Standard Output
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Log(string message, params object[] args )
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("[LOG] " + message, args);

            //Add more stuff later
            Console.WriteLine(builder.ToString());
            DebugConsole.WriteLine(builder.ToString());
            logFile.WriteLine(builder);
        }

        /// <summary>
        /// Prints a message to the Console Debug
        /// and to the standard error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Error(string message, bool isFatal = false, params string[] args)
        {
            StringBuilder errorMessage = new StringBuilder();
            errorMessage.AppendFormat("[ERROR] " + message, args);

            Console.Error.WriteLine(errorMessage);
            DebugConsole.WriteLine(errorMessage.ToString());
            logFile.WriteLine(errorMessage);

            if(isFatal)
            {

                logFile.WriteLine("[ERROR WAS FATAL]");
                CloseDebug();
                runningGame.Exit();
            }

        }

        public static void CloseDebug()
        {
            logFile.Flush();
            logFile.Dispose();
        }
    }
}
