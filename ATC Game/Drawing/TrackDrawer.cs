using ATC_Game.GameObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.Drawing
{
    /// <summary>
    /// It draws a track of th autopilot (lines between plane, its waypoints and landpoint and finally a destination airport)
    /// </summary>
    public class TrackDrawer
    {
        private Game1 _game;
        private Airplane _airplane;
        private bool _is_active;

        public TrackDrawer(Game1 game, Airplane airplane)
        {
            this._game = game;
            this._airplane = airplane;
            this._is_active = false;
        }

        /// <summary>
        /// Switch an activity of the drawer. Turn on or turn off.
        /// </summary>
        public void SwitchActivity ()
        {
            if (this._is_active)
                this._is_active = false;
            else
                this._is_active = true;
        }

        /// <summary>
        /// Draw an airplane track if the drawer is activated.
        /// </summary>
        public void Draw()
        {
            //TODO
        }
    }
}
