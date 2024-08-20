using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using System;

namespace ATC_Game.Control
{
    /// <summary>
    /// Class for control the airplane in autopilot mode.
    /// </summary>
    public class Autopilot
    {
        private Game1 _game;
        private Airplane _airplane;
        private float _time;
        public AutopilotOperation operation;

        // control the airplane
        private SpeedEqualizer _speed_equal;
        private AltitudeEqualizer _altitude_equal;
        private HeadingEqualizer _heading_equal;

        public Autopilot(Game1 game, Airplane airplane)
        {
            this._game = game;
            this._airplane = airplane;
            this._time = 0;
            this.operation = AutopilotOperation.Unknown;
            this._speed_equal = new SpeedEqualizer(this._airplane);
            this._altitude_equal = new AltitudeEqualizer(this._airplane);
            this._heading_equal = new HeadingEqualizer(this._airplane);
        }

        /// <summary>
        /// Update methid for autopilot.
        /// </summary>
        /// <param name="game_time">game time</param>
        public void Update(GameTime game_time)
        {
            StartAutopilot(game_time);
        }

        /// <summary>
        /// Call right autopilot function depending on the operation.
        /// </summary>
        /// <param name="game_time">game _time</param>
        private void StartAutopilot(GameTime game_time)
        {
            switch (this.operation)
            {
                case AutopilotOperation.TakeOff:
                    TakeOff(game_time);
                    break;
                case AutopilotOperation.Landing:
                    Landing(game_time);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Autopilot control of takeOff maneuver.
        /// </summary>
        /// <param name="game_time">game time</param>
        private void TakeOff(GameTime game_time)
        {
            this._time += (float)game_time.ElapsedGameTime.TotalSeconds;
            if (this._time >= 5) // acceleration
                this._speed_equal.EqualizeSpeed(game_time, Config.after_takeoff_speed);

            if (this._airplane.speed >= Config.min_takeoff_speed) // climb
                this._altitude_equal.EqualizeAltitude(game_time, Config.after_takeoff_alt);

            if (this._airplane.speed >= Config.after_takeoff_speed && this._airplane.altitude >= Config.after_takeoff_alt) // switch off the autopilot
                SwitchAPOff();
        }

        /// <summary>
        /// Start autopilot control for toWaypoint operation. Create trajectory of flight to elected waypoint. 
        /// </summary>
        public void ToWaypoint()
        {
            this.operation = AutopilotOperation.ToWaypoint;
            FlyAwayFromWPArea(this._airplane.waypoints[0].position);
            this._airplane.delayer.heading_equal.GenToWPTrajectory(this._airplane.waypoints[0]);
            SwitchAPOff(false);
        }

        /// <summary>
        /// Start autopilot heading control for ToLandpoint operation. Create trajectory of a flight to a landpoint for elected runway
        /// and prepare heading a position of a plane for landing.
        /// </summary>
        public void ToLandpoint()
        {
            this.operation = AutopilotOperation.ToWaypoint;
            if (!General.ObjectReachedPoint(this._airplane.center_position, this._airplane.landpoint.position, 110))
                this._airplane.delayer.heading_equal.GetToLWPTrajectory(this._airplane.landpoint);
            else
            {
                this._airplane.landpoint.is_active = false;
                this._airplane.landpoint = null;
                Console.WriteLine("Landing Point nebyl přiřazen.");
            }
            SwitchAPOff(false);
        }

        /// <summary>
        /// If the airplane position is in the neighborhood of waypoint. It fly away from this zone.
        /// </summary>
        /// <param name="destination_wp">waypoint object of flight destination</param>
        private void FlyAwayFromWPArea(Vector2 wp_dest, int neighborhood = 80)
        {
            if (General.ObjectReachedPoint(this._airplane.center_position, wp_dest, neighborhood))
            {
                // create auxiliary point for exit the waypoint neighborhood
                Vector2 pos = new Vector2(this._airplane.center_position.X + this._airplane.direction.X * neighborhood,
                                          this._airplane.center_position.Y + this._airplane.direction.Y * neighborhood);
                Waypoint aux_wp = new Waypoint(this._game, pos, "");
                this._airplane.waypoints.Insert(0, aux_wp);
            }
        }

        /// <summary>
        /// Check all airplane states and conditions for successful landing. 
        /// If the conditions are met, flight section of the airplane is setted to landing.
        /// </summary>
        public void PossibleToLand(GameTime game_time)
        {
            bool altitude = this._airplane.altitude <= 3000;
            bool speed = this._airplane.speed <= 13;
            Vector2 vec_to_airport = this._airplane.runway.map_position - this._airplane.center_position;
            bool distance = vec_to_airport.Length() <= 200 && vec_to_airport.Length() > 50;
            bool heading = General.GetHeading(vec_to_airport) <= this._airplane.runway.heading + 5
                           && General.GetHeading(vec_to_airport) >= this._airplane.runway.heading - 5;
            bool operation = this.operation != AutopilotOperation.Landing;
            Console.WriteLine(General.GetHeading(vec_to_airport).ToString());
            Console.WriteLine((this._airplane.heading + 5).ToString());
            Console.WriteLine((this._airplane.heading - 5).ToString());
            Console.WriteLine(vec_to_airport.Length().ToString());

            if (altitude && speed && distance && heading && operation)
            {
                this._airplane.trajectory.Clear();
                this._airplane.autopilot_on = true;
                this._airplane.flight_section = FlightSection.Final;
                this.operation = AutopilotOperation.Landing;
                this._time = 0;
            }
        }

        /// <summary>
        /// Autopilot control of landing maneuver.
        /// </summary>
        /// <param name="game_time">game time</param>
        private void Landing(GameTime game_time)
        {
            this._time += (float)game_time.ElapsedGameTime.TotalSeconds;
            Vector2 vector = this._airplane.runway.map_position - this._airplane.center_position;
            float distance = vector.Length();
            bool before_rwy_treshold = Math.Abs(General.GetHeading(vector) - this._airplane.runway.heading) <= 90 ? true : false;

            ControlLandingSpeed(this._time, distance, before_rwy_treshold);
            if (before_rwy_treshold)
            {
                ControlLandingHeading(this._airplane.center_position, this._airplane.runway.map_position, game_time);
                ControlLandingAltitude(distance);
            }
            else
                this._airplane.heading = this._airplane.runway.heading;
            FlightSectionChanges(distance, before_rwy_treshold);
        }

        /// <summary>
        /// Maintain or update a heding of the airplane in time to direct plane to the runway.
        /// </summary>
        /// <param name="airplane_pos">position of the airplane</param>
        /// <param name="rwy_pos">position of the airport runway</param>
        private void ControlLandingHeading(Vector2 airplane_pos, Vector2 rwy_pos, GameTime game_time)
        {
            this._airplane.delayer.heading_equal.LeadToRunway(airplane_pos, rwy_pos, game_time);
        }

        /// <summary>
        /// MAintain and decrease speed of airplane before touch down and brake the airplane on ground.
        /// </summary>
        /// <param name="time">game time from last speed change</param>
        /// <param name="distance">distance from the runway treshold</param>
        /// <param name="in_air">if the airplane id in the air (true) or already on ground (false)</param>
        private void ControlLandingSpeed(float time, float distance, bool in_air)
        {
            if (in_air && distance > 50 && this._airplane.speed > 10 && time >= 0.4) // airplane is far from the runway and fast
            {
                this._airplane.speed -= 1;
                this._time = 0;
            }
            else if (in_air && distance <= 50 && this._airplane.speed <= 10) // systematic braking before touch down
                this._airplane.speed = (int)(distance / 10) + 6;
            else if (!in_air) // after touch down braking
                this._airplane.speed = 6 - (int)(distance / 10);
        }

        /// <summary>
        /// Control an airplane altitude and decrease it in time.
        /// </summary>
        /// <param name="distance">distance from a treshold of the runway</param>
        private void ControlLandingAltitude(float distance)
        {
            int ground_alt = this._airplane.altitude - this._airplane.runway.altitude;
            int max_desired_alt = 50 * (int)distance + this._airplane.runway.altitude;
            if (this._airplane.altitude > max_desired_alt)
                this._airplane.altitude = (int)(Math.Round(max_desired_alt / 100d, 0) * 100); ;
        }

        /// <summary>
        /// Change flight section of the airplane enter the new part of flight
        /// </summary>
        /// <param name="distance">distance of airplane from treshold of the runway</param>
        /// <param name="in_air">if the airplane is in the air (true) or on ground</param>
        private void FlightSectionChanges (float distance, bool in_air)
        {
            if (distance <= 35 && this._airplane.flight_section != FlightSection.Landing)
                this._airplane.flight_section = FlightSection.Landing;
            else if (!in_air && this._airplane.flight_section != FlightSection.Landed && this._airplane.speed <= 2)
                this._airplane.flight_section = FlightSection.Landed;
        }

        /// <summary>
        /// Switch of the autopilot, enable manual control of the plane.
        /// <param name="switch_off">If it is true, set airplane attribute autopilot_on to false.</param>
        /// </summary>
        private void SwitchAPOff(bool switch_off = true)
        {
            this._time = 0;
            this.operation = AutopilotOperation.Unknown;
            this._airplane.delayer.SetActualFlightState();
            if (switch_off)
                this._airplane.autopilot_on = false;
        }
    }
}
