using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;



namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Runway class
    /// </summary>
    public class Runway
    {
        private Game1 _game;
        private Airport _airport;
        public string number; // (06, 24L, 9C, ...)
        public Vector2 touch_down_position; // positon in airport area
        private Vector2 _center_position; // touch_down_position of the runway center in the airport area
        public Vector2 map_position; // touch_down_position in whole game
        public int altitude; // altitude of the land_runway in feet
        public int heading; // direction of land_runway in degree
        public Vector2 direction; // direction of land_runway in vector
        public float rotation; // direction of rwy in float
        private int _rwy_lenght; // land_runway lenght in meters
        private int _rwy_width; // land_runway width in meters
        private Texture2D _texture;
        private Texture2D _active_texture;
        private WeightCat _max_category; // maximum weight category
        private float _time;
        public bool is_active;
        // lights
        private Texture2D _land_ligh_tex;
        private Vector2[] _light_pos; // center posiiton of lights
        private Vector2 _land_lights_start_pos;
        private Vector2 _land_lights_end_pos;
        // landing waypoint
        public LandingWaypoint land_waypoint;

        public Runway(Game1 game, Airport airport, string rwy_number, Vector2 center_pos, int heading, int lenght, int width)
        {
            this._game = game;
            this._airport = airport;
            this.number = rwy_number;
            this._rwy_lenght = lenght / 37;
            this._rwy_width = width / 9;
            this.heading = heading;
            this._center_position = center_pos;
            this.map_position = GetInMapPosition();
            this.touch_down_position = GetTouchDownPosition();
            this.altitude = 0; // TODO:
            this.direction = General.GetDirection(this.heading); // direction of land_runway in vector2
            this._texture = CreateRwyTexture(this._game.GraphicsDevice, Color.Black, this._rwy_lenght, this._rwy_width);
            this._active_texture = CreateRwyTexture(this._game.GraphicsDevice, Config.rwry_active_color, this._rwy_lenght, this._rwy_width);
            this._light_pos = new Vector2[Config.land_lights_number];
            this._time = 0;
            this.is_active = false;
            LoadLightsTexture();
            InitLightsPositions();
            InitLandingWP(); // landing WP initialization
        }

        /// <summary>
        /// Create texture of land_runway in right size.
        /// </summary>
        /// <param name="graphicsDevice">grpahics device</param>
        /// <returns>land_runway texture (rectangle)</returns>
        private Texture2D CreateRwyTexture(GraphicsDevice graphicsDevice, Color color, int lenght, int width)
        {
            Texture2D rwy_tex = new Texture2D(graphicsDevice, lenght, width);
            Color[] color_data = new Color[lenght * width];
            for (int i = 0; i < color_data.Length; i++)
                color_data[i] = color;
            rwy_tex.SetData(color_data);
            return rwy_tex;
        }

        /// <summary>
        /// Return global touch_down_position in whole game area space.
        /// </summary>
        /// <returns>map touch_down_position</returns>
        private Vector2 GetInMapPosition ()
        {
            return new Vector2(this._center_position.X + this._airport.GetTexturePosition().X, this._center_position.Y + this._airport.GetTexturePosition().Y);
        }

        /// <summary>
        /// Get a top left touch_down_position of the runway in the airport area.
        /// </summary>
        /// <returns>vector2 touch_down_position of th etop left corner.</returns>
        private Vector2 GetTouchDownPosition ()
        {
            return General.MovePosition(this._center_position, General.GetOpositeHeading(this.heading), this._rwy_lenght / 2 - 10);
        }

        /// <summary>
        /// Get a rectangle as a button for click event on the land_runway.
        /// </summary>
        /// <returns>a click event rectangle</returns>
        private Rectangle GetEventSquare ()
        {
            return new Rectangle((int)this.touch_down_position.X + 400, (int)this.touch_down_position.Y + 50, this._rwy_width, this._rwy_lenght / 2);
        }

        /// <summary>
        /// Load landing light textures.
        /// </summary>
        private void LoadLightsTexture()
        {
            this._land_ligh_tex = this._game.Content.Load<Texture2D>("landing_lights_on");
        }

        /// <summary>
        /// Create positions of landing lights for land_runway.
        /// </summary>
        private void InitLightsPositions ()
        {
            int lights_length = 90;
            this._land_lights_start_pos = General.MovePosition(this.map_position, General.GetOpositeHeading(this.heading), lights_length + this._rwy_lenght / 2 + 7);
            this._land_lights_end_pos = General.MovePosition(this.map_position, General.GetOpositeHeading(this.heading), this._rwy_lenght / 2 + 7);
            for (int i = 0; i < this._light_pos.Length; i++)
            {
                int distance = lights_length / this._light_pos.Length * (i + 1);
                this._light_pos[i] = General.MovePosition(this._land_lights_start_pos, this.heading, distance);
            }
        }

        /// <summary>
        /// Initialization of landing waypoint for this land_runway.
        /// </summary>
        private void InitLandingWP()
        {
            Vector2 pos = General.MovePosition(this.map_position, General.GetOpositeHeading(this.heading), 100 + this._rwy_lenght / 2);
            this.land_waypoint = new LandingWaypoint(this._game, pos, this.number + " - LWP", this);
        }

        /// <summary>
        /// UpdateGame landing lights of this land_runway. Shift active light to another in period.
        /// </summary>
        public void UpdateLandingLights (GameTime game_time)
        {
            for (int i = 0; i < this._light_pos.Length; i++)
            {
                this._light_pos[i] = General.NextFramePosInHeading(game_time, this._light_pos[i], this.direction, Config.land_lights_speed);
                if (General.ObjectReachedPoint(this._light_pos[i], this._land_lights_end_pos))
                    this._light_pos[i] = this._land_lights_start_pos;
            }
        }

        /// <summary>
        /// Return parent airport of this land_runway.
        /// </summary>
        /// <returns>parent airport</returns>
        public Airport GetMyAirport()
        {
            return this._airport;
        }

        /// <summary>
        /// Draw right variant of the land_runway texture.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        public void Draw (SpriteBatch sprite_batch)
        {
            Vector2 origin = new Vector2(this._rwy_lenght / 2, this._rwy_width/ 2);
            if (!this.is_active)
            {
                sprite_batch.Draw(this._texture, this.map_position, null, Config.bg_color, General.GetRotation(this.heading), origin, 1, SpriteEffects.None, 0.0f);
            }
            else
            {
                //Vector2 origin = new Vector2(this._active_texture.Width / 2, this._active_texture.Height / 2);
                sprite_batch.Draw(this._active_texture, this.map_position, null, Config.bg_color, General.GetRotation(this.heading), origin, 1, SpriteEffects.None, 1.0f);
            }
        }

        /// <summary>
        /// TexDraw actual state of rwy lights.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        public void DrawLights (SpriteBatch sprite_batch)
        {
            foreach (Vector2 light_position in this._light_pos)
                sprite_batch.Draw(this._land_ligh_tex, General.GetDrawPosition(light_position, this._land_ligh_tex), Color.White);
        }
    }
}
