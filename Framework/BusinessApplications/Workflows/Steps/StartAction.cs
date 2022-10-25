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

    using Framework.Properties;

    /// <summary>
    /// Start action of a workflow.
    /// </summary>
    public sealed class StartAction : PlaceholderAction {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public StartAction()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Gets the display title of workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to workflow</param>
        protected internal override string GetTitle(WorkflowControlledObject associatedObject) {
            return Resources.Start;
        }

    }

}