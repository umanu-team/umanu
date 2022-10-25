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

    using System;

    /// <summary>
    /// Attribute for previous assembly full or assembly qualified
    /// names of a class for automatic migrations of persistence
    /// mechanisms.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class PreviousNameAttribute : Attribute {

        /// <summary>
        /// Previous assembly full or assembly qualified name of
        /// class for automatic migrations of persistence mechanisms.
        /// </summary>
        public string PreviousName { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="previousName">previous assembly full or
        /// assembly qualified name of class for automatic
        /// migrations of persistence mechanisms</param>
        public PreviousNameAttribute(string previousName)
            : base() {
            this.PreviousName = previousName;
        }

    }

}