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

    using Framework.BusinessApplications;
    using Framework.BusinessApplications.Web;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding info pages.
    /// </summary>
    public class InfoPageController : BusinessPageController {

        /// <summary>
        /// Absolute URL of page - it may not be empty, not
        /// contain any special charaters except for dashes and has
        /// to start with a slash.
        /// </summary>
        public string AbsoluteUrl { get; private set; }

        /// <summary>
        /// Title and text of page.
        /// </summary>
        public InfoPage InfoPage { get; set; }

        /// <summary>
        /// Indicates whether text is supposed to be rendered as
        /// plain text or as rich text.
        /// </summary>
        public TextRenderMode TextRenderMode { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start with a slash</param>
        /// <param name="infoPage">title and text of page</param>
        /// <param name="textRenderMode">indicates whether text is
        /// supposed to be rendered as plain text or as rich text</param>
        public InfoPageController(IBusinessApplication businessApplication, string absoluteUrl, InfoPage infoPage, TextRenderMode textRenderMode)
            : base(businessApplication) {
            this.AbsoluteUrl = absoluteUrl;
            this.InfoPage = infoPage;
            this.TextRenderMode = textRenderMode;
        }

        /// <summary>
        /// Fills the page with information.
        /// </summary>
        protected void CreateInfoPage() {
            var title = this.InfoPage?.Title?.ToString();
            this.Page.Title = title;
            var actionBar = new ActionBar();
            actionBar.AddButtonRange(this.FilterButtonsForCurrentUser(this.GetButtons()));
            this.AddActionBarToPage(actionBar);
            var article = new InfoArticle(title, this.InfoPage?.Text?.ToString(), this.TextRenderMode);
            this.Page.ContentSection.AddChildControl(article);
            return;
        }

        /// <summary>
        /// Gets the buttons to be displayed on info page.
        /// </summary>
        /// <returns>buttons to be displayed on info page</returns>
        public virtual IEnumerable<ActionButton> GetButtons() {
            yield break;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public override bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = false;
            if (httpRequest.Url.AbsolutePath.Equals(this.AbsoluteUrl, StringComparison.OrdinalIgnoreCase)) {
                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase)) {
                    this.CreateInfoPage();
                    this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            }
            return isProcessed;
        }

    }

}