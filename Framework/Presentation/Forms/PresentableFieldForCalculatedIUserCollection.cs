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
    using Framework.Persistence.Directories;
    using System.Collections.Generic;

    /// <summary>
    /// Field for calculated collection of IUsers to be presented.
    /// </summary>
    public sealed class PresentableFieldForCalculatedIUserCollection : PresentableFieldForCalculatedValueCollection<IUser>, IPresentableFieldForIUserCollection {

        /// <summary>
        /// Directory to use for user resolval.
        /// </summary>
        public UserDirectory UserDirectory { get; set; }

        /// <summary>
        /// Sorts the list by a given field.
        /// </summary>
        /// <param name="fieldKey">key of field to order list by</param>
        public void Sort(string fieldKey) {
            var values = this.GetValues() as List<IUser>;
            values.Sort(new UserValueComparer(fieldKey));
            this.SetValues(values);
            return;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="calculateValuesDelegate">delegate for
        /// calculation of values</param>
        public PresentableFieldForCalculatedIUserCollection(IPresentableObject parentPresentableObject, string key, CalculateValuesDelegate<IUser> calculateValuesDelegate)
            : base(parentPresentableObject, key, calculateValuesDelegate) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="calculateValuesDelegate">delegate for
        /// calculation of values</param>
        /// <param name="passThroughValuesDelegate">delegate for
        /// pass-through of values</param>
        public PresentableFieldForCalculatedIUserCollection(IPresentableObject parentPresentableObject, string key, CalculateValuesDelegate<IUser> calculateValuesDelegate, PassThroughValuesDelegate<IUser> passThroughValuesDelegate)
            : base(parentPresentableObject, key, calculateValuesDelegate, passThroughValuesDelegate) {
            // nothing to do
        }

        /// <summary>
        /// Gets the values of presentable field as string.
        /// </summary>
        /// <returns>values of presentable field as string</returns>
        public override IEnumerable<string> GetValuesAsString() {
            foreach (var iUser in this) {
                yield return iUser?.UserName;
            }
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
            bool success = false;
            if (!this.IsReadOnly) {
                if (string.IsNullOrEmpty(item)) {
                    this.Add(null);
                    success = true;
                } else {
                    IUser user = this.UserDirectory.FindOneByVagueTerm(item, FilterScope.UserName);
                    if (null != user) {
                        this.Add(user);
                        success = true;
                    }
                }
            }
            return success;
        }

    }

}