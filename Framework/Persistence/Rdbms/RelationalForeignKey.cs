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

    /// <summary>
    /// Foreign key to connect a column in a database table to a
    /// primary key column in another database table.
    /// </summary>
    public sealed class RelationalForeignKey {

        /// <summary>
        /// Name of column containing foreign key.
        /// </summary>
        public string ForeignKeyColumn { get; set; }

        /// <summary>
        /// Name of table containing foreign key.
        /// </summary>
        public string ForeignKeyTable { get; set; }

        /// <summary>
        /// True if foreign key is supposed to be indexed, false
        /// otherwise.
        /// </summary>
        public bool IsIndexed { get; set; }

        /// <summary>
        /// Specifies what action happens to rows in foreign key
        /// table that are deleted in primary key table.
        /// </summary>
        public RelationalAlterAction OnDelete { get; set; }

        /// <summary>
        /// Specifies what action happens to rows in foreign key
        /// table that are updated in primary key table.
        /// </summary>
        public RelationalAlterAction OnUpdate { get; set; }

        /// <summary>
        /// Name of column containing primary key.
        /// </summary>
        public string PrimaryKeyColumn { get; set; }

        /// <summary>
        /// Name of table containing primary key.
        /// </summary>
        public string PrimaryKeyTable { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="foreignKeyTable">name of table containing
        /// foreign key</param>
        /// <param name="foreignKeyColumn">name of column containing
        /// foreign key</param>
        /// <param name="primaryKeyTable">name of table containing
        /// primary key</param>
        /// <param name="primaryKeyColumn">name of column containing
        /// primary key</param>
        public RelationalForeignKey(string foreignKeyTable, string foreignKeyColumn, string primaryKeyTable, string primaryKeyColumn) {
            this.ForeignKeyTable = foreignKeyTable;
            this.ForeignKeyColumn = foreignKeyColumn;
            this.IsIndexed = true;
            this.OnDelete = RelationalAlterAction.NoAction;
            this.OnUpdate = RelationalAlterAction.NoAction;
            this.PrimaryKeyTable = primaryKeyTable;
            this.PrimaryKeyColumn = primaryKeyColumn;
        }

    }
}
