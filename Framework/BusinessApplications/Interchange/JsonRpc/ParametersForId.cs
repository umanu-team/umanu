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

    using Framework.Persistence.Fields;
    using System;

    /// <summary>
    /// JSON-RPC parameters for ID.
    /// </summary>
    internal sealed class ParametersForId : Parameter {

        /// <summary>
        /// ID of persistent object.
        /// </summary>
        public Guid ObjectId {
            get { return this.objectId.Value; }
            set { this.objectId.Value = value; }
        }
        private readonly PersistentFieldForGuid objectId =
            new PersistentFieldForGuid(nameof(ObjectId));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ParametersForId()
            : base() {
            this.RegisterPersistentField(this.objectId);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="objectId">ID of persistent object</param>
        public ParametersForId(Guid objectId)
            : this() {
            this.ObjectId = objectId;
        }

    }

}