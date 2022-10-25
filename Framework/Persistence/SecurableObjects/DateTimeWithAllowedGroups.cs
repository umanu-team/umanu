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

namespace Framework.Persistence.SecurableObjects {

    using System;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Encapsulated DateTime value with allowed groups.
    /// </summary>
    public sealed class DateTimeWithAllowedGroups : PersistentObject {

        /// <summary>
        /// Encapsulated value.
        /// </summary>
        public DateTime Value {
            get { return this.value.Value; }
            set { this.value.Value = value; }
        }
        private readonly PersistentFieldForDateTime value =
            new PersistentFieldForDateTime(nameof(Value));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public DateTimeWithAllowedGroups()
            : base() {
            this.RegisterPersistentField(this.value);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="value">initial value</param>
        public DateTimeWithAllowedGroups(DateTime value)
            : this() {
            this.Value = value;
        }

    }

}