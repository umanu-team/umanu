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

    using System.Web;
    using Framework.Presentation.Web.Controllers;

    /// <summary>
    /// HTTP controller for denying users that are generally not
    /// allowed to access an application.
    /// </summary>
    public class AuthorizationController : IHttpController {

        /// <summary>
        /// Business application to process.
        /// </summary>
        public IBusinessApplication BusinessApplication { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        public AuthorizationController(IBusinessApplication businessApplication)
            : base() {
            this.BusinessApplication = businessApplication;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public virtual bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed;
            if (this.BusinessApplication.CurrentUserIsAuthorized()) {
                isProcessed = false;
            } else {
                httpResponse.Clear();
                httpResponse.StatusCode = 403;
                httpResponse.Write("Access denied.");
                isProcessed = true;
            }
            return isProcessed;
        }

    }

}