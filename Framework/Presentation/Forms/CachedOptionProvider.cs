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
    /// Base class for any provider class of options to be pulled in
    /// fields of views. All options are cached for the current
    /// request, e.g. for reuse in list table views.
    /// </summary>
    public abstract class CachedOptionProvider : OptionProvider {

        /// <summary>
        /// Cached options.
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> cachedOptions;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public CachedOptionProvider()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys).
        /// </summary>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>all options of this option provider (IDs of
        /// objects or user names of users need to be set as keys)</returns>
        public abstract IEnumerable<KeyValuePair<string, string>> GetOptions(IOptionDataProvider optionDataProvider);

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
            if (null == this.cachedOptions) {
                this.cachedOptions = new List<KeyValuePair<string, string>>(this.GetOptions(optionDataProvider));
            }
            return this.cachedOptions;
        }

    }

}