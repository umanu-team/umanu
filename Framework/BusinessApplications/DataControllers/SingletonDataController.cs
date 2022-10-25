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

namespace Framework.BusinessApplications.DataControllers {

    using Framework.BusinessApplications.Buttons;
    using Framework.BusinessApplications.DataProviders;
    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract base class for singleton data controllers.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public abstract class SingletonDataController<T> : MasterDetailDataController<T> where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dataProvider">provider of singleton data</param>
        public SingletonDataController(SingletonDataProvider<T> dataProvider)
            : base(dataProvider) {
            // nothing to do
        }

        /// <summary>
        /// Gets key chains to be preloaded for list table view.
        /// </summary>
        /// <returns>key chains to be preloaded for list table view</returns>
        public sealed override IList<string[]> GetKeyChainsToPreloadForListTableView() {
            return new List<string[]>(0);
        }

        /// <summary>
        /// Returns list table buttons. 
        /// </summary>
        /// <returns>List table buttons.</returns>
        public sealed override IEnumerable<ActionButton> GetListTableButtons() {
            yield break;
        }

        /// <summary>
        /// Gets the buttons to show on view form of items.
        /// </summary>
        /// <param name="element">object to get buttons for</param>
        public override IEnumerable<ActionButton> GetViewFormButtons(T element) {
            foreach (var button in base.GetViewFormButtons(element)) {
                if (!(button is CancelButton)) {
                    yield return button;
                }
            }
        }

    }

}