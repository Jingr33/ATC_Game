using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.GameObjects.AirplaneFeatures
{
    /// <summary>
    /// Class for creation a temporary route of an airplane (set trajectory, speed, altitude, heading)
    /// </summary>
    internal class FlightRoute
    {
        public Vector2[] trajectory; // array of points of airplane trajectory
        public int[] speed;
        public int[] altitude;
        public int[] heading;

        public FlightRoute()
        {
        }

        public void SetTakeOffRoute (Vector2 start_pos, Vector2 direction)
        {
        }
    }
}
