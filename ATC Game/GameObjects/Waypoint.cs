using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace ATC_Game.GameObjects
{
    public class Waypoint
    {
        private Game1 _game;
        private Texture2D _texture;
        private Texture2D _active_texture;
        private Vector2 _position;
        private string _name;
        private bool _is_active;

        /// <summary>
        /// Class for waypoints.
        /// </summary>
        public Waypoint(Game1 game, Vector2 position, string name)
        {
            this._game = game;
            this._position = position;
            this._name = name;
            this._texture = GetTexture();
            this._active_texture = GetActiveTexture();
            this._is_active = false;
        }

        /// <summary>
        /// Load texture of a waypoint.
        /// </summary>
        /// <returns>waypoint texture</returns>
        private Texture2D GetTexture()
        {
            return this._game.Content.Load<Texture2D>("waypoint");
        }

        /// <summary>
        /// Load active texture of a waypoint.
        /// </summary>
        /// <returns>waypoint active texture</returns>
        private Texture2D GetActiveTexture()
        {
            return this._game.Content.Load<Texture2D>("waypoint_active");
        }

        /// <summary>
        /// Create square of waypoint space for event of waypoint activation.
        /// </summary>
        /// <returns>rectangle of a waypoint space</returns>
        private Rectangle GetWPSquare()
        {
            return new Rectangle((int)this._position.X - 12, (int)this._position.Y - 12, 24, 24);
        }

        /// <summary>
        /// Update state of the waypoint (Event).
        /// </summary>
        public void Update ()
        {
            //TODO
        }

        /// <summary>
        /// Return position of top left corner of the texture.
        /// </summary>
        /// <returns></returns>
        private Vector2 GetTexturePos ()
        {
            float x_pos = this._position.X - this._texture.Width / 2;
            float y_pos = this._position.Y - this._texture.Height / 2;
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// Draw a waypoint texture.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, GetTexturePos(), Config.bg_color);
        }

    }
}
