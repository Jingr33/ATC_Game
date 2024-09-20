using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ATC_Game.InfoPanelContent
{
    /// <summary>
    /// Create and draw an active airport content of the obejct info panel.
    /// </summary>
    public class AirportInfoPanelContent
    {
        private Game1 _game;
        private ObjectInfoPanel _parent_panel;
        private Vector2 _topleft;
        private Airport _airport;

        private SpriteFont _font;
        private int _left_pad;
        private int _top_pad;
        private int _left_pad_col2;
        private int _top_pad_row1;
        private int _top_pad_row2;
        private int _top_pad_row3;
        private int _top_pad_row4;
        private float _text_big;
        private float _text_normal;
        private int _sm_btn_width;
        private int _sm_btn_height;
        private int _lg_btn_width;
        private int _lg_btn_height;
        private int stripe_width = 60;
        private int stripe_height = 30;
        private List<Rectangle> _rwy_info_rects;
        private Rectangle _air_traf_rect;
        private Texture2D _airplane_icon;
        private Texture2D _rwy_btn_tex;
        private Texture2D _arr_btn_tex;
        private Texture2D _dep_btn_tex;
        private Texture2D _arr_dep_btn_tex;
        private Texture2D _arr_rwy_btn_tex;
        private Texture2D _dep_rwy_btn_tex;
        private Texture2D _traf_btn_tex;
        private Texture2D _active_traf_btn_tex;
        private Texture2D _onground_stripe;
        private bool _is_air_traf_active;

        private ButtonState _left_last_state; // left mouse button last state
        private ButtonState _right_last_state; // right mouse button last state

        public AirportInfoPanelContent(Game1 game, ObjectInfoPanel parent_panel, Vector2 topleft)
        {
            _game = game;
            _parent_panel = parent_panel;
            _topleft = topleft;

            this._font = game.Content.Load<SpriteFont>("font");
            this._left_pad = 30;
            this._top_pad = 25;
            this._left_pad_col2 = this._left_pad + 100;
            this._top_pad_row1 = 5;
            this._top_pad_row2 = this._top_pad_row1 + 30;
            this._top_pad_row3 = this._top_pad_row2 + 30;
            this._top_pad_row4 = this._top_pad_row3 + 30;
            this._text_big = 1.1f;
            this._text_normal = 1.0f;
            this._sm_btn_width = 35;
            this._sm_btn_height = 25;
            this._lg_btn_width = 120;
            this._lg_btn_height = 30;
            this._airplane_icon = LoadAirplaneIconTex();
            this._rwy_btn_tex = CreateButtonTexture(this._game.GraphicsDevice, this._sm_btn_width, this._sm_btn_height, Config.border_gray);
            this._arr_btn_tex = CreateButtonTexture(this._game.GraphicsDevice, this._sm_btn_width, this._sm_btn_height, Config.arr_btn_color);
            this._dep_btn_tex = CreateButtonTexture(this._game.GraphicsDevice, this._sm_btn_width, this._sm_btn_height, Config.dep_btn_color);
            this._arr_dep_btn_tex = CreateDoubleButtonTexture(this._game.GraphicsDevice, this._sm_btn_width, this._sm_btn_height, Config.arr_btn_color, Config.dep_btn_color);
            this._arr_rwy_btn_tex = CreateButtonTexture(this._game.GraphicsDevice, this._sm_btn_width, this._sm_btn_height, Config.arr_btn_color);
            this._dep_rwy_btn_tex = CreateButtonTexture(this._game.GraphicsDevice, this._sm_btn_width, this._sm_btn_height, Config.dep_btn_color);
            this._traf_btn_tex = CreateButtonTexture(this._game.GraphicsDevice, this._lg_btn_width, this._lg_btn_height, Config.border_gray);
            this._active_traf_btn_tex = CreateButtonTexture(this._game.GraphicsDevice, this._lg_btn_width, this._lg_btn_height, Config.btn_active);
            this._is_air_traf_active = false;

            this._left_last_state = ButtonState.Released;
            this._right_last_state = ButtonState.Released;
        }

        /// <summary>
        /// Load an airplane texture icon.
        /// </summary>
        /// <returns></returns>
        private Texture2D LoadAirplaneIconTex ()
        {
            return _game.Content.Load<Texture2D>("plane");
        }

        /// <summary>
        /// Load an one color runway button box.
        /// </summary>
        /// <param name="graphics_device">graphics device</param>
        /// <param name="width">width of the button</param>
        /// <param name="height">height of the button</param>
        /// <param name="color">background color of the texture</param>
        /// <returns></returns>
        private Texture2D CreateButtonTexture (GraphicsDevice graphics_device, int width, int height, Color color)
        {
            Texture2D button = new Texture2D(graphics_device, width, height);
            Color[] color_data = new Color[width * height];
            for (int i = 0; i < width * height; i++)
                color_data[i] = color;
            button.SetData(color_data);
            return button;
        }

        /// <summary>
        /// Load a two color rwy button box.
        /// </summary>
        /// <param name="graphics_device">graphics device</param>
        /// <param name="width">width of the button</param>
        /// <param name="height">height of the button</param>
        /// <param name="color1">background color of the first part of the texture</param>
        /// <param name="color2">background color of the second part of the texture</param>
        /// <returns></returns>
        private Texture2D CreateDoubleButtonTexture(GraphicsDevice graphics_device, int width, int height, Color color1, Color color2)
        {
            Texture2D button = new Texture2D(graphics_device, width, height);
            Color[] color_data = new Color[width * height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (i <= width / 2)
                        color_data[i + j * width] = color1;
                    else
                        color_data[i + j * width] = color2;
            button.SetData(color_data);
            return button;
        }

        /// <summary>
        /// UpdateGame content of the airport info panel.
        /// </summary>
        /// <param name="airport"></param>
        public void UpdateContent (Airport airport)
        {
            this._airport = airport;
            this._rwy_info_rects = GetRwyButtonSquares(this._sm_btn_width, this._sm_btn_height);
            this._air_traf_rect = GetFlightsToAPSquare(this._lg_btn_width, this._lg_btn_height);
            FlightsToAPDrawerSwitcher();
            RwysInUseSwitcher();
            this._airport.flights_track_drawer.Update();

            this._left_last_state = this._game.mouse.LeftButton;
            this._right_last_state = this._game.mouse.RightButton;
        }

        /// <summary>
        /// Get a list of click event boxes for runways info rectangles.
        /// </summary>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">heigth of the rectangle</param>
        /// <returns>list of the rectangles for click event</returns>
        private List<Rectangle> GetRwyButtonSquares(int width, int height)
        {
            List<Rectangle> buttons = new List<Rectangle>();
            for (int i = 0; i < this._airport.runways.Count; i++)
            {
                Rectangle rwy_btn = new Rectangle((int)this._topleft.X + this._left_pad + i * (width + 5), (int)this._topleft.Y + 80 + this._top_pad + 450 + this._top_pad, width, height);
                buttons.Add(rwy_btn);
            }
            return buttons;
        }

        /// <summary>
        /// Return a rectangle object of the button for display a traffic in the air from/to this airport.
        /// </summary>
        private Rectangle GetFlightsToAPSquare (int width, int height)
        {
            return new Rectangle((int)this._topleft.X + this._left_pad, (int)this._topleft.Y + this._top_pad + this._top_pad_row4 + 100 + 450, width, height);
        }

        private List<Rectangle> GetOnGroundAPSquare (int width, int height)
        {
            List<Rectangle> on_grounds = new List<Rectangle>();
            for (int i = 0; i < this._airport.airplane_ghosts.Count; i++)
            {
                Rectangle ghost_plane = new Rectangle((int)this._topleft.X + this._left_pad + i * (width + 3), (int)this._topleft.Y + this._top_pad + 240 + 450, width, height);
                on_grounds.Add(ghost_plane);
            }
            return on_grounds;
        }
        /// <summary>
        /// Switch the activity of the FllightToAPDrawer to the oposite value if the button is clicked.
        /// </summary>
        private void FlightsToAPDrawerSwitcher ()
        {
            if (this._air_traf_rect.Contains(this._game.mouse.Position)
                && this._game.mouse.LeftButton == ButtonState.Released && this._left_last_state == ButtonState.Pressed)
            {
                this._airport.flights_track_drawer.SwitchActivity();
                this._is_air_traf_active = General.Switcher(this._is_air_traf_active);
            }
        }

        /// <summary>
        /// Check if one of the rwy boxes was click. If yes, add or remove this rwy from arrival or departure runways in use list.
        /// (depending on the left (arrival) of right (departure) button click)
        /// </summary>
        private void RwysInUseSwitcher()
        {
            for (int i = 0; i < this._rwy_info_rects.Count; i++)
            {
                // to arrival rwys in use list
                if (this._rwy_info_rects[i].Contains(this._game.mouse.Position)
                    && this._game.mouse.LeftButton == ButtonState.Released
                    && this._left_last_state == ButtonState.Pressed)
                    this._airport.UpdateInUseRwysLists(this._airport.runways[i], false);

                // to departure rwys in use list
                if (this._rwy_info_rects[i].Contains(this._game.mouse.Position)
                    && this._game.mouse.RightButton == ButtonState.Released
                    && this._right_last_state == ButtonState.Pressed)
                    this._airport.UpdateInUseRwysLists(this._airport.runways[i], true);

            }
        }

        /// <summary>
        /// Deactivate Flights to airport drawer.
        /// </summary>
        public void DeactivateFlightsToAPDrawer ()
        {
            this._is_air_traf_active = false;
            if (this._airport != null)
                this._airport.flights_track_drawer.is_active = false;
        }

        /// <summary>
        /// Draw an info airport panel content.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void Draw (SpriteBatch sprite_batch)
        {
            Vector2 topleft = new Vector2(this._topleft.X + this._left_pad, this._topleft.Y + this._top_pad);
            DrawAirportTraffic(sprite_batch, topleft);
            topleft.Y += 80;
            DrawAirportRwys(sprite_batch, topleft);
            topleft.Y += 115;
            DrawTrafficInAir(sprite_batch, topleft);
            topleft.Y += 45;
            DrawOnGroud(sprite_batch, topleft);
        }

        /// <summary>
        /// Draw a traffic from/to airport block.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void DrawAirportTraffic (SpriteBatch sprite_batch, Vector2 topleft)
        {
            sprite_batch.DrawString(this._font, "arriving:".ToUpper(), new Vector2(topleft.X, topleft.Y + this._top_pad_row1), Config.text_black, 0, Vector2.Zero, this._text_big, SpriteEffects.None, 0);
            sprite_batch.DrawString(this._font, "departing:".ToUpper(), new Vector2(topleft.X, topleft.Y + this._top_pad_row2), Config.text_black, 0, Vector2.Zero, this._text_big, SpriteEffects.None, 0);

            for (int i = 0; i < this._airport.arrival_airplanes.Count; i++)
                sprite_batch.Draw(this._airplane_icon, new Vector2(topleft.X + i * (this._airplane_icon.Width + 5) + this._left_pad_col2, topleft.Y + this._top_pad_row1 + 8), null, Config.bg_color, -0.7f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            for (int j = 0; j < this._airport.departure_airplanes.Count; j++) 
                sprite_batch.Draw(this._airplane_icon, new Vector2(topleft.X + j * (this._airplane_icon.Width + 5) + this._left_pad_col2, topleft.Y + this._top_pad_row2 + 8), null, Config.bg_color, -0.7f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a runway info.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void DrawAirportRwys (SpriteBatch sprite_batch, Vector2 topleft)
        {
            sprite_batch.DrawString(this._font, "Runways:", new Vector2(topleft.X, topleft.Y), Config.text_black, 0, Vector2.Zero, this._text_big, SpriteEffects.None, 0);
            for (int i = 0; i < this._airport.runways.Count; i++)
            {
                Texture2D texture = this._rwy_btn_tex;
                if (this._airport.in_use_arr.Contains(this._airport.runways[i]) && this._airport.in_use_dep.Contains(this._airport.runways[i]))
                    texture = this._arr_dep_btn_tex;
                else if (this._airport.in_use_arr.Contains(this._airport.runways[i]))
                    texture = this._arr_btn_tex;
                else if (this._airport.in_use_dep.Contains(this._airport.runways[i]))
                    texture = this._dep_btn_tex;

                Vector2 position = new Vector2(this._rwy_info_rects[i].X, this._rwy_info_rects[i].Y - 450 - 30);
                DrawRunwayInfo(sprite_batch, i, position, texture);
            }

            sprite_batch.DrawString(this._font, "Departures", new Vector2(topleft.X, topleft.Y + this._top_pad_row3), Config.text_black, 0, Vector2.Zero, this._text_normal, SpriteEffects.None, 0);
            for (int j = 0; j < this._airport.in_use_dep.Count; j++)
                sprite_batch.DrawString(this._font, this._airport.in_use_dep[j].number.ToString().ToUpper(), new Vector2(topleft.X + 95 + j * 30, topleft.Y + this._top_pad_row3), Config.dep_rwy_color, 0, Vector2.Zero, this._text_big, SpriteEffects.None, 0);
            sprite_batch.DrawString(this._font, "Arrivals", new Vector2(topleft.X, topleft.Y + this._top_pad_row4), Config.text_black, 0, Vector2.Zero, this._text_normal, SpriteEffects.None, 0);
            for (int k = 0; k < this._airport.in_use_arr.Count; k++)
                sprite_batch.DrawString(this._font, this._airport.in_use_arr[k].number.ToString().ToUpper(), new Vector2(topleft.X + 95 + k * 30, topleft.Y + this._top_pad_row4), Config.arr_rwy_color, 0, Vector2.Zero, this._text_big, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw one runway info button.
        /// </summary>
        /// <param name="sprite_batch"></param>
        /// <param name="order">order of the runway box in the row</param>
        /// <param name="text_color">color of the text</param>
        /// <param name="position">top left order of the box</param>
        public void DrawRunwayInfo (SpriteBatch sprite_batch, int order, Vector2 position, Texture2D texture)
        {
            Vector2 coord = new Vector2(position.X, position.Y + this._top_pad_row2 - 5);
            sprite_batch.Draw(texture, coord, Config.bg_color);
            sprite_batch.DrawString(this._font, this._airport.runways[order].number.ToString().ToUpper(), new Vector2(coord.X + 5, coord.Y + 5), Config.text_black);
        }

        /// <summary>
        /// Draw a traffic in the air button click switcher.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        /// <param name="position">top left touch_down_position of the block</param>
        public void DrawTrafficInAir (SpriteBatch sprite_batch, Vector2 position)
        {
            position.Y += 5;
            if (!this._is_air_traf_active)
                sprite_batch.Draw(this._traf_btn_tex, position, Config.bg_color);
            else
                sprite_batch.Draw(this._active_traf_btn_tex, position, Config.bg_color);
            sprite_batch.DrawString(this._font, "Traffic in the air", new Vector2(position.X + 5, position.Y + 7), Config.text_black);
        }

        /// <summary>
        /// Draw an onground traffic of the airport.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void DrawOnGroud (SpriteBatch sprite_batch, Vector2 topleft)
        {
            sprite_batch.DrawString(this._font, "On Ground:", new Vector2(topleft.X, topleft.Y), Config.text_black);


        }
    }
}
