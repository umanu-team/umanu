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
    using Framework.Persistence.Exceptions;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Action bar control.
    /// </summary>
    public class ActionBar : ActionBar<IProvidableObject> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ActionBar()
            : base(null) {
            // nothing to do
        }

    }

    /// <summary>
    /// Action bar control.
    /// </summary>
    /// <typeparam name="T">type of providable objects</typeparam>
    public class ActionBar<T> : CascadedControl where T : class, IProvidableObject {

        /// <summary>
        /// Current form or null.
        /// </summary>
        public IForm Form { get; private set; }

        /// <summary>
        /// Global actions.
        /// </summary>
        public GlobalActionSection GlobalActions { get; private set; }

        /// <summary>
        /// True if action bar is empty, false otherwise.
        /// </summary>
        public bool IsEmpty {
            get {
                return this.Controls.Count < 1 && this.GlobalActions.IsEmpty;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ActionBar()
            : this(null) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="form">current form or null</param>
        public ActionBar(IForm form)
            : base("div") {
            this.CssClasses.Add("actionbar");
            this.Form = form;
            this.GlobalActions = new GlobalActionSection();
        }

        /// <summary>
        /// Adds an action button to action bar.
        /// </summary>
        /// <param name="button">action button to add to action bar</param>
        public void AddButton(ActionButton button) {
            this.AddButton(button, null);
        }

        /// <summary>
        /// Adds an action button to action bar.
        /// </summary>
        /// <param name="button">action button to add to action bar</param>
        /// <param name="positionId">ID of position of parent widget
        /// or null</param>
        public void AddButton(ActionButton button, ulong? positionId) {
            if (button is ClientSideButton clientSideButton) {
                if (clientSideButton is CancelButton cancelButton) {
                    this.Controls.Add(new CancelWebButton(cancelButton));
                } else if (clientSideButton is EditButton<T> editButton) {
                    this.Controls.Add(new EditWebButton<T>(editButton));
                } else {
                    var buttonControl = new ClientSideWebButton(clientSideButton, positionId);
                    buttonControl.CssClasses.Add("offline");
                    this.Controls.Add(buttonControl);
                }
            } else {
                if (button is SaveButton<T> saveButton) {
                    this.Controls.Add(new SaveWebButton<T>(saveButton, this.Form));
                } else if (button is ServerSideButton serverSideButton) {
                    this.Controls.Add(new ServerSideWebButton<T>(serverSideButton, this.Form));
                } else {
                    throw new PersistenceException("Action button is of unknown type " + button.GetType().AssemblyQualifiedName + ".");
                }
            }
            return;
        }

        /// <summary>
        /// Adds a range of action buttons to action bar.
        /// </summary>
        /// <param name="buttons">action buttons to add to action bar</param>
        public void AddButtonRange(IEnumerable<ActionButton> buttons) {
            this.AddButtonRange(buttons, null);
        }

        /// <summary>
        /// Adds a range of action buttons to action bar.
        /// </summary>
        /// <param name="buttons">action buttons to add to action bar</param>
        /// <param name="positionId">ID of position of parent widget
        /// or null</param>
        public void AddButtonRange(IEnumerable<ActionButton> buttons, ulong? positionId) {
            foreach (var button in buttons) {
                this.AddButton(button, positionId);
            }
            return;
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            base.CreateChildControls(httpRequest);
            this.GlobalActions.CreateChildControls(httpRequest);
            return;
        }

        /// <summary>
        /// Gets a value indicating whether control is supposed to be
        /// rendered.
        /// </summary>
        /// <returns>true if control is supposed to be rendered,
        /// false otherwise</returns>
        protected override bool GetIsVisible() {
            return !this.IsEmpty;
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public override void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            base.HandleEvents(httpRequest, httpResponse);
            this.GlobalActions.HandleEvents(httpRequest, httpResponse);
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            base.RenderChildControls(html);
            this.GlobalActions.Render(html);
            return;
        }

    }

}