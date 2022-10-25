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

    using Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Properties;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Field for collections to be presented in a view.
    /// </summary>
    public abstract class ViewFieldForCollection : ViewFieldForEditableValue {

        /// <summary>
        /// Maximum number of values that may be set.
        /// </summary>
        public uint Limit {
            get { return this.limit.Value; }
            set { this.limit.Value = value; }
        }
        private readonly PersistentFieldForUInt limit =
            new PersistentFieldForUInt(nameof(Limit), uint.MaxValue);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForCollection()
            : base() {
            this.RegisterPersistentField(this.limit);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            if (this.Limit < 2) {
                errorMessage = Resources.PleaseEnterAValidValueForThisField;
            } else {
                errorMessage = Resources.PleaseEnterValidValuesForThisField;
                if (this.Limit < uint.MaxValue) {
                    errorMessage += " ";
                    errorMessage += string.Format(Resources.UpTo0ValuesAreAllowed, this.Limit);
                }
            }
            errorMessage += " " + this.GetInfoMessageAboutManditoriness();
            return errorMessage;
        }

        /// <summary>
        /// Gets the read-only value of a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to get
        /// read-only value of</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>read-only value of presentable field</returns>
        public sealed override string GetReadOnlyValueFor(IPresentableField presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string valueSeparator = ViewFieldForCollection.GetValueSeparator(this.GetValueSeparator(FieldRenderMode.ListTable));
            var valueBuilder = new StringBuilder();
            foreach (var stringValue in this.GetReadOnlyValuesFor(presentableField as IPresentableFieldForCollection, presentableObject, optionDataProvider)) {
                if (valueBuilder.Length > 0) {
                    valueBuilder.Append(valueSeparator);
                }
                valueBuilder.Append(stringValue);
            }
            return valueBuilder.ToString();
        }

        /// <summary>
        /// Gets the read-only value of a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to get
        /// read-only value of</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>read-only value of presentable field</returns>
        public virtual IEnumerable<string> GetReadOnlyValuesFor(IPresentableFieldForCollection presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return presentableField.GetValuesAsString();
        }

        /// <summary>
        /// Gets the characters to use for separating values.
        /// </summary>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <returns>characters to use for separating values</returns>
        public abstract ValueSeparator GetValueSeparator(FieldRenderMode renderMode);

        /// <summary>
        /// Gets a value separator as string.
        /// </summary>
        /// <param name="valueSeparator">type of value separator to
        /// get</param>
        /// <returns>value separator as string</returns>
        internal static string GetValueSeparator(ValueSeparator valueSeparator) {
            string text;
            if (ValueSeparator.None == valueSeparator) {
                text = null;
            } else if (ValueSeparator.Comma == valueSeparator) {
                text = ", ";
            } else if (ValueSeparator.Semicolon == valueSeparator) {
                text = "; ";
            } else if (ValueSeparator.Space == valueSeparator) {
                text = " ";
            } else if (ValueSeparator.LineBreak == valueSeparator) {
                text = "\n";
            } else {
                throw new PresentationException("Value seperator \"" + valueSeparator + "\" is unknown.");
            }
            return text;
        }

        /// <summary>
        /// Returns true if required values are set, false otherwise.
        /// </summary>
        /// <param name="presentableField">presentable field to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <returns>null if the required values are set, error
        /// message otherwise</returns>
        private string Validate(IPresentableFieldForCollection presentableField, ValidityCheck validityCheck) {
            string errorMessage = null;
            bool isMandatory = Mandatoriness.Required == this.Mandatoriness || (ValidityCheck.Strict == validityCheck && Mandatoriness.Desired == this.Mandatoriness);
            if (isMandatory) {
                using (var enumerator = presentableField.GetValuesAsString().GetEnumerator()) {
                    if (!enumerator.MoveNext()) {
                        errorMessage = this.GetDefaultErrorMessage();
                    }
                }
            }
            if (presentableField.Count > this.Limit) {
                errorMessage = this.GetDefaultErrorMessage();
            }
            return errorMessage;
        }

        /// <summary>
        /// Returns null if the specified values are valid, an error message
        /// otherwise.
        /// </summary>
        /// <param name="presentableField">presentable field to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>null if the specified values are valid, error
        /// message otherwise</returns>
        public virtual string Validate(IPresentableFieldForCollection presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return this.Validate(presentableField, validityCheck);
        }

        /// <summary>
        /// Returns null if the specified values are valid, an error
        /// message otherwise. This method checks each value against
        /// the method ViewFieldForElement.IsValidValue(string) of a
        /// provided view field for single elements.
        /// </summary>
        /// <param name="presentableField">presentable field to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="viewFieldForElement">view field to check
        /// each value against</param>
        /// <returns>null if the specified values are valid, error
        /// message otherwise</returns>
        protected string Validate(IPresentableFieldForCollection presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider, ViewFieldForElement viewFieldForElement) {
            string errorMessage = this.Validate(presentableField, validityCheck);
            if (string.IsNullOrEmpty(errorMessage)) {
                foreach (string fieldValue in presentableField.GetValuesAsString()) {
                    var presentableFieldForElement = new PresentableFieldForString(presentableField.ParentPresentableObject, presentableField.Key, fieldValue);
                    presentableFieldForElement.IsReadOnly = presentableField.IsReadOnly;
                    errorMessage = viewFieldForElement.Validate(presentableFieldForElement, validityCheck, presentableObject, optionDataProvider);
                    if (!string.IsNullOrEmpty(errorMessage)) {
                        break;
                    }
                }
            }
            return errorMessage;
        }

    }

}