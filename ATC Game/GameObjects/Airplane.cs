﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Color = Microsoft.Xna.Framework.Color;
using System.Net.Mime;
using System.Reflection.Metadata;
using ATC_Game.GameObjects.AirplaneFeatures;

namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class for game object plane.
    /// </summary>
    public class Airplane : GameObject
    {
        public int id;
        private Game1 _game;
        private Texture2D _texture;
        private Texture2D _marginal_texture;
        private string _type_name;
        private Vector2 _center_position;
        private Vector2 _draw_position;
        private Vector2 _rotation_center;
        private float _rotation;
        private Vector2 _direction { get; set; }
        private int _speed { get; set; }
        private Vector2[] _last_positions = new Vector2[2];
        private List<Vector2> _trajectory { get; set; }
        public bool destroy_me;
        public float time_in_game;

        // ADITIONAL FEATURES
        private ArrivalAlert _arrival_alert;
        public InfoStrip info_strip;
        public bool in_margin;

        public Airplane (Game1 game, int id, Vector2 center_position, Vector2 direction, int speed, int type_number)
            : base (center_position)
        {
            this.id = id;
            this._game = game;
            this._type_name = SetTypeName(type_number);
            this._texture = SetTexture(type_number);
            this._marginal_texture = SetMarginalTexture(type_number);
            this._center_position = center_position;
            this._direction = direction;
            this._speed = speed;
            this._rotation_center = new Vector2(this._texture.Width / 2, this._texture.Height / 2);
            this._rotation = (float)Math.Atan2(this._direction.Y, this._direction.X);
            this._trajectory = new List<Vector2> ();
            this.destroy_me = false;
            this.time_in_game = 0;
            // aditional alerts
            this._arrival_alert = AddArrivalAlert();
            this.info_strip = new InfoStrip(this._game, this);
            this.in_margin = false;
        }

        /// <summary>
        /// Update position of plane in the map.
        /// </summary>
        /// <param name="game_time"></param>
        public void Update (GameTime game_time)
        {
            if (_trajectory.Count > 0)
            {
                // zadaná trajektorie
            }
            else
            {
                this._center_position = NewCoordStraightOn(game_time);
                this._draw_position = GetTexturePosition(this._center_position, this._texture);
            }
            SaveLastPositions();

            this.time_in_game += (float)game_time.ElapsedGameTime.TotalSeconds;
            this.IsMissedApproach();

            this.in_margin = IsInMargin();
        }

        /// <summary>
        /// Get new Vector2 center position of an aircraft in straight direction of flight.
        /// </summary>
        /// <returns>new coord</returns>
        private Vector2 NewCoordStraightOn(GameTime game_time)
        {
            float x_pos_diff = this._direction.X * this._speed * (float)game_time.ElapsedGameTime.TotalSeconds;
            float y_pos_diff = this._direction.Y * this._speed * (float)game_time.ElapsedGameTime.TotalSeconds;
            return new Vector2(this._center_position.X + x_pos_diff, this._center_position.Y + y_pos_diff);
        }

        /// <summary>
        /// Set last two positions to an array.
        /// </summary>
        private void SaveLastPositions()
        {
            this._last_positions[0] = this._last_positions[1];
            this._last_positions[1] = this._center_position;
        }

        /// <summary>
        /// Set type name of an airplane.
        /// </summary>
        /// <param name="type_number">number of specific airplane type</param>
        /// <returns>name of airplane type</returns>
        private string SetTypeName(int type_number)
        {
            return Configuration.airplane_types[type_number];
        }

        /// <summary>
        /// Set a Texture2D icon for specific type of airplane.
        /// </summary>
        /// <param name="type_number">number of a specific airplane type</param>
        /// <returns>airplane type texture</returns>
        private Texture2D SetTexture (int type_number)
        {
            return _game.Content.Load<Texture2D>(Configuration.airplane_icons[type_number]);
        }

        private Texture2D SetMarginalTexture (int type_number)
        {
            return _game.Content.Load<Texture2D>(Configuration.airplane_margin_icons[type_number]);
        }

        /// <summary>
        /// Draw object into the game canvas.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, this._draw_position, null, Configuration.bg_color, this._rotation, this._rotation_center, 1.0f, SpriteEffects.None, 0f);
        }


        /// <summary>
        /// Mthod from draw a plane object if its position is on the edge of game map.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void MarginalDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._marginal_texture, this._draw_position, null, Configuration.bg_color, this._rotation, this._rotation_center, 1.0f, SpriteEffects.None, 0f);
        }


        /// <summary>
        /// Check if an airplane is in game map or  out of game map
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsInGameMap()
        {
            bool x_check = (this._center_position.X >= 0) && (this._center_position.X <= this._game.GetGameAreaSize().X);
            bool y_check = (this._center_position.Y >= 0) && (this._center_position.Y <= this._game.GetGameAreaSize().Y);
            if (x_check && y_check) return true;
            return false;
        }
        public bool IsInGameMap(int boundary_overlap)
        {
            bool x_check = (this._center_position.X + boundary_overlap >= 0) && (this._center_position.X - boundary_overlap <= this._game.GetGameAreaSize().X);
            bool y_check = (this._center_position.Y + boundary_overlap >= 0) && (this._center_position.Y - boundary_overlap <= this._game.GetGameAreaSize().Y);
            if (x_check && y_check) return true;
            return false;
        }
        public bool IsInGameMap(Vector2 position)
        {
            bool x_check = (position.X >= 0) && (position.X <= this._game.GetGameAreaSize().X);
            bool y_check = (position.Y >= 0) && (position.Y <= this._game.GetGameAreaSize().Y);
            if (x_check && y_check) return true;
            return false;
        }

        /// <summary>
        /// Check if it is miised approach an a plane is already out of map bounds.
        /// </summary>
        public void IsMissedApproach()
        {
            if ((this.time_in_game > 12) && !IsInGameMap(Configuration.alert_bound_dist))
            {
                this.destroy_me = true;
                return;
            }
            this.destroy_me = false;
        }

        /// <summary>
        /// Check if an airplane is in the margin distance and direction from boundaries fo a game map.
        /// </summary>
        /// <returns>true or false</returns>
        private bool IsInMargin()
        {
            bool x_coord = IsInGameMap(new Vector2(this._center_position.X + this._direction.X * Configuration.margin_dist, 10));
            bool y_coord = IsInGameMap(new Vector2(10, this._center_position.Y + this._direction.Y * Configuration.margin_dist));
            if (x_coord && y_coord)
                return false;
            return true;
            //orpav to 
        }

        /// <summary>
        /// Create arrival alert of an airplane and add it into a arrival_alerts list.
        /// </summary>
        /// <returns>arrival alert object</returns>
        private ArrivalAlert AddArrivalAlert()
        {
            ArrivalAlert alert = new ArrivalAlert(this._game, this, this._center_position);
            this._game._airplane_logic.arrival_alerts.Add(alert);
            return alert;
        }
    }
}
