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
    /// JSON-RPC parameters for bool and persistent object.
    /// </summary>
    internal sealed class ParametersForPersistentObjectAndIsToBeRemovedIfNotReferencedOnly : Parameter {

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
        /// Persistent object.
        /// </summary>
        public PersistentObject PersistentObject {
            get { return this.persistentObject.Value; }
            set { this.persistentObject.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<PersistentObject> persistentObject =
            new PersistentFieldForPersistentObject<PersistentObject>(nameof(PersistentObject), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForPersistentObjectAndIsToBeRemovedIfNotReferencedOnly()
            : base() {
            this.RegisterPersistentField(this.isToBeRemovedIfNotReferencedOnly);
            this.RegisterPersistentField(this.persistentObject);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistentObject">persistent object</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        public ParametersForPersistentObjectAndIsToBeRemovedIfNotReferencedOnly(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly)
            : this() {
            this.IsToBeRemovedIfNotReferencedOnly = isToBeRemovedIfNotReferencedOnly;
            this.PersistentObject = persistentObject;
        }

    }

}