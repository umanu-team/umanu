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

    /// <summary>
    /// Represents a template item with a list of child template
    /// items to be used to fill in templates.
    /// </summary>
    public sealed class TemplateKeyValuesPair : TemplateItem {

        /// <summary>
        /// Values of template item.
        /// </summary>
        public IEnumerable<TemplateDictionary> Values { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">key of template item</param>
        /// <param name="values">values of template item</param>
        public TemplateKeyValuesPair(string key, IEnumerable<TemplateDictionary> values)
            : base() {
            this.Key = key;
            this.Values = values;
        }

    }

}