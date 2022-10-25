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

    using Framework.BusinessApplications.Buttons;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Displays a button asking for data using a (potentially
    /// independent) form.
    /// </summary>
    public abstract class FormAction : ActionStep {

        /// <summary>
        /// True if workflow step is supposed to show the average
        /// duration in graphical representations of workflows, false
        /// otherwise.
        /// </summary>
        public override bool IsDisplayingAverageDuration {
            get { return true; }
        }

        /// <summary>
        /// Updated data object of form action form - this is null if
        /// no update took place.
        /// </summary>
        protected internal IProvidableObject UpdatedFormData { get; internal set; }

        /// <summary>
        /// Instantiates a new instance,
        /// </summary>
        public FormAction()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the template for the button for opening the form
        /// action form.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>template for button for opening the form action
        /// form</returns>
        public abstract ClientSideButtonTemplate GetButtonTemplate(WorkflowControlledObject associatedObject);

        /// <summary>
        /// Gets description to show on form action form.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>description to show on form action form</returns>
        public abstract string GetDescription(WorkflowControlledObject associatedObject);

        /// <summary>
        /// Gets the initial data object of form action form.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>initial data object of form action form</returns>
        public abstract IProvidableObject GetInitialFormData(WorkflowControlledObject associatedObject);

        /// <summary>
        /// Gets the list of view panes to show in form action form.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>list of view panes to show in form action form</returns>
        public abstract IEnumerable<ViewPane> GetViewPanes(WorkflowControlledObject associatedObject);

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
        protected internal override void ProcessViewFormButtons<T>(IList<ActionButton> viewFormButtons, WorkflowControlledObject associatedObject, DataProvider<T> dataProvider) {
            base.ProcessViewFormButtons<T>(viewFormButtons, associatedObject, dataProvider);
            var buttonTemplate = this.GetButtonTemplate(associatedObject);
            var button = new FormActionButton(buttonTemplate.Title, this);
            button.AllowedGroupsForReading.ReplaceBy(buttonTemplate.AllowedGroupsForReading);
            viewFormButtons.Add(button);
            return;
        }

    }

}