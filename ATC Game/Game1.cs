using ATC_Game.GameObjects;
using ATC_Game.GameObjects.AirplaneFeatures;
using ATC_Game.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// TODO:
// přide body vstupů, ke kterým to letí
// dodělej info strips
// generování dsestinací udělej tak aby přiletaly ze stran ze kterých to dává smysl (pokud bude letiště reálné)
// natoč heading správným směrem airplane.getHeading
// nefunguje heading
// predelej speed na ground speed at to neukazuje nereálné čísla
// přidat time schedule a flight status
// přidat funkce pro time section
// udělej stripes jako scrollable panel

namespace ATC_Game
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public List<Airplane> airplanes;
        public List<InfoStripe> infostripes;
        public AirplaneLogic _airplane_logic;

        private RenderTarget2D _game_render_target;
        private RenderTarget2D _strips_render_target;
        private Rectangle _game_area, _plane_strips_area;

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
        }

        protected override void Initialize()
        {
            this._game_render_target = new RenderTarget2D(GraphicsDevice, 900, 900);
            this._strips_render_target = new RenderTarget2D(GraphicsDevice, 400, 400);
            this._game_area = new Rectangle(400, 0, 900, 900);
            this._plane_strips_area = new Rectangle(0, 0, 400, 400);

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

            base.Update(game_time);
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
            this._spriteBatch.Draw(this._strips_render_target, this._plane_strips_area, this._bg_color);
            this._spriteBatch.End();
        }

        /// <summary>
        /// Draw game area content.
        /// </summary>
        private void DrawGameArea()
        {
            this._spriteBatch.Begin();
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
            return new Vector2(this._plane_strips_area.Width, this._plane_strips_area.Height);
        }
    }
}
