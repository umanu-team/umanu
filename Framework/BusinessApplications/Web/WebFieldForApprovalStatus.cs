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

    using Framework.BusinessApplications.Workflows;
    using Framework.BusinessApplications.Workflows.Forms;
    using Framework.Presentation;
    using Framework.Presentation.Web;
    using Framework.Properties;
    using Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Field control for an approval status.
    /// </summary>
    public class WebFieldForApprovalStatus : WebFieldForElement {

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForApprovalStatus ViewField { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// build control for</param>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="renderMode">render mode of field, e.g. for
        /// form or for list table</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to ID of
        /// field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to ID of
        /// field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        public WebFieldForApprovalStatus(IPresentableFieldForElement presentableField, ViewFieldForApprovalStatus viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.ViewField = viewField;
        }

        /// <summary>
        /// Gets a dictionary of available options.
        /// </summary>
        /// <returns>dictionary of available options</returns>
        private static IDictionary<ApprovalStatus, string> GetOptions() {
            var options = new Dictionary<ApprovalStatus, string>(3);
            options.Add(ApprovalStatus.NotInspected, Resources.NotInspected);
            options.Add(ApprovalStatus.Approved, Resources.Approved);
            options.Add(ApprovalStatus.Rejected, Resources.Rejected);
            return options;
        }

        /// <summary>
        /// Renders a control for editing the value(s).
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            var attributes = new Dictionary<string, string>(5);
            attributes.Add("id", this.ClientFieldId);
            attributes.Add("name", this.ClientFieldId);
            attributes.Add("size", "1");
            if (this.ViewField.IsAutofocused) {
                attributes.Add("autofocus", "autofocus");
            }
            if (Mandatoriness.Required == this.ViewField.Mandatoriness) {
                attributes.Add("required", "required");
            }
            html.AppendOpeningTag("select", attributes);
            if (Mandatoriness.Required != this.ViewField.Mandatoriness || string.IsNullOrEmpty(this.EditableValue)) {
                html.AppendOpeningTag("option");
                html.AppendClosingTag("option");
            }
            var options = WebFieldForApprovalStatus.GetOptions();
            foreach (var option in options) {
                var key = ((int?)option.Key)?.ToString(CultureInfo.InvariantCulture);
                attributes = new Dictionary<string, string>(2);
                attributes.Add("value", key);
                if (key == this.EditableValue) {
                    attributes.Add("selected", "selected");
                }
                html.AppendOpeningTag("option", attributes);
                html.AppendHtmlEncoded(option.Value);
                html.AppendClosingTag("option");
            }
            html.AppendClosingTag("select");
            return;
        }

        /// <summary>
        /// Renders a read-only paragraph showing the value(s).
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            var approvalStatus = (ApprovalStatus)this.PresentableField.ValueAsObject;
            if (FieldRenderMode.ListTable == this.RenderMode) {
                var cssClasses = new List<string>(2);
                string value;
                if (ApprovalStatus.NotInspected == approvalStatus) {
                    cssClasses.Add("status");
                    cssClasses.Add("active");
                    value = "⌛";
                } else if (ApprovalStatus.Approved == approvalStatus) {
                    cssClasses.Add("status");
                    cssClasses.Add("completed");
                    value = "✔";
                } else if (ApprovalStatus.Rejected == approvalStatus) {
                    cssClasses.Add("status");
                    cssClasses.Add("rejected");
                    value = "❌";
                } else {
                    throw new WorkflowException("Approval status \"" + approvalStatus + "\" is unknown.");
                }
                html.AppendOpeningTag("span", cssClasses);
                html.AppendHtmlEncoded(value);
                html.AppendClosingTag("span");
            } else {
                var options = WebFieldForApprovalStatus.GetOptions();
                html.AppendHtmlEncoded(options[approvalStatus]);
            }
            return;
        }

    }

}