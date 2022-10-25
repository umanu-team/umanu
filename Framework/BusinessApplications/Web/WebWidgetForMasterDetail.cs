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

    using Presentation;
    using Presentation.Forms;
    using Presentation.Web;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using Widgets;

    /// <summary>
    /// Web widget for list table.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public abstract class WebWidgetForMasterDetail<T> : WebWidget where T : class, IProvidableObject {

        /// <summary>
        /// URL of base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; private set; }

        /// <summary>
        /// Indicates whether data is to be diplayed.
        /// </summary>
        private bool isEmpty;

        /// <summary>
        /// ID of position of widget on parent page.
        /// </summary>
        public ulong PositionId { get; private set; }

        /// <summary>
        /// View widget to build control for.
        /// </summary>
        private readonly ViewWidgetForMasterDetail<T> viewWidget;

        /// <summary>
        /// Factory for building form controls.
        /// </summary>
        public WebFactory WebFactory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewWidget">view widget to build control for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="webFactory">factory for building form
        /// controls</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        public WebWidgetForMasterDetail(ViewWidgetForListTable<T> viewWidget, string fileBaseDirectory, WebFactory webFactory, ulong positionId)
            : base() {
            this.FileBaseDirectory = fileBaseDirectory;
            this.PositionId = positionId;
            this.viewWidget = viewWidget;
            this.WebFactory = webFactory;
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public sealed override void CreateChildControls(HttpRequest httpRequest) {
            var queryResolver = this.GetQueryResolver(httpRequest);
            var queryResult = queryResolver.Execute();
            if (queryResult.ProvidableObjects.Count > 0) {
                this.viewWidget.MasterDetailDataController.DataProvider.Preload(queryResult.ProvidableObjects, this.viewWidget.MasterDetailDataController.GetKeyChainsToPreloadForListTableView());
                this.isEmpty = false;
            } else {
                this.isEmpty = true;
            }
            var controlsForBusinessObjects = this.GetControlsForBusinessObjects(queryResult.ProvidableObjects, queryResult.IsSubset, queryResult.Description, out string widgetTitle);
            if (!string.IsNullOrEmpty(widgetTitle)) {
                this.Controls.Add(new Label("h1", widgetTitle));
            }
            foreach (var control in controlsForBusinessObjects) {
                this.Controls.Add(control);
            }
            base.CreateChildControls(httpRequest);
            return;
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
        protected abstract ICollection<Control> GetControlsForBusinessObjects(ICollection<T> businessObjects, bool isSubset, string description, out string widgetTitle);

        /// <summary>
        /// Gets a value indicating whether control is supposed to be
        /// rendered.
        /// </summary>
        /// <returns>true if control is supposed to be rendered,
        /// false otherwise</returns>
        protected sealed override bool GetIsVisible() {
            return this.viewWidget.IsVisibleIfEmpty || !this.isEmpty;
        }

        /// <summary>
        /// Gets the delegate for resolval of on click URL for a
        /// presentable object.
        /// </summary>
        /// <param name="businessObjects">business objects to get
        /// delegate for</param>
        /// <returns>delegate for resolval of on click URL for a
        /// presentable object</returns>
        protected virtual OnClickUrlDelegate GetOnClickUrlDelegate(IEnumerable<T> businessObjects) {
            OnClickUrlDelegate onClickUrlDelegate = null;
#if !DEBUG
            try {
#endif
            foreach (var businessObject in businessObjects) {
                if (null != this.viewWidget.MasterDetailDataController.GetViewFormView(businessObject)) {
                    onClickUrlDelegate = delegate (IPresentableObject clickedObject) {
                        string onClickUrl;
                        if (null != clickedObject) {
                            onClickUrl = "./" + this.PositionId.ToString(CultureInfo.InvariantCulture) + "/" + clickedObject.Id.ToString("N") + "/";
                        } else {
                            onClickUrl = null;
                        }
                        return onClickUrl;
                    };
                }
                break;
            }
#if !DEBUG
            } catch (System.Exception) {
                // ignore exceptions
            }
#endif
            return onClickUrlDelegate;
        }

        /// <summary>
        /// Get the resolver for providable objects based on query
        /// string.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>resolver for providable objects based on query
        /// string</returns>
        protected virtual QueryResolver<T> GetQueryResolver(HttpRequest httpRequest) {
            var dataProvider = this.viewWidget.MasterDetailDataController.DataProvider;
            var defaultMaximumSubsetSize = ulong.MaxValue; // this.viewWidget.MasterDetailDataController.DefaultMaximumSubsetSize is not used because paging puttons are not present
            return new SubsetQueryResolver<T>(httpRequest, dataProvider, defaultMaximumSubsetSize);
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public sealed override void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            base.HandleEvents(httpRequest, httpResponse);
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected sealed override void RenderChildControls(HtmlWriter html) {
            base.RenderChildControls(html);
            return;
        }

    }

}