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

    using Framework.Presentation.Exceptions;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field control for a single choice.
    /// </summary>
    public class WebFieldForChoice : WebFieldForElement {

        /// <summary>
        /// View field to build control for.
        /// </summary>
        private readonly ViewFieldForChoice viewFieldForChoice;

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
        public WebFieldForChoice(IPresentableFieldForElement presentableField, ViewFieldForChoice viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.viewFieldForChoice = viewField;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            base.CreateChildControls(httpRequest);
            if (FieldRenderMode.Form == this.RenderMode && PostBackState.ValidPostBack == this.PostBackState && !this.IsIncludedInPostBack) {
                this.IsIncludedInPostBack = true;
                if (Mandatoriness.Required == this.viewFieldForChoice.Mandatoriness) {
                    this.ErrorMessage = this.viewFieldForChoice.GetDefaultErrorMessage();
                }
            }
            return;
        }

        /// <summary>
        /// Gets the comparative key of this field if a comparison
        /// date is set.
        /// </summary>
        /// <returns>comparative key of this field if comparison date
        /// is set, null otherwise</returns>
        protected string GetComparativeKey() {
            string comparativeKey;
            var comparativeField = this.PresentableField.GetVersionedField(this.ComparisonDate);
            if (null == comparativeField) {
                comparativeKey = null;
            } else {
                comparativeKey = comparativeField.ValueAsString;
            }
            return comparativeKey;
        }

        /// <summary>
        /// Renders a control for editing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            this.RenderEditableValue(html, this.EditableValue);
            return;
        }

        /// <summary>
        /// Renders a control for editing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="selectedKey">selected key</param>
        protected virtual void RenderEditableValue(HtmlWriter html, string selectedKey) {
            // http://baymard.com/blog/drop-down-usability
            var optionProvider = this.viewFieldForChoice.OptionProvider;
            if (null == optionProvider) {
                throw new ArgumentNullException(nameof(this.viewFieldForChoice.OptionProvider), "Option provider of view field for key \"" + this.viewFieldForChoice.Key + "\" must not be null.");
            }
            var options = optionProvider.GetOptionDictionary(this.ParentPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
            if ((OptionControlType.RadioButtons == this.viewFieldForChoice.OptionControlType || (OptionControlType.Automatic == this.viewFieldForChoice.OptionControlType && options.Count < 7)) && Mandatoriness.Required == this.viewFieldForChoice.Mandatoriness) {
                uint i = 0;
                foreach (var option in options) {
                    if (i > 0) {
                        html.AppendSelfClosingTag("br");
                    }
                    html.AppendOpeningTag("label", "radio");
                    string id = this.ClientFieldId + "-" + i;
                    var radioButtonAttributes = new Dictionary<string, string>(6) {
                        { "id", id },
                        { "type", "radio" },
                        { "name", this.ClientFieldId },
                        { "value", System.Web.HttpUtility.HtmlEncode(option.Key) }
                    };
                    if (0 == i && this.viewFieldForChoice.IsAutofocused) {
                        radioButtonAttributes.Add("autofocus", "autofocus");
                    }
                    if (option.Key == selectedKey) {
                        radioButtonAttributes.Add("checked", "checked");
                    }
                    html.AppendSelfClosingTag("input", radioButtonAttributes);
                    html.AppendHtmlEncoded(option.Value);
                    html.AppendClosingTag("label");
                    i++;
                }
            } else {
                var attributes = new Dictionary<string, string>(5) {
                    { "id", this.ClientFieldId },
                    { "name", this.ClientFieldId },
                    { "size", "1" }
                };
                if (this.viewFieldForChoice.IsAutofocused) {
                    attributes.Add("autofocus", "autofocus");
                }
                if (Mandatoriness.Required == this.viewFieldForChoice.Mandatoriness) {
                    attributes.Add("required", "required");
                }
                html.AppendOpeningTag("select", attributes);
                if (Mandatoriness.Required != this.viewFieldForChoice.Mandatoriness || string.IsNullOrEmpty(selectedKey) || !options.ContainsKey(selectedKey)) {
                    html.Append("<option value=\"\">");
                    html.AppendHtmlEncoded(optionProvider.GetDisplayValueForNull());
                    html.AppendClosingTag("option");
                }
                foreach (var option in options) {
                    attributes = new Dictionary<string, string>(2) {
                        { "value", System.Web.HttpUtility.HtmlEncode(option.Key) }
                    };
                    if (option.Key == selectedKey) {
                        attributes.Add("selected", "selected");
                    }
                    html.AppendOpeningTag("option", attributes);
                    html.AppendHtmlEncoded(option.Value);
                    html.AppendClosingTag("option");
                }
                html.AppendClosingTag("select");
            }
            return;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            string selectedKey = this.PresentableField.ValueAsString;
            bool isDiffNew;
            if (this.ComparisonDate.HasValue) {
                string comparativeKey = this.GetComparativeKey();
                isDiffNew = string.IsNullOrEmpty(comparativeKey) || comparativeKey != selectedKey;
                if (isDiffNew && !string.IsNullOrEmpty(comparativeKey)) {
                    html.AppendOpeningTag("span", "diffrm");
                    this.RenderReadOnlyValue(html, comparativeKey, null);
                    html.AppendClosingTag("span");
                    if (!string.IsNullOrEmpty(selectedKey)) {
                        html.Append(' ');
                    }
                }
            } else {
                isDiffNew = false;
            }
            if (!string.IsNullOrEmpty(selectedKey)) {
                if (isDiffNew) {
                    html.AppendOpeningTag("span", "diffnew");
                }
                var clickableViewField = this.viewFieldForChoice as IClickableViewFieldWithOptionProvider;
                this.RenderReadOnlyValue(html, selectedKey, clickableViewField);
                if (isDiffNew) {
                    html.AppendClosingTag("span");
                }
            }
            return;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="selectedKey">key of value to be rendered</param>
        /// <param name="clickableViewField">clickable view field to
        /// be used for generation of on-click URL or null</param>
        protected void RenderReadOnlyValue(HtmlWriter html, string selectedKey, IClickableViewFieldWithOptionProvider clickableViewField) {
            string href = null;
            if (null != clickableViewField && Guid.TryParse(selectedKey, out Guid selectedId)) {
                href = clickableViewField.OnClickUrlDelegate?.Invoke(selectedId);
                if (!string.IsNullOrEmpty(href)) {
                    html.AppendOpeningTag("a", new KeyValuePair<string, string>[] {
                        new KeyValuePair<string, string>("href", href)
                    });
                }
            }
            var imgAttributes = new Dictionary<string, string> {
                { "src", this.viewFieldForChoice.OptionProvider.GetIconUrlFor(selectedKey, this.TopmostParentPresentableObject) }
            };
            string value = this.viewFieldForChoice.GetReadOnlyValueFor(this.PresentableField, this.TopmostParentPresentableObject, this.OptionDataProvider, selectedKey);
            if (!string.IsNullOrEmpty(value)) {
                value = System.Web.HttpUtility.HtmlEncode(value);
            }
            var optionDisplayStyle = this.viewFieldForChoice.OptionDisplayStyle;
            if (OptionDisplayStyle.TextOnly == optionDisplayStyle || OptionDisplayStyle.TextWithIconFallback == optionDisplayStyle) {
                if (!string.IsNullOrEmpty(value)) {
                    html.Append(value);
                } else if (OptionDisplayStyle.TextWithIconFallback == optionDisplayStyle && !string.IsNullOrEmpty(imgAttributes["src"])) {
                    html.AppendSelfClosingTag("img", imgAttributes);
                }
            } else if (OptionDisplayStyle.IconOnly == optionDisplayStyle || OptionDisplayStyle.IconWithTextFallback == optionDisplayStyle) {
                if (!string.IsNullOrEmpty(imgAttributes["src"])) {
                    if (!string.IsNullOrEmpty(value)) {
                        imgAttributes.Add("title", value);
                    }
                    html.AppendSelfClosingTag("img", imgAttributes);
                } else if (OptionDisplayStyle.IconWithTextFallback == optionDisplayStyle && !string.IsNullOrEmpty(value)) {
                    html.Append(value);
                }
            } else if (OptionDisplayStyle.None != optionDisplayStyle) {
                throw new PresentationException("Option display style \"" + optionDisplayStyle.ToString() + "\" is not supported.");
            }
            if (!string.IsNullOrEmpty(href)) {
                html.AppendClosingTag("a");
            }
            return;
        }

    }

}