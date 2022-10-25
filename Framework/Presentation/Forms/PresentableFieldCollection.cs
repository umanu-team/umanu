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

    using Framework.Presentation.Exceptions;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Keyed collection of presentable fields, optimized for fast
    /// access by key. Keys of fields may not change.
    /// </summary>
    public class PresentableFieldCollection : KeyedCollection<string, IPresentableField>, IPresentableFieldCollection {

        /// <summary>
        /// Gets the keys of all presentable fields of this
        /// collection.
        /// </summary>
        public IEnumerable<string> Keys {
            get {
                foreach (var presentableField in this) {
                    yield return presentableField.Key;
                }
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PresentableFieldCollection()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableFields">enumerable of presentable
        /// fields to add initially</param>
        public PresentableFieldCollection(IEnumerable<IPresentableField> presentableFields)
            : base() {
            this.AddRange(presentableFields);
        }

        /// <summary>
        /// Adds a range of presentable fields to this collection.
        /// </summary>
        /// <param name="items">range of fields to add</param>
        public void AddRange(IEnumerable<IPresentableField> items) {
            foreach (IPresentableField item in items) {
                this.Add(item);
            }
            return;
        }

        /// <summary>
        /// Determines whether the collection contains an element
        /// with the specified key.
        /// </summary>
        /// <param name="key">key to look for</param>
        /// <returns>true if an element for the given key exists,
        /// false otherwise</returns>
        public new bool Contains(string key) {
            return this.Contains(KeyChain.FromKey(key));
        }

        /// <summary>
        /// Determines whether the collection contains an element
        /// with the specified key chain.
        /// </summary>
        /// <param name="keyChain">key chain to look for</param>
        /// <returns>true if an element for the given key chain
        /// exists, false otherwise</returns>
        public bool Contains(string[] keyChain) {
            bool isContained;
            if (keyChain.LongLength < 2L) {
                string key;
                if (keyChain.LongLength < 1L) {
                    key = string.Empty;
                } else { // 1L == keyChain.LongLength
                    key = keyChain[0];
                }
                isContained = base.Contains(key);
            } else {
                isContained = null != this.Find(keyChain);
            }
            return isContained;
        }

        /// <summary>
        /// Finds a specific presentable field in this or any
        /// presentable child object.
        /// </summary>
        /// <param name="keyChain">key chain of presentable field to
        /// find</param>
        /// <returns>matching presentable field or null</returns>
        public IPresentableField Find(string[] keyChain) {
            IPresentableField resultField = null;
            if (keyChain.LongLength > 0L) {
                string key = keyChain[0];
                if (base.Contains(key)) {
                    if (keyChain.LongLength > 1L) {
                        IPresentableField fieldForKey = base[key];
                        if (fieldForKey.IsForSingleElement) {
                            var fieldForElement = fieldForKey as IPresentableFieldForElement;
                            var childObject = fieldForElement.ValueAsObject as IPresentableObject;
                            if (null != childObject) {
                                resultField = childObject.FindPresentableField(KeyChain.RemoveFirstLinkOf(keyChain));
                            }
                        } else { // !fieldForKey.IsForSingleElement
                            throw new PresentationException("Presentable field " + KeyChain.ToKey(keyChain) + " for collection cannot be resolved.");
                        }
                    } else {
                        resultField = base[key];
                    }
                }
            }
            return resultField;
        }

        /// <summary>
        /// Extracts the key of the specified persistent field.
        /// </summary>
        /// <param name="item">persistent field to extract key of</param>
        /// <returns>key of the specified persistent field</returns>
        protected override string GetKeyForItem(IPresentableField item) {
            return item.Key;
        }

    }

}