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

    using Framework.Persistence;
    using Framework.Presentation.Web;
    using Framework.Properties;
    using System;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding web app manifests for business
    /// applicarions.
    /// </summary>
    public class WebAppManifestController : Presentation.Web.Controllers.WebAppManifestController {

        /// <summary>
        /// Business application to get web app manifest for.
        /// </summary>
        public IBusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// get web app manifest for</param>
        public WebAppManifestController(IBusinessApplication businessApplication)
            : base(businessApplication.WebAppManifestUrl, businessApplication.RootUrl + "offline.html") {
            this.BusinessApplication = businessApplication;
        }

        /// <summary>
        /// Creates a web page for offline info.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if offline page was created, false
        /// otherwise</returns>
        protected override bool CreateOfflinePage(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed;
            if (!string.IsNullOrEmpty(this.BusinessApplication.WebAppManifestUrl)) {
                httpResponse.AppendHeader("Cache-Control", "must-revalidate");
                httpResponse.StatusCode = 200;
                var offlinePage = new LoginInfoPage(Resources.Offline, Resources.OfflineMessage, null, null, new Control[0]);
                offlinePage.AddWebAppManifest(this.AbsoluteUrl);
                offlinePage.SetPageSettings(this.BusinessApplication.PageSettings);
                var navTitle = BusinessPageController.InitializePage(offlinePage);
                navTitle.Text = this.BusinessApplication.Title;
                offlinePage.NavigationSection.TitleControl = navTitle;
                offlinePage.NavigationSection.NavigationItems.AddRange(this.BusinessApplication.GetNavigationItems(httpRequest));
                offlinePage.NavigationSection.AutoSelectItems(httpRequest);
                offlinePage.CreateChildControls(httpRequest);
                offlinePage.HandleEvents(httpRequest, httpResponse);
                if (!httpResponse.IsRequestBeingRedirected) {
                    offlinePage.Render(httpResponse);
                }
                isProcessed = true;
            } else {
                isProcessed = false;
            }
            return isProcessed;
        }

        /// <summary>
        /// Gets the asset links file to be provided.
        /// </summary>
        /// <returns>asset links file to be provided or null</returns>
        protected override File GetAssetLinksFile() {
            return null; // not supported yet - this is necessary for publishing PWA to app store
        }

        /// <summary>
        /// Gets the web app manifest to be provided.
        /// </summary>
        /// <returns>web app manifest to be provided or null</returns>
        protected override WebAppManifest GetWebAppManifest() {
            WebAppManifest webAppManifest;
            if (!string.IsNullOrEmpty(this.BusinessApplication.WebAppManifestUrl) && this.BusinessApplication.WebAppManifestUrl.StartsWith(this.BusinessApplication.RootUrl) && !this.BusinessApplication.WebAppManifestUrl.Substring(this.BusinessApplication.RootUrl.Length).Contains("/")) {
                webAppManifest = new WebAppManifest(this.BusinessApplication.Title, this.BusinessApplication.Title, null, this.BusinessApplication.RootUrl, this.BusinessApplication.RootUrl) {
                    BackgroundColor = "#ffffff",
                    ThemeColor = '#' + ((int)this.BusinessApplication.PrimaryColor).ToString("x6")
                };
                if (string.IsNullOrEmpty(this.BusinessApplication.PageSettings.Icon192Url)) {
                    throw new ArgumentNullException(nameof(this.BusinessApplication.PageSettings.Icon192Url), "Icon of size 192 x 192 pixels must not be null in page settings.");
                } else {
                    webAppManifest.Icons.Add(new WebAppManifestImage(this.BusinessApplication.PageSettings.Icon192Url, "192x192", "maskable"));
                }
                if (string.IsNullOrEmpty(this.BusinessApplication.PageSettings.Icon512Url)) {
                    throw new ArgumentNullException(nameof(this.BusinessApplication.PageSettings.Icon512Url), "Icon of size 512 x 512 pixels must not be null in page settings.");
                } else {
                    webAppManifest.Icons.Add(new WebAppManifestImage(this.BusinessApplication.PageSettings.Icon512Url, "512x512", "any"));
                }
            } else {
                webAppManifest = null;
            }
            return webAppManifest;
        }

    }

}