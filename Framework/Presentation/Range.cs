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

namespace Framework.Presentation {

    using System.Collections.Generic;

    /// <summary>
    /// Represents the indexes of a subset of elements.
    /// </summary>
    public class Range {

        /// <summary>
        /// Index of first position in items to return - "0" is the
        /// lowest index: "0" would return all items, whereas "5"
        /// would skip the five first items.
        /// </summary>
        public int StartPosition { get; private set; }

        /// <summary>
        /// Maximum number of items to return.
        /// </summary>
        public int MaxResults { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="startPosition">index of first position in
        /// items to return - "0" is the lowest index: "0" would
        /// return all items, whereas "5" would skip the five first
        /// items</param>
        /// <param name="maxResults">maximum number of items to
        /// return</param>
        /// <returns>subset of objects of collection</returns>
        public Range(int startPosition, int maxResults)
            : base() {
            if (startPosition < 0) {
                this.StartPosition = 0;
            } else {
                this.StartPosition = startPosition;
            }
            if (maxResults < 0) {
                this.MaxResults = 0;
            } else {
                this.MaxResults = maxResults;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="startPosition">index of first position in
        /// items to return - "0" is the lowest index: "0" would
        /// return all items, whereas "5" would skip the five first
        /// items</param>
        /// <param name="maxResults">maximum number of items to
        /// return</param>
        public Range(ulong startPosition, ulong maxResults)
            : this(Range.ConvertToInt(startPosition), Range.ConvertToInt(maxResults)) {
            // nothing to do
        }

        /// <summary>
        /// Reduces a collection to the subset matching the range.
        /// </summary>
        /// <param name="presentableObjects">collection to be reduced
        /// to subset matching the range</param>
        public void ApplyTo<T>(ICollection<T> presentableObjects) where T : IPresentableObject {
            if (this.StartPosition > 0 || presentableObjects.Count > this.MaxResults) {
                var subset = this.GetSubset(presentableObjects);
                presentableObjects.Clear();
                foreach (var presentableObject in subset) {
                    presentableObjects.Add(presentableObject);
                }
            }
            return;
        }

        /// <summary>
        /// Converts a ulong value into an integer value and limits
        /// it to int.MaxValue.
        /// </summary>
        /// <param name="ulongValue">ulong value to be converted</param>
        /// <returns>ulong value converted to integer</returns>
        private static int ConvertToInt(ulong ulongValue) {
            int intValue;
            if (ulongValue < int.MaxValue) {
                intValue = (int)ulongValue;
            } else {
                intValue = int.MaxValue;
            }
            return intValue;
        }

        /// <summary>
        /// Gets a subset of objects of a collection.
        /// </summary>
        /// <param name="presentableObjects">collection to get subset
        /// of</param>
        /// <returns>subset of objects of collection</returns>
        public ICollection<T> GetOf<T>(ICollection<T> presentableObjects) where T : IPresentableObject {
            ICollection<T> subset;
            if (this.StartPosition > 0 || presentableObjects.Count > this.MaxResults) {
                subset = this.GetSubset(presentableObjects);
            } else {
                subset = presentableObjects;
            }
            return subset;
        }

        /// <summary>
        /// Gets a subset of objects of a collection.
        /// </summary>
        /// <param name="presentableObjects">collection to get subset
        /// of</param>
        private ICollection<T> GetSubset<T>(ICollection<T> presentableObjects) where T : IPresentableObject {
            var batchSize = this.MaxResults;
            var countAfterSkip = presentableObjects.Count - this.StartPosition;
            if (countAfterSkip < 0) {
                countAfterSkip = 0;
            }
            if (batchSize > countAfterSkip) {
                batchSize = countAfterSkip;
            }
            ICollection<T> subset = new List<T>(batchSize);
            if (batchSize > 0) {
                var skip = this.StartPosition;
                foreach (var presentableObject in presentableObjects) {
                    if (skip > 0) {
                        skip--;
                    } else if (subset.Count < batchSize) {
                        subset.Add(presentableObject);
                    } else {
                        break;
                    }
                }
            }
            return subset;
        }

    }

}