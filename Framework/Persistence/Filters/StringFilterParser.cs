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

    /// <summary>
    /// Filter parser for parsing default string filters.
    /// </summary>
    public class StringFilterParser {

        /// <summary>
        /// Filter to parse.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="filter">filter to parse</param>
        public StringFilterParser(string filter)
            : base() {
            this.Filter = filter;
        }

        /// <summary>
        /// Parses a sub filter and returns it as FilterCriteria.
        /// </summary>
        /// <param name="filter">sub filter to parse</param>
        /// <returns>sub filter as FilterCriteria</returns>
        protected static FilterCriteria GetFilterCriteriaFromSubFilter(string filter) {
            FilterCriteria filterCriteria = FilterCriteria.Empty;
            FilterConnective filterConnective = FilterConnective.None;
            string remainingFilter = filter.Trim();
            while (remainingFilter.Length > 0) {
                int pos = 0;
                if ('(' == remainingFilter[0]) {
                    // sub filter
                    FilterCriteria subFilterCriteria =
                        StringFilterParser.ParseSubFilter(remainingFilter, out pos);
                    // add to filter criteria
                    if (FilterConnective.None == filterConnective) {
                        filterCriteria = new FilterCriteria(subFilterCriteria);
                    } else if (FilterConnective.And == filterConnective) {
                        filterCriteria = filterCriteria.And(subFilterCriteria);
                    } else if (FilterConnective.Or == filterConnective) {
                        filterCriteria = filterCriteria.Or(subFilterCriteria);
                    }
                    remainingFilter = StringFilterParser.GetRemainingFilter(remainingFilter, pos);
                } else if ('[' == remainingFilter[0]) {
                    try {
                        // field name
                        string fieldName =
                            StringFilterParser.ParseFieldName(remainingFilter, out pos);
                        remainingFilter = StringFilterParser.GetRemainingFilter(remainingFilter, pos);
                        // relational operator
                        RelationalOperator relationalOperator =
                            StringFilterParser.ParseRelationalOperator(remainingFilter, out pos);
                        remainingFilter = StringFilterParser.GetRemainingFilter(remainingFilter, pos);
                        // other value or field
                        FilterTarget filterTarget;
                        string valueOrOtherFieldName =
                            StringFilterParser.ParseValueOrField(remainingFilter, out pos, out filterTarget);
                        remainingFilter = StringFilterParser.GetRemainingFilter(remainingFilter, pos);
                        // add to filter criteria
                        if (FilterConnective.None == filterConnective) {
                            filterCriteria = new FilterCriteria(fieldName, relationalOperator, valueOrOtherFieldName, filterTarget);
                        } else if (FilterConnective.And == filterConnective) {
                            filterCriteria = filterCriteria.And(fieldName, relationalOperator, valueOrOtherFieldName, filterTarget);
                        } else if (FilterConnective.Or == filterConnective) {
                            filterCriteria = filterCriteria.Or(fieldName, relationalOperator, valueOrOtherFieldName, filterTarget);
                        }
                    } catch (FilterException) {
                        StringFilterParser.ThrowNewInvalidFilterException(filter);
                    }
                } else {
                    StringFilterParser.ThrowNewInvalidFilterException(filter);
                }
                // next filter connective
                if (remainingFilter.Length > 0) {
                    try {
                        filterConnective = ParseFilterConnective(remainingFilter, out pos);
                        remainingFilter = StringFilterParser.GetRemainingFilter(remainingFilter, pos);
                    } catch (FilterException) {
                        StringFilterParser.ThrowNewInvalidFilterException(filter);
                    }
                }
            }
            return filterCriteria;
        }

        /// <summary>
        /// Gets the remaining filter of "filter" starting at "pos".
        /// </summary>
        /// <param name="filter">filter to get remaining filter for</param>
        /// <param name="pos">position to cut filter to get the
        /// remaining filter</param>
        /// <returns>remaining filter of "filter" starting at "pos"</returns>
        protected static string GetRemainingFilter(string filter, int pos) {
            int startIndex;
            checked {
                startIndex = pos + 1;
            }
            if (startIndex < filter.Length) {
                filter = filter.Substring(startIndex).TrimStart();
            } else {
                filter = string.Empty;
            }
            return filter;
        }

        /// <summary>
        /// Parses a field name at the beginning of a filter.
        /// </summary>
        /// <param name="filter">filter to parse</param>
        /// <param name="pos">returns the ending position of the
        /// field name</param>
        /// <returns>parsed field name</returns>
        protected static string ParseFieldName(string filter, out int pos) {
            pos = filter.IndexOf(']');
            if (pos < 0) {
                throw new FilterException();
            }
            string fieldName = filter.Substring(1, pos - 1);
            return fieldName;
        }

        /// <summary>
        /// Parses a filter connective at the beginning of a filter.
        /// </summary>
        /// <param name="filter">filter to parse</param>
        /// <param name="pos">returns the ending position of the
        /// filter connective</param>
        /// <returns>parsed filter connective</returns>
        protected static FilterConnective ParseFilterConnective(string filter, out int pos) {
            FilterConnective filterConnective = FilterConnective.None;
            string filterUpperCase = filter.ToUpperInvariant();
            if (filterUpperCase.StartsWith("AND", StringComparison.Ordinal)) {
                filterConnective = FilterConnective.And;
                pos = 3;
            } else if (filterUpperCase.StartsWith("OR", StringComparison.Ordinal)) {
                filterConnective = FilterConnective.Or;
                pos = 2;
            } else {
                throw new FilterException();
            }
            return filterConnective;
        }

        /// <summary>
        /// Parses a relational operator at the beginning of a filter.
        /// </summary>
        /// <param name="filter">filter to parse</param>
        /// <param name="pos">returns the ending position of the
        /// relational operator</param>
        /// <returns>parsed relational operator</returns>
        protected static RelationalOperator ParseRelationalOperator(string filter, out int pos) {
            RelationalOperator relationalOperator = RelationalOperator.IsEqualTo;
            if (filter.StartsWith("CONTAINS", StringComparison.Ordinal)) {
                relationalOperator = RelationalOperator.Contains;
                pos = 8;
            } else if (filter.StartsWith("ENDSWITH", StringComparison.Ordinal)) {
                relationalOperator = RelationalOperator.EndsWith;
                pos = 8;
            } else if (filter.StartsWith("==", StringComparison.Ordinal)) {
                pos = 2;
            } else if (filter.StartsWith(">=", StringComparison.Ordinal)) {
                relationalOperator = RelationalOperator.IsGreaterThanOrEqualTo;
                pos = 2;
            } else if (filter.StartsWith(">", StringComparison.Ordinal)) {
                relationalOperator = RelationalOperator.IsGreaterThan;
                pos = 1;
            } else if (filter.StartsWith("<=", StringComparison.Ordinal)) {
                relationalOperator = RelationalOperator.IsLessThanOrEqualTo;
                pos = 2;
            } else if (filter.StartsWith("<", StringComparison.Ordinal)) {
                relationalOperator = RelationalOperator.IsLessThan;
                pos = 1;
            } else if (filter.StartsWith("!=", StringComparison.Ordinal)) {
                relationalOperator = RelationalOperator.IsNotEqualTo;
                pos = 2;
            } else if (filter.StartsWith("STARTSWITH", StringComparison.Ordinal)) {
                relationalOperator = RelationalOperator.StartsWith;
                pos = 10;
            } else {
                throw new FilterException();
            }
            return relationalOperator;
        }

        /// <summary>
        /// Parses a sub filter at the beginning of a filter.
        /// </summary>
        /// <param name="filter">filter to parse</param>
        /// <param name="pos">returns the ending position of the
        /// sub filter</param>
        /// <returns>parsed sub filter</returns>
        protected static FilterCriteria ParseSubFilter(string filter, out int pos) {
            uint openBracesCount = 1;
            pos = 0;
            while (openBracesCount > 0) {
                pos++;
                if ('(' == filter[pos]) {
                    openBracesCount++;
                } else if (')' == filter[pos]) {
                    openBracesCount--;
                }
            }
            string subFilter = filter.Substring(1, pos - 1);
            FilterCriteria subFilterCriteria =
                StringFilterParser.GetFilterCriteriaFromSubFilter(subFilter);
            return subFilterCriteria;
        }

        /// <summary>
        /// Parses a field name or value at the beginning of a
        /// filter.
        /// </summary>
        /// <param name="filter">filter to parse</param>
        /// <param name="pos">returns the ending position of the
        /// field name or value</param>
        /// <param name="filterTarget">enum indicating whether the
        /// parsed string is a field name or a value</param>
        /// <returns>parsed field name or value</returns>
        protected static string ParseValueOrField(string filter, out int pos, out FilterTarget filterTarget) {
            string valueOrOtherFieldName = string.Empty;
            string filterUpperCase = filter.ToUpperInvariant();
            try {
                if ('[' == filter[0]) {
                    // compare to other field
                    pos = filter.Substring(1).IndexOf(']') + 1;
                    if (0 == pos) {
                        throw new FilterException("Closing bracket is missing.");
                    }
                    valueOrOtherFieldName = filter.Substring(1, pos - 1);
                    filterTarget = FilterTarget.IsOtherField;
                } else if ('"' == filter[0]) {
                    // compare to other text value
                    pos = 0;
                    var potentialPos = 0;
                    do {
                        potentialPos += filter.Substring(potentialPos + 1).IndexOf('"') + 1;
                        if (potentialPos < 1 || filter[potentialPos - 1] != '\\') { // to skip quotation marks that are escaped by backslash
                            pos = potentialPos;
                        }
                    } while (pos != potentialPos);
                    if (0 == pos) {
                        throw new FilterException("Closing quaotation marks are missing.");
                    }
                    valueOrOtherFieldName = filter.Substring(1, pos - 1).Replace("\\\"", "\"");
                    filterTarget = FilterTarget.IsOtherTextValue;
                } else if (filterUpperCase.StartsWith("TRUE", StringComparison.Ordinal)) {
                    // compare to TRUE
                    pos = 4;
                    valueOrOtherFieldName = bool.TrueString;
                    filterTarget = FilterTarget.IsOtherBoolValue;
                } else if (filterUpperCase.StartsWith("FALSE", StringComparison.Ordinal)) {
                    // compare to FALSE
                    pos = 5;
                    valueOrOtherFieldName = bool.FalseString;
                    filterTarget = FilterTarget.IsOtherBoolValue;
                } else {
                    // compare to numeric value or GUID value
                    int nextSpacePos = filter.IndexOf(' ');
                    if (nextSpacePos < 0) {
                        nextSpacePos = int.MaxValue;
                    }
                    pos = filter.IndexOf(')');
                    if (pos < 0) {
                        pos = int.MaxValue;
                    }
                    if (nextSpacePos < pos) {
                        pos = nextSpacePos;
                    }
                    if (0 == pos) {
                        throw new FilterException("Value or other field name cannot be parsed.");
                    }
                    if (int.MaxValue == pos) {
                        pos--; // necessary for (pos + 1) later
                        valueOrOtherFieldName = filter;
                    } else {
                        valueOrOtherFieldName = filter.Substring(0, pos);
                    }
                    if (Guid.TryParse(valueOrOtherFieldName, out Guid guid)) {
                        filterTarget = FilterTarget.IsOtherGuidValue;
                    } else {
                        filterTarget = FilterTarget.IsOtherNumericValue;
                    }
                }
            } catch (IndexOutOfRangeException exception) {
                throw new FilterException("Value or other field name is missing.", exception);
            }
            return valueOrOtherFieldName;
        }

        /// <summary>
        /// Thorws a new FiltetException saying that the filter is
        /// invalid and cannot be parsed.
        /// </summary>
        /// <param name="filter">invalid filter</param>
        protected static void ThrowNewInvalidFilterException(string filter) {
            throw new FilterException("Sub filter \""
                + filter + "\" is invalid and cannot be parsed.");
        }

        /// <summary>
        /// Parses the filter and returns it as filter criteria.
        /// </summary>
        /// <returns>filter as filter criteria</returns>
        public FilterCriteria ToFilterCriteria() {
            return StringFilterParser.GetFilterCriteriaFromSubFilter(this.Filter);
        }

    }

}