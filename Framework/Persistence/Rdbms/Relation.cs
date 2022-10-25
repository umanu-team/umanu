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

    using System;
    using System.Data.Common;

    /// <summary>
    /// Represents a relation in a relational database.
    /// </summary>
    public sealed class Relation {

        /// <summary>
        /// ID of allowed groups of child object.
        /// </summary>
        public Guid? ChildAllowedGroupsId { get; set; }

        /// <summary>
        /// ID of child object.
        /// </summary>
        public Guid ChildId { get; set; }

        /// <summary>
        /// Type of child object.
        /// </summary>
        public string ChildType { get; set; }

        /// <summary>
        /// ID of parent object.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Relation()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the value of the specified column as nullable GUID.
        /// </summary>
        /// <param name="dataReader">data reader to get value form</param>
        /// <param name="ordinal">zero based column ordinal to get
        /// value from</param>
        /// <returns>value of specified column as nullable GUID</returns>
        public static Guid? GetNullableGuid(DbDataReader dataReader, int ordinal) {
            Guid? nullableGuid;
            if (dataReader.IsDBNull(ordinal)) {
                nullableGuid = null;
            } else {
                nullableGuid = dataReader.GetGuid(ordinal);
            }
            return nullableGuid;
        }

    }

}