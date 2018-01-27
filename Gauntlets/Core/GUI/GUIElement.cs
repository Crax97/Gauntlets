using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace CraxEngine.Core.GUI
{
    public class GUIElement : IComponent
    {

        public GUIElement(Transform transform = null, Color? color = null) {
            Transform = (transform != null) ? transform : new Transform();
            Color = (color != null) ? (Color)color : Color.White;
        }

        public void Initialize(Entity owner)
        {
        }

        public virtual object Clone()
        {
            throw new NotSupportedException("GUIElements cannot be cloned directly! You forgot to override GUIElement.Clone() maybe?");
        }
        
        public virtual void Update(float deltaTime, Entity parent)
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
