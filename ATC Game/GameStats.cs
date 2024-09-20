using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game
{
    /// <summary>
    /// Ćlass for managing game statistics (_time, _points, numbeer of planes, ...)
    /// </summary>
    public class GameStats
    {
        private Game1 _game;
        private float _time;
        private int _points;
        private int _landed_aircraft;
        private int _departed_aircraft;

        public GameStats(Game1 game)
        {
            this._game = game;
            this._time = 0;
            this._points = 0;
            this._landed_aircraft = 0;
            this._departed_aircraft = 0;
        }

        /// <summary>
        /// UpdateGame game stats vlaues.
        /// </summary>
        /// <param name="game_time">game _time</param>
        public void Update(GameTime game_time)
        {
            this._time += (float)game_time.ElapsedGameTime.TotalSeconds;
            this._points = 12;
            this._landed_aircraft = 23;
            this._departed_aircraft = 88;
        }

        /// <summary>
        /// Return _time in seconds.
        /// </summary>
        /// <returns></returns>
        public int GetTimeInSeconds ()
        {
            return (int)this._time;
        }

        /// <summary>
        /// Get _time info in string format (minutes:seconds).;
        /// </summary>
        /// <returns></returns>
        public string GetTimeAsString ()
        {
            int total_sec = GetTimeInSeconds();
            int minutes = total_sec / 60;
            int seconds = total_sec % 60;
            string str_min = minutes.ToString();
            string str_sec = seconds.ToString();
            if (seconds < 10)
                str_sec = "0" + str_sec;
            if (minutes < 10)
                str_min = "0" + str_min;

            return string.Format("{0}:{1}", str_min, str_sec);
        }

        /// <summary>
        /// Get a points number as a string.
        /// </summary>
        /// <returns></returns>
        public string GetPoints ()
        {
            return this._points.ToString();
        }

        /// <summary>
        /// Get number of landed aircraft as a string.
        /// </summary>
        /// <returns></returns>
        public string GetLandedAircraftNum ()
        {
            return this._landed_aircraft.ToString();
        }

        /// <summary>
        /// Get number of departed aircraft as a string.
        /// </summary>
        /// <returns></returns>
        public string GetDepartedAircraftNum ()
        {
            return this._departed_aircraft.ToString();
        }
    }
}
