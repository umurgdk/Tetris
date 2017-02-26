using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Otaku.Rendering;

namespace Otaku
{
    public enum SceneState
    {
        Enter,
        LoadContent,
        Play
    }

    public interface IScene
    {
        SceneState State { get; set; }
        List<IEntity> Entities { get; }
        Color BackgroundColor { get; }

        void LoadContent(ContentManager contentManager);

        void Enter();
        void Exit();

        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);
        T FindEntity<T>(string name) where T: IEntity;

        void Update(GameTime gameTime);
        void Render(RenderingContext renderingContext);
    }
}