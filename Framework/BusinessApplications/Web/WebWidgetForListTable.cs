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

namespace Framework.BusinessApplications.Web {

    using Framework.BusinessApplications.DataControllers;
    using Presentation;
    using Presentation.Web;
    using System.Collections.Generic;
    using Widgets;

    /// <summary>
    /// Web widget for list table.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class WebWidgetForListTable<T> : WebWidgetForMasterDetail<T> where T : class, IProvidableObject {

        /// <summary>
        /// View widget to build control for.
        /// </summary>
        private readonly ViewWidgetForListTable<T> viewWidget;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewWidget">view widget to build control for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="webFactory">factory for building form
        /// controls</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        public WebWidgetForListTable(ViewWidgetForListTable<T> viewWidget, string fileBaseDirectory, WebFactory webFactory, ulong positionId)
            : base(viewWidget, fileBaseDirectory, webFactory, positionId) {
            this.viewWidget = viewWidget;
        }

        /// <summary>
        /// Gets the control to be rendered for business objects.
        /// </summary>
        /// <param name="businessObjects">business objects to be
        /// displayed</param>
        /// <param name="isSubset">true if business objects are a
        /// subset, false if they are total</param>
        /// <param name="description">description text to be
        /// displayed</param>
        /// <param name="widgetTitle">title of widget to be set</param>
        /// <returns>new control to be rendered</returns>
        protected override ICollection<Control> GetControlsForBusinessObjects(ICollection<T> businessObjects, bool isSubset, string description, out string widgetTitle) {
            var listTableView = (this.viewWidget.MasterDetailDataController as ListTableDataController<T>).GetListTableView();
            listTableView.Description = description;
            widgetTitle = listTableView.Title;
            var listTable = new ListTable(businessObjects, listTableView, this.FileBaseDirectory, this.WebFactory) {
                IsFilterable = !isSubset,
                OnClickUrlDelegate = this.GetOnClickUrlDelegate(businessObjects)
            };
            return new Control[] { listTable };
        }

    }

}