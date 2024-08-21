using ATC_Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private bool _object_assigned;
        private Airplane _airplane;
        private Airport _airport;

        public ObjectInfoPanel(Game1 game)
        {
            this._game = game;
            this._object_assigned = false;
            this._airplane = null;
            this._airport = null;
        }

        /// <summary>
        /// Update evetns in the object info panel.
        /// </summary>
        /// <param name="game_time"></param>
        public void Update(GameTime game_time)
        {

        }

        /// <summary>
        /// Draw all textures of the object info panel.
        /// </summary>
        /// <param name="sprite_batch"></param>
        public void Draw(SpriteBatch sprite_batch)
        {

        }
    }
}
