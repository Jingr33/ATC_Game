using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.Control
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
        public HeadingEqualizer heading_equal;
        // mouse
        private MouseState mouse;

        public ReactionDelayer(Game1 game, Airplane airplane)
        {
            _game = game;
            _airplane = airplane;
            SetActualFlightState();
            _speed_equal = new SpeedEqualizer(_airplane);
            _alt_equal = new AltitudeEqualizer(_airplane);
            heading_equal = new HeadingEqualizer(_airplane);
        }

        /// <summary>
        /// Set actual flight values (speed, altitude and heading) of the airplane as desired values for airplanes controls.
        /// </summary>
        public void SetActualFlightState ()
        {
            desired_speed = _airplane.speed;
            desired_alt = _airplane.altitude;
            desired_heading = _airplane.heading;
            _setted_heading = desired_heading;
        }

        /// <summary>
        /// Check if any flight value was changed.
        /// </summary>
        public void UpdateReaction(GameTime game_time)
        {
            if (!this._airplane.autopilot_on)
            {
                UpdateSpeed(game_time);
                UpdateAltitude(game_time);
                if (!this._airplane.heading_autopilot_on)
                    UpdateHeading();
            }
        }

        /// <summary>
        /// Check if speed value was changed. Start the speed equalizer.
        /// </summary>
        private void UpdateSpeed(GameTime game_time)
        {
            if (this.desired_speed != this._airplane.speed)
                this._speed_equal.EqualizeSpeed(game_time, this.desired_speed);
        }

        /// <summary>
        /// Check if altitude value was changed. Start the altitude equalizer.
        /// </summary>
        /// <param name="game_time">game time</param>
        private void UpdateAltitude(GameTime game_time)
        {
            if (this.desired_alt != this._airplane.altitude)
                this._alt_equal.EqualizeAltitude(game_time, this.desired_alt);
        }

        /// <summary>
        /// Check if the heading value is change. if yes, block the heading controler and start heading equalizer.
        /// </summary>
        /// <param name="game_time">game time</param>
        private void UpdateHeading()
        {
            mouse = Mouse.GetState();
            if (this.desired_heading != this._setted_heading && mouse.LeftButton == ButtonState.Released)
            {
                this._airplane.heading_autopilot_on = true;
                this.heading_equal.Equalize(desired_heading);
                this._setted_heading = desired_heading;
            }
            // enabled controler of heading if the heading is already equalized
            if (this.desired_heading == _airplane.heading)
                this._airplane.heading_autopilot_on = false;
        }
    }
}
