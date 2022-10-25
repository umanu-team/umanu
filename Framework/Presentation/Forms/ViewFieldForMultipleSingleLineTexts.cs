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
    /// Field for multiple single-line texts to be presented in a
    /// view.
    /// </summary>
    public class ViewFieldForMultipleSingleLineTexts : ViewFieldForCollectionWithPlaceholder {

        /// <summary>
        /// Maximum number of characters of value.
        /// </summary>
        public ulong MaxLength {
            get { return this.maxLength.Value; }
            set { this.maxLength.Value = value; }
        }
        private readonly PersistentFieldForULong maxLength =
            new PersistentFieldForULong(nameof(MaxLength), ulong.MaxValue);

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
        /// Regular expression that must match for values of this
        /// field to be valid.
        /// </summary>
        public string ValidationPattern {
            get { return this.validationPattern.Value; }
            set { this.validationPattern.Value = value; }
        }
        private readonly PersistentFieldForString validationPattern =
            new PersistentFieldForString(nameof(ValidationPattern));

        /// <summary>
        /// Value separator to be used in form render mode.
        /// </summary>
        public ValueSeparator ValueSeparator {
            get { return (ValueSeparator)this.valueSeparator.Value; }
            set { this.valueSeparator.Value = (int)value; }
        }
        private readonly PersistentFieldForInt valueSeparator =
            new PersistentFieldForInt(nameof(ValueSeparator), (int)ValueSeparator.LineBreak);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForMultipleSingleLineTexts()
            : base() {
            this.RegisterPersistentField(this.maxLength);
            this.RegisterPersistentField(this.optionProvider);
            this.RegisterPersistentField(this.validationPattern);
            this.RegisterPersistentField(this.valueSeparator);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForMultipleSingleLineTexts(string title, string key, Mandatoriness mandatoriness)
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
        public ViewFieldForMultipleSingleLineTexts(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForMultipleSingleLineTexts(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            if (ulong.MaxValue == this.MaxLength) {
                errorMessage = base.GetDefaultErrorMessage();
            } else {
                if (this.Limit < 2) {
                    if (1 == this.MaxLength) {
                        errorMessage = Resources.PleaseEnterAValidValueWithAtMostOneCharacterForThisField;
                    } else {
                        errorMessage = string.Format(Resources.PleaseEnterAValidValueWithAtMost0CharactersForThisField, this.MaxLength);
                    }
                } else {
                    if (1 == this.MaxLength) {
                        errorMessage = Resources.PleaseEnterValidValuesWithAtMostOneCharacterForThisField;
                    } else {
                        errorMessage = string.Format(Resources.PleaseEnterValidValuesWithAtMost0CharactersForThisField, this.MaxLength);
                    }
                    if (this.Limit < uint.MaxValue) {
                        errorMessage += " ";
                        errorMessage += string.Format(Resources.UpTo0ValuesAreAllowed, this.Limit);
                    }
                }
                errorMessage += " " + this.GetInfoMessageAboutManditoriness();
            }
            return errorMessage;
        }

        /// <summary>
        /// Gets the characters to use for separating values.
        /// </summary>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <returns>characters to use for separating values</returns>
        public override ValueSeparator GetValueSeparator(FieldRenderMode renderMode) {
            ValueSeparator valueSeparator;
            if (FieldRenderMode.Form == renderMode) {
                if (null != this.OptionProvider && this.Limit > 1) {
                    valueSeparator = ValueSeparator.LineBreak;
                } else {
                    valueSeparator = this.ValueSeparator;
                }
            } else {
                valueSeparator = ValueSeparator.Comma;
            }
            return valueSeparator;
        }

        /// <summary>
        /// Returns null if the specified values are valid, an error
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
        /// <returns>null if the specified values are valid, error
        /// message otherwise</returns>
        public override string Validate(IPresentableFieldForCollection presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            var viewFieldForElement = new ViewFieldForSingleLineText(this.Key, this.Title, Mandatoriness.Optional) {
                DescriptionForEditMode = this.DescriptionForEditMode,
                DescriptionForViewMode = this.DescriptionForViewMode,
                MaxLength = this.MaxLength,
                OptionProvider = this.OptionProvider,
                Placeholder = this.Placeholder,
                ValidationPattern = this.ValidationPattern
            };
            return base.Validate(presentableField, validityCheck, presentableObject, optionDataProvider, viewFieldForElement);
        }

    }

}