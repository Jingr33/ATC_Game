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
        private string _number; // (06, 24L, 9C, ...)
        public Vector2 position; // positon in airport area
        public Vector2 map_position;
        public int altitude; // altitude of the runway in feet
        public int heading; // direction of runway in degree
        public Vector2 direction; // direction of runway in vector
        public float rotation; // direction of rwy in float
        private int _rwy_lenght; // runway lenght in meters
        private int _rwy_width; // runway width in meters
        private Texture2D _texture;
        private Texture2D _active_texture;
        private Vector2 _draw_position;
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

        public Runway(Game1 game, Airport airport, string rwy_number, Vector2 start_pos, int heading, int lenght, int width)
        {
            this._game = game;
            this._airport = airport;
            this._number = rwy_number;
            this.position = start_pos;
            this.map_position = GetInMapPosition();
            this.altitude = 0; // TODO:
            this.heading = heading;
            this.direction = General.GetDirection(this.heading); // direction of runway in vector2
            this._rwy_lenght = lenght;
            this._rwy_width = width;
            this._texture = CreateRwyTexture(this._game.GraphicsDevice, Color.Black);
            this._active_texture = CreateRwyTexture(this._game.GraphicsDevice, Color.DarkRed);
            this._draw_position = GetDrawPosition();
            this._light_pos = new Vector2[Config.land_lights_number];
            this._time = 0;
            this.is_active = false;
            LoadLightsTexture();
            InitLightsPositions();
            InitLandingWP(); // landing WP initialization

        }

        /// <summary>
        /// Create texture of runway in right size.
        /// </summary>
        /// <param name="graphicsDevice">grpahics device</param>
        /// <returns>runway texture (rectangle)</returns>
        private Texture2D CreateRwyTexture(GraphicsDevice graphicsDevice, Color color)
        {
            int tex_width = this._rwy_width / 9;
            int tex_lenght = this._rwy_lenght / 23;
            Texture2D rwy_tex = new Texture2D(graphicsDevice, tex_lenght, tex_width);
            Color[] color_data = new Color[tex_lenght * tex_width];
            for (int i = 0; i < color_data.Length; i++)
                color_data[i] = color;
            rwy_tex.SetData(color_data);
            return rwy_tex;
        }

        /// <summary>
        /// Return draw position of a runway.
        /// </summary>
        /// <returns>draw position</returns>
        private Vector2 GetDrawPosition ()
        {
            float x = this.map_position.X - 12; 
            float y = this.map_position.Y - this._texture.Height / 2;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Return global position in whole game area space.
        /// </summary>
        /// <returns>map position</returns>
        private Vector2 GetInMapPosition ()
        {
            return new Vector2(this.position.X + this._airport.GetTexturePosition().X, this.position.Y + this._airport.GetTexturePosition().Y);
        }

        /// <summary>
        /// Get a rectangle as a button for click event on the runway.
        /// </summary>
        /// <returns>a click event rectangle</returns>
        private Rectangle GetEventSquare ()
        {
            return new Rectangle((int)this.position.X + 400, (int)this.position.Y + 50, this._rwy_width, this._rwy_lenght / 2);
        }

        /// <summary>
        /// Load landing light textures.
        /// </summary>
        private void LoadLightsTexture()
        {
            this._land_ligh_tex = this._game.Content.Load<Texture2D>("landing_lights_on");
        }

        /// <summary>
        /// Create positions of landing lights for runway.
        /// </summary>
        private void InitLightsPositions ()
        {
            int lights_length = 90;
            this._land_lights_start_pos = General.PosInDirection(this.map_position, General.GetOpositeHeading(this.heading), lights_length + 17);
            this._land_lights_end_pos = General.PosInDirection(this.map_position, General.GetOpositeHeading(this.heading), 17);
            for (int i = 0; i < this._light_pos.Length; i++)
            {
                int distance = lights_length / this._light_pos.Length * (i + 1);
                this._light_pos[i] = General.PosInDirection(this.map_position, General.GetOpositeHeading(this.heading), distance);
            }
        }

        /// <summary>
        /// Initialization of landing waypoint for this runway.
        /// </summary>
        private void InitLandingWP()
        {
            Vector2 pos = General.PosInDirection(this.map_position, General.GetOpositeHeading(this.heading), 100);
            this.land_waypoint = new LandingWaypoint(this._game, pos, "LWP", this);
        }

        /// <summary>
        /// Update landing lights of this runway. Shift active light to another in period.
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
        /// Return parent airport of this runway.
        /// </summary>
        /// <returns>parent airport</returns>
        public Airport GetMyAirport()
        {
            return this._airport;
        }

        /// <summary>
        /// Draw right variant of the runway texture.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        public void Draw (SpriteBatch sprite_batch)
        {
            if (!this.is_active)
                sprite_batch.Draw(this._texture, this._draw_position, Color.White);
            else
                sprite_batch.Draw(this._active_texture, this._draw_position, Color.White);
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
