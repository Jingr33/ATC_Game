using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class of ghost airplanes (on ground planes). Ghost has all of the normal airplanes states, but it is only information object.
    /// </summary>
    public class AirplaneGhost
    {
        private Game1 _game;
        private Airplane _airplane;
        public string type { get; set; }
        public WeightCat weight_cat { get; set; }
        public string callsign { get; set; }
        public string registration { get; set; }
        public OperationType oper_type { get; set; } // arrival or departure
        public FlightSection flight_section { get; set; }
        public FlightStatus flight_status { get; set; }
        public string from_destination { get; set; }
        public string to_destination { get; set; }
        public Airport airport { get; set; }
        public Runway land_runway { get; set; }

        public AirplaneGhost(Game1 game, Airplane airplane)
        {
            this._game = game;
            this._airplane = airplane;
            InitRealPlaneStats();
        }

        /// <summary>
        /// Copy a real airplane stats to these attributes.
        /// </summary>
        private void InitRealPlaneStats ()
        {
            this.type = this._airplane.type;
            this.weight_cat = this._airplane.weight_cat;
            this.registration = this._airplane.registration;
            this.callsign = this._airplane.callsign;
            this.oper_type = this._airplane.oper_type;
            this.flight_section = this._airplane.flight_section;
            this.flight_status = this._airplane.flight_status;
            this.from_destination = this._airplane.destination;
            this.airport = this._airplane.airport;
            this.land_runway = this._airplane.runway;
            this.to_destination = GenerateToDestination();
        }

        /// <summary>
        /// Generate a departure destination for the real airplane.
        /// </summary>
        private string GenerateToDestination ()
        {
            //TODO: provizorni
            return this.from_destination;
        }
    }
}
