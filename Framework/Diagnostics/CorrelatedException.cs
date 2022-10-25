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

namespace Framework.Diagnostics {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception with correlation ID.
    /// </summary>
    [Serializable]
    public class CorrelatedException : Exception {

        /// <summary>
        /// Correlation ID.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public CorrelatedException()
            : base() {
            this.CorrelationId = Guid.NewGuid();
        }

        /// <summary>
        /// Instantiates a new instance with a specified error message.
        /// </summary>
        /// <param name="message">specified error message</param>
        public CorrelatedException(string message)
            : base(message) {
            this.CorrelationId = Guid.NewGuid();
        }

        /// <summary>
        /// Instantiates a new instance with a specified error message.
        /// </summary>
        /// <param name="message">specified error message</param>
        /// <param name="innerException">the instance of exception
        /// that caused the current exception</param>
        public CorrelatedException(string message, Exception innerException)
            : base(message, innerException) {
            this.CorrelationId = Guid.NewGuid();
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="info">SerializationInfo populated with data</param>
        /// <param name="context">source for deserialization</param>
        protected CorrelatedException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
                this.CorrelationId = (Guid)info.GetValue("CorrelationId", typeof(Guid));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo to populate with
        /// data</param>
        /// <param name="context">destination for serialization</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("CorrelationId", this.CorrelationId);
            return;
        }

    }

}