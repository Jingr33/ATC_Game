using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using ATC_Game.Drawing;

namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class of all airport objects.
    /// </summary>
    public class Airport : GameObject
    {
        private Game1 _game;
        public string name;
        private Texture2D _texture;
        private Texture2D _highlight_texture;
        private Vector2 _center_pos;
        private Vector2 _global_pos;
        public int traffic_density; // the higher number, the biiger chance of arriving or departing airplane
        private int _rwy_count; // count of all runways at the airport
        public List<Runway> runways; // land_runway numbers (06/24, 09L/27R)
        public List<Runway> in_use_dep; // take-off_runways in use for departures
        public List<Runway> in_use_arr; // land_runways in use for arrivals

        public List<AirplaneGhost> airplane_ghosts; // on ground airplanes
        public List<Airplane> arrival_airplanes;
        public List<Airplane> departure_airplanes;

        public FlightsToApDrawer flights_track_drawer;

        public bool is_active;
        private ButtonState _last_state;
        private int _width;
        private int _height;

        public Airport (Game1 game, string path, string name, Vector2 center_position, int density, List<string> rwy_info) 
            : base (center_position)
        {
            this._game = game;
            this.name = name;
            this._center_pos = center_position;
            this._global_pos = new Vector2(400 + center_position.X, 50 + center_position.Y);
            this.traffic_density = density;
            this._texture = GetTexture();
            this.runways = CreateRunways(rwy_info);
            this._width = this._texture.Width + 16;
            this._height = this._texture.Height + 16;
            this._highlight_texture = GetHighlighterTex();
            // TODO: provizorni
            this.in_use_dep = new List<Runway>() { this.runways[0] };
            this.in_use_arr = new List<Runway>() { this.runways[0] };

            this.airplane_ghosts = new List<AirplaneGhost>() { };
            this.arrival_airplanes = new List<Airplane>() { };
            this.departure_airplanes = new List<Airplane>() { };

            this.flights_track_drawer = new FlightsToApDrawer(this._game, this);

            this.is_active = false;
            this._last_state = ButtonState.Released;
        }

        /// <summary>
        /// Return texture of the airport.
        /// </summary>
        /// <returns>airport texture</returns>
        private Texture2D GetTexture ()
        {
            string tex_name = this.name.ToLower() + "-texture";
            return this._game.Content.Load<Texture2D>(tex_name);
        }

        /// <summary>
        /// Return a texture of the highlighted circle around the airport for active state.
        /// </summary>
        /// <returns>a highlighted texture</returns>
        private Texture2D GetHighlighterTex ()
        {
            Texture2D highlighter = new Texture2D(this._game.GraphicsDevice, this._width, this._height);
            Color[] color_data = new Color[this._width * this._height];
            for (int j = 0; j < this._height; j++)
            {
                for (int k = 0; k < this._width; k++)
                {
                    bool border_hor = (j < 3) || (j >= this._height - 3);
                    bool border_ver = (k < 3) || (k >= this._width - 3);
                    if (border_hor || border_ver)
                        color_data[this._width * j + k] = Color.CadetBlue;
                    else
                        color_data[this._width * j + k] = Color.Transparent;
                }
            }
            highlighter.SetData(color_data);
            return highlighter;
        }

        /// <summary>
        /// Create runways for the airport and add it to a list.
        /// </summary>
        /// <returns>land_runway list</returns>
        private List<Runway> CreateRunways (List<string> rwy_info)
        {
            List<Runway> runways = new List<Runway>();
            foreach (string rwy in rwy_info)
            {
                string[] rwy_ar = rwy.Split(';');
                Runway runway = new Runway(this._game, this, rwy_ar[0], General.FileDataToVector2(rwy_ar[1]), 
                                        Convert.ToInt32(rwy_ar[2]), Convert.ToInt32(rwy_ar[3]), Convert.ToInt32(rwy_ar[4]));
                runways.Add(runway);
            }
            return runways;
        }

        /// <summary>
        /// Update airplanes stats in time.
        /// </summary>
        public void Update (GameTime game_time)
        {
            foreach(Runway in_use_rwy in this.in_use_arr)
                in_use_rwy.UpdateLandingLights(game_time);
            UpdateActiveState();
        }

        /// <summary>
        /// Update activity click event of the airport.
        /// </summary>
        private void UpdateActiveState ()
        {
            if (GetAirportSquare().Contains(this._game.mouse.Position) && this._game.mouse.LeftButton == ButtonState.Released && this._last_state == ButtonState.Pressed)
                SwitchState();
            this._last_state = this._game.mouse.LeftButton;
        }

        /// <summary>
        /// Switch active state.
        /// </summary>
        private void SwitchState ()
        {
            if (this.is_active)
                this.is_active = false;
            else
            {
                this._game.map_generator.DeactiveAllAirports();
                this.is_active = true;
            }
        }

        /// <summary>
        /// Return area for a click event of the airport.
        /// </summary>
        /// <returns>rectangle area of the airport</returns>
        private Rectangle GetAirportSquare()
        {
            return new Rectangle((int)this._global_pos.X - this._width / 2, (int)this._global_pos.Y - this._height / 2, this._width, this._height);
        }

        /// <summary>
        /// Get vector touch_down_position of top left coner of airport texture
        /// </summary>
        /// <returns>texture draw touch_down_position</returns>
        public Vector2 GetTexturePosition()
        {
            int x_pos = (int)(this._center_pos.X - this._texture.Width / 2);
            int y_pos = (int)(this._center_pos.Y - this._texture.Height / 2);
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// Get a vector touch_down_position of the top left coner of the airport highlighter texture.
        /// </summary>
        /// <returns>highlighter draw touch_down_position</returns>
        public Vector2 GetHighlighterPosition()
        {
            int x_pos = (int)(this._center_position.X - this._width / 2);
            int y_pos = (int)(this._center_position.Y - this._height / 2);
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// Set a land_runway with preferential use for departures.
        /// </summary>
        /// <returns>land_runway in use for departures</returns>
        private Runway SetRwyInUseDeparture ()
        {
            return runways[0];
        }

        /// <summary>
        /// Set a land_runway with preferential ise dor arrivals.
        /// </summary>
        /// <returns>land_runway in use for arrivals</returns>
        private Runway SetRwyInUseArrival ()
        {
            return runways[0];
        }

        /// <summary>
        /// If the airplane is on ground right now, this method create aghost airplane in this airport and change airport stats.
        /// </summary>
        /// <param name="airplane">landed airplane</param>
        public void AirplaneLanded (Airplane airplane)
        {
            if (airplane.oper_type == OperationType.Arrival)
                RemoveAirplaneFrom(airplane, this.arrival_airplanes);
            else
                RemoveAirplaneFrom(airplane, this.departure_airplanes);

            AirplaneGhost airplaneGhost = new AirplaneGhost(this._game, airplane);
            this.airplane_ghosts.Add(airplaneGhost);
        }

        /// <summary>
        /// Remove airplane from departure or arrival (depending on the operation type) airplane list.
        /// </summary>
        /// <param name="airplane"></param>
        public void AirplaneDeparted (Airplane airplane)
        {
            if (airplane.oper_type == OperationType.Departure)
                RemoveAirplaneFrom(airplane, this.departure_airplanes);
            else
                RemoveAirplaneFrom(airplane, this.arrival_airplanes);
        }

        /// <summary>
        /// Add the airplane into the entered airplane list.
        /// </summary>
        /// <param name="airplane"></param>
        /// <param name="airplane_list"></param>
        public void AddAirplaneTo (Airplane airplane, List<Airplane> airplane_list)
        {
            airplane_list.Add(airplane);
        }

        /// <summary>
        /// Remoce the airplane from the entered airplane list.
        /// </summary>
        /// <param name="airplane"></param>
        /// <param name="airplane_list"></param>
        public void RemoveAirplaneFrom (Airplane airplane, List<Airplane> airplane_list)
        {
            airplane_list.Remove(airplane);
        }

        /// <summary>
        /// TexDraw an airport texture and all subordinate objects.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw (SpriteBatch spriteBatch)
        {
            // land_runway texture
            foreach (var runway in this.runways)
                runway.Draw(spriteBatch);
            // rwy in use for arrival lights
            foreach (Runway in_use_rwy in this.in_use_arr)
                in_use_rwy.DrawLights(spriteBatch);
            this.flights_track_drawer.Draw(spriteBatch);
            // airport texture
            //spriteBatch.Draw(this._texture, GetTexturePosition(), Config.bg_color);
            if (this.is_active)
                spriteBatch.Draw(this._highlight_texture, GetHighlighterPosition(), Config.bg_color);
        }

        /// <summary>
        /// Return list of all runways at the airport.
        /// </summary>
        /// <returns>runways list</returns>
        public List<Runway> GetAirportRunways()
        {
            return this.runways;
        }

        /// <summary>
        /// Update arrival and departure runways in use (lists), if player click into the runway button in the airpot info panel
        /// and change the runways there.
        /// </summary>
        /// <param name="switched_rwy">clicked runway</param>
        /// <param name="is_departure">if the new clicked runway is for departure or arrival list</param>
        public void UpdateInUseRwysLists(Runway switched_rwy, bool is_departure)
        {
            // remove from rwys for arrivals if it contains switched rwy
            if (!is_departure && this.in_use_arr.Contains(switched_rwy))
            {
                if (this.in_use_arr.Count > 1)
                    this.in_use_arr.Remove(switched_rwy);
            }
            // add to rwys for arrivals if it not contains switched rwy
            else if (!is_departure)
                this.in_use_arr.Add(switched_rwy);
            // remove from rwys for departures if it contains switched rwy
            else if (is_departure && this.in_use_dep.Contains(switched_rwy))
            {
                if (this.in_use_dep.Count > 1)
                    this.in_use_dep.Remove(switched_rwy);
            }
            // add to rwys for departures if it not contains switched rwy
            else if (is_departure)
                this.in_use_dep.Add(switched_rwy);
        }
    }
}
