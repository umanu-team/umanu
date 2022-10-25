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

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Formatter for MS SQL full text queries.
    /// </summary>
    public class MSSqlFullTextQueryFormatter {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        internal MSSqlFullTextQueryFormatter()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Formats a full-text query to become a valid SQL Server
        /// query.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to be
        /// formatted</param>
        /// <returns>formatted full-text query</returns>
        public string Format(string fullTextQuery) {
            fullTextQuery = MSSqlFullTextQueryFormatter.FormatFullTextQueryConnectives(fullTextQuery);
            fullTextQuery = MSSqlFullTextQueryFormatter.FormatFullTextQueryPlaceholders(fullTextQuery);
            return fullTextQuery;
        }

        /// <summary>
        /// Formats a connective of a full-text query to become a
        /// valid part of an SQL Server query.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to format</param>
        /// <param name="startIndex">start index of substring to
        /// format</param>
        /// <param name="endIndex">end index of substring to format</param>
        /// <returns>formatted full-text query</returns>
        private static string FormatFullTextQueryConnective(string fullTextQuery, int startIndex, int endIndex) {
            return fullTextQuery.Substring(0, startIndex)
                + fullTextQuery.Substring(startIndex, endIndex - startIndex + 1)
                .Replace(" AND NOT ", "ANDNOT")
                .Replace(" AND ", "AND")
                .Replace(" OR ", "OR")
                .Replace(" ", " AND ")
                .Replace("OR", " OR ")
                .Replace("AND", " AND ")
                .Replace("ANDNOT", " AND NOT ")
                + fullTextQuery.Substring(endIndex + 1);
        }

        /// <summary>
        /// Formats all connectives of a full-text query to become
        /// valid parts of an SQL Server query.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to format</param>
        /// <returns>formatted full-text query</returns>
        private static string FormatFullTextQueryConnectives(string fullTextQuery) {
            var quotationMarkIndexes = MSSqlFullTextQueryFormatter.GetReversedQuotationMarkIndexes(fullTextQuery);
            int endIndex = fullTextQuery.Length - 1;
            foreach (int quotationMarkIndex in quotationMarkIndexes) {
                if (endIndex > -1) {
                    fullTextQuery = MSSqlFullTextQueryFormatter.FormatFullTextQueryConnective(fullTextQuery, quotationMarkIndex + 1, endIndex);
                    endIndex = -1;
                } else {
                    endIndex = quotationMarkIndex;
                }
            }
            fullTextQuery = MSSqlFullTextQueryFormatter.FormatFullTextQueryConnective(fullTextQuery, 0, endIndex);
            return fullTextQuery;
        }

        /// <summary>
        /// Formats a placeholder of a full-text query to support
        /// suffix search as well as combined prefix and suffix
        /// search.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to format</param>
        /// <param name="startIndex">start index of substring to
        /// format</param>
        /// <param name="endIndex">end index of substring to format</param>
        /// <returns>formatted full-text query</returns>
        private static string FormatFullTextQueryPlaceholder(string fullTextQuery, int startIndex, int endIndex) {
            var fullTextQueryPart = fullTextQuery.Substring(startIndex, endIndex - startIndex + 1);
            if (fullTextQueryPart.StartsWith("\"*")) {
                var fullTextQueryPartArray = fullTextQueryPart.ToCharArray();
                Array.Reverse(fullTextQueryPartArray);
                fullTextQueryPart = new string(fullTextQueryPartArray);
            }
            return fullTextQuery.Substring(0, startIndex)
                + fullTextQueryPart
                + fullTextQuery.Substring(endIndex + 1);
        }

        /// <summary>
        /// Formats all placeholders of a full-text query to support
        /// suffix search as well as combined prefix and suffix
        /// search.
        /// </summary>
        /// <param name="fullTextQuery">full-text query to format</param>
        /// <returns>formatted full-text query</returns>
        private static string FormatFullTextQueryPlaceholders(string fullTextQuery) {
            var quotationMarkIndexes = MSSqlFullTextQueryFormatter.GetReversedQuotationMarkIndexes(fullTextQuery);
            if (quotationMarkIndexes.Count > 1) {
                int endIndex = -1;
                foreach (int quotationMarkIndex in quotationMarkIndexes) {
                    if (endIndex > -1) {
                        int startIndex = quotationMarkIndex;
                        fullTextQuery = MSSqlFullTextQueryFormatter.FormatFullTextQueryPlaceholder(fullTextQuery, startIndex, endIndex);
                        endIndex = -1;
                    } else {
                        endIndex = quotationMarkIndex;
                    }
                }
            } else {
                fullTextQuery = MSSqlFullTextQueryFormatter.FormatFullTextQueryPlaceholder(fullTextQuery, 0, fullTextQuery.Length - 1);
            }
            return fullTextQuery;
        }

        /// <summary>
        /// Gets all indexes of quotation marks in reversed order.
        /// </summary>
        /// <param name="fullTextQuery">full text query to find
        /// quotation marks in</param>
        /// <returns>all indexes of quotation marks in reversed order</returns>
        private static List<int> GetReversedQuotationMarkIndexes(string fullTextQuery) {
            var quotationMarkIndexes = new List<int>();
            int index = 0;
            foreach (char c in fullTextQuery) {
                if ('"' == c) {
                    quotationMarkIndexes.Add(index);
                }
                index++;
            }
            if (0 != quotationMarkIndexes.Count % 2) {
                quotationMarkIndexes.RemoveAt(quotationMarkIndexes.Count - 1);
            }
            quotationMarkIndexes.Reverse();
            return quotationMarkIndexes;
        }

    }

}