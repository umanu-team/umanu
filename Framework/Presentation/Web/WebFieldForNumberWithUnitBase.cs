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

namespace Framework.Presentation.Web {

    using Framework.Model.Units;
    using Framework.Presentation.Forms;
    using System;

    /// <summary>
    /// Base class of field controls for combination of number and
    /// unit of measure.
    /// </summary>
    public abstract class WebFieldForNumberWithUnitBase : WebFieldForObject {

        /// <summary>
        /// Editable number of this form field, which is either the
        /// field number or the postback number. Be aware: This
        /// property is supposed to be set during
        /// "CreateChildControls" and will be set to the old field
        /// value in any earlier state.
        /// </summary>
        protected string EditableNumber {
            get {
                string number;
                if (PostBackState.ValidPostBack == this.PostBackState) {
                    number = this.PostBackNumber;
                } else {
                    if (null == this.Value) {
                        number = string.Empty;
                    } else {
                        number = this.Value.NumberAsString;
                    }
                }
                return number;
            }
        }

        /// <summary>
        /// Editable unit of this form field, which is either the
        /// field unit or the postback unit. Be aware: This property
        /// is supposed to be set during "CreateChildControls" and
        /// will be set to the old field value in any earlier state.
        /// </summary>
        protected string EditableUnit {
            get {
                string unit;
                if (PostBackState.ValidPostBack == this.PostBackState) {
                    unit = this.PostBackUnit;
                } else {
                    if (null != this.Value && null != this.Value.Unit) {
                        unit = this.Value.Unit;
                    } else {
                        unit = string.Empty;
                    }
                }
                return unit;
            }
        }

        /// <summary>
        /// Current postback number of this form field. Be aware:
        /// This property is supposed to be set during
        /// "CreateChildControls" and will be set to null in any
        /// earlier state.
        /// </summary>
        public string PostBackNumber { get; protected set; }

        /// <summary>
        /// Current postback unit of this form field. Be aware:
        /// This property is supposed to be set during
        /// "CreateChildControls" and will be set to null in any
        /// earlier state.
        /// </summary>
        public string PostBackUnit { get; protected set; }

        /// <summary>
        /// Value as on form load as string.
        /// </summary>
        protected override string PreviousValue {
            get {
                return this.previousValue;
            }
        }
        private string previousValue;

        /// <summary>
        /// Current value of this field.
        /// </summary>
        public NumberWithAnyUnit Value { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private ViewFieldForElement viewField;

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
        public WebFieldForNumberWithUnitBase(IPresentableFieldForElement presentableField, ViewFieldForElement viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.Value = presentableField.ValueAsObject as NumberWithAnyUnit;
            this.previousValue = this.Value.Number + ' ' + this.Value.Unit;
            this.viewField = viewField;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            this.ErrorMessage = null;
            this.IsIncludedInPostBack = false;
            if (FieldRenderMode.Form == this.RenderMode && PostBackState.ValidPostBack == this.PostBackState) {
                if (this.IsReadOnly) {
                    this.IsIncludedInPostBack = true;
                } else {
                    this.PostBackNumber = httpRequest.Form[this.ClientFieldId];
                    this.PostBackUnit = httpRequest.Form["Unit_" + this.ClientFieldId];
                    if (null != this.PostBackNumber && null != this.PostBackUnit) {
                        this.IsIncludedInPostBack = true;
                        this.PostBackNumber = WebFieldForEditableValue.RemoveUnnecessaryWhiteSpace(this.PostBackNumber);
                        this.PostBackUnit = WebFieldForEditableValue.RemoveUnnecessaryWhiteSpace(this.PostBackUnit);
                        if (this.Value.NumberAsString != this.PostBackNumber || this.Value.Unit != this.PostBackUnit) {
                            var hashedPreviousValue = httpRequest.Form[this.ClientFieldId + "::"];
                            if (WebFieldForElement.GetHashedValueFor(this.PostBackNumber + ' ' + this.PostBackUnit) == hashedPreviousValue) {
                                this.PostBackNumber = this.Value.NumberAsString;
                                this.PostBackUnit = this.Value.Unit;
                            } else {
                                if (!this.Value.TrySetValueAsString(this.PostBackNumber, this.PostBackUnit)) {
                                    this.ErrorMessage = this.viewField.GetDefaultErrorMessage();
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

    }

}