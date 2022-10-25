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
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Abstract base class for fields for numbers to be presented in
    /// a view.
    /// </summary>
    public abstract class ViewFieldForNumberWithoutUnit : ViewFieldForElementWithPlaceholder {

        /// <summary>
        /// Number of decimal places based on step.
        /// </summary>
        public byte DecimalPlaces {
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
        /// True if number is supposed to be rendered with thousands
        /// separators, false otherwise.
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
        public ViewFieldForNumberWithoutUnit()
            : base() {
            this.RegisterPersistentField(this.hasThousandsSeparators);
            this.RegisterPersistentField(this.isRange);
            this.RegisterPersistentField(this.maxValue);
            this.RegisterPersistentField(this.minValue);
            this.RegisterPersistentField(this.optionProvider);
            this.RegisterPersistentField(this.step);
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
                errorMessage = this.GetErrorMessageForValueOutOfBounds(this.MinValue, this.MaxValue);
                errorMessage += ' ' + this.GetInfoMessageAboutManditoriness();
            }
            return errorMessage;
        }

        /// <summary>
        /// Gets the error message for value out of bounds.
        /// </summary>
        /// <param name="minValue">minimum value that is valid</param>
        /// <param name="maxValue">maximum value that is valid</param>
        /// <returns>error message for value out of bounds</returns>
        protected string GetErrorMessageForValueOutOfBounds(decimal minValue, decimal maxValue) {
            string errorMessage;
            if (decimal.MinValue == minValue) {
                errorMessage = string.Format(Resources.PleaseEnterAValidValueLessThan0ForThisField, maxValue);
            } else if (decimal.MaxValue == maxValue) {
                errorMessage = string.Format(Resources.PleaseEnterAValidValueGreaterThan0ForThisField, minValue);
            } else {
                errorMessage = string.Format(Resources.PleaseEnterAValidValueBetween0And1ForThisField, minValue, maxValue);
            }
            return errorMessage;
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
            string value = presentableField.ValueAsString;
            if (string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(value)) {
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valueAsNumber)) {
                    decimal stepBase;
                    if (decimal.MinValue == this.MinValue) {
                        stepBase = 0;
                    } else {
                        stepBase = this.MinValue;
                    }
                    try {
                        if (valueAsNumber < this.MinValue
                            || valueAsNumber > this.MaxValue
                            || (this.Step > 0m && (stepBase - valueAsNumber) % this.Step != 0m)) {
                            errorMessage = this.GetDefaultErrorMessage();
                        }
                    } catch (OverflowException) {
                        errorMessage = this.GetDefaultErrorMessage();
                    }
                } else {
                    errorMessage = this.GetDefaultErrorMessage();
                }
            }
            return errorMessage;
        }

    }

}