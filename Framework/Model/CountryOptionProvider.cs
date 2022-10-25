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

    using Presentation;
    using Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Provider class of country options to be pulled in fields of
    /// views.
    /// </summary>
    public sealed class CountryOptionProvider : OptionProvider {

        /// <summary>
        /// Instantiates an new instance.
        /// </summary>
        public CountryOptionProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets all options of this option provider (IDs of
        /// objects/users need to be set as keys).
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option provider (IDs of
        /// objects/users need to be set as keys)</returns>
        public override IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            foreach (var country in Country.Countries) {
                yield return new KeyValuePair<string, string>(country.Alpha2Code, country.Name);
            }
        }

    }

}