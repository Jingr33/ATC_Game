using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;


namespace ATC_Game.Drawing
{
    /// <summary>
    /// TexDraw a trajectory line of an autopiloted flight.
    /// </summary>
    internal class TrajectoryDrawer
    {
        private Game1 _game;
        private Airplane _airplane;
        private Texture2D _texture;
        private int _tex_width;
        private int _tex_height;

        public TrajectoryDrawer(Game1 game, Airplane airplane)
        {
            this._game = game;
            this._airplane = airplane;
            this._tex_width = 2;
            this._tex_height = 2;
            GetPointTexture();
        }

        private void GetPointTexture ()
        {

            this._texture = new Texture2D(this._game.GraphicsDevice, this._tex_width, this._tex_height);
            Color[] color_data = new Color[this._tex_width * this._tex_height];
            for (int i = 0; i < color_data.Length; i++)
                color_data[i] = Config.traj_color;
            this._texture.SetData(color_data);
        }

        public void Draw(SpriteBatch spriteBatch, ConcurrentQueue<Vector2> trajectory)
        {
            Vector2[] traj_ar = trajectory.ToArray(); 
            for (int i = 0; i < traj_ar.Length; i += 600)
                spriteBatch.Draw(this._texture, traj_ar[i], Config.traj_color);
        }
    }
}
