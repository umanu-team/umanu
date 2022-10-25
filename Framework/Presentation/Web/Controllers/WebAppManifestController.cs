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

    using Framework.Presentation.Forms;
    using Persistence;
    using Presentation.Converters;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding web app manifests.
    /// </summary>
    public abstract class WebAppManifestController : FileController {

        /// <summary>
        /// Absolute URL of web app manifest - it may not be empty,
        /// not contain any special charaters except for dashes and
        /// has to start with a slash.
        /// </summary>
        public string AbsoluteUrl { get; private set; }

        /// <summary>
        /// Absolute URL of auto-generated page to be displayed if
        /// application is offline.
        /// </summary>
        public string OfflinePageUrl { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="absolueUrl">absolute URL of web app manifest
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start with a slash</param>
        /// <param name="offlinePageUrl">absolute URL of
        /// auto-generated page to be displayed if application is
        /// offline</param>
        public WebAppManifestController(string absolueUrl, string offlinePageUrl)
            : base(CacheControl.MustRevalidate, true) {
            this.AbsoluteUrl = absolueUrl;
            this.OfflinePageUrl = offlinePageUrl;
        }

        /// <summary>
        /// Creates a web page for offline info.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if offline page was created, false
        /// otherwise</returns>
        protected abstract bool CreateOfflinePage(HttpRequest httpRequest, HttpResponse httpResponse);

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected sealed override File FindFile(Uri url) {
            File webAppManifestFile = null;
            if (url.AbsolutePath == this.AbsoluteUrl) {
                var webAppManifest = this.GetWebAppManifest();
                if (null != webAppManifest) {
                    var viewFields = new List<ViewField> {
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.BackgroundColor), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.Description), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.Dir), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.Display), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, new string[] { nameof(WebAppManifest.Icons), nameof(WebAppManifestImage.Purpose) }, Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, new string[] { nameof(WebAppManifest.Icons), nameof(WebAppManifestImage.Src) }, Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, new string[] { nameof(WebAppManifest.Icons), nameof(WebAppManifestImage.Sizes) }, Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, new string[] { nameof(WebAppManifest.Icons), nameof(WebAppManifestImage.Type) }, Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.Lang), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.Name), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.Orientation), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.Scope), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, new string[] { nameof(WebAppManifest.Screenshots), nameof(WebAppManifestImage.Purpose) }, Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, new string[] { nameof(WebAppManifest.Screenshots), nameof(WebAppManifestImage.Src) }, Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, new string[] { nameof(WebAppManifest.Screenshots), nameof(WebAppManifestImage.Sizes) }, Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, new string[] { nameof(WebAppManifest.Screenshots), nameof(WebAppManifestImage.Type) }, Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.ShortName), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.StartUrl), Mandatoriness.Optional),
                    new ViewFieldForSingleLineText(null, nameof(WebAppManifest.ThemeColor), Mandatoriness.Optional)
                };
                    var jsonWriter = new JsonWriter(viewFields, null, null) {
                        IsWritingEmptyValues = false,
                        KeyConversion = KeyConversion.PascalCaseToSnakeCase
                    };
                    webAppManifestFile = jsonWriter.WriteFile(string.Empty, webAppManifest);
                    webAppManifestFile.MimeType = "application/manifest+json";
                }
            }
            return webAppManifestFile;
        }

        /// <summary>
        /// Gets the asset links file to be provided.
        /// </summary>
        /// <returns>asset links file to be provided or null</returns>
        protected abstract File GetAssetLinksFile();

        /// <summary>
        /// Gets the web app manifest to be provided.
        /// </summary>
        /// <returns>web app manifest to be provided or null</returns>
        protected abstract WebAppManifest GetWebAppManifest();

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public sealed override bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            var isProcessed = base.ProcessRequest(httpRequest, httpResponse);
            if (!isProcessed && httpRequest.Url.AbsolutePath == this.OfflinePageUrl) {
                if ("GET" == httpRequest.HttpMethod.ToUpperInvariant()) {
                    isProcessed = this.CreateOfflinePage(httpRequest, httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                    isProcessed = true;
                }
            }
            if (!isProcessed && httpRequest.Url.AbsolutePath == "/.well-known/assetlinks.json") {
                if ("GET" == httpRequest.HttpMethod.ToUpperInvariant()) {
                    var assetLinksFile = this.GetAssetLinksFile();
                    if (null != assetLinksFile) {
                        this.RespondFile(httpRequest, httpResponse, assetLinksFile);
                        isProcessed = true;
                    }
                } else {
                    OptionsController.RejectRequest(httpResponse);
                    isProcessed = true;
                }
            }
            return isProcessed;
        }

        /// <summary>
        /// Updates the parent persistent object of a file for a
        /// specific URL.
        /// </summary>
        /// <param name="url">URL of file to update parent persistent
        /// object for</param>
        /// <returns>true on success, false otherwise</returns>
        protected sealed override bool UpdateParentPersistentObjectOfFile(Uri url) {
            return false;
        }

    }

}