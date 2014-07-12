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
using System.Threading.Tasks;

namespace LoLTimers
{
    /// <summary>
    /// Holds information about specific jungle camps and their timing info.
    /// </summary>
    public class JungleTimerInfo : ICloneable
    {
        public bool currentlyAlive;
        public int timeWhenKilled; // seconds into the game (e.g. 120 if camp killed at 2min)
        public readonly int respawnTime; // in seconds
        public readonly int initialSpawnTime; // in seconds
        public readonly string campName; // the name to display on the camp timer
        public int timeUntilRespawn; // used for display purposes only: in seconds
        public JungleCamps campType;

        public JungleTimerInfo(int respawnTime, int initialSpawnTime, string campName, JungleCamps campType)
        {
            currentlyAlive = true;
            timeWhenKilled = 0;
            this.respawnTime = respawnTime;
            this.initialSpawnTime = initialSpawnTime;
            this.campName = campName;
            timeUntilRespawn = 0;
            this.campType = campType;
        }

        public object Clone()
        {
            JungleTimerInfo clone = new JungleTimerInfo(respawnTime, initialSpawnTime, campName, campType);
            clone.currentlyAlive = currentlyAlive;
            clone.timeWhenKilled = timeWhenKilled;
            clone.timeUntilRespawn = timeUntilRespawn;
            return clone;
        }
    }
}
