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

namespace Framework.Presentation.Buttons {

    /// <summary>
    /// Button of action bar for link.
    /// </summary>
    public class LinkButton : ClientSideButton {

        /// <summary>
        /// Confirmation message to display on click.
        /// </summary>
        public string ConfirmationMessage { get; set; }

        /// <summary>
        /// Target URL of link.
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        ///<param name="targetUrl">target URL of link</param>
        public LinkButton(string title, string targetUrl)
            : base(title) {
            this.TargetUrl = targetUrl;
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
            string onClick;
            if (string.IsNullOrEmpty(this.ConfirmationMessage)) {
                onClick = "javascript:document.location='" + this.TargetUrl + "'";
            } else {
                onClick = "javascript:if(window.confirm('" + System.Web.HttpUtility.HtmlEncode(this.ConfirmationMessage) + "')){document.location = '" + this.TargetUrl + "'}";
            }
            return onClick;

        }

    }

}