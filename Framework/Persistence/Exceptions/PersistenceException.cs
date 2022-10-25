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

namespace Framework.Persistence.Exceptions {

    using System;

    /// <summary>
    /// Exception class for errors in persistence layer.
    /// </summary>
    [Serializable]
    public class PersistenceException : ApplicationException {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PersistenceException()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance with a specified error message.
        /// </summary>
        /// <param name="message">specified error message</param>
        public PersistenceException(string message)
            : base(message) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance with a specified error message.
        /// </summary>
        /// <param name="message">specified error message</param>
        /// <param name="innerException">the instance of exception
        /// that caused the current exception</param>
        public PersistenceException(string message, Exception innerException)
            : base(message, innerException) {
            // nothing to do
        }

    }

}