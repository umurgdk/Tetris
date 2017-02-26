using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Otaku.Rendering;

namespace Otaku
{
    public class Scene : IScene
    {
        public SceneState State { get; set; }
        public List<IEntity> Entities { get; }
        public Color BackgroundColor { get; }

        public Scene()
        {
            Entities = new List<IEntity>();
            BackgroundColor = Color.Black;
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public void AddEntity(IEntity entity)
        {
            entity.Scene = this;
            Entities.Add(entity);

            if (State == SceneState.Play)
                entity.Start();
        }

        public void RemoveEntity(IEntity entity)
        {
            Entities.Remove(entity);
        }

        public T FindEntity<T>(string name) where T : IEntity
        {
            return (T)Entities.FirstOrDefault(e => e.Name == name);
        }

        public virtual void Update(GameTime gameTime)
        {
            Entities.ForEach(e => e.Update(gameTime));
        }

        public virtual void Render(RenderingContext renderingContext)
        {
            Entities.ForEach(e => e.Render(renderingContext));
        }
    }
}