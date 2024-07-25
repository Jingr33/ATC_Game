using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.GameObjects.AirplaneFeatures.ReactionDelay
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
            this._airplane = airplane;
            this.desire_heading = this._airplane.heading;
            this._actual_heading = this._airplane.heading;
            this._actual_x_pos = this._airplane.center_position.X;
            this._actual_y_pos = this._airplane.center_position.Y;
        }

        /// <summary>
        /// Equalize a desired and actual heading of a plane. Create all trajectory of a turn.
        /// </summary>
        public void Equalize (int desire_heading, GameTime game_time)
        {
            this.desire_heading = desire_heading;
            this._airplane.trajectory = new ConcurrentQueue<Vector2> { };
            GenerateTrajectoryAsync();
        }

        /// <summary>
        /// Generate async all points of trajectory of a turn of the airplane.
        /// </summary>
        /// <param name="heading_step"></param>
        private void GenerateTrajectoryAsync (float heading_step = 0.02f)
        {
            float one_step = OneHeadingChange(heading_step);
            this._actual_heading = this._airplane.heading;
            this._actual_x_pos = this._airplane.center_position.X;
            this._actual_y_pos = this._airplane.center_position.Y;
            while ((int)this._actual_heading != this.desire_heading)
            {
                this._actual_heading = this._actual_heading + one_step;
                Vector2 direction = General.GetDirection((int)this._actual_heading);
                this._actual_x_pos = this._actual_x_pos + direction.X * 0.0166667f; // last number is const due to fps optimalization
                this._actual_y_pos = this._actual_y_pos + direction.Y * 0.0166667f;
                this._airplane.trajectory.Enqueue(new Vector2(this._actual_x_pos, this._actual_y_pos));
                this._airplane.heading_queue.Enqueue((int)this._actual_heading);
            }
        }

        /// <summary>
        /// Decides, if one step of heading goes up or down and return the step value.
        /// </summary>
        /// <param name="step">absolute step size</param>
        /// <returns></returns>
        private float OneHeadingChange (float step)
        {
            if (this._airplane.heading <= this.desire_heading) return step;
            return -step;
        }
    }
}
