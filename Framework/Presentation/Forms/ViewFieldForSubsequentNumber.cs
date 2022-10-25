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

    using Framework.Presentation.Exceptions;
    using Persistence.Fields;

    /// <summary>
    /// Field for subsequent number of another number to be presented
    /// in a view.
    /// </summary>
    public class ViewFieldForSubsequentNumber : ViewFieldForNumber {

        /// <summary>
        /// Internal key of field with previous number.
        /// </summary>
        public string PreviousFieldKey {
            get { return this.previousFieldKey.Value; }
            set { this.previousFieldKey.Value = value; }
        }
        private readonly PersistentFieldForString previousFieldKey =
            new PersistentFieldForString(nameof(PreviousFieldKey));

        /// <summary>
        /// Internal key chain of field with previous number.
        /// </summary>
        public string[] PreviousFieldKeyChain {
            get { return Model.KeyChain.FromKey(this.PreviousFieldKey); }
            set { this.PreviousFieldKey = Model.KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        public ViewFieldForSubsequentNumber()
            : base() {
            this.RegisterPersistentField(this.previousFieldKey);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a value
        /// in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        /// <param name="previousFieldKey">internal key of field with
        /// previous date/time</param>
        public ViewFieldForSubsequentNumber(string title, string key, Mandatoriness mandatoriness, decimal step, string previousFieldKey)
            : base(title, key, mandatoriness, step) {
            this.RegisterPersistentField(this.previousFieldKey);
            this.PreviousFieldKey = previousFieldKey;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a value
        /// in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        /// <param name="previousFieldKeyChain">internal key chain of
        /// field with previous date/time</param>
        public ViewFieldForSubsequentNumber(string title, string[] keyChain, Mandatoriness mandatoriness, decimal step, string[] previousFieldKeyChain)
            : base(title, keyChain, mandatoriness, step) {
            this.RegisterPersistentField(this.previousFieldKey);
            this.PreviousFieldKeyChain = previousFieldKeyChain;
        }

        /// <summary>
        /// Returns null if the specified value is valid, an error 
        /// message otherwise.
        /// </summary>
        /// <param name="presentableField">presentable field to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>null if the specified value is valid, error
        /// message otherwise</returns>
        public override string Validate(IPresentableFieldForElement presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string errorMessage = base.Validate(presentableField, validityCheck, presentableObject, optionDataProvider);
            string subsequentValue = presentableField.ValueAsString;
            var previousField = presentableField.ParentPresentableObject.FindPresentableField(this.PreviousFieldKeyChain) as IPresentableFieldForElement;
            if (null == previousField) {
                previousField = presentableObject.FindPresentableField(this.PreviousFieldKeyChain) as IPresentableFieldForElement;
            }
            if (null == previousField) {
                throw new KeyNotFoundException("Presentable field with key \"" + Model.KeyChain.ToKey(this.PreviousFieldKeyChain) + "\" cannot be found.");
            } else {
                string previousValue = previousField.ValueAsString;
                if (string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(subsequentValue) && !string.IsNullOrEmpty(previousValue)) {
                    if (decimal.TryParse(subsequentValue, out decimal subsequentValueAsDecimal) && decimal.TryParse(previousValue, out decimal previousValueAsDecimal)) {
                        if (subsequentValueAsDecimal < previousValueAsDecimal) {
                            errorMessage = this.GetErrorMessageForValueOutOfBounds(previousValueAsDecimal, this.MaxValue);
                            errorMessage += " " + this.GetInfoMessageAboutManditoriness();
                        }
                    } else {
                        errorMessage = this.GetDefaultErrorMessage();
                    }
                }
            }
            return errorMessage;
        }

    }

}