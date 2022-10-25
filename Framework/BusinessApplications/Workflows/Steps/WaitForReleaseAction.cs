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
    using System.Collections.Generic;

    /// <summary>
    /// Waits for users to fire a release user task.
    /// </summary>
    public abstract class WaitForReleaseAction : ActionStep {

        /// <summary>
        /// True if workflow step is supposed to show the average
        /// duration in graphical representations of workflows, false
        /// otherwise.
        /// </summary>
        public override bool IsDisplayingAverageDuration {
            get { return true; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WaitForReleaseAction()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets a template for the release button to show on view
        /// form of items.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        /// <returns>template for release button to show on view form
        /// of items</returns>
        protected abstract ServerSideButtonTemplate GetReleaseButtonTemplate(WorkflowControlledObject associatedObject);

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
            var releaseButtonTemplate = this.GetReleaseButtonTemplate(associatedObject);
            if (string.IsNullOrEmpty(releaseButtonTemplate.HashSalt)) {
                releaseButtonTemplate.HashSalt = this.Id.ToString("N"); // to allow multiple release buttons of same type with same label
            }
            var releaseButton = new ServerSideWorkflowButton<T>(releaseButtonTemplate, dataProvider);
            releaseButton.OnButtonClick += new ServerSideWorkflowButtonClickHandler(delegate (string promptInput) {
                return new WorkflowStepResult(this.NextStep);
            });
            viewFormButtons.Add(releaseButton);
            return;
        }

    }

}