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

    /// <summary>
    /// Field for calculated IUser to be presented.
    /// </summary>
    public sealed class PresentableFieldForCalculatedIUser : PresentableFieldForCalculatedValue<IUser>, IPresentableFieldForIUser {

        /// <summary>
        /// Directory to use for user resolval.
        /// </summary>
        public UserDirectory UserDirectory { get; set; }

        /// <summary>
        /// Value of this presentable field as string.
        /// </summary>
        public override string ValueAsString {
            get {
                return this.Value?.UserName;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="calculateValueDelegate">delegate for
        /// calculation of value</param>
        public PresentableFieldForCalculatedIUser(IPresentableObject parentPresentableObject, string key, CalculateValueDelegate<IUser> calculateValueDelegate)
            : base(parentPresentableObject, key, calculateValueDelegate) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of this field</param>
        /// <param name="key">internal key of this presentable field</param>
        /// <param name="calculateValueDelegate">delegate for
        /// calculation of value</param>
        /// <param name="passThroughValueDelegate">delegate for
        /// pass-through of value</param>
        public PresentableFieldForCalculatedIUser(IPresentableObject parentPresentableObject, string key, CalculateValueDelegate<IUser> calculateValueDelegate, PassThroughValueDelegate<IUser> passThroughValueDelegate)
            : base(parentPresentableObject, key, calculateValueDelegate, passThroughValueDelegate) {
            // nothing to do.
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
            bool success = false;
            if (!this.IsReadOnly) {
                if (string.IsNullOrEmpty(value)) {
                    this.Value = null;
                    success = true;
                } else {
                    var user = this.UserDirectory.FindOneByVagueTerm(value, FilterScope.UserName);
                    if (null != user) {
                        this.Value = user;
                        success = true;
                    }
                }
            }
            return success;
        }

    }

}