﻿using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
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
        private bool _speed_enabled;
        private bool _alt_enabled;
        private bool _head_enabled; // set heading control to enabled or disabled state
        private int _left_pad;
        //seq buttons
        private SequentialButton _speed_sbtn;
        private SequentialButton _altitude_sbtn;
        private SequentialButton _heading_sbtn;
        // buttons
        private Button _left_turn_btn;
        private Button _right_turn_btn;
        private Button _land_btn;

        public ControlPanel(Game1 game)
        {
            this._game = game;
            this.airplane = null;
            this._speed_enabled = true;
            this._alt_enabled = true;
            this._head_enabled = true;
            this._left_pad = 60;
            LoadControlButtons();
        }

        /// <summary>
        /// Load all elemnts in the control panel.
        /// </summary>
        private void LoadControlButtons ()
        {
            this._speed_sbtn = new SequentialButton(this._game, GetPosition(), new Vector2(this._left_pad, 5), "speed", 140, Config.min_control_speed, Config.max_control_speed);
            this._altitude_sbtn = new SequentialButton(this._game, GetPosition(), new Vector2(this._left_pad + 150, 5), "altitude", 160, Config.min_control_alt, Config.max_control_alt, false, 300, false);
            this._heading_sbtn = new SequentialButton(this._game, GetPosition(), new Vector2(this._left_pad + 320, 5), "heading", 152, 0, 360, true);
            this._left_turn_btn = new Button(this._game, GetPosition(), new Vector2(this._left_pad + 500, 5), "left turn", 116);
            this._right_turn_btn = new Button(this._game, GetPosition(), new Vector2(this._left_pad + 625, 5), "right turn", 122);
            this._land_btn = new Button(this._game, GetPosition(), new Vector2(this._left_pad + 755, 5), " land ", 75);
        }

        /// <summary>
        /// UpdateGame all element in the control panel in every moment.
        /// </summary>
        public void Update (GameTime game_time)
        {
            if (this.airplane != null)
            {
                this._speed_enabled = !this.airplane.autopilot_on; // disabled speed seq button if autopilot is on
                this._alt_enabled = !this.airplane.autopilot_on; // disabled altitude seq button if autopilot is on
                this._head_enabled = !(this.airplane.heading_autopilot_on || this.airplane.autopilot_on); // disabled heading seq button if the airplane is just in turn or autopilot is on
                this._speed_sbtn.value = this.airplane.delayer.desired_speed;
                this._altitude_sbtn.value = this.airplane.delayer.desired_alt;
                this._heading_sbtn.value = this.airplane.delayer.desired_heading;
                this.airplane.delayer.desired_speed = this._speed_sbtn.Update();
                this.airplane.delayer.desired_alt = this._altitude_sbtn.Update();
                if (this._head_enabled)
                    this.airplane.delayer.desired_heading = this._heading_sbtn.Update();
                UpdateOneClickButtons(game_time);
            }
            else
            {
                this._speed_enabled = true; // enabled speed control if there is no plane to control
                this._alt_enabled = true; // enabled altitude control if there is no plane to control
                this._head_enabled = true; // enabled heading control if there is no plane to control
                this._speed_sbtn.value = 0;
                this._altitude_sbtn.value = 0;
                this._heading_sbtn.value = 0;
            }
        }

        /// <summary>
        /// UpdateGame states of the standard buttons.
        /// </summary>
        private void UpdateOneClickButtons (GameTime game_time)
        {
            if (this._left_turn_btn.WasClicked())
            {
                this.airplane.autopilot.operation = AutopilotOperation.LeftTurn;
                this.airplane.heading_autopilot_on = true;
            }
            if (this._right_turn_btn.WasClicked())
            {
                this.airplane.autopilot.operation = AutopilotOperation.RightTurn;
                this.airplane.heading_autopilot_on = true;
            }
            if (this._land_btn.WasClicked())
                this.airplane.autopilot.PossibleToLand(game_time);
        }

        /// <summary>
        /// TexDraw all elements in control panel.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            this._speed_sbtn.Draw(spriteBatch, this._speed_enabled);
            this._altitude_sbtn.Draw(spriteBatch, this._alt_enabled);
            this._heading_sbtn.Draw(spriteBatch, this._head_enabled);
            this._left_turn_btn.Draw(spriteBatch);
            this._right_turn_btn.Draw(spriteBatch);
            this._land_btn.Draw(spriteBatch);
        }

        /// <summary>
        /// Return a position of the top left corner of the panel in the app window.
        /// </summary>
        /// <returns>vector position</returns>
        public Vector2 GetPosition ()
        {
            return new Vector2(400, 0);
        }
    }
}
