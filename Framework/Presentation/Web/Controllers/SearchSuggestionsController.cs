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

namespace Framework.Presentation.Web.Controllers {

    using Forms;

    /// <summary>
    /// HTTP controller for REST service of search suggestions.
    /// </summary>
    public class SearchSuggestionsController : LookupController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="absolutePath">absolute path of lookup
        /// controller - it may not be empty, not contain any special
        /// charaters except for dashes and has to start with a slash</param>
        /// <param name="endpointName">name of JSON endpoint in URL</param>
        /// <param name="lookupProvider">lookup provider to use</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        public SearchSuggestionsController(string absolutePath, string endpointName, SearchSuggestionProvider lookupProvider, IOptionDataProvider optionDataProvider)
            : base(absolutePath, new PresentableObject(), SearchSuggestionsController.GetFormView(endpointName, lookupProvider), optionDataProvider) {
            ((PresentableObject)this.Element).AddPresentableField(new PresentableFieldForString(this.Element, endpointName));
        }

        /// <summary>
        /// Get the form view to use for lookup controller.
        /// </summary>
        /// <param name="endpointName">name of JSON endpoint in URL</param>
        /// <param name="lookupProvider">lookup provider to use</param>
        /// <returns>form view to use for lookup controller</returns>
        private static FormView GetFormView(string endpointName, SearchSuggestionProvider lookupProvider) {
            var formView = new FormView();
            var viewPane = new ViewPaneForFields();
            var viewField = new ViewFieldForStringLookup(string.Empty, endpointName, Mandatoriness.Optional) {
                IsFillInAllowed = true,
                LookupProvider = lookupProvider
            };
            viewPane.ViewFields.Add(viewField);
            formView.ViewPanes.Add(viewPane);
            return formView;
        }

    }

}