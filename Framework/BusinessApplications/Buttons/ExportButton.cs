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

namespace Framework.BusinessApplications.Buttons {

    using Framework.BusinessApplications.DataControllers;
    using Framework.BusinessApplications.Web;
    using Framework.BusinessApplications.Web.Controllers;
    using Framework.BusinessApplications.Widgets;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Web.Controllers;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Button of action bar for canceling edit forms.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public class ExportButton<T> : ClientSideButton where T : class, IProvidableObject {

        /// <summary>
        /// Controller for resolval of data to be exported.
        /// </summary>
        protected ListTableDataController<T> ListTableDataController { get; private set; }

        /// <summary>
        /// Controller for search.
        /// </summary>
        public ISearchController<T> SearchController { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="listTableDataController">controller for
        /// resolval of data to be exported</param>
        public ExportButton(string title, ListTableDataController<T> listTableDataController)
            : this(title, listTableDataController, listTableDataController as ISearchController<T>) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="listTableDataController">controller for
        /// resolval of data to be exported</param>
        /// <param name="searchController">controller for search</param>
        public ExportButton(string title, ListTableDataController<T> listTableDataController, ISearchController<T> searchController)
            : base(title) {
            this.ListTableDataController = listTableDataController;
            this.SearchController = searchController;
        }

        /// <summary>
        /// Gets the child controllers for action button.
        /// </summary>
        /// <param name="element">object to get child controllers for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page</param>
        /// <returns>child controllers for form</returns>
        public override IEnumerable<IHttpController> GetChildControllers(IProvidableObject element, IBusinessApplication businessApplication, string absoluteUrl) {
            foreach (var childController in base.GetChildControllers(element, businessApplication, absoluteUrl)) {
                yield return childController;
            }
            if (null == this.SearchController) {
                yield return new CsvListController<T>(businessApplication, absoluteUrl + "export.csv", this.ListTableDataController);
            } else {
                yield return new CsvSearchController<T>(businessApplication, absoluteUrl + "export.csv", this.ListTableDataController, this.SearchController);
            }
        }

        /// <summary>
        /// Gets the client action to execute on click - it may be
        /// null or empty.
        /// </summary>
        /// <param name="positionId">ID of position of parent widget
        /// or null</param>
        /// <returns>client action to execute on click - it may be
        /// null or empty</returns>
        public override string GetOnClientClick(ulong? positionId) {
            string positionUrl = ViewWidget.GetPositionUrlFor("./", positionId);
            var search = SearchQueryResolver<T>.BuildSearchOnlyQueryString(HttpContext.Current.Request);
            return "javascript:document.location='" + positionUrl + "export.csv" + search + "';";
        }

    }

}