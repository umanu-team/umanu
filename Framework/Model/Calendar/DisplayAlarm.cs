﻿/*********************************************************************
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

    using Framework.Persistence.Fields;

    /// <summary>
    /// Represents an iCalendar display alarm as defined in RFC5545.
    /// </summary>
    public class DisplayAlarm : Alarm {

        /// <summary>
        /// Description (DESCRIPTION).
        /// This property provides a  description of the alarm.
        /// </summary>
        public string Description {
            get { return this.description.Value; }
            set { this.description.Value = value; }
        }
        private readonly PersistentFieldForString description =
            new PersistentFieldForString(nameof(Description));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public DisplayAlarm()
            : base() {
            this.RegisterPersistentField(this.description);
        }

    }

}