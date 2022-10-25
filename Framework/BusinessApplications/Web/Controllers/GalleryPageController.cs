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
    /// HTTP controller for responding business web pages for
    /// galleries.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class GalleryPageController<T> : MasterDetailPageController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Data controller for master/detail.
        /// </summary>
        public MasterDetailDataController<T> MasterDetailDataController { get; private set; }

        /// <summary>
        /// View to apply to gallery page.
        /// </summary>
        public GalleryView View { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of gallery page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="listTableDataController">data controller for
        /// list table and forms</param>
        public GalleryPageController(IBusinessApplication businessApplication, string absoluteUrl, ListTableDataController<T> listTableDataController)
            : this(businessApplication, absoluteUrl, listTableDataController, new GalleryView(listTableDataController.GetListTableView())) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of gallery page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="masterDetailDataController">data controller
        /// for master/detail</param>
        /// <param name="galleryView">view to apply to gallery page</param>
        public GalleryPageController(IBusinessApplication businessApplication, string absoluteUrl, MasterDetailDataController<T> masterDetailDataController, GalleryView galleryView)
            : base(businessApplication, absoluteUrl, masterDetailDataController) {
            this.MasterDetailDataController = masterDetailDataController;
            this.View = galleryView;
        }

        /// <summary>
        /// Gets the gallery control to be rendered.
        /// </summary>
        /// <param name="businessObjects">business objects to be
        /// displayed</param>
        /// <param name="isSubset">true if business objects are a
        /// subset, false if they are total</param>
        /// <param name="description">description text to be
        /// displayed</param>
        /// <param name="pageTitle">title of list page to be set</param>
        /// <returns>new gallery control to be rendered</returns>
        protected override ICollection<Control> GetControlsForBusinessObjects(ICollection<T> businessObjects, bool isSubset, string description, out string pageTitle) {
            pageTitle = this.View.Title;
            return new Control[] {
                new Gallery(businessObjects, this.View, this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(FieldRenderMode.ListTable)) {
                    Description = description,
                    OnClickUrlDelegate = this.GetOnClickUrlDelegate(businessObjects)
                }
            };
        }

    }

}