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
    /// <summary>
    /// Class for creating a sequential buttons.
    /// </summary>
    internal class SequentialButton
    {
        private Game1 _game;
        private Texture2D _texture;
        private Vector2 _parent_position; // top left corner of the panel (due to click event positions)
        private int _width;
        private int _height;
        private Color _bg_color;
        private Color _border_color;
        private string _title;
        private int _difference;
        private Vector2 _position;
        private SpriteFont _font;
        public int value { get; set; }
        private bool smooth;
        private int _min_value;
        private int _max_value;
        private bool _repeatable;

        private Rectangle _increase_btn;
        private Rectangle _decrease_btn;
        private MouseState _last_state;

        public SequentialButton(Game1 game, Vector2 parent_pos, Vector2 position, string title, int width, int min_val, int max_val, bool repeatable = false, 
                                int difference = 1, bool smooth = true)
        {
            this._game = game;
            this._position = position;
            this._parent_position = parent_pos;
            this._title = title;
            this._difference = difference;
            this.smooth = smooth;
            this._min_value = min_val;
            this._max_value = max_val;
            this._repeatable = repeatable;
            this._width = width;
            this._height = 40;
            this._bg_color = Color.AliceBlue;
            this._border_color = Color.CadetBlue;
            this._font = game.Content.Load<SpriteFont>("font");
            this._texture = CreateSeqButtonTex(this._game.GraphicsDevice);
            this.value = 0;
            this._last_state = Mouse.GetState();
            LoadChangeButtons();
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
        /// Load the buttons for increase or decrease the value.
        /// </summary>
        private void LoadChangeButtons ()
        {
            this._increase_btn = new Rectangle((int)this._position.X + this._width - 34 + (int)this._parent_position.X, (int)this._position.Y + 0 + (int)this._parent_position.Y, 34, this._height);
            this._decrease_btn = new Rectangle((int)this._position.X + 0 + (int)this._parent_position.X, (int)this._position.Y + 0 + (int)this._parent_position.Y, 34, this._height);
        }

        /// <summary>
        /// UpdateGame value in the seq button in every moment (check if the value in button was change and return actual value.
        /// </summary>
        /// <returns>value in the button</returns>
        public int Update()
        {
            if (smooth)
            {
                IncreaseEvent();
                DecreaseEvent();
            }
            else
            {
                IncreaseEvent(this._difference);
                DecreaseEvent(this._difference);
                this._last_state = Mouse.GetState();
            }
            return this.value;
        }

        /// <summary>
        /// Event for incrementation the value in the seq button
        /// </summary>
        private void IncreaseEvent ()
        {
            if (this._increase_btn.Contains(this._game.mouse.Position) && this._game.mouse.LeftButton == ButtonState.Pressed)
                this.value = ChangeSeqBtnValue(this._difference);
        }
        private void IncreaseEvent (int diff)
        {
            if (this._increase_btn.Contains(this._game.mouse.Position)
                && this._game.mouse.LeftButton == ButtonState.Released
                && this._last_state.LeftButton == ButtonState.Pressed)
                this.value = ChangeSeqBtnValue(diff);
        }

        /// <summary>
        /// Event for decrementation the value in the seq button.
        /// </summary>
        private void DecreaseEvent()
        {
            if (this._decrease_btn.Contains(this._game.mouse.Position) && this._game.mouse.LeftButton == ButtonState.Pressed)
                this.value = ChangeSeqBtnValue(this._difference, true);
        }
        private void DecreaseEvent(int diff)
        {
            if (this._decrease_btn.Contains(this._game.mouse.Position)
                && this._game.mouse.LeftButton == ButtonState.Released
                && this._last_state.LeftButton == ButtonState.Pressed)
                this.value  = ChangeSeqBtnValue(diff, true);
        }

        /// <summary>
        /// Set new value into the sequential button and check whether or not it exceeded the min or max border value.
        /// If it is repeatable seq button, it set value from max to min value and conversely.
        /// </summary>
        /// <param name="difference">one step difference between values</param>
        /// <param name="substraction">optional argument if the value is ti be added or substracted, default is addition</param>
        /// <returns>new sequential button value</returns>
        private int ChangeSeqBtnValue(int difference, bool substraction = false)
        {
            //new value calculation
            int new_value;
            new_value = this.value + difference;
            if (substraction)
                new_value = this.value - difference;
            // new value conditions
            if (new_value <= this._max_value && new_value >= this._min_value) return new_value;
            else if (new_value > this._max_value && this._repeatable) return this._min_value;
            else if (new_value < this._min_value && this._repeatable) return this._max_value;
            else return this.value;
            
        }

        /// <summary>
        /// TexDraw a Squential Button.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, new Rectangle((int)this._position.X, (int)this._position.Y, this._width, _height), Color.White);
            spriteBatch.DrawString(this._font, "-", new Vector2(this._position.X +  11, this._position.Y - 10), Color.DarkGray, 0, Vector2.Zero, 3f, SpriteEffects.None, 0);
            spriteBatch.DrawString(this._font, this.value.ToString(), new Vector2(this._position.X + this._width/2 - 17, this._position.Y + 15), Color.Black, 0, Vector2.Zero, 1.4f, SpriteEffects.None, 0);
            spriteBatch.DrawString(this._font, this._title.ToUpper(), new Vector2(this._position.X + 43, this._position.Y + 3), Color.Black, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            spriteBatch.DrawString(this._font, "+", new Vector2(this._position.X + this._width - 27, this._position.Y - 5), Color.Gray, 0, Vector2.Zero, 3f, SpriteEffects.None, 0);
        }
        /// <summary>
        /// TexDraw a Sequential button depending on an enabled or disabled state.
        /// </summary>
        /// <param name="spriteBatch">spritebatch</param>
        /// <param name="enabled">true, if it enabled</param>
        public void Draw(SpriteBatch spriteBatch, bool enabled)
        {
            if (enabled)
                Draw(spriteBatch);
            else
            {
                spriteBatch.Draw(this._texture, new Rectangle((int)this._position.X, (int)this._position.Y, this._width, _height), Color.White);
                spriteBatch.DrawString(this._font, this.value.ToString(), new Vector2(this._position.X + this._width / 2 - 17, this._position.Y + 15), Color.DarkGray, 0, Vector2.Zero, 1.4f, SpriteEffects.None, 0);
                spriteBatch.DrawString(this._font, this._title.ToUpper(), new Vector2(this._position.X + 43, this._position.Y + 3), Color.Black, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            }
        }
    }
}
