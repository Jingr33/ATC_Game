using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace ATC_Game
{
    /// <summary>
    /// Class for generation of airplane traffic.
    /// </summary>
    internal class TrafficGenerator
    {
        private Vector2 _screen_size;
        private Random _random = new Random();
        private float _off_distance; // the distance beyond the edge at which the plane is generated
        private int next_id;

        public TrafficGenerator(Vector2 screen_size)
        {
            this._screen_size = screen_size;
            this._off_distance = Config.plane_off_distance;
            this.next_id = 0;
        }

        /// <summary>
        /// Generate with some probability new airplane to site of a game map and add it to a airplane list.
        /// </summary>
        /// <param name="game">game</param>
        /// <param name="airplanes">list of airplanes</param>
        /// <param name="game_time">gametime</param>
        /// <returns>list of all airplanes in game</returns>
        public List<Airplane> UpdateTraffic(Game1 game, List<Airplane> airplanes, GameTime game_time)
        {
            int probability = _random.Next(0, Config.gen_probability);
            bool gen_new_plane = (probability == 0) && (airplanes.Count <= Config.max_plane_count);
            bool few_planes = airplanes.Count <= Config.min_plane_count;
            if (gen_new_plane || few_planes)
            {
                airplanes = AddNewAirplane(game, airplanes);
                this.next_id++;
                return airplanes;
            }
            return airplanes;
        }

        /// <summary>
        /// Create a new airplane to a some site of game map.
        /// </summary>
        /// <param name="game">game</param>
        /// <param name="existing_airplanes">all already created plane in game</param>
        /// <returns>list of all plane sin game (with new plane)</returns>
        public List<Airplane> AddNewAirplane(Game1 game, List<Airplane> existing_airplanes)
        {
            OperationType oper_type = OperationType.Arrival;
            Vector2 start_pos = GenerateEntryPosition();
            Vector2 start_direc = GenerateDirection(start_pos);
            int start_speed = GenerateSpeed();
            int type = GenerateType();
            string dest = GenerateDestination();
            int altitude = GenerateAltitude();
            FlightSection flight_section = FlightSection.Approach; // TODO
            FlightStatus flight_status = GenerateFlightStatus();
            Airplane new_plane = new Airplane(game, this.next_id, start_pos, start_direc, oper_type, start_speed, type, dest, altitude,
                                              flight_section, flight_status);
            existing_airplanes.Add(new_plane);
            game.infostripes.Add(new_plane.info_strip);
            return existing_airplanes;
        }



        /// <summary>
        /// Random generate entry position of airplane when entering a game map.
        /// </summary>
        /// <returns> position coordinates of entry point</returns>
        private Vector2 GenerateEntryPosition()
        {
            int rnd_site = _random.Next(0, 4);
            int rnd_height = _random.Next(0, (int)_screen_size.Y);
            int rnd_width = _random.Next(0, (int)_screen_size.X);
            switch (rnd_site)
            {
                case 0: // left
                    return new Vector2(-this._off_distance, rnd_height);
                case 1: // top
                    return new Vector2(rnd_width, -this._off_distance);
                case 2: // right
                    return new Vector2(this._screen_size.X + this._off_distance, rnd_height);
                case 3: // bottom
                    return new Vector2(rnd_width, _screen_size.Y + this._off_distance);
            }
            return new Vector2(rnd_height, -this._off_distance);
        }

        /// <summary>
        /// Generation of entry direciton (heading) of an airplane.
        /// </summary>
        /// <param name="start_pos">coordinates of entry point in canvas.</param>
        /// <returns>vector of direction</returns>
        private Vector2 GenerateDirection(Vector2 start_pos)
        {
            Vector2 center = new Vector2(_screen_size.X / 2, _screen_size.Y / 2);
            Vector2 long_direction = new Vector2(center.X - start_pos.X, center.Y - start_pos.Y);
            float lenght = (float)Math.Sqrt(Math.Pow(long_direction.X, 2) + Math.Pow(long_direction.Y, 2));
            return new Vector2(long_direction.X / lenght, long_direction.Y / lenght);
        }

        /// <summary>
        /// Random generation of entry velocity of an airplane.
        /// </summary>
        /// <returns>speed value of an airplane</returns>
        private int GenerateSpeed()
        {
            return this._random.Next(Config.min_speed, Config.max_speed);
        }

        /// <summary>
        /// Random generation of airplane type,
        /// </summary>
        /// <returns>number of airplane type</returns>
        private int GenerateType()
        {
            // TODO: tohle se zatím neřeší - nemám typy letadel
            return this._random.Next(Config.airplane_types.Length);
        }

        /// <summary>
        /// Choose random destination from a destination list
        /// </summary>
        /// <returns>target or start destination of airplane</returns>
        private string GenerateDestination()
        {
            int rnd = this._random.Next(Config.destinations.Length);
            return Config.destinations[rnd];
        }

        /// <summary>
        /// Generate random altitude of spawned airplane.
        /// </summary>
        /// <returns>altitude value</returns>
        private int GenerateAltitude()
        {
             return this._random.Next(Config.min_altitude, Config.max_altitude) * 100;
        }

        private FlightStatus GenerateFlightStatus()
        {
            return FlightStatus.InTime; // TODO
        }
    }
}
