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
    using Framework.Presentation.Converters;
    using Model;
    using System;
    using System.Web;

    /// <summary>
    /// HTTP controller for REST service of lookup fields.
    /// </summary>
    public class LookupController : IHttpController {

        /// <summary>
        /// Absolute path of lookup controller - it may not be empty,
        /// not contain any special charaters except for dashes and
        /// has to start with a slash.
        /// </summary>
        public string AbsolutePath {
            get {
                return this.absolutePath;
            }
            private set {
                if (!value.StartsWith("/", StringComparison.Ordinal)) {
                    throw new ArgumentException("Absolute URL path \"" + value + "\" of lookup controller does not start with a slash.");
                }
                this.absolutePath = value;
            }
        }
        private string absolutePath;

        /// <summary>
        /// Element to get lookup values for.
        /// </summary>
        public IPresentableObject Element { get; private set; }

        /// <summary>
        /// Form view to use.
        /// </summary>
        public FormView FormView { get; private set; }

        /// <summary>
        /// Data provider to use for lookup providers.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="absolutePath">absolute path of lookup
        /// controller - it may not be empty, not contain any special
        /// charaters except for dashes and has to start with a slash</param>
        /// <param name="element">element to get lookup values for</param>
        /// <param name="formView">form view to use</param>
        /// <param name="optionDataProvider">data provider to use for
        /// lookup providers</param>
        public LookupController(string absolutePath, IPresentableObject element, FormView formView, IOptionDataProvider optionDataProvider) {
            this.AbsolutePath = absolutePath;
            this.Element = element;
            this.FormView = formView;
            this.OptionDataProvider = optionDataProvider;
        }

        /// <summary>
        /// Gets up to 512 values matching query string as JSON list.
        /// </summary>
        /// <param name="lookupProvider">lookup provider to get
        /// values from</param>
        /// <param name="query">query string to get values for</param>
        /// <returns>up to 512 values matching query string as JSON
        /// list</returns>
        private string GetJsonValueListForQuery(LookupProvider lookupProvider, string query) {
            var jsonBuilder = new JsonBuilder();
            jsonBuilder.AppendArrayStart();
            bool isFirstValue = true;
            var values = lookupProvider.FindValuesByVagueTerm(query, this.Element, this.OptionDataProvider);
            ushort i = 0;
            foreach (var value in values) {
                if (null != value) {
                    if (isFirstValue) {
                        isFirstValue = false;
                    } else {
                        jsonBuilder.AppendSeparator();
                    }
                    jsonBuilder.AppendValue(value, true);
                    if (i < 512) { // Chromium displays the first 512 results only anyway
                        i++;
                    } else {
                        break;
                    }
                }
            }
            jsonBuilder.AppendArrayEnd();
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// Indicates whether current user is allowed to access the
        /// lookup provider for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to check permissions of
        /// lookup provider for</param>
        /// <returns>true if current user is allowed to access lookup
        /// provider for specifik key chain, false otherwise</returns>
        private bool IsCurrentUserAllowedToAccessLookupProviderFor(string[] keyChain) {
            bool isCurrentUserAllowed = false;
            if (null != this.Element && false == this.FormView.FindOneViewField(keyChain)?.IsReadOnly) {
                bool isPresentableFieldFound = false;
                while (!isPresentableFieldFound && keyChain.LongLength > 0) {
                    foreach (var presentableField in this.Element.FindPresentableFields(keyChain)) {
                        isPresentableFieldFound = true;
                        if (!presentableField.IsReadOnly) {
                            isCurrentUserAllowed = true;
                            break;
                        }
                    }
                    keyChain = KeyChain.RemoveLastLinkOf(keyChain);
                }
            }
            return isCurrentUserAllowed;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = false;
            if (httpRequest.Url.AbsolutePath.StartsWith(this.AbsolutePath) && httpRequest.Url.AbsolutePath.EndsWith(".json")) {
                string key = HttpUtility.UrlDecode(httpRequest.Url.AbsolutePath.Substring(this.AbsolutePath.Length, httpRequest.Url.AbsolutePath.Length - this.AbsolutePath.Length - 5));
                var keyChain = KeyChain.FromKey(key);
                KeyChain.RemoveIndexesFrom(keyChain);
                var viewField = this.FormView?.FindOneViewField(keyChain) as IViewFieldWithLookupProvider;
                if (null != viewField && !viewField.IsReadOnly && this.IsCurrentUserAllowedToAccessLookupProviderFor(keyChain)) {
                    int urlParametersIndex = httpRequest.RawUrl.IndexOf('?');
                    if (urlParametersIndex > 0) {
                        var urlParameters = httpRequest.RawUrl.Substring(urlParametersIndex + 1).Split('&');
                        foreach (string urlParameter in urlParameters) {
                            if (urlParameter.StartsWith("q=")) {
                                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                                    string query = HttpUtility.UrlDecode(urlParameter.Substring(2));
                                    httpResponse.Clear();
                                    httpResponse.AppendHeader("Cache-Control", "no-store");
                                    httpResponse.Charset = "utf-8";
                                    httpResponse.ContentType = "application/json";
                                    httpResponse.Write(this.GetJsonValueListForQuery(viewField.GetLookupProvider(), query));
                                } else {
                                    OptionsController.RejectRequest(httpResponse);
                                }
                                isProcessed = true;
                                break;
                            }
                        }
                    }
                }
            }
            return isProcessed;
        }

    }

}