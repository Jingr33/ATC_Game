using ATC_Game.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.Drawing
{
    /// <summary>
    /// TexDraw a trajectory line of an autopiloted flight.
    /// </summary>
    internal class TrajectoryDrawer
    {
        private Game1 _game;
        private Airplane _airplane;

        public TrajectoryDrawer(Game1 game, Airplane airplane)
        {
            this._game = game;
            this._airplane = airplane;
        }

        public void Draw()
        {

        }
    }
}
