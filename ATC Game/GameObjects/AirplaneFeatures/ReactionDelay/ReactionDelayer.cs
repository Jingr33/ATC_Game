using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.GameObjects.AirplaneFeatures.ReactionDelay
{
    /// <summary>
    /// this class create delay if the player change flight values.
    /// </summary>
    public class ReactionDelayer
    {
        private Game1 _game;
        private Airplane _airplane;
        //speed
        public int desired_speed;
        SpeedEqualizer _speed_equal;
        // altitude
        public int desired_alt;
        AltitudeEqualizer _alt_equal;
        // heading
        public int desired_heading;
        private int _setted_heading;
        HeadingEqualizer _heading_equal;
        // mouse
        private MouseState mouse;

        public ReactionDelayer(Game1 game, Airplane airplane)
        {
            this._game = game;
            this._airplane = airplane;
            this.desired_speed = this._airplane.speed;
            this.desired_alt = this._airplane.altitude;
            this.desired_heading = this._airplane.heading;
            this._setted_heading = this.desired_heading;
            this._speed_equal = new SpeedEqualizer(this._airplane);
            this._alt_equal = new AltitudeEqualizer(this._airplane);
            this._heading_equal = new HeadingEqualizer(this._airplane);
        }

        /// <summary>
        /// Check if any flight value was changed.
        /// </summary>
        public void UpdateReaction (GameTime game_time)
        {
            UpdateSpeed(game_time);
            UpdateAltitude(game_time);
            UpdateHeading();
        }

        /// <summary>
        /// Check if speed value was changed. Start the speed equalizer.
        /// </summary>
        private void UpdateSpeed (GameTime game_time)
        {
            if (this.desired_speed != this._airplane.speed)
                this._speed_equal.EqualizeSpeed(game_time, this.desired_speed);
        }

        private void UpdateAltitude (GameTime game_time)
        {
            if (this.desired_alt != this._airplane.altitude)
                this._alt_equal.EqualizeAltitude(game_time, this.desired_alt);
        }

        private void UpdateHeading ()
        {
            this.mouse = Mouse.GetState();
            if (this.desired_heading != this._setted_heading && this.mouse.LeftButton == ButtonState.Released)
            {
                this._heading_equal.Equalize(this.desired_heading);
                this._setted_heading = this.desired_heading;
            }
        }
    }
}
