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
    using Framework.BusinessApplications.Widgets;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract base class for form data controllers.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public abstract class FormDataController<T> where T : class, IProvidableObject {

        /// <summary>
        /// Provider of data.
        /// </summary>
        public DataProvider<T> DataProvider { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dataProvider">provider of form data</param>
        public FormDataController(DataProvider<T> dataProvider)
            : base() {
            this.DataProvider = dataProvider;
        }

        /// <summary>
        /// Gets the buttons to show on edit form of existing items.
        /// </summary>
        /// <param name="element">object to get buttons for</param>
        /// <returns>enumerable of buttons</returns>
        public virtual IEnumerable<ActionButton> GetEditFormButtons(T element) {
            var allUsers = new Group("All users");
            allUsers.Members.Add(UserDirectory.AnonymousUser);
            var formView = this.GetEditFormView(element);
            if (null != formView && !formView.IsReadOnlyFor(element)) {
                var saveButton = new SaveButton<T>(Resources.Save, this.DataProvider);
                saveButton.AllowedGroupsForReading.Add(allUsers);
                saveButton.RedirectionTarget = "./";
                yield return saveButton;
            }
            var cancelButton = new CancelButton(Resources.Cancel);
            cancelButton.AllowedGroupsForReading.Add(allUsers);
            yield return cancelButton;
        }

        /// <summary>
        /// Gets the edit form view for existing items.
        /// </summary>
        /// <param name="element">object to get form view for</param>
        /// <returns>edit form view for existing items</returns>
        public abstract FormView GetEditFormView(T element);

        /// <summary>
        /// Gets the buttons to show on edit form of new items.
        /// </summary>
        /// <param name="element">object to get buttons for</param>
        /// <returns>enumerable of buttons</returns>
        public virtual IEnumerable<ActionButton> GetNewFormButtons(T element) {
            var allUsers = new Group("All users");
            allUsers.Members.Add(UserDirectory.AnonymousUser);
            var saveButton = new SaveButton<T>(Resources.Save, this.DataProvider);
            saveButton.AllowedGroupsForReading.Add(allUsers);
            if (null == this.GetViewFormView(element)) {
                saveButton.RedirectionTarget = "../";
            } else {
                saveButton.RedirectionTarget = "../" + element.Id.ToString("N") + "/";
            }
            yield return saveButton;
            var cancelButton = new CancelButton(Resources.Cancel);
            cancelButton.AllowedGroupsForReading.Add(allUsers);
            cancelButton.RedirectionTarget = "../";
            yield return cancelButton;
        }

        /// <summary>
        /// Gets the edit form view for new items.
        /// </summary>
        /// <param name="type">type of new object</param>
        /// <returns>edit form view for new items</returns>
        public virtual FormView GetNewFormView(Type type) {
            T element = this.DataProvider.Create(type);
            if (null == element) {
                element = Activator.CreateInstance(type) as T;
            }
            return this.GetEditFormView(element);
        }

        /// <summary>
        /// Gets the display title of a specific hashed type.
        /// </summary>
        /// <param name="hashedType">hashed type to get display title
        /// for</param>
        /// <returns>display title of specific hashed type</returns>
        public abstract string GetTitleOfNewElement(string hashedType);

        /// <summary>
        /// Gets the actual type of a specific hashed type.
        /// </summary>
        /// <param name="hashedType">hashed type to get actual type
        /// for</param>
        /// <returns>actual type of specific hashed type</returns>
        public abstract Type GetTypeOfNewElement(string hashedType);

        /// <summary>
        /// Gets the buttons to show on view form of items.
        /// </summary>
        /// <param name="element">object to get buttons for</param>
        /// <returns>enumerable of buttons</returns>
        public virtual IEnumerable<ActionButton> GetViewFormButtons(T element) {
            var allUsers = new Group("All users");
            allUsers.Members.Add(UserDirectory.AnonymousUser);
            var formView = this.GetEditFormView(element);
            if (null != formView && !formView.IsReadOnlyFor(element)) {
                var editButton = new EditButton<T>(Resources.Edit, this);
                editButton.AllowedGroupsForReading.Add(allUsers);
                yield return editButton;
            }
            var cancelButton = new CancelButton(Resources.Close);
            cancelButton.AllowedGroupsForReading.Add(allUsers);
            cancelButton.RedirectionTarget = "../";
            yield return cancelButton;
            if (element.HasVersions) {
                var versionsButton = new HistoryButton<T>(Resources.History, this);
                versionsButton.AllowedGroupsForReading.Add(allUsers);
                yield return versionsButton;
            }
        }

        /// <summary>
        /// Gets the view form for items.
        /// </summary>
        /// <param name="element">object to get form view for</param>
        /// <returns>view form view for items</returns>
        public virtual FormView GetViewFormView(T element) {
            FormView viewFormView;
            var editFormView = this.GetEditFormView(element);
            if (null == editFormView) {
                viewFormView = null;
            } else {
                viewFormView = new FormView();
                viewFormView.CopyFrom(editFormView, CopyBehaviorForAllowedGroups.DoNotCopy, CopyBehaviorForAggregations.ShallowCopy, CopyBehaviorForCompositions.DeepCopy);
                viewFormView.SetReadOnly();
            }
            return viewFormView;
        }

        /// <summary>
        /// Gets the view form widgets for items.
        /// </summary>
        /// <param name="element">object to get view form widgets for</param>
        /// <returns>view form widgets for items</returns>
        public virtual IEnumerable<ViewWidget> GetViewFormWidgets(T element) {
            yield break;
        }

    }

}