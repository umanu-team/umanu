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

    using Framework.BusinessApplications.Web.Controllers;
    using Framework.BusinessApplications.Workflows;
    using Framework.BusinessApplications.Workflows.Steps;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Web.Controllers;
    using System.Collections.Generic;

    /// <summary>
    /// Button of action bar for link to form action page.
    /// </summary>
    internal sealed class FormActionButton : LinkButton, IWorkflowDiagramCapableButton {

        /// <summary>
        /// Form action to be processed.
        /// </summary>
        private readonly FormAction formAction;

        /// <summary>
        /// Indicates whether workflow step is supposed to be visible
        /// on forms pages.
        /// </summary>
        public bool IsVisibleOnFormPages { get; set; }

        /// <summary>
        /// Indicates whether workflow step is supposed to be visible
        /// on workflow diagram pages.
        /// </summary>
        public bool IsVisibleOnWorkflowDiagramPages { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="formAction">form action to be processed</param>
        public FormActionButton(string title, FormAction formAction)
            : base(title, formAction.Id.ToString("N") + ".html") {
            this.formAction = formAction;
            this.IsVisibleOnFormPages = true;
            this.IsVisibleOnWorkflowDiagramPages = true;
        }

        /// <summary>
        /// Gets the child controllers for action button.
        /// </summary>
        /// <param name="element">object to get child controllers for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page</param>
        /// <returns>child controllers for form</returns>
        public override IEnumerable<IHttpController> GetChildControllers(IProvidableObject element, IBusinessApplication businessApplication, string absoluteUrl) {
            foreach (var childController in base.GetChildControllers(element, businessApplication, absoluteUrl)) {
                yield return childController;
            }
            yield return new WorkflowFormActionPageController(element as WorkflowControlledObject, this.formAction, businessApplication, absoluteUrl);
        }

    }

}