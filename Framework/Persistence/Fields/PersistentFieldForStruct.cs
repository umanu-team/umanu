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

namespace Framework.Persistence.Fields {

    using System;

    /// <summary>
    /// Wrapper class for a field of any non-nullable element type to
    /// be stored in persistence mechanism.
    /// </summary>
    /// <typeparam name="TValue">type of value</typeparam>
    public abstract class PersistentFieldForStruct<TValue> : PersistentFieldForElement<TValue> where TValue : struct, IEquatable<TValue> {

        /// <summary>
        /// Value loaded from DbDataReader.
        /// </summary>
        protected TValue? valueFromDbDataReader;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">name of persistent field</param>
        public PersistentFieldForStruct(string key)
            : base(key) {
            // nothing to do
        }

        /// <summary>
        /// Sets the new value for persistent field which has been
        /// loaded from DbDataReader before.
        /// </summary>
        internal sealed override void SetValueFromDbDataReader() {
            this.SetValueUnsafe(this.valueFromDbDataReader.Value);
            this.valueFromDbDataReader = null;
            return;
        }

        /// <summary>
        /// Determines whether two value are different.
        /// </summary>
        /// <param name="x">first value to compare</param>
        /// <param name="y">second value to compare</param>
        /// <returns>true if the specified values are different,
        /// false otherwise</returns>
        protected override bool ValuesAreDifferent(TValue x, TValue y) {
            return !x.Equals(y);
        }

    }

}