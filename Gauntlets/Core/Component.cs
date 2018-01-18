using System;
using Microsoft.Xna.Framework;
using System.Xml;
namespace Gauntlets.Core
{
    public class Component
    {
        public Component()
        {
        }

        public abstract static IComponent CreateFromXmlNode(XmlNode node, Game game);
}
