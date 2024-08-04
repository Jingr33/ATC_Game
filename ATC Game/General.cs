using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace ATC_Game
{
    /// <summary>
    /// Class with general methods.
    /// </summary>
    public class General
    {
        /// <summary>
        /// Convert file data (format: xx:yy) to vector2
        /// </summary>
        /// <param name="file_data">information from file</param>
        /// <returns>vactor2 variable</returns>
        public static Vector2 FileDataToVector2(string file_data)
        {
            string[] array = file_data.Split(':');
            return new Vector2(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Get a direction of a flight in vector
        /// </summary>
        /// <param name="heading">heading of the airplane (0-360)</param>
        /// <returns>normal vector direction</returns>
        public static Vector2 GetDirection(int heading)
        {
            heading -= 90;
            if (heading < 0)
                heading += 360;
            double x = Math.Cos(heading * Math.PI / 180);
            double y = Math.Sin(heading * Math.PI / 180);
            return new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// Get a direciton of flight (heading) in degree.
        /// </summary>
        /// <param name="direction">direction of flight in vector</param>
        /// <returns>heading in degree</returns>
        public static int GetHeading(Vector2 direction)
        {
            int heading = (int)(Math.Atan2(direction.Y, direction.X) / Math.PI * 180 + 90);
            if (heading >= 0)
                return heading;
            return heading + 360;
        }


        /// <summary>
        /// Set next point in specified direction in specified distance from original point.
        /// </summary>
        /// <param name="position">original point position</param>
        /// <param name="heading">direction in degree</param>
        /// <param name="distance">distance from original point</param>
        /// <returns>vector of final position</returns>
        public static Vector2 PosInDirection (Vector2 position, int heading, int distance)
        {
            Vector2 direc = GetDirection(heading);
            float x_pos = position.X + direc.X * distance;
            float y_pos = position.Y + direc.Y * distance;
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// Get new Vector2 position in direction of heading for next frame of the game panel.
        /// </summary>
        /// <param name="game_time">game time</param>
        /// <param name="orig_pos">original position of object</param>
        /// <param name="direc">direction of object in vector2</param>
        /// <param name="speed">speed of object. Default is 1.</param>
        /// <returns>new coord</returns>
        public static Vector2 NextFramePosInHeading(GameTime game_time, Vector2 orig_pos, Vector2 direc, int speed = 1)
        {
            float x_pos = orig_pos.X + direc.X * (float)game_time.ElapsedGameTime.TotalSeconds * speed;
            float y_pos = orig_pos.Y + direc.Y * (float)game_time.ElapsedGameTime.TotalSeconds * speed;
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// Return oposite heading (oposite direction).
        /// </summary>
        /// <param name="heading">original heading</param>
        /// <returns>oposite heading</returns>
        public static int GetOpositeHeading (int heading)
        {
            heading -= 180;
            if (heading < 0)
                heading += 360;
            return heading;
        }

        /// <summary>
        /// Check if some obejct (center) position reached some position in the game map.
        /// </summary>
        /// <param name="obj_pos">a moving object position</param>
        /// <param name="ref_pos">a reference position for check</param>
        /// <param name="circle_dist">direction of straight move of an object</param>
        /// <returns></returns>
        public static bool ObjectReachedPoint (Vector2 obj_pos, Vector2 ref_pos, int circle_dist = 10)
        {
            bool cond = Math.Pow(obj_pos.X - ref_pos.X, 2) + Math.Pow(obj_pos.Y - ref_pos.Y, 2) <= circle_dist * circle_dist;
            if (cond)
                return true;
            return false;
        }

        /// <summary>
        /// Calculate position of top left corner for draw from center position of a game object.
        /// </summary>
        /// <param name="center_pos">center point</param>
        /// <param name="texture">a game object texture</param>
        /// <returns>draw position</returns>
        public static Vector2 GetDrawPosition(Vector2 center_pos, Texture2D texture)
        {
            return new Vector2(center_pos.X - texture.Width/2, center_pos.Y - texture.Height/2);
        }
        /// <summary>
        /// Calculate position of top left corner for draw from center position of a game object.
        /// </summary>
        /// <param name="center_pos">array of center points</param>
        /// <param name="texture">a game object texture</param>
        /// <returns>draw position</returns>
        public static Vector2[] GetDrawPosition(Vector2[] center_pos, Texture2D texture)
        {
            Vector2[] draw_pos = new Vector2[center_pos.Length];
            for (int i = 0; i < center_pos.Length; i++)
                draw_pos[i] = new Vector2(center_pos[i].X - texture.Width / 2, center_pos[i].Y - texture.Height / 2);
            return draw_pos;
        }
    }
}
