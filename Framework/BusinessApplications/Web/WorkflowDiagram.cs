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

namespace Framework.BusinessApplications.Web {

    using Framework.BusinessApplications.Buttons;
    using Framework.BusinessApplications.Workflows;
    using Framework.BusinessApplications.Workflows.Steps;
    using Framework.Model;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Web;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Control to be used on web diagrams of workflows.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public class WorkflowDiagram<T> : Control where T : class, IProvidableObject {

        /// <summary>
        /// Object which is associated to workflow.
        /// </summary>
        private readonly WorkflowControlledObject associatedObject;

        /// <summary>
        /// Data provider containing associated object of workflow.
        /// </summary>
        private readonly DataProvider<T> dataProvider;

        /// <summary>
        /// Dictionary of workflow step ID and inline web buttons for
        /// current workflow state.
        /// </summary>
        private IDictionary<Guid, List<Control>> inlineWebButtons;

        /// <summary>
        /// Workflow to render diagram for.
        /// </summary>
        private readonly Workflow workflow;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="workflow">workflow to render diagram for</param>
        /// <param name="dataProvider">data provider containing
        /// associated object of workflow</param>
        public WorkflowDiagram(Workflow workflow, DataProvider<T> dataProvider)
            : base("div") {
            this.associatedObject = workflow.GetAssociatedObjectForForms();
            this.CssClasses.Add("workflow");
            this.dataProvider = dataProvider;
            this.workflow = workflow;
        }

        /// <summary>
        /// Appends an action step to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="id">ID of step</param>
        /// <param name="title">title of step</param>
        /// <param name="iconUrl">URL of icon of step</param>
        /// <param name="averageDuration">average processing duration
        /// of steps of same type</param>
        /// <param name="cssClasses">CSS classes to add</param>
        /// <param name="historyItems">history items of step to
        /// render</param>
        private void AppendStepTo(HtmlWriter html, Guid id, string title, string iconUrl, TimeSpan? averageDuration, IEnumerable<string> cssClasses, IEnumerable<HistoryItem> historyItems) {
            html.AppendOpeningTag("div", "arrow");
            html.AppendClosingTag("div");
            html.AppendOpeningTag("div", cssClasses);
            if (!string.IsNullOrEmpty(iconUrl)) {
                var attributes = new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("src", iconUrl)
                };
                html.AppendOpeningTag("img", attributes);
            }
            html.AppendOpeningTag("div", "title");
            html.AppendHtmlEncoded(title);
            html.AppendClosingTag("div");
            foreach (var historyItem in historyItems) {
                html.AppendOpeningTag("div", "history");
                html.AppendHtmlEncoded(UtcDateTime.FormatAsReadOnlyValue(historyItem.PassedAt, DateTimeType.Date));
                string passedBy = historyItem.PassedBy?.DisplayName;
                if (!string.IsNullOrEmpty(passedBy)) {
                    html.Append(": ");
                    html.AppendHtmlEncoded(passedBy);
                }
                html.AppendClosingTag("div");
            }
            if (averageDuration.HasValue && averageDuration.Value.TotalMinutes > 2) {
                string averageDurationString;
                if (averageDuration.Value.TotalDays > 2) {
                    averageDurationString = Math.Round(averageDuration.Value.TotalDays).ToString() + ' ' + Resources.Days;
                } else if (averageDuration.Value.TotalHours > 2) {
                    averageDurationString = Math.Round(averageDuration.Value.TotalHours).ToString() + ' ' + Resources.Hours;
                } else {
                    averageDurationString = Math.Round(averageDuration.Value.TotalMinutes).ToString() + ' ' + Resources.Minutes;
                }
                html.AppendOpeningTag("div", "average");
                html.AppendHtmlEncoded(averageDurationString);
                html.AppendClosingTag("div");
            }
            if (this.inlineWebButtons.ContainsKey(id)) {
                foreach (var inlineWebButton in this.inlineWebButtons[id]) {
                    inlineWebButton.CssClasses.Add("diagrambutton");
                    inlineWebButton.Render(html);
                }
            }
            html.AppendClosingTag("div");
            return;
        }

        /// <summary>
        /// Appends the end of a number of lanes to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="numberOfLanes">number of lanes to end</param>
        private static void AppendEndOfLanesTo(HtmlWriter html, int numberOfLanes) {
            if (numberOfLanes > 1) {
                html.AppendClosingTag("td");
                html.AppendClosingTag("tr");
                WorkflowDiagram<T>.AppendLaneHeaderOrFooter(html, numberOfLanes);
                html.AppendClosingTag("tbody");
                html.AppendClosingTag("table");
            }
            return;
        }

        /// <summary>
        /// Appends an intermediate end to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="title">title of step</param>
        private static void AppendIntermediateEndTo(HtmlWriter html, string title) {
            html.AppendOpeningTag("div", "arrow");
            html.AppendClosingTag("div");
            html.AppendOpeningTag("div", "end");
            html.AppendHtmlEncoded(title);
            html.AppendClosingTag("div");
            return;
        }

        /// <summary>
        /// Appends a lane change to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        private static void AppendLaneChangeTo(HtmlWriter html) {
            html.AppendClosingTag("td");
            html.AppendOpeningTag("td");
            html.AppendClosingTag("td");
            html.AppendOpeningTag("td", "lane");
            return;
        }

        /// <summary>
        /// Appends a lane header/footer to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="numberOfLanes">number of lanes</param>
        private static void AppendLaneHeaderOrFooter(HtmlWriter html, int numberOfLanes) {
            int numberOfColumns = 2 * numberOfLanes - 1;
            html.AppendOpeningTag("tr");
            for (int i = 0; i < numberOfColumns; i++) {
                html.AppendOpeningTag("td");
                string cssClass;
                if (0 == i) {
                    cssClass = "leftline";
                } else if (numberOfColumns - 1 == i) {
                    cssClass = "rightline";
                } else {
                    cssClass = "fullline";
                }
                html.AppendOpeningTag("div", cssClass);
                html.AppendClosingTag("div");
                html.AppendClosingTag("td");
            }
            html.AppendClosingTag("tr");
            return;
        }

        /// <summary>
        /// Appends the start of a number of lanes to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="numberOfLanes">number of lanes to end</param>
        private static void AppendStartOfLanesTo(HtmlWriter html, int numberOfLanes) {
            if (numberOfLanes > 1) {
                html.AppendOpeningTag("table");
                html.AppendOpeningTag("tbody");
                WorkflowDiagram<T>.AppendLaneHeaderOrFooter(html, numberOfLanes);
                html.AppendOpeningTag("tr");
                html.AppendOpeningTag("td", "lane");
            }
            return;
        }

        /// <summary>
        /// Appends the start/end of a parallel activity to HTML
        /// response.
        /// </summary>
        /// <param name="html">HTML response</param>
        private static void AppendStartOrEndOfParallelActivityTo(HtmlWriter html) {
            html.AppendOpeningTag("div", "parallel");
            html.AppendClosingTag("div");
            return;
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            base.CreateChildControls(httpRequest);
            this.inlineWebButtons = new Dictionary<Guid, List<Control>>();
            var currentUser = this.workflow.ParentPersistentContainer.ParentPersistenceMechanism.UserDirectory.CurrentUser;
            var formAdapter = new FormAdapter(this.associatedObject);
            foreach (var activeWorkflowStep in this.workflow.FindActiveWorkflowSteps()) {
                if (!(activeWorkflowStep is ParallelismAction)) { // button events would be handled more than once otherwise
                    foreach (var additionalViewFormButton in activeWorkflowStep.GetAdditionalViewFormButtons<T>(this.associatedObject, this.dataProvider)) {
                        if (additionalViewFormButton.AllowedGroupsForReading.ContainsPermissionsFor(currentUser) && additionalViewFormButton is IWorkflowDiagramCapableButton workflowDiagramCapableButton && workflowDiagramCapableButton.IsVisibleOnWorkflowDiagramPages) {
                            Control webButton;
                            if (additionalViewFormButton is ClientSideButton clientSideButton) {
                                webButton = new ClientSideWebButton(clientSideButton, null);
                            } else if (additionalViewFormButton is ServerSideButton serverSideButton) {
                                serverSideButton.RedirectionTarget = string.Empty;
                                webButton = new ServerSideWebButton<T>(serverSideButton, formAdapter);
                            } else {
                                throw new NotSupportedException("Buttons of type " + additionalViewFormButton.GetType() + " are not supported to be displayed on workflow diagrams.");
                            }
                            if (!this.inlineWebButtons.ContainsKey(activeWorkflowStep.Id)) {
                                this.inlineWebButtons[activeWorkflowStep.Id] = new List<Control>();
                            }
                            this.inlineWebButtons[activeWorkflowStep.Id].Add(webButton);
                        }
                    }
                }
            }
            foreach (var undoButton in this.workflow.GetUndoButtons()) {
                if (undoButton.IsVisibleOnWorkflowDiagramPages && undoButton.AllowedGroupsForReading.ContainsPermissionsFor(currentUser)) {
                    var webButton = new ServerSideWebButton<T>(undoButton, formAdapter);
                    undoButton.RedirectionTarget = string.Empty;
                    if (!this.inlineWebButtons.ContainsKey(undoButton.IdOfWorkflowStepToBeUndone)) {
                        this.inlineWebButtons[undoButton.IdOfWorkflowStepToBeUndone] = new List<Control>();
                    }
                    this.inlineWebButtons[undoButton.IdOfWorkflowStepToBeUndone].Add(webButton);
                }
            }
            return;
        }

        /// <summary>
        /// Handles all server-side postback events. This is called
        /// after CreateChildControls(), but prior to Render().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="httpResponse">HTTP response for client</param>
        public override void HandleEvents(HttpRequest httpRequest, HttpResponse httpResponse) {
            base.HandleEvents(httpRequest, httpResponse);
            if ("POST" == httpRequest.HttpMethod.ToUpperInvariant()) {
                foreach (var keyValuePair in this.inlineWebButtons) {
                    foreach (var inlineWebButton in keyValuePair.Value) {
                        inlineWebButton.HandleEvents(httpRequest, httpResponse);
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Appends an action step to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="actionStep">action step to be added</param>
        /// <param name="renderedWorkflowSteps">list of workflow
        /// steps already rendered</param>
        /// <param name="lastStep">successor of last step to render</param>
        /// <param name="workflowStepSequence">parent workflow step
        /// sequence</param>
        private void RenderActionStep(HtmlWriter html, WorkflowStep actionStep, IList<WorkflowStep> renderedWorkflowSteps, WorkflowStep lastStep, WorkflowStepSequence workflowStepSequence) {
            if (actionStep.IsVisible) {
                string title = actionStep.GetTitle(this.associatedObject);
                string iconUrl = actionStep.GetIconUrl(this.associatedObject);
                var averageDuration = actionStep.GetAverageDuration();
                var matchingHistoryItems = new List<HistoryItem>();
                foreach (var historyItem in workflowStepSequence.History) {
                    if (historyItem.WorkflowStep?.Id == actionStep.Id) {
                        matchingHistoryItems.Add(historyItem);
                    }
                }
                var workflowStepStatus = this.workflow.GetStatusOf(actionStep);
                if (WorkflowStepStatus.Active == workflowStepStatus) {
                    this.AppendStepTo(html, actionStep.Id, title, iconUrl, averageDuration, new string[] { "active", "action", "step" }, matchingHistoryItems);
                } else if (WorkflowStepStatus.Completed == workflowStepStatus) {
                    this.AppendStepTo(html, actionStep.Id, title, iconUrl, averageDuration, new string[] { "completed", "action", "step" }, matchingHistoryItems);
                } else {
                    this.AppendStepTo(html, actionStep.Id, title, iconUrl, averageDuration, new string[] { "action", "step" }, matchingHistoryItems);
                }
            }
            var possibleNextSteps = new List<WorkflowStep>(actionStep.GetPossibleNextSteps());
            WorkflowStep nextStep;
            if (possibleNextSteps.Count < 1) {
                nextStep = null;
            } else {
                nextStep = possibleNextSteps[0];
            }
            this.RenderStep(html, nextStep, renderedWorkflowSteps, lastStep, workflowStepSequence);
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendOpeningTag("table");
            html.AppendOpeningTag("tbody");
            html.AppendOpeningTag("tr");
            html.AppendOpeningTag("td", "lane");
            this.RenderStep(html, this.workflow.FirstStep, new List<WorkflowStep>(), null, this.workflow);
            if (this.workflow.IsCompleted) {
                this.AppendStepTo(html, Guid.Empty, Resources.End, null, null, new string[] { "completed", "action", "step" }, new HistoryItem[0]);
            } else {
                this.AppendStepTo(html, Guid.Empty, Resources.End, null, null, new string[] { "action", "step" }, new HistoryItem[0]);
            }
            html.AppendClosingTag("td");
            html.AppendClosingTag("tr");
            html.AppendClosingTag("tbody");
            html.AppendClosingTag("table");
            return;
        }

        /// <summary>
        /// Appends a choice step to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="choiceStep">choice step to be added</param>
        /// <param name="renderedWorkflowSteps">list of workflow
        /// steps already rendered</param>
        /// <param name="lastStep">successor of last step to render</param>
        /// <param name="workflowStepSequence">parent workflow step
        /// sequence</param>
        private void RenderChoiceStep(HtmlWriter html, WorkflowStep choiceStep, IList<WorkflowStep> renderedWorkflowSteps, WorkflowStep lastStep, WorkflowStepSequence workflowStepSequence) {
            var possibleNextSteps = new List<WorkflowStep>(choiceStep.GetPossibleNextSteps());
            var leftStep = possibleNextSteps[0];
            var rightStep = possibleNextSteps[1];
            var closestCommonStep = new KeyValuePair<WorkflowStep, long>(null, long.MaxValue);
            if (null != leftStep) {
                closestCommonStep = leftStep.FindClosestCommonNextStepWith(rightStep);
            }
            if (null == closestCommonStep.Key) {
                var closestCommonLeftStep = new KeyValuePair<WorkflowStep, long>(null, long.MaxValue);
                if (null != leftStep) {
                    closestCommonLeftStep = leftStep.FindClosestCommonNextStepWith(lastStep);
                }
                var closestCommonRightStep = new KeyValuePair<WorkflowStep, long>(null, long.MaxValue);
                if (null != rightStep) {
                    closestCommonRightStep = rightStep.FindClosestCommonNextStepWith(lastStep);
                }
                if (closestCommonLeftStep.Value < closestCommonRightStep.Value) {
                    closestCommonStep = closestCommonLeftStep;
                } else {
                    closestCommonStep = closestCommonRightStep;
                }
            }
            if (choiceStep.IsVisible) {
                string title = choiceStep.GetTitle(this.associatedObject);
                string iconUrl = choiceStep.GetIconUrl(this.associatedObject);
                var averageDuration = choiceStep.GetAverageDuration();
                var matchingHistoryItems = new List<HistoryItem>();
                foreach (var historyItem in workflowStepSequence.History) {
                    if (historyItem.WorkflowStep?.Id == choiceStep.Id) {
                        matchingHistoryItems.Add(historyItem);
                    }
                }
                var workflowStepStatus = this.workflow.GetStatusOf(choiceStep);
                if (WorkflowStepStatus.Active == workflowStepStatus) {
                    this.AppendStepTo(html, choiceStep.Id, title, iconUrl, averageDuration, new string[] { "active", "choice", "step" }, matchingHistoryItems);
                } else if (WorkflowStepStatus.Completed == workflowStepStatus) {
                    this.AppendStepTo(html, choiceStep.Id, title, iconUrl, averageDuration, new string[] { "completed", "choice", "step" }, matchingHistoryItems);
                } else {
                    this.AppendStepTo(html, choiceStep.Id, title, iconUrl, averageDuration, new string[] { "choice", "step" }, matchingHistoryItems);
                }
                html.AppendOpeningTag("div", "hash");
                html.AppendClosingTag("div");
                WorkflowDiagram<T>.AppendStartOfLanesTo(html, 2);
                this.RenderStep(html, leftStep, renderedWorkflowSteps, closestCommonStep.Key, workflowStepSequence);
                WorkflowDiagram<T>.AppendLaneChangeTo(html);
                this.RenderStep(html, rightStep, renderedWorkflowSteps, closestCommonStep.Key, workflowStepSequence);
                WorkflowDiagram<T>.AppendEndOfLanesTo(html, 2);
            }
            this.RenderStep(html, closestCommonStep.Key, renderedWorkflowSteps, lastStep, workflowStepSequence);
            return;
        }

        /// <summary>
        /// Appends a parallelism action step to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="parallelismAction">parallelism action step
        /// to be added</param>
        /// <param name="renderedWorkflowSteps">list of workflow
        /// steps already rendered</param>
        /// <param name="lastStep">successor of last step to render</param>
        /// <param name="workflowStepSequence">parent workflow step
        /// sequence</param>
        private void RenderParallelismActionStep(HtmlWriter html, ParallelismAction parallelismAction, IList<WorkflowStep> renderedWorkflowSteps, WorkflowStep lastStep, WorkflowStepSequence workflowStepSequence) {
            WorkflowDiagram<T>.AppendStartOrEndOfParallelActivityTo(html);
            WorkflowDiagram<T>.AppendStartOfLanesTo(html, parallelismAction.ParallelWorkflowStepSequences.Count);
            bool isFirstLane = true;
            foreach (var parallelWorkflowStepSequence in parallelismAction.ParallelWorkflowStepSequences) {
                if (isFirstLane) {
                    isFirstLane = false;
                } else {
                    WorkflowDiagram<T>.AppendLaneChangeTo(html);
                }
                this.RenderStep(html, parallelWorkflowStepSequence.FirstStep.NextStep, new List<WorkflowStep>(), null, parallelWorkflowStepSequence);
            }
            WorkflowDiagram<T>.AppendEndOfLanesTo(html, parallelismAction.ParallelWorkflowStepSequences.Count);
            WorkflowDiagram<T>.AppendStartOrEndOfParallelActivityTo(html);
            var possibleNextSteps = new List<WorkflowStep>(parallelismAction.GetPossibleNextSteps());
            if (possibleNextSteps.Count > 1) {
                throw new WorkflowException("Parallelism action steps with more than one possible next step are not supported.");
            }
            this.RenderStep(html, parallelismAction.NextStep, renderedWorkflowSteps, lastStep, workflowStepSequence);
            return;
        }

        /// <summary>
        /// Appends a workflow step to HTML response.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="workflowStep">workflow step to be added</param>
        /// <param name="renderedWorkflowSteps">list of workflow
        /// steps already rendered</param>
        /// <param name="lastStep">successor of last step to render</param>
        /// <param name="workflowStepSequence">parent workflow step
        /// sequence</param>
        private void RenderStep(HtmlWriter html, WorkflowStep workflowStep, IList<WorkflowStep> renderedWorkflowSteps, WorkflowStep lastStep, WorkflowStepSequence workflowStepSequence) {
            if (lastStep != workflowStep) {
                if (null == workflowStep) {
                    WorkflowDiagram<T>.AppendIntermediateEndTo(html, Resources.End);
                } else {
                    if (renderedWorkflowSteps.Contains(workflowStep)) {
                        WorkflowDiagram<T>.AppendIntermediateEndTo(html, Resources.GoTo + " \"" + workflowStep.GetTitle(this.associatedObject) + "\"");
                    } else {
                        renderedWorkflowSteps.Add(workflowStep);
                        var parallelismAction = workflowStep as ParallelismAction;
                        if (null != parallelismAction) {
                            this.RenderParallelismActionStep(html, parallelismAction, renderedWorkflowSteps, lastStep, workflowStepSequence);
                        } else {
                            var possibleNextSteps = new List<WorkflowStep>(workflowStep.GetPossibleNextSteps());
                            if (possibleNextSteps.Count > 2) {
                                throw new WorkflowException("Workflow steps with more than two possible next steps are not supported yet. Please use multiple steps with two possible next steps instead.");
                            } else if (possibleNextSteps.Count > 1) {
                                this.RenderChoiceStep(html, workflowStep, renderedWorkflowSteps, lastStep, workflowStepSequence);
                            } else {
                                this.RenderActionStep(html, workflowStep, renderedWorkflowSteps, lastStep, workflowStepSequence);
                            }
                        }
                    }
                }
            }
            return;
        }

    }

}