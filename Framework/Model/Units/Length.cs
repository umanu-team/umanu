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
    /// Predefined units for length.
    /// </summary>
    public static class Length {

        /// <summary>
        /// Predefined unit for length in nanometres (nm).
        /// </summary>
        public static Unit InNanometres {
            get {
                return new Unit("nm", 0.000000001m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in micrometres (µm).
        /// </summary>
        public static Unit InMicrometres {
            get {
                return new Unit("µm", 0.000001m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in millimetres (mm).
        /// </summary>
        public static Unit InMillimetres {
            get {
                return new Unit("mm", 0.001m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in inches (in).
        /// </summary>
        public static Unit InInches {
            get {
                return new Unit("in", 0.0254m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in centimetres (cm).
        /// </summary>
        public static Unit InCentimetres {
            get {
                return new Unit("cm", 0.01m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in decimetres (dm).
        /// </summary>
        public static Unit InDecimetres {
            get {
                return new Unit("dm", 0.1m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in feet (ft).
        /// </summary>
        public static Unit InFeet {
            get {
                return new Unit("ft", 0.3048m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in yard (yd).
        /// </summary>
        public static Unit InYard {
            get {
                return new Unit("yd", 0.9144m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in metres (m).
        /// </summary>
        public static Unit InMetres {
            get {
                return new Unit("m");
            }
        }

        /// <summary>
        /// Predefined unit for length in decametres (dam).
        /// </summary>
        public static Unit InDecametres {
            get {
                return new Unit("dam", 10m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in hectometres (hm).
        /// </summary>
        public static Unit InHectometres {
            get {
                return new Unit("hm", 100m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in kilometres (km).
        /// </summary>
        public static Unit InKilometres {
            get {
                return new Unit("km", 1000m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in miles (mi).
        /// </summary>
        public static Unit InMiles {
            get {
                return new Unit("mi", 1609.344m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in megametres (Mm).
        /// </summary>
        public static Unit InMegametres {
            get {
                return new Unit("Mm", 1000000m, Length.InMetres);
            }
        }

        /// <summary>
        /// Predefined unit for length in gigametres (Gm).
        /// </summary>
        public static Unit InGigametres {
            get {
                return new Unit("Gm", 1000000000m, Length.InMetres);
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
            if (unit == Length.InCentimetres.Symbol) {
                result = Length.InCentimetres;
                success = true;
            } else if (unit == Length.InDecametres.Symbol) {
                result = Length.InDecametres;
                success = true;
            } else if (unit == Length.InDecimetres.Symbol) {
                result = Length.InDecimetres;
                success = true;
            } else if (unit == Length.InFeet.Symbol) {
                result = Length.InFeet;
                success = true;
            } else if (unit == Length.InGigametres.Symbol) {
                result = Length.InGigametres;
                success = true;
            } else if (unit == Length.InHectometres.Symbol) {
                result = Length.InHectometres;
                success = true;
            } else if (unit == Length.InInches.Symbol) {
                result = Length.InInches;
                success = true;
            } else if (unit == Length.InKilometres.Symbol) {
                result = Length.InKilometres;
                success = true;
            } else if (unit == Length.InMegametres.Symbol) {
                result = Length.InMegametres;
                success = true;
            } else if (unit == Length.InMetres.Symbol) {
                result = Length.InMetres;
                success = true;
            } else if (unit == Length.InMicrometres.Symbol) {
                result = Length.InMicrometres;
                success = true;
            } else if (unit == Length.InMiles.Symbol) {
                result = Length.InMiles;
                success = true;
            } else if (unit == Length.InMillimetres.Symbol) {
                result = Length.InMillimetres;
                success = true;
            } else if (unit == Length.InNanometres.Symbol) {
                result = Length.InNanometres;
                success = true;
            } else if (unit == Length.InYard.Symbol) {
                result = Length.InYard;
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}
