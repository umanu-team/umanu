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

    using Framework.Persistence;
    using System;
    using System.Globalization;

    /// <summary>
    /// Field for element of type nullable byte to be presented.
    /// </summary>
    public sealed class PresentableFieldForNullableByte : PresentableFieldForElement<byte?> {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.NullableByte;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of presentable field</param>
        public PresentableFieldForNullableByte(IPresentableObject parentPresentableObject, string key)
            : base(parentPresentableObject, key) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of presentable field</param>
        /// <param name="value">value of presentable field</param>
        public PresentableFieldForNullableByte(IPresentableObject parentPresentableObject, string key, byte? value)
            : base(parentPresentableObject, key, value) {
            // nothing to do
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected override string GetValueAsString() {
            string value;
            if (this.Value.HasValue) {
                value = this.Value.Value.ToString(CultureInfo.InvariantCulture);
            } else {
                value = string.Empty;
            }
            return value;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">new value to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public override bool TrySetValueAsString(string value) {
            bool success;
            if (string.IsNullOrEmpty(value)) {
                this.Value = null;
                success = true;
            } else {
                success = byte.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out byte parsedValue);
                this.Value = parsedValue;
            }
            return success;
        }

    }

}