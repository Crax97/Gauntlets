using CraxAwesomeEngine.Core.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core.Debugging
{
    static class DebugConsole
    {

        public static bool Enabled { get; private set; } = false;

        private const int commandDistanceFromOutput = 20;
        private static Rectangle consoleOutputRectangle;
        private static Rectangle commandOutputRectangle;
        private static SpriteFont consoleFont = null;
        private static string debugConsoleContent = "$";
        private static string consoleCommandContent = ">";
        private static Dictionary<string, Action<string[]>> commands;

        internal static void DebugConsoleInit (Game game, GraphicsDeviceManager manager)
        {
            consoleOutputRectangle = new Rectangle(Point.Zero, new Point(manager.PreferredBackBufferWidth, manager.PreferredBackBufferHeight / 2 - commandDistanceFromOutput));
            Vector2 commandPosition = new Vector2(manager.PreferredBackBufferWidth, commandDistanceFromOutput);
            commandOutputRectangle = new Rectangle(new Point(0, manager.PreferredBackBufferHeight / 2 - commandDistanceFromOutput), commandPosition.ToPoint());
            commands = new Dictionary<string, Action<string[]>>();
            consoleFont = game.Content.Load<SpriteFont>(Path.Combine("Fonts", "Console"));

            game.Window.TextInput += KeyCharCallback;

            RegisterCommand("clear", (string[] args) =>
            {
                Clear();
            });

            RegisterCommand("echo", (string[] args) =>
            {
                string message = "";

                for (int i = 1; i < args.Length; i++)
                {
                    message += " " + args[i];
                }

                DebugConsole.WriteLine(message);

            });

        }

        public static void RegisterCommand(string commandName, Action<string[]> command)
        {
            if (!commands.ContainsKey(commandName))
            {
                commands.Add(commandName, command);
            }
            else
            {
                throw new ArgumentException("Console already contains command " + commandName);
            }

        }

        private static void ParseCommand()
        {
            if (consoleCommandContent.Length > 1)
            {
                string[] commandAndArgs = consoleCommandContent.Substring(1).Split(' ');
                if(commands.ContainsKey(commandAndArgs[0]))
                {
                    //Runs the action
                    commands[commandAndArgs[0]](commandAndArgs);

                    consoleCommandContent = ">";

                }
                else
                {
                    WriteLine("Command " + commandAndArgs[0] + " not recognised!");
                }

            }
        }

        private static void KeyCharCallback(object sender, TextInputEventArgs args)
        {

            if (args.Key == Keys.Tab)
            {
                Enabled = !Enabled;
            }



            if (Enabled)
            {
                if (!char.IsControl(args.Character))
                {
                    consoleCommandContent += args.Character;
                }
                else
                {
                    if (args.Character == '\n' || args.Character == '\r' )
                    {
                        ParseCommand();
                        consoleCommandContent = ">";
                    }
                    else if (args.Character == '\b')
                    {
                        consoleCommandContent = (consoleCommandContent.Length > 2) ? consoleCommandContent.Substring(0, consoleCommandContent.Length - 1) : ">";
                    }
                }
            }
        }

        internal static void Draw(SpriteBatch batch)
        {
            if (Enabled)
            {
                batch.Draw(Debug.SinglePointTexture, consoleOutputRectangle, null, Color.DarkGray);
                batch.Draw(Debug.SinglePointTexture, commandOutputRectangle, null, Color.Gray);

                batch.DrawString(consoleFont, debugConsoleContent, Vector2.Zero, Color.White);
                batch.DrawString(consoleFont, consoleCommandContent, commandOutputRectangle.Location.ToVector2(), Color.White);

            }
        }

        public static void Write(string text)
        {
            debugConsoleContent += text;
        }

        public static void WriteLine(string text)
        {
            Write(text);
            debugConsoleContent += "\n$";
        }

        public static void Clear()
        {
            debugConsoleContent = "$";
        }


    }
}
