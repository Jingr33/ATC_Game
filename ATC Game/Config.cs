﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;


namespace ATC_Game
{
    public class Config
    {
        // ICONS
        public static string[] airplane_icons = new string[] {"plane"};
        public static string[] airplane_margin_icons = new string[] { "plane_margin" };
        public static string[] airplane_active_icons = new string[] { "plane_active" };

        // AIRPLANE VARIANTS
        public static string[] airplane_types = new string[] {"UNKNOWN"};
        public static string[] airplane_callsigns = new string[] { "CSA123" };
        public static string[] airplane_registration = new string[] { "OK-000" };
        public static string[] destinations = new string[] { "Brno", "Ostrava" };

        //TRAFFIC GENERATION
        public static int gen_interval = 2; // interval between two new airplanes
        public static int gen_probability = 3; // probability of new airplane generation
        public static int min_speed = 10; // minimum speed of airplanes
        public static int max_speed = 21; // maximum speed of airplanes
        public static int min_altitude = 30; // minimum flight level of arrival ariplane
        public static int max_altitude = 101; // maximum flight level of departure airplane
        public static int plane_off_distance = 50; //distance between boudaries of a map and spawn point
        public static int min_plane_count = 1; // minimal count of planes in the map
        public static int max_plane_count = 1; // maximu count of planes in the map
        // arrival alert
        public static int alert_bound_dist = 15; // distance of display an arrival alert from boudary of a game map
        public static float alert_freq = 0.5f; // arrival alert blink rate
        //traffic boudaries
        public static int margin_dist = 75; // distance which set mardinal texture
        // airplane stripe
        public static int stripe_height = 70; // airplane stripe height in plane stripe area
        public static int stripe_gap = 5; // space between two plane stripes in plane stripes area
        public static Color stripe_color = Color.AliceBlue; // airplane stripe color
        public static Color active_stripe_color = Color.CadetBlue;

        //COLORS
        public static Color bg_color = Color.Snow; // background color


        // REACTION DELAY
        public static float speed_step_time = 0.4f; // one knot change of speed duration
        public static float alt_step_time = 0.5f; // one flight level (100 feet) of altitude duration
    }
}
