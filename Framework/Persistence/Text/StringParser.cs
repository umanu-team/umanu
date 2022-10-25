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

    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Base class for any string pasers.
    /// </summary>
    public abstract class StringParser {

        /// <summary>
        /// List of array delimiters.
        /// </summary>
        public List<StringDelimiter> ArrayDelimiters { get; private set; }

        /// <summary>
        /// List of escape charaters.
        /// </summary>
        public List<char> EscapeCharacters { get; private set; }

        /// <summary>
        /// List of delimiters for non-parsable data.
        /// </summary>
        public List<StringDelimiter> NonParsableDataDelimiters { get; private set; }

        /// <summary>
        /// List of object delimiters.
        /// </summary>
        public List<StringDelimiter> ObjectDelimiters { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public StringParser() {
            this.ArrayDelimiters = new List<StringDelimiter>();
            this.EscapeCharacters = new List<char>();
            this.NonParsableDataDelimiters = new List<StringDelimiter>();
            this.ObjectDelimiters = new List<StringDelimiter>();
        }

        /// <summary>
        /// Returns a string array that contains the substrings of a
        /// specified value.
        /// </summary>
        /// <param name="value">trimmed value to split into
        /// substrings</param>
        /// <param name="options">Specify "RemoveEmptyEntries" to
        /// omit empty array elements from the array returned, or
        /// "None" to include empty array elements in the array
        /// returned.</param>
        /// <returns>a collection whose elements contain the
        /// substrings of the specified value - the property SplitType
        /// indicates whether the split worked out</returns>
        public virtual StringSplitCollection TrySplit(string value, StringSplitOptions options) {
            StringSplitCollection splitStrings = new StringSplitCollection();
            if (!string.IsNullOrEmpty(value)) {
                Stack<StringDelimiter> activeDelimiters = new Stack<StringDelimiter>();
                bool isCharacterEscaped = false;
                int startPos = 0;
                int pos = 0;
                foreach (char c in value) {
                    if (isCharacterEscaped) {
                        isCharacterEscaped = false;
                    } else if (this.EscapeCharacters.Contains(c)) {
                        isCharacterEscaped = true;
                    } else if (0 == pos) {
                        // look for start of active delimiter
                        if (this.TryAddArrayDelimiter(c, activeDelimiters)) {
                            splitStrings.SplitType = StringSplitType.Array;
                            startPos = pos + 1;
                        } else if (this.TryAddObjectDelimiter(c, activeDelimiters)) {
                            splitStrings.SplitType = StringSplitType.Object;
                            startPos = pos + 1;
                        } else {
                            splitStrings.Add(value);
                            break;
                        }
                    } else { // activeDelimiters.Count != 0
                        string splitString = null;
                        if (c == activeDelimiters.Peek().End) {
                            // look for end of active delimiter
                            if (1 == activeDelimiters.Count) {
                                splitString = value.Substring(startPos, pos - startPos);
                            }
                            activeDelimiters.Pop();
                        } else if (c == activeDelimiters.Peek().Separator) {
                            // look for separator of active delimiter
                            if (1 == activeDelimiters.Count) {
                                splitString = value.Substring(startPos, pos - startPos);
                                startPos = pos + 1;
                            }
                        } else {
                            // look for start of other delimiter
                            if (!this.TryAddArrayDelimiter(c, activeDelimiters)) {
                                if (!this.TryAddObjectDelimiter(c, activeDelimiters)) {
                                    this.TryAddNonParsableDataDelimiter(c, activeDelimiters);
                                }
                            }
                        }
                        if (null != splitString) {
                            splitString = splitString.Trim();
                            if (StringSplitOptions.None == options || (StringSplitOptions.RemoveEmptyEntries == options && string.Empty != splitString)) {
                                splitStrings.Add(splitString);
                            }
                        }
                    }
                    pos++;
                }
                if (activeDelimiters.Count > 0) {
                    splitStrings.SplitType = StringSplitType.Error;
                }
            }
            return splitStrings;
        }

        /// <summary>
        /// Checks whether a specified character is the start of an
        /// array delimiter and adds it to the stack of delimiters.
        /// </summary>
        /// <param name="c">character to check</param>
        /// <param name="delimiters">stack of delimiters</param>
        /// <returns>true if character is start of array delimiter,
        /// false otherwise</returns>
        private bool TryAddArrayDelimiter(char c, Stack<StringDelimiter> delimiters) {
            bool startIsFound = false;
            foreach (var arrayDelimiter in this.ArrayDelimiters) {
                if (c == arrayDelimiter.Start) {
                    delimiters.Push(arrayDelimiter);
                    startIsFound = true;
                    break;
                }
            }
            return startIsFound;
        }

        /// <summary>
        /// Checks whether a specified character is the start of a
        /// non-parsable data delimiter and adds it to the stack of
        /// delimiters.
        /// </summary>
        /// <param name="c">character to check</param>
        /// <param name="delimiters">stack of delimiters</param>
        /// <returns>true if character is start of non-parsable data
        /// delimiter, false otherwise</returns>
        private bool TryAddNonParsableDataDelimiter(char c, Stack<StringDelimiter> delimiters) {
            bool startIsFound = false;
            foreach (var nonParsableDataDelimiter in this.NonParsableDataDelimiters) {
                if (c == nonParsableDataDelimiter.Start) {
                    delimiters.Push(nonParsableDataDelimiter);
                    startIsFound = true;
                    break;
                }
            }
            return startIsFound;
        }

        /// <summary>
        /// Checks whether a specified character is the start of an
        /// object delimiter and adds it to the stack of delimiters.
        /// </summary>
        /// <param name="c">character to check</param>
        /// <param name="delimiters">stack of delimiters</param>
        /// <returns>true if character is start of object delimiter,
        /// false otherwise</returns>
        private bool TryAddObjectDelimiter(char c, Stack<StringDelimiter> delimiters) {
            bool startIsFound = false;
            foreach (var objectDelimiter in this.ObjectDelimiters) {
                if (c == objectDelimiter.Start) {
                    delimiters.Push(objectDelimiter);
                    startIsFound = true;
                    break;
                }
            }
            return startIsFound;
        }

    }

}