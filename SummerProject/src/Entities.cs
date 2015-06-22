using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    /// <summary>
    /// This class contains helper functions for creating and managing entities.
    /// </summary>
    static class Entities
    {
        private static int playerCounter;

        /// <summary>
        /// Create a player entity.
        /// </summary>
        public static Entity CreatePlayer(
            EntityWorld entityWorld,
            ContentManager content,
            string group,
            string tag,
            Vector2 position,
            bool localPlayer)
        {
            Entity player = entityWorld.CreateEntity();

            if (!string.IsNullOrEmpty(group))
                player.Group = group;

            if (!string.IsNullOrEmpty(tag))
                player.Tag = tag;

            player.AddComponent(new Transform {
                Position = position,
                Size = Vector2.One,
                Rotation = 0f
            });

            player.AddComponent(new Sprite {
                Texture = content.Load<Texture2D>("textures/objects/player"),
                Effects = SpriteEffects.None,
                LayerDepth = LayerDepth.Player
            });

            player.AddComponent(new PlayerInfo {
                PlayerId = playerCounter++,
                LocalPlayer = localPlayer,
                Level = 1,
                Experience = 0,
                Health = new Trait(100, 0),
                Mana = new Trait(100, 0)
            });

            player.AddComponent(new Inventory {
                HasKey = false
            });

            return player;
        }
    }
}
