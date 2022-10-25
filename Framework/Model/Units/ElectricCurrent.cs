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
    /// Predefined units for electric current.
    /// </summary>
    public static class ElectricCurrent {

        /// <summary>
        /// Predefined unit for electric current in amperes (A).
        /// </summary>
        public static Unit InAmperes {
            get {
                return new Unit("A");
            }
        }

        /// <summary>
        /// Predefined unit for electric current in abamperes (abamp).
        /// </summary>
        public static Unit InAbamperes {
            get {
                return new Unit("abamp", 10m, ElectricCurrent.InAmperes);
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
            if (unit == ElectricCurrent.InAmperes.Symbol) {
                result = ElectricCurrent.InAmperes;
                success = true;
            } else if (unit == ElectricCurrent.InAbamperes.Symbol) {
                result = ElectricCurrent.InAbamperes;
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}