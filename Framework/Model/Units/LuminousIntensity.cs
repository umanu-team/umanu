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
    /// Predefined units for luminous intensity.
    /// </summary>
    public static class LuminousIntensity {

        /// <summary>
        /// Predefined unit for luminous intensity in candela (cd).
        /// </summary>
        public static Unit InCandela {
            get {
                return new Unit("cd");
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
            if (unit == LuminousIntensity.InCandela.Symbol) {
                result = LuminousIntensity.InCandela;
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}