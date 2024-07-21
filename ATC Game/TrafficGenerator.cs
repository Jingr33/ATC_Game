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
        private Game1 _game;
        private Vector2 _screen_size;
        private Random _random = new Random();
        private float _off_distance; // the distance beyond the edge at which the plane is generated
        private int next_id;
        private List<Airplane> airplanes;
        private int _plane_count;
        private int _arrive_plane_count;
        private int _depart_plane_count;

        public TrafficGenerator(Game1 game, Vector2 screen_size)
        {
            this._game = game;
            this._screen_size = screen_size;
            this._off_distance = Config.plane_off_distance;
            this.airplanes = game.airplanes;
            this.next_id = 0;
            this._plane_count = 0;
            this._arrive_plane_count = 0;
            this._depart_plane_count = 0;
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
            int arr_dep = this._random.Next(2);
            //arr_dep = 2;
            if (arr_dep == 1)
                AddArrivalAirplane(game, existing_airplanes);
            else
                AddDepartAirplane(game, existing_airplanes);
            return existing_airplanes;
        }

        public List<Airplane> AddArrivalAirplane(Game1 game, List<Airplane> existing_airplanes)
        {
            OperationType oper_type = OperationType.Arrival;
            Vector2 start_pos = GenArrivalEntryPos();
            int start_heading = GenArrivalHeading(start_pos);
            int start_speed = GenArrivalSpeed();
            int type = GenerateType();
            string dest = GenerateDestination();
            int altitude = GenArrivalAltitude();
            FlightSection flight_section = FlightSection.Approach;
            FlightStatus flight_status = GenerateFlightStatus();
            Airplane new_plane = new Airplane(game, this.next_id, start_pos, start_heading, oper_type, start_speed, type, dest, altitude,
                                              flight_section, flight_status);
            existing_airplanes.Add(new_plane);
            game.infostripes.Add(new_plane.info_strip);
            return existing_airplanes;
        }

        public List<Airplane> AddDepartAirplane(Game1 game, List<Airplane> existing_airplanes)
        {
            OperationType oper_type = OperationType.Departure;
            Airport airport = ChooseDepartAirport(); // elected airport
            Runway runway = airport.in_use_dep; // rwy in use for departures
            Vector2 start_pos = SetRunwayDeparturePos(runway);
            int heading = runway.heading;
            int speed = 0;
            int type = GenerateType(); // TODO - ať vzlítají jen typy které přiletěly
            string dest = GenerateDestination();
            int altitude = 0; // TODO - ať se mění výška letadla pokud je v modelu take-off
            FlightSection flight_section = FlightSection.TakeOff;
            FlightStatus flight_status = GenerateFlightStatus();
            Airplane new_plane = new Airplane(game, this.next_id, start_pos, heading, oper_type, speed, type, dest, altitude,
                                              flight_section, flight_status);
            existing_airplanes.Add(new_plane);
            game.infostripes.Add(new_plane.info_strip);
            return existing_airplanes;
        }

        /// <summary>
        /// Random generate entry position of airplane when entering a game map.
        /// </summary>
        /// <returns> position coordinates of entry point</returns>
        private Vector2 GenArrivalEntryPos()
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
        /// Set departure position of a starting airplane from a runaway
        /// </summary>
        /// <param name="rwy">departure runway</param>
        /// <returns>position of airplane spawn</returns>
        private Vector2 SetRunwayDeparturePos (Runway rwy)
        {
            Vector2 airport_pos = rwy.GetMyAirport().GetTexturePosition();
            Vector2 rwy_pos = rwy.position;
            return airport_pos + rwy_pos;
        }

        /// <summary>
        /// Generation of entry direciton (start_heading) of an airplane.
        /// </summary>
        /// <param name="start_pos">coordinates of entry point in canvas.</param>
        /// <returns>vector of start_heading</returns>
        private int GenArrivalHeading(Vector2 start_pos)
        {
            Vector2 center = new Vector2(_screen_size.X / 2, _screen_size.Y / 2);
            Vector2 long_direction = new Vector2(center.X - start_pos.X, center.Y - start_pos.Y);
            //float lenght = (float)Math.Sqrt(Math.Pow(long_direction.X, 2) + Math.Pow(long_direction.Y, 2));
            //Vector2 direc = new Vector2(long_direction.X / lenght, long_direction.Y / lenght);
            int heading = (int)(Math.Atan2(long_direction.Y, long_direction.X) / Math.PI * 180 + 90);
            if (heading >= 0)
                return heading;
            return heading + 360;
        }

        /// <summary>
        /// Random generation of entry velocity of an airplane.
        /// </summary>
        /// <returns>speed value of an airplane</returns>
        private int GenArrivalSpeed()
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
        private int GenArrivalAltitude()
        {
             return this._random.Next(Config.min_altitude, Config.max_altitude) * 100;
        }

        private FlightStatus GenerateFlightStatus()
        {
            return FlightStatus.InTime; // TODO - náhodný flightstatus s pravděpodobností
        }

        /// <summary>
        /// Choose random airport (according to its traffic density) and add departure to this airport.
        /// </summary>
        /// <returns>one of the airports on the map</returns>
        private Airport ChooseDepartAirport ()
        {
            int rnd_dens = this._random.Next(this._game._map_generator.TraffDensitySum()); // random num from density sum
            int actual_range = 0;
            foreach (Airport airport in this._game._map_generator.airports)
            {
                actual_range += airport.traffic_density;
                if (rnd_dens < actual_range)
                    return airport;
            }
            return null;
        }

        /// <summary>
        /// Return count of airplanes in the game.
        /// </summary>
        /// <returns>count of airplanes</returns>
        public int GetAirplanesCount()
        {
            return this._plane_count;
        }

        /// <summary>
        /// Retrun actual count of arriving airplanes.
        /// </summary>
        /// <returns>count of arrivals</returns>
        public int GetArriveAirplanesCount()
        {
            int count = 0;
            foreach (Airplane airplane in this.airplanes)
                if (airplane.oper_type == OperationType.Arrival) count++;
            return count;
        }

        /// <summary>
        /// Return actual count of departing airplanes.
        /// </summary>
        /// <returns>count of departures</returns>
        public int GetDepartAirplanescount()
        {
            int count = 0;
            foreach (Airplane airplane in this.airplanes)
                if (airplane.oper_type == OperationType.Departure) count++;
            return count;
        }
    }
}
