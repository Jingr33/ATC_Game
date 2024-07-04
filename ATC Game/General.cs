using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace ATC_Game
{
    public class General
    {
        /// <summary>
        /// Convert file data (format: xx:yy) to vector2
        /// </summary>
        /// <param name="file_data">information from file</param>
        /// <returns>vactor2 variable</returns>
        public static Vector2 FileDataToVector2(string file_data)
        {
            string[] array = file_data.Split(':');
            return new Vector2(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture));
        }
    }
}
