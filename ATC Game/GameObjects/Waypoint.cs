using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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
        public Vector2 position;
        public string name;
        private bool _is_active;
        // state (active/deactive)
        public bool is_active;
        private ButtonState _previous_state;


        /// <summary>
        /// Class for all_waypoints.
        /// </summary>
        public Waypoint(Game1 game, Vector2 position, string name)
        {
            this._game = game;
            this.position = position;
            this.name = name;
            this._texture = GetTexture();
            this._active_texture = GetActiveTexture();
            this.is_active = false;
            this._previous_state = Mouse.GetState().LeftButton;
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
        protected virtual Rectangle GetWPSquare()
        {
            return new Rectangle((int)this.position.X - 12 + 400, (int)this.position.Y - 12 + 50, 24, 24);
        }

        /// <summary>
        /// Update click event state of the landing waypoint.
        /// </summary>
        public virtual void UpdateState()
        {
            if (GetWPSquare().Contains(this._game.mouse.Position) && this._game.mouse.LeftButton == ButtonState.Pressed && this._previous_state == ButtonState.Released)
                SwitchState();
            this._previous_state = this._game.mouse.LeftButton;
        }

        /// <summary>
        /// Switch active state between true and false if you call this method.
        /// </summary>
        protected virtual void SwitchState()
        {
            if (this.is_active)
                this.is_active = false;
            else
                this.is_active = true;
        }


        /// <summary>
        /// Return position of top left corner of the texture.
        /// </summary>
        /// <returns></returns>
        private Vector2 GetTexturePos ()
        {
            float x_pos = this.position.X - this._texture.Width / 2;
            float y_pos = this.position.Y - this._texture.Height / 2;
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// TexDraw a waypoint texture.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (this.is_active)
                spriteBatch.Draw(this._active_texture, GetTexturePos(), Config.bg_color);
            else
                spriteBatch.Draw(this._texture, GetTexturePos(), Config.bg_color);
        }

    }
}
