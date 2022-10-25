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

namespace Framework.Model {

    using Framework.Presentation.Forms;
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents an instant in UTC time. 
    /// </summary>
    public struct UtcDateTime {

        /// <summary>
        /// Represents the largest  possible value of DateTime
        /// (read-only).
        /// </summary>
        public static readonly DateTime MaxValue = DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);

        /// <summary>
        /// Represents the smallest possible value of DateTime
        /// (read-only).
        /// </summary>
        public static readonly DateTime MinValue = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

        /// <summary>
        /// Gets a DateTime object that is set to the current date
        /// and time on this computer, expressed as the Coordinated
        /// Universal Time (UTC).
        /// </summary>
        public static DateTime Now {
            get {
                return DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets the current date, expressed as the Coordinated
        /// Universal Time (UTC).
        /// </summary>
        public static DateTime Today {
            get {
                DateTime utcNow = UtcDateTime.Now;
                return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, 0, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Compares two instances of DateTime and returns a value
        /// that indicates whether the first instance is earlier
        /// than, the same as, or later than the second instance.
        /// </summary>
        /// <param name="value1">first instance to compare</param>
        /// <param name="value2">second instance to compare</param>
        /// <returns>A signed integer that indicates the relative
        /// order of the comparands: Less than zero if x is less than
        /// y. Equal to zero if x is equal to y. Greater than zero if
        /// x is greater than y.</returns>
        public static int Compare(DateTime value1, DateTime value2) {
            return DateTime.Compare(value1, value2);
        }

        /// <summary>
        /// Converts a date and time to Coordinated Universal Time
        /// (UTC).
        /// </summary>
        /// <param name="dateTime">date and time to convert to UTC</param>
        /// <returns>date and time converted to UTC</returns>
        public static DateTime ConvertToUniversalTime(DateTime dateTime) {
            DateTime utcDateTime;
            if (DateTimeKind.Utc == dateTime.Kind) {
                utcDateTime = dateTime;
            } else if (DateTime.MinValue == dateTime) {
                utcDateTime = UtcDateTime.MinValue;
            } else if (DateTime.MaxValue == dateTime) {
                utcDateTime = UtcDateTime.MaxValue;
            } else if (DateTimeKind.Local == dateTime.Kind) {
                utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime);
            } else {
                utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }
            return utcDateTime;
        }

        /// <summary>
        /// Converts a date and time to Coordinated Universal Time
        /// (UTC).
        /// </summary>
        /// <param name="dateTime">date and time to convert to UTC</param>
        /// <param name="value">string containing date and time the
        /// dateTime parameter was converted from</param>
        /// <returns>date and time converted to UTC</returns>
        private static DateTime ConvertToUniversalTime(DateTime dateTime, string value) {
            DateTime specificDateTime;
            if (DateTimeKind.Unspecified == dateTime.Kind && Regex.ForLocalIso8601DateTime.IsMatch(value)) {
                specificDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            } else {
                specificDateTime = dateTime;
            }
            return UtcDateTime.ConvertToUniversalTime(specificDateTime);
        }

        /// <summary>
        /// Returns the number of days in the specified month and
        /// year.
        /// </summary>
        /// <param name="year">year of month to get number of days
        /// for</param>
        /// <param name="month">month to get number of days for</param>
        /// <returns>number of days in month for the specified year</returns>
        public static int DaysInMonth(int year, int month) {
            return DateTime.DaysInMonth(year, month);
        }

        /// <summary>
        /// Returns the number of days in the specified year.
        /// </summary>
        /// <param name="year">year to get number of days for</param>
        /// <returns>number of days for the specified year</returns>
        public static int DaysInYear(int year) {
            int daysInYear;
            if (DateTime.IsLeapYear(year)) {
                daysInYear = 366;
            } else {
                daysInYear = 365;
            }
            return daysInYear;
        }

        /// <summary>
        /// Returns a value indicating whether two DateTime instances
        /// have the same date and time value.
        /// </summary>
        /// <param name="value1">first instance to compare</param>
        /// <param name="value2">second instance to compare</param>
        /// <returns>true if the two values are equal; otherwise,
        /// false</returns>
        public static bool Equals(DateTime value1, DateTime value2) {
            return DateTime.Equals(value1, value2);
        }

        /// <summary>
        /// Converts a date/time value into a formatted read-only
        /// string.
        /// </summary>
        /// <param name="dateTime">data/time value to convert</param>
        /// <param name="dateTimeType">type of date/time to format
        /// date/time value as</param>
        /// <returns>date/time value as formatted read-only string</returns>
        public static string FormatAsIso8601Value(DateTime dateTime, DateTimeType dateTimeType) {
            string value;
            if (DateTimeType.Date == dateTimeType) {
                value = dateTime.ToString("yyyy\\-MM\\-dd");
            } else if (DateTimeType.DateAndTime == dateTimeType) {
                if (0 == dateTime.Second) {
                    value = dateTime.ToString("yyyy\\-MM\\-dd\\THH\\:mmK");
                } else {
                    value = dateTime.ToString("yyyy\\-MM\\-dd\\THH\\:mm\\:ssK");
                }
            } else if (DateTimeType.LocalDateAndTime == dateTimeType) {
                if (0 == dateTime.Second) {
                    value = dateTime.ToLocalTime().ToString("yyyy\\-MM\\-dd\\THH\\:mm");
                } else {
                    value = dateTime.ToLocalTime().ToString("yyyy\\-MM\\-dd\\THH\\:mm\\:ss");
                }
            } else if (DateTimeType.Month == dateTimeType) {
                value = dateTime.ToString("yyyy\\-MM");
            } else if (DateTimeType.Time == dateTimeType) {
                if (0 == dateTime.Second) {
                    value = dateTime.ToString("HH\\:mm");
                } else {
                    value = dateTime.ToString("HH\\:mm\\:ss");
                }
            } else if (DateTimeType.Week == dateTimeType) {
                string week = new GregorianCalendar().GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString(CultureInfo.InvariantCulture);
                if (week.Length < 2) {
                    week = "0" + week;
                }
                value = dateTime.ToString("yyyy");
                value += "-W";
                value += week;
            } else {
                throw new ArgumentException("DateTimeType \"" + dateTimeType.ToString() + "\" is not known.");
            }
            return value;
        }

        /// <summary>
        /// Converts a date/time value into a formatted read-only
        /// string.
        /// </summary>
        /// <param name="dateTime">data/time value to convert</param>
        /// <param name="dateTimeType">type of date/time to format
        /// date/time value as</param>
        /// <returns>date/time value as formatted read-only string</returns>
        public static string FormatAsReadOnlyValue(DateTime dateTime, DateTimeType dateTimeType) {
            string value;
            if (DateTimeType.Date == dateTimeType) {
                value = dateTime.ToString("d");
            } else if (DateTimeType.DateAndTime == dateTimeType) {
                value = dateTime.ToString("g");
                if (DateTimeKind.Utc == dateTime.Kind) {
                    value += " UTC";
                }
            } else if (DateTimeType.LocalDateAndTime == dateTimeType) {
                value = dateTime.ToLocalTime().ToString("g");
            } else if (DateTimeType.Month == dateTimeType) {
                value = dateTime.ToString("y");
            } else if (DateTimeType.Time == dateTimeType) {
                value = dateTime.ToString("t");
            } else if (DateTimeType.Week == dateTimeType) {
                string week = new GregorianCalendar().GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString(CultureInfo.InvariantCulture);
                if (week.Length < 2) {
                    week = "0" + week;
                }
                value = dateTime.ToString("yyyy") + "-W" + week;
            } else {
                throw new ArgumentException("DateTimeType \"" + dateTimeType.ToString() + "\" is not known.");
            }
            return value;
        }

        /// <summary>
        /// Converts a date/time value into a Unix timestamp.
        /// </summary>
        /// <param name="dateTime">data/time value to convert</param>
        /// <returns>date/time value as unix timestamp</returns>
        public double FormatAsUnixTimestamp(DateTime dateTime) {
            var dateTimeSpan = new TimeSpan(dateTime.Ticks);
            var unixEpoch = new TimeSpan(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).Ticks);
            return Math.Round(dateTimeSpan.TotalSeconds - unixEpoch.TotalSeconds);
        }

        /// <summary>
        /// Deserializes a 64-bit binary value and recreates an
        /// original serialized UTC DateTime object.
        /// </summary>
        /// <param name="dateData">64-bit signed integer that encodes
        /// the Kind property in a 2-bit field and the Ticks property
        /// in a 62-bit field</param>
        /// <returns>UTC DateTime instance that is equivalent to the
        /// DateTime object that was serialized by the ToBinary
        /// method</returns>
        public static DateTime FromBinary(long dateData) {
            return UtcDateTime.ConvertToUniversalTime(DateTime.FromBinary(dateData));
        }

        /// <summary>
        /// Converts the specified Windows file time to an equivalent
        /// UTC time.
        /// </summary>
        /// <param name="fileTime">Windows file time expressed in
        /// ticks</param>
        /// <returns>UTC DateTime instance representing the same date
        /// and time as fileTime</returns>
        public static DateTime FromFileTime(long fileTime) {
            return DateTime.FromFileTimeUtc(fileTime);
        }

        /// <summary>
        /// Returns a UTC DateTime equivalent to the specified OLE
        /// Automation Date.
        /// </summary>
        /// <param name="value">OLE Automation Date value</param>
        /// <returns>UTC DateTime instance representing the same date
        /// and time as d</returns>
        public static DateTime FromOADate(double value) {
            return UtcDateTime.ConvertToUniversalTime(DateTime.FromOADate(value));
        }

        /// <summary>
        /// Returns an value indicating whether the specified year is
        /// a leap year.
        /// </summary>
        /// <param name="year">4-digit year</param>
        /// <returns>true if year is a leap year; otherwise, false</returns>
        public static bool IsLeapYear(int year) {
            return DateTime.IsLeapYear(year);
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <returns>UTC DateTime instance that is equivalent to the
        /// date and time contained in s</returns>
        public static DateTime Parse(string value) {
            return UtcDateTime.ConvertToUniversalTime(DateTime.Parse(value), value);
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <param name="provider">object that supplies
        /// culture-specific format information about s</param>
        /// <returns>UTC DateTime instance that is equivalent to the
        /// date and time contained in s</returns>
        public static DateTime Parse(string value, IFormatProvider provider) {
            return UtcDateTime.ConvertToUniversalTime(DateTime.Parse(value, provider), value);
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <param name="provider">object that supplies
        /// culture-specific format information about s</param>
        /// <param name="style">bitwise combination of enumeration
        /// values that indicates the permitted format of s (e.g.
        /// None)</param>
        /// <returns>UTC DateTime instance that is equivalent to the
        /// date and time contained in s</returns>
        public static DateTime Parse(string value, IFormatProvider provider, DateTimeStyles style) {
            return UtcDateTime.ConvertToUniversalTime(DateTime.Parse(value, provider, style), value);
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent. The format of
        /// the string representation must match the specified format
        /// exactly or an exception is thrown.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <param name="format">allowable format of s</param>
        /// <param name="provider">object that supplies
        /// culture-specific format information about s</param>
        /// <returns>UTC DateTime instance that is equivalent to the
        /// date and time contained in s</returns>
        public static DateTime ParseExact(string value, string format, IFormatProvider provider) {
            return UtcDateTime.ConvertToUniversalTime(DateTime.ParseExact(value, format, provider), value);
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent. The format of
        /// the string representation must match the specified format
        /// exactly or an exception is thrown.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <param name="format">allowable format of s</param>
        /// <param name="provider">object that supplies
        /// culture-specific format information about s</param>
        /// <param name="style">bitwise combination of enumeration
        /// values that indicates the permitted format of s (e.g.
        /// None)</param>
        /// <returns>UTC DateTime instance that is equivalent to the
        /// date and time contained in s</returns>
        public static DateTime ParseExact(string value, string format, IFormatProvider provider, DateTimeStyles style) {
            return UtcDateTime.ConvertToUniversalTime(DateTime.ParseExact(value, format, provider, style), value);
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent. The format of
        /// the string representation must match at least one of the
        /// specified formats exactly or an exception is thrown.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <param name="formats">array of allowable formats of s</param>
        /// <param name="provider">object that supplies
        /// culture-specific format information about s</param>
        /// <param name="style">bitwise combination of enumeration
        /// values that indicates the permitted format of s (e.g.
        /// None)</param>
        /// <returns>UTC DateTime instance that is equivalent to the
        /// date and time contained in s</returns>
        public static DateTime ParseExact(string value, string[] formats, IFormatProvider provider, DateTimeStyles style) {
            return UtcDateTime.ConvertToUniversalTime(DateTime.ParseExact(value, formats, provider, style), value);
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <param name="result">When this method returns, contains
        /// the UTC DateTime value equivalent to the date and time
        /// contained in s, if the conversion succeeded, or MinValue
        /// if the conversion failed. This parameter is passed
        /// uninitialized.</param>
        /// <returns>true if s was converted successfully; otherwise,
        /// false</returns>
        public static bool TryParse(string value, out DateTime result) {
            bool success = DateTime.TryParse(value, out result);
            if (success) {
                if (Regex.ForStandAloneTime.IsMatch(value)) {
                    result = new DateTime(1, 1, 1, result.Hour, result.Minute, result.Second, 0, DateTimeKind.Utc);
                } else {
                    result = UtcDateTime.ConvertToUniversalTime(result, value);
                }
            }
            return success;
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <param name="provider">object that supplies
        /// culture-specific format information about s</param>
        /// <param name="style">bitwise combination of enumeration
        /// values that indicates the permitted format of s (e.g.
        /// None)</param>
        /// <param name="result">When this method returns, contains
        /// the UTC DateTime value equivalent to the date and time
        /// contained in s, if the conversion succeeded, or MinValue
        /// if the conversion failed. This parameter is passed
        /// uninitialized.</param>
        /// <returns>true if s was converted successfully; otherwise,
        /// false</returns>
        public static bool TryParse(string value, IFormatProvider provider, DateTimeStyles style, out DateTime result) {
            bool success = DateTime.TryParse(value, provider, style, out result);
            if (success) {
                result = UtcDateTime.ConvertToUniversalTime(result, value);
            }
            return success;
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent. The format of
        /// the string representation must match the specified format
        /// exactly or an exception is thrown.
        /// </summary>
        /// <param name="value">string that contains a date and time to
        /// convert</param>
        /// <param name="format">allowable format of s</param>
        /// <param name="provider">object that supplies
        /// culture-specific format information about s</param>
        /// <param name="style">bitwise combination of enumeration
        /// values that indicates the permitted format of s (e.g.
        /// None)</param>
        /// <param name="result">When this method returns, contains
        /// the UTC DateTime value equivalent to the date and time
        /// contained in s, if the conversion succeeded, or MinValue
        /// if the conversion failed. This parameter is passed
        /// uninitialized.</param>
        /// <returns>true if s was converted successfully; otherwise,
        /// false</returns>
        public static bool TryParseExact(string value, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result) {
            bool success = DateTime.TryParseExact(value, format, provider, style, out result);
            if (success) {
                result = UtcDateTime.ConvertToUniversalTime(result, value);
            }
            return success;
        }

        /// <summary>
        /// Converts the specified string representation of a date
        /// and time to its UTC DateTime equivalent. The format of
        /// the string representation must match at least one of the
        /// specified formats exactly or an exception is thrown.
        /// </summary>
        /// <param name="value">string that contains a date and time
        /// to convert</param>
        /// <param name="formats">array of allowable formats of s</param>
        /// <param name="provider">object that supplies
        /// culture-specific format information about s</param>
        /// <param name="style">bitwise combination of enumeration
        /// values that indicates the permitted format of s (e.g.
        /// None)</param>
        /// <param name="result">When this method returns, contains
        /// the UTC DateTime value equivalent to the date and time
        /// contained in s, if the conversion succeeded, or MinValue
        /// if the conversion failed. This parameter is passed
        /// uninitialized.</param>
        /// <returns>true if s was converted successfully; otherwise,
        /// false</returns>
        public static bool TryParseExact(string value, string[] formats, IFormatProvider provider, DateTimeStyles style, out DateTime result) {
            bool success = DateTime.TryParseExact(value, formats, provider, style, out result);
            if (success) {
                result = UtcDateTime.ConvertToUniversalTime(result, value);
            }
            return success;
        }

    }

}