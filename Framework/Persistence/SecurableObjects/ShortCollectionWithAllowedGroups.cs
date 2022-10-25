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

    using Framework.Persistence.Fields;

    /// <summary>
    /// Encapsulated short values with allowed groups.
    /// </summary>
    public sealed class ShortCollectionWithAllowedGroups : PersistentObject {

        /// <summary>
        /// Encapsulated values.
        /// </summary>
        public PersistentFieldForShortCollection Values { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ShortCollectionWithAllowedGroups()
            : base() {
            this.Values = new PersistentFieldForShortCollection(nameof(Values));
            this.RegisterPersistentField(this.Values);
        }

    }

}
