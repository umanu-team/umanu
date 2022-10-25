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
    /// Predefined units for memory.
    /// </summary>
    public static class Memory {

        #region bits

        /// <summary>
        /// Predefined unit for memory in bits (bit).
        /// </summary>
        public static Unit InBits {
            get {
                return new Unit("bit", 0.125m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in kilobits (kbit).
        /// </summary>
        public static Unit InKilobits {
            get {
                return new Unit("kbit", 125m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in kibibits (Kibit).
        /// </summary>
        public static Unit InKibibits {
            get {
                return new Unit("Kibit", 128m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in megabits (Mbit).
        /// </summary>
        public static Unit InMegabits {
            get {
                return new Unit("Mbit", 125000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in mebibits (Mibit).
        /// </summary>
        public static Unit InMebibits {
            get {
                return new Unit("Mibit", 131072m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in gigabits (Gbit).
        /// </summary>
        public static Unit InGigabits {
            get {
                return new Unit("Gbit", 125000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in gibibits (Gibit).
        /// </summary>
        public static Unit InGibibits {
            get {
                return new Unit("Gibit", 134217728m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in terabits (Tbit).
        /// </summary>
        public static Unit InTerabits {
            get {
                return new Unit("Tbit", 125000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in tebibits (Tibit).
        /// </summary>
        public static Unit InTebibits {
            get {
                return new Unit("Tibit", 137438953472m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in petabits (Pbit).
        /// </summary>
        public static Unit InPetabits {
            get {
                return new Unit("Pbit", 125000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in pebibits (Pibit).
        /// </summary>
        public static Unit InPebibits {
            get {
                return new Unit("Pibit", 140737488355328m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in exabits (Ebit).
        /// </summary>
        public static Unit InExabits {
            get {
                return new Unit("Ebit", 125000000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in exbibits (Eibit).
        /// </summary>
        public static Unit InExbibits {
            get {
                return new Unit("Eibit", 144115188075855872m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in zettabits (Zbit).
        /// </summary>
        public static Unit InZettabits {
            get {
                return new Unit("Zbit", 125000000000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in zebibits (Zibit).
        /// </summary>
        public static Unit InZebibits {
            get {
                return new Unit("Zibit", 147573952589676412928m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in yottabits (Ybit).
        /// </summary>
        public static Unit InYottabits {
            get {
                return new Unit("Ybit", 125000000000000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in yobibits (Yibit).
        /// </summary>
        public static Unit InYobibits {
            get {
                return new Unit("Yibit", 151115727451828646838272m, Memory.InBytes);
            }
        }

        #endregion

        #region bytes

        /// <summary>
        /// Predefined unit for memory in bytes (B).
        /// </summary>
        public static Unit InBytes {
            get {
                return new Unit("B");
            }
        }

        /// <summary>
        /// Predefined unit for memory in kilobytes (kB).
        /// </summary>
        public static Unit InKilobytes {
            get {
                return new Unit("kB", 1000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in kibibytes (KiB).
        /// </summary>
        public static Unit InKibibytes {
            get {
                return new Unit("KiB", 1024m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in megabytes (MB).
        /// </summary>
        public static Unit InMegabytes {
            get {
                return new Unit("MB", 1000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in mebibytes (MiB).
        /// </summary>
        public static Unit InMebibytes {
            get {
                return new Unit("MiB", 1048576m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in gigabytes (GB).
        /// </summary>
        public static Unit InGigabytes {
            get {
                return new Unit("GB", 1000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in gibibytes (GiB).
        /// </summary>
        public static Unit InGibibytes {
            get {
                return new Unit("GiB", 1073741824m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in terabytes (TB).
        /// </summary>
        public static Unit InTerabytes {
            get {
                return new Unit("TB", 1000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in tebibytes (TiB).
        /// </summary>
        public static Unit InTebibytes {
            get {
                return new Unit("TiB", 1099511627776m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in petabytes (PB).
        /// </summary>
        public static Unit InPetabytes {
            get {
                return new Unit("PB", 1000000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in pebibytes (PiB).
        /// </summary>
        public static Unit InPebibytes {
            get {
                return new Unit("PiB", 1125899906842624m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in exabytes (EB).
        /// </summary>
        public static Unit InExabytes {
            get {
                return new Unit("EB", 1000000000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in exbibytes (EiB).
        /// </summary>
        public static Unit InExbibytes {
            get {
                return new Unit("EiB", 1152921504606846976m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in zettabytes (ZB).
        /// </summary>
        public static Unit InZettabytes {
            get {
                return new Unit("ZB", 1000000000000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in zebibytes (ZiB).
        /// </summary>
        public static Unit InZebibytes {
            get {
                return new Unit("ZiB", 1180591620717411303424m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in yottabytes (YB).
        /// </summary>
        public static Unit InYottabytes {
            get {
                return new Unit("YB", 1000000000000000000000000m, Memory.InBytes);
            }
        }

        /// <summary>
        /// Predefined unit for memory in yobibytes (YiB).
        /// </summary>
        public static Unit InYobibytes {
            get {
                return new Unit("YiB", 1208925819614629174706176m, Memory.InBytes);
            }
        }

        #endregion

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
            if (unit == Memory.InBits.Symbol) {
                result = Memory.InBits;
                success = true;
            } else if (unit == Memory.InBytes.Symbol) {
                result = Memory.InBytes;
                success = true;
            } else if (unit == Memory.InExabits.Symbol) {
                result = Memory.InExabits;
                success = true;
            } else if (unit == Memory.InExabytes.Symbol) {
                result = Memory.InExabytes;
                success = true;
            } else if (unit == Memory.InExbibits.Symbol) {
                result = Memory.InExbibits;
                success = true;
            } else if (unit == Memory.InExbibytes.Symbol) {
                result = Memory.InExbibytes;
                success = true;
            } else if (unit == Memory.InGibibits.Symbol) {
                result = Memory.InGibibits;
                success = true;
            } else if (unit == Memory.InGibibytes.Symbol) {
                result = Memory.InGibibytes;
                success = true;
            } else if (unit == Memory.InGigabits.Symbol) {
                result = Memory.InGigabits;
                success = true;
            } else if (unit == Memory.InGigabytes.Symbol) {
                result = Memory.InGigabytes;
                success = true;
            } else if (unit == Memory.InKibibits.Symbol) {
                result = Memory.InKibibits;
                success = true;
            } else if (unit == Memory.InKibibytes.Symbol) {
                result = Memory.InKibibytes;
                success = true;
            } else if (unit == Memory.InKilobits.Symbol) {
                result = Memory.InKilobits;
                success = true;
            } else if (unit == Memory.InKilobytes.Symbol) {
                result = Memory.InKilobytes;
                success = true;
            } else if (unit == Memory.InMebibits.Symbol) {
                result = Memory.InMebibits;
                success = true;
            } else if (unit == Memory.InMebibytes.Symbol) {
                result = Memory.InMebibytes;
                success = true;
            } else if (unit == Memory.InMegabits.Symbol) {
                result = Memory.InMegabits;
                success = true;
            } else if (unit == Memory.InMegabytes.Symbol) {
                result = Memory.InMegabytes;
                success = true;
            } else if (unit == Memory.InPebibits.Symbol) {
                result = Memory.InPebibits;
                success = true;
            } else if (unit == Memory.InPebibytes.Symbol) {
                result = Memory.InPebibytes;
                success = true;
            } else if (unit == Memory.InPetabits.Symbol) {
                result = Memory.InPetabits;
                success = true;
            } else if (unit == Memory.InPetabytes.Symbol) {
                result = Memory.InPetabytes;
                success = true;
            } else if (unit == Memory.InTebibits.Symbol) {
                result = Memory.InTebibits;
                success = true;
            } else if (unit == Memory.InTebibytes.Symbol) {
                result = Memory.InTebibytes;
                success = true;
            } else if (unit == Memory.InTerabits.Symbol) {
                result = Memory.InTerabits;
                success = true;
            } else if (unit == Memory.InTerabytes.Symbol) {
                result = Memory.InTerabytes;
                success = true;
            } else if (unit == Memory.InYobibits.Symbol) {
                result = Memory.InYobibits;
                success = true;
            } else if (unit == Memory.InYobibytes.Symbol) {
                result = Memory.InYobibytes;
                success = true;
            } else if (unit == Memory.InYottabits.Symbol) {
                result = Memory.InYottabits;
                success = true;
            } else if (unit == Memory.InYottabytes.Symbol) {
                result = Memory.InYottabytes;
                success = true;
            } else if (unit == Memory.InZebibits.Symbol) {
                result = Memory.InZebibits;
                success = true;
            } else if (unit == Memory.InZebibytes.Symbol) {
                result = Memory.InZebibytes;
                success = true;
            } else if (unit == Memory.InZettabits.Symbol) {
                result = Memory.InZettabits;
                success = true;
            } else if (unit == Memory.InZettabytes.Symbol) {
                result = Memory.InZettabytes;
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}