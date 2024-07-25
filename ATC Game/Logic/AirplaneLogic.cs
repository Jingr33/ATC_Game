using ATC_Game.GameObjects;
using ATC_Game.GameObjects.AirplaneFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.Logic
{
    public class AirplaneLogic
    {
        private Game1 _game;
        private TrafficGenerator _trafficGenerator;
        private float _airplane_spawn_time;
        private AirplaneLogic _airplane_logic;

        // ADITIONAL FEATURES
        public List<ArrivalAlert> arrival_alerts;

        public AirplaneLogic(Game1 game)
        {
            this._game = game;
            InitializeAirplanes();
        }

        public void InitializeAirplanes()
        {
            this._airplane_spawn_time = 0;
            this.arrival_alerts = new List<ArrivalAlert>();
            this._trafficGenerator = new TrafficGenerator(this._game, this._game.GetGameAreaSize());
        }

        public void UpdateAirplanes (GameTime game_time)
        {
            TryAddNewAirplane(game_time);
            UpdateAllAirplanes(game_time);
            UpdateArrivalAlerts(game_time);
            RemoveMissedAirplanes();
        }

        /// <summary>
        /// Addnew airplane with some probability if there the conditions are for add new airplane met.
        /// </summary>
        /// <param name="game_time">game _time</param>
        private void TryAddNewAirplane (GameTime game_time)
        {
            this._airplane_spawn_time += (float)game_time.ElapsedGameTime.TotalSeconds;
            if (this._airplane_spawn_time >= Config.gen_interval)
            {
                this._game.airplanes = this._trafficGenerator.UpdateTraffic(this._game, this._game.airplanes, game_time);
                this._airplane_spawn_time = 0;
            }
        }


        /// <summary>
        /// Update state and atributttes of all airplanes in the game.
        /// </summary>
        /// <param name="game_time">game _time</param>
        private void UpdateAllAirplanes (GameTime game_time)
        {
            foreach (Airplane airplane in this._game.airplanes)
            {
                airplane.Update(game_time);
            }
        }

        /// <summary>
        /// Update all arrival alerts states.
        /// </summary>
        private void UpdateArrivalAlerts(GameTime game_time)
        {
            List<int> for_removal = new List<int>();
            for (int i = 0; i < this.arrival_alerts.Count; i++)
            {
                this.arrival_alerts[i].UpdateAlert(game_time);
                if (this.arrival_alerts[i].destroy_me)
                    for_removal.Add(i);
            }
            foreach (int i in for_removal)
                this.arrival_alerts.Remove(arrival_alerts[i]);
        }

        /// <summary>
        /// Remove airplanes which are out of map bounds.
        /// </summary>
        private void RemoveMissedAirplanes()
        {
            List<int> remove = new List<int> {};
            for (int i = 0; i < this._game.airplanes.Count; i++)
            {
                if (this._game.airplanes[i].destroy_me)
                    remove.Add(i);
            }
            foreach (int i in remove)
            {
                this._game.infostripes.Remove(this._game.infostripes[i]); // remove infostrip
                this._game.airplanes.Remove(this._game.airplanes[i]); // remove airplane
            }
        }

        /// <summary>
        /// Set attribute of Airplane is_active for all airplanes to false and is_active state of info strips to false.
        /// </summary>
        public void DeactivateAllPlanes ()
        {
            foreach (Airplane plane in this._game.airplanes)
                plane.is_active = false;
            foreach (InfoStripe stripe in this._game.infostripes)
                stripe.is_active = false;
        }

        /// <summary>
        /// If there is some active airplane in the game, it return this plane.
        /// </summary>
        /// <returns>active airplane</returns>
        public Airplane GetActiveAirplane ()
        {
            foreach (Airplane plane in this._game.airplanes)
                if (plane.is_active) return plane;
            return null;
        }
    }
}
