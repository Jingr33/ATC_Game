using ATC_Game.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.Drawing
{
    /// <summary>
    /// Draw a track lines of the flights the to choosen airport.
    /// </summary>
    public class FlightsToApDrawer
    {
        private Game1 _game;
        private Airport _airport;
        public bool is_active;
        private bool _last_state;

        public FlightsToApDrawer(Game1 game, Airport airport)
        {
            this._game = game;
            this._airport = airport;
            this.is_active = false;
            this._last_state = this.is_active;
        }

        /// <summary>
        /// Switch an activity of the drawer. Turn on or turn off.
        /// </summary>
        public void SwitchActivity ()
        {
            this.is_active = General.Switcher(this.is_active);
        }

        /// <summary>
        /// Update a FlightsToApDrawer.
        /// </summary>
        public void Update ()
        {
            if (this.is_active)
            {
                foreach (Airplane airplane in this._airport.departure_airplanes)
                {
                    airplane.track_drawer.is_active = true;
                    airplane.track_drawer.Update(airplane);
                }
                foreach (Airplane airplane in this._airport.arrival_airplanes)
                {
                    airplane.track_drawer.is_active = true;
                    airplane.track_drawer.Update(airplane);
                }
            }
            // switch off flights to airport drawer
            if (this._last_state && !this.is_active)
                 foreach (Airplane airplane in this._game.airplanes)
                    airplane.track_drawer.is_active=false;
            this._last_state = this.is_active;
        }

        /// <summary>
        /// Draw an airplane tracks of flights to choosen airport if the drawer is activated.
        /// </summary>
        public void Draw (SpriteBatch sprite_batch)
        {
            if (this.is_active)
            {
                foreach (Airplane airplane in this._airport.departure_airplanes)
                    airplane.track_drawer.Draw(sprite_batch);
                foreach (Airplane airplane in this._airport.arrival_airplanes)
                    airplane.track_drawer.Draw(sprite_batch);
            }
        }
    }
}
