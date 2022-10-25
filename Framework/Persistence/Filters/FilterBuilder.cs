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

namespace Framework.Persistence.Filters {

    using Framework.Persistence.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Filter builder for building default filters. For performance
    /// reasons there is no automatic check for validity of filters.
    /// Invalid filters might cause an exception when used.
    /// </summary>
    /// <typeparam name="T">type of filter to build</typeparam>
    public abstract class FilterBuilder<T> {

        /// <summary>
        /// String builder for concatenation of filter parts.
        /// </summary>
        protected StringBuilder filterStringBuilder;

        /// <summary>
        /// Stack of connectives of sub filters. The topmost
        /// connective has to be attached as soon as the topmost sub
        /// filter ends.
        /// </summary>
        protected Stack<FilterConnective> subFilterConnectiveStack;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public FilterBuilder() {
            this.filterStringBuilder = new StringBuilder();
            this.subFilterConnectiveStack = new Stack<FilterConnective>();
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
        protected virtual void AppendCondition(string fieldName, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, FilterConnective filterConnective, Type contentBaseType) {
            this.AppendFieldName(fieldName);
            if (null == valueOrOtherFieldName) {
                this.AppendNullValueOrThrowException(relationalOperator, filterTarget);
            } else {
                this.AppendRelationalOperator(relationalOperator);
                this.AppendValueOrOtherFieldName(relationalOperator, valueOrOtherFieldName, filterTarget, contentBaseType);
            }
            this.AppendFilterConnective(filterConnective);
            return;
        }

        /// <summary>
        /// Closes the sub filter that has been started at last.
        /// </summary>
        protected virtual void AppendEndOfSubFilter() {
            this.filterStringBuilder.Append(')');
            this.AppendFilterConnective(this.subFilterConnectiveStack.Pop());
            return;
        }

        /// <summary>
        /// Appends a name of field.
        /// </summary>
        /// <param name="fieldName">name of field</param>
        protected virtual void AppendFieldName(string fieldName) {
            this.filterStringBuilder.Append('[');
            this.filterStringBuilder.Append(fieldName);
            this.filterStringBuilder.Append(']');
            return;
        }

        /// <summary>
        /// Appends a filter connective to next filter.
        /// </summary>
        /// <param name="filterConnective">logical connective to
        /// previous filter</param>
        protected virtual void AppendFilterConnective(FilterConnective filterConnective) {
            if (FilterConnective.None != filterConnective) {
                this.filterStringBuilder.Append(' ');
                if (FilterConnective.And == filterConnective) {
                    this.filterStringBuilder.Append("AND");
                } else if (FilterConnective.Or == filterConnective) {
                    this.filterStringBuilder.Append("OR");
                } else {
                    throw new FilterException("Filter connective "
                        + filterConnective.ToString() + " is not supported.");
                }
                this.filterStringBuilder.Append(' ');
            }
            return;
        }

        /// <summary>
        /// Appends a comparison to NULL.
        /// </summary>
        protected virtual void AppendIsNull() {
            this.filterStringBuilder.Append(" == NULL");
            return;
        }

        /// <summary>
        /// Appends a comparison to NOT NULL.
        /// </summary>
        protected virtual void AppendIsNotNull() {
            this.filterStringBuilder.Append(" != NULL");
            return;
        }

        /// <summary>
        /// Appends a null value or throws an exception if filter
        /// target does not allow null values.
        /// </summary>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="filterTarget">determines whether filter
        /// target is another value or the name of another field</param>
        protected void AppendNullValueOrThrowException(RelationalOperator relationalOperator, FilterTarget filterTarget) {
            if (FilterTarget.IsOtherTextValue == filterTarget) {
                if (RelationalOperator.IsEqualTo == relationalOperator) {
                    this.AppendIsNull();
                } else if (RelationalOperator.IsNotEqualTo == relationalOperator) {
                    this.AppendIsNotNull();
                } else {
                    throw new PersistenceException("Relational operator " + relationalOperator.ToString() + " is not supported in combination with null values.");
                }
            } else {
                throw new PersistenceException("Filter target " + filterTarget.ToString() + " is not supported in combination with null values.");
            }
            return;
        }

        /// <summary>
        /// Opens a new sub filter.
        /// </summary>
        /// <param name="filterConnective">logical connective to
        /// previous filter</param>
        protected virtual void AppendStartOfSubFilter(FilterConnective filterConnective) {
            this.subFilterConnectiveStack.Push(filterConnective);
            this.filterStringBuilder.Append('(');
            return;
        }

        /// <summary>
        /// Appends the string representation of a relational operator.
        /// </summary>
        /// <param name="relationalOperator">relational operator</param>
        /// <returns>string representation of the relational operator</returns>
        protected abstract void AppendRelationalOperator(RelationalOperator relationalOperator);

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
        protected abstract void AppendValueOrOtherFieldName(RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, Type contentBaseType);

        /// <summary>
        /// Initializes filter builder with filter chain.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to build
        /// filter for</param>
        protected virtual void Initialize(FilterCriteria filterCriteria) {
            while (null != filterCriteria && !filterCriteria.IsEmpty) {
                var connective = FilterConnective.None;
                if (!filterCriteria.IsLastFilterOfChain) {
                    connective = filterCriteria.Next.Connective;
                }
                if (filterCriteria.HasSubFilterCriteria) {
                    this.AppendStartOfSubFilter(connective);
                    this.Initialize(filterCriteria.SubFilterCriteria);
                    this.AppendEndOfSubFilter();
                } else {
                    this.AppendCondition(filterCriteria.FieldName, filterCriteria.RelationalOperator, filterCriteria.ValueOrOtherFieldName, filterCriteria.FilterTarget, connective, filterCriteria.ContentBaseType);
                }
                filterCriteria = filterCriteria.Next;
            }
            return;
        }

        /// <summary>
        /// Converts the value of this instance to a default filter.
        /// </summary>
        /// <returns>value of this instance as default filter</returns>
        public abstract T ToFilter();

    }

}