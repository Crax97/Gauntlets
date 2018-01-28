using CraxAwesomeEngine.Content.Scripts.Proxies;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Xml;
namespace CraxAwesomeEngine.Core
{
    
    [MoonSharpUserData]
    public class Transform : IComponent
	{

        public static Vector2Proxy WindowHalfSize { get; private set; } = Vector2Proxy.Zero;

		private Vector2Proxy position;
		private Vector2Proxy scale;
		private float rotation;

		private Transform parent = null;
		private List<Transform> childs;

        public Transform() {
            position = WindowHalfSize;
            rotation = 0.0f;
            scale = Vector2Proxy.One;
            childs = new List<Transform>();
        }

		public static void UpdateGraphicsSize(GraphicsDeviceManager manager)
		{
			WindowHalfSize = new Vector2Proxy(manager.PreferredBackBufferWidth / 2, manager.PreferredBackBufferHeight / 2);

		}

		public Transform(Vector2Proxy position = null, Vector2Proxy scale = null, float angle = 0.0f)
		{
			this.position = (position == null ? WindowHalfSize : (Vector2Proxy)position);
			this.scale = (scale == null ? Vector2Proxy.One : (Vector2Proxy)scale);
			rotation = angle;

			childs = new List<Transform>(0);
		}

		public Matrix getMVPMatrix()
		{
			return Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateRotationX(rotation) * Matrix.CreateTranslation(position.X, position.Y, 1);
		}

		public Vector2Proxy LocalPosition 
		{
			get{
				return position - WindowHalfSize;
			}
            set {
                position = value + WindowHalfSize;
            }
		}

		public Vector2 getWorldPosition()
		{
			Vector2 pos = Vector2.Zero;
			Transform current = this;
			while (current != null)
			{
				pos += current.LocalPosition;
				current = current.parent;
			}

			return pos;
		}

		public Vector2Proxy Position
		{
			get
			{
				return position;
			}
            set
            {
                position = value;
            }
		}

		public Vector2Proxy LocalScale
		{ 
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

		public float Rotation
		{
			get
			{
				return rotation;
			}
            set
            {
                rotation = value;
            }
		}

		public void Translate(Vector2Proxy translation)
		{
			position += ( translation ) ;

			foreach (Transform child in childs)
			{
				child.Translate(translation);
			}

		}

        public void Translate(float X, float Y)
        {
            position += new Vector2Proxy(X, Y);
        }

		public void Scale(Vector2Proxy scale)
		{
			this.scale += scale;
		}

		public void Rotate(float rotation)
		{
			this.rotation += rotation;
			while (this.rotation >= 180) this.rotation -= 180;
			while (this.rotation <= -180) this.rotation += 180;
		}

		public void SetParent(Transform otherParent)
		{
			parent = otherParent;
			parent.childs.Add(this);
		}

		public void AddChild(Transform otherChild)
		{

			otherChild.parent = this;
			childs.Add(otherChild);

		}

        public void Update(float deltaTime, Entity parent) { }
		public void Initialize(Entity owner) { }
		public void Destroy() { }

        public object Clone()
        {
            Transform t = new Transform();
            t.position = this.position;
            t.rotation = this.rotation;
            t.scale = this.scale;
            t.parent = this.parent;

            foreach (Transform child in childs) 
            {
                t.AddChild((Transform)child.Clone());
            }

            return t;

        }

        public void SetupFromXmlNode(XmlNode node, Game game)
        {
            if (node != null)
            {
                XmlAttributeCollection attributes = node.Attributes;

                string rotationStr = (attributes["rotation"] != null) ? attributes["rotation"].Value : null;
                rotation = (rotationStr != null) ? (float.Parse(rotationStr)) : 0.0f;

                position = XmlComponentsReaders.Vector2FromXmlAttribute(attributes["position"], Vector2Proxy.Zero);
                scale = XmlComponentsReaders.Vector2FromXmlAttribute(attributes["position"], Vector2Proxy.One);
            }
        }
        
    }
}
