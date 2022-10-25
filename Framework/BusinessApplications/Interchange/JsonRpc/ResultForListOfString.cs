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

    using System.Collections.Generic;
    using Framework.Persistence.Fields;

    /// <summary>
    /// JSON-RPC result for list of string values.
    /// </summary>
    internal sealed class ResultForListOfString : Result {

        /// <summary>
        /// Result values.
        /// </summary>
        public PersistentFieldForStringCollection Values { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ResultForListOfString()
            : base() {
            this.Values = new PersistentFieldForStringCollection(nameof(Values));
            this.RegisterPersistentField(this.Values);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="values">result values</param>
        public ResultForListOfString(IEnumerable<string> values)
            : this() {
            this.Values.AddRange(values);
        }

    }

}