using System.Xml;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;
using CraxAwesomeEngine.Core.Scripting;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Gauntlets.Editor.EntityEditor")]
namespace CraxAwesomeEngine.Core
{

    /// <summary>
    /// The interactive things of a <see cref="World"/>
    /// </summary>
    [MoonSharpUserData]
    [Serializable]
    public class Entity : ICloneable
	{
		private Transform transform;
		private bool isEnabled = true;
        public int InstanceId { get; private set; }
        public int Id { get; private set; }
        public string Name { get; set; }

        internal static List<Entity> knownEntities;
		List<IComponent> components;

        protected Entity() : this(new Transform())
		{
		}

        private Entity(Transform t, string name = "Entity", int id = 0) {
            components = new List<IComponent>();
            transform = t;
            InstanceId = 0;
            Id = id;
            Name = name;
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

        public static void InitializeEntities()
		{

            GameSerializer.DeserializeEntities(out knownEntities);

		}

        /// <summary>
        /// Adds the selected IComponent to the Entity's components list.
        /// Throws ArgumentException if the IComponent's type is Component.TRANSFORM
        /// </summary>
        /// <param name="c">The IComponent to be added.</param>
		public void AddComponent(IComponent c)
		{
            //Only one transform can be attached to an Entity!
            if (c.GetType() == typeof(Transform)) 
                throw new ArgumentException("Entites can only have one transform!");
			components.Add(c);
            c.Initialize(this);
		}

        /// <summary>
        /// Gets the componets of T type.
        /// If T.getComponent() == Component.TRANSFORM, the method returns
        /// a list with the entity's single transform.
        /// </summary>
        /// <returns>The requested components of type T, an empty list if there's none.</returns>
        /// <typeparam name="T">The type requested.</typeparam>
        public List<T> GetComponents<T>(bool allowInherited = true) where T : class, IComponent
        {
            List<T> elems = new List<T>();
            //If we're looking for a Transform
            if (typeof(T) == typeof(Transform))
            {
                elems.Add(Transform as T);
                return elems;
            }
            foreach (IComponent component in components)
            {

                if (typeof(T) == component.GetType() || (component is T && allowInherited)) elems.Add(component as T);

            }

            return elems;
        }

        /// <summary>
        /// Gets the componets of type typename.
        /// If typename == "Transform", the method returns
        /// a list with the entity's single transform.
        /// </summary>
        /// <returns>The requested components of type typeName, an empty list if there's none.</returns>
        /// <typeparam name="typeName">The type requested.</typeparam>
        public List<object> GetComponents(string typeName, bool allowInherited = true)
        {
            var type = ComponentRecord.GetAssociatedType(typeName);
            var elems = new List<object>();
            //If we're looking for a Transform
            if (type == typeof(Transform))
            {
                elems.Add(Transform);
                return elems;
            }

            foreach (IComponent component in components)
            {

                if (type == component.GetType() || (component.GetType().IsSubclassOf(type) && allowInherited))
                    elems.Add(component );

            }

            return elems;
        }

        /// <summary>
        /// Gets the first component of type typeName,
        /// useful in lua scripts.
        /// </summary>
        /// <param name="typeName">The searched IComponent type name</param>
        /// <returns>The first component found, null if none</returns>
        public object GetComponent(string typeName, bool allowInherited = true)
        {
            var type = ComponentRecord.GetAssociatedType(typeName);
            //If we're looking for a Transform
            if (type == typeof(Transform))
                return Transform;

            foreach (IComponent component in components)
            {

                if (type == component.GetType() || (component.GetType().IsSubclassOf(type) && allowInherited))
                    return component;

            }
            return null;
        }

        /// <summary>
        /// Searches for the first component of type T in the Entity's components list.
        /// </summary>
        /// <typeparam name="T">The component to search for.</typeparam>
        /// <returns>The first component of type T, null if there's none</returns>
        public T GetComponent<T>(bool allowInherited = true) where T : class, IComponent
        {

            //If we're looking for a Transform
            if (typeof(T) == typeof(Transform))
                return Transform as T;

            foreach (IComponent component in components)
            {
                
                if (typeof(T) == component.GetType() || (component is T && allowInherited)) return component as T;
                    
            }
            return null;
        }

        /// <summary>
        /// Clones the instance identified by the id
        /// and adds it to the current GameWorld,
        /// throws if it can't find the entity
        /// </summary>
        /// <param name="id">The identifier of the entity to clone</param>
        /// <returns>The cloned entity, throws if it can't find it</returns>
        public static Entity Instantiate(int id) {
            int i = 0;
            while(i < knownEntities.Count ) {
                if(knownEntities[i].Id == id) {
                    Entity e = (Entity)knownEntities[i].Clone();
                    e.Name += e.InstanceId;
                    World.Current.AddEntity(e);
                    i = knownEntities.Count + 1;
                    return e;
                }
                i++;
            }

            throw new ArgumentException("Cannot find requested Entity!");
            
        }

        /// <summary>
        /// Called when the simulation of
        /// the current GameWorld begins
        /// </summary>
		public virtual void OnBegin()
		{
			foreach (IComponent c in components)
			{
				c.Initialize(this);
			}
		}

        /// <summary>
        /// Called when the simulation of the current world updates,
        /// </summary>
        /// <param name="deltaTime">The time between the current frame and the last</param>
		public virtual void Update(float deltaTime)
		{
            if (Enabled)
            {
                foreach (IComponent c in components)
                {
                    c.Update(deltaTime, this);
                }
            }
		}

        /// <summary>
        /// Called when the entity is destroyed.
        /// </summary>
		public virtual void OnDestroy()
		{
			foreach (IComponent c in components)
			{
				c.Destroy();
			}
		}
        
        /// <summary>
        /// Destroys the current entity.
        /// </summary>
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
        
        public object Clone()
        {
            Transform t = (Transform)transform.Clone();
            Entity e = new Entity(t, this.Name, this.Id);   
            e.InstanceId = ++this.InstanceId;

            foreach(ICloneable cloneable in components) {
                    e.components.Add((IComponent)cloneable.Clone());
            }

            return e;

        }
    }
}
