using ATC_Game.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;

namespace ATC_Game
{
    /// <summary>
    /// Class for generation static object (airports and their runways, waypoints, ...) and all game map items.
    /// </summary>
    public class MapGenerator
    {
        private Game1 _game;
        private Maps _map;
        private Texture2D _map_texture;
        private string _map_name;
        private string _path;

        //MAP ITEMS
        public List<Airport> airports;

        public MapGenerator(Game1 game, Maps map)
        {
            this._game = game;
            this._map = map;
            this.airports = new List<Airport>();
            Init();
        }

        private void Init()
        {
            this._map_name = this._map.ToString();
            this._path = string.Format("maps/{0}", this._map.ToString().ToLower());
            LoadAirports();
        }

        /// <summary>
        /// Load all airports for a map a create instances of airports and their airport items (runways, ...)
        /// </summary>
        private void LoadAirports ()
        {
            Console.WriteLine(this._path);
            foreach (string file in Directory.EnumerateFiles(this._path + "/airports/", "*.txt"))
            {
                string[] con = File.ReadAllLines(file);
                Console.WriteLine(con);
                List<string> rwy_info = new List<string>();
                for (int i = 3; i < con.Length; i++)
                    rwy_info.Add(con[i]);
                Console.WriteLine(string.Format("{0}/{1}/{2}/{3}", con[0], con[1], con[2], con[3]));
                Airport airport = new Airport(this._game, this._path, con[0], General.FileDataToVector2(con[1]), Convert.ToInt32(con[2]), rwy_info);
                this.airports.Add(airport);
            }
        }

        /// <summary>
        /// Get sum of traffic density of all airports.
        /// </summary>
        /// <returns>sum of airports density</returns>
        public int TraffDensitySum ()
        {
            int sum = 0;
            foreach (Airport airport in this.airports)
                sum += airport.traffic_density;
            return sum;
        }

        /// <summary>
        /// Draw map objects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw (SpriteBatch spriteBatch)
        {
            foreach (Airport airport in this.airports)
            {
                airport.Draw(spriteBatch);
            }
        }
    }
}
