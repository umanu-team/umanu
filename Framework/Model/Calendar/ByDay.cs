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
    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Specified a by day rule.
    /// </summary>
    public class ByDay : PersistentObject {

        /// <summary>
        /// Specific day of week.
        /// </summary>
        public DayOfWeek? DayOfWeek {
            get { return (DayOfWeek?)this.dayOfWeek.Value; }
            set { this.dayOfWeek.Value = (int?)value; }
        }
        private readonly PersistentFieldForNullableInt dayOfWeek =
            new PersistentFieldForNullableInt(nameof(DayOfWeek));

        /// <summary>
        /// Occurrence of a specific day - may only be specified in
        /// combination with frequency MONTHLY or with frquency
        /// YEARLY when ByWeekNumber is not specified.
        /// </summary>
        public int? Occurrence {
            get { return this.occurrence.Value; }
            set { this.occurrence.Value = value; }
        }
        private readonly PersistentFieldForNullableInt occurrence =
            new PersistentFieldForNullableInt(nameof(Occurrence));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ByDay()
            : base() {
            this.RegisterPersistentField(this.dayOfWeek);
            this.RegisterPersistentField(this.occurrence);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dayOfWeek">specific day of week</param>
        public ByDay(DayOfWeek dayOfWeek)
            : this() {
            this.DayOfWeek = dayOfWeek;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dayOfWeek">specific day of week</param>
        /// <param name="occurrence">occurrence of a specific day</param>
        public ByDay(DayOfWeek dayOfWeek, int occurrence)
            : this(dayOfWeek) {
            this.Occurrence = occurrence;
        }

        /// <summary>
        /// Determines whether this rule matches a specific date by
        /// day of week and occurrence for a monthly frequency.
        /// </summary>
        /// <param name="date">date to check</param>
        /// <returns>true if rule matches date, false otherwise</returns>
        public bool MatchesForMonth(DateTime date) {
            bool isMatch;
            if (this.MatchesForWeek(date)) {
                if (null == this.Occurrence || 0 == this.Occurrence) {
                    isMatch = true;
                } else if (this.Occurrence > 0) {
                    isMatch = Math.Ceiling(date.Day / 7m) == this.Occurrence;
                } else { // this.Occurrence < 0
                    int totalDaysOfMonth = DateTime.DaysInMonth(date.Year, date.Month);
                    isMatch = Math.Ceiling((totalDaysOfMonth - date.Day + 1) / 7m) * -1 == this.Occurrence;
                }
            } else {
                isMatch = false;
            }
            return isMatch;
        }

        /// <summary>
        /// Determines whether this rule matches a specific date by
        /// day of week.
        /// </summary>
        /// <param name="date">date to check</param>
        /// <returns>true if rule matches date by day of week, false
        /// otherwise</returns>
        public bool MatchesForWeek(DateTime date) {
            return date.DayOfWeek == this.DayOfWeek;
        }

        /// <summary>
        /// Determines whether this rule matches a specific date by
        /// day of week and occurrence for a yearly frequency.
        /// </summary>
        /// <param name="date">date to check</param>
        /// <returns>true if rule matches date, false otherwise</returns>
        public bool MatchesForYear(DateTime date) {
            bool isMatch;
            if (this.MatchesForWeek(date)) {
                if (null == this.Occurrence || 0 == this.Occurrence) {
                    isMatch = true;
                } else if (this.Occurrence > 0) {
                    isMatch = Math.Ceiling(date.DayOfYear / 7m) == this.Occurrence;
                } else { // this.Occurrence < 0
                    int totalDaysOfYear = UtcDateTime.DaysInYear(date.Year);
                    isMatch = Math.Ceiling((totalDaysOfYear - date.DayOfYear + 1) / 7m) * -1 == this.Occurrence;
                }
            } else {
                isMatch = false;
            }
            return isMatch;
        }

    }

}