using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Color = Microsoft.Xna.Framework.Color;

namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class describing all game objects.
    /// </summary>
    public class GameObject
    {
        protected Vector2 _center_position;
        public GameObject(Vector2 center_position)
        {
            this._center_position = center_position;
        }

        /// <summary>
        /// Get center touch_down_position of object in Vector2.
        /// </summary>
        /// <returns>Position of object center.</returns>
        public Vector2 GetCenterPosition()
        {
            return this._center_position;
        }

        /// <summary>
        /// Get vector touch_down_position of draw point of an game object texture.
        /// </summary>
        /// <param name="center_position">center touch_down_position of a game object</param>
        /// <param name="texture">texture of a game object</param>
        /// <returns>top left touch_down_position of texture</returns>
        public virtual Vector2 GetTexturePosition(Vector2 center_position, Texture2D texture)
        {
            float x_pos = center_position.X - texture.Width / 2;
            float y_pos = center_position.Y - texture.Height / 2;
            return new Vector2(x_pos, y_pos);
        }
    }
}
