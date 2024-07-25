using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.Control
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
            _airplane = airplane;
            _desired_altitude = airplane.altitude;
            time = 0;
        }

        /// <summary>
        /// Check if the _time has passed, then change the altitude of the airplane by one flight level nearer to desired altitude.
        /// </summary>
        /// <param name="game_time">game _time</param>
        /// <param name="desired_alt">altitude setted in the altitude controler</param>
        public void EqualizeAltitude(GameTime game_time, int desired_alt)
        {
            _desired_altitude = desired_alt;
            time += (float)game_time.ElapsedGameTime.TotalSeconds;
            if (time >= Config.alt_step_time)
            {
                int diff = Math.Abs(_desired_altitude - _airplane.altitude);
                // fast equalizing
                if (diff > 500)
                {
                    _airplane.altitude += OneFLChange();
                    time = 0;
                }
                // slow equalizing
                else if (diff <= 500 && time >= Config.alt_step_time * 2)
                {
                    _airplane.altitude += OneFLChange();
                    time = 0;
                }
            }

        }

        /// <summary>
        /// Decides, if the flightlevel change goes up or down.
        /// </summary>
        /// <returns>one flight level altitude in feet</returns>
        private int OneFLChange()
        {
            if (_airplane.altitude <= _desired_altitude) return 100;
            return -100;
        }
    }
}
