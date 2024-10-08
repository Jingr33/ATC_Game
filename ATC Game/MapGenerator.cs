﻿using ATC_Game.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using System.Net;

namespace ATC_Game
{
    /// <summary>
    /// Class for generation static object (airports and their runways, all_waypoints, ...) and all game map items.
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
        public List<Waypoint> all_waypoints;
        public List<LandingWaypoint> all_landpoints;

        public MapGenerator(Game1 game, Maps map)
        {
            this._game = game;
            this._map = map;
            this.airports = new List<Airport>();
            this.all_waypoints = new List<Waypoint>();
            this.all_landpoints = new List<LandingWaypoint>();
            Init();
        }

        private void Init()
        {
            this._map_name = this._map.ToString();
            this._map_texture = LoadGameMap();
            this._path = string.Format("maps/{0}", this._map.ToString().ToLower());
            LoadAirports();
            LoadWaypoints();
            LoadLandpoints();
        }

        private Texture2D LoadGameMap ()
        {
            return _game.Content.Load<Texture2D>(this._map_name);
        }

        /// <summary>
        /// Load all airports for a map a create instances of airports and their airport items (runways, ...)
        /// </summary>
        private void LoadAirports ()
        {
            foreach (string file in Directory.EnumerateFiles(this._path + "/airports/", "*.txt"))
            {
                string[] con = File.ReadAllLines(file);
                List<string> rwy_info = new List<string>();
                for (int i = 3; i < con.Length; i++)
                    rwy_info.Add(con[i]);
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
        /// Load all all_waypoints of the map. Create their instances.
        /// </summary>
        private void LoadWaypoints ()
        {
            using (StreamReader sr = new StreamReader(this._path + "/waypoints.csv"))
            {
                string one_wp = sr.ReadLine();
                while ((one_wp = sr.ReadLine()) != null)
                {
                    string[] data_wp = one_wp.Split(';');
                    Waypoint waypoint = new Waypoint(this._game, General.FileDataToVector2(data_wp[0]), data_wp[1]);
                    this.all_waypoints.Add(waypoint);
                }
            }
        }

        /// <summary>
        /// Add element to a landing _points list.
        /// </summary>
        /// <param name="LWP">one landing point</param>
        public void LoadLandpoints ()
        {
            foreach (Airport airport in this.airports)
                foreach (Runway rwy in airport.runways)
                    this.all_landpoints.Add(rwy.land_waypoint);
        }

        /// <summary>
        /// Set deactive state to all airports.
        /// </summary>
        public void DeactiveAllAirports ()
        {
            foreach(Airport airport in this.airports)
                airport.is_active = false;
        }

        /// <summary>
        /// Get an active airport if it possible. If not, return null.
        /// </summary>
        /// <returns>an active airport object</returns>
        public Airport GetActiveAirport()
        {
            foreach (Airport airport in this.airports)
                if (airport.is_active)
                    return airport;
            return null;
        }

        /// <summary>
        /// If there is some active airport, return true. If not -> false.
        /// </summary>
        /// <returns></returns>
        public bool IsSomeActiveAirport ()
        {
            foreach (Airport airport in this.airports)
                if (airport.is_active)
                    return true;
            return false;
        }

        /// <summary>
        /// Deactivate all waypoints in the game map.
        /// </summary>
        public void DeactiveAllWaypoints ()
        {
            foreach (Waypoint wp in this.all_waypoints)
                wp.is_active = false;
        }

        /// <summary>
        /// Deactivate all waypoints in the game map.
        /// </summary>
        public void DeactiveAllLandpoints ()
        {
            foreach (LandingWaypoint lwp in this.all_landpoints)
                lwp.is_active = false;
        }

        /// <summary>
        /// Return all active all_waypoints right now.
        /// </summary>
        /// <returns>list of active all_waypoints</returns>
        public List<Waypoint> GetActiveWaypoints()
        {
            List<Waypoint> list = new List<Waypoint>();
            foreach (Waypoint wp in this.all_waypoints)
                if (wp.is_active)
                    list.Add(wp);
            return list;
        }

        

        /// <summary>
        /// TexDraw map objects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._map_texture, Vector2.Zero, Config.bg_color);
            foreach (Airport airport in this.airports)
                airport.Draw(spriteBatch);
            foreach (Waypoint wp in this.all_waypoints)
                wp.Draw(spriteBatch);
            foreach(LandingWaypoint lwp in this.all_landpoints)
                lwp.Draw(spriteBatch);
        }
    }
}
