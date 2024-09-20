using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Microsoft.Xna.Framework.Media;

namespace ATC_Game
{
    /// <summary>
    /// Panel with all optins of the game.
    /// </summary>

    public class HUDPanel
    {
        private Game1 _game;
        private SpriteFont _font;
        private Texture2D _time_icon;
        private Texture2D _coin_icon;
        private Texture2D _arriving_plane_icon;
        private Texture2D _departing_plane_icon;
        private Rectangle _menu_rect;
        private Texture2D _menu_icon;

        private Vector2 topleft;
        private int _top_pad;
        private int _left_pad;
        private float text_scale;
        private int btn_size;

        private ButtonState _last_state;
        
        public HUDPanel(Game1 game)
        {
            this._game = game;
            this._font = game.Content.Load<SpriteFont>("font");
            this.topleft = new Vector2(0, 0);
            this._top_pad = 15;
            this._left_pad = 5;
            this.text_scale = 1.2f;
            this.btn_size = 30;
            this._last_state = ButtonState.Released;
            LoadClickEventSquares();
            LoadIcons();
        }

        /// <summary>
        /// Load click event button rectangles.
        /// </summary>
        private void LoadClickEventSquares ()
        {
            this._menu_rect = new Rectangle((int)this.topleft.X + this._left_pad + 5, (int)this.topleft.Y + this._top_pad - 8, this.btn_size, this.btn_size);
        }

        /// <summary>
        /// Load all icons in the panel.
        /// </summary>
        private void LoadIcons ()
        {
            this._time_icon = this._game.Content.Load<Texture2D>("time_icon");
            this._coin_icon = this._game.Content.Load<Texture2D>("coin_icon");
            this._arriving_plane_icon = this._game.Content.Load<Texture2D>("arriving_plane_icon");
            this._departing_plane_icon = this._game.Content.Load<Texture2D>("departing_plane_icon");
            this._menu_icon = this._game.Content.Load<Texture2D>("menu_icon");
        }

        /// <summary>
        /// UpdateGame menu panel events.
        /// </summary>
        public void Update()
        {
            OnMenuClick();
        }

        /// <summary>
        /// Switch between game and menu state if the menu button is pressed.
        /// </summary>
        private void OnMenuClick ()
        {
            if (this._menu_rect.Contains(this._game.mouse.Position)
                && this._game.mouse.LeftButton == ButtonState.Released && this._last_state == ButtonState.Pressed)
            {
                this._game.game_state = General.Switcher(this._game.game_state, GameState.Game, GameState.Menu);
            }
            this._last_state = this._game.mouse.LeftButton;
        }

        /// <summary>
        /// Draw a menu panel content.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        public void Draw(SpriteBatch sprite_batch)
        {
            DrawSettings(sprite_batch);
            DrawGameStats(sprite_batch);
        }

        /// <summary>
        /// Draw icons for game settings.
        /// </summary>
        public void DrawSettings (SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(this._menu_icon, new Vector2(this._menu_rect.X, this._menu_rect.Y), Config.bg_color);
        }

        /// <summary>
        /// Draw game stats.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void DrawGameStats (SpriteBatch sprite_batch)
        {
            string time = this._game.game_stats.GetTimeAsString();
            string points = this._game.game_stats.GetPoints();
            string landed_planes = this._game.game_stats.GetLandedAircraftNum();
            string departed_planes = this._game.game_stats.GetDepartedAircraftNum();

            sprite_batch.Draw(this._time_icon, new Vector2(this.topleft.X + this._left_pad + 135, this.topleft.Y + this._top_pad), Config.bg_color);
            sprite_batch.DrawString(this._font, time, new Vector2(this.topleft.X + this._left_pad + 160, this.topleft.Y + this._top_pad), Config.game_stats_color, 0, Vector2.Zero, this.text_scale, SpriteEffects.None, 1);
            sprite_batch.Draw(this._coin_icon, new Vector2(this.topleft.X + this._left_pad + 220, this.topleft.Y + this._top_pad), Config.bg_color);
            sprite_batch.DrawString(this._font, points, new Vector2(this.topleft.X + this._left_pad + 245, this.topleft.Y + this._top_pad), Config.game_stats_color, 0, Vector2.Zero, this.text_scale, SpriteEffects.None, 1);
            sprite_batch.Draw(this._arriving_plane_icon, new Vector2(this.topleft.X + this._left_pad + 285, this.topleft.Y + this._top_pad), Config.bg_color);
            sprite_batch.DrawString(this._font, landed_planes, new Vector2(this.topleft.X + this._left_pad + 310, this.topleft.Y + this._top_pad), Config.game_stats_color, 0, Vector2.Zero, this.text_scale, SpriteEffects.None, 1);
            sprite_batch.Draw(this._departing_plane_icon, new Vector2(this.topleft.X + this._left_pad + 345, this.topleft.Y + this._top_pad), Config.bg_color);
            sprite_batch.DrawString(this._font, departed_planes, new Vector2(this.topleft.X + this._left_pad + 370, this.topleft.Y + this._top_pad), Config.game_stats_color, 0, Vector2.Zero, this.text_scale, SpriteEffects.None, 1);
        } 
    }
}
