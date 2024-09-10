using ATC_Game.Drawing;
using ATC_Game.GameObjects;
using ATC_Game.GameObjects.AirplaneFeatures;
using ATC_Game.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

// TODO:
// přide body vstupů, ke kterým to letí
// generování dsestinací udělej tak aby přilétaly ze stran ze kterých to dává smysl (pokud bude letiště reálné)
// predelej na ground speed at to neukazuje nereálné čísla
// tlačítka na posouvání stripeama - možná
// pri ceste na lwp se na konci nezmeni heading v control panelu
// pokud se letisti zmeni rwy_in_use, vem vsechny letadla co tam pristavaji a jednou funkci jim tu land_runway zmen z tridy toho letiste
// dodelej event na land_runway pro kliknuti mysi... normalne ke zobrazeni info, mas pripravený clickSquare
// letadlo vyletí z heading autopilota a controlery se nastaví někdy na aktuální údaje, ale přitom letadlo třeba zrychlovalo na vyšší rychlost
// doddelej track drawer
// airplane info panel - arrow icon mezi destinacemi
// třípísmené kódy letitšť dodělej
// at se do airplane ghosts, arrival departure airplanes v letistich neco hodí hned od začátku hry
// pokud letadlo odletí z letiště oddělej ho z airplaneghost a změn stats letiště
// vykresli do mapy k runwayim jejich názvy
// change v airport info panelu at funguje
// on ground letadla - dodělej, ať to funguje
// zkontroluj přepínání rwy in use v airport info panelu az budes mit letiste co má víc rwys


namespace ATC_Game
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public List<Airplane> airplanes;
        public List<InfoStripe> infostripes;
        public AirplaneLogic airplane_logic;
        public MapGenerator map_generator;
        public ControlPanel control_panel;
        public ObjectInfoPanel info_panel;

        private RenderTarget2D _game_render_target, _strips_render_target, _control_render_target, _object_info_target;
        private Rectangle _game_area, _plane_stripes_area, _control_area, _info_area;
        private Vector2 _plane_stripes_offset; // displayd part of scrollable panel
        private int stripes_block_height; // total height of content in plane strips panel

        // COLORS
        public Color _bg_color = Color.Snow;

        // mouse state
        public MouseState mouse;
        private ButtonState _previous_state;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this._graphics.PreferredBackBufferHeight = 1000;
            this._graphics.PreferredBackBufferWidth = 1350;
            this._graphics.ApplyChanges();
            this._previous_state = ButtonState.Released;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // game area
            this._game_render_target = new RenderTarget2D(GraphicsDevice, 900, 900);
            this._game_area = new Rectangle(400, 50, 900, 900);
            this.map_generator = new MapGenerator(this, Maps.Prague);
            // plane stripes area
            this._strips_render_target = new RenderTarget2D(GraphicsDevice, 400, 450);
            this._plane_stripes_area = new Rectangle(0, 50, 400, 450);
            this._plane_stripes_offset = Vector2.Zero;
            this.stripes_block_height = (Config.max_plane_count + 1) * (Config.stripe_height + Config.stripe_gap) + 10;
            // plane control area 
            this._control_render_target = new RenderTarget2D(GraphicsDevice, 1400, 50);
            this.control_panel = new ControlPanel(this);
            this._control_area = new Rectangle(0, 0, 1400, 50);
            // object info area
            this._object_info_target = new RenderTarget2D(GraphicsDevice, 400, 500);
            this.info_panel = new ObjectInfoPanel(this, 400, 500);
            this._info_area = new Rectangle(0, 450, 400, 500);

            this.airplanes = new List<Airplane>();
            this.infostripes = new List<InfoStripe>();
            this.airplane_logic = new AirplaneLogic(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime game_time)
        {
            this.airplane_logic.UpdateAirplanes(game_time);
            UpdateScrollablePanel();
            UpdateGameArea(game_time);
            UpdateControlPanel(game_time);
            UpdateInfoPanel(game_time);
            base.Update(game_time);
        }

        /// <summary>
        /// Update displayed position of plane stripe panel
        /// </summary>
        private void UpdateScrollablePanel ()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                this._plane_stripes_offset.Y -= 5;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                this._plane_stripes_offset.Y += 5;

            int disp_height = this.stripes_block_height - this._plane_stripes_area.Height;
            if (disp_height < 0)
                disp_height = 0;

            this._plane_stripes_offset.Y = Math.Clamp(this._plane_stripes_offset.Y, 0, disp_height);
        }

        /// <summary>
        /// Update events states in the game panel.
        /// </summary>
        private void UpdateGameArea (GameTime game_time)
        {
            UpdateAirplaneActiveState();
            UpdateAirports(game_time);
            UpdateWaypoints();
        }

        /// <summary>
        /// Update airplane active/deactive state.
        /// </summary>
        private void UpdateAirplaneActiveState ()
        {
            this.mouse = Mouse.GetState();
            for (int i = 0; i < this.airplanes.Count; i++)
            //foreach (Airplane plane in this.airplanes)
            {
                if ((this.airplanes[i].GetAirplaneSquare().Contains(this.mouse.Position) 
                    || this.infostripes[i].GetStripeSquare(i).Contains(this.mouse.Position))
                    && this.mouse.LeftButton == ButtonState.Pressed && this._previous_state == ButtonState.Released)
                {
                    if (!this.airplanes[i].is_active)
                    {
                        this.airplane_logic.DeactivateAllPlanes();
                        this.airplanes[i].Activate();
                    }
                    else
                        this.airplanes[i].Deactivate();
                }
            }

            if (this.airplane_logic.AreAllPlanesDeactive())
            {
                this.map_generator.DeactiveAllWaypoints();
                this.map_generator.DeactiveAllLandpoints();
            }
            this._previous_state = this.mouse.LeftButton;
        }

        /// <summary>
        /// Update airports animations in the game area.
        /// </summary>
        private void UpdateAirports (GameTime game_time)
        {
            foreach (Airport airport in this.map_generator.airports)
                airport.Update(game_time);
        }

        /// <summary>
        /// Update waypoint and landing point events and animations in the game area.
        /// </summary>
        private void UpdateWaypoints()
        {
            // all_waypoints
            foreach (Waypoint wp in this.map_generator.all_waypoints)
                wp.UpdateState();
            // landing points
            foreach (LandingWaypoint lwp in this.map_generator.all_landpoints)
                lwp.UpdateState();
        }

        /// <summary>
        /// Update event states in the control panel
        /// </summary>
        private void UpdateControlPanel(GameTime game_time)
        {
            this.control_panel.Update(game_time);
        }

        /// <summary>
        /// Update events in the object info panel.
        /// </summary>
        /// <param name="game_time"></param>
        private void UpdateInfoPanel (GameTime game_time)
        {
            this.info_panel.Update(game_time);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(this._bg_color);

            GraphicsDevice.SetRenderTarget(this._game_render_target);
            GraphicsDevice.Clear(this._bg_color);
            DrawGameArea();

            GraphicsDevice.SetRenderTarget(this._strips_render_target);
            GraphicsDevice.Clear(this._bg_color);
            DrawPlaneStripsContent();

            GraphicsDevice.SetRenderTarget(this._control_render_target);
            GraphicsDevice.Clear(this._bg_color);
            DrawControlArea();

            GraphicsDevice.SetRenderTarget(this._object_info_target);
            GraphicsDevice.Clear(this._bg_color);
            DrawInfoPanel();

            GraphicsDevice.SetRenderTarget(null);
            DrawMainLayout();

            base.Draw(gameTime);
        }

        /// <summary>
        /// TexDraw main layout of a game.
        /// </summary>
        private void DrawMainLayout()
        {
            this._spriteBatch.Begin();
            this._spriteBatch.Draw(this._game_render_target, this._game_area, this._bg_color);
            Rectangle scroll_offset = new Rectangle(0, (int)this._plane_stripes_offset.Y, this._plane_stripes_area.Width, this._plane_stripes_area.Height);
            this._spriteBatch.Draw(this._strips_render_target, this._plane_stripes_area, scroll_offset, this._bg_color);
            this._spriteBatch.Draw(this._control_render_target, this._control_area, this._bg_color);
            this._spriteBatch.Draw(this._object_info_target, this._info_area, this._bg_color);
            this._spriteBatch.End();
        }

        /// <summary>
        /// TexDraw game area content.
        /// </summary>
        private void DrawGameArea()
        {
            this._spriteBatch.Begin();
            this.map_generator.Draw(this._spriteBatch); // draw a game map
            foreach (Airplane airplane in this.airplanes)
                airplane.Draw(this._spriteBatch);
            foreach (ArrivalAlert alert in this.airplane_logic.arrival_alerts)
                alert.DrawArrivalAlert(this._spriteBatch);
            this._spriteBatch.End();
        }

        /// <summary>
        /// TexDraw plane strips area content.
        /// </summary>
        private void DrawPlaneStripsContent()
        {
            this._spriteBatch.Begin();
            for(int i = 0; i < this.infostripes.Count; i++)
                this.infostripes[i].Draw(this._spriteBatch, i);
            this._spriteBatch.End();
        }

        /// <summary>
        /// TexDraw control panel area content.
        /// </summary>
        private void DrawControlArea()
        {
            this._spriteBatch.Begin();
            this.control_panel.Draw(this._spriteBatch);
            this._spriteBatch.End();
        }

        /// <summary>
        /// Draw textures of the object info panel.
        /// </summary>
        private void DrawInfoPanel()
        {
            this._spriteBatch.Begin();
            this.info_panel.Draw(this._spriteBatch);
            this._spriteBatch.End();
        }

        /// <summary>
        /// Get Vector2 of screen height and screen width.
        /// </summary>
        /// <returns>screen size</returns>
        public Vector2 GetScreenSize()
        {
            return new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// Get Vector2 of game area height and game area width.
        /// </summary>
        /// <returns>game area size</returns>
        public Vector2 GetGameAreaSize()
        {
            return new Vector2(this._game_area.Width, this._game_area.Height);
        }

        /// <summary>
        /// Get Vector2 of plane strips area height and plane strips area width.
        /// </summary>
        /// <returns>plane strips area size</returns>
        public Vector2 GetPlaneStripsArea()
        {
            return new Vector2(this._plane_stripes_area.Width, this._plane_stripes_area.Height);
        }
    }
}
