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
    /// Provides traceable data for property changing events.
    /// </summary>
    internal sealed class TraceablePropertyChangingEventArgs : PropertyChangingEventArgs {

        /// <summary>
        /// List of objects that were involved in this event already.
        /// </summary>
        public List<object> SenderTrace { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="propertyName">the name of the property whose
        /// value is changing</param>
        /// <param name="senderTrace">list of objects that were
        /// involved in this event already</param>
        public TraceablePropertyChangingEventArgs(string propertyName, List<object> senderTrace)
            : base(propertyName) {
            this.SenderTrace = senderTrace;
        }

    }

}
