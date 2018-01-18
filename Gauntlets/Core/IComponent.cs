using System;
using System.Xml;

using Microsoft.Xna.Framework;
namespace Gauntlets
{

	public enum Component
	{
		SPRITE,
        ANIMATED_SPRITE,
		TRANSFORM,
		GAMEOBJECT,
        GUI_ELEMENT
    }

    public interface IComponent : ICloneable
	{
		void Initialize();
		void Update(float deltaTime);
		void Destroy();

        void SetupFromXmlNode(XmlNode node, Game game);

		Component getComponent();
		String getComponentName();
	}
}
