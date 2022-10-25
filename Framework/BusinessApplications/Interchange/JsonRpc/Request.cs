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
    internal sealed class Request : Message {

        /// <summary>
        /// Name of method.
        /// </summary>
        public string Method {
            get { return this.method.Value; }
            private set { this.method.Value = value; }
        }
        private readonly PersistentFieldForString method =
            new PersistentFieldForString(MethodField);

        /// <summary>
        /// Name of persistent field "Method".
        /// </summary>
        public const string MethodField = "method";

        /// <summary>
        /// Parameters of method.
        /// </summary>
        public Parameter Parameters {
            get { return this.parameters.Value; }
            set { this.parameters.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<Parameter> parameters =
            new PersistentFieldForPersistentObject<Parameter>(ParametersField, CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Name of persistent field "Parameters".
        /// </summary>
        public const string ParametersField = "params";

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Request()
            : base() {
            this.RegisterPersistentField(this.method);
            this.RegisterPersistentField(this.parameters);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Request(string method)
            : this() {
            this.Method = method;
        }

    }

}