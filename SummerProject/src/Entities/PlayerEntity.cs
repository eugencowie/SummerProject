using Artemis;
using Microsoft.Xna.Framework;

namespace SummerProject
{
    static class PlayerEntity
    {
        public static int nextPlayerId = 1;

        public static Entity BuildEntity(EntityWorld entityManager)
        {
            Entity entity = entityManager.CreateEntity();
            entity.Tag = "player";

            // The block component contains data about the block.
            entity.AddComponent(new PlayerComponent() {
                PlayerId = nextPlayerId,
                LocalPlayer = true
            });

            // The transform component contains position, size and orientation data.
            entity.AddComponent(new TransformComponent() {
                Size = new Vector2(40, 40)
            });

            // The renderable component tells the rendering system to render the block.
            entity.AddComponent(new RenderableComponent());

            nextPlayerId++;
            return entity;
        }
    }
}
