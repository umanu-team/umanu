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
    /// Field control for multiple choices.
    /// </summary>
    public class WebFieldForMultipleChoices : WebFieldForCollection {

        /// <summary>
        /// View field to build control for.
        /// </summary>
        protected ViewFieldForMultipleChoices ViewFieldForMultipleChoices { get; private set; }

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
        public WebFieldForMultipleChoices(IPresentableFieldForCollection presentableField, ViewFieldForMultipleChoices viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.ViewFieldForMultipleChoices = viewField;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            base.CreateChildControls(httpRequest);
            if (FieldRenderMode.Form == this.RenderMode && PostBackState.ValidPostBack == this.PostBackState && !this.IsIncludedInPostBack) {
                this.IsIncludedInPostBack = true;
                if (Mandatoriness.Required == this.ViewFieldForMultipleChoices.Mandatoriness) {
                    this.ErrorMessage = this.ViewFieldForMultipleChoices.GetDefaultErrorMessage();
                } else {
                    if (this.PresentableField.Count > 0) {
                        this.PresentableField.Clear();
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Gets the comparative keys of this field if a comparison
        /// date is set.
        /// </summary>
        /// <returns>comparative keys of this field if comparison date
        /// is set, null otherwise</returns>
        protected IEnumerable<string> GetComparativeKeys() {
            IEnumerable<string> comparativeKeys;
            var comparativeField = this.PresentableField.GetVersionedField(this.ComparisonDate);
            if (null == comparativeField) {
                comparativeKeys = null;
            } else {
                comparativeKeys = comparativeField.GetValuesAsString();
            }
            return comparativeKeys;
        }

        /// <summary>
        /// Renders a control for editing the values.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            List<string> selectedKeys;
            if (PostBackState.ValidPostBack == this.PostBackState) {
                selectedKeys = this.PostBackValues;
            } else {
                selectedKeys = new List<string>(this.PresentableField.GetValuesAsString());
            }
            this.RenderEditableValue(html, selectedKeys);
            return;
        }

        /// <summary>
        /// Renders a control for editing the values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="selectedKeys">list of selected keys</param>
        protected virtual void RenderEditableValue(HtmlWriter html, IList<string> selectedKeys) {
            var optionProvider = this.ViewFieldForMultipleChoices.OptionProvider;
            if (null == optionProvider) {
                throw new ArgumentNullException(nameof(this.ViewFieldForMultipleChoices.OptionProvider), "Option provider of view field for key \"" + this.ViewFieldForMultipleChoices.Key + "\" must not be null.");
            }
            if (optionProvider is GroupedOptionProvider groupedOptionProvider) {
                bool isFirstOptionProviderGroup = true;
                foreach (var optionProviderGroup in groupedOptionProvider.GetOptionProviders()) {
                    if (null != optionProviderGroup) {
                        this.RenderEditableValueSelectBox(html, selectedKeys, optionProviderGroup, isFirstOptionProviderGroup && this.ViewFieldForMultipleChoices.IsAutofocused);
                        isFirstOptionProviderGroup = false;
                    }
                }
            } else {
                var options = optionProvider.GetOptionDictionary(this.ParentPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
                if (this.ViewFieldForMultipleChoices.Limit < 2) {
                    // http://baymard.com/blog/drop-down-usability
                    if (Mandatoriness.Required == this.ViewFieldForMultipleChoices.Mandatoriness && options.Count < 7) {
                        this.RenderEditableValueBoxes(html, selectedKeys, options, "radio");
                    } else {
                        this.RenderEditableValueSelectBox(html, selectedKeys, optionProvider, this.ViewFieldForMultipleChoices.IsAutofocused);
                    }
                } else {
                    this.RenderEditableValueBoxes(html, selectedKeys, options, "checkbox");
                }
            }
            return;
        }

        /// <summary>
        /// Renders checkboxes for editing values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="selectedKeys">list of selected keys</param>
        /// <param name="options">dictionary of suggested values</param>
        /// <param name="htmlType">HTML type of boxes, either
        /// checkbox or radio</param>
        private void RenderEditableValueBoxes(HtmlWriter html, IList<string> selectedKeys, IDictionary<string, string> options, string htmlType) {
            uint i = 0;
            foreach (var option in options) {
                if (i > 0) {
                    html.AppendSelfClosingTag("br");
                }
                html.AppendOpeningTag("label", htmlType);
                string id = this.ClientFieldId + "-" + i;
                var checkboxAttributes = new Dictionary<string, string>(6) {
                    { "id", id },
                    { "type", htmlType },
                    { "name", this.ClientFieldId },
                    { "value", System.Web.HttpUtility.HtmlEncode(option.Key) }
                };
                if (0 == i && this.ViewFieldForMultipleChoices.IsAutofocused) {
                    checkboxAttributes.Add("autofocus", "autofocus");
                }
                if (selectedKeys.Contains(option.Key) || (this.ViewFieldForMultipleChoices.IsAutoSelectionEnabled && selectedKeys.Count < 1)) {
                    checkboxAttributes.Add("checked", "checked");
                }
                html.AppendSelfClosingTag("input", checkboxAttributes);
                html.AppendHtmlEncoded(option.Value);
                html.AppendClosingTag("label");
                i++;
            }
            return;
        }

        /// <summary>
        /// Renders a select box for editing values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="selectedKeys">list of selected keys</param>
        /// <param name="optionProvider">provider for suggested
        /// values</param>
        /// <param name="isAutofocused">value indicating whether to
        /// focus select box as the first one</param>
        private void RenderEditableValueSelectBox(HtmlWriter html, IList<string> selectedKeys, OptionProvider optionProvider, bool isAutofocused) {
            if (selectedKeys.Count > 1) {
                throw new ArgumentException("Mutliple selections are not allowed for select boxes.", nameof(selectedKeys));
            } else {
                string selectedKey;
                if (selectedKeys.Count < 1) {
                    selectedKey = null;
                } else {
                    selectedKey = selectedKeys[0];
                }
                var options = optionProvider.GetOptionDictionary(this.ParentPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
                var attributes = new Dictionary<string, string>(4) {
                    { "id", this.ClientFieldId },
                    { "name", this.ClientFieldId },
                    { "size", "1" }
                };
                if (isAutofocused) {
                    attributes.Add("autofocus", "autofocus");
                }
                html.AppendOpeningTag("select", attributes);
                if (Mandatoriness.Required != this.ViewFieldForMultipleChoices.Mandatoriness || string.IsNullOrEmpty(selectedKey) || !options.ContainsKey(selectedKey)) {
                    html.Append("<option value=\"\">");
                    html.AppendHtmlEncoded(optionProvider.GetDisplayValueForNull());
                    html.AppendClosingTag("option");
                }
                foreach (var option in options) {
                    attributes = new Dictionary<string, string>(2) {
                        { "value", System.Web.HttpUtility.HtmlEncode(option.Key) }
                    };
                    if (selectedKey == option.Key) {
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
        /// Renders a read only paragraph showing the values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="selectedOption">option to be rendered</param>
        /// <param name="optionProvider">option provider to be used</param>
        /// <param name="clickableViewField">clickable view field to
        /// be used for generation of on-click URL or null</param>
        private void RenderReadOnlyOption(HtmlWriter html, KeyValuePair<string, string> selectedOption, OptionProvider optionProvider, IClickableViewFieldWithOptionProvider clickableViewField) {
            string href = null;
            if (null != clickableViewField && Guid.TryParse(selectedOption.Key, out Guid selectedId)) {
                href = clickableViewField.OnClickUrlDelegate?.Invoke(selectedId);
                if (!string.IsNullOrEmpty(href)) {
                    html.AppendOpeningTag("a", new KeyValuePair<string, string>[] {
                            new KeyValuePair<string, string>("href", href)
                        });
                }
            }
            var imgAttributes = new Dictionary<string, string>(2) {
                    { "src", optionProvider.GetIconUrlFor(selectedOption.Key, this.TopmostParentPresentableObject) }
                };
            string value = System.Web.HttpUtility.HtmlEncode(selectedOption.Value);
            var optionDisplayStyle = this.ViewFieldForMultipleChoices.OptionDisplayStyle;
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

        /// <summary>
        /// Renders a read only paragraph showing the values.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            var selectedKeys = new List<string>(this.PresentableField.GetValuesAsString());
            List<string> comparativeKeys;
            var comparativeKeyEnumerable = this.GetComparativeKeys();
            if (null == comparativeKeyEnumerable) {
                comparativeKeys = null;
            } else {
                comparativeKeys = new List<string>(comparativeKeyEnumerable);
            }
            this.RenderReadOnlyValue(html, selectedKeys, comparativeKeys);
            return;
        }

        /// <summary>
        /// Renders a read only paragraph showing the values.
        /// </summary>
        /// <param name="html">HTML response</param>
        /// <param name="selectedKeys">list of selected keys</param>
        /// <param name="comparativeKeys">list of comparative keys</param>
        protected override void RenderReadOnlyValue(HtmlWriter html, IList<string> selectedKeys, IList<string> comparativeKeys) {
            var optionProvider = this.ViewFieldForMultipleChoices.OptionProvider;
            bool isFirstValue = true;
            if (null != comparativeKeys) {
                var comparativeOptions = optionProvider.FindReadOnlyOptionsForKeys(comparativeKeys, this.ParentPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
                foreach (var comparativeOption in comparativeOptions) {
                    if (!selectedKeys.Contains(comparativeOption.Key)) {
                        if (isFirstValue) {
                            isFirstValue = false;
                        } else {
                            html.Append(this.GetValueSeparator());
                        }
                        html.AppendOpeningTag("span", "diffrm");
                        this.RenderReadOnlyOption(html, comparativeOption, optionProvider, null);
                        html.AppendClosingTag("span");
                    }
                }
            }
            var clickableViewField = this.ViewFieldForMultipleChoices as IClickableViewFieldWithOptionProvider;
            var selectedOptions = optionProvider.FindReadOnlyOptionsForKeys(selectedKeys, this.ParentPresentableObject, this.TopmostParentPresentableObject, this.OptionDataProvider);
            foreach (var selectedOption in selectedOptions) {
                if (isFirstValue) {
                    isFirstValue = false;
                } else {
                    html.Append(this.GetValueSeparator());
                }
                bool isSelectedKeyContainedInComparativeKeys = null == comparativeKeys || comparativeKeys.Contains(selectedOption.Key);
                if (!isSelectedKeyContainedInComparativeKeys) {
                    html.AppendOpeningTag("span", "diffnew");
                }
                this.RenderReadOnlyOption(html, selectedOption, optionProvider, clickableViewField);
                if (!isSelectedKeyContainedInComparativeKeys) {
                    html.AppendClosingTag("span");
                }
            }
            return;
        }

    }

}