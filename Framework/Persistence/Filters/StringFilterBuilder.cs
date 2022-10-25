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

    using Exceptions;
    using System;

    /// <summary>
    /// Filter builder for building default string filters. For
    /// performance reasons there is no automatic check for validity
    /// of filters. Invalid filters might cause an exception when
    /// used.
    /// </summary>
    public class StringFilterBuilder : FilterBuilder<string> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to build
        /// filter for</param>
        public StringFilterBuilder(FilterCriteria filterCriteria)
            : base() {
            this.Initialize(filterCriteria);
        }

        /// <summary>
        /// Appends the string representation of a relational operator.
        /// </summary>
        /// <param name="relationalOperator">relational operator</param>
        /// <returns>string representation of the relational operator</returns>
        protected override void AppendRelationalOperator(RelationalOperator relationalOperator) {
            this.filterStringBuilder.Append(' ');
            if (RelationalOperator.Contains == relationalOperator) {
                this.filterStringBuilder.Append("CONTAINS");
            } else if (RelationalOperator.EndsWith == relationalOperator) {
                this.filterStringBuilder.Append("ENDSWITH");
            } else if (RelationalOperator.IsEqualTo == relationalOperator) {
                this.filterStringBuilder.Append("==");
            } else if (RelationalOperator.IsGreaterThan == relationalOperator) {
                this.filterStringBuilder.Append('>');
            } else if (RelationalOperator.IsGreaterThanOrEqualTo == relationalOperator) {
                this.filterStringBuilder.Append(">=");
            } else if (RelationalOperator.IsLessThan == relationalOperator) {
                this.filterStringBuilder.Append('<');
            } else if (RelationalOperator.IsLessThanOrEqualTo == relationalOperator) {
                this.filterStringBuilder.Append("<=");
            } else if (RelationalOperator.IsNotEqualTo == relationalOperator) {
                this.filterStringBuilder.Append("!=");
            } else if (RelationalOperator.StartsWith == relationalOperator) {
                this.filterStringBuilder.Append("STARTSWITH");
            } else {
                throw new FilterException("Relational operator "
                    + relationalOperator.ToString() + " is not supported.");
            }
            this.filterStringBuilder.Append(' ');
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
            if (FilterTarget.IsOtherField == filterTarget) {
                this.filterStringBuilder.Append('[');
            } else if (FilterTarget.IsOtherTextValue == filterTarget) {
                this.filterStringBuilder.Append('\"');
            }
            if (FilterTarget.IsOtherBoolValue == filterTarget) {
                bool boolValueIsTrue = bool.Parse(valueOrOtherFieldName.ToString());
                if (boolValueIsTrue) {
                    this.filterStringBuilder.Append("TRUE");
                } else {
                    this.filterStringBuilder.Append("FALSE");
                }
            } else if (FilterTarget.IsOtherTextValue == filterTarget) {
                this.filterStringBuilder.Append(valueOrOtherFieldName.ToString().Replace("\"", "\\\""));
            } else {
                this.filterStringBuilder.Append(valueOrOtherFieldName.ToString());
            }
            if (FilterTarget.IsOtherField == filterTarget) {
                this.filterStringBuilder.Append(']');
            } else if (FilterTarget.IsOtherTextValue == filterTarget) {
                this.filterStringBuilder.Append('\"');
            }
            return;
        }

        /// <summary>
        /// Converts the value of this instance to a default string
        /// filter.
        /// </summary>
        /// <returns>value of this instance as default string filter</returns>
        public override string ToFilter() {
            return this.filterStringBuilder.ToString();
        }

    }

}
