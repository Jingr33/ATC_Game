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
        /// <param name="heading">degree_angle of the airplane (0-360)</param>
        /// <returns>normal vector direction</returns>
        public static Vector2 GetDirection(float heading)
        {
            heading = GetAngle(heading);
            double x = Math.Cos(heading * Math.PI / 180);
            double y = Math.Sin(heading * Math.PI / 180);
            Vector2 direction = new Vector2((float)x, (float)y);
            return Vector2.Normalize(direction);
        }

        /// <summary>
        /// Get a heading of the object from angle in degree_angle.
        /// </summary>
        /// <param name="angle">angle in degree_angle</param>
        /// <returns>heading</returns>
        public static float GetHeading (float angle)
        {
            return HeadingBordersCheck(angle + 90);
        }
        /// <summary>
        /// Get a direciton of the obejct in heading value.
        /// </summary>
        /// <param name="direction">direction of flight in vector</param>
        /// <returns>heading in degree_angle</returns>
        public static float GetHeading(Vector2 direction)
        {
            float degree_angle = (float)(Math.Atan2(direction.Y, direction.X) / Math.PI * 180);
            return GetHeading(HeadingBordersCheck(degree_angle));
        }

        /// <summary>
        /// Get an degree_angle of the airplane from degree_angle value.
        /// </summary>
        /// <param name="heading">degree_angle of the airplane</param>
        /// <returns>degree_angle of the airplane in degree_angle</returns>
        public static float GetAngle(float heading)
        {
            return HeadingBordersCheck(heading - 90);
        }

        /// <summary>
        /// Get a direciton of the obejct in heading value.
        /// </summary>
        /// <param name="direction">vector direction of the object</param>
        /// <returns>direction in radians</returns>
        public static float GetRotation(Vector2 direction)
        {
            return (float)Math.Atan2(direction.Y, direction.X);
        }
        /// <summary>
        /// GGet a direciton of the obejct in heading value.
        /// </summary>
        /// <param name="heading">heading of the object</param>
        /// <returns>direction in radians</returns>
        public static float GetRotation(float heading)
        {
            float angle = GetAngle(heading);
            return angle / 180 * (float)Math.PI;
        }

        /// <summary>
        /// Set next point in specified direction in specified distance from original point.
        /// </summary>
        /// <param name="position">original point touch_down_position</param>
        /// <param name="heading">direction in degree_angle</param>
        /// <param name="distance">distance from original point</param>
        /// <returns>vector of final touch_down_position</returns>
        public static Vector2 PosInDirection (Vector2 position, int heading, int distance)
        {
            Vector2 direc = GetDirection(heading);
            float x_pos = position.X + direc.X * distance;
            float y_pos = position.Y + direc.Y * distance;
            return new Vector2(x_pos, y_pos);
        }

        /// <summary>
        /// Get new Vector2 touch_down_position in direction of degree_angle for next frame of the game panel.
        /// </summary>
        /// <param name="game_time">game time</param>
        /// <param name="orig_pos">original touch_down_position of object</param>
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
        /// Return oposite degree_angle (oposite direction).
        /// </summary>
        /// <param name="heading">original degree_angle</param>
        /// <returns>oposite degree_angle</returns>
        public static int GetOpositeHeading (int heading)
        {
            heading -= 180;
            if (heading < 0)
                heading += 360;
            return heading;
        }

        /// <summary>
        /// Check if the degree_angle value is between 0 and 360, return modied value if it is necessary.
        /// </summary>
        /// <param name="heading">degree_angle value for check</param>
        /// <returns>check resp. modified degree_angle value</returns>
        public static float HeadingBordersCheck(float heading)
        {
            //if (heading >= 0 && heading < 360) return heading;
            //else if (heading < 0) return heading + 360;
            //else return heading - 360;
            heading = heading % 360;
            if (heading < 0)
                heading += 360;
            return heading;
        }

        /// <summary>
        /// Get a degree_angle of straight flight from actual touch_down_position to the waypoint touch_down_position.
        /// </summary>
        /// <param name="actual_pos">actual touch_down_position of an object</param>
        /// <param name="wp_pos">a touch_down_position of the waypoint</param>
        /// <returns>degree_angle to the waypoint</returns>
        public static float HeadingToWaypoint (Vector2 actual_pos, Vector2 wp_pos)
        {
            Vector2 direction = Vector2.Normalize(wp_pos - actual_pos); // normal vector with direction of the flight
            return GetHeading(direction);
        }

        /// <summary>
        /// Check if some obejct (center) touch_down_position reached some touch_down_position in the game map.
        /// </summary>
        /// <param name="obj_pos">a moving object touch_down_position</param>
        /// <param name="ref_pos">a reference touch_down_position for check</param>
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
        /// Calculate touch_down_position of top left corner for draw from center touch_down_position of a game object.
        /// </summary>
        /// <param name="center_pos">center point</param>
        /// <param name="texture">a game object texture</param>
        /// <returns>draw touch_down_position</returns>
        public static Vector2 GetDrawPosition(Vector2 center_pos, Texture2D texture)
        {
            return new Vector2(center_pos.X - texture.Width/2, center_pos.Y - texture.Height/2);
        }
        /// <summary>
        /// Calculate touch_down_position of top left corner for draw from center touch_down_position of a game object.
        /// </summary>
        /// <param name="center_pos">array of center points</param>
        /// <param name="texture">a game object texture</param>
        /// <returns>draw touch_down_position</returns>
        public static Vector2[] GetDrawPosition(Vector2[] center_pos, Texture2D texture)
        {
            Vector2[] draw_pos = new Vector2[center_pos.Length];
            for (int i = 0; i < center_pos.Length; i++)
                draw_pos[i] = new Vector2(center_pos[i].X - texture.Width / 2, center_pos[i].Y - texture.Height / 2);
            return draw_pos;
        }

        /// <summary>
        /// Get a moved position from the original position of the object by a distance in some degree_angle.
        /// </summary>
        /// <param name="orig_pos">original position of the object</param>
        /// <param name="heading"> degree_angle of the object/param>
        /// <param name="distance">absolute distance of the shift</param>
        /// <returns>new position of the object</returns>
        public static Vector2 MovePosition (Vector2 orig_pos, float heading, float distance)
        {
            Vector2 direc = GetDirection(heading);
            return new Vector2(orig_pos.X + direc.X * distance, orig_pos.Y + direc.Y * distance);

        }

        /// <summary>
        /// It calculate a guideline of line in some direction.
        /// </summary>
        /// <param name="heading">degree_angle of direction</param>
        /// <returns>guideline value</returns>
        public static float GetGuideline (float heading)
        {
            float degree = HeadingBordersCheck(heading + 90);
            return (float)Math.Tan(MathHelper.ToRadians(degree)); // ??? degree_angle to stupne
        }

        /// <summary>
        /// Calculate distance between point and line.
        /// </summary>
        /// <param name="point">point coordinates</param>
        /// <param name="line_a">a parameter of line</param>
        /// <param name="line_b">b parameter of line</param>
        /// <param name="line_c">c parameter of line</param>
        /// <returns>distance between line and point</returns>
        public static double PointAndLineDist (Vector2 point, float line_a, float line_b, float line_c)
        {
            return Math.Abs(line_a*point.X + line_b*point.Y + line_c) / Math.Sqrt(Math.Pow(line_a, 2) + Math.Pow(line_b, 2));
        }

        /// <summary>
        /// Switch a bool value to the second variant.
        /// </summary>
        /// <param name="actual_value">actual value of the bool</param>
        /// <returns></returns>
        public static bool Switcher(bool actual_value)
        {
            if (actual_value)
                return false;
            else
                return true;
        }
    }
}
