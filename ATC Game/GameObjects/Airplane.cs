using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Color = Microsoft.Xna.Framework.Color;
using System.Net.Mime;
using System.Reflection.Metadata;
using ATC_Game.GameObjects.AirplaneFeatures;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Concurrent;
using ATC_Game.Control;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ATC_Game.Drawing;

namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class for game object plane.
    /// </summary>
    public class Airplane : GameObject
    {
        public int id;
        private Game1 _game;
        private int _type_num;
        private Texture2D _texture;
        private Texture2D _marginal_texture;
        private Texture2D _active_texture;
        public Vector2 center_position;
        private Vector2 _draw_position;
        private Vector2 _rotation_center;
        private float _rotation; // direction of flight in radians
        private Vector2[] _last_positions = new Vector2[2];
        private int _site_lenght;
        public ConcurrentQueue<Vector2> trajectory { get; set; }
        public ConcurrentQueue<int> heading_queue { get; set; }
        public bool destroy_me;
        public float time_in_game;
        public bool is_active;
        private bool _last_state;

        // FLIGHT STATS
        public string type {  get; set; }
        public Vector2 direction { get; set; } // direction of flight as a vector
        public WeightCat weight_cat { get; set; }
        public string callsign { get; set; }
        public string registration { get; set; }
        public OperationType oper_type { get; set; } // arrival or departure
        public FlightSection flight_section { get; set; }
        public FlightStatus flight_status { get; set; }
        public string destination { get; set; }
        public int altitude { get; set; }
        public int heading { get; set; } // direction of flight in degree + 90°
        public int speed { get; set; } // means ground speed
        public Airport airport { get; set; }
        
        // ADITIONAL FEATURES
        private ArrivalAlert _arrival_alert;
        public InfoStripe info_strip;
        public bool in_margin;
        // reaction delay
        public ReactionDelayer delayer;
        public bool heading_enabled;
        // autopilot
        public bool autopilot_on;
        private Autopilot _autopilot;
        public List<Waypoint> waypoints;
        // drawing
        private TrajectoryDrawer _traj_drawer;

        public Airplane (Game1 game, int id, Vector2 center_position, int heading, OperationType oper_type, int speed, int type_number, 
                        string destination, int altitude, FlightSection flight_section, FlightStatus flight_status, Airport airport)
            : base (center_position)
        {
            this.id = id;
            this._game = game;
            this._type_num = type_number;
            this.type = SetTypeName(type_number);
            this._texture = SetTexture(type_number);
            this._marginal_texture = SetMarginalTexture(type_number);
            this._active_texture = SetActiveTexture(type_number);
            this.center_position = center_position;
            this._site_lenght = 40;
            // flight status
            this.heading = heading;
            this.direction = GetDirection(this.heading);
            this.weight_cat = GetWeightCategory();
            this.callsign = Config.airplane_callsigns[this._type_num];
            this.registration = Config.airplane_registration[this._type_num];
            this.flight_section = flight_section;
            this.flight_status = flight_status;
            this.airport = airport;
            this.oper_type = oper_type;
            this.destination = destination;
            this.altitude = altitude;
            this.speed = speed;
            this._rotation_center = new Vector2(this._texture.Width / 2, this._texture.Height / 2);
            this._rotation = GetRotation(this.direction);
            this.trajectory = new ConcurrentQueue<Vector2> { };
            this.heading_queue = new ConcurrentQueue<int> { };

            this.destroy_me = false;
            this.time_in_game = 0;
            this.is_active = false;
            this._last_state = false;
            // aditional alerts
            this._arrival_alert = AddArrivalAlert();
            this.info_strip = new InfoStripe(this._game, this);
            this.in_margin = false;

            //reaction delay
            this.heading_enabled = true;
            this.delayer = new ReactionDelayer(this._game,this);
            // autopilot
            this._autopilot = new Autopilot(this._game, this);
            this.autopilot_on = SetAutopilotAtSpawn();
            this.waypoints = new List<Waypoint>();
            // drawing
            this._traj_drawer = new TrajectoryDrawer(this._game, this);
        }

        /// <summary>
        /// Update position of plane in the map.
        /// </summary>
        /// <param name="game_time"></param>
        public void Update (GameTime game_time)
        {
            UpdateAutopilot(game_time); // autopilot

            if (!this.trajectory.IsEmpty) { // trajectory generation
                SetNextPosition();
                this.heading_enabled = false;
            }
            else {
                this.center_position = NewCoordStraightOn(game_time);
                this.heading_enabled = true;
            }

            this._rotation = GetRotation(this.direction);
            this.direction = GetDirection(this.heading);
            this._draw_position = this.center_position;
            this.delayer.UpdateReaction(game_time);
            SaveLastPositions();

            this.time_in_game += (float)game_time.ElapsedGameTime.TotalSeconds;
            this.IsMissedAirplane();
            this.in_margin = IsInMargin();
            UpdateWaypoints();
            this._last_state = this.is_active;
        }

        private void UpdateWaypoints ()
        {
            WaypointReached();
            if (this.is_active)
            {
                SetAirplaneWPs(); // if the aircraft has just been activated
                DeactiveAirplaneWps(); // if the airplane has just been deactivated
                // add new waypoint
                foreach (Waypoint wp in this._game.map_generator.GetActiveWaypoints())
                    if (!this.waypoints.Contains(wp)) 
                        this.waypoints.Add(wp);
                // remove deactivated all_waypoints
                for (int i = 0; i < this.waypoints.Count; i++)
                    if (!this.waypoints[i].is_active)
                        this.waypoints.Remove(this.waypoints[i]); return;
            }
        }

        /// <summary>
        /// Remove waypoint from flight plan (waypoint list) if the waypoint position was reached.
        /// </summary>
        private void WaypointReached ()
        {
            for (int i = 0; i < this.waypoints.Count; i++)
            {
                if (General.ObjectReachedPoint(this.center_position, this.waypoints[i].position))
                {
                    this.waypoints[i].is_active = false;
                    this.waypoints.Remove(this.waypoints[i]);
                    break;
                }
            }
        }

        /// <summary>
        /// Set an airplane all_waypoints to asctual state, if the plane was activated.
        /// </summary>
        private void SetAirplaneWPs ()
        {
            if (this.is_active && !this._last_state)
            {
                this._game.map_generator.DeactiveAllWaypoints();
                foreach (Waypoint wp in this.waypoints)
                    wp.is_active = true;
            }
        }

        /// <summary>
        /// Deactivate all airplane all_waypoints if the plane has been just deactivated.
        /// </summary>
        private void DeactiveAirplaneWps ()
        {
            if (!this.is_active && this._last_state)
                this._game.map_generator.DeactiveAllWaypoints();
        }

        /// <summary>
        /// Update autopilot state this.autopilot_on in every moment.
        /// </summary>
        private void UpdateAutopilot(GameTime game_time)
        {
            if (this.autopilot_on) // autopilot for completly autopiloted functions
                this._autopilot.Update(game_time);

            if (this.waypoints.Count > 0 && !autopilot_on) // autopilot for heading piloted functions
            {
                this.autopilot_on = true;
                this._autopilot.ToWaypoint();
            }
            else if (this.trajectory.IsEmpty && this.autopilot_on && this._autopilot.operation == AutopilotOperation.Unknown)
                this.autopilot_on = false;
        }

        /// <summary>
        /// Set next position of an airplane  in trajectory list of points.
        /// </summary>
        private void SetNextPosition ()
        {
            for (int i = 0; i < this.speed; i++)
            {
                if (this.trajectory.TryDequeue(out Vector2 next_pos)
                    && this.heading_queue.TryDequeue(out int next_heading))
                /* explaination - if the ConcurrentQueue<Vector2> is empty, TryDequeue return false
                 *                if the ConcurrentQueue<Vector2> isnt empty, TryDeququ reaturn true and call first element in the queue
                 *                after use remove the element
                 */
                {
                    this.heading = next_heading;
                    this.center_position = next_pos;
                    //this.traj_frame_time = 0;
                }
            }
        }

        /// <summary>
        /// Get new Vector2 center position of an aircraft in straight direction of flight.
        /// </summary>
        /// <returns>new coord</returns>
        private Vector2 NewCoordStraightOn(GameTime game_time)
        {
            float x_pos_diff = this.direction.X * this.speed * (float)game_time.ElapsedGameTime.TotalSeconds;
            float y_pos_diff = this.direction.Y * this.speed * (float)game_time.ElapsedGameTime.TotalSeconds;
            return new Vector2(this.center_position.X + x_pos_diff, this.center_position.Y + y_pos_diff);
        }


        /// <summary>
        /// Set last two positions to an array.
        /// </summary>
        private void SaveLastPositions()
        {
            this._last_positions[0] = this._last_positions[1];
            this._last_positions[1] = this.center_position;
        }

        /// <summary>
        /// Set type name of an airplane.
        /// </summary>
        /// <param name="type_number">number of specific airplane type</param>
        /// <returns>name of airplane type</returns>
        private string SetTypeName(int type_number)
        {
            return Config.airplane_types[type_number];
        }

        /// <summary>
        /// Set a Texture2D icon for specific type of airplane.
        /// </summary>
        /// <param name="type_number">number of a specific airplane type</param>
        /// <returns>airplane type texture</returns>
        private Texture2D SetTexture (int type_number)
        {
            return _game.Content.Load<Texture2D>(Config.airplane_icons[type_number]);
        }

        private Texture2D SetMarginalTexture (int type_number)
        {
            return _game.Content.Load<Texture2D>(Config.airplane_margin_icons[type_number]);
        }

        /// <summary>
        ///  Set a Texture2D icon for specific type of airplane.
        /// </summary>
        /// <param name="type_number">number of a specific airplane type</param>
        /// <returns>airplane type texture</returns>
        private Texture2D SetActiveTexture (int type_number)
        {
            return _game.Content.Load<Texture2D>(Config.airplane_active_icons[type_number]);
        }

        /// <summary>
        /// Draw all airplanes textures.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            if (this.is_active)
                this.ActiveDraw(_spriteBatch);
            else if (this.in_margin) // if a plane is on the edge of game map
                this.MarginalDraw(_spriteBatch);
            else
                this.TexDraw(_spriteBatch);

        }

        /// <summary>
        /// TexDraw object into the game canvas.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        private void TexDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, this._draw_position, null, Config.bg_color, this._rotation, this._rotation_center, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// TexDraw active airplane into a canvas
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        private void ActiveDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._active_texture, this._draw_position, null, Config.bg_color, this._rotation, this._rotation_center, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Mthod from draw a plane object if its position is on the edge of game map.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        private void MarginalDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._marginal_texture, this._draw_position, null, Config.bg_color, this._rotation, this._rotation_center, 1.0f, SpriteEffects.None, 0f);
        }

        private void DrawTrajectory ()
        {
            foreach ()
        }

        /// <summary>
        /// Get a rectangle of space which is occupeited by an airplane.
        /// </summary>
        /// <returns>occupied space</returns>
        public Rectangle GetAirplaneSquare ()
        {
            int x = (int)this.center_position.X - this._site_lenght/2 + 400;
            int y  = (int)this.center_position.Y - this._site_lenght/2 + 50;
            return new Rectangle(x, y, this._site_lenght, this._site_lenght);
        }

        /// <summary>
        /// Activate this airplane (is_active state to true)
        /// </summary>
        public void Activate ()
        {
            this.is_active = true;
            this.info_strip.is_active = true;
            this._game.control_panel.airplane = this;
        }

        /// <summary>
        /// Deactivate this airplane (is_active to false)
        /// </summary>
        public void Deactivate ()
        {
            this.is_active = false;
            this.info_strip.is_active = false;
            this._game.control_panel.airplane = null;
        }

        /// <summary>
        /// Check if an airplane is in game map or  out of game map
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsInGameMap()
        {
            bool x_check = (this.center_position.X >= 0) && (this.center_position.X <= this._game.GetGameAreaSize().X);
            bool y_check = (this.center_position.Y >= 0) && (this.center_position.Y <= this._game.GetGameAreaSize().Y);
            if (x_check && y_check) return true;
            return false;
        }
        public bool IsInGameMap(int boundary_overlap)
        {
            bool x_check = (this.center_position.X + boundary_overlap >= 0) && (this.center_position.X - boundary_overlap <= this._game.GetGameAreaSize().X);
            bool y_check = (this.center_position.Y + boundary_overlap >= 0) && (this.center_position.Y - boundary_overlap <= this._game.GetGameAreaSize().Y);
            if (x_check && y_check) return true;
            return false;
        }
        public bool IsInGameMap(Vector2 position)
        {
            bool x_check = (position.X >= 0) && (position.X <= this._game.GetGameAreaSize().X);
            bool y_check = (position.Y >= 0) && (position.Y <= this._game.GetGameAreaSize().Y);
            if (x_check && y_check) return true;
            return false;
        }

        /// <summary>
        /// Check if it is miised approach an a plane is already out of map bounds.
        /// </summary>
        public void IsMissedAirplane()
        {
            if ((this.time_in_game > 12) && !IsInGameMap(Config.alert_bound_dist))
            {
                this.destroy_me = true;
                return;
            }
            this.destroy_me = false;
        }

        /// <summary>
        /// Check if an airplane is in the margin distance and direction from boundaries fo a game map.
        /// </summary>
        /// <returns>true or false</returns>
        private bool IsInMargin()
        {
            bool x_coord = IsInGameMap(new Vector2(this.center_position.X + this.direction.X * Config.margin_dist, 10));
            bool y_coord = IsInGameMap(new Vector2(10, this.center_position.Y + this.direction.Y * Config.margin_dist));
            if (x_coord && y_coord)
                return false;
            return true;
            //orpav to 
        }

        /// <summary>
        /// Create arrival alert of an airplane and add it into a arrival_alerts list.
        /// </summary>
        /// <returns>arrival alert object</returns>
        private ArrivalAlert AddArrivalAlert()
        {
            ArrivalAlert alert = new ArrivalAlert(this._game, this, this.center_position);
            this._game.airplane_logic.arrival_alerts.Add(alert);
            return alert;
        }

        /// <summary>
        /// Get a direciton of flight (heading) in degree.
        /// </summary>
        /// <returns>heading in degree</returns>
        private int GetHeading()
        {
            int heading = (int)(Math.Atan2(this.direction.Y, this.direction.X) / Math.PI * 180 + 90);
            if (heading >= 0)
                return heading;
            return heading + 360;
        }

        /// <summary>
        /// Get a direction of a flight in vector
        /// </summary>
        /// <param name="heading">heading of the airplane (0-360)</param>
        /// <returns>normal vector direction</returns>
        private Vector2 GetDirection (int heading)
        {
            heading -= 90;
            if (heading < 0)
                heading += 360;
            double x = Math.Cos(heading * Math.PI / 180);
            double y = Math.Sin(heading * Math.PI / 180);
            return new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// Get a rotation (direction of the airplane) in radians.
        /// </summary>
        /// <param name="direction">normal vector of the airplane</param>
        /// <returns>direction in radinans</returns>
        private float GetRotation (Vector2 direction)
        {
            return (float)Math.Atan2(this.direction.Y, this.direction.X);
        }

        /// <summary>
        /// Get weight catégory of an airplane
        /// </summary>
        /// <returns>weight category</returns>
        private WeightCat GetWeightCategory ()
        {
            return WeightCat.A;
        }

        /// <summary>
        /// Switch on an autopilot, if the airplane is generated as a departure flight.
        /// </summary>
        /// <returns>true if autpilot is on</returns>
        private bool SetAutopilotAtSpawn ()
        {
            if (this.flight_section == FlightSection.TakeOff)
            {
                this._autopilot.operation = AutopilotOperation.TakeOff;
                return true;
            }
            return false;
        }
    }
}
