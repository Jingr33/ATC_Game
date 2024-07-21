using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace ATC_Game.GameObjects
{
    internal class SequentialButton
    {
        private Game1 _game;
        private Texture2D _texture;
        private int _width;
        private int _height;
        private Color _bg_color;
        private Color _border_color;
        private string _title;
        private int _difference;
        private Vector2 _position;
        private SpriteFont _font;
        public int value { get; set; }

        public SequentialButton(Game1 game, Vector2 position, string title, int difference, int width)
        {
            this._game = game;
            this._position = position;
            this._title = title;
            this._difference = difference;
            this._width = width;
            this._height = 40;
            this._bg_color = Color.AliceBlue;
            this._border_color = Color.CadetBlue;
            this._font = game.Content.Load<SpriteFont>("font");
            this._texture = CreateSeqButtonTex(this._game.GraphicsDevice);
            this.value = 0;
        }

        /// <summary>
        /// Create a texture of a squential button.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns>texture of a button</returns>
        private Texture2D CreateSeqButtonTex(GraphicsDevice graphicsDevice)
        {
            Texture2D rect = new Texture2D(graphicsDevice, this._width, this._height);
            Color[] color_data = new Color[this._width * this._height];
            // background
            for (int i = 0; i < color_data.Length; i++)
                color_data[i] = this._bg_color;
            // border
            for (int j = 0; j < this._height; j++)
            {
                for (int k = 0; k < this._width; k++)
                {
                    bool border_hor = (j < 2) || (j >= this._height - 2);
                    bool border_ver = (k < 2) || (k >= 33 && k <= 34) || (k >= this._width - 34 && k <= this._width - 33) || (k >= this._width - 2);
                    if (border_hor || border_ver) // skip center of width
                        color_data[this._width * j + k] = this._border_color;
                    else
                        color_data[this._width * j + k] = this._bg_color;
                }
            }
            rect.SetData(color_data);
            return rect;
        }

        /// <summary>
        /// Update value in the seq button in every moment (check if the value in button was change and return actual value.
        /// </summary>
        /// <returns>value in the button</returns>
        public int Update()
        {
            IncreaseEvent();
            DecreaseEvent();
            return this.value;
        }

        /// <summary>
        /// Event for incrementation the value in the seq button
        /// </summary>
        private void IncreaseEvent ()
        {
            Rectangle button_zone = new Rectangle((int)this._position.X + this._width - 34, (int)this._position.Y + 0, 34, this._height);
            if (button_zone.Contains(this._game.mouse.Position) && this._game.mouse.LeftButton == ButtonState.Pressed)
                this.value++;
        }

        /// <summary>
        /// Event for decrementation the value in the seq button.
        /// </summary>
        private void DecreaseEvent()
        {
            Rectangle button_zone = new Rectangle((int)this._position.X + 0, (int)this._position.Y + 0, 34, this._height);
            if (button_zone.Contains(this._game.mouse.Position) && this._game.mouse.LeftButton == ButtonState.Pressed)
                this.value--;
        }

        /// <summary>
        /// Draw a Squential Button.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, new Rectangle((int)this._position.X, (int)this._position.Y, this._width, _height), Color.White);
            spriteBatch.DrawString(this._font, "-", new Vector2(this._position.X +  11, this._position.Y - 10), Color.DarkGray, 0, Vector2.Zero, 3f, SpriteEffects.None, 0);
            spriteBatch.DrawString(this._font, this.value.ToString(), new Vector2(this._position.X + this._width/2 - 10, this._position.Y + 15), Color.Black, 0, Vector2.Zero, 1.4f, SpriteEffects.None, 0);
            spriteBatch.DrawString(this._font, this._title.ToUpper(), new Vector2(this._position.X + 43, this._position.Y + 3), Color.Black, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            spriteBatch.DrawString(this._font, "+", new Vector2(this._position.X + this._width - 27, this._position.Y - 5), Color.Gray, 0, Vector2.Zero, 3f, SpriteEffects.None, 0);
        }

    }
}
