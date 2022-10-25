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
    using System.Collections.Generic;

    /// <summary>
    /// Represents a relational join for merging tables.
    /// </summary>
    public sealed class RelationalJoin : IEquatable<RelationalJoin> {

        /// <summary>
        /// Field predicates to join columns on.
        /// </summary>
        public Dictionary<string, string> FieldPredicates { get; private set; }

        /// <summary>
        /// Full name chain of field this join is for.
        /// </summary>
        public string FullFieldName { get; set; }

        /// <summary>
        /// Default alias of root table of command.
        /// </summary>
        public const string RootTableAlias = "r0";

        /// <summary>
        /// String predicates to join columns on.
        /// </summary>
        public Dictionary<string, string> StringPredicates { get; private set; }

        /// <summary>
        /// Alias of table to join - may be null or empty.
        /// </summary>
        public string TableAlias { get; set; }

        /// <summary>
        /// Name of table to join.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tableName">name of table to join</param>
        /// <param name="tableAlias">alias of table to join</param>
        public RelationalJoin(string tableName, string tableAlias) {
            this.FieldPredicates = new Dictionary<string, string>();
            this.StringPredicates = new Dictionary<string, string>();
            this.TableAlias = tableAlias;
            this.TableName = tableName;
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type either by full field name or, if
        /// full field name is null, by all properties.
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal, false otherwise</returns>
        public bool Equals(RelationalJoin other) {
            bool isEqual;
            if (null == other) {
                isEqual = false;
            } else if (string.IsNullOrEmpty(this.FullFieldName)) {
                isEqual = this.FullFieldName == other.FullFieldName
                    && this.TableAlias == other.TableAlias
                    && this.TableName == other.TableName
                    && this.FieldPredicates.Count == other.FieldPredicates.Count
                    && this.StringPredicates.Count == other.StringPredicates.Count;
                if (isEqual) {
                    foreach (var predicate in this.FieldPredicates) {
                        if (!other.FieldPredicates.ContainsKey(predicate.Key) || other.FieldPredicates[predicate.Key] != predicate.Value) {
                            isEqual = false;
                            break;
                        }
                    }
                }
                if (isEqual) {
                    foreach (var predicate in this.StringPredicates) {
                        if (!other.StringPredicates.ContainsKey(predicate.Key) || other.StringPredicates[predicate.Key] != predicate.Value) {
                            isEqual = false;
                            break;
                        }
                    }
                }
            } else { // !string.IsNullOrEmpty(this.FullFieldName)
                isEqual = this.FullFieldName.Equals(other.FullFieldName, StringComparison.Ordinal);
            }
            return isEqual;
        }

    }

}