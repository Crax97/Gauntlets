using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Gauntlets.Core
{
    public class GUIElement : IComponent
    {

        public GUIElement() {
            Transform = new Transform();
            Color = Color.White;
        }

        public Component getComponent()
        {
            return Component.GUI_ELEMENT;
        }

        public string getComponentName()
        {
            return "GUI Element";
        }

        public void Initialize()
        {
        }

        public virtual void SetupFromXmlNode(XmlNode node, Game game) {
            return;
        }

        public virtual object Clone()
        {
            throw new NotSupportedException("GUIElements cannot be cloned directly");
        }

        public virtual void Update(float deltaTime)
        {
            return;
        }

        public virtual void Destroy()
        {
            return;
        }

        internal virtual void Draw(SpriteBatch batch, Entity parent) { return; }

        public Sprite Sprite { get; protected set; }
        public Transform Transform { get; protected set; }
        public Color Color { get; set; }

    }
}
