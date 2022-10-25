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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Field for multiple numbers to be presented in a view.
    /// </summary>
    public class ViewFieldForMultipleNumbers : ViewFieldForCollectionWithPlaceholder {

        /// <summary>
        /// Number of decimal places based on step.
        /// </summary>
        private byte DecimalPlaces {
            get {
                return BitConverter.GetBytes(decimal.GetBits(this.Step)[3])[2];
            }
        }

        /// <summary>
        /// Gets the format string to be used for rendering the
        /// values of this view field.
        /// </summary>
        public string FormatString {
            get {
                var formatStringBuilder = new StringBuilder();
                if (this.HasThousandsSeparators) {
                    formatStringBuilder.Append("#,");
                }
                formatStringBuilder.Append("0.");
                if (decimal.Zero == this.Step) {
                    for (byte b = 0; b < byte.MaxValue; b++) {
                        formatStringBuilder.Append('#');
                    }
                } else {
                    for (byte b = 0; b < this.DecimalPlaces; b++) {
                        formatStringBuilder.Append('0');
                    }
                }
                return formatStringBuilder.ToString();
            }
        }

        /// <summary>
        /// True if numbers are supposed to be rendered with
        /// thousands separators, false otherwise.
        /// </summary>
        public bool HasThousandsSeparators {
            get { return this.hasThousandsSeparators.Value; }
            set { this.hasThousandsSeparators.Value = value; }
        }
        private readonly PersistentFieldForBool hasThousandsSeparators =
            new PersistentFieldForBool(nameof(HasThousandsSeparators), true);

        /// <summary>
        /// True to display this field using a range control, false
        /// to display this field using a test box control.
        /// </summary>
        public bool IsRange {
            get { return this.isRange.Value; }
            set { this.isRange.Value = value; }
        }
        private readonly PersistentFieldForBool isRange =
            new PersistentFieldForBool(nameof(IsRange), false);

        /// <summary>
        /// Maximum value that is valid.
        /// </summary>
        public decimal MaxValue {
            get { return this.maxValue.Value; }
            set { this.maxValue.Value = value; }
        }
        private readonly PersistentFieldForDecimal maxValue =
            new PersistentFieldForDecimal(nameof(MaxValue), decimal.MaxValue);

        /// <summary>
        /// Minimum value that is valid.
        /// </summary>
        public decimal MinValue {
            get { return this.minValue.Value; }
            set { this.minValue.Value = value; }
        }
        private readonly PersistentFieldForDecimal minValue =
            new PersistentFieldForDecimal(nameof(MinValue), decimal.Zero);

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
        /// Allowed size of the steps between values - set this to 0
        /// to allow any value or to 1 to allow integer values only.
        /// </summary>
        public decimal Step {
            get { return this.step.Value; }
            set {
                if (value < 0m) {
                    throw new ArgumentException("Step may not be < 0.", nameof(value));
                }
                this.step.Value = value;
            }
        }
        private readonly PersistentFieldForDecimal step =
            new PersistentFieldForDecimal(nameof(Step), decimal.Zero);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForMultipleNumbers()
            : base() {
            this.RegisterPersistentField(this.hasThousandsSeparators);
            this.RegisterPersistentField(this.isRange);
            this.RegisterPersistentField(this.maxValue);
            this.RegisterPersistentField(this.minValue);
            this.RegisterPersistentField(this.optionProvider);
            this.RegisterPersistentField(this.step);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        public ViewFieldForMultipleNumbers(string title, string key, Mandatoriness mandatoriness, decimal step)
            : this(title, mandatoriness, step) {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        public ViewFieldForMultipleNumbers(string title, string[] keyChain, Mandatoriness mandatoriness, decimal step)
            : this(title, mandatoriness, step) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to 0 to allow any value or to 1 to
        /// allow integer values only</param>
        private ViewFieldForMultipleNumbers(string title, Mandatoriness mandatoriness, decimal step)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
            this.Step = step;
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            if (decimal.MinValue == this.MinValue && decimal.MaxValue == this.MaxValue) {
                errorMessage = base.GetDefaultErrorMessage();
            } else {
                if (this.Limit < 2) {
                    if (decimal.MinValue == this.MinValue) {
                        errorMessage = string.Format(Resources.PleaseEnterAValidValueLessThan0ForThisField, this.MaxValue);
                    } else if (decimal.MaxValue == this.MaxValue) {
                        errorMessage = string.Format(Resources.PleaseEnterAValidValueGreaterThan0ForThisField, this.MinValue);
                    } else {
                        errorMessage = string.Format(Resources.PleaseEnterAValidValueBetween0And1ForThisField, this.MinValue, this.MaxValue);
                    }
                } else {
                    if (decimal.MinValue == this.MinValue) {
                        errorMessage = string.Format(Resources.PleaseEnterValidValuesLessThan0ForThisField, this.MaxValue);
                    } else if (decimal.MaxValue == this.MaxValue) {
                        errorMessage = string.Format(Resources.PleaseEnterValidValuesGreaterThan0ForThisField, this.MinValue);
                    } else {
                        errorMessage = string.Format(Resources.PleaseEnterValidValuesBetween0And1ForThisField, this.MinValue, this.MaxValue);
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
        /// Gets the read-only value of a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to get
        /// read-only value of</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>read-only value of presentable field</returns>
        public override IEnumerable<string> GetReadOnlyValuesFor(IPresentableFieldForCollection presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            foreach (string readOnlyValue in presentableField.GetValuesAsString()) {
                if (long.TryParse(readOnlyValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long readOnlyLongValue)) {
                    yield return readOnlyLongValue.ToString(this.FormatString);
                } else if (decimal.TryParse(readOnlyValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal readOnlyDecimalValue)) {
                    yield return readOnlyDecimalValue.ToString(this.FormatString);
                }
            }
        }

        /// <summary>
        /// Gets the characters to use for separating values.
        /// </summary>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <returns>characters to use for separating values</returns>
        public override ValueSeparator GetValueSeparator(FieldRenderMode renderMode) {
            return ValueSeparator.Semicolon;
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
            var viewFieldForElement = new ViewFieldForNumber(this.Key, this.Title, Mandatoriness.Optional, this.Step);
            viewFieldForElement.DescriptionForEditMode = this.DescriptionForEditMode;
            viewFieldForElement.DescriptionForViewMode = this.DescriptionForViewMode;
            viewFieldForElement.IsRange = this.IsRange;
            viewFieldForElement.MaxValue = this.MaxValue;
            viewFieldForElement.MinValue = this.MinValue;
            viewFieldForElement.OptionProvider = this.OptionProvider;
            viewFieldForElement.Placeholder = this.Placeholder;
            return base.Validate(presentableField, validityCheck, presentableObject, optionDataProvider, viewFieldForElement);
        }

    }

}