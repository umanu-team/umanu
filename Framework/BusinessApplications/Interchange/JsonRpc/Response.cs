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

    /// <summary>
    /// Base class for any JSON-RPC response.
    /// </summary>
    internal sealed class Response : Message {

        /// <summary>
        /// Result of method call.
        /// </summary>
        public Result Result {
            get { return this.result.Value; }
            set { this.result.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<Result> result =
            new PersistentFieldForPersistentObject<Result>(ResultField, CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Name of persistent field "Result".
        /// </summary>
        public const string ResultField = "result";

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Response()
            : base() {
            this.RegisterPersistentField(this.result);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="request">request this response is for</param>
        public Response(Request request)
            : this() {
            this.Id = request.Id;
        }

    }

}