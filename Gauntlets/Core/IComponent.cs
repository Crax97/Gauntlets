using System;
using System.Xml;

using Microsoft.Xna.Framework;
using Gauntlets.Core;

namespace Gauntlets
{

    public interface IComponent : ICloneable
	{
		void Initialize();
		void Update(float deltaTime, Entity parent);
		void Destroy();

        void SetupFromXmlNode(XmlNode node, Game game);
	}

}
