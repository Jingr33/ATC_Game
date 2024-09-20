using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ATC_Game
{
    /// <summary>
    /// Class for menu of the game.
    /// </summary>
    public class MenuPanel
    {
        private Game1 _game;
        private Rectangle _back_to_game_btn, _new_game_btn, _settings_btn, _quit_btn;
        private Texture2D _btn_texture, _btn_texture_hover;
        private SpriteFont _font;
        private bool _back_to_game_hover, _new_game_hover, _setting_hover, _quit_hover;
        private int _btn_width, _btn_height;
        private Vector2 _origin; // origin position of the textures
        private ButtonState _last_state;

        public MenuPanel(Game1 game)
        {
            this._game = game;
            this._btn_width = 500;
            this._btn_height = 75;
            LoadButtonPositions();
            LoadButtonTextures();
            this._font = game.Content.Load<SpriteFont>("font");
            this._origin = new Vector2(this._btn_width / 2, this._btn_height / 2);
            this._back_to_game_hover = false;
            this._new_game_hover = false;
            this._setting_hover = false;
            this._quit_hover = false;
            this._last_state = ButtonState.Released;
        }

        /// <summary>
        /// Load the positon and sizes of the menu buttons.
        /// </summary>
        private void LoadButtonPositions ()
        {
            int x = 1450 / 2 - this._btn_width / 2;
            int y = 300;
            this._back_to_game_btn = new Rectangle(x, y, this._btn_width, this._btn_height);
            this._new_game_btn = new Rectangle(x, y + 100, this._btn_width, this._btn_height);
            this._settings_btn = new Rectangle(x, y + 200, this._btn_width, this._btn_height);
            this._quit_btn = new Rectangle(x, y + 300, this._btn_width, this._btn_height);
        }

        /// <summary>
        /// Load the button textures
        /// </summary>
        private void LoadButtonTextures ()
        {
            this._btn_texture = CreateButtonTexture();
            this._btn_texture_hover = CreateButtonTexture(true);
        }

        /// <summary>
        /// Create the texture of one button in the menu panel.
        /// </summary>
        /// <param name="text">text in the button</param>
        /// <param name="color">color of the button</param>
        /// <param name="border">border color of the button</param>
        /// <returns>texture of the button</returns>
        private Texture2D CreateButtonTexture(bool is_hover = false)
        {
            Color bg_color = is_hover ? Config.menu_btn_hover_color : Config.menu_btn_color;
            Texture2D button = new Texture2D(this._game.GraphicsDevice, this._btn_width, this._btn_height);
            Color[] color_data = new Color[this._btn_width * this._btn_height];
            for (int i = 0; i < color_data.Length; i++)
                color_data[i] = bg_color;
            button.SetData(color_data);
            return button;
        }

        /// <summary>
        /// Update the menu panel.
        /// </summary>
        public void Update()
        {
            this._back_to_game_hover = ButtonHoverCheck(this._back_to_game_btn);
            this._new_game_hover = ButtonHoverCheck(this._new_game_btn);
            this._setting_hover = ButtonHoverCheck(this._settings_btn);
            this._quit_hover = ButtonHoverCheck(this._quit_btn);
            BackToGameEvent();
            QuitGameEvent();
            //TODO:

            this._last_state = this._game.mouse.LeftButton;
        }

        /// <summary>
        /// Check if the mouse position is in the button area.
        /// </summary>
        /// <param name="btn_rect">rectangle of the button position</param>
        /// <returns>if the button is hovered</returns>
        private bool ButtonHoverCheck (Rectangle btn_rect)
        {
            if (btn_rect.Contains(this._game.mouse.Position))
                return true;
            return false;
        }

        /// <summary>
        /// Check if the button was clicked.
        /// </summary>
        /// <param name="btn_rect">rectangle of the button position</param>
        /// <returns>if the button was clicked</returns>
        private bool ButtonClickCheck (Rectangle btn_rect)
        {
            if (btn_rect.Contains(this._game.mouse.Position)
                && this._game.mouse.LeftButton == ButtonState.Released && this._last_state == ButtonState.Pressed)
                return true;
            return false;
        }

        /// <summary>
        /// If the button back to game is clicked, the game switch to game panel.
        /// </summary>
        private void BackToGameEvent ()
        {
            if (ButtonClickCheck(this._back_to_game_btn))
                this._game.game_state = GameState.Game;
        }

        /// <summary>
        /// Quit the game.
        /// </summary>
        private void QuitGameEvent ()
        {
            if (ButtonClickCheck(this._quit_btn))
                this._game.Exit();
        }

        /// <summary>
        /// Draw the menu panel.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void Draw (SpriteBatch sprite_batch)
        {
            Texture2D back_to_game_tex = this._back_to_game_hover ? this._btn_texture_hover : this._btn_texture;
            Texture2D new_game_tex = this._new_game_hover ? this._btn_texture_hover : this._btn_texture;
            Texture2D setting_tex = this._setting_hover ? this._btn_texture_hover : this._btn_texture;
            Texture2D quit_tex = this._quit_hover ? this._btn_texture_hover : this._btn_texture;

            Vector2 btn1_pos = new Vector2(725, 300 + this._btn_height / 2);          
            sprite_batch.Draw(back_to_game_tex, btn1_pos, null, Config.bg_color, 0, this._origin, 1, SpriteEffects.None, 1);
            sprite_batch.DrawString(this._font, "Back to the game".ToUpper(), new Vector2(btn1_pos.X + 240, btn1_pos.Y + 40), Config.text_black, 0, this._origin, 1.4f, SpriteEffects.None, 1);
            Vector2 btn2_pos = new Vector2(725, 300 + this._btn_height / 2 + 100);
            sprite_batch.Draw(new_game_tex, btn2_pos, null, Config.bg_color, 0, this._origin, 1, SpriteEffects.None, 1);
            sprite_batch.DrawString(this._font, "New game".ToUpper(), new Vector2(btn2_pos.X + 290, btn2_pos.Y + 40), Config.text_black, 0, this._origin, 1.4f, SpriteEffects.None, 1);
            Vector2 btn3_pos = new Vector2(725, 300 + this._btn_height / 2 + 200);
            sprite_batch.Draw(setting_tex, btn3_pos, null, Config.bg_color, 0, this._origin, 1, SpriteEffects.None, 1);
            sprite_batch.DrawString(this._font, "Settings".ToUpper(), new Vector2(btn3_pos.X + 295, btn3_pos.Y + 40), Config.text_black, 0, this._origin, 1.4f, SpriteEffects.None, 1);
            Vector2 btn4_pos = new Vector2(725, 300 + this._btn_height / 2 + 300);
            sprite_batch.Draw(quit_tex, btn4_pos, null, Config.bg_color, 0, this._origin, 1, SpriteEffects.None, 1);
            sprite_batch.DrawString(this._font, "Quit game".ToUpper(), new Vector2(btn4_pos.X + 290, btn4_pos.Y + 40), Config.text_black, 0, this._origin, 1.4f, SpriteEffects.None, 1);
        }
    }
}
