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

    using Forms;
    using Framework.Presentation.Exceptions;
    using Framework.Properties;
    using Persistence;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Web;

    /// <summary>
    /// Converter for CSV format to presentable objects.
    /// </summary>
    public sealed class CsvReader<T> where T : IPresentableObject, new() {

        /// <summary>
        /// Validation result of CSV data.
        /// </summary>
        private class ValidationResult {

            /// <summary>
            /// Error message if CSV data is not valid.
            /// </summary>
            public string ErrorMessage { get; set; }

            /// <summary>
            /// True if CSV data is valid, false otherwise.
            /// </summary>
            public bool IsValid {
                get { return string.IsNullOrEmpty(this.ErrorMessage); }
            }

        }

        /// <summary>
        /// Delegate to be used for creation of new items.
        /// </summary>
        public CreateNewItemDelegate<T> CreateNewItemDelegate { get; set; }

        /// <summary>
        /// True to ignore fields that are contained in view but not
        /// in presentable object, false to throw an exception if a
        /// field is missing in presentable object.
        /// </summary>
        public bool IgnoreMissingFields { get; private set; }

        /// <summary>
        /// Data provider to use for option providers.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; private set; }

        /// <summary>
        /// CSV separator to be used.
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// List view to be applied for field mapping.
        /// </summary>
        public IListTableView View { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// for this list table</param>
        public CsvReader(IOptionDataProvider optionDataProvider, BuildingRule buildingRule)
            : this(optionDataProvider, buildingRule, new ListTableView()) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// for this list table</param>
        /// <param name="view">list view to be applied for field
        /// mapping</param>
        public CsvReader(IOptionDataProvider optionDataProvider, BuildingRule buildingRule, IListTableView view) {
            this.CreateNewItemDelegate = delegate () {
                return new T();
            };
            this.IgnoreMissingFields = BuildingRule.IgnoreMissingFields == buildingRule;
            this.OptionDataProvider = optionDataProvider;
            this.Separator = ';';
            this.View = view;
        }

        /// <summary>
        /// Detects the line separator used in CSV data.
        /// </summary>
        /// <param name="csvData">CSV data to be read</param>
        /// <returns>line separator used in CSV data</returns>
        private static string GetLineSeparator(string csvData) {
            string lineSeparator;
            if (csvData.Contains("\r\n")) {
                lineSeparator = "\r\n";
            } else if (csvData.Contains("\n")) {
                lineSeparator = "\n";
            } else {
                lineSeparator = "\r";
            }
            return lineSeparator;
        }

        /// <summary>
        /// Reads CSV data into presentable objects.
        /// </summary>
        /// <param name="csvData">CSV data to be read</param>
        /// <returns>CSV data as presentable objects</returns>
        public IEnumerable<T> Read(string csvData) {
            return this.Read(csvData, null);
        }

        /// <summary>
        /// Reads CSV data into presentable objects.
        /// </summary>
        /// <param name="csvData">CSV data to be read</param>
        /// <param name="validationResult">result of CSV data
        /// validation - no validation is done if validation result
        /// is null</param>
        /// <returns>CSV data as presentable objects</returns>
        private IEnumerable<T> Read(string csvData, ValidationResult validationResult) {
            string lineSeparator = CsvReader<T>.GetLineSeparator(csvData);
            IEnumerable<ViewFieldForEditableValue> viewFields = null;
            bool isInQuotation = false;
            var stringBuilder = new StringBuilder();
            var csvValues = new List<string>();
            if (!csvData.EndsWith(lineSeparator)) {
                csvData += lineSeparator;
            }
            foreach (var character in csvData) {
                if ('"' == character) {
                    isInQuotation = !isInQuotation;
                    if (isInQuotation && stringBuilder.Length > 0 && '"' != stringBuilder[stringBuilder.Length - 1]) {
                        if (null == validationResult) {
                            throw new FormatException(Resources.CsvQuotesError);
                        } else {
                            validationResult.ErrorMessage = Resources.CsvQuotesError;
                            yield break;
                        }
                    }
                }
                if (!isInQuotation && character == this.Separator) {
                    csvValues.Add(CsvReader<T>.RemoveQuotes(stringBuilder.ToString()));
                    stringBuilder.Clear();
                } else {
                    stringBuilder.Append(character);
                    if (!isInQuotation
                        && (lineSeparator.Length == 1 && character == lineSeparator[0]
                        || lineSeparator.Length == 2 && stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 2] == lineSeparator[0] && character == lineSeparator[1])) {
                        stringBuilder.Remove(stringBuilder.Length - lineSeparator.Length, lineSeparator.Length);
                        csvValues.Add(CsvReader<T>.RemoveQuotes(stringBuilder.ToString()));
                        stringBuilder.Clear();
                        if (null == viewFields) {
                            viewFields = this.ReadCsvHead(csvValues);
                        } else {
                            yield return this.ReadCsvLine(csvValues, viewFields, validationResult);
                            if (false == validationResult?.IsValid) {
                                yield break;
                            }
                        }
                        csvValues = new List<string>(csvValues.Count);
                    }
                }
            }
        }

        /// <summary>
        /// Reads a CSV file into presentable objects. The text
        /// encoding has to be UTF-8 with BOM, otherwise an exception
        /// will be thrown.
        /// </summary>
        /// <param name="file">CSV file to be read</param>
        /// <returns>CSV data as presentable objects</returns>
        public IEnumerable<T> Read(File file) {
            return this.Read(file.GetBytesAsString());
        }

        /// <summary>
        /// Reads a CSV file into presentable objects. The text
        /// encoding to be used can be specified.
        /// </summary>
        /// <param name="file">CSV file to be read</param>
        /// <param name="encoding">encoding to be used</param>
        /// <returns>CSV data as presentable objects</returns>
        public IEnumerable<T> Read(File file, Encoding encoding) {
            return this.Read(file.GetBytesAsString(encoding));
        }

        /// <summary>
        /// Reads a CSV head.
        /// </summary>
        /// <param name="csvValues">csv line to read</param>
        /// <returns>view fields for CSV head</returns>
        private IEnumerable<ViewFieldForEditableValue> ReadCsvHead(IList<string> csvValues) {
            foreach (string key in csvValues) {
                if (string.IsNullOrEmpty(key)) {
                    break; // trim noisy separators at the end of header line
                }
                ViewFieldForEditableValue viewField = null;
                foreach (var potentialViewField in this.View.ViewFields) {
                    var potentialViewFieldForEditableValue = potentialViewField as ViewFieldForEditableValue;
                    if (null != potentialViewFieldForEditableValue && potentialViewFieldForEditableValue.Key == key || potentialViewFieldForEditableValue.Title == key) {
                        viewField = potentialViewFieldForEditableValue;
                        break;
                    }
                }
                if (null == viewField) {
                    viewField = new ViewFieldForSingleLineText(key, key, Mandatoriness.Optional);
                }
                yield return viewField;
            }
        }

        /// <summary>
        /// Reads a CSV line.
        /// </summary>
        /// <param name="csvValues">values of CSV line to read</param>
        /// <param name="viewFields">view fields of CSV file</param>
        /// <param name="validationResult">result of CSV data
        /// validation - no validation is done if validation result
        /// is null</param>
        /// <returns>presentable object for CSV line</returns>
        private T ReadCsvLine(IList<string> csvValues, IEnumerable<ViewFieldForEditableValue> viewFields, ValidationResult validationResult) {
            var item = this.CreateNewItemDelegate();
            using (var csvValueEnumerator = csvValues.GetEnumerator()) {
                foreach (var viewField in viewFields) {
                    bool isCsvValuePresent = csvValueEnumerator.MoveNext();
                    var presentableField = item.FindPresentableField(viewField.Key);
                    if (null == presentableField) {
                        if (!this.IgnoreMissingFields) {
                            var errorMessage = string.Format(Resources.FieldMappingError, viewField.Key);
                            if (null == validationResult) {
                                throw new Exceptions.KeyNotFoundException(errorMessage);
                            } else {
                                validationResult.ErrorMessage = errorMessage;
                            }
                        }
                    } else if (isCsvValuePresent) {
                        if (presentableField is IPresentableFieldWithOptionDataProvider presentableFieldWithOptionDataProvider) {
                            presentableFieldWithOptionDataProvider.OptionDataProvider = this.OptionDataProvider;
                        }
                        if (presentableField is IPresentableFieldForIUser presentableFieldForIUser) {
                            presentableFieldForIUser.UserDirectory = this.OptionDataProvider.UserDirectory;
                        }
                        if (presentableField.IsForSingleElement) {
                            var presentableFieldForElement = presentableField as IPresentableFieldForElement;
                            if (null != presentableFieldForElement) {
                                var resolvedValue = this.ResolveValue(csvValueEnumerator.Current, presentableField.ParentPresentableObject, item, viewField);
                                if (!presentableFieldForElement.TrySetValueAsString(resolvedValue) && null != validationResult) {
                                    validationResult.ErrorMessage = string.Format(Resources.CsvValueError, resolvedValue, viewField.Key);
                                }
                            }
                        } else {
                            var presentableFieldForCollection = presentableField as IPresentableFieldForCollection;
                            var viewFieldForCollection = viewField as ViewFieldForCollection;
                            if (null != presentableFieldForCollection && null != viewFieldForCollection) {
                                string valueSeparator = ViewFieldForCollection.GetValueSeparator(viewFieldForCollection.GetValueSeparator(FieldRenderMode.ListTable));
                                foreach (var value in csvValueEnumerator.Current.Split(new string[] { valueSeparator }, StringSplitOptions.RemoveEmptyEntries)) {
                                    var resolvedValue = this.ResolveValue(value, presentableField.ParentPresentableObject, item, viewField);
                                    if (!presentableFieldForCollection.TryAddString(resolvedValue) && null != validationResult) {
                                        validationResult.ErrorMessage = string.Format(Resources.CsvValueError, resolvedValue, viewField.Key);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// Removes quotes from a string, that is starting
        ///  and ending in quotes
        /// </summary>
        /// <returns>string without leading and trailing
        /// quotes</returns>
        private static string RemoveQuotes(string stringWithQuotes) {
            string trimmedString = stringWithQuotes;
            if (stringWithQuotes.StartsWith("\"") && stringWithQuotes.EndsWith("\"")) {
                trimmedString = stringWithQuotes.Substring(1, stringWithQuotes.Length - 2);
                trimmedString = trimmedString.Replace("\"\"", "\"");
            }
            return trimmedString;
        }

        /// <summary>
        /// Resolves a value from lookup/option provider.
        /// </summary>
        /// <param name="value">value to be resolved</param>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to resolve value for</param>
        /// <param name="viewField">view field of value</param>
        /// <returns>resolved value</returns>
        private string ResolveValue(string value, IPresentableObject parentPresentableObject, T topmostPresentableObject, ViewFieldForEditableValue viewField) {
            if (viewField is IViewFieldWithOptionProvider viewFieldWithOptionProvider) {
                value = viewFieldWithOptionProvider.OptionProvider.FindKeyForValue(value, topmostPresentableObject, parentPresentableObject, this.OptionDataProvider);
            } else if (viewField is IViewFieldWithLookupProvider viewFieldWithLookupProvider) {
                var lookupProvider = viewFieldWithLookupProvider.GetLookupProvider();
                if (lookupProvider is PresentableObjectLookupProvider presentableObjectLookupProvider) {
                    value = presentableObjectLookupProvider.FindKeyForValue(value, topmostPresentableObject, this.OptionDataProvider)?.Id.ToString("N");
                } else if (lookupProvider is StringLookupProvider stringLookupProvider) {
                    value = stringLookupProvider.FindKeyForValue(value, topmostPresentableObject, this.OptionDataProvider);
                } else {
                    throw new PresentationException("Lookup provider of type " + lookupProvider.Type + " is not known.");
                }
            }
            return value;
        }

        /// <summary>
        /// Indicates whether CSV data is valid.
        /// </summary>
        /// <param name="csvData">CSV data to be verified</param>
        /// <returns>null if CSV data is valid, error message
        /// otherwise</returns>
        public string Validate(string csvData) {
            string errorMessage;
            var validationResult = new ValidationResult();
            if (new List<T>(this.Read(csvData, validationResult)).Count > -1 && !validationResult.IsValid) {
                errorMessage = validationResult.ErrorMessage;
            } else {
                errorMessage = null;
            }
            return errorMessage;
        }

        /// <summary>
        /// Indicates whether data in CSV file is valid.
        /// </summary>
        /// <param name="file">CSV file to be read</param>
        /// <returns>null if CSV data is valid, error message
        /// otherwise</returns>
        public string Validate(File file) {
            return this.Validate(file.GetBytesAsString());
        }

        /// <summary>
        /// Indicates whether data in CSV file is valid.
        /// </summary>
        /// <param name="file">CSV file to be read</param>
        /// <param name="encoding">encoding to be used</param>
        /// <returns>null if CSV data is valid, error message
        /// otherwise</returns>
        public string Validate(File file, Encoding encoding) {
            return this.Validate(file.GetBytesAsString(encoding));
        }

    }

}