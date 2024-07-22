using Microsoft.Xna.Framework;
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
        // heading

        public ReactionDelayer(Game1 game, Airplane airplane)
        {
            this._game = game;
            this._airplane = airplane;
            this.desired_speed = this._airplane.speed;
            this._speed_equal = new SpeedEqualizer(this._airplane);
        }

        /// <summary>
        /// Check if any flight value was changed.
        /// </summary>
        public void UpdateReaction (GameTime game_time)
        {
            UpdateSpeed(game_time);
        }

        /// <summary>
        /// Check if speed value was changed. Start the speed equalizer.
        /// </summary>
        private void UpdateSpeed (GameTime game_time)
        {
            if (this.desired_speed != this._airplane.speed)
                this._speed_equal.EqualizeSpeed(game_time, this.desired_speed);
        }
    }
}
