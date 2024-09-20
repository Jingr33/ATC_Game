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
using Microsoft.Xna.Framework.Media;



namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class for landing waypoint.
    /// It is waypoint for every land_runway. Behind this point the plane goes to land with automatic land system.
    /// </summary>
    public class LandingWaypoint : Waypoint
    {
        private Game1 _game;
        public Vector2 position;
        private Texture2D _texture;
        private Texture2D _active_texture;
        private string _name;
        public Runway runway;
        private int _heading;
        public Vector2[] turn_center_pos;
        public bool is_active;
        private ButtonState _previous_state;

        public LandingWaypoint (Game1 game, Vector2 position, string name, Runway runway) 
            : base(game, position, name)
        {
            this._game = game;
            this.position = position;
            this._name = name;
            this._texture = GetTexture();
            this._active_texture = GetActiveTexture();
            this.runway = runway;
            this._heading = runway.heading;
            this.turn_center_pos = new Vector2[2];
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
        /// It sets the final turn center _points of the landing waypoint on both sides of the lwp.
        /// </summary>
        /// <param name="turn_radius">radius of the final turn</param>
        public void SetTurnCenterPositions(float turn_radius)
        {
            Vector2 rwy_direciton = General.GetDirection(this.runway.heading);
            Vector2 direction0 = new Vector2(-rwy_direciton.Y, rwy_direciton.X);
            Vector2 direction1 = new Vector2(rwy_direciton.Y, -rwy_direciton.X);
            this.turn_center_pos[0] = new Vector2(this.position.X + turn_radius * direction0.X, this.position.Y + turn_radius * direction0.Y);
            this.turn_center_pos[1] = new Vector2(this.position.X + turn_radius * direction1.X, this.position.Y + turn_radius * direction1.Y);
        }
        
        /// <summary>
        /// Landing waypoint active state switcher (if you call the function state is switched).
        /// </summary>
        protected override void SwitchState()
        {
            this.is_active = General.Switcher(this.is_active);
        }

        /// <summary>
        /// TexDraw a landing waypoint.
        /// </summary>
        /// <param name="spriteBatch">sprite batch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(this._texture.Width / 2, this._texture.Height / 2);
            if (this.is_active)
                spriteBatch.Draw(this._active_texture, this.position, null,  Config.bg_color, General.GetRotation(this.runway.heading), origin, 1, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(this._texture, this.position, null, Config.bg_color, General.GetRotation(this.runway.heading), origin, 1, SpriteEffects.None, 0);
        }
    }
}
