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
    /// Control for button of action bar with server-side on-click
    /// form action.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    internal sealed class SaveWebButton<T> : ServerSideWebButtonBase<T> where T : class, IProvidableObject {

        /// <summary>
        /// Action button to display.
        /// </summary>
        public SaveButton<T> SaveButton { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="saveButton">save button to display</param>
        /// <param name="form">current form or null</param>
        public SaveWebButton(SaveButton<T> saveButton, IForm form)
            : base(saveButton, form) {
            this.SaveButton = saveButton;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            yield return new KeyValuePair<string, string>("class", "primary");
            string formSelector;
            if (string.IsNullOrEmpty(this.SaveButton.FormId)) {
                formSelector = "form:last";
            } else {
                formSelector = "form#" + this.SaveButton.FormId;
            }
            string submitForm = "$(this).removeAttr('onclick');$('body').append($(document.createElement('div')).addClass('haze').css('opacity','0').animate({opacity:0.5}));$('" + formSelector + "').prepend($(document.createElement('input')).attr('type', 'hidden').attr('name', 'onClickAction').attr('value', '" + this.SaveButton.GetHashCode().ToString(CultureInfo.InvariantCulture) + "')).submit();";
            if (this.SaveButton.PromptsForUserInput) {
                yield return new KeyValuePair<string, string>("onclick", "javascript:var s;do{s=window.prompt('" + System.Web.HttpUtility.HtmlEncode(this.SaveButton.ConfirmationMessage?.Replace("'", "\\'")) + "','');if(null!=s){s=s.trim();}if(s){$(this).children('form').append($(document.createElement('input')).attr({name:'promptInput',type:'hidden',value:s}));" + submitForm + "}}while(''==s)");
            } else if (string.IsNullOrEmpty(this.SaveButton.ConfirmationMessage)) {
                yield return new KeyValuePair<string, string>("onclick", "javascript:" + submitForm);
            } else {
                yield return new KeyValuePair<string, string>("onclick", "javascript:if(window.confirm('" + System.Web.HttpUtility.HtmlEncode(this.SaveButton.ConfirmationMessage?.Replace("'", "\\'")) + "')){" + submitForm + "}");
            }
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendOpeningTag("span", "icon");
            html.Append("save");
            html.AppendClosingTag("span");
            html.AppendOpeningTag("span", "label");
            html.AppendHtmlEncoded(this.SaveButton.Title);
            html.AppendClosingTag("span");
            return;
        }

    }

}