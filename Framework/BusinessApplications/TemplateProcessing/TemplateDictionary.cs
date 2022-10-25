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

namespace Framework.BusinessApplications.TemplateProcessing {

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;

    /// <summary>
    /// Keyed collection for template items, optimized for fast
    /// access by key.
    /// </summary>
    public sealed class TemplateDictionary : KeyedCollection<string, TemplateItem> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public TemplateDictionary()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Adds a template value to the end of the collection.
        /// </summary>
        /// <param name="key">key of template item</param>
        /// <param name="value">value of template item</param>
        public void Add(string key, bool value) {
            this.Add(new TemplateKeyValuePair(key, value.ToString(CultureInfo.InvariantCulture)));
            return;
        }

        /// <summary>
        /// Adds a template value to the end of the collection.
        /// </summary>
        /// <param name="key">key of template item</param>
        /// <param name="value">value of template item</param>
        public void Add(string key, string value) {
            this.Add(new TemplateKeyValuePair(key, value));
            return;
        }

        /// <summary>
        /// Adds a template list to the end of the collection.
        /// </summary>
        /// <param name="key">key of template item</param>
        /// <param name="values">values of template item</param>
        public void Add(string key, IEnumerable<TemplateDictionary> values) {
            this.Add(new TemplateKeyValuesPair(key, values));
            return;
        }

        /// <summary>
        /// Extracts the key of the template item.
        /// </summary>
        /// <param name="templateItem">template item to extract key
        /// of</param>
        /// <returns>key of the specified template item</returns>
        protected override string GetKeyForItem(TemplateItem templateItem) {
            return templateItem.Key;
        }

        /// <summary>
        /// Copies this template dictionry and merges it with another
        /// template dictionary which overrides the values of this
        /// template dictionary.
        /// </summary>
        /// <param name="templateDictionary">template dictionary
        /// overriding the values of copy of this template dictionary</param>
        /// <returns>copy of template dictionary merged with other
        /// template dictionary</returns>
        internal TemplateDictionary MergeWith(TemplateDictionary templateDictionary) {
            TemplateDictionary mergedTemplateDictionary;
            if (null == templateDictionary) {
                mergedTemplateDictionary = this;
            } else {
                mergedTemplateDictionary = new TemplateDictionary();
                foreach (var templateItem in templateDictionary) {
                    mergedTemplateDictionary.Add(templateItem);
                }
                foreach (var templateItem in this) {
                    if (!templateDictionary.Contains(templateItem.Key)) {
                        mergedTemplateDictionary.Add(templateItem);
                    }
                }
            }
            return mergedTemplateDictionary;
        }

    }

}