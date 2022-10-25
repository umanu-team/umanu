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

namespace Framework.Presentation.Forms {

    /// <summary>
    /// Common regular expressions.
    /// </summary>
    public static class Regex {

        /// <summary>
        /// Regular expression for matching a hyperlink within a
        /// text - match is stored in $1.
        /// </summary>
        public static readonly System.Text.RegularExpressions.Regex ForHyperlinkInText = new System.Text.RegularExpressions.Regex("([a-zA-Z0-9]+\\:\\/\\/[^\\s<>]+[a-zA-Z0-9\\/])", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for matching an HTML hyperlink tag
        /// within a text - match is stored in $1, href of match is
        /// stored in $2.
        /// </summary>
        public static readonly System.Text.RegularExpressions.Regex ForHyperlinkTagInText = new System.Text.RegularExpressions.Regex("(<a [^<>]*href=\"([a-zA-Z0-9]+\\:\\/\\/[^\\s<>\"]+[a-zA-Z0-9\\/])\"[^<>]*>)", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for matching a local ISO 8601
        /// date/time string - match is stored in $1.
        /// </summary>
        public static readonly System.Text.RegularExpressions.Regex ForLocalIso8601DateTime = new System.Text.RegularExpressions.Regex("([0-9]{4}\\-[0-9]{2}\\-[0-9]{2}T[0-2][0-9]\\:[0-5][0-9](\\:[0-5][0-9])?)", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for matching multiple spaces.
        /// </summary>
        public static readonly System.Text.RegularExpressions.Regex ForMultipleSpaces = new System.Text.RegularExpressions.Regex(" {2,}", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for matching a stand-alone email
        /// address.
        /// </summary>
        public static readonly System.Text.RegularExpressions.Regex ForStandAloneEmailAddress = new System.Text.RegularExpressions.Regex("^[^@,;\\s]+@[^@,;\\s]+$", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for matching a stand-alone hyperlink.
        /// </summary>
        public static readonly System.Text.RegularExpressions.Regex ForStandAloneHyperlink = new System.Text.RegularExpressions.Regex("^\\w+\\:\\/\\/.+$", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for matching a stand-alone time.
        /// </summary>
        public static readonly System.Text.RegularExpressions.Regex ForStandAloneTime = new System.Text.RegularExpressions.Regex("^[0-2][0-9]\\:[0-5][0-9](\\:[0-5][0-9])?$", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for matching am XML tag.
        /// </summary>
        public static readonly System.Text.RegularExpressions.Regex ForXmlTag = new System.Text.RegularExpressions.Regex("<[^<>]*>", System.Text.RegularExpressions.RegexOptions.Compiled);

    }

}