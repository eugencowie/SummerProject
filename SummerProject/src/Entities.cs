using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    /// <summary>
    /// This class contains extension methods for creating and managing entities.
    /// </summary>
    static class Entities
    {
        /// <summary>
        /// Used to keep track of the number of players in order to assign unique
        /// player IDs.
        /// </summary>
        private static int playerCount;


        /// <summary>
        /// Extension method for creating an entity with optional group and tag
        /// parameters.
        /// </summary>
        public static Entity CreateEntity(
            this EntityWorld entityWorld,
            string group = default(string),
            string tag = default(string))
        {
            Entity entity = entityWorld.CreateEntity();

            if (!string.IsNullOrEmpty(group))
                entity.Group = group;

            if (!string.IsNullOrEmpty(tag))
                entity.Tag = tag;

            return entity;
        }


        /// <summary>
        /// Extension method for creating a player.
        /// </summary>
        public static Entity AddPlayerComponents(
            this Entity entity,
            ContentManager content,
            Vector2 position = default(Vector2),
            bool localPlayer = default(bool))
        {
            entity.AddComponent(new Transform {
                Position = position,
                Size = Vector2.One,
                Rotation = 0f
            });

            entity.AddComponent(new Sprite {
                Texture = content.Load<Texture2D>("textures/objects/player"),
                Effects = SpriteEffects.None,
                LayerDepth = LayerDepth.Player
            });

            entity.AddComponent(new PlayerInfo {
                PlayerId = playerCount++,
                LocalPlayer = localPlayer,
                Level = 1,
                Experience = 0,
                Health = new Trait(100, 0),
                Mana = new Trait(100, 0)
            });

            entity.AddComponent(new Inventory {
                HasKey = false
            });

            entity.AddComponent(new Pathfinder {
                Destination = position,
                Speed = 4f
            });

            return entity;
        }


        /// <summary>
        /// Extension method for creating an enemy.
        /// </summary>
        public static Entity AddEnemyComponents(
            this Entity entity,
            ContentManager content,
            Vector2 position = default(Vector2),
            float rotation = default(float),
            SpriteEffects effects = default(SpriteEffects))
        {
            entity.AddComponent(new Transform {
                Position = position,
                Size = Vector2.One,
                Rotation = rotation
            });

            entity.AddComponent(new Sprite {
                Texture = content.Load<Texture2D>("textures/objects/enemy"),
                Effects = effects,
                LayerDepth = LayerDepth.Player
            });

            entity.AddComponent(new Inventory());

            return entity;
        }


        /// <summary>
        /// Extension method for creating a background tile.
        /// </summary>
        public static Entity AddBackgroundTileComponents(
            this Entity entity,
            ContentManager content,
            Vector2 position = default(Vector2),
            float rotation = default(float),
            string texture = default(string),
            SpriteEffects effects = default(SpriteEffects))
        {
            entity.AddComponent(new Transform {
                Position = position,
                Size = Vector2.One,
                Rotation = rotation
            });

            entity.AddComponent(new Sprite {
                Texture = content.Load<Texture2D>(texture),
                Effects = effects,
                LayerDepth = LayerDepth.Background
            });

            return entity;
        }


        /// <summary>
        /// Extension method for creating an object tile.
        /// </summary>
        public static Entity AddObjectTileComponents(
            this Entity entity,
            ContentManager content,
            Vector2 position = default(Vector2),
            float rotation = default(float),
            string tileTexture = default(string),
            SpriteEffects effects = default(SpriteEffects))
        {
            entity.AddComponent(new Transform {
                Position = position,
                Size = Vector2.One,
                Rotation = rotation
            });

            entity.AddComponent(new Sprite {
                Texture = content.Load<Texture2D>(tileTexture),
                Effects = effects,
                LayerDepth = LayerDepth.Object
            });

            return entity;
        }
    }
}
