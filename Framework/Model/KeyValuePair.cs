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

namespace Framework.Model {

    using Framework.Persistence;
    using Framework.Persistence.Fields;

    /// <summary>
    /// Persistent pair of key and value, each of type string.
    /// </summary>
    public class KeyValuePair : PersistentObject {

        /// <summary>
        /// Key of value.
        /// </summary>
        public string KeyField {
            get { return this.keyField.Value; }
            set { this.keyField.Value = value; }
        }
        private readonly PersistentFieldForString keyField =
            new PersistentFieldForString(nameof(KeyField));

        /// <summary>
        /// Value for key.
        /// </summary>
        public string Value {
            get { return this.value.Value; }
            set { this.value.Value = value; }
        }
        private readonly PersistentFieldForString value =
            new PersistentFieldForString(nameof(Value));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public KeyValuePair()
            : base() {
            this.RegisterPersistentField(this.keyField);
            this.RegisterPersistentField(this.value);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">key of value</param>
        /// <param name="value">value for key</param>
        public KeyValuePair(string key, string value)
            : this() {
            this.KeyField = key;
            this.Value = value;
        }

    }

}