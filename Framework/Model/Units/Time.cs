/*********************************************************************
 * Umanu Framework / (C) Umanu Team / http://www.umanu.de/           *
 *                                                                   *
 * This program is free software: you can redistribute it and/or     *
 * modify it under the terms of the GNU Lesser General Public        *
 * License as published by the Free Software Foundation, either      *
 * version 3 of the License, or (at your option) any later version.  *
 *                                                                   *
 * This program is distributed in the hope that it will be useful,   *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of    *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the     *
 * GNU Lesser General Public License for more details.               *
 *                                                                   *
 * You should have received a copy of the GNU Lesser General Public  *
 * License along with this program.                                  *
 * If not, see <http://www.gnu.org/licenses/>.                       *
 *********************************************************************/

namespace Framework.Model.Units {

    /// <summary>
    /// Predefined units for time.
    /// </summary>
    public static class Time {

        /// <summary>
        /// Predefined unit for time in nanoseconds (ns).
        /// </summary>
        public static Unit InNanoseconds {
            get {
                return new Unit("ns", 0.000000001m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in microseconds (µs).
        /// </summary>
        public static Unit InMicroseconds {
            get {
                return new Unit("µs", 0.000001m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in milliseconds (ms).
        /// </summary>
        public static Unit InMilliseconds {
            get {
                return new Unit("ms", 0.001m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in centiseconds (cs).
        /// </summary>
        public static Unit InCentiseconds {
            get {
                return new Unit("cs", 0.01m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in deciseconds (ds).
        /// </summary>
        public static Unit InDeciseconds {
            get {
                return new Unit("ds", 0.1m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in seconds (s).
        /// </summary>
        public static Unit InSeconds {
            get {
                return new Unit("s");
            }
        }

        /// <summary>
        /// Predefined unit for time in decaseconds (das).
        /// </summary>
        public static Unit InDecaseconds {
            get {
                return new Unit("das", 10m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in minutes (min).
        /// </summary>
        public static Unit InMinutes {
            get {
                return new Unit("min", 60m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in hectoseconds (hs).
        /// </summary>
        public static Unit InHectoseconds {
            get {
                return new Unit("hs", 100m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in hours (h).
        /// </summary>
        public static Unit InHours {
            get {
                return new Unit("h", 3600m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in kiloseconds (ks).
        /// </summary>
        public static Unit InKiloseconds {
            get {
                return new Unit("ks", 1000m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in days (d).
        /// </summary>
        public static Unit InDays {
            get {
                return new Unit("d", 86400, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in weeks (wk).
        /// </summary>
        public static Unit InWeeks {
            get {
                return new Unit("wk", 604800, Time.InSeconds);
            }
        }


        /// <summary>
        /// Predefined unit for time in megaseconds (Ms).
        /// </summary>
        public static Unit InMegaseconds {
            get {
                return new Unit("Ms", 1000000m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in months (mo).
        /// </summary>
        public static Unit InMonths {
            get {
                return new Unit("mo", 2629746, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in Years (a).
        /// </summary>
        public static Unit InYears {
            get {
                return new Unit("a", 31556952, Time.InSeconds);
            }
        }

        /// <summary>
        /// Predefined unit for time in gigaseconds (Gs).
        /// </summary>
        public static Unit InGigaseconds {
            get {
                return new Unit("Gs", 1000000000m, Time.InSeconds);
            }
        }

        /// <summary>
        /// Converts the specified string representation of a unit to
        /// an instance of Unit.
        /// </summary>
        /// <param name="unit">string representation of unit</param>
        /// <param name="result">When this method returns, contains
        /// the Unit equivalent to the string repesentation, if the
        /// conversion succeeded, or null if the conversion failed.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true if value was converted successfully;
        /// otherwise, false</returns>
        public static bool TryParse(string unit, out Unit result) {
            bool success;
            if (unit == Time.InCentiseconds.Symbol) {
                result = Time.InCentiseconds;
                success = true;
            } else if (unit == Time.InDays.Symbol) {
                result = Time.InDays;
                success = true;
            } else if (unit == Time.InDecaseconds.Symbol) {
                result = Time.InDecaseconds;
                success = true;
            } else if (unit == Time.InDeciseconds.Symbol) {
                result = Time.InDeciseconds;
                success = true;
            } else if (unit == Time.InGigaseconds.Symbol) {
                result = Time.InGigaseconds;
                success = true;
            } else if (unit == Time.InHectoseconds.Symbol) {
                result = Time.InHectoseconds;
                success = true;
            } else if (unit == Time.InHours.Symbol) {
                result = Time.InHours;
                success = true;
            } else if (unit == Time.InKiloseconds.Symbol) {
                result = Time.InKiloseconds;
                success = true;
            } else if (unit == Time.InMegaseconds.Symbol) {
                result = Time.InMegaseconds;
                success = true;
            } else if (unit == Time.InMicroseconds.Symbol) {
                result = Time.InMicroseconds;
                success = true;
            } else if (unit == Time.InMilliseconds.Symbol) {
                result = Time.InMilliseconds;
                success = true;
            } else if (unit == Time.InMinutes.Symbol) {
                result = Time.InMinutes;
                success = true;
            } else if (unit == Time.InMonths.Symbol) {
                result = Time.InMonths;
                success = true;
            } else if (unit == Time.InNanoseconds.Symbol) {
                result = Time.InNanoseconds;
                success = true;
            } else if (unit == Time.InSeconds.Symbol) {
                result = Time.InSeconds;
                success = true;
            } else if (unit == Time.InWeeks.Symbol) {
                result = Time.InWeeks;
                success = true;
            } else if (unit == Time.InYears.Symbol) {
                result = Time.InYears;
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}