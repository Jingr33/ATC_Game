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
    /// Class for equalization the desired and original speed.
    /// </summary>
    internal class SpeedEqualizer
    {
        private Airplane _airplane;
        private int _desired_speed;
        private float _time;

        public SpeedEqualizer(Airplane airplane)
        {
            _airplane = airplane;
            _desired_speed = _airplane.speed;
            _time = 0;
        }

        /// <summary>
        /// Equalize actual airplane speed with desired speed in _time
        /// </summary>
        /// <param name="game_time">game _time</param>
        /// <param name="desired_speed">setted speed in speed controler</param>
        public void EqualizeSpeed(GameTime game_time, int desired_speed)
        {
            _time += (float)game_time.ElapsedGameTime.TotalSeconds;
            _desired_speed = desired_speed;
            if (_time >= Config.speed_step_time)
            {
                int diff = Math.Abs(_desired_speed - _airplane.speed);
                // fast equalizing
                if (diff > 5)
                {
                    _airplane.speed += OneStepChange();
                    _time = 0;
                }
                // slow equalizing
                else if (diff <= 5 && _time >= Config.speed_step_time + 1 - diff / 5)
                {
                    _airplane.speed += OneStepChange();
                    _time = 0;
                }
            }
        }

        /// <summary>
        /// Return value for change of speed by one step.
        /// </summary>
        /// <returns></returns>
        private int OneStepChange()
        {
            if (_airplane.speed <= _desired_speed) return 1;
            return -1;
        }
    }
}
