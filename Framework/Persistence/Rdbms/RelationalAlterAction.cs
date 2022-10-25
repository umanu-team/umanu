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

    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies what action happens to rows in a table that are
    /// altered, if those rows have a referential relationship.
    /// </summary>
    public enum RelationalAlterAction {

        /// <summary>
        /// The SQL Server Database Engine raises an error and the
        /// action on the row in the parent table is rolled back.
        /// </summary>
        [EnumMemberAttribute]
        NoAction = 0,

        /// <summary>
        /// Corresponding rows are altered in the referencing table
        /// if that row is altered the parent table.
        /// </summary>
        [EnumMemberAttribute]
        Cascade = 1,

        /// <summary>
        /// All the values that make up the foreign key are set to
        /// NULL when the corresponding row in the parent table is
        /// altered. For this constraint to execute, the foreign key
        /// columns must be nullable.
        /// </summary>
        [EnumMemberAttribute]
        SetNull = 2,

        /// <summary>
        /// All the values that make up the foreign key are set to
        /// their default values when the corresponding row in the
        /// parent table is altered. For this constraint to execute,
        /// all foreign key columns must have default definitions. If
        /// a column is nullable, and there is no explicit default
        /// value set, NULL becomes the implicit default value of the
        /// column.
        /// </summary>
        [EnumMemberAttribute]
        SetDefault = 3

    }

}