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

namespace Framework.Model.Units {

    using Persistence;
    using Persistence.Fields;
    using System.Globalization;

    /// <summary>
    /// Combination of length, width and height.
    /// </summary>
    public class Dimensions : PersistentObject {

        /// <summary>
        /// Height of dimensions.
        /// </summary>
        public decimal? Height {
            get { return this.height.Value; }
            set { this.height.Value = value; }
        }
        private readonly PersistentFieldForNullableDecimal height =
            new PersistentFieldForNullableDecimal(nameof(Height));

        /// <summary>
        /// Height of dimensions as string.
        /// </summary>
        public string HeightAsString {
            get {
                string s;
                if (this.Height.HasValue) {
                    s = this.Height.Value.ToString(CultureInfo.InvariantCulture);
                } else {
                    s = string.Empty;
                }
                return s;
            }
        }

        /// <summary>
        /// Length of dimensions.
        /// </summary>
        public decimal? Length {
            get { return this.length.Value; }
            set { this.length.Value = value; }
        }
        private readonly PersistentFieldForNullableDecimal length =
            new PersistentFieldForNullableDecimal(nameof(Length));

        /// <summary>
        /// Length of dimensions as string.
        /// </summary>
        public string LengthAsString {
            get {
                string s;
                if (this.Length.HasValue) {
                    s = this.Length.Value.ToString(CultureInfo.InvariantCulture);
                } else {
                    s = string.Empty;
                }
                return s;
            }
        }

        /// <summary>
        /// Width of dimensions.
        /// </summary>
        public decimal? Width {
            get { return this.width.Value; }
            set { this.width.Value = value; }
        }
        private readonly PersistentFieldForNullableDecimal width =
            new PersistentFieldForNullableDecimal(nameof(Width));

        /// <summary>
        /// Width of dimensions as string.
        /// </summary>
        public string WidthAsString {
            get {
                string s;
                if (this.Width.HasValue) {
                    s = this.Width.Value.ToString(CultureInfo.InvariantCulture);
                } else {
                    s = string.Empty;
                }
                return s;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public Dimensions()
            : base() {
            this.RegisterPersistentField(this.height);
            this.RegisterPersistentField(this.length);
            this.RegisterPersistentField(this.width);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="length">length of dimensions</param>
        /// <param name="width">width of dimensions</param>
        /// <param name="height">height of dimensions</param>
        public Dimensions(decimal? length, decimal? width, decimal? height)
            : this() {
            this.Length = length;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Converts the string representation of dimensions into the
        /// value of this object. A return value indicates whether
        /// the conversion succeeded.
        /// </summary>
        /// <param name="length">length of dimensions</param>
        /// <param name="width">width of dimensions</param>
        /// <param name="height">height of dimensions</param>
        /// <returns>true if value could be set successfully, false
        /// otherwise</returns>
        public bool TrySetValueAsString(string length, string width, string height) {
            return this.length.TrySetValueAsString(length)
                && this.width.TrySetValueAsString(width)
                && this.height.TrySetValueAsString(height);
        }

    }

}