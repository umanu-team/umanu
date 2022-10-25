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

    using System.Collections.Generic;
    using Framework.Model;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Waits for certain fields in associated object to have
    /// specific values.
    /// </summary>
    public abstract class WaitForFieldValuesAction : ActionStep {

        /// <summary>
        /// True if workflow step is supposed to show the average
        /// duration in graphical representations of workflows, false
        /// otherwise.
        /// </summary>
        public override bool IsDisplayingAverageDuration {
            get { return true; }
        }

        /// <summary>
        /// True to ignore missing fields, false otherwise.
        /// </summary>
        protected virtual bool IgnoreMissingFields {
            get { return false; }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public WaitForFieldValuesAction()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Executes this workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>result of execution of workflow step</returns>
        protected internal override WorkflowStepResult Execute(WorkflowControlledObject associatedObject) {
            var condition = this.GetCondition(associatedObject);
            bool valuesMustMatch = WaitForFieldValuesCondition.AllFieldValuesMustMatch == condition
                || WaitForFieldValuesCondition.OneFieldValueMustMatch == condition;
            bool oneOccurrenceIsSufficient = WaitForFieldValuesCondition.OneFieldValueMustMatch == condition
                || WaitForFieldValuesCondition.OneFieldValueMustNotMatch == condition;
            WorkflowStepResult result;
            if (oneOccurrenceIsSufficient) {
                result = new WorkflowStepResult();
            } else { // !oneOccurrenceIsSufficient
                result = new WorkflowStepResult(this.NextStep);
            }
            foreach (var fieldValue in this.GetFieldValues(associatedObject)) {
                IPresentableField presentableField = associatedObject.FindPresentableField(fieldValue.KeyField);
                if (null != presentableField) {
                    bool isValueFound = false;
                    if (presentableField.IsForSingleElement) {
                        var fieldForElement = presentableField as IPresentableFieldForElement;
                        isValueFound = fieldForElement.ValueAsString == fieldValue.Value;
                    } else {
                        var fieldForCollection = presentableField as IPresentableFieldForCollection;
                        foreach (string value in fieldForCollection.GetValuesAsString()) {
                            if (value == fieldValue.Value) {
                                isValueFound = true;
                                break;
                            }
                        }
                    }
                    if (oneOccurrenceIsSufficient) {
                        if (valuesMustMatch && isValueFound || !valuesMustMatch && !isValueFound) {
                            result = new WorkflowStepResult(this.NextStep);
                            break;
                        }
                    } else { // !oneOccurrenceIsSufficient
                        if (valuesMustMatch && !isValueFound || !valuesMustMatch && isValueFound) {
                            result = new WorkflowStepResult();
                            break;
                        }
                    }
                } else if (!this.IgnoreMissingFields) {
                    throw new KeyNotFoundException("Presentable field for view field with key \"" + fieldValue.KeyField + "\" cannot be found.");
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the condition to be fulfilled in order to pass
        /// workflow step.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>condition to be fulfilled in order to pass
        /// workflow step</returns>
        protected abstract WaitForFieldValuesCondition GetCondition(WorkflowControlledObject associatedObject);

        /// <summary>
        /// Gets the dictionary of values of fields having specific
        /// keys to wait for.
        /// </summary>
        /// <param name="associatedObject">object which is associated
        /// to this workflow instance</param>
        /// <returns>dictionary of values of fields having specific
        /// keys to wait for</returns>
        protected abstract IEnumerable<KeyValuePair> GetFieldValues(WorkflowControlledObject associatedObject);

    }

}