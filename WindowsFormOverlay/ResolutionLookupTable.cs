/*  LoL Jungle Timers - automatic timing of neutral monster camps in League of Legends.
    Copyright (C) 2014  Matthew Whittington

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

    If you need to contact me please feel free to email at gbbwhittington@gmail.com.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormsTimerOverlay
{
    /// <summary>
    /// A lookup table for positioning the timers at different resolutions.
    /// </summary>
    static class ResolutionLookupTable
    {
        // The lookup table
        private static Dictionary<int, Tuple<double, double>> lookupTable = new Dictionary<int, Tuple<double, double>>
        {
            {1920, new Tuple<double, double>(0.165, 0.677)}, // 1920 wide resolutions
            {1680, new Tuple<double, double>(0.170, 0.677)}, // 1680 wide resolutions
            {1600, new Tuple<double, double>(0.165, 0.677)}, // 1600 wide resoltuion, box res isn't wide enough to carry the timers properly anyway
            {1440, new Tuple<double, double>(0.170, 0.677)} // 1440 wide resolution
        };

        // The default key to lookup if the key requested doesn't exist
        private const int defaultTableKey = 1920;

        /// <summary>
        /// Access the resolution lookup table and return the relevant values.
        /// Will return a default set if no matching value exists.
        /// </summary>
        /// <param name="resolutionWidth">The width of the resolution to find offsets for.</param>
        /// <returns>A 2-tuple of doubles which represents the percentage through the given resolution
        /// at which the timers should be displayed to not write over the minimap or other important
        /// UI information in-game. For example, 0.165, 0.677 means the first place to write timers should
        /// be at width*0.165 and the second should be at width*0.677.</returns>
        public static Tuple<double, double> GetResolutionOffsets(int resolutionWidth) {
            if (lookupTable.ContainsKey(resolutionWidth))
                return lookupTable[resolutionWidth];
            return lookupTable[defaultTableKey];
        }
    }
}
