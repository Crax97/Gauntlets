﻿using System.Xml;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Gauntlets.Core
{
	public static partial class SpriteBatchExtensor
	{

        public static void DrawEntity(this SpriteBatch batch, Entity entity, GameTime time)
        {
            if (entity.Enabled)
            {
                List<Sprite> sprites = entity.GetComponents<Sprite>();
                foreach (Sprite sprite in sprites)
                {
                    if (sprite.IsVisible)
                        batch.Draw(sprite.Texture, entity.Transform.PositionInCameraSpace,
                                   sprite.Source, Color.White, entity.Transform.Rotation, sprite.SpriteCenter,
                                    entity.Transform.LocalScale, SpriteEffects.None, sprite.RenderingOrder);
                }
            }
        }
	}

    public class Entity : ICloneable
	{
        private int instanceCount = 0;
		private Transform transform;
		private bool isEnabled = true;
        public int InstanceId { get; private set; }
        public int Id { get; private set; }
        public string Name { get; set; }

        private static List<Entity> knownEntities;
		List<IComponent> components;

        protected Entity() : this(new Transform())
		{
		}

        private Entity(Transform t) {
            components = new List<IComponent>();
            transform = t;
            InstanceId = 0;
            Id = 0;
            Name = "";
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:Gauntlets.Core.Entity"/>,
        /// comparing them by Id.
        /// </summary>
        /// <param name="e">The <see cref="object"/> to compare with the current <see cref="T:Gauntlets.Core.Entity"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="T:Gauntlets.Core.Entity"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object e) {
            Entity other = e as Entity;
            return (other != null ) ? (other.Id == this.Id) : false;

        }

        /// <summary>
        /// Checks if the Entity's instanceId is the same as 
        /// compare's instanceId.
        /// </summary>
        /// <returns><c>true</c>, if the instanceIds are the same, <c>false</c> otherwise.</returns>
        /// <param name="compare">The other entity.</param>
        public bool IsSameInstance(Entity compare) {
            return this.InstanceId == compare.InstanceId;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public static void InitializeEntities(string EntitiesFile, Game game)
		{

            using (FileStream file = File.Open(EntitiesFile, FileMode.Open))
            {

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(file, settings);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                XmlNodeList entities = doc["Entities"].ChildNodes;

                knownEntities = new List<Entity>(entities.Count);

                foreach(XmlNode entity in entities){
                    int id = int.Parse(entity.Attributes["Id"].Value);
                    string name = entity.Attributes["Name"].Value;

                    Entity e = new Entity();
                    e.Id = id;
                    e.Name = name;

                    XmlNodeList components = entity["Components"].ChildNodes;

                    foreach(XmlNode componentNode in components) {
                        string componentType = componentNode.LocalName;

                        //Transform doesn't count, as every entity has one
                        if (componentType != "Transform")
                        {
                            IComponent component = ComponentAttribute.GetInstanceOfType(componentType);
                            component.SetupFromXmlNode(componentNode, game);
                            e.AddComponent(component);
                        } else 
                        {
                            e.Transform.SetupFromXmlNode(componentNode, game);
                        }
                    }

                    knownEntities.Add(e);
                }


            }

		}

        /// <summary>
        /// Adds the selected IComponent to the Entity's components list.
        /// Throws ArgumentException if the IComponent's type is Component.TRANSFORM
        /// </summary>
        /// <param name="c">The IComponent to be added.</param>
		public void AddComponent(IComponent c)
		{
            //Only one transform can be attached to an Entity!
            if (c.getComponent() == Component.TRANSFORM) 
                throw new ArgumentException("Entites can only have one transform!");
			components.Add(c);
		}

        /// <summary>
        /// Gets the componets of T type.
        /// If T.getComponent() == Component.TRANSFORM, the method returns
        /// a list with the entity's single transform.
        /// </summary>
        /// <returns>The requested components.</returns>
        /// <typeparam name="T">The type requested.</typeparam>
        public List<T> GetComponents<T>() where T : class, IComponent, new()
		{
			T comparator = new T();
			List<T> elems = new List<T>();

            //Transform shouldn't be inside the component list, but if the users requests it
            //through the GetComponents<T> method, we just create a dummy list with the
            //(single)Transform and return it back.
            if (comparator.getComponent() == Component.TRANSFORM)
            {
                elems.Add(Transform as T);
                return elems;
            }

			foreach (IComponent c in components)
			{
				if (c.getComponent() == comparator.getComponent())
					elems.Add((T)c);
			}

			return elems;
		}

        public static Entity Instantiate(int id) {
            int i = 0;
            while(i < knownEntities.Count ) {
                if(knownEntities[i].Id == id) {
                    Entity e = (Entity)knownEntities[i].Clone();
                    e.Name = e.Name + " " + e.InstanceId;
                    World.Current.AddEntity(e);
                    i = knownEntities.Count + 1;
                    return e;
                }
                i++;
            }

            throw new ArgumentException("Cannot find requested Entity!");
            
        }

		public virtual void OnBegin()
		{
			foreach (IComponent c in components)
			{
				c.Initialize();
			}
		}
		public virtual void Update(float deltaTime)
		{
            if (Enabled)
            {
                foreach (IComponent c in components)
                {
                    c.Update(deltaTime);
                }
            }
		}
		public virtual void OnDestroy()
		{
			foreach (IComponent c in components)
			{
				c.Destroy();
			}
		}

		public void Destroy()
		{
			OnDestroy();
            World.Current.RemoveEntity(this);
		}

		public Transform Transform
		{
			get
			{
				return transform;
			}
		}

		public bool Enabled
		{
			get { return isEnabled; }
			set { isEnabled = value; }
		}

		private void ParseEntity(int id)
		{
			
		}

        public object Clone()
        {
            Transform t = (Transform)transform.Clone();
            Entity e = new Entity(t);   
            e.Id = this.Id;
            e.Name = this.Name;
            e.InstanceId = ++this.InstanceId;

            foreach(ICloneable cloneable in components) {
                    e.AddComponent((IComponent)cloneable.Clone());
            }

            return e;

        }
    }
}
