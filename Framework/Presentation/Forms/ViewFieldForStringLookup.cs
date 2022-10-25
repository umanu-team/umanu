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

    using Framework.Persistence.Fields;

    /// <summary>
    /// Field for string lookup values to be presented in a view.
    /// </summary>
    public class ViewFieldForStringLookup : ViewFieldForLookup {

        /// <summary>
        /// Indicates whether value not contained in lookup provider
        /// is allowed.
        /// </summary>
        public bool IsFillInAllowed {
            get { return this.isFillInAllowed.Value; }
            set { this.isFillInAllowed.Value = value; }
        }
        private readonly PersistentFieldForBool isFillInAllowed =
            new PersistentFieldForBool(nameof(IsFillInAllowed), false);

        /// <summary>
        /// Provider for suggested values.
        /// </summary>
        public StringLookupProvider LookupProvider {
            get { return this.lookupProvider.Value; }
            set { this.lookupProvider.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<StringLookupProvider> lookupProvider =
            new PersistentFieldForPersistentObject<StringLookupProvider>(nameof(LookupProvider), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForStringLookup()
            : base() {
            this.RegisterPersistentField(this.isFillInAllowed);
            this.RegisterPersistentField(this.lookupProvider);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a value
        /// in this field is required</param>
        public ViewFieldForStringLookup(string title, string key, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForStringLookup(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForStringLookup(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

        /// <summary>
        /// Creates a presentable field that can hold the value of
        /// view field.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of created field</param>
        /// <returns>presentable field that can hold the value of
        /// view field</returns>
        public override IPresentableFieldForElement CreatePresentableField(IPresentableObject parentPresentableObject) {
            return new PresentableFieldForString(parentPresentableObject, this.Key);
        }

        /// <summary>
        /// Gets a boolean value indicating whether values not
        /// contained in lookup provider are allowed.
        /// </summary>
        /// <returns>true if values not contained in lookup provider
        /// are allowed, false otherwise</returns>
        public sealed override bool GetIsFillInAllowed() {
            return this.IsFillInAllowed;
        }

        /// <summary>
        /// Gets the provider for suggested values.
        /// </summary>
        public sealed override LookupProvider GetLookupProvider() {
            return this.LookupProvider;
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
        public override string GetReadOnlyValueFor(IPresentableFieldForElement presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            var readOnlyValue = this.LookupProvider.FindValueForKey(presentableField.ValueAsString, presentableObject, optionDataProvider);
            if (string.IsNullOrEmpty(readOnlyValue) && this.GetIsFillInAllowed()) {
                readOnlyValue = presentableField.ValueAsString;
            }
            return readOnlyValue;
        }

        /// <summary>
        /// Gets the value as object for a read-only value.
        /// </summary>
        /// <param name="readOnlyValue">read-only value to get value
        /// as object for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>value as object for read-only value</returns>
        public override object ParseReadOnlyValue(string readOnlyValue, IOptionDataProvider optionDataProvider) {
            var value = this.LookupProvider.FindKeyForValue(readOnlyValue, new PresentableObject(), optionDataProvider);
            if (string.IsNullOrEmpty(value) && this.GetIsFillInAllowed()) {
                value = readOnlyValue;
            }
            return value;
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
            string key = presentableField.ValueAsString;
            if (string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(key)) {
                if (!this.GetIsFillInAllowed() && !this.LookupProvider.ContainsKey(key, presentableObject, optionDataProvider)) {
                    errorMessage = this.GetDefaultErrorMessage();
                }
            }
            return errorMessage;
        }

    }

}