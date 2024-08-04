using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.Control
{
    /// <summary>
    /// This is a class for create a trajectory of a airplane heading changes.
    /// </summary>
    public class HeadingEqualizer
    {
        private Airplane _airplane;
        private int desired_heading;
        private float _actual_heading;
        private float _actual_x_pos;
        private float _actual_y_pos;

        public HeadingEqualizer(Airplane airplane)
        {
            _airplane = airplane;
            desired_heading = _airplane.heading;
            _actual_heading = _airplane.heading;
            _actual_x_pos = _airplane.center_position.X;
            _actual_y_pos = _airplane.center_position.Y;
        }

        /// <summary>
        /// Equalize a desired and actual heading of a plane. Create all trajectory of a turn.
        /// </summary>
        /// <param name="desire_heading">desired heading for straight flight of the airpolane to the destination wyapoint.</param>
        /// <param name="game_time"> game time</param>
        public void Equalize(int desire_heading, GameTime game_time)
        {
            this.desired_heading = desire_heading;
            _airplane.trajectory = new ConcurrentQueue<Vector2> { };
            Task.Run(() => GenerateTrajectoryAsync());
        }

        /// <summary>
        /// Generate trajectory of a flight to waypoint.
        /// </summary>
        /// <param name="WP">destination waypoint object instance</param>
        /// <param name="heading_step">one heading change</param>
        public void GenToWPTrajectory(Waypoint WP, float heading_step = 0.02f)
        {
            this._actual_heading = this._airplane.heading;
            this._actual_x_pos = this._airplane.center_position.X;
            this._actual_y_pos = this._airplane.center_position.Y;
            this._airplane.trajectory = new ConcurrentQueue<Vector2> { };
            this.desired_heading = HeadingToWP(WP.position);
            Task.Run(() => GenToWPTrajectoryAsync(WP, heading_step));
        }

        /// <summary>
        /// Generate trajectory of a flight to waypoint async.
        /// </summary>
        /// <param name="WP">waypoint destination objecgt</param>
        /// <param name="heading_step">one frame heading step</param>
        private void GenToWPTrajectoryAsync (Waypoint WP, float heading_step)
        {
            float one_step = OneHeadingChange(heading_step);
            bool wp_reached = false;
            while (!wp_reached)
                if (this.desired_heading != (int)this._actual_heading)
                    StraightenDirection(WP, one_step);
                else
                    wp_reached = FlightStraight(WP);
        }

        /// <summary>
        /// Straighten direction of a flight by one step.
        /// </summary>
        /// <param name="WP">waypoint object (destination of flight)</param>
        /// <param name="one_step">size of one heading shift</param>
        private void StraightenDirection (Waypoint WP, float one_step)
        {
            this.desired_heading = HeadingToWP(WP.position);
            SetNextTurnPoint(one_step);
        }

        /// <summary>
        /// Create next flight point of the airplane in the straight line of travel. Return bool, if the destination point was already reached.
        /// </summary>
        /// <param name="WP">waypoint object, destination of flight</param>
        /// <returns>if the destination was reached</returns>
        private bool FlightStraight (Waypoint WP)
        {
            Vector2 flight_direc = General.GetDirection(this.desired_heading);
            SetNextStraightPoint(flight_direc);
            bool wp_reached = General.ObjectReachedPoint(new Vector2(this._actual_x_pos, this._actual_y_pos), WP.position);
            return wp_reached;
        }

        /// <summary>
        /// Generate async all points of trajectory of a turn of the airplane.
        /// </summary>
        /// <param name="heading_step">one heading change</param>
        private void GenerateTrajectoryAsync(float heading_step = 0.02f)
        {
            float one_step = OneHeadingChange(heading_step);
            _actual_heading = _airplane.heading;
            _actual_x_pos = _airplane.center_position.X;
            _actual_y_pos = _airplane.center_position.Y;
            while ((int)_actual_heading != desired_heading)
                SetNextTurnPoint(one_step);
        }

        /// <summary>
        /// Change all flight parameters for next point of trajectory. Return new posint, for check the position.
        /// </summary>
        /// <param name="one_step">heading step change</param>
        private void SetNextTurnPoint (float one_step)
        {
            _actual_heading = HeadingBordersCheck(_actual_heading + one_step);
            Vector2 direction = General.GetDirection((int)_actual_heading);
            _actual_x_pos = _actual_x_pos + direction.X * 0.0166667f; // last number is const due to fps optimalization
            _actual_y_pos = _actual_y_pos + direction.Y * 0.0166667f;
            _airplane.trajectory.Enqueue(new Vector2(this._actual_x_pos, this._actual_y_pos));
            _airplane.heading_queue.Enqueue((int)_actual_heading);
        }

        /// <summary>
        /// Add new point and new heading in trajectory queue in straight direction and return this point.
        /// </summary>
        private void SetNextStraightPoint (Vector2 direction)
        {
            this._actual_x_pos = this._actual_x_pos + direction.X * 0.0166667f; // last number is const due to fps optimalization
            this._actual_y_pos = this._actual_y_pos + direction.Y * 0.0166667f;
            Vector2 new_pos = new Vector2(this._actual_x_pos, this._actual_y_pos);
            this._airplane.trajectory.Enqueue(new_pos);
            this._airplane.heading_queue.Enqueue(this.desired_heading);
        }

        /// <summary>
        /// Decides, if one step of heading goes up or down and return the step value.
        /// </summary>
        /// <param name="step">absolute step size</param>
        /// <returns></returns>
        private float OneHeadingChange(float step)
        {
            if (this._airplane.heading <= this.desired_heading)
            {
                float right_turn = this.desired_heading - this._airplane.heading;
                float left_turn = this._airplane.heading + 360 - desired_heading;
                if (right_turn <= left_turn) return step;
                return -step;
            }
            else
            {
                float left_turn = this._airplane.heading - this.desired_heading;
                float right_turn = 360 - this._airplane.heading + this.desired_heading;
                if (left_turn <= right_turn) return -step;
                return step;
            }
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

        /// <summary>
        /// Return heading of the plane to elected waypoint.
        /// </summary>
        /// <param name="WP_pos">vector position of waypoint</param>
        /// <returns>heading value</returns>
        private int HeadingToWP (Vector2 WP_pos)
        {
            Vector2 coord_dist = new Vector2(WP_pos.X - this._actual_x_pos, WP_pos.Y - this._actual_y_pos);
            return General.GetHeading(coord_dist);
        }
    }
}
