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

namespace Framework.Persistence.Rdbms {

    using System.Collections.Generic;

    /// <summary>
    /// View for a sub table of a relational database.
    /// </summary>
    internal sealed class RelationalViewForSubTable : RelationalViewForTable {

        /// <summary>
        /// Name of this view.
        /// </summary>
        public override string Name {
            get { return RelationalDatabase.GetViewNameForSubTable(this.TableName); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">related persistence
        /// mechanism</param>
        public RelationalViewForSubTable(PersistenceMechanism persistenceMechanism)
            : base(persistenceMechanism) {
            // nothing to do
        }

        /// <summary>
        /// Gets an enumerable of fields this view is for.
        /// </summary>
        /// <returns>enumerable of fields this view is for</returns>
        protected override IEnumerable<string> GetFieldNames() {
            yield return nameof(PersistentObject.Id);
            yield return "ParentID";
            yield return "ParentIndex";
            yield return "Value";
        }

    }

}