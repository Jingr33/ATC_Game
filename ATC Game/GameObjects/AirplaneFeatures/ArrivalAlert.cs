using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Color = Microsoft.Xna.Framework.Color;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;


namespace ATC_Game.GameObjects.AirplaneFeatures
{
    /// <summary>
    /// Class for generation and imaging an arrival alert
    /// </summary>
    public class ArrivalAlert
    {
        public bool destroy_me;

        private Game1 _game;
        private Airplane _parent_plane;
        private Vector2 _position;
        private Texture2D _texture;
        private int _bound_dist; // distance from boundaries of a game map
        private bool _is_turned_on;
        private float _one_state_time;
        private float _change_time; // elapsed _time from last switch of alert state

        public ArrivalAlert(Game1 game, Airplane parent_plane, Vector2 plane_start_pos)
        {
            this._game = game;
            this._parent_plane = parent_plane;
            this._is_turned_on = true;
            this._bound_dist = Config.alert_bound_dist;
            this.destroy_me = false;
            this._one_state_time = Config.alert_freq;
            this._change_time = 0;
            this._position = CalculatePosition(plane_start_pos);
            this._texture = _game.Content.Load<Texture2D>("arrival_alert");
        }

        public void UpdateAlert(GameTime game_time)
        {
            if (_parent_plane.IsInGameMap())
            {
                destroy_me = true;
                return;
            }

            _change_time += (float)game_time.ElapsedGameTime.TotalSeconds;
            if (_change_time > _one_state_time)
            {
                SwitchAlertState();
                _change_time = 0;
            }
        }

        /// <summary>
        /// Switch alert state which means if alert just shinig or not.
        /// </summary>
        private void SwitchAlertState()
        {
            if (_is_turned_on)
            {
                _is_turned_on = false;
                return;
            }
            _is_turned_on = true;
        }

        /// <summary>
        /// TexDraw a arrival alert into a game map
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void DrawArrivalAlert(SpriteBatch spriteBatch)
        {
            if (_is_turned_on)
                spriteBatch.Draw(_texture, _position, Color.White);
        }

        /// <summary>
        /// Calcute position in the game map of ariival alert.
        /// </summary>
        /// <param name="plane_pos">start position of a plane</param>
        /// <returns>point of alert</returns>
        private Vector2 CalculatePosition(Vector2 plane_pos)
        {
            Vector2 alert_pos = new Vector2(plane_pos.X, plane_pos.Y);
            Vector2 screen_size = _game.GetGameAreaSize();
            if (alert_pos.X <= 0)
                alert_pos.X = this._bound_dist;
            else if (alert_pos.X >= screen_size.X)
                alert_pos.X = screen_size.X - this._bound_dist;
            if (alert_pos.Y <= 0)
                alert_pos.Y = this._bound_dist;
            else if (alert_pos.Y >= screen_size.Y)
                alert_pos.Y = screen_size.Y - this._bound_dist;
            return alert_pos;
        }

        /// <summary>
        /// Remove sefl from a arrival alert list.
        /// </summary>
        private void DestroySelf()
        {
            _game.airplane_logic.arrival_alerts.Remove(this);
        }
    }
}
