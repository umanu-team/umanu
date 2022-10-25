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
    using Persistence;
    using Persistence.Filters;
    using System;

    /// <summary>
    /// Field of persistent object to be presented.
    /// </summary>
    /// <typeparam name="T">type of value</typeparam>
    public class PresentableFieldForPersistentObject<T> : PresentableFieldForElement<T>, IPresentableFieldWithOptionDataProvider where T : PersistentObject, new() {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.PersistentObject;
            }
        }

        /// <summary>
        /// Option data provider to use for ID resolval.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of presentable field</param>
        public PresentableFieldForPersistentObject(IPresentableObject parentPresentableObject, string key)
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
        public PresentableFieldForPersistentObject(IPresentableObject parentPresentableObject, string key, T value)
            : base(parentPresentableObject, key) {
            this.Value = value;
        }

        /// <summary>
        /// Converts the value of this field to a string value.
        /// </summary>
        /// <returns>value of this field as string</returns>
        protected sealed override string GetValueAsString() {
            string value;
            if (null == this.Value) {
                value = string.Empty;
            } else {
                value = this.Value.ToString();
                if (this.Value.Type.ToString() == value) {
                    value = this.Value.Id.ToString("N");
                }
            }
            return value;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of value of this field and sets it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">ID of persistent object to be set</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public sealed override bool TrySetValueAsString(string value) {
            bool success;
            if (string.IsNullOrEmpty(value)) {
                this.Value = null;
                success = true;
            } else if (Guid.TryParse(value, out Guid id)) {
                if (null == this.OptionDataProvider) {
                    throw new PresentationException("ID of persistent object cannot be resolved because property \"" + nameof(this.OptionDataProvider) + "\" is not set.  In case you are working with view fields this might be because an incompatible type of view field is used (e.g. " + nameof(ViewFieldForStringChoice) + " instead of " + nameof(ViewFieldForPresentableObjectChoice) + ").");
                }
                var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, id);
                var items = this.OptionDataProvider.Find<T>(filterCriteria, SortCriterionCollection.Empty);
                if (1 == items.Count) {
                    this.Value = items[0];
                    success = true;
                } else {
                    success = false;
                }
            } else {
                success = false;
            }
            return success;
        }

    }

}