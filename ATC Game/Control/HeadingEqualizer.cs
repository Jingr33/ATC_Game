using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ATC_Game.Control
{
    /// <summary>
    /// This is a class for create a trajectory of a airplane actual_head changes.
    /// </summary>
    public class HeadingEqualizer
    {
        private Airplane _airplane;
        private int desired_heading;
        private float _actual_heading;
        private float _actual_x_pos;
        private float _actual_y_pos;
        private float _time;
        public HeadingEqualizer(Airplane airplane)
        {
            _airplane = airplane;
            desired_heading = _airplane.heading;
            _actual_heading = _airplane.heading;
            _actual_x_pos = _airplane.center_position.X;
            _actual_y_pos = _airplane.center_position.Y;
            this._time = 0;
        }

        /// <summary>
        /// Equalize a desired and actual actual_head of a plane. Create all trajectory of a turn.
        /// </summary>
        /// <param name="desire_heading">desired actual_head for straight flight of the airpolane to the destination wyapoint.</param>
        /// <param name="game_time"> game _time</param>
        public void Equalize(int desire_heading)
        {
            this.desired_heading = desire_heading;
            _airplane.trajectory = new ConcurrentQueue<Vector2> { };
            Task.Run(() => GenerateTrajectoryAsync());
        }

        /// <summary>
        /// Generate trajectory of a flight to waypoint.
        /// </summary>
        /// <param name="WP">destination waypoint object instance</param>
        /// <param name="heading_step">one actual_head change</param>
        public void GenToWPTrajectory(Waypoint WP, float heading_step = 0.02f)
        {
            SetActualAirplaneState();
            this.desired_heading = HeadingToWP(WP.position);
            Task.Run(() => GenToWPTrajectoryAsync(WP, heading_step));
        }

        /// <summary>
        /// Generate a trajectory of a flight to a landing act_position already with land_runway actual_head.
        /// </summary>
        /// <param name="LWP">landing waypoint object</param>
        /// <param name="heading_step">one step of actual_head change</param>
        public void GetToLWPTrajectory(LandingWaypoint LWP, float heading_step = 0.02f)
        {
            SetActualAirplaneState();
            this.desired_heading = HeadingToWP(LWP.position);
            Task.Run(() => GenToLWPTrajectoryAsync(LWP, heading_step));
        }

        /// <summary>
        /// Change heading of the airplane in _time to lead it to the its land_runway.
        /// </summary>
        /// <param name="airplane_pos">actual touch_down_position of the airplane</param>
        /// <param name="rwy_pos">touch_down_position of the treshold of the land_runway</param>
        /// <param name="game_time">game _time</param>
        public void LeadToRunway (Vector2 airplane_pos, Vector2 rwy_pos, GameTime game_time)
        {
            this._time += (float)game_time.ElapsedGameTime.TotalSeconds;
            float head_to_rwy = General.GetHeading(rwy_pos - airplane_pos); // tady zkus udelat neco lepsiho
            if (this._time > 0.4 && head_to_rwy != this._airplane.heading)
            {
                int heading_change = (int)OneHeadingChange(1, this._airplane.heading, head_to_rwy);
                this._airplane.heading += heading_change;
                this._time = 0;
            }
        }

        /// <summary>
        /// Create circle trajectory of one holding turn of the airplane.
        /// </summary>
        /// <param name="heading_step">one heading step</param>
        public void OneTurnTrajectory (float heading_step)
        {
            SetActualAirplaneState();
            this.desired_heading = (int)this._actual_heading;
            this._actual_heading += heading_step;
            this._airplane.trajectory = new ConcurrentQueue<Vector2> { };
            this._airplane.heading_queue = new ConcurrentQueue<int> { };
            for (float i = 0; i < 180; i += Math.Abs(heading_step))
            {
                SetNextTurnPoint(heading_step);
                this._actual_heading += heading_step;
            }

        }

        /// <summary>
        /// Set actual states of the airplane.
        /// </summary>
        private void SetActualAirplaneState()
        {
            this._actual_heading = this._airplane.heading;
            this._actual_x_pos = this._airplane.center_position.X;
            this._actual_y_pos = this._airplane.center_position.Y;
            this._airplane.trajectory = new ConcurrentQueue<Vector2> { };
            this._airplane.heading_queue = new ConcurrentQueue<int> { };
        }

        /// <summary>
        /// Generate trajectory of a flight to waypoint async.
        /// </summary>
        /// <param name="wp_position">Posiiton of destination waypoint</param>
        /// <param name="heading_step">one frame actual_head step</param>
        private void GenToWPTrajectoryAsync (Vector2 wp_position, float heading_step)
        {
            bool wp_reached = false;
            while (!wp_reached)
            {
                if (this.desired_heading != Math.Round(this._actual_heading, 1))
                {
                    float one_step = OneHeadingChange(heading_step, this._actual_heading, this.desired_heading);
                    StraightenDirection(wp_position, one_step);
                }
                else
                {
                    wp_reached = FlightStraight(wp_position);
                    // update of desired actual_head from this act_position
                    this.desired_heading = (int)General.HeadingToWaypoint(new Vector2(this._actual_x_pos, this._actual_y_pos), wp_position);
                }
            }
        }
        /// <summary>
        /// Generate trajectory of a flight to waypoint async.
        /// </summary>
        /// <param name="WP">waypoint destination objecgt</param>
        /// <param name="heading_step">one frame actual_head step</param>
        private void GenToWPTrajectoryAsync (Waypoint WP, float heading_step)
        {
            Vector2 wp_position = WP.position;
            GenToWPTrajectoryAsync(wp_position, heading_step);
        }

        /// <summary>
        /// Generate trajectory of a flight to landpoint async.
        /// </summary>
        /// <param name="LWP">landing waypoint object</param>
        /// <param name="heading_step">one frame actual_head step</param>
        private void GenToLWPTrajectoryAsync(LandingWaypoint LWP, float heading_step)
        {
            float turn_radius = CalcRadiusOfTurn(heading_step); // RADIUS of final turn
            (Vector2 final_center_pos, bool is_left_turn) = FindCenterPosOfTurn(turn_radius, LWP);
            Vector2 turn_point = FindFinalTurnPoint(LWP, turn_radius, heading_step, final_center_pos, is_left_turn); // final start turn act_position of final turn
            // _points of the trajectory
            this.desired_heading = HeadingToWP(new Vector2(LWP.position.X - 50, LWP.position.Y - 50)); // desired actual_head is setted to the final turn start act_position
            GenToWPTrajectoryAsync(turn_point, heading_step); // generate first part of trajectory
            this.desired_heading = LWP.runway.heading; // the final turn start act_position is setted to the land_runway actual_head
            heading_step = is_left_turn ? -heading_step : heading_step;
            Console.WriteLine(heading_step.ToString());
            while (this.desired_heading != (int)this._actual_heading) // generate final turn of trajectory 
                SetNextTurnPoint(heading_step);
        }

        /// <summary>
        /// Straighten act_direction of a flight by one step.
        /// </summary>
        /// <param name="WP">waypoint object (destination of flight)</param>
        /// <param name="one_step">size of one actual_head shift</param>
        private void StraightenDirection (Vector2 WP_poisiton, float one_step)
        {
            this.desired_heading = HeadingToWP(WP_poisiton);
            SetNextTurnPoint(one_step);
        }

        /// <summary>
        /// Create next flight act_position of the airplane in the straight line of travel. Return bool, if the destination act_position was already reached.
        /// </summary>
        /// <param name="WP">waypoint object, destination of flight</param>
        /// <returns>if the destination was reached</returns>
        private bool FlightStraight (Vector2 WP_position)
        {
            Vector2 flight_direc = General.GetDirection(this.desired_heading);
            SetNextStraightPoint(flight_direc);
            bool wp_reached = General.ObjectReachedPoint(new Vector2(this._actual_x_pos, this._actual_y_pos), WP_position, 2);
            return wp_reached;
        }

        /// <summary>
        /// Generate async all _points of trajectory of a turn of the airplane.
        /// </summary>
        /// <param name="heading_step">one actual_head change</param>
        private void GenerateTrajectoryAsync(float heading_step = 0.02f)
        {
            float one_step = OneHeadingChange(heading_step, this._actual_heading, this.desired_heading);
            _actual_heading = _airplane.heading;
            _actual_x_pos = _airplane.center_position.X;
            _actual_y_pos = _airplane.center_position.Y;
            while ((int)_actual_heading != desired_heading)
                SetNextTurnPoint(one_step);
        }

        /// <summary>
        /// Change all flight parameters for next act_position of trajectory. Return new posint, for check the touch_down_position.
        /// </summary>
        /// <param name="one_step">actual_head step change</param>
        private void SetNextTurnPoint (float one_step)
        {
            _actual_heading = General.HeadingBordersCheck(_actual_heading + one_step);
            Vector2 direction = General.GetDirection((int)_actual_heading);
            _actual_x_pos = _actual_x_pos + direction.X * 0.0166667f; // last number is const due to fps optimalization
            _actual_y_pos = _actual_y_pos + direction.Y * 0.0166667f;
            _airplane.trajectory.Enqueue(new Vector2(this._actual_x_pos, this._actual_y_pos));
            _airplane.heading_queue.Enqueue((int)_actual_heading);
        }

        /// <summary>
        /// Add new act_position and new actual_head in trajectory queue in straight act_direction and return this act_position.
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
        /// Decides, if one step of actual_head goes up or down and return the step value.
        /// </summary>
        /// <param name="step">absolute step size</param>
        /// <returns>one actual_head change</returns>
        private float OneHeadingChange(float step, float actual_heading, float end_heading)
        {
            if (actual_heading <= end_heading)
            {
                float right_turn = end_heading - actual_heading;
                float left_turn = actual_heading + 360 - end_heading;
                if (right_turn <= left_turn) return step;
                return -step;
            }
            else
            {
                float left_turn =actual_heading - end_heading;
                float right_turn = 360 - actual_heading + end_heading;
                if (left_turn <= right_turn) return -step;
                return step;
            }
        }

        /// <summary>
        /// Return actual_head of the plane to elected waypoint.
        /// </summary>
        /// <param name="WP_pos">vector touch_down_position of waypoint</param>
        /// <returns>actual_head value</returns>
        private int HeadingToWP (Vector2 WP_pos)
        {
            Vector2 coord_dist = new Vector2(WP_pos.X - this._actual_x_pos, WP_pos.Y - this._actual_y_pos);
            return (int)General.GetHeading(coord_dist);
        }

        /// <summary>
        /// Method calculates a radius of final turn to the Landing waypoint.
        /// </summary>
        /// <param name="heading_step">actual_head change between two game frames</param>
        /// <returns>radius of final turn</returns>
        private float CalcRadiusOfTurn(float heading_step)
        {
            //Vector2 one_direc_step = General.GetDirection(heading_step);
            //Vector2 two_direc_step = General.GetDirection(heading_step * 2);
            //// three _points for definition of circle
            //Vector2 x1 = new Vector2(0, 0);
            //Vector2 x2 = new Vector2(one_direc_step.X, one_direc_step.Y);
            //Vector2 x3 = new Vector2(two_direc_step.X, two_direc_step.Y) * 2;
            //// calculation of guidelines of normal of sections between individual _points
            //float gl1 = (x2.Y - x1.Y) / (x2.X - x1.X);
            //float gl2 = (x3.Y - x2.Y) / (x3.X - x2.X);
            //// calculation of a center touch_down_position of a turn circle
            //float centerX = (gl1 * gl2 * (x1.Y - x3.Y) + gl2 * (x1.X + x2.X) - gl1 * (x2.X + x3.X)) / (2 * (gl2 - gl1));
            //float centerY = -1 / gl1 * (centerX - (x1.X + x2.X) / 2) + (x1.Y + x2.Y) / 2;
            //Vector2 center = new Vector2(centerX, centerY);
            //// return the turn radius
            //return Vector2.Distance(center, x1);
            return 48; // test
        }

        /// <summary>
        /// It try to find Point of start the final turn to the landing waypoint. Return this turn act_position or touch_down_position of the plane.
        /// </summary>
        /// <param name="LWP">landing waypoint object</param>
        /// <param name="turn_radius">radius of final turn</param>
        /// <param name="heading_step">one actual_head change between two game frames</param>
        /// <param name="final_center_pos">positon of a start of the final turn before the landing waypoint</param>
        /// <param name="is_left_final">bool value, if the final turn is to left or to right site in the direction of flight (left is true)</param>
        /// <returns>final turn start act_position</returns>
        private Vector2 FindFinalTurnPoint(LandingWaypoint LWP, float turn_radius, float heading_step, Vector2 final_center_pos, bool is_left_final)
        {
            Vector2 position1 = new Vector2(this._actual_x_pos, this._actual_y_pos);
            Vector2 position2 = new Vector2(this._actual_x_pos, this._actual_y_pos);
            float heading1 = this._actual_heading;
            float heading2 = this._actual_heading;
            for (int i = 0; i < (190 / heading_step); i++)
            {
                Vector2 turn_point1 = CalcOneFinalTurnPoint(LWP, heading1, turn_radius, final_center_pos, is_left_final);
                Vector2 turn_point2 = CalcOneFinalTurnPoint(LWP, heading2, turn_radius, final_center_pos, is_left_final);

                // if these are the right _points to ending the first turn check
                if (IsHeadingToThePoint(turn_point1, position1, heading1))
                    return turn_point1;
                else if (IsHeadingToThePoint(turn_point2, position2, heading2))
                    return turn_point2;

                heading1 = General.HeadingBordersCheck(heading1 + heading_step);
                heading2 = General.HeadingBordersCheck(heading2 - heading_step);
                Vector2 direction1 = General.GetDirection(heading1);
                Vector2 direction2 = General.GetDirection(heading2);
                position1.X = position1.X + direction1.X * 0.0166667f; // last number is const due to fps optimalization
                position1.Y = position1.Y + direction1.Y * 0.0166667f; // last number is const due to fps optimalization
                position2.X = position2.X + direction2.X * 0.0166667f;
                position2.Y = position2.Y + direction2.Y * 0.0166667f;
            }
            return new Vector2(this._actual_x_pos, this._actual_y_pos); // if the ending turn act_position wasn'param found
        }

        /// <summary>
        /// It calculates touch_down_position of a start of a final turn to landing waypoint.
        /// </summary>
        /// <param name="LWP">landing waypoint object</param>
        /// <param name="plane_heading">actual heading of the airplane</param>
        /// <param name="turn_radius">radius of final turn</param>
        /// <param name="turn_center">circle center of the final turn</param>
        /// <param name="is_left_turn">bool value, if the final turn is to left or to right site in the direction of flight (left is true)</param>
        /// <returns>act_position of a stat of a final turn of trajectory</returns>
        private Vector2 CalcOneFinalTurnPoint (LandingWaypoint LWP, float plane_heading, float turn_radius, Vector2 turn_center, bool is_left_turn)
        {
            float rotation_angle = GetRotationAngle(plane_heading, LWP.runway.heading);
            Vector2 turn_point = PositionAtCircle(turn_radius, turn_center, LWP.runway.heading, rotation_angle, is_left_turn);
            return turn_point;
        }

        /// <summary>
        /// Calculate the angle rotation in the turn (actual_head change of turn).
        /// </summary>
        /// <param name="turn_radius">radius of the turn</param>
        /// <returns>angle of rotation of turn in degree</returns>
        private float GetRotationAngle(float plane_heading, float heading_in_wp)
        {
            float rot_ang = heading_in_wp - plane_heading;
            if (rot_ang >= 180)
                rot_ang -= 360;
            else if (rot_ang <= -180)
                rot_ang += 360;
            return rot_ang;
        }

        /// <summary>
        /// It calculates center act_position of circle of an airplane turn
        /// </summary>
        /// <param name="turn_radius">radius of the turn</param>
        /// <param name="LWP">landing waypoint object</param>
        /// <returns>center touch_down_position of circle and bool value, if the final turn os to left or to right site (left = true)</returns>
        private (Vector2, bool) FindCenterPosOfTurn (float turn_radius, LandingWaypoint LWP)
        {
            Vector2 flight_direc = Vector2.Normalize(new Vector2(this._actual_x_pos, this._actual_y_pos) - LWP.position);
            Vector2 rwy_direc = General.GetDirection(General.HeadingBordersCheck(this._actual_heading));
            float determinant = VectorDeterminant(rwy_direc, flight_direc);
            LWP.SetTurnCenterPositions(turn_radius);
            Vector2 center_pos = determinant > 0 ? LWP.turn_center_pos[0] : LWP.turn_center_pos[1];
            bool is_left_turn = determinant <= 0;
            //Console.WriteLine(is_left_turn.ToString() + " " + determinant.ToString());
            return (center_pos, is_left_turn);
        }

        /// <summary>
        /// It calculates a act_position at a circle witch some radius. Position of act_position is given by circle radius and a rotation angle.
        /// </summary>
        /// <param name="turn_radius">radius of a turn</param>
        /// <param name="turn_center">center touch_down_position of a circle</param>
        /// <param name="turn_heading">actual_head (angle) of rotation</param>
        /// <returns>touch_down_position of a act_position at the circle</returns>
        private Vector2 PositionAtCircle(float turn_radius, Vector2 turn_center, int rwy_heading, float turn_heading, bool is_left_center)
        {
            float total_heading = General.HeadingBordersCheck(rwy_heading + turn_heading);
            float radian_angle = MathHelper.ToRadians(General.HeadingBordersCheck(360 - turn_heading + 90));
            int turn_direc = is_left_center ? 1 : -1; // this method is for left final turn, if is need to do a right final turn, sign in coords calculation changes
            float x_pos = (float)(turn_center.X + turn_radius * Math.Cos(radian_angle) * turn_direc);
            float y_pos = (float)(turn_center.Y + turn_radius * Math.Sin(radian_angle) * turn_direc);
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// Check if the object actual_head to a turn act_position. Return turn value.
        /// </summary>
        /// <param name="turn_point">turn act_position vector</param>
        /// <returns>bool value</returns>
        private bool IsHeadingToThePoint(Vector2 turn_point, Vector2 actual_pos, float actual_head)
        {
            float distance = (turn_point - actual_pos).Length();
            Vector2 direction = Vector2.Normalize(turn_point - actual_pos);
            float calc_head = General.GetHeading(direction);
            if (Math.Abs(calc_head - actual_head) < 1) return true; // uprav na 0.1
            return false;
            //float k = General.GetGuideline(actual_head);
            //float q = actual_pos.Y - k * actual_pos.X;
            //if (General.PointAndLineDist(turn_point, -k, 1, -q) < 2)
            //    return true;
            //return false;
        }

        /// <summary>
        /// Calculates an angle between two vectors.
        /// </summary>
        /// <param name="lwp_pos">landing waypoint touch_down_position</param>
        /// <param name="plane_pos">plane touch_down_position</param>
        /// <param name="rwy_heading">vector of the land_runway</param>
        /// <returns>a value of the angle</returns>
        private float AngleBetweenVectors (Vector2 lwp_pos, Vector2 plane_pos, float rwy_heading)
        {
            Vector2 v1 = General.GetDirection(rwy_heading);
            Vector2 v2 = plane_pos - lwp_pos;
            float cross = Cross(v1, v2);
            float lenght1 = v1.Length();
            float lenght2 = v2.Length();
            float rad_angle = cross / lenght1 / lenght2;
            return MathHelper.ToDegrees(rad_angle);
        }

        /// <summary>
        /// It calculates a vector cross between two vectors in 2D.
        /// </summary>
        /// <param name="V1">vector 1</param>
        /// <param name="V2">vector 2</param>
        /// <returns>value of the vector cross</returns>
        private float Cross (Vector2 V1, Vector2 V2)
        {
            return V1.X * V2.X + V1.Y * V2.Y;
        }

        /// <summary>
        /// Calculates vector product of two 2D vectors.
        /// </summary>
        /// <param name="A">first vector</param>
        /// <param name="B">second vector</param>
        /// <returns>2D vector product (scalar)</returns>
        private float VectorDeterminant(Vector2 A, Vector2 B)
        {
            return A.X * B.Y - A.Y * B.X;
        }


        /// <summary>
        /// Calculates normalized vector for a line between start act_position and end act_position of a Beziers curve.
        /// </summary>
        /// <param name="P0">start act_position of the curve</param>
        /// <param name="P3">end act_position of the curve</param>
        /// <returns>get normalized vector</returns>
        //private Vector2 FirstLastPointsLineNormal (Vector2 P0, Vector2 P3)
        //{
        //    Vector2 unit_direc = Vector2.Normalize(P3 - P0);
        //    return new Vector2(-unit_direc.Y, unit_direc.X);
        //}

        /// <summary>
        /// Method for calculation inner points of bezier curve.
        /// </summary>
        /// <param name="P0">first act_position of a curve</param>
        /// <param name="D0">start vector of a curve</param>
        /// <param name="P3">last act_position of a curve</param>
        /// <param name="D3">final act_position of a curve</param>
        /// <param name="turn_intens">radius od a turn you want</param>
        /// <returns>vector2 array of two points</returns>
        //private Vector2[] GetInnerPointsOfBezier(Vector2 P0, Vector2 D0, Vector2 P3, Vector2 D3, float turn_intens)
        //{
        //    float k1 = turn_intens; // a turn parameter for every turn in the curve
        //    float k2 = turn_intens;
        //    Vector2 P1 = P0 + D0 * turn_intens;
        //    Vector2 P2 = P3 - D3 * turn_intens; // inner vectors
        //    float kappa_start = 0;
        //    float kappa_end = 100; // curvatures

        //    for (int a = 0; a < 100; a++)
        //    {
        //        // inner _points
        //        P1 = P0 + D0 * turn_intens;
        //        P2 = P3 - D3 * turn_intens;
        //        // some _points on the curve
        //        List<Vector2> curve_points = new List<Vector2>();
        //        for (int t = 0; t < 100; t++)
        //            curve_points.Add(BezierCurvePoint(t, P0, P1, P2, P3));
        //        // derivates (directions) for a curves
        //        List<Vector2> derivates = new List<Vector2>();
        //        for (int i = 1; i < curve_points.Count; i++)
        //            derivates.Add(curve_points[i] - curve_points[i - 1]);
        //        // calculation of curvatures of the _points
        //        List<float> curvatures = new List<float>();
        //        for (int j = 1; j < derivates.Count; j++)
        //        {
        //            Vector2 tangent = derivates[j - 1];
        //            Vector2 tangent_next = curve_points[j];
        //            float curvature = Vector2.Distance(Vector2.Zero, tangent) > 0
        //            ? Math.Abs(VectorDeterminant(tangent, tangent_next)) / (float)Math.Pow(Vector2.Distance(Vector2.Zero, tangent), 3)
        //            : 0.0f;
        //            curvatures.Add(curvature);
        //        }
        //        // average curvature at the start and end of the curve
        //        kappa_start = curvatures.Take(10).Average();
        //        kappa_end = curvatures.Skip(Math.Max(0, curvatures.Count - 10)).Average();
        //        // change of curve parameters
        //        if (kappa_start > kappa_end)
        //            k1 *= 0.99f;
        //        else
        //            k2 *= 0.99f;
        //        //Console.WriteLine("a: " + a.ToString());
        //        Console.WriteLine(a + ": " + k1);
        //        //Console.WriteLine(kappa_start + " start");
        //        //Console.WriteLine(kappa_end + " end");
        //    }
        //    return new Vector2[] {P1, P2};
        //}

        /// <summary>
        /// It calculates one act_position of Bezier curve.
        /// </summary>
        /// <param name="param">position in the curve</param>
        /// <param name="P0">start act_position</param>
        /// <param name="P1">first inner act_position</param>
        /// <param name="P2">second inner act_position</param>
        /// <param name="P3">final act_position</param>
        /// <returns>one act_position on the Bezier curve</returns>
        //private Vector2 BezierCurvePoint (float param, Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3)
        //{
        //    return (float)Math.Pow(1 - param, 3) * P0 + 3 * (float)Math.Pow(1 - param, 2) * param * P1 + 3 * (1 - param) * (float)Math.Pow(param, 2) * P2 + (float)Math.Pow(param, 3) * P3;
        //}

        /// <summary>
        /// It calculates vector act_direction in the ont act_position of Bezier curve.
        /// </summary>
        /// <param name="param">position in the curve</param>
        /// <param name="P0">first act_position</param>
        /// <param name="P1">first inner act_position</param>
        /// <param name="P2">last inner act_position</param>
        /// <param name="P3">last act_position</param>
        /// <returns>one vector of act_direction in the curve</returns>
        //private Vector2 BezierCurveTangent (float param, Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3)
        //{
        //    return 3 * (float)Math.Pow(1 - param, 2) * (P1 - P0) + 6 * (1 - param) * param * (P2 - P1) + 3 * (float)Math.Pow(param, 2) * (P3 - P2);
        //}

        /// <summary>
        /// It calculates center of the circle.
        /// </summary>
        /// <param name="act_position">one act_position on the circle</param>
        /// <param name="act_direction">tangent of the act_position in the circle</param>
        /// <param name="turn_radius">radius of the turn</param>
        /// <returns>center of the circle</returns>
        //private Vector2 CircleCenterPoint(Vector2 act_position, Vector2 act_direction, float turn_radius, bool is_left_turn)
        //{
        //    if (is_left_turn)
        //        return act_position + turn_radius * new Vector2(-act_direction.Y, act_direction.X);
        //    else
        //        return act_position - turn_radius * new Vector2(act_direction.Y, -act_direction.X);
        //}

        /// <summary>
        /// Get intersection points of two circles.
        /// </summary>
        /// <param name="center1">center act_position of the first circle</param>
        /// <param name="radius1">radius of the first circle</param>
        /// <param name="center2">center act_position of the second circle</param>
        /// <param name="radius2">radius of the second circle</param>
        /// <returns>two intersection points</returns>
        //private (Vector2, Vector2) CirclesInstersections(Vector2 center1, float radius1, Vector2 center2, float radius2)
        //{
        //    float d = Vector2.Distance(center1, center2); // distance between centers
        //    float a = (radius1 * radius1 - radius2 * radius2 + d * d) / (2 * d); // distance between center1 and line between circles intersections
        //    float h = (float)Math.Sqrt(radius1 * radius1 - a * a); // height between a and intersection
        //    Vector2 direction = (center2 - center1) / d; // normal vector of act_direction from center1 to center2
        //    Vector2 base_point = center1 + a * direction; // a act_position between intersection on the axis of circles

        //    Vector2 intersection1 = new Vector2(base_point.X + h * direction.Y, base_point.Y - h * direction.X);
        //    Vector2 intersection2 = new Vector2(base_point.X - h * direction.Y, base_point.Y + h * direction.X);
        //    return (intersection1, intersection2);
        //}

        /// <summary>
        /// Get one of two intersection of the two circles. Choose nearer intersection in the act_direction of move.
        /// </summary>
        /// <param name="D0">act_direction of th epoint on the circle</param>
        /// <param name="P0">act_position on the circle</param>
        /// <param name="intersect1">one of the circles intersections</param>
        /// <param name="intersect2">second intersection</param>
        /// <param name="center">the center of the circle</param>
        /// <returns>the right intersection</returns>
        //private Vector2 ChooseOneCirclesIntersect(Vector2 D0, Vector2 P0, Vector2 intersect1, Vector2 intersect2, Vector2 center)
        //{
        //    bool is_cv_rot = IsClockwiseRotation(D0, P0, center); // clockwise / counterclock. rotation
        //    float orig_angle = MathF.Atan2(P0.Y - center.Y, P0.X - center.X); // angle of the original act_position on the circle
        //    float angle1 = MathF.Atan2(intersect1.Y - center.Y, intersect1.X - center.X); // angle of the intersection 1
        //    float angle2 = MathF.Atan2(intersect2.Y - center.Y, intersect2.X - center.X); // angle of the intersection 2

        //    Vector2 choosen_intersect;
        //    if (is_cv_rot)
        //        choosen_intersect = (angle1 > orig_angle && angle1 < angle2) ? intersect1 : intersect2;
        //    else
        //        choosen_intersect = (angle1 < orig_angle && angle1 > angle2) ? intersect1 : intersect2;
        //    return choosen_intersect;
        //}

        /// <summary>
        /// Find, if the turn is to left side or not.
        /// /// </summary>
        /// <param name="act_position">actual positon of an object</param>
        /// <param name="wp_position">positon of the waypoint</param>
        /// <returns>return true, if the turn is left turn, otherwise false</returns>
        //private bool IsLeftTurnFirst(Vector2 act_position, Vector2 wp_position)
        //{
        //    Vector2 wp_direc = wp_position - act_position;
        //    Vector2 normalized_direct = Vector2.Normalize(wp_direc);
        //    float cross = (normalized_direct.X * wp_direc.Y) - (normalized_direct.Y * wp_direc.X);
        //    return cross >= 0;
        //}

        /// <summary>
        /// It finds two intersections of a line (defined with two points) and a circle (defined through a center act_position and the radius). It is'nt check, if there are really two intersections.
        /// </summary>
        /// <param name="P1">one act_position on the line</param>
        /// <param name="P2">second act_position on the line</param>
        /// <param name="center">a center act_position of the circle</param>
        /// <param name="radius">a radius of the circle</param>
        /// <returns>two vector points of the intersections</returns>
        //private (Vector2, Vector2) LineCircleIntersections(Vector2 P1, Vector2 P2, Vector2 center, float radius)
        //{
        //    Vector2 d = P2 - P1;
        //    Vector2 f = P1 - center;
        //    // koeficients
        //    float a = Vector2.Dot(d, d);
        //    float b = 2 * Vector2.Dot(f, d);
        //    float c = Vector2.Dot(f, f) - radius * radius;
        //    float discriminant = (float)Math.Sqrt(b * b - 4 * a * c);
        //    // equation results
        //    float t1 = (-b - discriminant) / (2 * a);
        //    float t2 = (-b + discriminant) / (2 * a);
        //    // intersections
        //    Vector2 intersection1 = P1 + t1 * d;
        //    Vector2 intersection2 = P1 + t2 * d;
        //    return (intersection1, intersection2);
        //}

        /// <summary>
        /// Find the right intersection between line and circle in the calculation of the inner tangent of two circles.
        /// </summary>
        /// <param name="intersect_lc1">on of two intersections</param>
        /// <param name="intersect_lc2">second intersection</param>
        /// <param name="intersect_cc">an intersection between this circle and the auxiliary Thalet circle between this and the second main circle</param>
        /// <returns>the right intersection for next calculation</returns>
        //private Vector2 ChooseOneLCIntersection(Vector2 intersect_lc1, Vector2 intersect_lc2, Vector2 intersect_cc)
        //{
        //    float lc1_inters_dist = (intersect_cc - intersect_lc1).Length();
        //    float lc2_inters_dist = (intersect_cc + intersect_lc2).Length();
        //    if (lc1_inters_dist <= lc2_inters_dist)
        //        return intersect_lc1;
        //    return intersect_lc2;
        //}

        /// <summary>
        /// It calculates the position of the intersection of a diameter line and second circle and get this intersection. 
        /// Intersection for a transition from direct flight to the final turn.
        /// </summary>
        /// <param name="center1">center of the first circle</param>
        /// <param name="first_circ_tangent">first circle tangent act_position as the act_position of trasition from the first turn to the direct flight</param>
        /// <param name="center2">center of the second circle</param>
        /// <param name="radius2">radius of the second circle</param>
        /// <returns>act_position for trasition from direct flight to the final turn</returns>
        //private Vector2 SecondTangentPoint(Vector2 center1, Vector2 first_circ_tangent, Vector2 center2, float radius2)
        //{
        //    Vector2 tangent1_vect = Vector2.Normalize(first_circ_tangent - center1); // normal vector of a tanget line of the first circle
        //    Vector2 tangent2_vect = new Vector2(-tangent1_vect.Y, tangent1_vect.X); // noemal vector of a tangent line of the second circle
        //    Vector2 sec_tangent_point = center2 + tangent2_vect * radius2;
        //    return sec_tangent_point;
        //}

        /// <summary>
        /// Add each act_position of the arc into the trajectory queue of the airplane
        /// </summary>
        /// <param name="start_point">the start act_position of the arc</param>
        /// <param name="end_point">the end act_position of the arc</param>
        /// <param name="circle_center">circle center of the arc</param>
        /// <param name="circle_rad">circle radius of the arc</param>
        /// <param name="heading_step">absolute value of one change of the actual_head</param>
        //private void AddArcToTrajectory(Vector2 start_point, Vector2 end_point, Vector2 circle_center, float circle_rad, float heading_step)
        //{
        //    float start_angle = MathHelper.ToDegrees((float)Math.Atan2(start_point.Y - circle_center.Y, start_point.X - circle_center.X));
        //    float end_angle = MathHelper.ToDegrees((float)Math.Atan2(end_point.Y - circle_center.Y, end_point.X - circle_center.X));
        //    float head_change = OneHeadingChange(heading_step, start_angle, end_angle);
        //    float actual_angle = General.HeadingBordersCheck(start_angle);
        //    Vector2 actual_pos = start_point;
        //    while (!General.ObjectReachedPoint(actual_pos, end_point, 2))
        //    {
        //        actual_angle = General.HeadingBordersCheck(actual_angle + head_change);
        //        actual_pos.X = (float)(circle_center.X + circle_rad * Math.Cos(MathHelper.ToRadians(actual_angle)));
        //        actual_pos.Y = (float)(circle_center.Y + circle_rad * Math.Sin(MathHelper.ToRadians(actual_angle)));
        //        _airplane.trajectory.Enqueue(actual_pos);
        //        _airplane.heading_queue.Enqueue((int)General.HeadingBordersCheck(actual_angle - 90));
        //    }
        //}

        //private void CreateBezierTrajectory(Vector2 wp_pos, Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3)
        //{
        //    float parameter = 0; // touch_down_position on the curve
        //    while (parameter <= 1)
        //    {
        //        Vector2 actual_pos = BezierCurvePoint(parameter, P0, P1, P2, P3);
        //        this._actual_x_pos = actual_pos.X;
        //        this._actual_y_pos = actual_pos.Y;
        //        this._actual_heading = General.GetHeading(BezierCurveTangent(parameter, P0, P1, P2, P3));
        //        this._airplane.trajectory.Enqueue(new Vector2(this._actual_x_pos, this._actual_y_pos));
        //        this._airplane.heading_queue.Enqueue((int)_actual_heading);
        //        parameter += 0.000166667f;
        //    }

        //}

        /// <summary>
        /// Find, if the rotation on the circle is clockwise.
        /// </summary>
        /// <param name="D0">act_direction of the act_position on the circle</param>
        /// <param name="P0">a act_position on the circle</param>
        /// <param name="center">a center of the circle</param>
        /// <returns>true, if the rotation is clockwise, otherwise false</returns>
        //private bool IsClockwiseRotation(Vector2 D0, Vector2 P0, Vector2 center)
        //{
        //    Vector2 P1 = P0 + D0;
        //    Vector2 V1 = Vector2.Normalize(P0 - center);
        //    Vector2 V2 = Vector2.Normalize(P1 - center);
        //    if (VectorDeterminant(V1, V2) < 0)
        //        return true; // clockwise
        //    return false; // counterclockwise

        //}

        //private Texture2D CreateTexture(GraphicsDevice graphicsDevice)
        //{
        //    Texture2D stripe = new Texture2D(graphicsDevice, 5, 5);
        //    Color[] color_data = new Color[5*5];
        //    for (int i = 0; i < color_data.Length; i++)
        //        color_data[i] = Color.Red;
        //    stripe.SetData(color_data);
        //    return stripe;
        //}

        //public void Draw(SpriteBatch sb)
        //{
        //    if (this.texture.Count == 0 || this.posit.Count == 0) return;
        //    for (int i = 0; i < this.texture.Count; i++)
        //    {
        //        sb.Draw(this.texture[i], this.posit[i], Color.White);
        //    }
        //}
    }
}
