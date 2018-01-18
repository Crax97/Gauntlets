using System;
using Gauntlets.Core;

namespace Gauntlets.Core
{
	public interface IGUIElement
	{

        Sprite Sprite {
            get;
        }

        Transform Transform {
            get;            
        }

	}
}
