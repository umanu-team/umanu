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

    using System;
    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Represents opening hours.
    /// </summary>
    public class OpeningHours : PersistentObject {

        /// <summary>
        /// Hour of closing time.
        /// </summary>
        public byte? ClosingTimeHour {
            get { return this.closingTimeHour.Value; }
            set { this.closingTimeHour.Value = value; }
        }
        private readonly PersistentFieldForNullableByte closingTimeHour =
            new PersistentFieldForNullableByte(nameof(ClosingTimeHour));

        /// <summary>
        /// Minute of closing time.
        /// </summary>
        public byte? ClosingTimeMinute {
            get { return this.closingTimeMinute.Value; }
            set { this.closingTimeMinute.Value = value; }
        }
        private readonly PersistentFieldForNullableByte closingTimeMinute =
            new PersistentFieldForNullableByte(nameof(ClosingTimeMinute));

        /// <summary>
        /// Day of week.
        /// </summary>
        public DayOfWeek? DayOfWeek {
            get { return (DayOfWeek?)this.dayOfWeek.Value; }
            set { this.dayOfWeek.Value = (int?)value; }
        }
        private readonly PersistentFieldForNullableInt dayOfWeek =
            new PersistentFieldForNullableInt(nameof(DayOfWeek));

        /// <summary>
        /// Hour of opening time.
        /// </summary>
        public byte? OpeningTimeHour {
            get { return this.openingTimeHour.Value; }
            set { this.openingTimeHour.Value = value; }
        }
        private readonly PersistentFieldForNullableByte openingTimeHour =
            new PersistentFieldForNullableByte(nameof(OpeningTimeHour));

        /// <summary>
        /// Minute of opening time.
        /// </summary>
        public byte? OpeningTimeMinute {
            get { return this.openingTimeMinute.Value; }
            set { this.openingTimeMinute.Value = value; }
        }
        private readonly PersistentFieldForNullableByte openingTimeMinute =
            new PersistentFieldForNullableByte(nameof(OpeningTimeMinute));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public OpeningHours()
            : base() {
            this.RegisterPersistentField(this.closingTimeHour);
            this.RegisterPersistentField(this.closingTimeMinute);
            this.RegisterPersistentField(this.dayOfWeek);
            this.RegisterPersistentField(this.openingTimeHour);
            this.RegisterPersistentField(this.openingTimeMinute);
        }

    }

}