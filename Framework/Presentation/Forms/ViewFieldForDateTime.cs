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

    using Framework.Model;
    using Framework.Persistence.Fields;
    using Framework.Properties;
    using System;

    /// <summary>
    /// Field for date and/or time to be presented in a view.
    /// </summary>
    public class ViewFieldForDateTime : ViewFieldForElement {

        /// <summary>
        /// Type of date and/or time.
        /// </summary>
        public DateTimeType DateTimeType {
            get { return (DateTimeType)this.dateTimeType.Value; }
            set { this.dateTimeType.Value = (int)value; }
        }
        private readonly PersistentFieldForInt dateTimeType =
            new PersistentFieldForInt(nameof(DateTimeType), 0);

        /// <summary>
        /// Maximum value that is valid.
        /// </summary>
        public DateTime MaxValue {
            get { return this.maxValue.Value; }
            set { this.maxValue.Value = value; }
        }
        private readonly PersistentFieldForDateTime maxValue =
            new PersistentFieldForDateTime(nameof(MaxValue), UtcDateTime.MaxValue);

        /// <summary>
        /// Minimum value that is valid.
        /// </summary>
        public DateTime MinValue {
            get { return this.minValue.Value; }
            set { this.minValue.Value = value; }
        }
        private readonly PersistentFieldForDateTime minValue =
            new PersistentFieldForDateTime(nameof(MinValue), UtcDateTime.MinValue);

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
        /// Allowed size of the steps between values. Set this to
        /// TimeSpan.Zero to allow any step.
        /// </summary>
        public TimeSpan Step {
            get {
                return new TimeSpan(this.step.Value);
            }
            set {
                if (value < TimeSpan.Zero) {
                    throw new ArgumentException("Step may not be < 0.", nameof(value));
                }
                this.step.Value = value.Ticks;
            }
        }
        private readonly PersistentFieldForLong step =
            new PersistentFieldForLong(nameof(Step), TimeSpan.Zero.Ticks);

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        public ViewFieldForDateTime()
            : base() {
            this.RegisterPersistentField(this.dateTimeType);
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
        /// values - set this to TimeSpan.Zero to allow any step</param>
        /// <param name="dateTimeType">type of date and/or time</param>
        public ViewFieldForDateTime(string title, string key, Mandatoriness mandatoriness, TimeSpan step, DateTimeType dateTimeType)
            : this(title, mandatoriness, step, dateTimeType) {
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
        /// values - set this to TimeSpan.Zero to allow any step</param>
        /// <param name="dateTimeType">type of date and/or time</param>
        public ViewFieldForDateTime(string title, string[] keyChain, Mandatoriness mandatoriness, TimeSpan step, DateTimeType dateTimeType)
            : this(title, mandatoriness, step, dateTimeType) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="step">allowed size of the steps between
        /// values - set this to TimeSpan.Zero to allow any step</param>
        /// <param name="dateTimeType">type of date and/or time</param>
        private ViewFieldForDateTime(string title, Mandatoriness mandatoriness, TimeSpan step, DateTimeType dateTimeType)
            : this() {
            this.DateTimeType = dateTimeType;
            this.Mandatoriness = mandatoriness;
            this.Step = step;
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
            return new PresentableFieldForNullableDateTime(parentPresentableObject, this.Key);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public override string GetDefaultErrorMessage() {
            string errorMessage;
            if (UtcDateTime.MinValue == this.MinValue && UtcDateTime.MaxValue == this.MaxValue) {
                if (DateTimeType.Date == this.DateTimeType) {
                    errorMessage = Resources.PleaseEnterAValidDateForThisField;
                } else if (DateTimeType.DateAndTime == this.DateTimeType || DateTimeType.LocalDateAndTime == this.DateTimeType) {
                    errorMessage = Resources.PleaseEnterAValidDateAndTimeForThisField;
                } else if (DateTimeType.Month == this.DateTimeType) {
                    errorMessage = Resources.PleaseEnterAValidMonthForThisField;
                } else if (DateTimeType.Time == this.DateTimeType) {
                    errorMessage = Resources.PleaseEnterAValidTimeForThisField;
                } else if (DateTimeType.Week == this.DateTimeType) {
                    errorMessage = Resources.PleaseEnterAValidWeekForThisField;
                } else {
                    throw new InvalidOperationException("DateTimeType \"" + this.DateTimeType.ToString() + "\" is not known.");
                }
            } else {
                errorMessage = this.GetErrorMessageForValueOutOfBounds(this.MinValue, this.MaxValue);
            }
            errorMessage += " " + this.GetInfoMessageAboutManditoriness();
            return errorMessage;
        }

        /// <summary>
        /// Gets the error message for value out of bounds.
        /// </summary>
        /// <param name="minValue">minimum value that is valid</param>
        /// <param name="maxValue">maximum value that is valid</param>
        /// <returns>error message for value out of bounds</returns>
        protected string GetErrorMessageForValueOutOfBounds(DateTime minValue, DateTime maxValue) {
            string errorMessage;
            if (DateTimeType.Date == this.DateTimeType) {
                errorMessage = Resources.PleaseEnterAValidDateBetween0And1ForThisField;
            } else if (DateTimeType.DateAndTime == this.DateTimeType || DateTimeType.LocalDateAndTime == this.DateTimeType) {
                errorMessage = Resources.PleaseEnterAValidDateAndTimeBetween0And1ForThisField;
            } else if (DateTimeType.Month == this.DateTimeType) {
                errorMessage = Resources.PleaseEnterAValidMonthBetween0And1ForThisField;
            } else if (DateTimeType.Time == this.DateTimeType) {
                errorMessage = Resources.PleaseEnterAValidTimeBetween0And1ForThisField;
            } else if (DateTimeType.Week == this.DateTimeType) {
                errorMessage = Resources.PleaseEnterAValidWeekBetween0And1ForThisField;
            } else {
                throw new InvalidOperationException("DateTimeType \"" + this.DateTimeType.ToString() + "\" is not known.");
            }
            errorMessage = string.Format(errorMessage, UtcDateTime.FormatAsReadOnlyValue(minValue, this.DateTimeType), UtcDateTime.FormatAsReadOnlyValue(maxValue, this.DateTimeType));
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
            string value;
            if (null == presentableField.ValueAsObject) {
                value = string.Empty;
            } else {
                var dateTime = presentableField.ValueAsObject as DateTime?;
                value = UtcDateTime.FormatAsReadOnlyValue(dateTime.Value, this.DateTimeType);
            }
            return value;
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
            DateTime? value;
            if (UtcDateTime.TryParse(readOnlyValue, out DateTime parsedValue)) {
                value = parsedValue;
            } else {
                value = null;
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
            string value = presentableField.ValueAsString;
            if (string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(value)) {
                if (UtcDateTime.TryParse(value, out DateTime valueAsDateTime)) {
                    if (valueAsDateTime < this.MinValue
                        || valueAsDateTime > this.MaxValue
                        || (this.Step > TimeSpan.Zero && (this.MinValue.Ticks - valueAsDateTime.Ticks) % this.Step.Ticks != 0)) {
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