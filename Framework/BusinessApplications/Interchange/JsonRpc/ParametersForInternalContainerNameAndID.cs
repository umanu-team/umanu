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

    using System;
    using Framework.Persistence.Fields;

    /// <summary>
    /// JSON-RPC parameters for internal name of container and ID.
    /// </summary>
    internal sealed class ParametersForInternalContainerNameAndID : ParametersForInternalContainerName {

        /// <summary>
        /// ID of persistent object.
        /// </summary>
        public Guid ObjectID {
            get { return this.objectID.Value; }
            set { this.objectID.Value = value; }
        }
        private readonly PersistentFieldForGuid objectID =
            new PersistentFieldForGuid(nameof(ObjectID));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForInternalContainerNameAndID()
            : base() {
            this.RegisterPersistentField(this.objectID);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container</param>
        /// <param name="objectId">ID of persistent object</param>
        public ParametersForInternalContainerNameAndID(string internalContainerName, Guid objectId)
            : this() {
            this.InternalContainerName = internalContainerName;
            this.ObjectID = objectId;
        }

    }

}