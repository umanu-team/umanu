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

namespace Framework.Model.Calendar {

    using Framework.Persistence;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provider for bank holidays.
    /// </summary>
    public class BankHolidayProvider : PersistentObject {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public BankHolidayProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets all dates of bank holidays.
        /// </summary>
        /// <param name="minDateTime">minimum date/time</param>
        /// <param name="maxDateTime">maximum date/time</param>
        /// <returns>all dates of bank holidays</returns>
        public IEnumerable<DateTime> GetBankHolidays(DateTime minDateTime, DateTime maxDateTime) {
            for (int year = minDateTime.Year; year <= maxDateTime.Year; year++) {
                foreach (var bankHoliday in this.GetBankHolidaysFor(year)) {
                    if (bankHoliday >= minDateTime && bankHoliday <= maxDateTime) {
                        yield return bankHoliday;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all dates of bank holidays of a specific year.
        /// </summary>
        /// <param name="year">year to get bank holidays for</param>
        /// <returns>all dates of bank holidays for year</returns>
        protected virtual IEnumerable<DateTime> GetBankHolidaysFor(int year) {
            throw new NotImplementedException("Method GetBankHolidaysForYear(int) has to be implemented in each derived class.");
        }

        /// <summary>
        /// Gets the date for easter sunday for a specific year using
        /// the extended gauss algorithm.
        /// </summary>
        /// <param name="year">year to get easter sunday for</param>
        /// <returns> easter sunday for specific year</returns>
        protected static DateTime GetEasterSundayFor(int year) {
            var k = year / 100;
            var a = year % 19;
            var d = (19 * a + (15 + (3 * k + 3) / 4 - (8 * k + 13) / 25)) % 30;
            var og = 21 + d - ((d + a / 11) / 29);
            var day = og + (7 - (og - (7 - (year + year / 4 + (2 - (3 * k + 3) / 4)) % 7)) % 7);
            int month = 3;
            if (day > 31) {
                day -= 31;
                month++;
            }
            return new DateTime(year, month, day);
        }

    }

}