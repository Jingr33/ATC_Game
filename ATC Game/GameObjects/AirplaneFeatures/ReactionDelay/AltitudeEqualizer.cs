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
    /// Class for equalization the desired and original altitude.
    /// </summary>
    internal class AltitudeEqualizer
    {
        private Airplane _airplane;
        private int _desired_altitude;
        private float time;
        
        public AltitudeEqualizer(Airplane airplane)
        {
            this._airplane = airplane;
            this._desired_altitude = airplane.altitude;
            this.time = 0;
        }

        /// <summary>
        /// Check if the time has passed, then change the altitude of the airplane by one flight level nearer to desired altitude.
        /// </summary>
        /// <param name="game_time">game time</param>
        /// <param name="desired_alt">altitude setted in the altitude controler</param>
        public void EqualizeAltitude (GameTime game_time, int desired_alt)
        {
            this._desired_altitude = desired_alt;
            this.time += (float)game_time.ElapsedGameTime.TotalSeconds;
            if (this.time >= Config.alt_step_time)
            {
                int diff = Math.Abs(this._desired_altitude - this._airplane.altitude);
                // fast equalizing
                if (diff > 500)
                {
                    this._airplane.altitude += OneFLChange();
                    time = 0;
                }
                // slow equalizing
                else if (diff <= 500 && (this.time >= (Config.alt_step_time * 2)))
                {
                    this._airplane.altitude += OneFLChange();
                    time = 0;
                }
            }

        }

        /// <summary>
        /// Decides, if the flightlevel change goes up or down.
        /// </summary>
        /// <returns>one flight level altitude in feet</returns>
        private int OneFLChange ()
        {
            if (this._airplane.altitude <= this._desired_altitude) return 100;
            return -100;
        }
    }
}
