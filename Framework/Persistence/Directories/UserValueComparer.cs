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

namespace Framework.Persistence.Directories {

    using Framework.Persistence;
    using Framework.Persistence.Exceptions;
    using Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Compares two users by a given field.
    /// </summary>
    public class UserValueComparer : IComparer<IUser> {

        /// <summary>
        /// Key of field to compare users by.
        /// </summary>
        private string fieldKey;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldKey">key of field to compare users by</param>
        public UserValueComparer(string fieldKey) {
            this.fieldKey = fieldKey;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating
        /// whether one is less than, equal to, or greater than the
        /// other.
        /// </summary>
        /// <param name="x">first object to compare</param>
        /// <param name="y">second object to compare</param>
        /// <returns>A signed integer that indicates the relative
        /// order of the comparands: Less than zero if x is less than
        /// y. Equal to zero if x is equal to y. Greater than zero if
        /// x is greater than y.</returns>
        public int Compare(IUser x, IUser y) {
            int result;
            if (null == x) {
                if (null == y) {
                    result = 0;
                } else {
                    result = -1;
                }
            } else if (null == y) {
                result = 1;
            } else {
                if (nameof(IUser.Id) == this.fieldKey) {
                    result = x.Id.CompareTo(y.Id);
                } else if (nameof(IUser.Birthday) == this.fieldKey) {
                    if (x.Birthday.HasValue && y.Birthday.HasValue) {
                        result = x.Birthday.Value.CompareTo(y.Birthday.Value);
                    } else if (!x.Birthday.HasValue) {
                        if (!y.Birthday.HasValue) {
                            result = 0;
                        } else {
                            result = -1;
                        }
                    } else {
                        result = 1;
                    }
                } else if (nameof(IUser.CreatedAt) == this.fieldKey) {
                    result = x.CreatedAt.CompareTo(y.CreatedAt);
                } else if (nameof(IUser.ModifiedAt) == this.fieldKey) {
                    result = x.ModifiedAt.CompareTo(y.ModifiedAt);
                } else {
                    string valueX;
                    string valueY;
                    var xField = x.FindPresentableField(this.fieldKey) as IPresentableFieldForElement;
                    var yField = y.FindPresentableField(this.fieldKey) as IPresentableFieldForElement;
                    if (null != xField && null != yField) {
                        valueX = xField.ValueAsString;
                        valueY = yField.ValueAsString;
                    } else {
                        throw new DirectoryException("Comparing users by field with key \"" + this.fieldKey + "\" is not supported.");
                    }
                    result = valueX.CompareTo(valueY);
                }
            }
            return result;
        }

    }

}