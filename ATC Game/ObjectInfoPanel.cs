using ATC_Game.GameObjects;
using ATC_Game.InfoPanelContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATC_Game
{
    /// <summary>
    /// Object for creation of the object info panel.
    /// </summary>
    public class ObjectInfoPanel
    {
        private Game1 _game;
        private Airplane _airplane;
        private Airport _airport;
        private bool _is_airplane_active;
        private bool _is_airport_active;
        public int width;
        public int height;
        private int _padding;

        private int _tab_size;
        private int _border_thickness;
        private Texture2D _airport_texture;
        private Texture2D _airplane_texture;
        private Texture2D _border_texture;
        private Texture2D _active_tab;
        private Texture2D _deactive_tab;
        private AirplaneInfoPanelContent _airplane_content;
        private AirportInfoPanelContent _airport_content;

        private ButtonState _last_state;

        public ObjectInfoPanel(Game1 game, int width, int height)
        {
            this._game = game;
            this._airplane = null;
            this._airport = null;
            this._is_airport_active = false;
            this._is_airplane_active = false;
            this._padding = 5;
            this._tab_size = 60;
            this._border_thickness = 1;
            this.width = width - this._padding * 2;
            this.height = height - this._padding * 2;
            this._airport_texture = LoadAirportTexture();
            this._airplane_texture = LoadAirplaneTexture();
            this._border_texture = LoadTabTexture(this.width, this.height - this._tab_size, false);
            this._active_tab = LoadTabTexture(this._tab_size, this._tab_size, true);
            this._deactive_tab = LoadTabTexture(this._tab_size, this._tab_size, false);
            this._airplane_content = new AirplaneInfoPanelContent(this._game, this, new Vector2(0, this._tab_size));
            this._airport_content = new AirportInfoPanelContent(this._game, this, new Vector2(0, this._tab_size));
            this._last_state = ButtonState.Released;
        }

        /// <summary>
        /// Load an airport texture button.
        /// </summary>
        /// <returns></returns>
        private Texture2D LoadAirportTexture()
        {
            return this._game.Content.Load<Texture2D>("airport_icon");
        }

        /// <summary>
        /// Load an airplane texture button.
        /// </summary>
        /// <returns></returns>
        private Texture2D LoadAirplaneTexture()
        {
            return this._game.Content.Load<Texture2D>("plane_icon");
        }

        /// <summary>
        /// Load an active tab texture.
        /// </summary>
        /// <param name="active_texture">Is the button texture for active or deactive state.</param>
        /// <returns>texture of click button</returns>
        private Texture2D LoadTabTexture(int width, int height, bool active_texture)
        {
            Texture2D tab = new Texture2D(this._game.GraphicsDevice, width, height);
            Color[] color_data = new Color[width * height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bool vertical = j < this._border_thickness || j >= width - this._border_thickness;
                    bool horizontal = active_texture ? i < this._border_thickness : (i < this._border_thickness || i >= height - this._border_thickness);
                    if (vertical || horizontal)
                        color_data[width * i + j] = Color.Black;
                    else
                        color_data[width * i + j] = Color.Transparent;
                }
            }
            tab.SetData(color_data);
            return tab;
        }

        /// <summary>
        /// UpdateGame events in the object info panel.
        /// </summary>
        /// <param name="game_time">game _time</param>
        public void Update(GameTime game_time)
        {
            GenerateTabs();
            SetTabsActivity();
            SetPanelContent();
        }

        /// <summary>
        /// UpdateGame airport and airplane objects in the panel.
        /// </summary>
        /// <param name="game_time"></param>
        private void GenerateTabs()
        {
            Airplane plane = this._game.airplane_logic.GetActiveAirplane();
            Airport airport = this._game.map_generator.GetActiveAirport();
            // airplane tab
            if (plane != this._airplane)
                this._airplane = plane;

            // airport tab from active airport
            if (airport != null && airport != this._airport)
                this._airport = airport;
            // airport tab from airplane final destination
            else if (airport == null && plane != null)
                this._airport = plane.airport;
            // deactive airport tab
            else if (airport == null && plane == null)
                this._airport = null;
        }

        /// <summary>
        /// Set an activity (or deactivity) of the airplane and airport tabs.
        /// </summary>
        private void SetTabsActivity ()
        {
            // no tabs actualy exist
            if (this._airplane == null && this._airport == null)
            {
                this._is_airport_active = false;
                this._is_airplane_active = false;
                this._airplane_content.DeactivateTrackDrawer();
                this._airport_content.DeactivateFlightsToAPDrawer();
            }
            // airplane tab is exist
            else if (this._airplane != null && this._airport == null)
            {
                this._is_airplane_active = true;
                this._is_airport_active = false;
                this._airport_content.DeactivateFlightsToAPDrawer();
            }
            // airport tab is active
            else if (this._airport != null && this._airplane == null)
            {
                this._is_airport_active = true;
                this._is_airplane_active = false;
                this._airplane_content.DeactivateTrackDrawer();
            }
            // both exits -> click event
            else
                TabActivityClickSwitcher();
        }

        /// <summary>
        /// Switch activity of the airplane and airport tab. if you click one of them, its activity id setted to true, other to false.
        /// </summary>
        private void TabActivityClickSwitcher ()
        {
            // airplane tab
            if (GetTabClickSquare(0).Contains(this._game.mouse.Position) 
                && this._game.mouse.LeftButton == ButtonState.Released && this._last_state == ButtonState.Pressed)
            {
                this._is_airplane_active = true;
                this._is_airport_active = false;
                this._airport_content.DeactivateFlightsToAPDrawer();
            }
            // airport tab
            else if (GetTabClickSquare(1).Contains(this._game.mouse.Position)
                    && this._game.mouse.LeftButton == ButtonState.Released && this._last_state == ButtonState.Pressed)
            {
                this._is_airport_active = true;
                this._is_airplane_active = false;
                this._airplane_content.DeactivateTrackDrawer();
            }
            //last button state change
            this._last_state = this._game.mouse.LeftButton;
        }

        /// <summary>
        /// Get a click event rectangle of the tab.
        /// </summary>
        /// <param name="position">touch_down_position of the tab on a tab row</param>
        /// <returns>rectangle of the tab</returns>
        private Rectangle GetTabClickSquare (int position)
        {
            return new Rectangle(this._padding + position * (this._tab_size + 5), this._padding + 450, this._tab_size, this._tab_size);
        }

        /// <summary>
        /// Create and draw a panel content about an active airport or airpoane into objectInfoPanel.
        /// </summary>
        private void SetPanelContent()
        {
            if (this._is_airplane_active) 
            { 
                this._airplane_content.UpdateContent(this._airplane);
                this._airport_content.UpdateContent(this._airplane.airport);
            }
            else if (this._is_airport_active)
                this._airport_content.UpdateContent(this._airport);
        }

        /// <summary>
        /// Draw all textures of the object info panel.
        /// </summary>
        /// <param name="sprite_batch">sprite_batch</param>
        public void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(this._border_texture, new Vector2(this._padding, this._padding + this._tab_size - this._border_thickness), Config.bg_color);

            if (this._airplane != null)
            {
                sprite_batch.Draw(this._airplane_texture, new Vector2(this._padding, this._padding), Config.bg_color);
                DrawActivityTab(sprite_batch, this._is_airplane_active, 0);
            }

            if (this._airport != null)
            {
                sprite_batch.Draw(this._airport_texture, new Vector2(this._padding + this._tab_size + 5, this._padding), Config.bg_color);
                DrawActivityTab(sprite_batch, this._is_airport_active, 1);        
            }

            DrawActualContent(sprite_batch);
        }

        /// <summary>
        /// Draw a activity version of the tab border.
        /// </summary>
        /// <param name="sprite_batch">sprite batch</param>
        /// <param name="is_active">if the border version is active or deactive</param>
        /// <param name="position">positon of the tab in the tab row</param>
        private void DrawActivityTab (SpriteBatch sprite_batch, bool is_active, int position)
        {
            Texture2D texture = this._deactive_tab;
            if (is_active)
                texture = this._active_tab;
            sprite_batch.Draw(texture, new Vector2(this._padding + position * (this._tab_size + 5), this._padding), Config.bg_color);
        }

        /// <summary>
        /// Draw an actual info panel content.
        /// </summary>
        private void DrawActualContent (SpriteBatch sprite_batch)
        {
            if (this._is_airplane_active)
                this._airplane_content.Draw(sprite_batch);
            else if (this._is_airport_active)
                this._airport_content.Draw(sprite_batch);
        }
    }
}
