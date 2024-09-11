using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ATC_Game.InfoPanelContent
{
    /// <summary>
    /// Create info panel content if the airplane is choosen and active.
    /// </summary>
    public class AirplaneInfoPanelContent
    {
        private Game1 _game;
        private Airplane _airplane;
        private ObjectInfoPanel _parent_panel;
        private Vector2 _topleft;
        private int _padding;
        private Rectangle _track_schema_rect;
        private bool _track_drawer_is_active;

        private Texture2D _arrow_texture;
        private Texture2D _airplane_texture;
        private Texture2D _wp_texture;
        private Texture2D _lwp_texture;
        private Texture2D _border_line_texture;
        private SpriteFont _font;
        private Texture2D _active_track_drawer;

        private ButtonState _last_state;

        //paddings
        private int _left_pad_col1;
        private int _left_pad_col2;
        private int _left_pad_center_col;
        private int _top_pad_row1;
        private int _top_pad_row2;
        private int _top_pad_header1;
        private int _top_pad_header2;
        private int _left_pad_header1;
        private int _left_pad_header2;
        private float _header_scale;
        private float _content_scale;

        public AirplaneInfoPanelContent(Game1 game, ObjectInfoPanel parent_panel, Vector2 top_left)
        {
            this._game = game;
            this._parent_panel = parent_panel;
            this._topleft = top_left;
            this._padding = 20;
            this._arrow_texture = LoadArrowTexture();
            this._airplane_texture = LoadAirplaneTexture();
            this._wp_texture = LoadWPTexture();
            this._lwp_texture = LoadLWPTexture();
            this._border_line_texture = GetBorderLineTexture(this._game.GraphicsDevice);
            this._track_schema_rect = GetFlightTrackSchemaSquare();
            this._active_track_drawer = GetActiveTrackDrawerTex(this._game.GraphicsDevice, this._track_schema_rect);
            this._font = game.Content.Load<SpriteFont>("font");
            this._last_state = ButtonState.Released;
            this._track_drawer_is_active = false;

            this._left_pad_col1 = 50;
            this._left_pad_col2 = 240;
            this._left_pad_center_col = 140;
            this._top_pad_row1 = 15;
            this._top_pad_row2 = 55;
            this._top_pad_header1 = 0;
            this._top_pad_header2 = 40;
            this._left_pad_header1 = 40;
            this._left_pad_header2 = 230;
            this._header_scale = 0.8f;
            this._content_scale = 1;

        }

        /// <summary>
        /// Return texture of an arrow border of an airplne track schema.
        /// </summary>
        /// <returns>texture of the airplane</returns>
        private Texture2D LoadArrowTexture ()
        {
            return this._game.Content.Load<Texture2D>("arrow_info_icon");
        }

        /// <summary>
        /// Return texture of a plane of an airplne track schema.
        /// </summary>
        /// <returns>texture of the airplane</returns>
        private Texture2D LoadAirplaneTexture ()
        {
            return this._game.Content.Load<Texture2D>("plane_info_icon");
        }

        /// <summary>
        /// Return texture of a waypoint border of an airplne track schema.
        /// </summary>
        /// <returns>texture of the wp</returns>
        private Texture2D LoadWPTexture()
        {
            return this._game.Content.Load<Texture2D>("wp_info_icon");
        }

        /// <summary>
        /// Return texture of a landing waypoint border of an airplne track schema.
        /// </summary>
        /// <returns>texture of the lwp</returns>
        private Texture2D LoadLWPTexture()
        {
            return this._game.Content.Load<Texture2D>("lwp_info_icon");
        }

        /// <summary>
        /// Return a rectangle of the click event of the flight track schema.   
        /// </summary>
        /// <returns>rectangle of the click button</returns>
        private Rectangle GetFlightTrackSchemaSquare()
        {
            return new Rectangle((int)this._topleft.X + 10, (int)this._topleft.Y + 8 + 450, this._parent_panel.width - 10, this._arrow_texture.Height + 50);
        }

        /// <summary>
        /// Return an active focue border of the flight track schema.
        /// </summary>
        /// <param name="graphics_device"></param>
        /// <param name="track_schem">a rectangle of the track schema</param>
        /// <returns></returns>
        private Texture2D GetActiveTrackDrawerTex(GraphicsDevice graphics_device, Rectangle track_schem)
        {
            Texture2D rect = new Texture2D(graphics_device, track_schem.Width, track_schem.Height);
            Color[] color_data = new Color[track_schem.Width * track_schem.Height];
            for (int j = 0; j < track_schem.Height; j++)
            {
                for (int k = 0; k < track_schem.Width; k++)
                {
                    if (j < 2 || j >= track_schem.Height - 2 || k < 2 || k >= track_schem.Width - 2) // skip center of width
                        color_data[track_schem.Width * j + k] = Color.DarkMagenta;
                    else
                        color_data[track_schem.Width * j + k] = Color.Transparent;
                }
            }
            rect.SetData(color_data);
            return rect;
        }


        /// <summary>
        /// Return a texture of the gray line (border).
        /// </summary>
        /// <param name="graphics_device"></param>
        /// <returns>texture of the border bottom</returns>
        private Texture2D GetBorderLineTexture (GraphicsDevice graphics_device)
        {
            int width = this._parent_panel.width - 40;
            int height = 1;
            Texture2D border = new Texture2D(graphics_device, width, height);
            Color[] color_data = new Color[width * height];
            for (int i = 0; i < color_data.Length; i++)
                color_data[i] = Config.border_gray;
            border.SetData(color_data);
            return border;
        }

        /// <summary>
        /// Update airplane actual data.
        /// </summary>
        /// <param name="airplane">acive ariplane</param>
        public void UpdateContent(Airplane airplane)
        {
            this._airplane = airplane;
            TrackDrawerSwitcher();
            this._airplane.track_drawer.Update(this._airplane);
        }

        /// <summary>
        /// Switch activity od the flight track drawer and activity of the track schema focus.
        /// </summary>
        private void TrackDrawerSwitcher ()
        {
            if (this._track_schema_rect.Contains(this._game.mouse.Position)
                && this._game.mouse.LeftButton == ButtonState.Released && this._last_state == ButtonState.Pressed)
            {
                this._airplane.track_drawer.SwitchActivity();
                if (this._track_drawer_is_active)
                    this._track_drawer_is_active = false;
                else
                    this._track_drawer_is_active = true;
            }
            this._last_state = this._game.mouse.LeftButton;
        }

        /// <summary>
        /// Set track drawer activity to false.
        /// </summary>
        public void DeactiveTrackDrawer()
        {
            this._track_drawer_is_active = false;
            if (this._airplane != null)
                this._airplane.track_drawer.is_active = false;
        }

        /// <summary>
        /// Draw an airplane info panel.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void Draw (SpriteBatch sprite_batch)
        {
            DrawTrackSchemaBorder(sprite_batch);
            DrawTrackSchema(sprite_batch);
            DrawAirpalneInfo(sprite_batch);
        }

        /// <summary>
        /// Draw a track schema of the flight.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        private void DrawTrackSchema (SpriteBatch sprite_batch)
        {
            int count = 0;
            // draw plane icon
            DrawOneIcon(sprite_batch, this._airplane_texture, count);
            DrawIconName(sprite_batch, this._airplane.registration, count);
            // draw wp icons
            for (int i = 0; i < this._airplane.waypoints.Count; i++)
            {
                if ((i > 4 && this._airplane.landpoint != null) || (i > 5)) break; // due to flight track space overload
                count += 1;
                DrawOneIcon(sprite_batch, this._wp_texture, count);
                DrawIconName(sprite_batch, this._airplane.waypoints[count - 1].name, count);
            }
            // draw lwp icon
            if (this._airplane.landpoint != null)
            {
                count += 1;
                DrawOneIcon(sprite_batch, this._lwp_texture, count);
                DrawIconName(sprite_batch, this._airplane.landpoint.name, count);
            }
            // draw border box
            for (int i = 0; i <= count; i++)
                DrawOneIcon(sprite_batch, this._arrow_texture, i);
        }

        /// <summary>
        /// Draw one texture icon in info panel.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        /// <param name="icon">texture of the icon</param>
        /// <param name="order">order of the icon box</param>
        private void DrawOneIcon (SpriteBatch sprite_batch, Texture2D icon, int order)
        {
            sprite_batch.Draw(icon, 
                new Vector2(this._padding + this._topleft.X + order * this._arrow_texture.Width, this._padding + this._topleft.Y),
                Config.bg_color);
        }

        /// <summary>
        /// Draw a waypoint name into the one flight track box.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        /// <param name="text">text</param>
        /// <param name="order">order of the track box</param>
        private void DrawIconName (SpriteBatch sprite_batch, string text, int order)
        {
            Color color = order % 2 == 0 ? Color.DarkSlateGray : Color.DarkViolet;
            sprite_batch.DrawString(this._font, text, 
                new Vector2(this._padding + this._topleft.X + order * this._arrow_texture.Width, this._padding + this._topleft.Y + 55), 
                color, 0.37f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a track schema border of it is needed.
        /// </summary>
        /// <param name="sprite_batch"></param>
        private void DrawTrackSchemaBorder (SpriteBatch sprite_batch)
        {
            if (this._track_drawer_is_active)
                sprite_batch.Draw(this._active_track_drawer, new Vector2(this._topleft.X + 10, this._topleft.Y + 8), Config.bg_color);
        }

        /// <summary>
        /// Draw an airplane info panel part of the airplane info content panel.
        /// </summary>
        /// <param name="spritebatch">sprite abtch</param>
        private void DrawAirpalneInfo (SpriteBatch spritebatch)
        {
            Vector2 topleft = new Vector2(this._topleft.X, this._topleft.Y + 110);
            DrawDestinationInfo(spritebatch, topleft);
            topleft.Y += 80;
            DrawBorderLine(spritebatch, topleft);
            DrawFlightInfo(spritebatch, topleft);
            topleft.Y += 85;
            DrawBorderLine(spritebatch, topleft);
            DrawSectionsInfo(spritebatch, topleft);
            topleft.Y += 80;
            DrawBorderLine(spritebatch, topleft);
            DrawStateInfo(spritebatch, topleft);
        }

        /// <summary>
        /// Draw a Deatination block of the airplane info panel.
        /// </summary>
        /// <param name="spritebatch"></param>
        /// <param name="topleft">top left corner of th block </param>
        private void DrawDestinationInfo (SpriteBatch spritebatch, Vector2 topleft)
        {
            int left_column_pad = 50;
            int right_column_pad = 230;
            int top_ap_pad = this._top_pad_row1 + 25;
            int top_rwy_pad = top_ap_pad + 15;
            if (this._airplane.oper_type == OperationType.Arrival)
            {
                int transmitter1 = right_column_pad;
                right_column_pad = left_column_pad;
                left_column_pad = transmitter1;
            }
            float ap_code_scale = 1.3f;
            // header
            spritebatch.DrawString(this._font, "from".ToUpper(), new Vector2(topleft.X + left_column_pad, topleft.Y + this._top_pad_header1), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, "to".ToUpper(), new Vector2(topleft.X + right_column_pad, topleft.Y + this._top_pad_header1), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            // content
            spritebatch.DrawString(this._font, "XYA".ToUpper(), new Vector2(topleft.X + left_column_pad, topleft.Y + this._top_pad_row1), Config.text_black, 0, Vector2.Zero, ap_code_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.airport.name.ToString(), new Vector2(topleft.X + left_column_pad, topleft.Y + top_ap_pad), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.runway.number.ToString().ToUpper(), new Vector2(topleft.X + left_column_pad, topleft.Y + top_rwy_pad), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);

            spritebatch.DrawString(this._font, "XYD".ToUpper(), new Vector2(topleft.X + right_column_pad, topleft.Y + this._top_pad_row1), Config.text_black, 0, Vector2.Zero, ap_code_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.destination, new Vector2(topleft.X + right_column_pad, topleft.Y + top_ap_pad), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);

        }

        /// <summary>
        /// Draw a flight info (typ, weight category, callsign and registration).
        /// </summary>
        /// <param name="spritebatch"></param>
        /// <param name="topleft">top left corner of th block </param>
        private void DrawFlightInfo (SpriteBatch spritebatch, Vector2 topleft)
        {
            // header
            spritebatch.DrawString(this._font, "type".ToUpper(), new Vector2(topleft.X + this._left_pad_header1, topleft.Y + this._top_pad_header1), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, "weight category".ToUpper(), new Vector2(topleft.X + this._left_pad_header2, topleft.Y + this._top_pad_header1), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, "registration".ToUpper(), new Vector2(topleft.X + this._left_pad_header1, topleft.Y + this._top_pad_header2), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, "callsign".ToUpper(), new Vector2(topleft.X + this._left_pad_header2, topleft.Y + this._top_pad_header2), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            // content
            spritebatch.DrawString(this._font, this._airplane.type.ToString().ToUpper(), new Vector2(topleft.X + this._left_pad_col1, topleft.Y + this._top_pad_row1), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.weight_cat.ToString().ToUpper(), new Vector2(topleft.X + this._left_pad_col2, topleft.Y + this._top_pad_row1), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.registration.ToString().ToUpper(), new Vector2(topleft.X + this._left_pad_col1, topleft.Y + this._top_pad_row2), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.callsign.ToString().ToUpper(), new Vector2(topleft.X + this._left_pad_col2, topleft.Y + this._top_pad_row2), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a flight section info (operation tayp, flight status and flight section).
        /// </summary>
        /// <param name="spritebatch"></param>
        /// <param name="topleft">top left corner of th block </param>
        private void DrawSectionsInfo (SpriteBatch spritebatch, Vector2 topleft)
        {
            float operation_scale = 1.1f;
            int top_pad_shift = -10;
            // header
            spritebatch.DrawString(this._font, "flight section".ToUpper(), new Vector2(topleft.X + this._left_pad_header1, topleft.Y + this._top_pad_header2 + top_pad_shift), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, "schedule".ToUpper(), new Vector2(topleft.X + this._left_pad_header2, topleft.Y + this._top_pad_header2 + top_pad_shift), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            // content
            spritebatch.DrawString(this._font, this._airplane.oper_type.ToString().ToUpper(), new Vector2(topleft.X + this._left_pad_center_col, topleft.Y + this._top_pad_row1 + top_pad_shift), Config.text_black, 0, Vector2.Zero, operation_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.flight_section.ToString(), new Vector2(topleft.X + this._left_pad_col1, topleft.Y + this._top_pad_row2 + top_pad_shift), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.flight_status.ToString(), new Vector2(topleft.X + this._left_pad_col2, topleft.Y + this._top_pad_row2 + top_pad_shift), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw an aircraft actual states (altitude, heading and speed) into the aircarft info panel.
        /// </summary>
        /// <param name="spritebatch">sprite batch</param>
        /// <param name="heaght">height of the state section</param>
        /// <param name="topleft">top left corner coords of the section</param>
        private void DrawStateInfo (SpriteBatch spritebatch, Vector2 topleft)
        {
            // header
            spritebatch.DrawString(this._font, "altitude".ToUpper(), new Vector2(topleft.X + this._left_pad_header1, topleft.Y + this._top_pad_header1), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, "heading".ToUpper(), new Vector2(topleft.X + this._left_pad_header2, topleft.Y + this._top_pad_header1), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, "speed".ToUpper(), new Vector2(topleft.X + this._left_pad_header1, topleft.Y + this._top_pad_header2), Config.text_gray, 0, Vector2.Zero, this._header_scale, SpriteEffects.None, 0);
            // content
            spritebatch.DrawString(this._font, this._airplane.altitude.ToString(), new Vector2(topleft.X + this._left_pad_col1, topleft.Y + this._top_pad_row1), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.heading.ToString(), new Vector2(topleft.X + this._left_pad_col2, topleft.Y + this._top_pad_row1), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
            spritebatch.DrawString(this._font, this._airplane.speed.ToString(), new Vector2(topleft.X + this._left_pad_col1, topleft.Y + this._top_pad_row2), Config.text_black, 0, Vector2.Zero, this._content_scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a bottom border of the airplane info panel block.
        /// </summary>
        /// <param name="sprite_batch">sprite abtch</param>
        /// <param name="topleft">Top left position of the texture</param>
        private void DrawBorderLine (SpriteBatch sprite_batch, Vector2 topleft)
        {
            sprite_batch.Draw(this._border_line_texture, new Vector2(topleft.X + 25, topleft.Y - 5), Config.bg_color);
        }
    }
}
