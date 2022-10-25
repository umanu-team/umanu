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
    using System;

    /// <summary>
    /// Represents a geo coordinate.
    /// </summary>
    public class GeoCoordinate : PersistentObject {

        /// <summary>
        /// Latitudes north of the equator shall be specified as a
        /// positive value less than or equal to 90. Latitudes south
        /// of the equator shall be specified as a negative value.
        /// </summary>
        public decimal? Latitude {
            get {
                return this.latitude.Value;
            }
            set {
                if (value.HasValue && (value < -90m || value > 90m)) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Latitude " + value + " is invalid. Latitudes must be >= -90 and <= 90.");
                } else {
                    this.latitude.Value = value;
                }
            }
        }
        private readonly PersistentFieldForNullableDecimal latitude =
            new PersistentFieldForNullableDecimal(nameof(Latitude));

        /// <summary>
        /// Longitudes east of the prime meridian shall be specified
        /// as a positive value less than or equal to 180. Longitudes
        /// west of the meridian shall be specified as a negative
        /// value.
        /// </summary>
        public decimal? Longitude {
            get {
                return this.longitude.Value;
            }
            set {
                if (value.HasValue && (value < -180m || value > 180m)) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Longitude " + value + " is invalid. Longitudes must be >= -180 and <= 180.");
                } else {
                    this.longitude.Value = value;
                }
            }
        }
        private readonly PersistentFieldForNullableDecimal longitude =
            new PersistentFieldForNullableDecimal(nameof(Longitude));

        /// <summary>
        /// Square factor for conversion from radians to degrees.
        /// </summary>
        private const double squareFactor = Math.PI / 180.0;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public GeoCoordinate()
            : base() {
            this.RegisterPersistentField(this.latitude);
            this.RegisterPersistentField(this.longitude);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="latitude">latitude of geo coordinate</param>
        /// <param name="longitude">longitude of geo coordinate</param>
        public GeoCoordinate(decimal latitude, decimal longitude)
            : this() {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Gets the approximate distance to another geo coordinate
        /// in kilometers. The calculation is just aproximate because
        /// it is not based on the exeact geoid of the earth. Instead
        /// it assumes the earth to be a sphere.
        /// </summary>
        /// <param name="geoCoordinate">geo coordinate to get
        /// approximate distance to</param>
        /// <returns>approximate distance to other geo coordinate in
        /// kilometers or null if at least one required value for the
        /// calculation is null</returns>
        public double? GetDistanceInKilometersTo(GeoCoordinate geoCoordinate) {
            double? distanceInKilometers;
            if (this.Latitude.HasValue && this.Longitude.HasValue && null != geoCoordinate && geoCoordinate.Latitude.HasValue && geoCoordinate.Longitude.HasValue) {
                const double earthRadius = 6378.137; // GRS 80
                var latitude1 = this.GetLatitudeInDegrees();
                var longitude1 = this.GetLongitudeInDegrees();
                var latitude2 = geoCoordinate.GetLatitudeInDegrees();
                var longitude2 = geoCoordinate.GetLongitudeInDegrees();
                distanceInKilometers = earthRadius * Math.Acos(Math.Sin(latitude1) * Math.Sin(latitude2) + Math.Cos(latitude1) * Math.Cos(latitude2) * Math.Cos(longitude2 - longitude1));
            } else {
                distanceInKilometers = null;
            }
            return distanceInKilometers;
        }

        /// <summary>
        /// Gets the latitude of geo coordinate in degrees.
        /// </summary>
        private double GetLatitudeInDegrees() {
            return Convert.ToDouble(this.Latitude.Value) * GeoCoordinate.squareFactor;
        }

        /// <summary>
        /// Get the longitude of geo coordinate in degrees.
        /// </summary>
        private double GetLongitudeInDegrees() {
            return Convert.ToDouble(this.Longitude.Value) * GeoCoordinate.squareFactor;
        }

        /// <summary>
        /// Gets a value indicating whether another geo coordinate is
        /// within a specified radius.
        /// </summary>
        /// <param name="geoCoordinate">geo coordinate to check
        /// existence within specified radius for</param>
        /// <param name="radiusInKilometers">radius in kilometers to
        /// check existence of other geo coordinate in</param>
        /// <returns>true if geo coordinate is within radius, false
        /// if not or null if at least one required value for the
        /// calculation is null</returns>
        public bool? IsWithinRadius(GeoCoordinate geoCoordinate, double radiusInKilometers) {
            bool? isWithinRadius;
            var distanceInKilometers = this.GetDistanceInKilometersTo(geoCoordinate);
            if (distanceInKilometers.HasValue) {
                isWithinRadius = distanceInKilometers.Value <= radiusInKilometers;
            } else {
                isWithinRadius = null;
            }
            return isWithinRadius;
        }

    }

}