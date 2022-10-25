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

    /// <summary>
    /// Provider for bank holidays of Germany.
    /// </summary>
    public abstract class BankHolidayProviderForDe : BankHolidayProvider {

        /// <summary>
        /// Instanitates a new instance.
        /// </summary>
        public BankHolidayProviderForDe()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the all sains day of a year.
        /// </summary>
        /// <param name="year">year to get bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetAllSaintsOf(int year) {
            return new DateTime(year, 11, 1);
        }

        /// <summary>
        /// Gets the ascension day of a year.
        /// </summary>
        /// <param name="easterSunday">easter sunday of year to get
        /// bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetAscensionDayOf(DateTime easterSunday) {
            return easterSunday.AddDays(39);
        }

        /// <summary>
        /// Gets the boxing day of a year.
        /// </summary>
        /// <param name="year">year to get bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetBoxingDayOf(int year) {
            return new DateTime(year, 12, 26);
        }

        /// <summary>
        /// Gets the christmas day of a year.
        /// </summary>
        /// <param name="year">year to get bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetChristmasDayOf(int year) {
            return new DateTime(year, 12, 25);
        }

        /// <summary>
        /// Gets the corpus christi of a year.
        /// </summary>
        /// <param name="easterSunday">easter sunday of year to get
        /// bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetCorpusChristiOf(DateTime easterSunday) {
            return easterSunday.AddDays(60);
        }

        /// <summary>
        /// Gets the easter monday of a year.
        /// </summary>
        /// <param name="easterSunday">easter sunday of year to get
        /// bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetEsterMondayOf(DateTime easterSunday) {
            return easterSunday.AddDays(1);
        }

        /// <summary>
        /// Gets the german unity day of a year.
        /// </summary>
        /// <param name="year">year to get bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetGermanUnityDayOf(int year) {
            return new DateTime(year, 10, 3);
        }

        /// <summary>
        /// Gets the good friday of a year.
        /// </summary>
        /// <param name="easterSunday">easter sunday of year to get
        /// bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetGoodFridayOf(DateTime easterSunday) {
            return easterSunday.AddDays(-2);
        }

        /// <summary>
        /// Gets the labor day of a year.
        /// </summary>
        /// <param name="year">year to get bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetLaborDayOf(int year) {
            return new DateTime(year, 5, 1);
        }

        /// <summary>
        /// Gets the new years day of a year.
        /// </summary>
        /// <param name="year">year to get bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetNewYearsDayOf(int year) {
            return new DateTime(year, 1, 1);
        }

        /// <summary>
        /// Gets the with monday of a year.
        /// </summary>
        /// <param name="easterSunday">easter sunday of year to get
        /// bank holiday for</param>
        /// <returns>date of bank holiday</returns>
        protected DateTime GetWithMondayOf(DateTime easterSunday) {
            return easterSunday.AddDays(50);
        }

    }

}