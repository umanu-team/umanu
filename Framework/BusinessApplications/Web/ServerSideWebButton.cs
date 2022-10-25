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

namespace Framework.BusinessApplications.Web {

    using Framework.BusinessApplications.Buttons;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Control for button of action barwith server-side on-click
    /// action.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    internal sealed class ServerSideWebButton<T> : ServerSideWebButtonBase<T> where T : class, IProvidableObject {

        /// <summary>
        /// URL to call on form submit.
        /// </summary>
        public string ActionUrl { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="actionButton">action button to display</param>
        /// <param name="form">current form or null</param>
        public ServerSideWebButton(ServerSideButton actionButton, IForm form)
            : base(actionButton, form) {
            // nothing to do
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            this.ActionUrl = httpRequest.Url.PathAndQuery;
            base.CreateChildControls(httpRequest);
            return;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            string submitForm = "$(this).removeAttr('onclick');$('body').append($(document.createElement('div')).addClass('haze').css('opacity','0').animate({opacity:0.5}));$(this).children('form').submit();";
            if (this.ServerSideButton.PromptsForUserInput) {
                yield return new KeyValuePair<string, string>("onclick", "javascript:var s;do{s=window.prompt('" + System.Web.HttpUtility.HtmlEncode(this.ServerSideButton.ConfirmationMessage?.Replace("'", "\\'")) + "','');if(null!=s){s=s.trim();}if(s){$(this).children('form').append($(document.createElement('input')).attr({name:'promptInput',type:'hidden',value:s}));" + submitForm + "}}while(''==s)");
            } else if (string.IsNullOrEmpty(this.ServerSideButton.ConfirmationMessage)) {
                yield return new KeyValuePair<string, string>("onclick", "javascript:" + submitForm);
            } else {
                yield return new KeyValuePair<string, string>("onclick", "javascript:if(window.confirm('" + System.Web.HttpUtility.HtmlEncode(this.ServerSideButton.ConfirmationMessage?.Replace("'", "\\'")) + "')){" + submitForm + "}");
            }
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.Append("<form action=\"");
            html.Append(this.ActionUrl);
            html.Append("\" method=\"post\">");
            html.AppendHiddenInputTag("onClickAction", this.ServerSideButton.GetHashCode().ToString(CultureInfo.InvariantCulture));
            html.AppendHtmlEncoded(this.ServerSideButton.Title);
            html.Append("</form>");
            return;
        }

    }

}