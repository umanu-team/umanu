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
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Framework.Persistence.Fields;

    /// <summary>
    /// JSON-RPC result for object values.
    /// </summary>
    internal sealed class ResultForObject : Result {

        /// <summary>
        /// Object value.
        /// </summary>
        public object Value {
            get {
                object value;
                byte[] buffer = Convert.FromBase64String(this.value.Value);
                using (MemoryStream memoryStream = new MemoryStream(buffer)) {
                    value = new BinaryFormatter().Deserialize(memoryStream);
                }
                return value;
            }
            set {
                using (MemoryStream memoryStream = new MemoryStream()) {
                    new BinaryFormatter().Serialize(memoryStream, value);
                    this.value.Value = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
        private readonly PersistentFieldForString value =
            new PersistentFieldForString(nameof(Value));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ResultForObject()
            : base() {
            this.RegisterPersistentField(this.value);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="value">result value</param>
        public ResultForObject(object value)
            : this() {
            this.Value = value;
        }

    }

}