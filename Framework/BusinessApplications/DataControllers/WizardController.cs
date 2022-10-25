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

namespace Framework.BusinessApplications.DataControllers {

    using Framework.BusinessApplications.Buttons;
    using Framework.BusinessApplications.DataProviders;
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
    /// Base class of wizard controllers to be used for wizard pages.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public abstract class WizardController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Updated data object of form - this is null if no update
        /// took place.
        /// </summary>
        protected internal T UpdatedFormData { get; internal set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WizardController()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the providable object to be used for form.
        /// </summary>
        /// <param name="id">ID from request URL or null</param>
        /// <returns>providable object to be used for form</returns>
        public abstract T CreateElement(Guid? id);

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
                var dataProvider = new WizardDataProvider<T>(this);
                var saveButton = new SaveButton<T>(Resources.Save, dataProvider);
                saveButton.AllowedGroupsForReading.Add(allUsers);
                saveButton.RedirectionTarget = null;
                yield return saveButton;
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
        /// Gets the HTTP controllers to respond for entered form
        /// data.
        /// </summary>
        /// <param name="element">providable object
        /// containing the entered form data</param>
        /// <param name="pageName">name of requested page - all
        /// returned HTTP controllers must listen to this page name
        /// in order to be processed</param>
        /// <returns>HTTP contollers to use as response to entered
        /// form data</returns>
        public abstract IEnumerable<IHttpController> ProcessFormDataOf(T element, string pageName);

    }

}