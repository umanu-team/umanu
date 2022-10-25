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

namespace Framework.Presentation.Converters {

    using Framework.Persistence;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Builder class for JSON output.
    /// </summary>
    public sealed class JsonBuilder {

        /// <summary>
        /// List of key/value pairs of characters to be escaped in
        /// JSON where key is the character to be escaped and value
        /// is the escaped version of the character.
        /// </summary>
        public IList<KeyValuePair<char, string>> CharactersToBeEscaped { get; private set; }

        /// <summary>
        /// Type of conversion to be applied for keys.
        /// </summary>
        public KeyConversion KeyConversion { get; set; }

        /// <summary>
        /// Gets the number of charaters in current instance.
        /// </summary>
        public int Length {
            get { return this.stringBuilder.Length; }
        }

        /// <summary>
        /// Encapsulted string builder.
        /// </summary>
        private readonly StringBuilder stringBuilder;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public JsonBuilder()
            : base() {
            this.CharactersToBeEscaped = new List<KeyValuePair<char, string>>(new KeyValuePair<char, string>[] {
                new KeyValuePair<char, string>('\\', "\\\\"),
                new KeyValuePair<char, string>('"', "\\\""),
                new KeyValuePair<char, string>('\n', "\\n"),
                new KeyValuePair<char, string>('\r', "\\r"),
                new KeyValuePair<char, string>('\t', "\\t"),
                new KeyValuePair<char, string>('\u0000', "\\\u0000"),
                new KeyValuePair<char, string>('\u0001', "\\\u0001"),
                new KeyValuePair<char, string>('\u0002', "\\\u0002"),
                new KeyValuePair<char, string>('\u0003', "\\\u0003"),
                new KeyValuePair<char, string>('\u0004', "\\\u0004"),
                new KeyValuePair<char, string>('\u0005', "\\\u0005"),
                new KeyValuePair<char, string>('\u0006', "\\\u0006"),
                new KeyValuePair<char, string>('\u0007', "\\\u0007"),
                new KeyValuePair<char, string>('\u0008', "\\\u0008"),
                new KeyValuePair<char, string>('\u0009', "\\\u0009"),
                new KeyValuePair<char, string>('\u000A', "\\\u000A"),
                new KeyValuePair<char, string>('\u000B', "\\\u000B"),
                new KeyValuePair<char, string>('\u000C', "\\\u000C"),
                new KeyValuePair<char, string>('\u000D', "\\\u000D"),
                new KeyValuePair<char, string>('\u000E', "\\\u000E"),
                new KeyValuePair<char, string>('\u000F', "\\\u000F"),
                new KeyValuePair<char, string>('\u0010', "\\\u0010"),
                new KeyValuePair<char, string>('\u0011', "\\\u0011"),
                new KeyValuePair<char, string>('\u0012', "\\\u0012"),
                new KeyValuePair<char, string>('\u0013', "\\\u0013"),
                new KeyValuePair<char, string>('\u0014', "\\\u0014"),
                new KeyValuePair<char, string>('\u0015', "\\\u0015"),
                new KeyValuePair<char, string>('\u0016', "\\\u0016"),
                new KeyValuePair<char, string>('\u0017', "\\\u0017"),
                new KeyValuePair<char, string>('\u0018', "\\\u0018"),
                new KeyValuePair<char, string>('\u0019', "\\\u0019"),
                new KeyValuePair<char, string>('\u001A', "\\\u001A"),
                new KeyValuePair<char, string>('\u001B', "\\\u001B"),
                new KeyValuePair<char, string>('\u001C', "\\\u001C"),
                new KeyValuePair<char, string>('\u001D', "\\\u001D"),
                new KeyValuePair<char, string>('\u001E', "\\\u001E"),
                new KeyValuePair<char, string>('\u001F', "\\\u001F")
        });
            this.stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Appends &quot;]&quot; to indicate the end of an array.
        /// </summary>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendArrayEnd() {
            this.stringBuilder.Append(']');
            return this;
        }

        /// <summary>
        /// Appends &quot;[&quot; to indicate the start of an array.
        /// </summary>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendArrayStart() {
            this.stringBuilder.Append('[');
            return this;
        }

        /// <summary>
        /// Appends the JSON string representation of a key to a
        /// string builder.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendKey(string key) {
            this.stringBuilder.Append('\"');
            this.stringBuilder.Append(this.ConvertKey(key));
            this.stringBuilder.Append("\":");
            return this;
        }

        /// <summary>
        /// Appends the JSON string representation of a pair of key
        /// and value to a string builder.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="value">value to append</param>
        /// <param name="isStringValue">true if value is of type
        /// string, false otherwise</param>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendKeyValuePair(string key, string value, bool isStringValue) {
            this.AppendKey(key);
            this.AppendValue(value, isStringValue);
            return this;
        }

        /// <summary>
        /// Appends the JSON string representation of a pair of key
        /// and values to a string builder.
        /// </summary>
        /// <param name="key">key to append</param>
        /// <param name="values">values to append</param>
        /// <param name="areStringValues">true if values are of type
        /// string, false otherwise</param>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendKeyValuePair(string key, IEnumerable<string> values, bool areStringValues) {
            this.AppendKey(key);
            this.AppendArrayStart();
            var isFirstValue = true;
            foreach (var value in values) {
                if (isFirstValue) {
                    isFirstValue = false;
                } else {
                    this.AppendSeparator();
                }
                this.AppendValue(value, areStringValues);
            }
            this.AppendArrayEnd();
            return this;
        }

        /// <summary>
        /// Converts a key as defined in key conversion rules.
        /// </summary>
        /// <param name="key">key to be converted</param>
        /// <returns>converted key</returns>
        private string ConvertKey(string key) {
            string convertedKey;
            if (KeyConversion.PascalCaseToSnakeCase == this.KeyConversion) {
                var convertedKeyBuilder = new StringBuilder();
                foreach (var c in key) {
                    if (char.IsUpper(c)) {
                        if (convertedKeyBuilder.Length > 0) {
                            convertedKeyBuilder.Append('_');
                        }
                        convertedKeyBuilder.Append(char.ToLowerInvariant(c));
                    } else {
                        convertedKeyBuilder.Append(c);
                    }
                }
                convertedKey = convertedKeyBuilder.ToString();
            } else {
                convertedKey = key;
            }
            return convertedKey;
        }

        /// <summary>
        /// Appends &quot;}&quot; to indicate the end of an object.
        /// </summary>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendObjectEnd() {
            this.stringBuilder.Append('}');
            return this;
        }

        /// <summary>
        /// Appends &quot;{&quot; to indicate the start of an object.
        /// </summary>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendObjectStart() {
            this.stringBuilder.Append('{');
            return this;
        }

        /// <summary>
        /// Appends a comma as separator for array/object values.
        /// </summary>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendSeparator() {
            this.stringBuilder.Append(',');
            return this;
        }

        /// <summary>
        /// Appends a value to JSON builder.
        /// </summary>
        /// <param name="value">value to append</param>
        /// <param name="isStringValue">true if value is of type
        /// string, false otherwise</param>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder AppendValue(string value, bool isStringValue) {
            if (null == value || (!isStringValue && string.Empty == value)) {
                this.stringBuilder.Append("null");
            } else {
                if (isStringValue) {
                    this.stringBuilder.Append('\"');
                    value = this.EscapeString(value);
                } else {
                    if (bool.TrueString == value) {
                        value = "true";
                    } else if (bool.FalseString == value) {
                        value = "false";
                    }
                }
                this.stringBuilder.Append(value);
                if (isStringValue) {
                    this.stringBuilder.Append('\"');
                }
            }
            return this;
        }

        /// <summary>
        /// Removes all characters from current instance.
        /// </summary>
        /// <returns>current instance of JSON builder</returns>
        public JsonBuilder Clear() {
            this.stringBuilder.Clear();
            return this;
        }

        /// <summary>
        /// Escapes a string value for use in JSON strings.
        /// </summary>
        /// <param name="value">string value to escape</param>
        /// <returns>escaped string value for use in JSON strings</returns>
        public string EscapeString(string value) {
            foreach (var characterToBeReplaced in this.CharactersToBeEscaped) {
                value = value.Replace(characterToBeReplaced.Key.ToString(CultureInfo.InvariantCulture), characterToBeReplaced.Value);
            }
            return value;
        }

        /// <summary>
        /// Converts the value of this instance into a JSON file.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <returns>value of this instance as JSON file</returns>
        public File ToFile(string name) {
            return new File(name, "application/json", Encoding.UTF8.GetBytes(this.ToString()));
        }

        /// <summary>
        /// Converts the value of this instance into a JSON string.
        /// </summary>
        /// <returns>value of this instance into a JSON string</returns>
        public override string ToString() {
            return this.stringBuilder.ToString();
        }

    }

}