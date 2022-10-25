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

    using Persistence;
    using Persistence.Directories;
    using Persistence.Filters;
    using System.Collections.Generic;

    /// <summary>
    /// Provider for person lookup values to be pulled in person
    /// fields of views.
    /// </summary>
    public class PersonLookupProvider : StringLookupProvider {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public PersonLookupProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Finds values by a vague search term containing at least
        /// parts of the value.
        /// </summary>
        /// <param name="vagueTerm">search term for value</param>
        /// <param name="presentableObject">presentable object to get
        /// lookup values for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        /// <returns>matching values</returns>
        public override IEnumerable<string> FindValuesByVagueTerm(string vagueTerm, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            foreach (var user in optionDataProvider.UserDirectory.FindByVagueTerm(vagueTerm, FilterScope.UserNameAndDisplayName)) {
                if (null != user) {
                    yield return user.DisplayName + " (" + user.UserName + ")";
                }
            }
        }

        /// <summary>
        /// Finds the value for a key.
        /// </summary>
        /// <param name="key">key to get value for</param>
        /// <param name="presentableObject">presentable object to get
        /// lookup values for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        /// <returns>value for key if key is contained or null if key
        /// is not contained</returns>
        public override string FindValueForKey(string key, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string value;
            if (string.IsNullOrEmpty(key)) {
                value = null;
            } else {
                var filterCriteria = new FilterCriteria(nameof(IUser.UserName), RelationalOperator.IsEqualTo, key, FilterTarget.IsOtherTextValue);
                var user = optionDataProvider.UserDirectory.FindOne(filterCriteria, SortCriterionCollection.Empty);
                if (null == user) {
                    value = null;
                } else {
                    value = user.DisplayName + " (" + user.UserName + ")";
                }
            }
            return value;
        }

    }

}