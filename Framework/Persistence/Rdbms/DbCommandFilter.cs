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
    using System.Data.Common;

    /// <summary>
    /// Filter with parameters for command of RDBMS database.
    /// </summary>
    internal sealed class DbCommandFilter {

        /// <summary>
        /// Gets or sets the RDBMS filter statement.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// List of parameters associated with this
        /// SqlCommandFilter and their respective mappings to
        /// columns.
        /// </summary>
        public IList<DbParameter> Parameters { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public DbCommandFilter() {
            this.Parameters = new List<DbParameter>();
        }

        /// <summary>
        /// Returns the SQL filter as a string.
        /// </summary>
        /// <returns>SQL filter as string</returns>
        public override string ToString() {
            string filter = this.Filter;
            foreach (var parameter in this.Parameters) {
                string value = parameter.Value.ToString();
                if (parameter.Value is string) {
                    value = "\'" + value + "\'";
                }
                filter = filter.Replace(parameter.ParameterName, value);
            }
            return filter;
        }

    }

}
