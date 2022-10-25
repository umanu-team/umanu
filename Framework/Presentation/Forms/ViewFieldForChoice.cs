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
    using Framework.Properties;

    /// <summary>
    /// Field for choice to be presented in a view.
    /// </summary>
    public abstract class ViewFieldForChoice : ViewFieldForElement, IViewFieldWithOptionProvider {

        /// <summary>
        /// Type of control to use for displaying options. Usually
        /// this should be set to automatic.
        /// </summary>
        public OptionControlType OptionControlType {
            get { return (OptionControlType)this.optionControlType.Value; }
            set { this.optionControlType.Value = (int)value; }
        }
        private readonly PersistentFieldForInt optionControlType =
            new PersistentFieldForInt(nameof(OptionControlType), (int)OptionControlType.Automatic);

        /// <summary>
        /// Style of how to display an option.
        /// </summary>
        public OptionDisplayStyle OptionDisplayStyle {
            get { return (OptionDisplayStyle)this.optionDisplayStyle.Value; }
            set { this.optionDisplayStyle.Value = (int)value; }
        }
        private readonly PersistentFieldForInt optionDisplayStyle =
            new PersistentFieldForInt(nameof(OptionDisplayStyle), (int)OptionDisplayStyle.IconWithTextFallback);

        /// <summary>
        /// Provider for suggested values.
        /// </summary>
        public OptionProvider OptionProvider {
            get { return this.optionProvider.Value; }
            set { this.optionProvider.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<OptionProvider> optionProvider =
            new PersistentFieldForPersistentObject<OptionProvider>(nameof(OptionProvider), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForChoice()
            : base() {
            this.RegisterPersistentField(this.optionControlType);
            this.RegisterPersistentField(this.optionDisplayStyle);
            this.RegisterPersistentField(this.optionProvider);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage = Resources.PleaseSelectAValidValueForThisField;
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
        public override string GetReadOnlyValueFor(IPresentableFieldForElement presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            return this.GetReadOnlyValueFor(presentableField, presentableObject, optionDataProvider, presentableField.ValueAsString);
        }

        /// <summary>
        /// Gets the read-only value of a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to get
        /// read-only value of</param>
        /// <param name="topmostPresentableObject">topmost presentable
        /// parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="selectedKey">key to get read-only value for</param>
        /// <returns>read-only value of presentable field</returns>
        public string GetReadOnlyValueFor(IPresentableFieldForElement presentableField, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider, string selectedKey) {
            return this.OptionProvider.FindReadOnlyValueForKey(selectedKey, presentableField.ParentPresentableObject, topmostPresentableObject, optionDataProvider);
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
            return this.OptionProvider.FindKeyForValue(readOnlyValue, new PresentableObject(), new PresentableObject(), optionDataProvider);
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
                var keyCount = this.OptionProvider.CountKey(key, presentableField.ParentPresentableObject, presentableObject, optionDataProvider);
                if (keyCount < 1) {
                    errorMessage = this.GetDefaultErrorMessage();
                } else if (keyCount > 1) {
                    errorMessage = Resources.TheSelectedValueDoesNotHaveAUniqueKey + " " + this.GetDefaultErrorMessage();
                }
            }
            return errorMessage;
        }

    }

}