﻿/*********************************************************************
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
    using Framework.BusinessApplications.Web;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;

    /// <summary>
    /// View widget for list table.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class ViewWidgetForListTable<T> : ViewWidgetForMasterDetail<T> where T : class, IProvidableObject {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewWidgetForListTable()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="masterDetailDataController">data controller for
        /// list and forms</param>
        internal ViewWidgetForListTable(MasterDetailDataController<T> masterDetailDataController)
            : this() {
            this.MasterDetailDataController = masterDetailDataController;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="listTableDataController">data controller for
        /// list and forms</param>
        /// <param name="isVisibleIfEmpty">indicates whether widget
        /// is supposed to be shown even if no data is to be
        /// displayed</param>
        public ViewWidgetForListTable(ListTableDataController<T> listTableDataController, bool isVisibleIfEmpty)
            : this(listTableDataController) {
            this.IsVisibleIfEmpty = isVisibleIfEmpty;
        }

        /// <summary>
        /// Gets the control for view widget.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        /// <returns>control for view widget</returns>
        public override Control GetControlWithoutWidgetCss(IBusinessApplication businessApplication, ulong positionId) {
            return new WebWidgetForListTable<T>(this, businessApplication.FileBaseDirectory, businessApplication.GetWebFactory(FieldRenderMode.ListTable), positionId);
        }

    }

}