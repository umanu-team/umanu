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

namespace Framework.Persistence.Text {

    using System.Collections.ObjectModel;

    /// <summary>
    /// Collection for split strings.
    /// </summary>
    public class StringSplitCollection : Collection<string> {

        /// <summary>
        /// Type of split.
        /// </summary>
        public StringSplitType SplitType { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public StringSplitCollection()
            : base() {
            this.SplitType = StringSplitType.None;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="splitType">type of split</param>
        public StringSplitCollection(StringSplitType splitType) {
            this.SplitType = splitType;
        }

    }

}