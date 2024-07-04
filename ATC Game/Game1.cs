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
// dodělej info strips
// generování dsestinací udělej tak aby přiletaly ze stran ze kterých to dává smysl (pokud bude letiště reálné)
// predelej speed na ground speed at to neukazuje nereálné čísla
// přidat time schedule a flight status
// přidat funkce pro time section
// udělej stripes jako scrollable panel
// tlačítka na posouvání stripeama

namespace ATC_Game
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public List<Airplane> airplanes;
        public List<InfoStripe> infostripes;
        public AirplaneLogic _airplane_logic;
        public MapGenerator _map_generator;

        private RenderTarget2D _game_render_target;
        private RenderTarget2D _strips_render_target;
        private Rectangle _game_area, _plane_stripes_area;
        private Vector2 _plane_stripes_offset; // displayd part of scrollable panel
        private int stripes_block_height; // total height of content in plane strips panel

        // COLORS
        public Color _bg_color = Color.Snow;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this._graphics.PreferredBackBufferHeight = 900;
            this._graphics.PreferredBackBufferWidth = 1400;
            this._graphics.ApplyChanges();
            IsMouseVisible = true;
            Console.Write("START");
        }

        protected override void Initialize()
        {
            // game area
            this._game_render_target = new RenderTarget2D(GraphicsDevice, 900, 900);
            this._game_area = new Rectangle(400, 0, 900, 900);
            this._map_generator = new MapGenerator(this, Maps.Prague);
            // plane stripes area
            this._strips_render_target = new RenderTarget2D(GraphicsDevice, 400, 450);
            this._plane_stripes_area = new Rectangle(0, 0, 400, 450);
            this._plane_stripes_offset = Vector2.Zero;
            this.stripes_block_height = (Config.max_plane_count + 1) * (Config.stripe_height + Config.stripe_gap) + 10;

            this.airplanes = new List<Airplane>();
            this.infostripes = new List<InfoStripe>();
            this._airplane_logic = new AirplaneLogic(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime game_time)
        {
            this._airplane_logic.UpdateAirplanes(game_time);
            UpdateScrollablePanel();
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(this._bg_color);

            GraphicsDevice.SetRenderTarget(this._game_render_target);
            GraphicsDevice.Clear(this._bg_color);
            DrawGameArea();

            GraphicsDevice.SetRenderTarget(this._strips_render_target);
            GraphicsDevice.Clear(this._bg_color);
            DrawPlaneStripsContent();


            GraphicsDevice.SetRenderTarget(null);
            DrawMainLayout();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw main layout of a game.
        /// </summary>
        private void DrawMainLayout()
        {
            this._spriteBatch.Begin();
            this._spriteBatch.Draw(this._game_render_target, this._game_area, this._bg_color);
            Rectangle scroll_offset = new Rectangle(0, (int)this._plane_stripes_offset.Y, this._plane_stripes_area.Width, this._plane_stripes_area.Height);
            this._spriteBatch.Draw(this._strips_render_target, this._plane_stripes_area, scroll_offset, this._bg_color);
            this._spriteBatch.End();
        }

        /// <summary>
        /// Draw game area content.
        /// </summary>
        private void DrawGameArea()
        {
            this._spriteBatch.Begin();
            this._map_generator.Draw(this._spriteBatch); // draw a game map
            foreach (Airplane airplane in this.airplanes)
            {
                if (!airplane.in_margin)
                    airplane.Draw(this._spriteBatch);
                else // if a plane is on the edge of game map
                    airplane.MarginalDraw(this._spriteBatch);
            }
            foreach (ArrivalAlert alert in this._airplane_logic.arrival_alerts)
            {
                alert.DrawArrivalAlert(this._spriteBatch);
            }
            this._spriteBatch.End();
        }

        /// <summary>
        /// Draw plane strips area content.
        /// </summary>
        private void DrawPlaneStripsContent()
        {
            this._spriteBatch.Begin();
            for(int i = 0; i < this.infostripes.Count; i++)
            {
                this.infostripes[i].Draw(this._spriteBatch, i);
            }
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
