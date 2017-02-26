using Microsoft.Xna.Framework;
using Otaku.Rendering;

namespace Otaku
{
    public interface IEntity
    {
        bool Alive { get; set; }
        string Name { get; set; }
        IScene Scene { get; set; }
        Transform Transform { get; }

        void Start();
        void Update(GameTime gameTime);
        void Render(RenderingContext renderingContext);
    }
}