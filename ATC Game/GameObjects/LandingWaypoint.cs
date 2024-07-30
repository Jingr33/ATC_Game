using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using Point = System.Drawing.Point;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework;



namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class for landing waypoint.
    /// It is waypoint for every runway. Behind this point the plane goes to land with automatic land system.
    /// </summary>
    public class LandingWaypoint : Waypoint
    {
        private Game1 _game;
        private Vector2 _position;
        private Vector2 _draw_position;
        private Texture2D _texture;
        private Texture2D _active_texture;
        private string _name;
        private Runway _runway;
        private int _heading;
        // state (active/deactive)
        public bool is_active;
        private ButtonState _previous_state;

        public LandingWaypoint (Game1 game, Vector2 position, string name, Runway runway) 
            : base(game, position, name)
        {
            this._game = game;
            this._position = position;
            this._name = name;
            this._texture = GetTexture();
            this._active_texture = GetActiveTexture();
            this._draw_position = General.GetDrawPosition(this._position, this._texture);
            this._runway = runway;
            this._heading = runway.heading;
            this.is_active = false;
            this._previous_state = Mouse.GetState().LeftButton;
        }

        /// <summary>
        /// Load texture of landing waypoint
        /// </summary>
        /// <returns>texture of non-active landing waypoint</returns>
        private Texture2D GetTexture()
        {
            return this._game.Content.Load<Texture2D>("landing_waypoint");
        }

        /// <summary>
        /// Load texture of landing waypoint in active state.
        /// </summary>
        /// <returns>texture of active landing waypoint</returns>
        private Texture2D GetActiveTexture()
        {
            return this._game.Content.Load<Texture2D>("landing_waypoint_active");
        }
        
        /// <summary>
        /// Landing waypoint active state switcher (if you call the function state is switched).
        /// </summary>
        protected override void SwitchState()
        {
            if (this.is_active)
                this.is_active = false;
            else
                this.is_active = true;
        }
        /// <summary>
        /// Draw a landing waypoint.
        /// </summary>
        /// <param name="spriteBatch">sprite batch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.is_active)
                spriteBatch.Draw(this._active_texture, this._draw_position, Config.bg_color);
            else
                spriteBatch.Draw(this._texture, this._draw_position, Config.bg_color);
            Console.WriteLine(this.is_active.ToString());
        }
    }
}
