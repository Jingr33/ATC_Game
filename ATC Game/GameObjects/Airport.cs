using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Graphics;

namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class of all airport objects.
    /// </summary>
    public class Airport : GameObject
    {
        private Game1 _game;
        private string _name;
        private Texture2D _texture;
        private Vector2 _center_pos;
        public int traffic_density; // the higher number, the biiger chance of arriving or departing airplane
        private int _rwy_count; // count of all runways at the airport
        public List<Runway> runways; // runway numbers (06/24, 09L/27R)
        public Runway in_use_dep; // runway in use for departures
        public Runway in_use_arr; // runway in use for arrivals

        public Airport (Game1 game, string path, string name, Vector2 center_position, int density, List<string> rwy_info) 
            : base (center_position)
        {
            this._game = game;
            this._name = name;
            this._center_pos = center_position;
            this.traffic_density = density;
            this._texture = GetTexture(path);
            this.runways = CreateRunways(rwy_info);
            // TODO: provizorni
            this.in_use_dep = this.runways[0];
            this.in_use_arr = this.runways[0];
        }

        private Texture2D GetTexture (string path)
        {
            string tex_name = this._name.ToLower() + "-texture";
            return this._game.Content.Load<Texture2D>(tex_name);
        }

        /// <summary>
        /// Create runways for the airport and add it to a list.
        /// </summary>
        /// <returns>runway list</returns>
        private List<Runway> CreateRunways (List<string> rwy_info)
        {
            List<Runway> runways = new List<Runway>();
            foreach (string rwy in rwy_info)
            {
                string[] rwy_ar = rwy.Split(';');
                Runway runway = new Runway(this._game, this, rwy_ar[0], General.FileDataToVector2(rwy_ar[1]), 
                                        General.FileDataToVector2(rwy_ar[2]), Convert.ToInt32(rwy_ar[3]), Convert.ToInt32(rwy_ar[4]));
                runways.Add(runway);
            }
            return runways;
        }

        /// <summary>
        /// Get vector position of top left coner of airport texture
        /// </summary>
        /// <returns>texture draw position</returns>
        public Vector2 GetTexturePosition()
        {
            int x_pos = (int)(this._center_pos.X - this._texture.Width / 2);
            int y_pos = (int)(this._center_pos.Y - this._texture.Height / 2);
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// Set a runway with preferential use for departures.
        /// </summary>
        /// <returns>runway in use for departures</returns>
        private Runway SetRwyInUseDeparture ()
        {
            return runways[0];
        }

        /// <summary>
        /// Set a runway with preferential ise dor arrivals.
        /// </summary>
        /// <returns>runway in use for arrivals</returns>
        private Runway SetRwyInUseArrival ()
        {
            return runways[0];
        }

        /// <summary>
        /// Draw an airport texture.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, GetTexturePosition(), Config.bg_color);
        }

        /// <summary>
        /// Return list of all runways at the airport.
        /// </summary>
        /// <returns>runways list</returns>
        public List<Runway> GetAirportRunways()
        {
            return this.runways;
        }
    }
}
