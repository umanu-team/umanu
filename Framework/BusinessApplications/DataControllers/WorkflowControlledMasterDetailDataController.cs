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
    using Framework.BusinessApplications.Workflows;
    using Framework.Persistence;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract base class for master/detail data controllers for
    /// workflow controlled types.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public abstract class WorkflowControlledMasterDetailDataController<T> : PersistentMasterDetailDataController<T> where T : WorkflowControlledObject, new() {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dataProvider">provider of master/detail data</param>
        public WorkflowControlledMasterDetailDataController(WorkflowControlledDataProvider<T> dataProvider)
            : base(dataProvider) {
            // nothing to do
        }

        /// <summary>
        /// Gets the buttons to show on edit form of existing items.
        /// </summary>
        /// <param name="element">object to get buttons for</param>
        /// <returns>enumerable of buttons</returns>
        public sealed override IEnumerable<ActionButton> GetEditFormButtons(T element) {
            var editFormButtons = new List<ActionButton>(this.GetUnprocessedEditFormButtons(element));
            element?.Workflow?.ProcessEditFormButtons(editFormButtons, this.DataProvider);
            return editFormButtons;
        }

        /// <summary>
        /// Gets the edit form view for existing items. This method is
        /// sealed because the framework relies on final forms views
        /// to be returned by workflow steps. If a data provider
        /// returned different form views than the ones processed by
        /// the current workflow step, this might lead into mean
        /// errors.
        /// </summary>
        /// <param name="element">object to get form view for</param>
        public sealed override FormView GetEditFormView(T element) {
            var editFormView = this.GetUnprocessedEditFormView(element);
            element?.Workflow?.ProcessEditFormView(ref editFormView, element);
            return editFormView;
        }

        /// <summary>
        /// Gets the buttons to show on edit form of existing items.
        /// They can be processed by the current workflow step.
        /// </summary>
        /// <param name="element">object to get form view for</param>
        /// <returns>enumerable of unprocessed edit form buttons</returns>
        public virtual IEnumerable<ActionButton> GetUnprocessedEditFormButtons(T element) {
            return base.GetEditFormButtons(element);
        }

        /// <summary>
        /// Gets the edit form view for existing items. It can be
        /// processed by the current workflow step.
        /// </summary>
        /// <param name="element">object to get form view for</param>
        /// <returns>unprocessed edit form view for existing items</returns>
        public abstract FormView GetUnprocessedEditFormView(T element);

        /// <summary>
        /// Gets the buttons to show on view form of items. They can
        /// be processed by the current workflow step.
        /// </summary>
        /// <param name="element">object to get form view for</param>
        /// <returns>enumerable of unprocessed view form buttons</returns>
        public virtual IEnumerable<ActionButton> GetUnprocessedViewFormButtons(T element) {
            foreach (var button in base.GetViewFormButtons(element)) {
                yield return button;
            }
            var workflowButton = new WorkflowDiagramButton<T>(this.DataProvider);
            workflowButton.AllowedGroupsForReading.AddRange(element.AllowedGroups.ForReading);
            yield return workflowButton;
        }

        /// <summary>
        /// Gets the view form view. It can be processed by the
        /// current workflow step.
        /// </summary>
        /// <param name="element">object to get form view for</param>
        /// <returns>unprocessed view form view</returns>
        public virtual FormView GetUnprocessedViewFormView(T element) {
            FormView viewFormView;
            var editFormView = this.GetUnprocessedEditFormView(element);
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
        /// Gets the buttons to show on view form of items.
        /// </summary>
        /// <param name="element">object to get buttons for</param>
        /// <returns>enumerable of buttons</returns>
        public sealed override IEnumerable<ActionButton> GetViewFormButtons(T element) {
            var viewFormButtons = new List<ActionButton>(this.GetUnprocessedViewFormButtons(element));
            element?.Workflow?.ProcessViewFormButtons(viewFormButtons, this.DataProvider);
            return viewFormButtons;
        }

        /// <summary>
        /// Gets the view form view for items. This method is
        /// sealed because the framework relies on final forms views
        /// to be returned by workflow steps. If a data provider
        /// returned different form views than the ones processed by
        /// the current workflow step, this might lead into mean
        /// errors.
        /// </summary>
        /// <param name="element">object to get form view
        /// for</param>
        public sealed override FormView GetViewFormView(T element) {
            var viewFormView = this.GetUnprocessedViewFormView(element);
            element?.Workflow?.ProcessViewFormView(ref viewFormView, element);
            return viewFormView;
        }

    }

}