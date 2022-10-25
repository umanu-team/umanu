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

    using System.Web;

    /// <summary>
    /// Base class of HTTP controllers for responding web pages.
    /// </summary>
    /// <typeparam name="TPage">type of page to respond</typeparam>
    public abstract class PageController<TPage> : IHttpController where TPage: IPage, new() {

        /// <summary>
        /// Page to be rendered as result of web request.
        /// </summary>
        protected TPage Page { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="page">page to respond</param>
        public PageController(TPage page) {
            this.Page = page;
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
            this.Page.CreateChildControls(httpRequest);
            this.Page.HandleEvents(httpRequest, httpResponse);
            if (!httpResponse.IsRequestBeingRedirected) {
                this.Page.Render(httpResponse);
            }
            return true;
        }

    }

}