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
    using Persistence.Directories;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field for multiple elements of type IUser to be presented.
    /// </summary>
    public class PresentableFieldForIUserCollection : PresentableFieldForCollection<IUser>, IPresentableFieldForIUserCollection {

        /// <summary>
        /// Base type of value of this field.
        /// </summary>
        public override Type ContentBaseType {
            get {
                return TypeOf.IUser;
            }
        }

        /// <summary>
        /// Directory to use for user resolval.
        /// </summary>
        public UserDirectory UserDirectory { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        public PresentableFieldForIUserCollection(IPresentableObject parentPresentableObject, string key)
            : base(parentPresentableObject, key) {
            // nothing to do
        }

        /// <summary>
        /// Returns an enumerator that iterates through the
        /// collection. 
        /// </summary>
        /// <returns>enumerator that iterates through the collection</returns>
        public override IEnumerable<string> GetValuesAsString() {
            foreach (var iUser in this) {
                yield return iUser?.UserName;
            }
        }

        /// <summary>
        /// Removes a specific object from the collection.
        /// </summary>
        /// <param name="item">specific object to remove from
        /// collection</param>
        /// <returns>true if object was successfully removed from
        /// collection, false otherwise or if object was not
        /// contained in collection</returns>
        public sealed override bool Remove(IUser item) {
            bool success = false;
            if (null != item) {
                for (var index = this.Count - 1; index > -1; index--) {
                    var member = this[index];
                    if (member?.Id == item.Id) {
                        this.Items.RemoveAt(index);
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Sorts the list by a given field.
        /// </summary>
        /// <param name="fieldKey">key of field to order list by</param>
        public void Sort(string fieldKey) {
            this.Sort(new UserValueComparer(fieldKey));
            return;
        }

        /// <summary>
        /// Converts the string representation of a value to the type
        /// of values of this field and adds it. A return value
        /// indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="item">new value to add</param>
        /// <returns>true if value could be added successfully, false
        /// otherwise</returns>
        public override bool TryAddString(string item) {
            IUser user = this.UserDirectory.FindOneByVagueTerm(item, FilterScope.UserName);
            this.Add(user);
            return string.IsNullOrEmpty(item) || null != user;
        }

    }

}