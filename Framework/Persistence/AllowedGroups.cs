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

namespace Framework.Persistence {

    using Framework.Persistence.Fields;

    /// <summary>
    /// Permission set of allowed groups.
    /// </summary>
    public sealed class AllowedGroups : PersistentObject {

        /// <summary>
        /// Indicates whether allowed groups for reading or writing
        /// are set.
        /// </summary>
        public bool IsEmpty {
            get {
                return this.ForReading.Count < 1 && this.ForWriting.Count < 1;
            }
        }

        /// <summary>
        /// Allowed groups for reading.
        /// </summary>
        public PersistentFieldForGroupCollection ForReading { get; private set; }

        /// <summary>
        /// Allowed groups for writing.
        /// </summary>
        public PersistentFieldForGroupCollection ForWriting { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public AllowedGroups()
            : base() {
            this.AllowedGroups = this;
            this.ForReading = new PersistentFieldForGroupCollection(nameof(ForReading), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.ForReading);
            this.ForWriting = new PersistentFieldForGroupCollection(nameof(ForWriting), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.ForWriting);
        }

    }

}