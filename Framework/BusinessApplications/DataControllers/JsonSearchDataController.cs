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

    using Framework.BusinessApplications.DataProviders;
    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Proxy data controller to be used for JSON data search.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    internal sealed class JsonSearchDataController<T> : ListTableDataController<T> where T : PersistentObject, IProvidableObject, new() {

        /// <summary>
        /// View to apply on search results.
        /// </summary>
        private readonly SearchResultView searchResultsView;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data for preloading from</param>
        /// <param name="searchResultsView">view to apply on search
        /// results</param>
        public JsonSearchDataController(PersistenceMechanism persistenceMechanism, SearchResultView searchResultsView)
            : base(new JsonSearchDataProvider<T>(persistenceMechanism)) {
            this.searchResultsView = searchResultsView;
        }

        /// <summary>
        /// Gets the edit form view for existing items.
        /// </summary>
        /// <param name="element">object to get form view for</param>
        /// <returns>edit form view for existing items</returns>
        public override FormView GetEditFormView(T element) {
            return null;
        }

        /// <summary>
        /// Gets the buttons to show on list table.
        /// </summary>
        /// <returns>buttons to show on list table</returns>
        public override IEnumerable<ActionButton> GetListTableButtons() {
            yield break;
        }

        /// <summary>
        /// Gets the view to use for list table.
        /// </summary>
        /// <returns>view to use for list table</returns>
        public override IListTableView GetListTableView() {
            var listTableView = new ListTableView();
            if (this.searchResultsView.TitleKeyChain.LongLength > 0L) {
                listTableView.ViewFields.Add(new ViewFieldForSingleLineText(null, this.searchResultsView.TitleKeyChain, Mandatoriness.Optional));
            } else {
                listTableView.ViewFields.Add(new ViewFieldForTitle(null));
            }
            if (this.searchResultsView.DescriptionKeyChain.LongLength > 0L) {
                listTableView.ViewFields.Add(new ViewFieldForSingleLineText(null, this.searchResultsView.DescriptionKeyChain, Mandatoriness.Optional));
            }
            return listTableView;
        }

    }

}