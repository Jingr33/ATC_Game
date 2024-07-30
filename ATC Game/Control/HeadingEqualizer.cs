using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.Control
{
    /// <summary>
    /// This is a class for create a trajectory of a airplane heading changes.
    /// </summary>
    internal class HeadingEqualizer
    {
        private Airplane _airplane;
        private int desire_heading;
        private float _actual_heading;
        private float _actual_x_pos;
        private float _actual_y_pos;

        public HeadingEqualizer(Airplane airplane)
        {
            _airplane = airplane;
            desire_heading = _airplane.heading;
            _actual_heading = _airplane.heading;
            _actual_x_pos = _airplane.center_position.X;
            _actual_y_pos = _airplane.center_position.Y;
        }

        /// <summary>
        /// Equalize a desired and actual heading of a plane. Create all trajectory of a turn.
        /// </summary>
        public void Equalize(int desire_heading, GameTime game_time)
        {
            this.desire_heading = desire_heading;
            _airplane.trajectory = new ConcurrentQueue<Vector2> { };
            GenerateTrajectoryAsync();
        }

        /// <summary>
        /// Generate async all points of trajectory of a turn of the airplane.
        /// </summary>
        /// <param name="heading_step"></param>
        private void GenerateTrajectoryAsync(float heading_step = 0.02f)
        {
            float one_step = OneHeadingChange(heading_step);
            _actual_heading = _airplane.heading;
            _actual_x_pos = _airplane.center_position.X;
            _actual_y_pos = _airplane.center_position.Y;
            while ((int)_actual_heading != desire_heading)
            {
                _actual_heading = HeadingBordersCheck(_actual_heading + one_step);
                Vector2 direction = General.GetDirection((int)_actual_heading);
                _actual_x_pos = _actual_x_pos + direction.X * 0.0166667f; // last number is const due to fps optimalization
                _actual_y_pos = _actual_y_pos + direction.Y * 0.0166667f;
                _airplane.trajectory.Enqueue(new Vector2(_actual_x_pos, _actual_y_pos));
                _airplane.heading_queue.Enqueue((int)_actual_heading);
            }
        }

        /// <summary>
        /// Decides, if one step of heading goes up or down and return the step value.
        /// </summary>
        /// <param name="step">absolute step size</param>
        /// <returns></returns>
        private float OneHeadingChange(float step)
        {
            if (this._airplane.heading <= this.desire_heading)
            {
                float right_turn = this.desire_heading - this._airplane.heading;
                float left_turn = this._airplane.heading + 360 - desire_heading;
                if (right_turn <= left_turn) return step;
                return -step;
            }
            else
            {
                float left_turn = this._airplane.heading - this.desire_heading;
                float right_turn = 360 - this._airplane.heading + this.desire_heading;
                if (left_turn <= right_turn) return -step;
                return step;
            }
            //TODO
            //if (_airplane.heading <= desire_heading) return step;
            //return -step;
        }

        /// <summary>
        /// Check if the heading value is between 0 and 360, return modied value if it is necessary.
        /// </summary>
        /// <param name="heading">heading value for check</param>
        /// <returns>check resp. modified heading value</returns>
        private float HeadingBordersCheck (float heading)
        {
            if (heading >= 0 && heading < 360) return heading;
            else if (heading < 0) return heading + 360;
            else return heading - 360;
        }
    }
}
