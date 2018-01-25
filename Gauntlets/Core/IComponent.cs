using System;
using System.Xml;

using Microsoft.Xna.Framework;
using CraxEngine.Core;

namespace CraxEngine
{

    public interface IComponent : ICloneable
	{
		void Initialize();
		void Update(float deltaTime, Entity parent);
		void Destroy();
	}

}
