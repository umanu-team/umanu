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
    using System.Web;

    /// <summary>
    /// Lightweight controller for processing options requests.
    /// </summary>
    public class OptionsController : IHttpController {

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
            if (httpRequest.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase)) {
                OptionsController.RespondOptions(httpResponse);
                isProcessed = true;
            } else {
                isProcessed = false;
            }
            return isProcessed;
        }

        /// <summary>
        /// Rejects a web request with unsupported method.
        /// </summary>
        /// <param name="httpResponse">http response for web request</param>
        public static void RejectRequest(HttpResponse httpResponse) {
            httpResponse.Clear();
            httpResponse.StatusCode = 405; // Method Not Allowed
            return;
        }

        /// <summary>
        /// Responds to an options request.
        /// </summary>
        /// <param name="httpResponse">http response for web request</param>
        private static void RespondOptions(HttpResponse httpResponse) {
            httpResponse.Clear();
            httpResponse.AppendHeader("Allow", "GET, POST, OPTIONS, PUT, PROPFIND, PROPPATCH, DELETE, LOCK, UNLOCK"); // TODO: MKCOL, MOVE, COPY
            httpResponse.AppendHeader("DAV", "1,2");
            httpResponse.AppendHeader("MS-Author-Via", "DAV");
            return;
        }

    }

}