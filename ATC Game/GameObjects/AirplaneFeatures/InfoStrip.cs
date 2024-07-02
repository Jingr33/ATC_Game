using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game.GameObjects.AirplaneFeatures
{
    public class InfoStrip
    {
        private Game1 _game;
        private Airplane _parent_airplane;
        private Texture2D _stripe_texture;
        private SpriteFont _font;
        private int _width;
        private int _height;
        private Color _color;

        public InfoStrip(Game1 game, Airplane parent_airplane)
        {
            this._game = game;
            this._parent_airplane = parent_airplane;
            this._width = 350;
            this._height = 50;
            this._color = Color.AliceBlue;
            this._font = this._game.Content.Load<SpriteFont>("font");
            this._stripe_texture = CreateStripeTexture(this._game.GraphicsDevice);
        }

        /// <summary>
        /// Return a rectangle texture of a airplane stripe
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        private Texture2D CreateStripeTexture(GraphicsDevice graphicsDevice)
        {
            return new Texture2D(graphicsDevice, this._width, this._height);
        }

        public void Draw (SpriteBatch spriteBatch)
        {

        }

    }
}
