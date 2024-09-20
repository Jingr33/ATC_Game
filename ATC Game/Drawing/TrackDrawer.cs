using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ATC_Game.Drawing
{
    /// <summary>
    /// It draws a track of th autopilot (lines between plane, its waypoints and landpoint and finally a destination airport)
    /// </summary>
    public class TrackDrawer
    {
        private Game1 _game;
        private Airplane _airplane;
        public bool is_active;
        private Texture2D _one_point_tex;
        private List<Vector2> _line_coords;
        private int _last_coords_count;
        private List<Vector2>[] _point_coords;
        private bool _regenerate_track;

        public TrackDrawer(Game1 game)
        {
            this._game = game;
            this.is_active = false;
            this._one_point_tex = LoadPointTexture(this._game.GraphicsDevice);
            this._line_coords = new List<Vector2>();
            this._last_coords_count = 0;
            this._regenerate_track = true;

        }

        /// <summary>
        /// Load a one touch_down_position texture of the track.
        /// </summary>
        /// <param name="graphics_device"></param>
        private Texture2D LoadPointTexture (GraphicsDevice graphics_device)
        {
            int size = 1;
            Texture2D point = new Texture2D(graphics_device, size, size);
            Color[] color_data = new Color[size * size];
            for (int i = 0; i < size * size; i++)
                color_data[i] = Config.track_color;
            point.SetData(color_data);
            return point;
        }

        /// <summary>
        /// UpdateGame airplane track drawer.
        /// </summary>
        /// <param name="airplane">airplane for draw its track</param>
        public void Update(Airplane airplane)
        {
            if (this.is_active)
            {
                this._airplane = airplane;
                GetLineCoords();
                this._regenerate_track = RegenerateTrack();
                GetTrackPoints();
            }
        }

        /// <summary>
        /// Get a list of line coords.
        /// </summary>
        private void GetLineCoords ()
        {
            if (this.is_active)
            {
                this._line_coords.Clear();
                this._line_coords.Add(this._airplane.center_position); // add plane pos
                foreach (Waypoint wp in this._airplane.waypoints) // add waypoints pos
                    this._line_coords.Add(wp.position);
                if (this._airplane.landpoint != null) // add landpoint pos
                    this._line_coords.Add(this._airplane.landpoint.position);
                this._line_coords.Add(this._airplane.runway.map_position); // add runway pos
            }
        }

        /// <summary>
        /// If the airplane track is changed or the airplane reached some track touch_down_position, return true, else false.
        /// </summary>
        private bool RegenerateTrack ()
        {
            bool track_changed = false;
            if (this._last_coords_count != this._line_coords.Count)
                track_changed = true;
            this._last_coords_count = this._line_coords.Count;
            return track_changed;
        }

        /// <summary>
        /// create whole track list witch all track _points (if it is necesarry).
        /// </summary>
        private void GetTrackPoints()
        {
            if (this._regenerate_track) // reset whole array
                this._point_coords = new List<Vector2>[this._line_coords.Count - 1];

            for (int i = 0; i < this._line_coords.Count - 1; i++)
            {
                if (i == 0 || this._regenerate_track)
                    this._point_coords[i] = GetOneTrackLine(this._line_coords[i], this._line_coords[i + 1]);
            }
        }

        /// <summary>
        /// Add one line between two track _points into the track list.
        /// </summary>
        /// <param name="start_point">start touch_down_position of the line</param>
        /// <param name="end_point">end touch_down_position of the line</param>
        /// <returns>list of texture positions</returns>
        private List<Vector2> GetOneTrackLine (Vector2 start_point, Vector2 end_point)
        {
            Vector2 direc = Vector2.Normalize(end_point - start_point);
            int iteration = (int)Math.Abs((end_point - start_point).Length() / direc.Length());
            List<Vector2> points = new List<Vector2>(iteration);
            for (int i = 0; i < iteration; i++)
                points.Add(new Vector2(start_point.X + direc.X * i, start_point.Y + direc.Y * i));
            return points;
        }

        /// <summary>
        /// Switch an activity of the drawer. Turn on or turn off.
        /// </summary>
        public void SwitchActivity ()
        {
            if (this.is_active)
                this.is_active = false;
            else
                this.is_active = true;
        }

        /// <summary>
        /// Draw an airplane track if the drawer is activated.
        /// </summary>
        public void Draw(SpriteBatch sprite_batch)
        {
            if (!this.is_active) return;
            foreach (List<Vector2> line in this._point_coords)
            {
                if (line == null) continue;
                for (int i = 0; i < line.Count; i++)
                {
                    if (i < 16 || i > line.Count - 16) continue;
                    sprite_batch.Draw(this._one_point_tex, line[i], Config.bg_color);
                }
            }
        }
    }
}
