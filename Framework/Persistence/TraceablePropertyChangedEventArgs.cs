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

    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Provides traceable data for property changed events.
    /// </summary>
    internal sealed class TraceablePropertyChangedEventArgs : PropertyChangedEventArgs {

        /// <summary>
        /// Key chain of the property whose value changed.
        /// </summary>
        public string[] KeyChain { get; private set; }

        /// <summary>
        /// List of objects that were involved in this event already.
        /// </summary>
        public IList<object> SenderTrace { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="keyChain">key chain of the property whose
        /// value changed</param>
        /// <param name="senderTrace">list of objects that were
        /// involved in this event already</param>
        public TraceablePropertyChangedEventArgs(string[] keyChain, IList<object> senderTrace)
            : base(Model.KeyChain.ToKey(keyChain)) {
            this.KeyChain = keyChain;
            this.SenderTrace = senderTrace;
        }

    }

}