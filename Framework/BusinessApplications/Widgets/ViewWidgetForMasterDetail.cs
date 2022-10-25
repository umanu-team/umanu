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

namespace Framework.BusinessApplications.Widgets {

    using Framework.BusinessApplications.DataControllers;
    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using Presentation.Buttons;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// View widget for list table.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public abstract class ViewWidgetForMasterDetail<T> : ViewWidget, IShortLinkableWidget where T : class, IProvidableObject {

        /// <summary>
        /// Indicates whether widget is supposed to be shown even if
        /// no data is to be displayed.
        /// </summary>
        public bool IsVisibleIfEmpty { get; set; }

        /// <summary>
        /// Data controller for list table and forms.
        /// </summary>
        public MasterDetailDataController<T> MasterDetailDataController { get; protected set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewWidgetForMasterDetail()
            : base() {
            this.IsVisibleIfEmpty = true;
        }
      
        /// <summary>
        /// Indicates whether an object with a specific ID is
        /// contained.
        /// </summary>
        /// <param name="id">specific ID to find</param>
        /// <returns>true if object with specific ID is contained,
        /// false otherwise</returns>
        public bool Contains(Guid id) {
            bool isElementWithIdContained = false;
            if (null != this.MasterDetailDataController.DataProvider.FindOne(id.ToString("N"))) {
                var businessObjects = this.MasterDetailDataController.DataProvider.GetAll();
                foreach (var businessObject in businessObjects) {
                    if (businessObject.Id == id) {
                        isElementWithIdContained = true;
                        break;
                    }
                }
            }
            return isElementWithIdContained;
        }

        /// <summary>
        /// Gets the buttons to be displayed for widget.
        /// </summary>
        /// <returns>buttons to be displayed for widget</returns>
        public override IEnumerable<ActionButton> GetButtons() {
            return this.MasterDetailDataController.GetListTableButtons();
        }

        /// <summary>
        /// Gets the child controllers for widget.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        /// <returns>child controllers for view widget</returns>
        public override IEnumerable<IHttpController> GetChildControllers(IBusinessApplication businessApplication, string absoluteUrl, ulong? positionId) {
            string positionUrl = ViewWidget.GetPositionUrlFor(absoluteUrl, positionId);
            yield return new FormPageController<T>(businessApplication, positionUrl, this.MasterDetailDataController, "view.html", FormType.View) {
                IsRedirectingToCorrectPathAutomatically = true
            };
            if (this.MasterDetailDataController is ListTableDataController<T> listTableDataController) {
                if (listTableDataController is ISearchController<T> searchController) {
                    yield return new JsonSearchController<T>(businessApplication, positionUrl + "list.json", listTableDataController, searchController);
                    yield return new RssSearchController<T>(businessApplication, positionUrl + "feed.rss", listTableDataController, searchController);
                } else {
                    yield return new JsonListController<T>(businessApplication, positionUrl + "list.json", listTableDataController);
                    yield return new RssListController<T>(businessApplication, positionUrl + "feed.rss", listTableDataController);
                }
            }
            foreach (var childController in base.GetChildControllers(businessApplication, absoluteUrl, positionId)) {
                yield return childController;
            }
            var redirectionController = new RedirectionController();
            redirectionController.RedirectionRules.Add(new RedirectionRule(positionUrl, RedirectionRuleType.Equals, absoluteUrl));
            yield return redirectionController;
        }

    }

}