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

namespace Framework.Persistence.Directories {

    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Filter builder for building LDAP string filters. For
    /// performance reasons there is no automatic check for validity
    /// of filters. Invalid filters might cause an exception when
    /// used.
    /// </summary>
    internal sealed class ActiveDirectoryFilterBuilder : FilterBuilder<string> {

        /// <summary>
        /// Stack of count of open braces of sub filters.
        /// </summary>
        private Stack<ulong> openBracesCountStack;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to build
        /// filter for</param>
        public ActiveDirectoryFilterBuilder(FilterCriteria filterCriteria)
            : base() {
            this.openBracesCountStack = new Stack<ulong>();
            this.openBracesCountStack.Push(0);
            this.Initialize(filterCriteria);
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
            this.AppendFilterConnective(filterConnective);
            this.filterStringBuilder.Append('(');
            if (RelationalOperator.IsNotEqualTo == relationalOperator) {
                this.filterStringBuilder.Append("!(");
            }
            this.filterStringBuilder.Append(fieldName);
            this.AppendRelationalOperator(relationalOperator);
            this.AppendValueOrOtherFieldName(relationalOperator, valueOrOtherFieldName, filterTarget, contentBaseType);
            if (RelationalOperator.IsNotEqualTo == relationalOperator) {
                this.filterStringBuilder.Append(')');
            }
            this.filterStringBuilder.Append(')');
            return;
        }

        /// <summary>
        /// Closes the sub filter that has been started at last.
        /// </summary>
        protected override void AppendEndOfSubFilter() {
            ulong openBracesCount = this.openBracesCountStack.Pop();
            while (openBracesCount > 1) {
                this.filterStringBuilder.Append(')');
                openBracesCount--;
            }
            if (openBracesCount > 0) {
                openBracesCount = this.openBracesCountStack.Pop();
                this.openBracesCountStack.Push(openBracesCount + 1);
            }
            return;
        }


        /// <summary>
        /// Appends a filter connective to next filter.
        /// </summary>
        /// <param name="filterConnective">logical connective to
        /// previous filter</param>
        protected override void AppendFilterConnective(FilterConnective filterConnective) {
            if (FilterConnective.None != filterConnective) {
                this.filterStringBuilder.Append('(');
                ulong openBracesCount = this.openBracesCountStack.Pop();
                this.openBracesCountStack.Push(openBracesCount + 1);
                if (FilterConnective.And == filterConnective) {
                    this.filterStringBuilder.Append('&');
                } else if (FilterConnective.Or == filterConnective) {
                    this.filterStringBuilder.Append('|');
                } else {
                    throw new FilterException("Filter connective "
                        + filterConnective.ToString() + " is not supported.");
                }
            }
            return;
        }

        /// <summary>
        /// Opens a new sub filter.
        /// </summary>
        /// <param name="filterConnective">logical connective to
        /// previous filter</param>
        protected override void AppendStartOfSubFilter(FilterConnective filterConnective) {
            this.openBracesCountStack.Push(0);
            this.AppendFilterConnective(filterConnective);
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
                throw new FilterException("Comparison of fields to other fields is not supported for LDAP filters.");
            } else if (FilterTarget.IsOtherBoolValue == filterTarget) {
                bool boolValueIsTrue = (bool)valueOrOtherFieldName;
                if (boolValueIsTrue) {
                    this.filterStringBuilder.Append("true");
                } else {
                    this.filterStringBuilder.Append("false");
                }
            } else {
                string value;
                if (TypeOf.Guid == contentBaseType) {
                    byte[] byteGuid = ((Guid)valueOrOtherFieldName).ToByteArray();
                    StringBuilder octetStringBuilder = new StringBuilder(48);
                    foreach (byte b in byteGuid) {
                        octetStringBuilder.Append('\\');
                        octetStringBuilder.Append(b.ToString("x2"));
                    }
                    value = octetStringBuilder.ToString();
                } else {
                    value = valueOrOtherFieldName.ToString()
                        .Replace("*", "\\2a")
                        .Replace("(", "\\28")
                        .Replace(")", "\\29")
                        .Replace("\\", "\\5c");
                }
                if (RelationalOperator.Contains == relationalOperator) {
                    value = '*' + value + '*';
                } else if (RelationalOperator.EndsWith == relationalOperator) {
                    value = '*' + value;
                } else if (RelationalOperator.StartsWith == relationalOperator) {
                    value += '*';
                } else if (RelationalOperator.IsGreaterThan == relationalOperator
                    || RelationalOperator.IsLessThan == relationalOperator) {
                    sbyte summand;
                    if (RelationalOperator.IsGreaterThan == relationalOperator) {
                        summand = 1;
                    } else { // RelationalOperator.IsLessThan == relationalOperator
                        summand = -1;
                    }
                    if (TypeOf.Byte == contentBaseType
                        || TypeOf.Int == contentBaseType
                        || TypeOf.Long == contentBaseType
                        || TypeOf.SByte == contentBaseType
                        || TypeOf.Short == contentBaseType
                        || TypeOf.UInt == contentBaseType
                        || TypeOf.UShort == contentBaseType) {
                        value = (Convert.ToInt64(valueOrOtherFieldName, CultureInfo.InvariantCulture.NumberFormat) + summand).ToString(CultureInfo.InvariantCulture);
                    } else if (TypeOf.ULong == contentBaseType) {
                        ulong valueAsUlong = Convert.ToUInt64(valueOrOtherFieldName, CultureInfo.InvariantCulture.NumberFormat);
                        if (RelationalOperator.IsGreaterThan == relationalOperator) {
                            value = (valueAsUlong + 1).ToString(CultureInfo.InvariantCulture);
                        } else { // RelationalOperator.IsLessThan == relationalOperator
                            if (0 == valueAsUlong) {
                                value = "-1";
                            } else {
                                value = (valueAsUlong - 1).ToString(CultureInfo.InvariantCulture);
                            }
                        }
                    } else if (TypeOf.Decimal == contentBaseType) {
                        value = (Convert.ToDecimal(valueOrOtherFieldName, CultureInfo.InvariantCulture.NumberFormat) + summand).ToString(CultureInfo.InvariantCulture);
                    } else if (TypeOf.Double == contentBaseType
                        || TypeOf.Float == contentBaseType) {
                        value = (Convert.ToDouble(valueOrOtherFieldName, CultureInfo.InvariantCulture.NumberFormat) + summand).ToString(CultureInfo.InvariantCulture);
                    } else {
                        throw new FilterException("Invalid data type \""
                            + contentBaseType.ToString()
                            + "\" for comparison \""
                            + relationalOperator.ToString()
                            + "\" in filter.");
                    }
                }
                this.filterStringBuilder.Append(value);
            }
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
                || RelationalOperator.IsEqualTo == relationalOperator
                || RelationalOperator.IsNotEqualTo == relationalOperator
                || RelationalOperator.StartsWith == relationalOperator) {
                this.filterStringBuilder.Append('=');
            } else if (RelationalOperator.IsGreaterThan == relationalOperator
                || RelationalOperator.IsGreaterThanOrEqualTo == relationalOperator) {
                this.filterStringBuilder.Append(">=");
            } else if (RelationalOperator.IsLessThan == relationalOperator
                || RelationalOperator.IsLessThanOrEqualTo == relationalOperator) {
                this.filterStringBuilder.Append("<=");
            } else {
                throw new FilterException("Relational operator "
                    + relationalOperator.ToString() + " is not supported.");
            }
            return;
        }

        /// <summary>
        /// Converts the value of this instance to a default string
        /// filter.
        /// </summary>
        /// <returns>value of this instance as default string filter</returns>
        public override string ToFilter() {
            if (openBracesCountStack.Count > 1) {
                throw new FilterException("Filter can not be build because "
                    + openBracesCountStack.Count + " sub filters were not closed.");
            }
            StringBuilder missingBracesBuilder = new StringBuilder();
            ulong openBracesCount = this.openBracesCountStack.Peek();
            while (openBracesCount > 0) {
                missingBracesBuilder.Append(')');
                openBracesCount--;
            }
            return this.filterStringBuilder.ToString()
                + missingBracesBuilder.ToString();
        }

    }

}