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

namespace Framework.Presentation.Web {

    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Helper class for URLs.
    /// </summary>
    public static class UrlUtility {

        /// <summary>
        /// Builds a query string for URL parameters.
        /// </summary>
        /// <param name="urlParameters">URL parameters to build query
        /// string for</param>
        /// <returns>query string for URL parameters</returns>
        public static string BuildQueryStringFor(IEnumerable<KeyValuePair<string, string>> urlParameters) {
            var queryStringBuilder = new StringBuilder();
            bool isFirstParameter = true;
            foreach (var urlParameter in urlParameters) {
                if (isFirstParameter) {
                    isFirstParameter = false;
                    queryStringBuilder.Append('?');
                } else {
                    queryStringBuilder.Append('&');
                }
                queryStringBuilder.Append(urlParameter.Key);
                queryStringBuilder.Append('=');
                queryStringBuilder.Append(urlParameter.Value);
            }
            return queryStringBuilder.ToString();
        }

        /// <summary>
        /// Gets the slugified URL name for a title.
        /// </summary>
        /// <param name="title">title to get URL name for</param>
        /// <returns>slugified URL name for title</returns>
        public static string GetUrlNameFor(string title) {
            const string allowedCharacters = "abcdefghijklmnopqrstuvwxyz0123456789-_~";
            var urlNameBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(title)) {
                title = title.ToLowerInvariant();
                bool isPreviousCharacterDash = false;
                foreach (char c in title) {
                    if (' ' == c || '-' == c) {
                        if (!isPreviousCharacterDash) {
                            urlNameBuilder.Append('-');
                            isPreviousCharacterDash = true;
                        }
                    } else if ('ä' == c) {
                        urlNameBuilder.Append("ae");
                        isPreviousCharacterDash = false;
                    } else if ('é' == c || 'è' == c || 'ê' == c) {
                        urlNameBuilder.Append("e");
                        isPreviousCharacterDash = false;
                    } else if ('ñ' == c) {
                        urlNameBuilder.Append("n");
                        isPreviousCharacterDash = false;
                    } else if ('ö' == c) {
                        urlNameBuilder.Append("oe");
                        isPreviousCharacterDash = false;
                    } else if ('ü' == c) {
                        urlNameBuilder.Append("ue");
                        isPreviousCharacterDash = false;
                    } else if ('ß' == c) {
                        urlNameBuilder.Append("ss");
                        isPreviousCharacterDash = false;
                    } else if (allowedCharacters.Contains(c.ToString(CultureInfo.InvariantCulture))) {
                        urlNameBuilder.Append(c);
                        isPreviousCharacterDash = false;
                    }
                }
            }
            return urlNameBuilder.ToString();
        }

    }

}