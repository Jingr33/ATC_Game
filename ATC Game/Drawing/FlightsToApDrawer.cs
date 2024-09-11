using ATC_Game.GameObjects;
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
        private bool _is_active;

        public FlightsToApDrawer(Game1 game)
        {
            this._game = game;
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
        /// Draw an airplane tracks of flights to choosen airport if the drawer is activated.
        /// </summary>
        public void Draw ()
        {
            //TODO:
        }
    }
}
