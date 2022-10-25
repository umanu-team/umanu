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

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the result of a rating activity.
    /// </summary>
    public sealed class RatingResult : IEquatable<RatingResult> {

        /// <summary>
        /// Average value of ratings.
        /// </summary>
        public double Average { get; private set; }

        /// <summary>
        /// Total number of ratings.
        /// </summary>
        public ulong NumberOfRatings { get; private set; }

        /// <summary>
        /// Dictionary of all ratings
        /// </summary>
        /// <value>
        /// Dictionary of ratings.
        /// </value>
        public Dictionary<ulong, ulong> Ratings { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="ratings">ratings to be processed</param>
        public RatingResult(IEnumerable<Rating> ratings)
            : base() {
            ulong sumOfRatings = 0;
            this.NumberOfRatings = 0;
            this.Ratings = new Dictionary<ulong, ulong>();
            this.Ratings[1] = 0;
            this.Ratings[2] = 0;
            this.Ratings[3] = 0;
            this.Ratings[4] = 0;
            this.Ratings[5] = 0;
            foreach (var rating in ratings) {
                sumOfRatings += rating.Score;
                this.NumberOfRatings++;
                this.Ratings[rating.Score]++;
            }
            if (this.NumberOfRatings > 0) {
                this.Average = (double)sumOfRatings / this.NumberOfRatings;
            } else {
                this.Average = 0;
            }
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type.
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal, false otherwise</returns>
        public bool Equals(RatingResult other) {
            return null != other
                && this.Average.Equals(other.Average)
                && this.NumberOfRatings.Equals(other.NumberOfRatings);
        }

    }

}