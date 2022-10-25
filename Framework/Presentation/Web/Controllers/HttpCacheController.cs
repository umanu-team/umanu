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

    using Framework.Model;
    using System;
    using System.Web;

    /// <summary>
    /// Base class of lightweight controllers for processing http
    /// requests that can be cached.
    /// </summary>
    public abstract class HttpCacheController : IHttpController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public HttpCacheController() {
            // nothing to do
        }

        /// <summary>
        ///  Only respond file data if it was modified after this
        ///  date/time.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>if modified since date/time</returns>
        protected static DateTime GetIfModifiedSince(HttpRequest httpRequest) {
            if (DateTime.TryParse(httpRequest.Headers["If-Modified-Since"], out DateTime ifModifiedSince)) {
                ifModifiedSince = ifModifiedSince.ToUniversalTime();
            } else {
                ifModifiedSince = UtcDateTime.MinValue;
            }
            return ifModifiedSince;
        }

        /// <summary>
        ///  Only respond file data if it was not modified after this
        ///  date/time.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>if unmodified since date/time</returns>
        protected static DateTime GetIfUnmodifiedSince(HttpRequest httpRequest) {
            if (DateTime.TryParse(httpRequest.Headers["If-Unmodified-Since"], out DateTime ifUnmodifiedSince)) {
                ifUnmodifiedSince = ifUnmodifiedSince.ToUniversalTime();
            } else {
                ifUnmodifiedSince = UtcDateTime.MaxValue;
            }
            return ifUnmodifiedSince;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public abstract bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse);

    }

}