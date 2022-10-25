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
    using System.Collections.Generic;

    /// <summary>
    /// Field for collection of choices to be presented in a view.
    /// </summary>
    public abstract class ViewFieldForMultipleChoices : ViewFieldForCollection, IViewFieldWithOptionProvider {

        /// <summary>
        /// Indicates whether automatic selection is enabled. If set
        /// to true, all options will be selected in forms
        /// automatically as long as no option is selected in
        /// presentable field.
        /// </summary>
        public bool IsAutoSelectionEnabled {
            get { return this.isAutoSelectionEnabled.Value; }
            set { this.isAutoSelectionEnabled.Value = value; }
        }
        private readonly PersistentFieldForBool isAutoSelectionEnabled =
            new PersistentFieldForBool(nameof(IsAutoSelectionEnabled), false);

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
        public ViewFieldForMultipleChoices()
            : base() {
            this.RegisterPersistentField(this.isAutoSelectionEnabled);
            this.RegisterPersistentField(this.optionDisplayStyle);
            this.RegisterPersistentField(this.optionProvider);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            if (this.Limit < 2) {
                errorMessage = Resources.PleaseSelectAValidValueForThisField;
            } else {
                errorMessage = Resources.PleaseSelectOneOrMoreValidValuesForThisField;
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
        /// <param name="topmostPresentableObject">topmost 
        /// presentable parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>read-only value of presentable field</returns>
        public override IEnumerable<string> GetReadOnlyValuesFor(IPresentableFieldForCollection presentableField, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            foreach (var readOnlyOptionForKey in this.OptionProvider.FindReadOnlyOptionsForKeys(presentableField.GetValuesAsString(), presentableField.ParentPresentableObject, topmostPresentableObject, optionDataProvider)) {
                yield return readOnlyOptionForKey.Value;
            }
        }

        /// <summary>
        /// Gets the characters to use for separating values.
        /// </summary>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <returns>characters to use for separating values</returns>
        public override ValueSeparator GetValueSeparator(FieldRenderMode renderMode) {
            ValueSeparator valueSeparator;
            if (FieldRenderMode.Form == renderMode && this.IsReadOnly) {
                valueSeparator = ValueSeparator.LineBreak;
            } else { // for editable values web browsers use commas in post-backs
                valueSeparator = ValueSeparator.Comma;
            }
            return valueSeparator;
        }

    }

}