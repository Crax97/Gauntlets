using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using CraxEngine.Core.GUI;

namespace CraxEngine.Core { 

	public class World
	{

		List<Entity> entities;
        public static World Current { get; private set; } = null;


		public World()
		{
			entities = new List<Entity>(0);
            if (Current == null) Current = this;
		}

		public void AddEntity(Entity e)
		{
			entities.Add(e);
            e.OnBegin();
		}

		public void RemoveEntity(Entity e)
		{
			foreach (Entity entity in entities)
			{
                if (entity.Equals(e))
                {
                    entities.Remove(entity);
                    entity.OnDestroy();
                    break;
                }
			}
		}

		public void BeginSimulation()
		{
			foreach (Entity e in entities)
			{
				e.OnBegin();
			}
		}

		public void Update(float delta)
		{

			foreach (Entity e in entities)
			{
				if(e.Enabled)
					e.Update(delta);
			}

		}

		public void EndSimulation()
		{
			foreach (Entity e in entities)
			{
				e.OnDestroy();
			}
		}

		public void DrawSprites(SpriteBatch batch, GameTime time)
		{
			foreach (Entity e in entities)
			{
				batch.DrawEntity(e, time);
			}
        }

        public void DrawGUI(SpriteBatch batch) {

            var EnabledEntities = from Entity in entities
                                  where Entity.Enabled
                                  select Entity;

            foreach (Entity e in EnabledEntities)
            {
                List<GUIElement> elements = e.GetComponents<GUIElement>();
                foreach(GUIElement element in elements) {
                    element.Draw(batch, e);
                }
            }
        }

        public List<Entity> GetEntitiesByName(string name)
        {
            var result = from Entity in entities
                         where Entity.Name == name
                         select Entity;

            return result.ToList();
        }

        public List<Entity> GetEntitiesById(int id)
        {
            var result = from entity in entities
                          where entity.Id == id
                          select entity;

            return result.ToList();
        }

	}
}
