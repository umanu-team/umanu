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

namespace Framework.Presentation.Web {

    using Forms;
    using Framework.Presentation.Web.Buttons;
    using Persistence;
    using Persistence.Directories;
    using Properties;
    using System.Collections.Generic;

    /// <summary>
    /// Lightweight system form web page without references.
    /// </summary>
    internal sealed class LoginFormPage : Page {

        /// <summary>
        /// True if all postback values of this page are valid, false
        /// otherwise. Be aware: This property is supposed to be set
        /// during "CreateChildControls" and will be set to null in
        /// any earlier state.
        /// </summary>
        public bool? HasValidValue {
            get { return this.form.HasValidValue; }
        }

        /// <summary>
        /// Form of page.
        /// </summary>
        private readonly Form form;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of page</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="imprintUrl">URL of imprint page</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="presentableObject">presentable object to
        /// render form for</param>
        /// <param name="viewPane">view pane to render form for</param>
        /// <param name="userDirectory"> user directory to use</param>
        public LoginFormPage(string title, string text, string imprintUrl, string privacyNoticeUrl, IPresentableObject presentableObject, ViewPane viewPane, UserDirectory userDirectory)
            : this(title, text, null, imprintUrl, privacyNoticeUrl, presentableObject, viewPane, userDirectory) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of page</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="imprintUrl">URL of imprint page</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="presentableObject">presentable object to
        /// render form for</param>
        /// <param name="viewPane">view pane to render form for</param>
        /// <param name="userDirectory"> user directory to use</param>
        /// <param name="buttonControls">controls of buttons</param>
        public LoginFormPage(string title, string text, string imprintUrl, string privacyNoticeUrl, IPresentableObject presentableObject, ViewPane viewPane, UserDirectory userDirectory, IEnumerable<Control> buttonControls)
            : this(title, text, null, imprintUrl, privacyNoticeUrl, presentableObject, viewPane, userDirectory, buttonControls) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of page</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="errorMessage">error message to be displayed</param>
        /// <param name="imprintUrl">URL of imprint page</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="presentableObject">presentable object to
        /// render form for</param>
        /// <param name="viewPane">view pane to render form for</param>
        /// <param name="userDirectory"> user directory to use</param>
        public LoginFormPage(string title, string text, string errorMessage, string imprintUrl, string privacyNoticeUrl, IPresentableObject presentableObject, ViewPane viewPane, UserDirectory userDirectory)
            : this(title, text, errorMessage, imprintUrl, privacyNoticeUrl, presentableObject, viewPane, userDirectory, new Control[] { new SubmitButton(Resources.Submit) }) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of page</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="errorMessage">error message to be displayed</param>
        /// <param name="imprintUrl">URL of imprint page</param>
        /// <param name="privacyNoticeUrl">URL of privacy notice</param>
        /// <param name="presentableObject">presentable object to
        /// render form for</param>
        /// <param name="viewPane">view pane to render form for</param>
        /// <param name="userDirectory"> user directory to use</param>
        /// <param name="buttonControls">controls of buttons</param>
        public LoginFormPage(string title, string text, string errorMessage, string imprintUrl, string privacyNoticeUrl, IPresentableObject presentableObject, ViewPane viewPane, UserDirectory userDirectory, IEnumerable<Control> buttonControls)
            : base() {
            this.Title = title;
            var articleControl = new WebControl("article");
            if (!string.IsNullOrEmpty(title)) {
                articleControl.AddChildControl(new Label("h1", title));
            }
            if (!string.IsNullOrEmpty(text)) {
                articleControl.AddChildControl(new Label("p", text));
            }
            if (!string.IsNullOrEmpty(errorMessage)) {
                var errorLabel = new Label("p", errorMessage);
                errorLabel.CssClasses.Add("formerror");
                articleControl.AddChildControl(errorLabel);
            }
            var formView = new FormView();
            formView.ViewPanes.Add(viewPane);
            var optionDataProvider = new OptionDataProvider(new DummyPersistenceMechanism(userDirectory, SecurityModel.ApplyPermissions));
            this.form = new Form(presentableObject, formView, null, optionDataProvider, BuildingRule.ThrowExceptionsOnMissingFields, null) {
                UnsavedChangesMessage = null
            };
            articleControl.AddChildControl(this.form);
            articleControl.AddChildControls(buttonControls);
            this.ContentSection.AddChildControl(articleControl);
            var footer = new WebControl("footer");
            if (!string.IsNullOrEmpty(imprintUrl)) {
                footer.AddChildControl(new Link(Resources.Imprint, imprintUrl));
            }
            if (!string.IsNullOrEmpty(privacyNoticeUrl)) {
                footer.AddChildControl(new Link(Resources.PrivacyNotice, privacyNoticeUrl));
            }
            if (!footer.IsEmpty) {
                this.ContentSection.AddChildControl(footer);
            }
        }

    }

}