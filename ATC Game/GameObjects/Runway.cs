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
        private Vector2 _position; // positon in airport area
        private Vector2 _direction; // direction of runway in vector
        private float _rotation; // direction of rwy in float
        private int _rwy_lenght; // runway lenght in meters
        private int _rwy_width; // runway width in meters
        private WeightCat _max_category; // maximum weight category

        public Runway(Game1 game, Airport airport, string rwy_number, Vector2 start_pos, Vector2 direction, int lenght, int width)
        {
            this._game = game;
            this._airport = airport;
            this._number = rwy_number;
            this._position = start_pos;
            this._direction = direction;
            this._rwy_lenght = lenght;
            this._rwy_width = width;
        }

        /// <summary>
        /// Create texture of runway in right size.
        /// </summary>
        /// <param name="graphicsDevice">grpahics device</param>
        /// <returns>runway texture (rectangle)</returns>
        private Texture2D CreateRwyTexture(GraphicsDevice graphicsDevice)
        {
            int tex_width = this._rwy_width / 6;
            int tex_lenght = this._rwy_lenght / 50;
            Texture2D rwy_tex = new Texture2D(graphicsDevice, tex_lenght, tex_width);
            Color[] color_data = new Color[tex_lenght * tex_width];
            for (int i = 0; i < color_data.Length; i++)
                color_data[i] = Color.Black;
            rwy_tex.SetData(color_data);
            return rwy_tex;
        }

    }
}
