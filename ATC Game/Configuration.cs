using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;


namespace ATC_Game
{
    public class Configuration
    {
        public static string[] airplane_types = new string[] {"unknown"};
        public static string[] airplane_icons = new string[] {"plane"};
        public static string[] airplane_margin_icons = new string[] { "plane_margin" };

        //TRAFFIC GENERATION
        public static int gen_interval = 2; // interval between two new airplanes
        public static int gen_probability = 3; // probability of new airplane generation
        public static int min_speed = 50; // minimum speed of airplanes
        public static int max_speed = 101; // maximum speed of airplanes
        public static int plane_off_distance = 50; //distance between boudaries of a map and spawn point
        public static int min_plane_count = 2; // minimal count of planes in the map
        public static int max_plane_count = 6; // maximu count of planes in the map
        // arrival alert
        public static int alert_bound_dist = 15; // distance of display an arrival alert from boudary of a game map
        public static float alert_freq = 0.5f; // arrival alert blink rate
        //traffic boudaries
        public static int margin_dist = 75; // distance which set mardinal texture

        //COLORS
        public static Color bg_color = Color.Snow; // background color
    }
}
