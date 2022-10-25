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

    using Framework.Persistence.Fields;

    /// <summary>
    /// Represents a postal address.
    /// </summary>
    public class PostalAddress : GeoCoordinate {

        /// <summary>
        /// City of address.
        /// </summary>
        public string City {
            get { return this.city.Value; }
            set { this.city.Value = value; }
        }
        private readonly PersistentFieldForString city =
            new PersistentFieldForString(nameof(City));

        /// <summary>
        /// Country of address. If this property is set, the country
        /// alpha 2 code will be adjusted automatically.
        /// </summary>
        public Country Country {
            get {
                return Country.GetByAlpha2Code(this.CountryAlpha2Code);
            }
            set {
                this.CountryAlpha2Code = value.Alpha2Code;
            }
        }

        /// <summary>
        /// ISO 3166-1 alpha-2 country code of address. If this
        /// property is set, the country will be adjusted
        /// automatically.
        /// </summary>
        public string CountryAlpha2Code {
            get { return this.countryAlpha2Code.Value; }
            set { this.countryAlpha2Code.Value = value; }
        }
        private readonly PersistentFieldForString countryAlpha2Code =
            new PersistentFieldForString(nameof(CountryAlpha2Code));

        /// <summary>
        /// House number of address.
        /// </summary>
        public string HouseNumber {
            get { return this.houseNumber.Value; }
            set { this.houseNumber.Value = value; }
        }
        private readonly PersistentFieldForString houseNumber =
            new PersistentFieldForString(nameof(HouseNumber));

        /// <summary>
        /// P.O. box of address.
        /// </summary>
        public string PostOfficeBox {
            get { return this.postOfficeBox.Value; }
            set { this.postOfficeBox.Value = value; }
        }
        private readonly PersistentFieldForString postOfficeBox =
            new PersistentFieldForString(nameof(PostOfficeBox));

        /// <summary>
        /// State of address.
        /// </summary>
        public string State {
            get { return this.state.Value; }
            set { this.state.Value = value; }
        }
        private readonly PersistentFieldForString state =
            new PersistentFieldForString(nameof(State));

        /// <summary>
        /// Street of address.
        /// </summary>
        public string Street {
            get { return this.street.Value; }
            set { this.street.Value = value; }
        }
        private readonly PersistentFieldForString street =
            new PersistentFieldForString(nameof(Street));

        /// <summary>
        /// Zip code of address.
        /// </summary>
        public string ZipCode {
            get { return this.zipCode.Value; }
            set { this.zipCode.Value = value; }
        }
        private readonly PersistentFieldForString zipCode =
            new PersistentFieldForString(nameof(ZipCode));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PostalAddress()
            : base() {
            this.RegisterPersistentField(this.city);
            this.RegisterPersistentField(this.countryAlpha2Code);
            this.RegisterPersistentField(this.houseNumber);
            this.RegisterPersistentField(this.postOfficeBox);
            this.RegisterPersistentField(this.state);
            this.RegisterPersistentField(this.street);
            this.RegisterPersistentField(this.zipCode);
        }

    }

}