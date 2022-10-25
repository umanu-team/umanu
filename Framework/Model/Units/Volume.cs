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
    /// Predefined units for volumes.
    /// </summary>
    public static class Volume {

        /// <summary>
        /// Predefined unit for volume in centilitres (cl).
        /// </summary>
        public static Unit InCentilitres {
            get {
                return new Unit("cl", 0.00001m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic centimetres (cm³).
        /// </summary>
        public static Unit InCubicCentimetres {
            get {
                return new Unit("cm³", 0.000001m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic decametres (dam³).
        /// </summary>
        public static Unit InCubicDecametres {
            get {
                return new Unit("dam³", 1000m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic decimetres (dm³).
        /// </summary>
        public static Unit InCubicDecimetres {
            get {
                return new Unit("dm³", 0.001m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic foot (ft³).
        /// </summary>
        public static Unit InCubicFoot {
            get {
                return new Unit("ft³", 0.028316846592m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic hectometres (hm³).
        /// </summary>
        public static Unit InCubicHectometres {
            get {
                return new Unit("hm³", 1000000m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic inches (in³).
        /// </summary>
        public static Unit InCubicInches {
            get {
                return new Unit("in³", 0.000016387064m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic kilometres (km³).
        /// </summary>
        public static Unit InCubicKilometres {
            get {
                return new Unit("km³", 1000000000m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic metres (m³).
        /// </summary>
        public static Unit InCubicMetres {
            get {
                return new Unit("m³");
            }
        }

        /// <summary>
        /// Predefined unit for volume in cubic millimetres (mm³).
        /// </summary>
        public static Unit InCubicMillimetres {
            get {
                return new Unit("mm³", 0.000000001m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in decilitres (dl).
        /// </summary>
        public static Unit InDecilitres {
            get {
                return new Unit("dl", 0.0001m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in US gallons (gal).
        /// </summary>
        public static Unit InGallons {
            get {
                return new Unit("gal", 0.003785411784m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in kilolitres (kl).
        /// </summary>
        public static Unit InKilolitres {
            get {
                return new Unit("kl", 1m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in litres (l).
        /// </summary>
        public static Unit InLitres {
            get {
                return new Unit("l", 0.001m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in megalitres (Ml).
        /// </summary>
        public static Unit InMegalitres {
            get {
                return new Unit("Ml", 1000m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in microlitres (μl).
        /// </summary>
        public static Unit InMicrolitres {
            get {
                return new Unit("μl", 0.000000001m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in millilitres (ml).
        /// </summary>
        public static Unit InMillilitres {
            get {
                return new Unit("ml", 0.000001m, Volume.InCubicMetres);
            }
        }

        /// <summary>
        /// Predefined unit for volume in US pints (pt).
        /// </summary>
        public static Unit InPints {
            get {
                return new Unit("pt", 0.000473176473m, Volume.InCubicMetres);
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
            if (unit == Volume.InCentilitres.Symbol) {
                result = Volume.InCentilitres;
                success = true;
            } else if (unit == Volume.InCubicCentimetres.Symbol) {
                result = Volume.InCubicCentimetres;
                success = true;
            } else if (unit == Volume.InCubicDecametres.Symbol) {
                result = Volume.InCubicDecametres;
                success = true;
            } else if (unit == Volume.InCubicDecimetres.Symbol) {
                result = Volume.InCubicDecimetres;
                success = true;
            } else if (unit == Volume.InCubicFoot.Symbol) {
                result = Volume.InCubicFoot;
                success = true;
            } else if (unit == Volume.InCubicHectometres.Symbol) {
                result = Volume.InCubicHectometres;
                success = true;
            } else if (unit == Volume.InCubicInches.Symbol) {
                result = Volume.InCubicInches;
                success = true;
            } else if (unit == Volume.InCubicKilometres.Symbol) {
                result = Volume.InCubicKilometres;
                success = true;
            } else if (unit == Volume.InCubicMetres.Symbol) {
                result = Volume.InCubicMetres;
                success = true;
            } else if (unit == Volume.InCubicMillimetres.Symbol) {
                result = Volume.InCubicMillimetres;
                success = true;
            } else if (unit == Volume.InDecilitres.Symbol) {
                result = Volume.InDecilitres;
                success = true;
            } else if (unit == Volume.InGallons.Symbol) {
                result = Volume.InGallons;
                success = true;
            } else if (unit == Volume.InKilolitres.Symbol) {
                result = Volume.InKilolitres;
                success = true;
            } else if (unit == Volume.InLitres.Symbol) {
                result = Volume.InLitres;
                success = true;
            } else if (unit == Volume.InMegalitres.Symbol) {
                result = Volume.InMegalitres;
                success = true;
            } else if (unit == Volume.InMicrolitres.Symbol) {
                result = Volume.InMicrolitres;
                success = true;
            } else if (unit == Volume.InMillilitres.Symbol) {
                result = Volume.InMillilitres;
                success = true;
            } else if (unit == Volume.InPints.Symbol) {
                result = Volume.InPints;
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}