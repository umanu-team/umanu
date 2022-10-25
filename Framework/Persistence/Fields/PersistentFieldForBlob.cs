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

namespace Framework.Persistence.Fields {

    using DocumentFormat.OpenXml.Packaging;
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Wrapper class for a field of type binary large object to be
    /// stored in persistence mechanism.
    /// </summary>
    public sealed class PersistentFieldForBlob : PersistentFieldForElement<byte[]> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.ByteArray;
            }
        }

        /// <summary>
        /// Value loaded from DbDataReader.
        /// </summary>
        private byte[] valueFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        public PersistentFieldForBlob(string key)
            : base(key) {
            this.IsFullTextIndexed = true;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        /// <param name="value">value of persistent field</param>
        public PersistentFieldForBlob(string key, byte[] value)
            : this(key) {
            this.Value = value;
        }

        /// <summary>
        /// Copies the value of persistent field.
        /// </summary>
        /// <returns>copy of value of persistent field</returns>
        internal override object CopyValue() {
            byte[] copy;
            if (null == this.Value) {
                copy = null;
            } else {
                copy = new byte[this.Value.LongLength];
                this.Value.CopyTo(copy, 0);
            }
            return copy;
        }

        /// <summary>
        /// Gets the value of this presentable field as plain text.
        /// As an example, this is used for full-text indexing.
        /// </summary>
        /// <returns>value of this presentable field as plain text</returns>
        public override string GetValueAsPlainText() {
            var plainTextBuilder = new StringBuilder();
            if (null != this.Value) {
                var memoryStream = new MemoryStream(this.Value);
                try {
                    var isProcessed = false;
                    try {
                        using (var wordDocument = WordprocessingDocument.Open(memoryStream, false)) {
                            memoryStream = null;
                            if (null != wordDocument?.MainDocumentPart?.Document?.InnerText) {
                                plainTextBuilder.Append(wordDocument.MainDocumentPart.Document.InnerText);
                                isProcessed = true;
                            }
                        }
                    } catch (Exception) {
                        // ignore exceptions - unfortunately there is no way to detect whether a document is valid before opening it 
                    }
                    if (!isProcessed) {
                        try {
                            using (var excelDocument = SpreadsheetDocument.Open(memoryStream, false)) {
                                memoryStream = null;
                                if (null != excelDocument?.WorkbookPart?.SharedStringTablePart?.SharedStringTable) {
                                    foreach (var sharedStringElement in excelDocument.WorkbookPart.SharedStringTablePart.SharedStringTable) {
                                        if (null != sharedStringElement) {
                                            string sharedString = sharedStringElement.InnerText;
                                            if (!string.IsNullOrEmpty(sharedString) && plainTextBuilder.Length > 0) {
                                                plainTextBuilder.Append(' ');
                                            }
                                            plainTextBuilder.Append(sharedString);
                                        }
                                    }
                                    isProcessed = true;
                                }
                            }
                        } catch (Exception) {
                            // ignore exceptions - unfortunately there is no way to detect whether a document is valid before opening it 
                        }
                    }
                    if (!isProcessed) {
                        try {
                            using (var powerPointDocument = PresentationDocument.Open(memoryStream, false)) {
                                memoryStream = null;
                                if (null != powerPointDocument?.PresentationPart?.SlideParts) {
                                    foreach (var slidePart in powerPointDocument.PresentationPart.SlideParts) {
                                        string slideText = slidePart.Slide.InnerText;
                                        if (!string.IsNullOrEmpty(slideText) && plainTextBuilder.Length > 0) {
                                            plainTextBuilder.Append(' ');
                                        }
                                        plainTextBuilder.Append(slideText);
                                    }
                                    isProcessed = true;
                                }
                            }
                        } catch (Exception) {
                            // ignore exceptions - unfortunately there is no way to detect whether a document is valid before opening it 
                        }
                    }
                } finally {
                    if (null != memoryStream) {
                        memoryStream.Dispose();
                    }
                }
            }
            return plainTextBuilder.ToString();
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected internal override string GetValueAsString() {
            string value;
            if (null == this.Value) {
                value = string.Empty;
            } else {
                value = Convert.ToBase64String(this.Value, Base64FormattingOptions.None);
            }
            return value;
        }

        /// <summary>
        /// Loads a new value for persistent field from a specified
        /// index of a DbDataReader, but does not set the value yet.
        /// </summary>
        /// <param name="dataReader">data reader to load value from</param>
        /// <param name="ordinal">index of data reader to load value
        /// from</param>
        internal override void LoadValueFromDbDataReader(DbDataReader dataReader, int ordinal) {
            if (dataReader.IsDBNull(ordinal)) {
                this.valueFromDbDataReader = null;
            } else {
                this.valueFromDbDataReader = (byte[])dataReader.GetValue(ordinal);
            }
            return;
        }

        /// <summary>
        /// Sets the new value for persistent field which has been
        /// loaded from DbDataReader before.
        /// </summary>
        internal override void SetValueFromDbDataReader() {
            this.SetValueUnsafe(this.valueFromDbDataReader);
            this.valueFromDbDataReader = null;
            return;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">new value to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public override bool TrySetValueAsString(string value) {
            this.Value = Convert.FromBase64String(value);
            return true;
        }

        /// <summary>
        /// Determines whether two value are different.
        /// </summary>
        /// <param name="x">first value to compare</param>
        /// <param name="y">second value to compare</param>
        /// <returns>true if the specified values are different,
        /// false otherwise</returns>
        protected override bool ValuesAreDifferent(byte[] x, byte[] y) {
            bool isEqual = true;
            if ((null == x && null != y) || (null != x && null == y)) {
                isEqual = false;
            } else if (null != x && null != y) {
                if (x.LongLength == y.LongLength) {
                    for (long i = 0; i < x.LongLength; i++) {
                        if (x[i] != y[i]) {
                            isEqual = false;
                            break;
                        }
                    }
                } else {
                    isEqual = false;
                }
            }
            return !isEqual;
        }

    }

}