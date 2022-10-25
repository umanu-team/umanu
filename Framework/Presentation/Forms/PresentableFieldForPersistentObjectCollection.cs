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
    using System.Collections.Generic;

    /// <summary>
    /// Field of multiple persistent objects to be presented.
    /// </summary>
    /// <typeparam name="T">type of values</typeparam>
    public class PresentableFieldForPersistentObjectCollection<T> : PresentableFieldForCollection<T>, IPresentableFieldWithOptionDataProvider where T : PersistentObject, new() {

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
        /// <param name="key">internal key of this presentable field</param>
        public PresentableFieldForPersistentObjectCollection(IPresentableObject parentPresentableObject, string key)
            : base(parentPresentableObject, key) {
            // nothing to do
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public sealed override IEnumerable<string> GetValuesAsString() {
            foreach (var persistentObject in this) {
                yield return persistentObject?.Id.ToString("N");
            }
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of values of this field and adds it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="item">ID of persistent object to add</param>
        /// <returns>true if value could be added successfully, false
        /// otherwise</returns>
        public sealed override bool TryAddString(string item) {
            bool success;
            if (string.IsNullOrEmpty(item)) {
                success = true;
            } else if (Guid.TryParse(item, out Guid id)) {
                if (null == this.OptionDataProvider) {
                    throw new PresentationException("ID of persistent object cannot be resolved because property \"" + nameof(this.OptionDataProvider) + "\" is not set.  In case you are working with view fields this might be because an incompatible type of view field is used (e.g. " + nameof(ViewFieldForMultipleStringChoices) + " instead of " + nameof(ViewFieldForMultiplePresentableObjectChoices) + ").");
                }
                var filterCriteria = new FilterCriteria(nameof(PersistentObject.Id), RelationalOperator.IsEqualTo, id);
                var items = this.OptionDataProvider.Find<T>(filterCriteria, SortCriterionCollection.Empty);
                if (1 == items.Count) {
                    this.Add(items[0]);
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