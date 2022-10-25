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

    using Presentation.Forms;
    using System.Globalization;

    /// <summary>
    /// Provider class of culture options to be pulled in fields of
    /// views.
    /// </summary>
    public sealed class CultureOptionProvider : StringOptionDictionary {

        /// <summary>
        /// Instantiates an new instance.
        /// </summary>
        public CultureOptionProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates an new instance.
        /// </summary>
        /// <param name="cultureTypes">bitwise combinable filter for
        /// cultures to be provided</param>
        public CultureOptionProvider(CultureTypes cultureTypes)
            : this() {
            var cultures = CultureInfo.GetCultures(cultureTypes);
            foreach (var culture in cultures) {
                if (!string.IsNullOrEmpty(culture.Name)) {
                    this.Add(culture.Name, culture.DisplayName);
                }
            }
            this.SortByValue();
        }

    }

}