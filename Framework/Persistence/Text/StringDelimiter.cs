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

namespace Framework.Persistence.Text {

    /// <summary>
    /// String delimiter for separation of substrings.
    /// </summary>
    public class StringDelimiter {

        /// <summary>
        /// End of delimiter.
        /// </summary>
        public char End { get; set; }

        /// <summary>
        /// Separator of elements.
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// Start of delimiter.
        /// </summary>
        public char Start { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public StringDelimiter() {
            this.Start = this.Separator = this.End = char.MinValue;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="start">start of delimiter</param>
        /// <param name="separator">of elements</param>
        /// <param name="end">end of delimiter</param>
        public StringDelimiter(char start, char separator, char end)
            : this() {
            this.Start = start;
            this.Separator = separator;
            this.End = end;
        }

    }

}