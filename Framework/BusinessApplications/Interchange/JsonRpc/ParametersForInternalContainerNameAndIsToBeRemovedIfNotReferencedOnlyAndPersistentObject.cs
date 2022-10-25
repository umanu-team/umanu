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

namespace Framework.BusinessApplications.Interchange.JsonRpc {

    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// JSON-RPC parameters for internal name of container and
    /// bool and persistent object.
    /// </summary>
    internal sealed class ParametersForInternalContainerNameAndIsToBeRemovedIfNotReferencedOnlyAndPersistentObject : ParametersForInternalContainerNameAndPersistentObject {

        /// <summary>
        /// True if object is to be removed if not referenced by
        /// other objects only, false otherwise.
        /// </summary>
        public bool IsToBeRemovedIfNotReferencedOnly {
            get { return this.isToBeRemovedIfNotReferencedOnly.Value; }
            set { this.isToBeRemovedIfNotReferencedOnly.Value = value; }
        }
        private readonly PersistentFieldForBool isToBeRemovedIfNotReferencedOnly =
            new PersistentFieldForBool(nameof(IsToBeRemovedIfNotReferencedOnly));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForInternalContainerNameAndIsToBeRemovedIfNotReferencedOnlyAndPersistentObject()
            : base() {
            this.RegisterPersistentField(this.isToBeRemovedIfNotReferencedOnly);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        /// <param name="persistentObject">persistent object</param>
        public ParametersForInternalContainerNameAndIsToBeRemovedIfNotReferencedOnlyAndPersistentObject(string internalContainerName, bool isToBeRemovedIfNotReferencedOnly, PersistentObject persistentObject)
            : this() {
            this.InternalContainerName = internalContainerName;
            this.IsToBeRemovedIfNotReferencedOnly = isToBeRemovedIfNotReferencedOnly;
            this.PersistentObject = persistentObject;
        }

    }

}