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

    using Framework.BusinessApplications.DataProviders;
    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web.Controllers;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Button of action bar for action form.
    /// </summary>
    /// <typeparam name="T">type of providable object to be processed
    /// on action form</typeparam>
    public abstract class ActionFormButton<T> : LinkButton where T : class, IProvidableObject {

        /// <summary>
        /// URL to redirect to after button click. A value of
        /// string.Empty causes a redirect to the same page, whereas
        /// null suppresses any redirection.
        /// </summary>
        public string RedirectionTarget {
            get { return this.saveButton.RedirectionTarget; }
            set { this.saveButton.RedirectionTarget = value; }
        }

        /// <summary>
        /// Save button of form.
        /// </summary>
        private readonly SaveButton<T> saveButton;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="urlName">name of the url for the action form
        /// </param>
        public ActionFormButton(string title, string urlName)
            : base(title, ActionFormButton<T>.GetTargetUrlFor(urlName)) {
            var dataProvider = new ActionFormDataProvider<T>(this);
            this.saveButton = new SaveButton<T>(Resources.Save, dataProvider);
            this.RedirectionTarget = "./";
        }

        /// <summary>
        /// Gets the child controllers for action button.
        /// </summary>
        /// <param name="element">object to get child controllers for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page</param>
        /// <returns>child controllers for form</returns>
        public sealed override IEnumerable<IHttpController> GetChildControllers(IProvidableObject element, IBusinessApplication businessApplication, string absoluteUrl) {
            foreach (var childController in base.GetChildControllers(element, businessApplication, absoluteUrl)) {
                yield return childController;
            }
            string absolutePath;
            if (null == element) {
                absolutePath = absoluteUrl;
            } else {
                absolutePath = absoluteUrl + element.Id.ToString("N") + '/';
            }
            yield return new ActionFormPageController<T>(this, businessApplication, absolutePath, this.TargetUrl);
        }

        /// <summary>
        /// Gets the providable object to be used for form.
        /// </summary>
        /// <returns>providable object to be used for form</returns>
        public abstract T GetElement();

        /// <summary>
        /// Gets the buttons to show on form of existing items.
        /// </summary>
        /// <param name="element">providable object to get
        /// buttons for</param>
        /// <returns>enumerable of buttons</returns>
        public virtual IEnumerable<ActionButton> GetFormButtons(T element) {
            var allUsers = new Group("All users");
            allUsers.Members.Add(UserDirectory.AnonymousUser);
            var formView = this.GetFormView(element);
            if (null != formView && !formView.IsReadOnlyFor(element)) {
                this.saveButton.AllowedGroupsForReading.Add(allUsers);
                yield return this.saveButton;
            }
            var cancelButton = new CancelButton(Resources.Cancel);
            cancelButton.AllowedGroupsForReading.Add(allUsers);
            yield return cancelButton;
        }

        /// <summary>
        /// Gets the form view for providable object.
        /// </summary>
        /// <param name="element">providable object to get form view
        /// for</param>
        /// <returns>form view for providable object</returns>
        public abstract FormView GetFormView(T element);

        /// <summary>
        /// Gets the target url based on the given url name.
        /// </summary>
        /// <param name="urlName"></param>
        /// <returns>target url based on the given url name</returns>
        private static string GetTargetUrlFor(string urlName) {
            if (urlName.Contains("/")) {
                throw new ArgumentException("URL name \'" + urlName + "\' of action form contains a slash.");
            }
            var targetUrl = urlName + ".html";
            return targetUrl;
        }

        /// <summary>
        /// Processes the entered form data.
        /// </summary>
        /// <param name="element">providable object containing the
        /// entered form data</param>
        public abstract void ProcessFormData(T element);

    }

}