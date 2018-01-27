using System;
using System.Xml;

using Microsoft.Xna.Framework;
using CraxAwesomeEngine.Core;

namespace CraxAwesomeEngine
{

    public interface IComponent : ICloneable
	{
		void Initialize(Entity owner);
		void Update(float deltaTime, Entity parent);
		void Destroy();
	}

}
