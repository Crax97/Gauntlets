using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml;
namespace Gauntlets.Core
{
	public class Transform : IComponent
	{

        public static Vector2 WindowHalfSize { get; private set; } = Vector2.Zero;

		private Vector2 position;
		private Vector2 scale;
		private float rotation;

		private Transform parent = null;
		private List<Transform> childs;

        public Transform() {
            position = WindowHalfSize;
            rotation = 0.0f;
            scale = Vector2.One;
            childs = new List<Transform>();
        }

		public static void UpdateGraphicsSize(GraphicsDeviceManager manager)
		{
			WindowHalfSize = new Vector2(manager.PreferredBackBufferWidth / 2, manager.PreferredBackBufferHeight / 2);

		}

		public Transform(Vector2? position = null, Vector2? scale = null, float angle = 0.0f)
		{
			this.position = (position == null ? WindowHalfSize : (Vector2)position);
			this.scale = (scale == null ? Vector2.One : (Vector2)scale);
			rotation = angle;

			childs = new List<Transform>(0);
		}

		public Matrix getMVPMatrix()
		{
			return Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateRotationX(rotation) * Matrix.CreateTranslation(position.X, position.Y, 1);
		}

		public Vector2 LocalPosition 
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

		public Vector2 PositionInCameraSpace
		{
			get
			{
				return position;
			}
		}

		public Vector2 LocalScale
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
		}

		public void Translate(Vector2 translation)
		{
			position += ( translation ) ;

			foreach (Transform child in childs)
			{
				child.Translate(translation);
			}

		}

		public void Scale(Vector2 scale)
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
		public Component getComponent() {
			return Component.TRANSFORM;
		}

		public string getComponentName() {
			return "Transform";
		}

        public void setupFromXmlNode(XmlNode node) {
            
        }

		public void Update(float delta) { }
		public void Initialize() { }
		public void Destroy() { }

        public void SetupFromXmlNode(XmlNode node, Game game)
        {

            XmlAttributeCollection attributes = node.Attributes;
            string[] positionCoords = (attributes["position"] != null) ? attributes["position"].Value.Split(' ') : null;
            string[] scaleCoords = (attributes["scale"] != null) ? attributes["scale"].Value.Split(' ') : null;
            string rotationStr = (attributes["rotation"] != null) ? attributes["rotation"].Value : null;

            rotation = (rotationStr != null) ? (float.Parse(rotationStr)) : 0.0f;
            position = (positionCoords != null) ? new Vector2(float.Parse(positionCoords[0]), float.Parse(positionCoords[1])) + WindowHalfSize : WindowHalfSize;
            scale = (scaleCoords != null) ? new Vector2(float.Parse(scaleCoords[0]), float.Parse(scaleCoords[1])) : Vector2.One;
        }

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
    }
}
