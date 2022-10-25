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

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provider for bank holidays of North Rhine-Westphalia in
    /// Germany.
    /// </summary>
    public sealed class BankHolidayProviderForDeNw : BankHolidayProviderForDe {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public BankHolidayProviderForDeNw()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets all dates of bank holidays of a specific year.
        /// </summary>
        /// <param name="year">year to get bank holidays for</param>
        /// <returns>all dates of bank holidays for year</returns>
        protected override IEnumerable<DateTime> GetBankHolidaysFor(int year) {
            var easterSunday = BankHolidayProvider.GetEasterSundayFor(year);
            yield return this.GetNewYearsDayOf(year);
            yield return this.GetGoodFridayOf(easterSunday);
            yield return easterSunday;
            yield return this.GetEsterMondayOf(easterSunday);
            yield return this.GetLaborDayOf(year);
            yield return this.GetAscensionDayOf(easterSunday);
            yield return this.GetWithMondayOf(easterSunday);
            yield return this.GetCorpusChristiOf(easterSunday);
            yield return this.GetGermanUnityDayOf(year);
            yield return this.GetAllSaintsOf(year);
            yield return this.GetBoxingDayOf(year);
        }

    }

}