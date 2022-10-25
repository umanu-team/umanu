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

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web;

    /// <summary>
    /// Lightweight controller class for processing http page
    /// requests.
    /// </summary>
    [Serializable]
    public class MultiPageController : Dictionary<string, IPage>, IHttpController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public MultiPageController()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="info">SerializationInfo populated with data</param>
        /// <param name="context">source for deserialization</param>
        protected MultiPageController(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            // nothing to do
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
            bool isProcessed;
            if (this.TryGetValue(httpRequest.Url.AbsolutePath, out IPage page)) {
                var httpMethod = httpRequest.HttpMethod.ToUpperInvariant();
                if ("GET" == httpMethod || "POST" == httpMethod) {
                    page.CreateChildControls(httpRequest);
                    page.HandleEvents(httpRequest, httpResponse);
                    page.Render(httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            } else {
                isProcessed = false;
            }
            return isProcessed;
        }

    }

}