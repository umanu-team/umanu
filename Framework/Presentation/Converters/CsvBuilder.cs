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
    using System.Text;

    /// <summary>
    /// Builder class for CSV output.
    /// </summary>
    public sealed class CsvBuilder {

        /// <summary>
        /// True if current line does not contain any data, false
        /// otherwise.
        /// </summary>
        private bool isCurrentLineEmpty;

        /// <summary>
        /// Gets the number of charaters in current instance.
        /// </summary>
        public int Length {
            get { return this.stringBuilder.Length; }
        }

        /// <summary>
        /// Line break character(s) to be useed.
        /// </summary>
        public string NewLine { get; private set; }

        /// <summary>
        /// CSV value separator to be used.
        /// </summary>
        public char Separator { get; private set; }

        /// <summary>
        /// Encapsulted string builder.
        /// </summary>
        private readonly StringBuilder stringBuilder;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="newLine">line break character(s) to be used,
        /// e.g. Environment.NewLine</param>
        /// <param name="separator">CSV value separator to be used</param>
        public CsvBuilder(string newLine, char separator)
            : base() {
            this.isCurrentLineEmpty = true;
            this.NewLine = newLine;
            this.Separator = separator;
            this.stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Appends a line break to CSV builder.
        /// </summary>
        /// <returns>current instance of CSV builder</returns>
        public CsvBuilder AppendNewLine() {
            this.stringBuilder.Append(this.NewLine);
            this.isCurrentLineEmpty = true;
            return this;
        }

        /// <summary>
        /// Appends a line of values to CSV builder.
        /// </summary>
        /// <param name="values">values to be appended</param>
        /// <returns>current instance of CSV builder</returns>
        public CsvBuilder AppendLine(string[] values) {
            foreach (var value in values) {
                this.AppendValue(value);
            }
            this.AppendNewLine();
            return this;
        }

        /// <summary>
        /// Appends a value to CSV builder.
        /// </summary>
        /// <param name="value">value to append</param>
        /// <returns>current instance of CSV builder</returns>
        public CsvBuilder AppendValue(string value) {
            if (this.isCurrentLineEmpty) {
                this.isCurrentLineEmpty = false;
            } else {
                this.stringBuilder.Append(this.Separator);
            }
            if (!string.IsNullOrEmpty(value)) {
                this.stringBuilder.Append("\"");
                var cleanedValue = value.Replace("\"", "\"\"");
                this.stringBuilder.Append(cleanedValue);
                this.stringBuilder.Append("\"");
            }
            return this;
        }

        /// <summary>
        /// Removes all characters from current instance.
        /// </summary>
        /// <returns>current instance of CSV builder</returns>
        public CsvBuilder Clear() {
            this.stringBuilder.Clear();
            this.isCurrentLineEmpty = true;
            return this;
        }

        /// <summary>
        /// Converts the value of this instance into a CSV file.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <returns>value of this instance as CSV file</returns>
        public File ToFile(string name) {
            const string byteOrderMark = "\xfeff";
            return new File(name, "text/csv", Encoding.UTF8.GetBytes(byteOrderMark + this.ToString()));
        }

        /// <summary>
        /// Converts the value of this instance into a CSV string.
        /// </summary>
        /// <returns>value of this instance as CSV string</returns>
        public override string ToString() {
            return this.stringBuilder.ToString();
        }

    }

}