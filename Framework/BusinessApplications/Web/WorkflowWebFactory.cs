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

    using Framework.BusinessApplications.Workflows.Forms;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using System;

    /// <summary>
    /// Factory for creating controls to be used in list tables for
    /// workflow controlled providable objects.
    /// </summary>
    public class WorkflowWebFactory : WebFactory {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="renderMode">render mode of fields, e.g. for
        /// forms or for list tables</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// of list tables</param>
        /// <param name="richTextEditorSettings">rich-text-editor
        /// settings</param>
        public WorkflowWebFactory(FieldRenderMode renderMode, IOptionDataProvider optionDataProvider, BuildingRule buildingRule, IRichTextEditorSettings richTextEditorSettings)
            : base(renderMode, optionDataProvider, buildingRule, richTextEditorSettings) {
            // nothing to do
        }

        /// <summary>
        /// Registers all default mappings for fields for elements.
        /// </summary>
        protected override void RegisterDefaultMappingsForFieldsForElements() {
            base.RegisterDefaultMappingsForFieldsForElements();
            this.RegisterFieldMapping<ViewFieldForMaterialNumber>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMaterialNumber(presentableFieldForElement, (ViewFieldForMaterialNumber)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForApprovalStatus>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForApprovalStatus(presentableFieldForElement, (ViewFieldForApprovalStatus)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            return;
        }

        /// <summary>
        /// Registers all default mappings for read-only fields.
        /// </summary>
        protected override void RegisterDefaultMappingsForReadOnlyFields() {
            base.RegisterDefaultMappingsForReadOnlyFields();
            this.RegisterFieldMapping<ViewFieldForWorkflowStepTitle>(delegate (ViewField viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForWorkflowStepTitle((ViewFieldForWorkflowStepTitle)viewField, this.RenderMode, topmostParentPresentableObject);
            });
            this.RegisterFieldMapping<ViewFieldForWorkflowStepIcon>(delegate (ViewField viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForWorkflowStepIcon((ViewFieldForWorkflowStepIcon)viewField, this.RenderMode, topmostParentPresentableObject);
            });
            this.RegisterFieldMapping<ViewFieldForStatusOfWorkflowStep>(delegate (ViewField viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForStatusOfWorkflowStep((ViewFieldForStatusOfWorkflowStep)viewField, this.RenderMode, topmostParentPresentableObject);
            });
            return;
        }

    }

}