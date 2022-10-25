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

    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Controller for web applications.
    /// </summary>
    public abstract class ApplicationController : IHttpController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ApplicationController() {
            // nothing to do
        }

        /// <summary>
        /// Gets all HTTP controllers to process.
        /// </summary>
        /// <returns>HTTP controllers to process</returns>
        public abstract IEnumerable<IHttpController> GetHttpControllers();

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
            foreach (var httpController in this.GetHttpControllers()) {
                isProcessed = httpController.ProcessRequest(httpRequest, httpResponse);
                if (isProcessed) {
                    break;
                }
            }
            return isProcessed;
        }

    }

}