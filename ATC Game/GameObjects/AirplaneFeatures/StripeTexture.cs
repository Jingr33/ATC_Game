using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.GameObjects.AirplaneFeatures
{
    public class StripeTexture
    {
        protected SpriteFont _font;
        protected int _width;
        protected int _height;

        public StripeTexture(Game1 game)
        {
            this._font = game.Content.Load<SpriteFont>("font");
            this._width = 370;
            this._height = Config.stripe_height;
        }

        /// <summary>
        /// Return a rectangle texture of a airplane stripe
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        protected virtual Texture2D CreateStripeTexture(GraphicsDevice graphicsDevice, Color color)
        {
            Texture2D stripe = new Texture2D(graphicsDevice, this._width, this._height);
            Color[] color_data = new Color[this._width * this._height];
            for (int i = 0; i < color_data.Length; i++)
                color_data[i] = color;
            stripe.SetData(color_data);
            return stripe;
        }

        /// <summary>
        /// Add string states of flight info a stripe
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        /// <param name="pos">position of top left coner of a stripe</param>
        /// <param name="type">airplane type</param>
        /// <param name="w_cat">weight category</param>
        /// <param name="callsign">callsign</param>
        /// <param name="regis">registration number</param>
        /// <param name="oper_type">operation type (arrival / departure)</param>
        /// <param name="section">section of flight</param>
        /// <param name="altitude">actual altitude</param>
        /// <param name="heading">actual heading</param>
        /// <param name="g_speed">actual ground speed</param>
        protected virtual void AddStrings (SpriteBatch spriteBatch, Vector2 pos, string type, WeightCat w_cat, string callsign, string regis, 
                                           OperationType oper_type, FlightSection section, int altitude, int heading, int g_speed)
        {
            int sh = Config.stripe_height; // stripe height
            string type_weight = string.Format("{0}/{1}", type, w_cat.ToString());
            string callsign_str = string.Format("call: {0}", callsign);
            string regis_str = string.Format("reg: {0}", regis);
            string oper_type_str = string.Format("{0}", oper_type.ToString().ToUpper());
            string section_str = string.Format("{0}", section.ToString());
            string altitude_str = string.Format("alt: {0} ft", altitude);
            string speed_str = string.Format("gsp: {0} kt", g_speed);
            string heading_str = string.Format("head: {0}", heading);
            spriteBatch.DrawString(this._font, type_weight, new Vector2(pos.X + 5, pos.Y + 5), Color.Black);
            spriteBatch.DrawString(this._font, callsign_str, new Vector2(pos.X + 5, pos.Y + sh/3 + 5), Color.Black);
            spriteBatch.DrawString(this._font, regis_str, new Vector2(pos.X + 5, pos.Y + sh/3*2 + 5), Color.Black);
            spriteBatch.DrawString(this._font, oper_type_str, new Vector2(pos.X + 155, pos.Y + 5), Color.Black);
            spriteBatch.DrawString(this._font, section_str, new Vector2(pos.X + 155, pos.Y + sh/2 + 5), Color.Black);
            spriteBatch.DrawString(this._font, altitude_str, new Vector2(pos.X + 285, pos.Y + 5), Color.Black);
            spriteBatch.DrawString(this._font, speed_str, new Vector2(pos.X + 285, pos.Y + sh/3 + 5), Color.Black);
            spriteBatch.DrawString(this._font, heading_str, new Vector2(pos.X + 285, pos.Y + sh/3*2 + 5), Color.Black);
        }
    }
}
