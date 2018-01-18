using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Gauntlets.Core
{
    public class GUIElement : Entity, IComponent
    {

        public GUIElement() {
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
            throw new NotSupportedException("How the fuck where you able to call this function?");
        }

        public virtual object Clone()
        {
            return new NotSupportedException("GUIElements cannot be cloned directly");
        }


        public Sprite Sprite { get; protected set; }

    }
}
