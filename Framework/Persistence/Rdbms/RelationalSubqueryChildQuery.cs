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
    /// Represents a child query of a relational subquery.
    /// </summary>
    public sealed class RelationalSubqueryChildQuery {

        /// <summary>
        /// Internal name of child table or view.
        /// </summary>
        public string InternalNameOfContainer { get; private set; }

        /// <summary>
        /// Indicates whether child query is for inline n:1 relation.
        /// </summary>
        public bool IsForInlineRelation { get; private set; }

        /// <summary>
        /// Related part of field name chain.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">related part of field name chain</param>
        /// <param name="internalNameOfContainer">internal name of
        /// child table or view</param>
        /// <param name="isForInlineRelation">indicates whether child
        /// query is for inline n:1 relation</param>
        public RelationalSubqueryChildQuery(string key, string internalNameOfContainer, bool isForInlineRelation)
            : base() {
            this.InternalNameOfContainer = internalNameOfContainer;
            this.IsForInlineRelation = isForInlineRelation;
            this.Key = key;
        }

    }

}