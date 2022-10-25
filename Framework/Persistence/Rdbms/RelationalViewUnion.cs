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

    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a relational union as part of a relational view.
    /// </summary>
    public sealed class RelationalViewUnion {

        /// <summary>
        /// List of pairs of target column name of view as key and
        /// column name of source table as value for dynamic columns.
        /// </summary>
        public IList<KeyValuePair<string, string>> DynamicColumns { get; private set; }

        /// <summary>
        /// Filter criteria to be applied for union.
        /// </summary>
        public FilterCriteria FilterCriteria { get; set; }

        /// <summary>
        /// List of tuples representing static columns for view with
        /// column name of view as Item1, value as Item2 and type as
        /// Item3.
        /// </summary>
        public IList<Tuple<string, string, Type>> StaticColumns { get; set; }

        /// <summary>
        /// Name of source table.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tableName">name of source table</param>
        public RelationalViewUnion(string tableName)
            : base() {
            this.DynamicColumns = new List<KeyValuePair<string, string>>();
            this.FilterCriteria = FilterCriteria.Empty;
            this.StaticColumns = new List<Tuple<string, string, Type>>();
            this.TableName = tableName;
        }

    }

}
