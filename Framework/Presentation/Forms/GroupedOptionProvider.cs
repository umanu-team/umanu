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

    using System.Collections.Generic;

    /// <summary>
    /// Provider class of groups of options to be pulled in fields of
    /// views.
    /// </summary>
    public class GroupedOptionProvider : OptionProvider {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public GroupedOptionProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the icon URLs for all option of this option
        /// provider. (IDs of objects or user names of users need to
        /// be set as keys).
        /// </summary>
        /// <param name="presentableObject">presentable object to get
        /// icon URLs for</param>
        /// <returns>icon URLs for all options of this option
        /// provider (IDs of objects or user names of users need to
        /// be set as keys)</returns>
        protected internal sealed override IEnumerable<KeyValuePair<string, string>> GetIconUrls(IPresentableObject presentableObject) {
            foreach (var optionProvider in this.GetOptionProviders()) {
                foreach (var iconUrl in optionProvider?.GetIconUrls(presentableObject)) {
                    yield return iconUrl;
                }
            }
        }

        /// <summary>
        /// Gets all option providers.
        /// </summary>
        /// <returns>all option providers</returns>
        public virtual IEnumerable<OptionProvider> GetOptionProviders() {
            yield break;
        }

        /// <summary>
        /// Gets all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys).
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object to get options for</param>
        /// <param name="topmostPresentableObject">topmost 
        /// presentable object to get options for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys)</returns>
        public sealed override IEnumerable<KeyValuePair<string, string>> GetOptions(IPresentableObject parentPresentableObject, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            foreach (var optionProvider in this.GetOptionProviders()) {
                foreach (var option in optionProvider?.GetOptions(parentPresentableObject, topmostPresentableObject, optionDataProvider)) {
                    yield return option;
                }
            }
        }

    }

}