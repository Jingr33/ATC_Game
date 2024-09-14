using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace ATC_Game.GameObjects
{
    /// <summary>
    /// Class for creating normal buttons.
    /// </summary>
    internal class Button
    {
        private Game1 _game;
        private Texture2D _texture;
        private int _width;
        private int _height;
        private Color _bg_color;
        private Color _border_color;
        private string _title;
        private Vector2 _position;
        private Vector2 _parent_position; // top left position of the parent panel (due to click event button positions)
        private SpriteFont _font;

        private Rectangle _event_space;
        ButtonState _mouse_state;
        ButtonState _last_state;

        public Button(Game1 game, Vector2 parent_pos, Vector2 position, string title, int width)
        {
            this._game = game;
            this._position = position;
            this._parent_position = parent_pos;
            this._title = title;
            this._width = width;
            this._height = 40;
            this._bg_color = Color.AliceBlue;
            this._border_color = Color.CadetBlue;
            this._font = game.Content.Load<SpriteFont>("font");
            this._texture = CreateButtonTex(this._game.GraphicsDevice);
            this._mouse_state = ButtonState.Released;
            this._last_state = ButtonState.Released;
            LoadEvent();
        }

        /// <summary>
        /// Create visual part of a button.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        private Texture2D CreateButtonTex (GraphicsDevice graphicsDevice)
        {
            Texture2D rect = new Texture2D(graphicsDevice, this._width, this._height);
            Color[] color_data = new Color[this._width * this._height];
            for (int j = 0; j < this._height; j++)
            {
                for (int k = 0; k < this._width; k++)
                {
                    bool border_hor = (j < 2) || (j >= this._height - 2);
                    bool border_ver = (k < 2) || (k >= this._width - 2);
                    if (border_hor || border_ver)
                        color_data[this._width * j + k] = this._border_color;
                    else
                        color_data[this._width * j + k] = this._bg_color;
                }
            }
            rect.SetData(color_data);
            return rect;
        }

        /// <summary>
        /// Create event part of the button.
        /// </summary>
        private void LoadEvent ()
        {
            this._event_space = new Rectangle((int)this._position.X + (int)this._parent_position.X, (int)this._position.Y + (int)this._parent_position.Y, this._width, this._height);
        }

        /// <summary>
        /// Activate button function.
        /// </summary>
        /// <returns>bool value if the event was clicked</returns>
        public bool WasClicked()
        {
            bool is_clicked = false;
            this._mouse_state = this._game.mouse.LeftButton;
            if (this._event_space.Contains(this._game.mouse.Position) && this._mouse_state == ButtonState.Released && this._last_state == ButtonState.Pressed)
                is_clicked = true;
            this._last_state = this._mouse_state;
            return is_clicked;
        }

        /// <summary>
        /// TexDraw a Button.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, new Rectangle((int)this._position.X, (int)this._position.Y, this._width, _height), Color.White);
            spriteBatch.DrawString(this._font, this._title.ToUpper(), new Vector2(this._position.X + 5, this._position.Y + 12), Color.Black, 0, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
        }

    }
}
