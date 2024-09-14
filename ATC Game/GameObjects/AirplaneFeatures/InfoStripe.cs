using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.GameObjects.AirplaneFeatures
{
    public class InfoStripe : StripeTexture
    {
        private Game1 _game;
        private Airplane _plane;
        public int id;
        private Texture2D _stripe_texture;
        private SpriteFont _font;
        private Color _color;
        private Color _active_color;
        public bool is_active;

        public InfoStripe(Game1 game, Airplane parent_airplane) : base(game)
        {
            this._game = game;
            this._plane = parent_airplane;
            this.id = parent_airplane.id;
            this._color = Config.stripe_color;
            this._active_color = Config.active_stripe_color;
            this._stripe_texture = CreateStripeTexture(this._game.GraphicsDevice, this._color);
            this.is_active = this._plane.is_active;
        }

        /// <summary>
        /// Return rectangle of the stripe touch_down_position for click event.
        /// </summary>
        /// <param name="stripe_row">row of the stripe in the blosk</param>
        /// <returns>rectangle of click event button</returns>
        public Rectangle GetStripeSquare (int stripe_row)
        {
            return new Rectangle(GetXPosition(), GetYPosition(stripe_row) + 50, this._width, this._height);
        }

        /// <summary>
        /// TexDraw a info stripe for a given airplane (it depends on the is_active state).
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        /// <param name="y_pos">row (touch_down_position) of a stripe in plane sptripe area</param>
        public void Draw (SpriteBatch spriteBatch, int stripe_row)
        {
            int x_pos = GetXPosition();
            int y_pos = GetYPosition(stripe_row);
            Vector2 pos = new Vector2(x_pos, y_pos);
            spriteBatch.Draw(this._stripe_texture, pos, SetColor());
            AddStrings(spriteBatch, pos, this._plane.type, this._plane.weight_cat, this._plane.callsign, this._plane.registration,
                       this._plane.oper_type, this._plane.flight_section, this._plane.altitude, this._plane.heading, this._plane.speed);
        }

        /// <summary>
        /// Return X touch_down_position of the stripe in the stripe area.
        /// </summary>
        /// <returns>integer x positon coord</returns>
        private int GetXPosition ()
        {
            return (int)((this._game.GetPlaneStripsArea().X - this._width) / 2);
        }

        /// <summary>
        /// Return Y touch_down_position of the stripe in the stripe area
        /// </summary>
        /// <param name="stripe_row">row of the stripe in the block</param>
        /// <returns>Y coord</returns>
        private int GetYPosition (int stripe_row)
        {
            return (Config.stripe_height + Config.stripe_gap) * stripe_row + Config.stripe_gap;
        }

        /// <summary>
        /// Set the backgroud color of a stripe depending on the is_active state.
        /// </summary>
        /// <returns></returns>
        private Color SetColor ()
        {
            if (this.is_active)
                return this._active_color;
            return _color;
        }

    }
}
