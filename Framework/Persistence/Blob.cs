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

namespace Framework.Persistence {

    using Framework.Persistence.Fields;

    /// <summary>
    /// Represents a persistent binary large object.
    /// </summary>
    public class Blob : PersistentObject {

        /// <summary>
        /// Binary data.
        /// </summary>
        public byte[] Bytes {
            get { return this.bytes.Value; }
            set { this.bytes.Value = value; }
        }
        private readonly PersistentFieldForBlob bytes =
            new PersistentFieldForBlob(nameof(Bytes));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Blob()
            : base() {
            this.RegisterPersistentField(this.bytes);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="bytes">binary data</param>
        public Blob(byte[] bytes)
            : this() {
            this.Bytes = bytes;
        }

    }

}