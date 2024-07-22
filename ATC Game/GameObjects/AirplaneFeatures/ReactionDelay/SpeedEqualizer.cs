using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.GameObjects.AirplaneFeatures.ReactionDelay
{
    /// <summary>
    /// Class for equalization the desired and original speed.
    /// </summary>
    internal class SpeedEqualizer
    {
        private Airplane _airplane;
        private int _desired_speed;
        private float time;

        public SpeedEqualizer(Airplane airplane)
        {
            this._airplane = airplane;
            this._desired_speed = this._airplane.speed;
            this.time = 0;
        }

        /// <summary>
        /// Equalize actual airplane speed with desired speed in time
        /// </summary>
        /// <param name="game_time">game time</param>
        /// <param name="desired_speed">setted speed in speed controler</param>
        public void EqualizeSpeed (GameTime game_time, int desired_speed)
        {
            this.time += (float)game_time.ElapsedGameTime.TotalSeconds;
            this._desired_speed = desired_speed;
            if (this.time >= Config.speed_step_time)
            {
                int diff = Math.Abs(this._desired_speed - this._airplane.speed);
                // fast equalizing
                if (diff > 5)
                {
                    this._airplane.speed += OneStepChange();
                    time = 0;
                }
                // slow equalizing
                else if (diff <= 5 && (this.time >= (Config.speed_step_time + 1 - diff/5)))
                {
                    this._airplane.speed += OneStepChange();
                    time = 0;
                }
            }
        }

        /// <summary>
        /// Return value for change of speed by one step.
        /// </summary>
        /// <returns></returns>
        private int OneStepChange ()
        {
            if (this._airplane.speed <= this._desired_speed) return 1;
            return -1;
        }
    }
}
