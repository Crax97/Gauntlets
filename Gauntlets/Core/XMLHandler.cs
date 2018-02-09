using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using CraxAwesomeEngine.Core.Scripting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace CraxAwesomeEngine.Core
{
    public static class GameSerializer
    {
        private static Game serializedGame;

        public static void Init(Game game)
        {
            serializedGame = game;
        }

        sealed class Vector2SerializatorSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Vector2 vector = (Vector2)obj;
                info.AddValue("X", vector.X);
                info.AddValue("Y", vector.Y);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Vector2 vector = (Vector2)obj;
                vector.X = info.GetSingle("X");
                vector.Y = info.GetSingle("Y");
                return vector;
            }
        }
        sealed class ColorSerializer : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Color color = (Color)obj;
                info.AddValue("r", color.R);
                info.AddValue("g", color.G);
                info.AddValue("b", color.B);
                info.AddValue("a", color.A);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Color color = (Color)obj;
                color.R = info.GetByte("r");
                color.G = info.GetByte("g");
                color.B = info.GetByte("b");
                color.A = info.GetByte("a");
                return color;
            }
        }
        
        sealed class TextureSerializer : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Texture2D texture = obj as Texture2D;
                info.AddValue("texture", texture.Name);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                return serializedGame.Content.Load<Texture2D>(info.GetString("texture"));
            }
        }
        sealed class SpriteFontSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                SpriteFont font = obj as SpriteFont;
                info.AddValue("font", font.Texture.Name);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                return serializedGame.Content.Load<SpriteFont>(info.GetString("font"));
            }
        }
        sealed class RectangleSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Rectangle rect = (Rectangle)obj;
                info.AddValue("width", rect.Width);
                info.AddValue("height", rect.Height);
                info.AddValue("X", rect.Center.X);
                info.AddValue("Y", rect.Center.Y);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                return new Rectangle(info.GetInt32("X"), info.GetInt32("Y"), info.GetInt32("width"), info.GetInt32("height"));
            }
        }
        sealed class GameScriptSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                GameScript script = (GameScript)obj;
                info.AddValue("file", script.ScriptFileName);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                return new GameScript(info.GetString("file"));
            }
        }



        public static void SerializeEntities(List<Entity> entites)
        {
            SurrogateSelector selector = new SurrogateSelector();
            StreamingContext context = new StreamingContext(StreamingContextStates.All);
            selector.AddSurrogate(typeof(Vector2), context, new Vector2SerializatorSurrogate());
            selector.AddSurrogate(typeof(Color), context, new ColorSerializer());
            selector.AddSurrogate(typeof(Texture2D), context, new TextureSerializer());
            selector.AddSurrogate(typeof(SpriteFont), context, new SpriteFontSurrogate());
            selector.AddSurrogate(typeof(Rectangle), context, new RectangleSurrogate());
            selector.AddSurrogate(typeof(GameScript), context, new GameScriptSurrogate());

            using (FileStream entitiesFile = File.Open(Path.Combine("Content", "Data", "Entities.data"), FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.SurrogateSelector = selector;
                formatter.Serialize(entitiesFile, entites);

            }
        }
        public static void DeserializeEntities(out List<Entity> entities)
        {
            entities = new List<Entity>();
            SurrogateSelector selector = new SurrogateSelector();
            StreamingContext context = new StreamingContext(StreamingContextStates.All);
            selector.AddSurrogate(typeof(Vector2), context, new Vector2SerializatorSurrogate());
            selector.AddSurrogate(typeof(Color), context, new ColorSerializer());
            selector.AddSurrogate(typeof(Texture2D), context, new TextureSerializer());
            selector.AddSurrogate(typeof(SpriteFont), context, new SpriteFontSurrogate());
            selector.AddSurrogate(typeof(Rectangle), context, new RectangleSurrogate());
            selector.AddSurrogate(typeof(GameScript), context, new GameScriptSurrogate());

            using (FileStream entitiesFile = File.Open(Path.Combine("Content", "Data", "Entities.data"), FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.SurrogateSelector = selector;

                while(entitiesFile.Position < entitiesFile.Length)
                {
                    Entity actual = formatter.Deserialize(entitiesFile) as Entity;
                    entities.Add(actual);
                }

            }
        }
    }
}
