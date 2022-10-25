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

namespace Framework.Presentation.Forms {

    using Framework.Persistence;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Non-thread-safe collection of search facets.
    /// </summary>
    public sealed class SearchFacetCollection : Collection<ViewFieldForElement> {

        /// <summary>
        /// Dictionary of replaced keys as key and actual key chains
        /// as value.
        /// </summary>
        public IDictionary<string, string[]> ReplacedKeysDictionary {
            get {
                if (null == this.replacedKeysDictionary) {
                    this.CreateSearchFacetsWithReplacedKeys();
                }
                return this.replacedKeysDictionary;
            }
        }
        private IDictionary<string, string[]> replacedKeysDictionary;

        /// <summary>
        /// List of search facets with replaced keys for values.
        /// </summary>
        public IEnumerable<ViewFieldForElement> WithReplacedKeys {
            get {
                if (null == this.withReplacedKeys) {
                    this.CreateSearchFacetsWithReplacedKeys();
                }
                return this.withReplacedKeys;
            }
        }
        private IList<ViewFieldForElement> withReplacedKeys;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public SearchFacetCollection()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Removes all search facets.
        /// </summary>
        protected override void ClearItems() {
            base.ClearItems();
            this.ClearSearchFacetsWithReplacedKeys();
            return;
        }

        /// <summary>
        /// Clears the search facets with replaced keys and the
        /// dictionary of replaced keys.
        /// </summary>
        private void ClearSearchFacetsWithReplacedKeys() {
            this.withReplacedKeys = null;
            this.replacedKeysDictionary = null;
            return;
        }

        /// <summary>
        /// Creates the search facets with replaced keys and the
        /// dictionary of replaced keys.
        /// </summary>
        private void CreateSearchFacetsWithReplacedKeys() {
            this.withReplacedKeys = new List<ViewFieldForElement>(this.Count);
            this.replacedKeysDictionary = new Dictionary<string, string[]>(this.Count);
            foreach (var searchFacet in this) {
                var searchFacetWithReplacedKey = Activator.CreateInstance(searchFacet.Type) as ViewFieldForElement;
                searchFacetWithReplacedKey.CopyFrom(searchFacet, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                searchFacetWithReplacedKey.Key = "v-" + searchFacet.Key.Replace('.', '-');
                this.withReplacedKeys.Add(searchFacetWithReplacedKey);
                this.replacedKeysDictionary.Add(searchFacetWithReplacedKey.Key, searchFacet.KeyChain);
            }
            return;
        }

        /// <summary>
        /// Inserts a search facet at the specified index.
        /// </summary>
        /// <param name="index">zero-based index at which search
        /// facet should be inserted</param>
        /// <param name="item">search facet to insert</param>
        protected override void InsertItem(int index, ViewFieldForElement item) {
            base.InsertItem(index, item);
            this.ClearSearchFacetsWithReplacedKeys();
            return;
        }

        /// <summary>
        /// Removes the search facet at the specified index.
        /// </summary>
        /// <param name="index">zero-based index of the search facet
        /// to remove</param>
        protected override void RemoveItem(int index) {
            base.RemoveItem(index);
            this.ClearSearchFacetsWithReplacedKeys();
            return;
        }

        /// <summary>
        /// Replaces the search facet at the specified index.
        /// </summary>
        /// <param name="index">zero-based index of the search facet
        /// to replace</param>
        /// <param name="item">new value for the search facet at the
        /// specified index</param>
        protected override void SetItem(int index, ViewFieldForElement item) {
            base.SetItem(index, item);
            this.ClearSearchFacetsWithReplacedKeys();
            return;
        }

    }

}