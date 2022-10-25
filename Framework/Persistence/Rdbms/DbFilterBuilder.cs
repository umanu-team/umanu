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

    using Framework.Model;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// Filter builder for building SQL filters. For performance
    /// reasons there is no automatic check for validity of filters.
    /// Invalid filters might cause an exception when used.
    /// </summary>
    internal sealed class DbFilterBuilder : FilterBuilder<DbCommandFilter> {

        /// <summary>
        /// Filter with parameters for SQL command.
        /// </summary>
        private readonly DbCommandFilter commandFilter;

        /// <summary>
        /// Mapper for mapping .NET data types to data types of the
        /// RDBMS.
        /// </summary>
        private readonly DataTypeMapper dataTypeMapper;

        /// <summary>
        /// Converter for field names to be used in relational queries.
        /// </summary>
        private readonly IRelationalFieldNameConverter fieldNameConverter;

        /// <summary>
        /// True if relational subqueries are used, false otherwise.
        /// </summary>
        private readonly bool hasRelationalSubqueries;

        /// <summary>
        /// Join builder to use for lookup of internal names of
        /// containers.
        /// </summary>
        private readonly DbJoinBuilder joinBuilder;

        /// <summary>
        /// Number of parameters used.
        /// </summary>
        private ulong parameterCount;

        /// <summary>
        /// Collection of relational subqueries.
        /// </summary>
        private readonly RelationalSubqueryCollection relationalSubqueries;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to build
        /// filter for</param>
        /// <param name="fieldNameConverter">converter for field
        /// names to be used in relational queries</param>
        /// <param name="dataTypeMapper">mapper for mapping .NET data
        /// types to data types of the RDBMS</param>
        /// <param name="joinBuilder">join builder to use for lookup
        /// of internal names of containers</param>
        /// <param name="relationalSubqueries">collection of
        /// relational subqueries</param>
        public DbFilterBuilder(FilterCriteria filterCriteria, IRelationalFieldNameConverter fieldNameConverter, DataTypeMapper dataTypeMapper, DbJoinBuilder joinBuilder, RelationalSubqueryCollection relationalSubqueries)
            : base() {
            this.commandFilter = new DbCommandFilter();
            this.dataTypeMapper = dataTypeMapper;
            this.fieldNameConverter = fieldNameConverter;
            this.hasRelationalSubqueries = relationalSubqueries.Count > 0;
            this.joinBuilder = joinBuilder;
            this.parameterCount = 0;
            this.relationalSubqueries = relationalSubqueries;
            var orderedFilterCriteria = filterCriteria.Sort();
            this.Initialize(orderedFilterCriteria);
        }

        /// <summary>
        /// Appends a new single condition to this filter. The
        /// condition can be connected to further conditions by
        /// calling this method multiple times.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to use for comparison</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        /// <param name="filterConnective">logical connective to
        /// previous filter</param>
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        protected override void AppendCondition(string fieldName, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, FilterConnective filterConnective, Type contentBaseType) {
            throw new NotSupportedException("Method " + nameof(AppendCondition) + " is not supported for " + nameof(DbFilterBuilder) + ". Please use method " + nameof(AppendConditions) + " instead.");
        }

        /// <summary>
        /// Appends a new conditions for an inline comparison of an
        /// ID. This would work by calling
        /// AppendConditionsAsSubqueries(...) as well. However, this
        /// variant is a performance optimization.
        /// </summary>
        /// <param name="filterCondition">filter condition to be
        /// appended</param>
        private void AppendConditionAsInlineIdComparison(FilterCondition filterCondition) {
            this.filterStringBuilder.Append(RelationalJoin.RootTableAlias);
            this.filterStringBuilder.Append('.');
            this.filterStringBuilder.Append(this.fieldNameConverter.Escape(filterCondition.FieldNameChain[0]));
            this.AppendRelationalOperator(filterCondition.RelationalOperator);
            this.AppendValue(filterCondition.RelationalOperator, filterCondition.ValueOrOtherFieldName, filterCondition.ContentBaseType);
            this.AppendFilterConnective(filterCondition.FilterConnective);
            return;
        }

        /// <summary>
        /// Appends multiple new conditions for the same parent field
        /// to this filter. The conditions can be connected to
        /// further conditions by calling this method multiple times.
        /// </summary>
        /// <param name="filterConditions">filter conditions to be
        /// appended</param>
        private void AppendConditions(LinkedList<FilterCondition> filterConditions) {
            var firstFilterCondition = filterConditions.First?.Value;
            if (null != firstFilterCondition) {
                if (this.hasRelationalSubqueries && !this.joinBuilder.HasRelationalJoinFor(firstFilterCondition.FieldName, this.relationalSubqueries[firstFilterCondition.FieldName].IsForSubTable)) {
                    var relationalSubquery = this.relationalSubqueries[firstFilterCondition.FieldName];
                    if (1 == filterConditions.Count && RelationalOperator.IsEqualTo == firstFilterCondition.RelationalOperator && !relationalSubquery.IsForSubTable && 1 == relationalSubquery.ChildQueries.Count && relationalSubquery.ChildQueries[0].IsForInlineRelation && 2L == firstFilterCondition.FieldNameChain.LongLength && nameof(PersistentObject.Id).ToUpperInvariant() == firstFilterCondition.FieldNameChain[1].ToUpperInvariant()) {
                        this.AppendConditionAsInlineIdComparison(firstFilterCondition); // performance optimization
                    } else {
                        this.AppendConditionsAsSubqueries(filterConditions, relationalSubquery);
                    }
                } else {
                    this.AppendConditionsAsJoins(filterConditions);
                }
            }
            return;
        }

        /// <summary>
        /// Appends multiple new conditions for the same parent field
        /// to this filter as joins.
        /// </summary>
        /// <param name="filterConditions">filter conditions to be
        /// appended</param>
        private void AppendConditionsAsJoins(LinkedList<FilterCondition> filterConditions) {
            foreach (var filterCondition in filterConditions) {
                base.AppendCondition(filterCondition.FieldName, filterCondition.RelationalOperator, filterCondition.ValueOrOtherFieldName, filterCondition.FilterTarget, filterCondition.FilterConnective, filterCondition.ContentBaseType);
            }
        }

        /// <summary>
        /// Appends multiple new conditions for the same parent field
        /// to this filter as subqueries.
        /// </summary>
        /// <param name="filterConditions">filter conditions to be
        /// appended</param>
        /// <param name="relationalSubquery">common relational
        /// subquery for filter conditions</param>
        private void AppendConditionsAsSubqueries(LinkedList<FilterCondition> filterConditions, RelationalSubquery relationalSubquery) {
            bool isFilterConditionForEqualsNullWithChildViews = false;
            bool isFirstChildView = true;
            foreach (var childQuery in relationalSubquery.ChildQueries) {
                string fieldNameQualifier;
                if (isFirstChildView) {
                    fieldNameQualifier = RelationalJoin.RootTableAlias + '.';
                } else {
                    fieldNameQualifier = string.Empty;
                }
                if (childQuery.IsForInlineRelation) {
                    this.filterStringBuilder.Append(this.fieldNameConverter.Escape(fieldNameQualifier + childQuery.Key));
                } else {
                    this.filterStringBuilder.Append(fieldNameQualifier + nameof(PersistentObject.Id));
                    if (isFirstChildView) {
                        isFilterConditionForEqualsNullWithChildViews = filterConditions.Last.Value.IsForEqualsNull;
                        if (isFilterConditionForEqualsNullWithChildViews) {
                            this.filterStringBuilder.Append(" NOT"); //  double negate comparison for IS NULL
                        }
                    }
                    this.filterStringBuilder.Append(" IN (SELECT ParentID FROM ");
                    this.filterStringBuilder.Append(RelationalDatabase.CollectionRelationsTableName);
                    this.filterStringBuilder.Append(" WHERE ParentField='");
                    this.filterStringBuilder.Append(childQuery.Key);
                    this.filterStringBuilder.Append("' AND ChildID");
                }
                this.filterStringBuilder.Append(" IN (SELECT ");
                this.filterStringBuilder.Append(nameof(PersistentObject.Id));
                this.filterStringBuilder.Append(" FROM ");
                this.filterStringBuilder.Append(childQuery.InternalNameOfContainer);
                this.filterStringBuilder.Append(" WHERE ");
                isFirstChildView = false;
            }
            if (relationalSubquery.IsForSubTable) {
                if (isFirstChildView) {
                    this.filterStringBuilder.Append(RelationalJoin.RootTableAlias);
                    this.filterStringBuilder.Append('.');
                }
                this.filterStringBuilder.Append(nameof(PersistentObject.Id));
                this.filterStringBuilder.Append(" IN (SELECT ParentID FROM ");
                this.filterStringBuilder.Append(relationalSubquery.SubTableView);
                this.filterStringBuilder.Append(" WHERE ");
            }
            foreach (var filterCondition in filterConditions) {
                bool isCoalesceQuery = RelationalOperator.IsNotEqualTo == filterCondition.RelationalOperator && null != filterCondition.ValueOrOtherFieldName;
                if (isCoalesceQuery) {
                    this.filterStringBuilder.Append("COALESCE("); // for handling NULL values in DB as not equal
                }
                if (relationalSubquery.IsForSubTable) {
                    this.filterStringBuilder.Append("Value");
                } else {
                    this.AppendFieldName(filterCondition.FieldName);
                }
                if (isCoalesceQuery) {
                    this.filterStringBuilder.Append(",'')"); // for handling NULL values in DB as not equal
                }
                if (null == filterCondition.ValueOrOtherFieldName) {
                    RelationalOperator relationalOperator;
                    if (isFilterConditionForEqualsNullWithChildViews) {
                        relationalOperator = RelationalOperator.IsNotEqualTo; //  double negate comparison for IS NULL
                    } else {
                        relationalOperator = filterCondition.RelationalOperator;
                    }
                    this.AppendNullValueOrThrowException(relationalOperator, filterCondition.FilterTarget);
                } else {
                    this.AppendRelationalOperator(filterCondition.RelationalOperator);
                    this.AppendValueOrOtherFieldName(filterCondition.RelationalOperator, filterCondition.ValueOrOtherFieldName, filterCondition.FilterTarget, filterCondition.ContentBaseType);
                }
                if (filterCondition == filterConditions.Last.Value) {
                    if (relationalSubquery.IsForSubTable) {
                        this.filterStringBuilder.Append(')');
                    }
                    foreach (var childQuery in relationalSubquery.ChildQueries) {
                        this.filterStringBuilder.Append(')');
                        if (!childQuery.IsForInlineRelation) {
                            this.filterStringBuilder.Append(')');
                        }
                    }
                }
                this.AppendFilterConnective(filterCondition.FilterConnective);
            }
            return;
        }

        /// <summary>
        /// Appends multiple new conditions for the same parent field
        /// to this filter and clears the list of filter conditions
        /// afterwards. The conditions can be connected to further
        /// conditions by calling this method multiple times.
        /// </summary>
        /// <param name="filterConditions">filter conditions to be
        /// appended</param>
        private void AppendConditionsAndClear(LinkedList<FilterCondition> filterConditions) {
            this.AppendConditions(filterConditions);
            filterConditions.Clear();
            return;
        }

        /// <summary>
        /// Appends a name of field.
        /// </summary>
        /// <param name="fieldName">name of field</param>
        protected override void AppendFieldName(string fieldName) {
            string internalFieldName = this.joinBuilder.GetInternalFieldNameOf(fieldName);
            if (string.IsNullOrEmpty(internalFieldName)) {
                var keyChain = KeyChain.FromKey(fieldName);
                internalFieldName = keyChain[keyChain.LongLength - 1];
            }
            this.filterStringBuilder.Append(this.fieldNameConverter.Escape(internalFieldName));
            return;
        }

        /// <summary>
        /// Appends a comparison to NULL.
        /// </summary>
        protected override void AppendIsNull() {
            this.filterStringBuilder.Append(" IS NULL");
            return;
        }

        /// <summary>
        /// Appends a comparison to NOT NULL.
        /// </summary>
        protected override void AppendIsNotNull() {
            this.filterStringBuilder.Append(" IS NOT NULL");
            return;
        }

        /// <summary>
        /// Appends the string representation of a relational operator.
        /// </summary>
        /// <param name="relationalOperator">relational operator</param>
        /// <returns>string representation of the relational operator</returns>
        protected override void AppendRelationalOperator(RelationalOperator relationalOperator) {
            if (RelationalOperator.Contains == relationalOperator
                || RelationalOperator.EndsWith == relationalOperator
                || RelationalOperator.StartsWith == relationalOperator) {
                this.filterStringBuilder.Append(" LIKE ");
            } else if (RelationalOperator.IsEqualTo == relationalOperator) {
                this.filterStringBuilder.Append("=");
            } else if (RelationalOperator.IsGreaterThan == relationalOperator) {
                this.filterStringBuilder.Append('>');
            } else if (RelationalOperator.IsGreaterThanOrEqualTo == relationalOperator) {
                this.filterStringBuilder.Append(">=");
            } else if (RelationalOperator.IsLessThan == relationalOperator) {
                this.filterStringBuilder.Append('<');
            } else if (RelationalOperator.IsLessThanOrEqualTo == relationalOperator) {
                this.filterStringBuilder.Append("<=");
            } else if (RelationalOperator.IsNotEqualTo == relationalOperator) {
                this.filterStringBuilder.Append("<>");
            } else {
                throw new FilterException("Relational operator "
                    + relationalOperator.ToString() + " is not supported.");
            }
            return;
        }

        /// <summary>
        /// Appends a value.
        /// </summary>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to be appended</param>
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        private void AppendValue(RelationalOperator relationalOperator, object value, Type contentBaseType) {
            bool isLikeOperator = RelationalOperator.Contains == relationalOperator
                || RelationalOperator.EndsWith == relationalOperator
                || RelationalOperator.StartsWith == relationalOperator;
            if (isLikeOperator && null != value) {
                var stringValue = value.ToString();
                stringValue = stringValue
                    .Replace("%", "\a%")
                    .Replace("_", "\a_")
                    .Replace("[", "\a[")
                    .Replace("]", "\a]")
                    .Replace("^", "\a^");
                if (RelationalOperator.Contains == relationalOperator
                    || RelationalOperator.EndsWith == relationalOperator) {
                    stringValue = "%" + stringValue;
                }
                if (RelationalOperator.Contains == relationalOperator
                    || RelationalOperator.StartsWith == relationalOperator) {
                    stringValue = stringValue + "%";
                }
                value = stringValue;
            }
            this.parameterCount++;
            var parameter = this.dataTypeMapper.CreateDbParameter("@p" + this.parameterCount, value, contentBaseType);
            this.filterStringBuilder.Append(parameter.ParameterName);
            if (isLikeOperator) {
                this.filterStringBuilder.Append(" ESCAPE '\a'");
            }
            this.commandFilter.Parameters.Add(parameter);
            return;
        }

        /// <summary>
        /// Appends a value or the name of another field.
        /// </summary>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        protected override void AppendValueOrOtherFieldName(RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, Type contentBaseType) {
            if (FilterTarget.IsOtherBoolValue == filterTarget) {
                bool boolValueIsTrue = (bool)valueOrOtherFieldName;
                if (boolValueIsTrue) {
                    this.filterStringBuilder.Append('1');
                } else {
                    this.filterStringBuilder.Append('0');
                }
            } else if (FilterTarget.IsOtherField == filterTarget) {
                this.AppendFieldName(PersistentField.ValidateKey(valueOrOtherFieldName.ToString()));
            } else {
                this.AppendValue(relationalOperator, valueOrOtherFieldName, contentBaseType);
            }
            return;
        }

        /// <summary>
        /// Applies parameters to command text, so that no parameters
        /// are contained in command text any more.
        /// </summary>
        /// <param name="commandText">command text to apply
        /// parameters to</param>
        /// <param name="parameters">parameters to be applied</param>
        /// <returns>command text with applied parameters</returns>
        public static string ApplyParameters(string commandText, IEnumerable<DbParameter> parameters) {
            if (null != parameters) {
                var sortedParameters = new List<DbParameter>(parameters); // sorting is necessary to avoid @p12 to be replaced by value of @p1 + "2"
                sortedParameters.Sort(delegate (DbParameter a, DbParameter b) {
                    return b.ParameterName.Length.CompareTo(a.ParameterName.Length);
                });
                foreach (var parameter in sortedParameters) {
                    string value = parameter.Value.ToString();
                    if (DbType.AnsiString == parameter.DbType
                        || DbType.AnsiStringFixedLength == parameter.DbType
                        || DbType.Date == parameter.DbType
                        || DbType.DateTime == parameter.DbType
                        || DbType.DateTime2 == parameter.DbType
                        || DbType.DateTimeOffset == parameter.DbType
                        || DbType.Guid == parameter.DbType
                        || DbType.String == parameter.DbType
                        || DbType.StringFixedLength == parameter.DbType
                        || DbType.Xml == parameter.DbType) {
                        value = "'" + value + "'";
                    }
                    commandText = commandText.Replace(parameter.ParameterName, value);
                }
            }
            return commandText;
        }

        /// <summary>
        /// Initializes filter builder with filter chain.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to build
        /// filter for</param>
        protected override void Initialize(FilterCriteria filterCriteria) {
            var filterConditions = new LinkedList<FilterCondition>();
            while (null != filterCriteria && !filterCriteria.IsEmpty) {
                var connective = FilterConnective.None;
                if (!filterCriteria.IsLastFilterOfChain) {
                    connective = filterCriteria.Next.Connective;
                }
                if (filterCriteria.HasSubFilterCriteria) {
                    this.AppendConditionsAndClear(filterConditions);
                    this.AppendStartOfSubFilter(connective);
                    this.Initialize(filterCriteria.SubFilterCriteria);
                    this.AppendEndOfSubFilter();
                } else {
                    var previousFilterCondition = filterConditions.Last?.Value;
                    var currentFilterCondition = new FilterCondition(filterCriteria.FieldName, filterCriteria.RelationalOperator, filterCriteria.ValueOrOtherFieldName, filterCriteria.FilterTarget, connective, filterCriteria.ContentBaseType);
                    if (currentFilterCondition.IsForEqualsNull || true == previousFilterCondition?.IsForEqualsNull) {
                        this.AppendConditionsAndClear(filterConditions);
                    } else if (this.hasRelationalSubqueries && null != previousFilterCondition) {
                        bool isPreviousFilterForSubTable = this.relationalSubqueries[previousFilterCondition.FieldName].IsForSubTable;
                        bool isCurrentFilterForSubTable = this.relationalSubqueries[currentFilterCondition.FieldName].IsForSubTable;
                        if (isPreviousFilterForSubTable != isCurrentFilterForSubTable
                            || (isCurrentFilterForSubTable && previousFilterCondition.FieldName != currentFilterCondition.FieldName)
                            || (!isCurrentFilterForSubTable && previousFilterCondition.FieldNameChainOfParent != currentFilterCondition.FieldNameChainOfParent)) {
                            this.AppendConditionsAndClear(filterConditions);
                        }
                    }
                    filterConditions.AddLast(currentFilterCondition);
                }
                filterCriteria = filterCriteria.Next;
            }
            this.AppendConditionsAndClear(filterConditions);
            return;
        }

        /// <summary>
        /// Converts the value of this instance to an SQL WHERE
        /// clause.
        /// </summary>
        /// <returns>value of this instance as SQL WHERE clause</returns>
        public override DbCommandFilter ToFilter() {
            this.commandFilter.Filter = this.filterStringBuilder.ToString();
            return this.commandFilter;
        }

    }

}