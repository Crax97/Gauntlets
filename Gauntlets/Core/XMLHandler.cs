using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CraxAwesomeEngine.Core;
using CraxAwesomeEngine.Core.GUI;
using static CraxAwesomeEngine.Core.AnimatedSprite;
using CraxAwesomeEngine.Content.Scripts.Proxies;

namespace CraxAwesomeEngine.Core
{
    static class XmlComponentsReaders
    {
        public static object ColorFromXmlAttribute(XmlAttribute attribute)
        {
            Color color = Color.White;
            if (attribute != null)
            {
                string[] vals = attribute.Value.Split(' ');
                color.R = byte.Parse(vals[0]);
                color.G = byte.Parse(vals[1]);
                color.B = byte.Parse(vals[2]);
                color.A = (vals[3] != null) ? byte.Parse(vals[3]) : (byte)255;
            }
            return color;
        }
        public static Vector2Proxy Vector2FromXmlAttribute(XmlAttribute attribute, Vector2Proxy defaultValue)
        {

            if (attribute != null)
            {

                string[] vals = attribute.Value.Split(' ');
                float X = float.Parse(vals[0]);
                float Y = float.Parse(vals[1]);
                return new Vector2Proxy(X, Y);
            }
            else
            {
                return defaultValue;
            }
        }

        public static object TransformFromXmlNode(XmlNode node,Game game)
        {
            if (node != null)
            {
                XmlAttributeCollection attributes = node.Attributes;

                string rotationStr = (attributes["rotation"] != null) ? attributes["rotation"].Value : null;
                float rotation = (rotationStr != null) ? (float.Parse(rotationStr)) : 0.0f;

                Vector2Proxy position = Vector2FromXmlAttribute(attributes["position"], Vector2Proxy.Zero);
                Vector2Proxy scale = Vector2FromXmlAttribute(attributes["position"], Vector2Proxy.One);
                return new Transform(position, scale, rotation);
            }
            return new Transform();
        }


        public static object SpriteFromXmlNode(XmlNode node, Game game)
        {
            XmlAttributeCollection attrib = node.Attributes;

            string textureName = attrib["texture"].Value;
            if (textureName == null) throw new ArgumentNullException("<Sprite ...></Sprite> must have at least a texture name!");

            Texture2D Texture = game.Content.Load<Texture2D>(textureName);

            string widthStr = (attrib["width"] != null) ? attrib["width"].Value : null;
            string heightStr = (attrib["height"] != null) ? attrib["height"].Value : null;
            int width, height;

            //Schema: width [height] =
            //"full" => Texture.Width [Height]
            //"half" => Texture.Width/2 [Height]/2
            //"/n" => Texture.Width / n [Height / n]
            //"n" => n
            //nothing => full


            if (widthStr != null)
            {
                if (widthStr == "full")
                    width = Texture.Width;
                else if (widthStr == "half")
                    width = Texture.Width / 2;
                else if (widthStr[0] == '/')
                    width = Texture.Width / int.Parse(widthStr.Substring(1));
                else
                    width = int.Parse(widthStr);
            }
            else
            {
                width = Texture.Width;
            }

            if (heightStr != null)
            {
                if (heightStr == "full")
                    height = Texture.Height;
                else if (heightStr == "half")
                    height = Texture.Height / 2;
                else if (heightStr[0] == '/')
                    height = Texture.Height / int.Parse(heightStr.Substring(1));
                else
                    height = int.Parse(heightStr);
            }
            else
            {
                height = Texture.Height;
            }
            int row = 0, column = 0;
            string rowStr = (attrib["row"] != null) ? attrib["row"].Value : null;
            if (rowStr != null)
            {
                row = int.Parse(rowStr);
            }
            else
            {
                row = 0;
            }

            string colStr = (attrib["column"] != null) ? attrib["column"].Value : null;
            if (colStr != null)
            {
                column = int.Parse(colStr);
            }

            float renderingOrder = 0.0f;
            string roStr = (attrib["renderng_order"] != null) ? attrib["renderng_order"].Value : null;
            if (roStr != null)
            {
                renderingOrder = float.Parse(roStr);
            }

            return new Sprite(Texture, row, column, width, height, renderingOrder);
        }

        public static object AnimatedSpriteFromXmlNode(XmlNode node, Game game)
        {

            XmlAttributeCollection attrib = node.Attributes;

            string textureName = attrib["texture"].Value;
            if (textureName == null) throw new ArgumentNullException("<AnimatedSprite ...></AnimatedSprite> must have at least a texture name!");

            Texture2D Texture = game.Content.Load<Texture2D>(textureName);

            string widthStr = (attrib["width"] != null) ? attrib["width"].Value : null;
            string heightStr = (attrib["height"] != null) ? attrib["height"].Value : null;
            int width, height;
            if (widthStr != null)
            {
                if (widthStr == "full")
                    width = Texture.Width;
                else if (widthStr == "half")
                    width = Texture.Width / 2;
                else if (widthStr[0] == '/')
                    width = Texture.Width / int.Parse(widthStr.Substring(1));
                else
                    width = int.Parse(widthStr);
            }
            else
            {
                width = Texture.Width;
            }

            if (heightStr != null)
            {
                if (heightStr == "full")
                    height = Texture.Height;
                else if (heightStr == "half")
                    height = Texture.Height / 2;
                else if (heightStr[0] == '/')
                    height = Texture.Height / int.Parse(heightStr.Substring(1));
                else
                    height = int.Parse(heightStr);
            }
            else
            {
                height = Texture.Height;
            }

            uint renderingOrder = 0;
            string roStr = (attrib["renderng_order"] != null) ? attrib["renderng_order"].Value : null;
            if (roStr != null)
            {
                renderingOrder = uint.Parse(roStr);
            }

            //Parsing frames
            XmlNodeList framesNodes = node.ChildNodes;
            if (framesNodes.Count == 0) throw new ArgumentException("An animated frame must have at least one <Frame...></Frame> element!");

            List<AnimationFrame> frames = new List<AnimationFrame>(framesNodes.Count);
            foreach (XmlNode frameNode in framesNodes)
            {
                XmlAttributeCollection collection = frameNode.Attributes;

                //All these three attributes MUST be present!
                int row = int.Parse(collection["row"].Value);
                int column = int.Parse(collection["column"].Value);
                float timeBetween = float.Parse(collection["time"].Value);

                frames.Add(new AnimationFrame(column, row, timeBetween));
            }

            return new AnimatedSprite(Texture, frames, width, height, renderingOrder);
        }

        public static object GUILabelFromXmlNode(XmlNode node, Game game)
        {

            XmlAttributeCollection attributes = node.Attributes;
            string labelText = (attributes["label"] != null) ? attributes["label"].Value.Replace("\\n", "\n") : "";
            SpriteFont font = (attributes["font"] != null && attributes["font"].Value != "default") ? game.Content.Load<SpriteFont>(attributes["font"].Value) : GUILabel.DefaultSpriteFont;
            float depth = (attributes["rendering_depth"] != null) ? float.Parse(attributes["rendering_depth"].Value) : 0.0f;
            Color color = (Color)ColorFromXmlAttribute(attributes["color"]);
            Vector2 Center = Vector2FromXmlAttribute(attributes["center"], Vector2Proxy.Zero);

            GUILabel Label = new GUILabel(labelText, depth, font, (Transform)TransformFromXmlNode(node.ChildNodes[0], game) );
            return Label;

        }

        /// <summary>
        /// Setups this component from an XML node.
        /// Schema:
        /// [texture="texture name"]    | Setups the GUITextBox's sprite component with this sprite.
        /// [label="string"]            | Setups this GUITextBox's label. If none is present, the default "Text!" string is used.
        /// [offset="n n"]              | Setups this GUITextBox's label offset. If none is present, the default Vector2.Zero value is used.
        /// [font="font name"]          | Setups this GUITextBox's <see cref="SpriteFont"/>. If not present, GUILabel.DefaultSpriteFont will be used.
        /// If a <see cref="Transform"/> child node is present, the Transform property of this component will be setup accordingly
        /// </summary>
        /// <param name="node"></param>
        /// <param name="game"></param>
        public static object GUITextBoxFromXmlNode(XmlNode node, Game game)
        {
            XmlAttributeCollection attributes = node.Attributes;
            Texture2D texture = game.Content.Load<Texture2D>(attributes["texture"].Value);
            Sprite sprite = new Sprite(texture, 0, 0, texture.Width, texture.Height, 0.0f);
            SpriteFont font = (attributes["font"] != null) ? game.Content.Load<SpriteFont>(Path.Combine("Fonts", attributes["font"].Value)) : GUILabel.DefaultSpriteFont;

            Vector2 Offset = Vector2FromXmlAttribute(attributes["offset"], Vector2Proxy.Zero);
            Transform transform = (Transform)TransformFromXmlNode(node.ChildNodes[0], game);
            string labelText = (attributes["label"] != null) ? attributes["label"].Value : "Text!";
            GUITextBox result = new GUITextBox(texture, labelText, transform);
            result.SetFont(font);
            return result;
        }

        public static object GUIButtonFromXmlNode(XmlNode node, Game game)
        {
            XmlAttributeCollection attributes = node.Attributes;
            string textureName = attributes["texture"].Value;
            if (textureName == null) throw new ArgumentException("<GUIButton ...></GUIButton> must have at least a texture name!");
            Texture2D theTexture = game.Content.Load<Texture2D>(textureName);

            GUIButton button = new GUIButton(theTexture);
            button.Sprite.RenderingOrder = (attributes["rendering_order"] != null) ? float.Parse(attributes["rendering_order"].Value) : 0;
            button.Extension = button.Sprite.Size;
            return button;
        }

        public static object GUIImageFromXmlNode(XmlNode node, Game game)
        {
            XmlAttributeCollection attribs = node.Attributes;
            string textureName = (attribs["texture"] != null) ? attribs["texture"].Value : throw new ArgumentNullException("<GUIImage ...></GUIImage> must have a texture attribute!");
            Texture2D spriteTexture = game.Content.Load<Texture2D>(textureName);
            return new GUIImage(spriteTexture);

        }

    }
}
