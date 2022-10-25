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

    using System;

    /// <summary>
    /// Helper class for text.
    /// </summary>
    public static class TextUtility {

        /// <summary>
        /// Truncates a text to a maximum length. If possible, the
        /// text is truncated after a full sentence. If not, it is
        /// truncated after a full word. If that is neither possible
        /// the text is cut within a word.
        /// </summary>
        /// <param name="text">text to truncate</param>
        /// <param name="maxLength">maximum length of text</param>
        /// <returns>text truncated to maximum length</returns>
        public static string Truncate(string text, int maxLength) {
            if (text.Length > maxLength) {
                var indexes = new int[3];
                indexes[0] = text.LastIndexOf(". ", maxLength);
                indexes[1] = text.LastIndexOf("! ", maxLength);
                indexes[2] = text.LastIndexOf("? ", maxLength);
                Array.Sort(indexes);
                var index = indexes[2];
                if (index > 0) {
                    text = text.Substring(0, index + 1);
                } else if (maxLength > 3) {
                    index = text.LastIndexOf(' ', maxLength - 4);
                    if (index > 0) {
                        text = text.Substring(0, index + 1) + "...";
                    } else {
                        text = text.Substring(0, maxLength - 3) + "...";
                    }
                } else {
                    text = text.Substring(0, maxLength);
                }
            }
            return text;
        }

    }

}