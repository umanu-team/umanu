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

    using System;

    /// <summary>
    /// Predefined units for temperature.
    /// </summary>
    public static class Temperature {

        /// <summary>
        /// Exception message for non matching units.
        /// </summary>
        private static string unitExceptionMessage = "Non temperature units cannot be converted into temperature units.";

        /// <summary>
        /// Predefined unit for temperature in celsius (°C).
        /// </summary>
        public static Unit InCelsius {
            get {
                return new Unit("°C");
            }
        }

        /// <summary>
        /// Predefined unit for temperature in fahrenheit (°F).
        /// </summary>
        public static Unit InFahrenheit {
            get {
                return new Unit("°F");
            }
        }

        /// <summary>
        /// Predefined unit for temperature in kelvin (K).
        /// </summary>
        public static Unit InKelvin {
            get {
                return new Unit("K");
            }
        }

        /// <summary>
        /// Predefined unit for temperature in rankine (°R).
        /// </summary>
        public static Unit InRankine {
            get {
                return new Unit("°R", 5m / 9m, Temperature.InKelvin);
            }
        }

        # region convert to celsius

        /// <summary>
        /// Converts a temperature to a temperature in celsius.
        /// </summary>
        /// <param name="value">temperature to convert</param>
        /// <returns>temperature converted to celsius</returns>
        public static NumberWithFixUnit ConvertToCelsius(NumberWithFixUnit value) {
            return new NumberWithFixUnit(Temperature.ConvertToCelsiusValue(value), Temperature.InCelsius);
        }

        /// <summary>
        /// Converts a temperature to a temperature in celsius.
        /// </summary>
        /// <param name="value">value of temperature</param>
        /// <param name="unit">unit of temperature</param>
        /// <returns>temperature converted to celsius</returns>
        public static NumberWithFixUnit ConvertToCelsius(decimal value, Unit unit) {
            return Temperature.ConvertToCelsius(new NumberWithFixUnit(value, unit));
        }

        /// <summary>
        /// Converts a temperature to a temperature in celsius.
        /// </summary>
        /// <param name="value">temperature to convert</param>
        /// <returns>value of temperature converted to celsius</returns>
        public static decimal? ConvertToCelsiusValue(NumberWithFixUnit value) {
            decimal? convertedValue;
            if (!value.HasValue) {
                convertedValue = null;
            } else if (value.BaseUnit.Equals(Temperature.InCelsius)) {
                convertedValue = value.NumberInBaseUnit;
            } else if (value.BaseUnit.Equals(Temperature.InFahrenheit)) {
                convertedValue = (value.NumberInBaseUnit - 32m) * 5m / 9m;
            } else if (value.BaseUnit.Equals(Temperature.InKelvin)) {
                convertedValue = value.NumberInBaseUnit - 273.15m;
            } else {
                throw new ArgumentException(Temperature.unitExceptionMessage, "value");
            }
            return convertedValue;
        }

        /// <summary>
        /// Converts a temperature to a temperature in celsius.
        /// </summary>
        /// <param name="value">value of temperature</param>
        /// <param name="unit">unit of temperature</param>
        /// <returns>value of temperature converted to celsius</returns>
        public static decimal? ConvertToCelsiusValue(decimal value, Unit unit) {
            return Temperature.ConvertToCelsiusValue(new NumberWithFixUnit(value, unit));
        }

        #endregion

        # region convert to fahrenheit

        /// <summary>
        /// Converts a temperature to a temperature in fahrenheit.
        /// </summary>
        /// <param name="value">temperature to convert</param>
        /// <returns>temperature converted to fahrenheit</returns>
        public static NumberWithFixUnit ConvertToFahrenheit(NumberWithFixUnit value) {
            return new NumberWithFixUnit(Temperature.ConvertToFahrenheitValue(value), Temperature.InFahrenheit);
        }

        /// <summary>
        /// Converts a temperature to a temperature in fahrenheit.
        /// </summary>
        /// <param name="value">value of temperature</param>
        /// <param name="unit">unit of temperature</param>
        /// <returns>temperature converted to fahrenheit</returns>
        public static NumberWithFixUnit ConvertToFahrenheit(decimal value, Unit unit) {
            return Temperature.ConvertToFahrenheit(new NumberWithFixUnit(value, unit));
        }

        /// <summary>
        /// Converts a temperature to a temperature in fahrenheit.
        /// </summary>
        /// <param name="value">temperature to convert</param>
        /// <returns>value of temperature converted to fahrenheit</returns>
        public static decimal? ConvertToFahrenheitValue(NumberWithFixUnit value) {
            decimal? convertedValue;
            if (!value.HasValue) {
                convertedValue = null;
            } else if (value.BaseUnit.Equals(Temperature.InCelsius)) {
                convertedValue = value.NumberInBaseUnit * 9m / 5m + 32m;
            } else if (value.BaseUnit.Equals(Temperature.InFahrenheit)) {
                convertedValue = value.NumberInBaseUnit;
            } else if (value.BaseUnit.Equals(Temperature.InKelvin)) {
                convertedValue = value.NumberInBaseUnit * 9m / 5m - 459.67m;
            } else {
                throw new ArgumentException(Temperature.unitExceptionMessage, "value");
            }
            return convertedValue;
        }

        /// <summary>
        /// Converts a temperature to a temperature in fahrenheit.
        /// </summary>
        /// <param name="value">value of temperature</param>
        /// <param name="unit">unit of temperature</param>
        /// <returns>value of temperature converted to fahrenheit</returns>
        public static decimal? ConvertToFahrenheitValue(decimal value, Unit unit) {
            return Temperature.ConvertToFahrenheitValue(new NumberWithFixUnit(value, unit));
        }

        #endregion

        # region convert to kelvin

        /// <summary>
        /// Converts a temperature to a temperature in kelvin.
        /// </summary>
        /// <param name="value">temperature to convert</param>
        /// <returns>temperature converted to kelvin</returns>
        public static NumberWithFixUnit ConvertToKelvin(NumberWithFixUnit value) {
            return new NumberWithFixUnit(Temperature.ConvertToKelvinValue(value), Temperature.InKelvin);
        }

        /// <summary>
        /// Converts a temperature to a temperature in kelvin.
        /// </summary>
        /// <param name="value">value of temperature</param>
        /// <param name="unit">unit of temperature</param>
        /// <returns>temperature converted to kelvin</returns>
        public static NumberWithFixUnit ConvertToKelvin(decimal value, Unit unit) {
            return Temperature.ConvertToKelvin(new NumberWithFixUnit(value, unit));
        }

        /// <summary>
        /// Converts a temperature to a temperature in kelvin.
        /// </summary>
        /// <param name="value">temperature to convert</param>
        /// <returns>value of temperature converted to kelvin</returns>
        public static decimal? ConvertToKelvinValue(NumberWithFixUnit value) {
            decimal? convertedValue;
            if (!value.HasValue) {
                convertedValue = null;
            } else if (value.BaseUnit.Equals(Temperature.InCelsius)) {
                convertedValue = value.NumberInBaseUnit + 273.15m;
            } else if (value.BaseUnit.Equals(Temperature.InFahrenheit)) {
                convertedValue = (value.NumberInBaseUnit + 459.67m) * 5m / 9m;
            } else if (value.BaseUnit.Equals(Temperature.InKelvin)) {
                convertedValue = value.NumberInBaseUnit;
            } else {
                throw new ArgumentException(Temperature.unitExceptionMessage, "value");
            }
            return convertedValue;
        }

        /// <summary>
        /// Converts a temperature to a temperature in kelvin.
        /// </summary>
        /// <param name="value">value of temperature</param>
        /// <param name="unit">unit of temperature</param>
        /// <returns>value of temperature converted to kelvin</returns>
        public static decimal? ConvertToKelvinValue(decimal value, Unit unit) {
            return Temperature.ConvertToKelvinValue(new NumberWithFixUnit(value, unit));
        }

        #endregion

        # region convert to rankine

        /// <summary>
        /// Converts a temperature to a temperature in rankine.
        /// </summary>
        /// <param name="value">temperature to convert</param>
        /// <returns>temperature converted to rankine</returns>
        public static NumberWithFixUnit ConvertToRankine(NumberWithFixUnit value) {
            return Temperature.ConvertToKelvin(value).ConvertToUnit(Temperature.InRankine);
        }

        /// <summary>
        /// Converts a temperature to a temperature in rankine.
        /// </summary>
        /// <param name="value">value of temperature</param>
        /// <param name="unit">unit of temperature</param>
        /// <returns>temperature converted to rankine</returns>
        public static NumberWithFixUnit ConvertToRankine(decimal value, Unit unit) {
            return Temperature.ConvertToKelvin(value, unit).ConvertToUnit(Temperature.InRankine);
        }

        /// <summary>
        /// Converts a temperature to a temperature in rankine.
        /// </summary>
        /// <param name="value">temperature to convert</param>
        /// <returns>value of temperature converted to rankine</returns>
        public static decimal? ConvertToRankineValue(NumberWithFixUnit value) {
            return Temperature.ConvertToKelvin(value).ConvertValueToUnit(Temperature.InRankine);
        }

        /// <summary>
        /// Converts a temperature to a temperature in rankine.
        /// </summary>
        /// <param name="value">value of temperature</param>
        /// <param name="unit">unit of temperature</param>
        /// <returns>value of temperature converted to rankine</returns>
        public static decimal? ConvertToRankineValue(decimal value, Unit unit) {
            return Temperature.ConvertToKelvin(value, unit).ConvertValueToUnit(Temperature.InRankine);
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
            if (unit == Temperature.InCelsius.Symbol) {
                result = Temperature.InCelsius;
                success = true;
            } else if (unit == Temperature.InFahrenheit.Symbol) {
                result = Temperature.InFahrenheit;
                success = true;
            } else if (unit == Temperature.InKelvin.Symbol) {
                result = Temperature.InKelvin;
                success = true;
            } else if (unit == Temperature.InRankine.Symbol) {
                result = Temperature.InRankine;
                success = true;
            } else {
                result = null;
                success = false;
            }
            return success;
        }

    }

}