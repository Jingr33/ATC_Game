﻿using ATC_Game.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace ATC_Game
{
    public class ControlPanel
    {
        private Game1 _game;
        public Airplane airplane;
        public bool is_active;
        private SequentialButton _speed_sbtn;
        private SequentialButton _altitude_sbtn;
        private SequentialButton _heading_sbtn;

        public ControlPanel(Game1 game)
        {
            this._game = game;
            this.airplane = null;
            LoadControlButtons();
        }

        /// <summary>
        /// Load all elemnts in the control panel.
        /// </summary>
        private void LoadControlButtons ()
        {
            this._speed_sbtn = new SequentialButton(this._game, new Vector2(10, 5), "speed", 5, 140);
            this._altitude_sbtn = new SequentialButton(this._game, new Vector2(300, 5), "altitude", 1, 160);
            this._heading_sbtn = new SequentialButton(this._game, new Vector2(600, 5), "heading", 1, 152);
        }

        /// <summary>
        /// Update all element in the control panel in every moment.
        /// </summary>
        public void Update ()
        {
            if (this.airplane != null)
            {
                this._speed_sbtn.value = this.airplane.speed;
                this._altitude_sbtn.value = this.airplane.altitude;
                this._heading_sbtn.value = this.airplane.heading;
                this.airplane.speed = this._speed_sbtn.Update();
                this.airplane.altitude = this._altitude_sbtn.Update();
                this.airplane.heading = this._heading_sbtn.Update();
            }
            else
            {
                this._speed_sbtn.value = 0;
                this._altitude_sbtn.value = 0;
                this._heading_sbtn.value = 0;
            }
        }

        /// <summary>
        /// Draw all elements in control panel.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            this._speed_sbtn.Draw(spriteBatch);
            this._altitude_sbtn.Draw(spriteBatch);
            this._heading_sbtn.Draw(spriteBatch);
        }
    }
}
