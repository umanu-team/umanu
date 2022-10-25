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
    /// Represents a rating.
    /// </summary>
    public sealed class Rating : PersistentObject {

        /// <summary>
        /// Score of rating.
        /// </summary>
        public byte Score {
            get { return this.score.Value; }
            set { this.score.Value = value; }
        }
        private readonly PersistentFieldForByte score =
            new PersistentFieldForByte(nameof(Score));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Rating()
            : base() {
            this.RegisterPersistentField(this.score);
        }

    }

}