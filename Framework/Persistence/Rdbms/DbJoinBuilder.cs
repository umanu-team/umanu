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

    using Framework.Persistence.Exceptions;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Join builder for resolving sub objects of filters. For
    /// performance reasons there is no automatic check for validity
    /// of filters. Invalid filters might cause an exception when
    /// used.
    /// </summary>
    public sealed class DbJoinBuilder {

        /// <summary>
        /// Converter for field names to be used in relational queries.
        /// </summary>
        private readonly IRelationalFieldNameConverter fieldNameConverter;

        /// <summary>
        /// List of relational joins.
        /// </summary>
        public ICollection<RelationalJoin> RelationalJoins { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="relationalJoins">list of relational joins</param>
        /// <param name="fieldNameConverter">converter for field
        /// names to be used in relational queries</param>
        public DbJoinBuilder(ICollection<RelationalJoin> relationalJoins, IRelationalFieldNameConverter fieldNameConverter) {
            this.fieldNameConverter = fieldNameConverter;
            this.RelationalJoins = relationalJoins;
        }

        /// <summary>
        /// Gets the internal field name for use in joins of a full
        /// field name chain.
        /// </summary>
        /// <param name="fullFieldName">full field name chain to get
        /// internal name for use in joins for</param>
        /// <returns>internal field name for use in joins or null if
        /// no join exists for it</returns>
        public string GetInternalFieldNameOf(string fullFieldName) {
            string internalFieldName;
            int indexOfLastDot = fullFieldName.LastIndexOf('.');
            if (indexOfLastDot < 0) {
                internalFieldName = RelationalJoin.RootTableAlias + '.' + fullFieldName;
                foreach (var relationalJoin in this.RelationalJoins) {
                    if (relationalJoin.FullFieldName == fullFieldName) {
                        internalFieldName = relationalJoin.TableAlias + ".Value";
                        break;
                    }
                }
            } else {
                internalFieldName = null;
                string fieldParentsChain = fullFieldName.Substring(0, indexOfLastDot);
                foreach (var relationalJoin in this.RelationalJoins) {
                    if (relationalJoin.FullFieldName == fullFieldName) {
                        internalFieldName = relationalJoin.TableAlias + ".Value";
                        break;
                    } else if (relationalJoin.FullFieldName == fieldParentsChain) {
                        string shortFieldName = fullFieldName.Substring(indexOfLastDot + 1);
                        internalFieldName = relationalJoin.TableAlias + '.' + shortFieldName;
                    }
                }
            }
            return internalFieldName;
        }

        /// <summary>
        /// Indicates whether a relational join for a full field name
        /// chain exists.
        /// </summary>
        /// <param name="fullFieldName">full field name chain to
        /// check existence of join for</param>
        /// <param name="isForSubTable">indicates whether relational
        /// join is supposed to be for a sub table</param>
        /// <returns>true if a relational join for a full field name
        /// exists, false otherwise</returns>
        public bool HasRelationalJoinFor(string fullFieldName, bool isForSubTable) {
            var hasRelationalJoin = false;
            if (!isForSubTable) {
                int indexOfLastDot = fullFieldName.LastIndexOf('.');
                if (indexOfLastDot < 0) {
                    foreach (var relationalJoin in this.RelationalJoins) {
                        if (relationalJoin.FullFieldName == fullFieldName) {
                            hasRelationalJoin = true;
                            break;
                        }
                    }
                } else {
                    string fieldParentsChain = fullFieldName.Substring(0, indexOfLastDot);
                    foreach (var relationalJoin in this.RelationalJoins) {
                        if (relationalJoin.FullFieldName == fullFieldName || relationalJoin.FullFieldName == fieldParentsChain) {
                            hasRelationalJoin = true;
                            break;
                        }
                    }
                }
            }
            return hasRelationalJoin;
        }

        /// <summary>
        /// Converts the enumerable of relational joins of this
        /// instance to an SQL JOIN clause.
        /// </summary>
        /// <param name="joinType">type of joins to be used</param>
        /// <returns>relational joins as SQL JOIN clause</returns>
        public string ToJoin(RelationalJoinType joinType) {
            var joinBuilder = new StringBuilder();
            foreach (var join in this.RelationalJoins) {
                if (RelationalJoinType.InnerJoin == joinType) {
                    joinBuilder.Append("INNER JOIN ");
                } else if (RelationalJoinType.LeftOuterJoin == joinType) {
                    joinBuilder.Append("LEFT OUTER JOIN ");
                } else {
                    throw new PersistenceException("Relational join type " + joinType.ToString() + " is not supported.");
                }
                joinBuilder.Append(join.TableName);
                joinBuilder.Append(" AS ");
                joinBuilder.Append(join.TableAlias);
                joinBuilder.Append(' ');
                bool isFirstPredicate = true;
                if (join.FieldPredicates.Count > 0) {
                    joinBuilder.Append("ON ");
                    foreach (var fieldPredicate in join.FieldPredicates) {
                        if (isFirstPredicate) {
                            isFirstPredicate = false;
                        } else {
                            joinBuilder.Append("AND ");
                        }
                        joinBuilder.Append(this.fieldNameConverter.Escape(fieldPredicate.Key));
                        joinBuilder.Append("=");
                        joinBuilder.Append(this.fieldNameConverter.Escape(fieldPredicate.Value));
                        joinBuilder.Append(' ');
                    }
                }
                if (join.StringPredicates.Count > 0) {
                    if (isFirstPredicate) {
                        joinBuilder.Append("ON ");
                    }
                    foreach (var stringPredicate in join.StringPredicates) {
                        if (isFirstPredicate) {
                            isFirstPredicate = false;
                        } else {
                            joinBuilder.Append("AND ");
                        }
                        joinBuilder.Append(this.fieldNameConverter.Escape(stringPredicate.Key));
                        joinBuilder.Append("='");
                        joinBuilder.Append(stringPredicate.Value);
                        joinBuilder.Append("' ");
                    }
                }
            }
            return joinBuilder.ToString();
        }

    }

}