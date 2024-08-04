using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private void StartAutopilot (GameTime game_time)
        {
            switch (this.operation)
            {
                case AutopilotOperation.TakeOff:
                    TakeOff(game_time);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Autopilot control of takeOff maneuver.
        /// </summary>
        /// <param name="game_time"></param>
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
        public void ToWaypoint ()
        {
            FlyAwayFromWPArea();
            this._airplane.delayer.heading_equal.GenToWPTrajectory(this._airplane.waypoints[0]);
            SwitchAPOff(false);
        }

        /// <summary>
        /// If the airplane position is in the neighborhood of waypoint. It fly away from this zone.
        /// </summary>
        /// <param name="destination_wp">waypoint object of flight destination</param>
        private void FlyAwayFromWPArea ()
        {
            int neighborhood = 80;
            Waypoint dest_wp = this._airplane.waypoints[0];
            if (General.ObjectReachedPoint(this._airplane.center_position, dest_wp.position, neighborhood))
            {
                // create auxiliary point for exit the waypoint neighborhood
                Vector2 pos = new Vector2(this._airplane.center_position.X + this._airplane.direction.X*neighborhood, 
                                          this._airplane.center_position.Y + this._airplane.direction.Y*neighborhood);
                Waypoint aux_wp = new Waypoint(this._game, pos, "");
                this._airplane.waypoints.Insert(0, aux_wp);
            }
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
