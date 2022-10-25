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

namespace Framework.BusinessApplications.Web.Controllers {

    using Framework.BusinessApplications.DataControllers;
    using Framework.Presentation;
    using Framework.Presentation.Web;
    using Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// HTTP controller for responding business web pages for list
    /// tables.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class ListTablePageController<T> : MasterDetailPageController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Default maximum number of providable objects per &quot;list&quot;
        /// page if top parameter is not present.
        /// </summary>
        public override ulong DefaultMaximumSubsetSize {
            get {
                if (!this.defaultMaximumSubsetSize.HasValue) {
                    if (this.ListTableDataController?.GetListTableView()?.Groupings > 0) {
                        this.defaultMaximumSubsetSize = ulong.MaxValue;
                    } else {
                        this.defaultMaximumSubsetSize = base.DefaultMaximumSubsetSize;
                    }
                }
                return this.defaultMaximumSubsetSize.Value;
            }
            set { this.defaultMaximumSubsetSize = value; }
        }
        private ulong? defaultMaximumSubsetSize;

        /// <summary>
        /// Data controller for list table and forms.
        /// </summary>
        public ListTableDataController<T> ListTableDataController { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of list table page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="listTableDataController">data controller for
        /// list table and forms</param>
        public ListTablePageController(IBusinessApplication businessApplication, string absoluteUrl, ListTableDataController<T> listTableDataController)
            : base(businessApplication, absoluteUrl, listTableDataController) {
            this.ListTableDataController = listTableDataController;
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
        /// <param name="pageTitle">title of list page to be set</param>
        /// <returns>new control to be rendered</returns>
        protected override ICollection<Control> GetControlsForBusinessObjects(ICollection<T> businessObjects, bool isSubset, string description, out string pageTitle) {
            var listTableView = this.ListTableDataController.GetListTableView();
            if (string.IsNullOrEmpty(listTableView.Title)) {
                listTableView.Description = ("Please set the title property of list table view in order to display the page title correctly. " + listTableView.Description).TrimEnd();
            }
            if (!string.IsNullOrEmpty(description)) {
                listTableView.Description += ' ' + description;
            }
            pageTitle = listTableView.Title;
            var listTable = new ListTable(businessObjects, listTableView, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(FieldRenderMode.ListTable)) {
                IsFilterable = !isSubset,
                OnClickUrlDelegate = this.GetOnClickUrlDelegate(businessObjects)
            };
            return new Control[] { listTable };
        }

    }

}