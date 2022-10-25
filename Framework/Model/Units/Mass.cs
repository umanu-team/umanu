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
    /// Predefined units for mass.
    /// </summary>
    public static class Mass {

        /// <summary>
        /// Predefined unit for mass in nanograms (ng).
        /// </summary>
        public static Unit InNanograms {
            get {
                return new Unit("ng", 0.000000001m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in micrograms (µg).
        /// </summary>
        public static Unit InMicrograms {
            get {
                return new Unit("µg", 0.000001m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in milligrams (mg).
        /// </summary>
        public static Unit InMilligrams {
            get {
                return new Unit("mg", 0.001m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in grains (gr).
        /// </summary>
        public static Unit InGrains {
            get {
                return new Unit("gr", 0.06479891m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in centigrams (cg).
        /// </summary>
        public static Unit InCentigrams {
            get {
                return new Unit("cg", 0.01m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in decigrams (dg).
        /// </summary>
        public static Unit InDecigrams {
            get {
                return new Unit("dg", 0.1m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in grams (g).
        /// </summary>
        public static Unit InGrams {
            get {
                return new Unit("g");
            }
        }

        /// <summary>
        /// Predefined unit for mass in drams (dr).
        /// </summary>
        public static Unit InDrams {
            get {
                return new Unit("dr", 1.7718451953125m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in decagrams (dag).
        /// </summary>
        public static Unit InDecagrams {
            get {
                return new Unit("dag", 10m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in ounce (oz).
        /// </summary>
        public static Unit InOunce {
            get {
                return new Unit("oz", 28.349523125m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in hectograms (hg).
        /// </summary>
        public static Unit InHectograms {
            get {
                return new Unit("hg", 100m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in pounds (lb).
        /// </summary>
        public static Unit InPounds {
            get {
                return new Unit("lb", 453.59237m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in kilograms (kg).
        /// </summary>
        public static Unit InKilograms {
            get {
                return new Unit("kg", 1000m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in quarter (qtr).
        /// </summary>
        public static Unit InQuarter {
            get {
                return new Unit("qtr", 11339.80925m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in hundredweight (cwt).
        /// </summary>
        public static Unit InHundredweight {
            get {
                return new Unit("cwt", 45359.237m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in megagrams (Mg).
        /// </summary>
        public static Unit InMegagrams {
            get {
                return new Unit("Mg", 1000000m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in tonnes (t).
        /// </summary>
        public static Unit InTonnes {
            get {
                return new Unit("t", 1000000m, Mass.InGrams);
            }
        }

        /// <summary>
        /// Predefined unit for mass in gigagrams (Gg).
        /// </summary>
        public static Unit InGigagrams {
            get {
                return new Unit("Gg", 1000000000m, Mass.InGrams);
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
            if (unit == Mass.InCentigrams.Symbol) {
                result = Mass.InCentigrams;
                success = true;
            } else if (unit == Mass.InDecagrams.Symbol) {
                result = Mass.InDecagrams;
                success = true;
            } else if (unit == Mass.InDecigrams.Symbol) {
                result = Mass.InDecigrams;
                success = true;
            } else if (unit == Mass.InDrams.Symbol) {
                result = Mass.InDrams;
                success = true;
            } else if (unit == Mass.InGigagrams.Symbol) {
                result = Mass.InGigagrams;
                success = true;
            } else if (unit == Mass.InGrains.Symbol) {
                result = Mass.InGrains;
                success = true;
            } else if (unit == Mass.InGrams.Symbol) {
                result = Mass.InGrams;
                success = true;
            } else if (unit == Mass.InHectograms.Symbol) {
                result = Mass.InHectograms;
                success = true;
            } else if (unit == Mass.InHundredweight.Symbol) {
                result = Mass.InHundredweight;
                success = true;
            } else if (unit == Mass.InKilograms.Symbol) {
                result = Mass.InKilograms;
                success = true;
            } else if (unit == Mass.InMegagrams.Symbol) {
                result = Mass.InMegagrams;
                success = true;
            } else if (unit == Mass.InMicrograms.Symbol) {
                result = Mass.InMicrograms;
                success = true;
            } else if (unit == Mass.InMilligrams.Symbol) {
                result = Mass.InMilligrams;
                success = true;
            } else if (unit == Mass.InNanograms.Symbol) {
                result = Mass.InNanograms;
                success = true;
            } else if (unit == Mass.InOunce.Symbol) {
                result = Mass.InOunce;
                success = true;
            } else if (unit == Mass.InPounds.Symbol) {
                result = Mass.InPounds;
                success = true;
            } else if (unit == Mass.InQuarter.Symbol) {
                result = Mass.InQuarter;
                success = true;
            } else if (unit == Mass.InTonnes.Symbol) {
                result = Mass.InTonnes;
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}
