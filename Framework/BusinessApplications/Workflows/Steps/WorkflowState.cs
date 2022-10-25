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

namespace Framework.BusinessApplications.Workflows.Steps {

    using Framework.Persistence;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Base class of all workflow steps.
    /// </summary>
    public abstract class WorkflowState : PersistentObject {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WorkflowState()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the object which is associated to this workflow to
        /// be used in forms.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>object which is associated to this workflow</returns>
        protected internal virtual void GetAssociatedObjectForForms(WorkflowControlledObject associatedObject) {
            return;
        }

        /// <summary>
        /// Gets the additional buttons to be shown on edit form of
        /// items.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>additional buttons to be shown on edit form of
        /// items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        internal IEnumerable<ActionButton> GetAdditionalEditFormButtons<T>(WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            var additionalEditFormButtons = new List<ActionButton>();
            this.ProcessEditFormButtons(additionalEditFormButtons, associatedObject, dataProvider);
            return additionalEditFormButtons;
        }

        /// <summary>
        /// Gets the additional buttons to be shown on view form of
        /// items.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>additional buttons to be shown on view form of
        /// items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        internal IEnumerable<ActionButton> GetAdditionalViewFormButtons<T>(WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            var additionalViewFormButtons = new List<ActionButton>();
            this.ProcessViewFormButtons(additionalViewFormButtons, associatedObject, dataProvider);
            return additionalViewFormButtons;
        }

        /// <summary>
        /// Returns true if the specified associated object is valid
        /// for a specific form view, false otherwise.
        /// </summary>
        /// <param name="editFormView">edit form view to check
        /// validity against</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <returns>true if the specified key is valid, false
        /// otherwise</returns>
        protected bool IsValidValue(FormView editFormView, WorkflowControlledObject associatedObject, ValidityCheck validityCheck) {
            if (associatedObject.IsNew || associatedObject.IsChanged) {
                throw new WorkflowException("Validity of associated object cannot be checked because it has pending unsaved changes.");
            }
            var persistenceMechanism = associatedObject.ParentPersistentContainer.ParentPersistenceMechanism.CopyWithCurrentUserPrivileges();
            var associatedObjectWithReducedPermissions = persistenceMechanism.FindContainer<WorkflowControlledObject>().FindOne(associatedObject.Id);
            if (null == associatedObjectWithReducedPermissions) {
                throw new WorkflowException("Validity of associated object cannot be checked because it does not exist in persistence mechanism any more.");
            }
            return editFormView.IsValidValue(associatedObjectWithReducedPermissions, validityCheck, new OptionDataProvider(persistenceMechanism));
        }

        /// <summary>
        /// Adjusts/exchanges the buttons to show on edit form of
        /// items.
        /// </summary>
        /// <param name="editFormButtons">buttons for edit form to be
        /// processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>buttons to show on edit form of items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        protected internal virtual void ProcessEditFormButtons<T>(IList<ActionButton> editFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the edit form for existing items for
        /// current worflow state.
        /// </summary>
        /// <param name="editFormView">edit form for existing items
        /// to be processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal virtual void ProcessEditFormView(ref FormView editFormView, WorkflowControlledObject associatedObject) {
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the buttons to show on view form of
        /// items.
        /// </summary>
        /// <param name="viewFormButtons">buttons for view form to be
        /// processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <param name="dataProvider">data provider of parent data
        /// controller</param>
        /// <returns>buttons to show on view form of items</returns>
        /// <typeparam name="T">type of objects data provider
        /// provides</typeparam>
        protected internal virtual void ProcessViewFormButtons<T>(IList<ActionButton> viewFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) where T : class, IProvidableObject {
            return;
        }

        /// <summary>
        /// Adjusts/exchanges the view form for items for current
        /// worflow state.
        /// </summary>
        /// <param name="viewFormView">view form for items to be
        /// processed</param>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal virtual void ProcessViewFormView(ref FormView viewFormView, WorkflowControlledObject associatedObject) {
            return;
        }

    }

}