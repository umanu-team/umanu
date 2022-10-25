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

namespace Framework.Persistence.Rdbms.MSSql {

    /// <summary>
    /// Converter for field names to be used in MS SQL queries.
    /// </summary>
    public sealed class MSSqlFieldNameConverter : IRelationalFieldNameConverter {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public MSSqlFieldNameConverter()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Escapes a field name to be used in relational queries.
        /// </summary>
        /// <param name="fieldName">field name to be escaped</param>
        /// <returns>field name to be used in relational queries</returns>
        public string Escape(string fieldName) {
            string escapedFieldName;
            var postDotPosition = fieldName.IndexOf('.') + 1;
            if (postDotPosition > 0) {
                escapedFieldName = fieldName.Substring(0, postDotPosition) + '"' + fieldName.Substring(postDotPosition) + '"';
            } else {
                escapedFieldName = '"' + fieldName + '"';
            }
            return escapedFieldName;
        }

    }

}