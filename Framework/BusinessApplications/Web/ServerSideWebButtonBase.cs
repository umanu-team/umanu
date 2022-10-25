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
    using Framework.Presentation.Web.Controllers;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Base control for button of action bar with server-side
    /// on-click action.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    internal abstract class ServerSideWebButtonBase<T> : Control where T : class, IProvidableObject {

        /// <summary>
        /// Current form or null.
        /// </summary>
        private readonly IForm form;

        /// <summary>
        /// Server side button to display.
        /// </summary>
        public ServerSideButton ServerSideButton { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="serverSideButton">server side button to
        /// display</param>
        /// <param name="form">current form or null</param>
        public ServerSideWebButtonBase(ServerSideButton serverSideButton, IForm form)
            : base("div") {
            this.form = form;
            this.ServerSideButton = serverSideButton;
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public sealed override void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            if (this.ServerSideButton.GetHashCode().ToString(CultureInfo.InvariantCulture) == httpRequest.Form["onClickAction"]) {
                var promptInput = httpRequest.Form["promptInput"];
                if (!string.IsNullOrEmpty(promptInput)) {
                    promptInput = WebFieldForEditableValue.RemoveUnnecessaryWhiteSpace(promptInput);
                }
#if !DEBUG
                try {
#endif
                this.ServerSideButton.OnClick(this.form, promptInput);
                var redirectionTarget = this.ServerSideButton.RedirectionTarget;
                if (null != redirectionTarget) {
                    if (string.Empty == redirectionTarget) {
                        redirectionTarget = httpRequest.Url.PathAndQuery;
                    }
                    if (false != this.form?.HasValidValue) {
                        RedirectionController.RedirectRequest(httpResponse, redirectionTarget);
                    }
                }
#if !DEBUG
                } catch (System.ApplicationException exception) {
                    this.form.ErrorMessage = exception.Message;
                    Framework.Model.JobQueue.Log?.WriteEntry(exception, Diagnostics.LogLevel.Error);
                }
#endif
            }
            return;
        }

    }

}