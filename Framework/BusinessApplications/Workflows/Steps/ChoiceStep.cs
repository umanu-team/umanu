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

    /// <summary>
    /// Workflow branch which is a single step of a workflow.
    /// Consider deriving from TrueFalseChoice instead if you need a
    /// branch with two possible next steps.
    /// </summary>
    public abstract class ChoiceStep : WorkflowStep {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ChoiceStep()
            : base() {
            // nothing to do
        }

    }

}