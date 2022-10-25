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

namespace Framework.Presentation.Buttons {

    using Framework.BusinessApplications;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Action button of action bar.
    /// </summary>
    public abstract class ActionButton : IEquatable<ActionButton> {

        /// <summary>
        /// Allowed groups for reading/writing this object.
        /// </summary>
        public PersistentFieldForGroupCollection AllowedGroupsForReading { get; private set; }

        /// <summary>
        /// Salt to be used for generation of hash. This needs to be
        /// set if you like to use 100% identical buttons.
        /// </summary>
        public string HashSalt { get; set; }

        /// <summary>
        /// Display caption of button.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        public ActionButton(string title) {
            this.AllowedGroupsForReading = new PersistentFieldForGroupCollection(string.Empty, CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.Title = title;
        }

        /// <summary>
        /// Gets the hash of this action button dependent on type,
        /// title, icon url and number and title of allowed groups
        /// for reading.
        /// </summary>
        /// <returns>hash of this action button</returns>
        public override int GetHashCode() {
            int hashCode;
            try {
                string s = this.GetType().FullName;
                s += this.AllowedGroupsForReading.Count;
                foreach (var allowedGroupForReading in this.AllowedGroupsForReading) {
                    s += allowedGroupForReading.Title;
                }
                if (!string.IsNullOrEmpty(this.HashSalt)) {
                    s += this.HashSalt;
                }
                if (!string.IsNullOrEmpty(this.Title)) {
                    s += this.Title;
                }
                hashCode = s.GetHashCode();
            } catch (ObjectNotFoundException) {
                hashCode = -1; // work around to avoid exceptions in permission resolval of other buttons after delete button has been pressed
            }
            return hashCode;
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type by hash code (type, title and
        /// icon url).
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal, false otherwise</returns>
        public bool Equals(ActionButton other) {
            bool isEqual;
            if (null == other) {
                isEqual = false;
            } else {
                isEqual = this.GetHashCode().Equals(other.GetHashCode());
            }
            return isEqual;
        }

        /// <summary>
        /// Gets the child controllers for action button.
        /// </summary>
        /// <param name="element">object to get child controllers for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page</param>
        /// <returns>child controllers for form</returns>
        public virtual IEnumerable<IHttpController> GetChildControllers(IProvidableObject element, IBusinessApplication businessApplication, string absoluteUrl) {
            yield break;
        }

    }

}