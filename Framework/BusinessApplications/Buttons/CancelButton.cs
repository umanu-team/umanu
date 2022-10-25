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

namespace Framework.BusinessApplications.Buttons {

    using Framework.Presentation.Buttons;
    using System.Web;

    /// <summary>
    /// Button of action bar for cancelling forms.
    /// </summary>
    public class CancelButton : ClientSideButton {

        /// <summary>
        /// URL to redirect to after button click. If null, history
        /// is gone back on click.
        /// </summary>
        public string RedirectionTarget { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        public CancelButton(string title)
            : base(title) {
            this.RedirectionTarget = "view.html";
        }

        /// <summary>
        /// Gets the client action to execute on click - it may be
        /// null or empty.
        /// </summary>
        /// <param name="positionId">ID of position of parent widget
        /// or null</param>
        /// <returns>client action to execute on click - it may be
        /// null or empty</returns>
        public override string GetOnClientClick(ulong? positionId) {
            string onClientClick;
            if (string.IsNullOrEmpty(this.RedirectionTarget) || (null != HttpContext.Current?.Request?.UrlReferrer && this.GetAbsoluteRedirectionUrl() == HttpContext.Current.Request.UrlReferrer.AbsolutePath)) {
                onClientClick = "javascript:$('body').upcore('allowUnsafeUnload');history.back();this.onclick=false;";
            } else {
                onClientClick = "javascript:$('body').upcore('allowUnsafeUnload');document.location='" + this.RedirectionTarget + "';";
            }
            return onClientClick;
        }

        /// <summary>
        /// Gets the absolute redirection URL. This is used to find
        /// out whether it is okay to just go back in history instead
        /// of reloading the whole redirection target page.
        /// </summary>
        /// <returns>absolute redirection URL</returns>
        private string GetAbsoluteRedirectionUrl() {
            string absoluteRedirectionUrl;
            if (this.RedirectionTarget.Contains("//") || this.RedirectionTarget.StartsWith("/")) {
                absoluteRedirectionUrl = this.RedirectionTarget;
            } else {
                int lastSlashPosition;
                absoluteRedirectionUrl = HttpContext.Current.Request.Url.AbsolutePath;
                if (!absoluteRedirectionUrl.EndsWith("/")) {
                    lastSlashPosition = absoluteRedirectionUrl.LastIndexOf('/');
                    if (lastSlashPosition > 0) {
                        absoluteRedirectionUrl = absoluteRedirectionUrl.Substring(0, lastSlashPosition + 1);
                    }
                }
                string redirectionPath, redirectionFile;
                lastSlashPosition = this.RedirectionTarget.LastIndexOf('/');
                if (lastSlashPosition > -1) {
                    var lastTargetSlashLength = lastSlashPosition + 1;
                    redirectionPath = this.RedirectionTarget.Substring(0, lastTargetSlashLength);
                    if (this.RedirectionTarget.Length > lastTargetSlashLength) {
                        redirectionFile = this.RedirectionTarget.Substring(lastTargetSlashLength);
                    } else {
                        redirectionFile = string.Empty;
                    }
                } else {
                    redirectionPath = string.Empty;
                    redirectionFile = this.RedirectionTarget;
                }
                redirectionPath = redirectionPath.Replace("/./", "/");
                if (redirectionPath.StartsWith("./")) {
                    if (redirectionPath.Length > 2) {
                        redirectionPath = redirectionPath.Substring(2);
                    } else {
                        redirectionPath = string.Empty;
                    }
                }
                while (redirectionPath.EndsWith("../")) {
                    redirectionPath = redirectionPath.Substring(0, redirectionPath.Length - 3);
                    var nextToLastSlashPosition = absoluteRedirectionUrl.Substring(0, absoluteRedirectionUrl.Length - 1).LastIndexOf('/');
                    if (nextToLastSlashPosition > 0) {
                        absoluteRedirectionUrl = absoluteRedirectionUrl.Substring(0, nextToLastSlashPosition + 1);
                    }
                }
                absoluteRedirectionUrl = absoluteRedirectionUrl + redirectionPath + redirectionFile;
            }
            return absoluteRedirectionUrl;
        }

    }

}