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

    using Framework.Presentation.Converters;
    using Framework.Presentation.Forms;
    using Properties;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Field control for multiline rich text.
    /// </summary>
    public class WebFieldForMultilineRichText : WebFieldForElement {

        /// <summary>
        /// Rich-text-editor settings.
        /// </summary>
        public IRichTextEditorSettings RichTextEditorSettings { get; private set; }

        /// <summary>
        /// View field to build control for.
        /// </summary>
        public ViewFieldForMultilineRichText ViewField { get; private set; }

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
        /// <param name="richTextEditorSettings">rich-text-editor
        /// settings</param>
        public WebFieldForMultilineRichText(IPresentableFieldForElement presentableField, ViewFieldForMultilineRichText viewField, FieldRenderMode renderMode, IPresentableObject topmostParentPresentableObject, IOptionDataProvider optionDataProvider, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, IRichTextEditorSettings richTextEditorSettings)
            : base(presentableField, viewField, renderMode, topmostParentPresentableObject, optionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) {
            this.RichTextEditorSettings = richTextEditorSettings;
            this.ViewField = viewField;
        }

        /// <summary>
        /// Adds a rich-text-editor data button if required.
        /// </summary>
        /// <param name="dataButtonsWriter">data button writer to
        /// add button to</param>
        /// <param name="isButtonRequired">true if button is
        /// required, false otherwise</param>
        /// <param name="title">title of button</param>
        /// <param name="command">command of button</param>
        /// <param name="iconUrl">URL of icon of button, this may be
        /// null or empty</param>
        private static void AddDataButtonIfRequired(JsonBuilder dataButtonsWriter, bool isButtonRequired, string title, string command, string iconUrl) {
            if (isButtonRequired) {
                if (dataButtonsWriter.Length > 1) {
                    dataButtonsWriter.AppendSeparator();
                }
                dataButtonsWriter.AppendArrayStart();
                dataButtonsWriter.AppendValue(title, true);
                dataButtonsWriter.AppendSeparator();
                dataButtonsWriter.AppendValue(command, true);
                if (!string.IsNullOrEmpty(iconUrl)) {
                    dataButtonsWriter.AppendSeparator();
                    dataButtonsWriter.AppendValue(iconUrl, true);
                }
                dataButtonsWriter.AppendArrayEnd();
            }
            return;
        }

        /// <summary>
        /// Cleans a post-back value.
        /// </summary>
        /// <param name="value">post-back value to clean</param>
        /// <param name="presentableObject">presentable object to
        /// clean value for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// options</param>
        /// <returns>cleaned post-back value</returns>
        protected override string CleanPostBackValue(string value, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            string cleanedValue = value;
            cleanedValue = this.RemoveForbiddenHtmlAttributes(cleanedValue);
            cleanedValue = WebFieldForMultilineRichText.RemoveDuplicatedLineBreaks(cleanedValue);
            cleanedValue = cleanedValue.Replace("<div>", "<br />"); // to keep line breaks in Chrome and Edge
            cleanedValue = this.RemoveForbiddenHtmlTags(cleanedValue);
            cleanedValue = cleanedValue.Replace("\r", " ").Replace("\n", " ");
            cleanedValue = XmlUtility.RemoveEmptyTags(cleanedValue, new string[] { "td" });
            cleanedValue = WebFieldForMultilineRichText.TrimLineBreaks(cleanedValue);
            return base.CleanPostBackValue(cleanedValue, presentableObject, optionDataProvider);
        }

        /// <summary>
        /// Gets an enumerable of all allowed HTML tags.
        /// </summary>
        /// <returns>enumerable of all allowed HTML tags</returns>
        private IEnumerable<string> GetAllowedHtmlTags() {
            if (AutomaticHyperlinkDetection.IsDisabled == this.ViewField.AutomaticHyperlinkDetection) {
                yield return "a";
            }
            if (this.ViewField.HasBoldButton) {
                yield return "b";
            }
            if (this.ViewField.HasIndentButtons) {
                yield return "blockquote";
            }
            yield return "br";
            if (this.ViewField.HasFontSizeButton || this.ViewField.HasTextColorButton) {
                yield return "font";
            }
            if (this.ViewField.HasAlignmentButtons) {
                yield return "div";
            }
            if (this.ViewField.HasItalicButton) {
                yield return "i";
            }
            if (this.ViewField.HasImageButton) {
                yield return "img";
            }
            if (this.ViewField.HasBulletsButton || this.ViewField.HasNumberingButton) {
                yield return "li";
            }
            if (this.ViewField.HasNumberingButton) {
                yield return "ol";
            }
            if (this.ViewField.HasStrikethroughButton) {
                yield return "strike";
            }
            if (this.ViewField.HasSubscriptButton) {
                yield return "sub";
            }
            if (this.ViewField.HasSuperscriptButton) {
                yield return "sup";
            }
            if (this.ViewField.HasTableButtons) {
                yield return "table";
                yield return "td";
                yield return "tr";
            }
            if (this.ViewField.HasUnderlineButton) {
                yield return "u";
            }

            if (this.ViewField.HasBulletsButton) {
                yield return "ul";
            }
        }

        /// <summary>
        /// Removes all duplicated line breaks in HTML value.
        /// </summary>
        /// <param name="value">HTML value to remove duplicated line
        /// breaks from</param>
        /// <returns>HTML value without duplicated line breaks</returns>
        private static string RemoveDuplicatedLineBreaks(string value) {
            string cleanedValue = value.Replace("<p><br></p>", "<br />");
            cleanedValue = cleanedValue.Replace("<p><font><br></font></p>", "<br />");
            return cleanedValue;
        }

        /// <summary>
        /// Removes all HTML tags and attributes that are not allowed.
        /// </summary>
        /// <param name="value">HTML value to remove forbidden tags
        /// and attributes from</param>
        /// <returns>HTML value without forbidden tags and attributes</returns>
        private string RemoveForbiddenHtmlAttributes(string value) {
            var allowedHtmlTags = this.GetAllowedHtmlTags();
            string cleanedValue = XmlUtility.RemoveAttributes(value, "em");
            cleanedValue = XmlUtility.RemoveAttributes(cleanedValue, "p", new string[] { "align" });
            cleanedValue = XmlUtility.RemoveAttributes(cleanedValue, "strong");
            foreach (string allowedHtmlTag in allowedHtmlTags) {
                if ("a" == allowedHtmlTag) {
                    cleanedValue = XmlUtility.RemoveAttributes(cleanedValue, allowedHtmlTag, new string[] { "href" });
                } else if ("div" == allowedHtmlTag) {
                    cleanedValue = XmlUtility.RemoveAttributes(cleanedValue, allowedHtmlTag, new string[] { "align" });
                } else if ("font" == allowedHtmlTag) {
                    var allowedHtmlAttributes = new List<string>(2);
                    if (this.ViewField.HasTextColorButton) {
                        allowedHtmlAttributes.Add("color");
                    }
                    if (this.ViewField.HasFontSizeButton) {
                        allowedHtmlAttributes.Add("size");
                    }
                    cleanedValue = XmlUtility.RemoveAttributes(cleanedValue, allowedHtmlTag, allowedHtmlAttributes);
                } else if ("img" == allowedHtmlTag) {
                    cleanedValue = XmlUtility.RemoveAttributes(cleanedValue, allowedHtmlTag, new string[] { "height", "src", "style", "width" });
                } else if ("td" == allowedHtmlTag) {
                    cleanedValue = XmlUtility.RemoveAttributes(cleanedValue, allowedHtmlTag, new string[] { "align", "colspan", "rowspan" });
                } else {
                    cleanedValue = XmlUtility.RemoveAttributes(cleanedValue, allowedHtmlTag);
                }
            }
            return cleanedValue;
        }

        /// <summary>
        /// Removes all HTML tags that are not allowed.
        /// </summary>
        /// <param name="value">HTML value to remove forbidden tags
        /// from</param>
        /// <returns>HTML value without forbidden tags</returns>
        private string RemoveForbiddenHtmlTags(string value) {
            var allowedHtmlTags = this.GetAllowedHtmlTags();
            string cleanedValue = value;
            if (this.ViewField.HasAlignmentButtons) {
                cleanedValue = System.Text.RegularExpressions.Regex.Replace(cleanedValue, "<p align=([^>]*?)>(.*?)</p>", "<div align=$1>$2</div>");
            }
            cleanedValue = cleanedValue.Replace("<em>", "<i>")
                .Replace("</em>", "</i>")
                .Replace("<strong>", "<b>")
                .Replace("</strong>", "</b>")
                .Replace("</p>", "<br />")
                .Replace("<br/>", "<br />")
                .Replace("<br>", "<br />");
            foreach (string allowedHtmlTag in allowedHtmlTags) {
                cleanedValue = WebFieldForMultilineRichText.ReplaceTagsByPlaceholders(cleanedValue, allowedHtmlTag);
            }
            cleanedValue = Regex.ForXmlTag.Replace(cleanedValue, string.Empty);
            cleanedValue = WebFieldForMultilineRichText.ReplacePlaceholdersByTags(cleanedValue);
            return cleanedValue;
        }

        /// <summary>
        /// Renders a control for editing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderEditableValue(HtmlWriter html) {
            var dataButtonsWriter = new JsonBuilder();
            dataButtonsWriter.AppendArrayStart();
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasBoldButton, Resources.Bold, "bold", this.RichTextEditorSettings.BoldButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasItalicButton, Resources.Italic, "italic", this.RichTextEditorSettings.ItalicButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasUnderlineButton, Resources.Underline, "underline", this.RichTextEditorSettings.UnderlineButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasStrikethroughButton, Resources.Strikethrough, "strikeThrough", this.RichTextEditorSettings.StrikethroughButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasSubscriptButton, Resources.Subscript, "subscript", this.RichTextEditorSettings.SubscriptButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasSuperscriptButton, Resources.Superscript, "superscript", this.RichTextEditorSettings.SuperscriptButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasFontSizeButton, Resources.FontSize, "fontSize", null);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasTextColorButton, Resources.TextColor, "foreColor", null);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasRemoveFormatButton, Resources.RemoveFormat, "removeFormat", this.RichTextEditorSettings.RemoveFormatButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasBulletsButton, Resources.Bullets, "insertUnorderedList", this.RichTextEditorSettings.BulletsButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasNumberingButton, Resources.Numbering, "insertOrderedList", this.RichTextEditorSettings.NumberingButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasIndentButtons, Resources.DecreaseIndent, "outdent", this.RichTextEditorSettings.DecreaseIndentButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasIndentButtons, Resources.IncreaseIndent, "indent", this.RichTextEditorSettings.IncreaseIndentButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasAlignmentButtons, Resources.AlignTextLeft, "justifyLeft", this.RichTextEditorSettings.AlignTextLeftButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasAlignmentButtons, Resources.CenterText, "justifyCenter", this.RichTextEditorSettings.CenterTextButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasAlignmentButtons, Resources.AlignTextRight, "justifyRight", this.RichTextEditorSettings.AlignTextRightButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, AutomaticHyperlinkDetection.IsDisabled == this.ViewField.AutomaticHyperlinkDetection, Resources.InsertLink, "createLink", this.RichTextEditorSettings.InsertLinkButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, AutomaticHyperlinkDetection.IsDisabled == this.ViewField.AutomaticHyperlinkDetection, Resources.RemoveLink, "unlink", this.RichTextEditorSettings.RemoveLinkButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasImageButton, Resources.InsertImage, "insertImage", this.RichTextEditorSettings.InsertImageButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasTableButtons, Resources.InsertTable, "insertTable", this.RichTextEditorSettings.InsertTableButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasTableButtons, Resources.InsertTableColumn, "insertTableColumn", this.RichTextEditorSettings.InsertTableColumnButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasTableButtons, Resources.InsertTableRow, "insertTableRow", this.RichTextEditorSettings.InsertTableRowButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasTableButtons, Resources.RemoveTableColumn, "removeTableColumn", this.RichTextEditorSettings.RemoveTableColumnButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasTableButtons, Resources.RemoveTableRow, "removeTableRow", this.RichTextEditorSettings.RemoveTableRowButtonIconUrl);
            WebFieldForMultilineRichText.AddDataButtonIfRequired(dataButtonsWriter, this.ViewField.HasToggleFullWindowButton, Resources.ToggleFullWindow, "toggleFullWindow", this.RichTextEditorSettings.ToggleFullWindowButtonIconUrl);
            dataButtonsWriter.AppendArrayEnd();
            var attributes = new Dictionary<string, string>(6) {
                { "id", this.ClientFieldId },
                { "class", "rte" },
                { "data-buttons", dataButtonsWriter.ToString().Replace("\"", "&quot;") },
                { "name", this.ClientFieldId }
            };
            if (this.ViewField.IsAutofocused) {
                attributes.Add("autofocus", "autofocus");
            }
            if (Mandatoriness.Required == this.ViewField.Mandatoriness) {
                attributes.Add("required", "required");
            }
            html.AppendOpeningTag("textarea", attributes);
            if (!string.IsNullOrEmpty(this.EditableValue)) {
                html.Append(this.EditableValue);
            }
            html.AppendClosingTag("textarea");
            return;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            if (FieldRenderMode.Form == this.RenderMode && this.IsReadOnly) {
                this.RenderDisplayTitle(html);
                html.AppendOpeningTag("div", "rt");
                this.RenderReadOnlyValue(html);
                var descriptionForViewMode = this.ViewField.DescriptionForViewMode;
                if (!string.IsNullOrEmpty(descriptionForViewMode)) {
                    html.AppendSelfClosingTag("br");
                    html.AppendHtmlEncoded(descriptionForViewMode);
                }
                html.AppendClosingTag("div");
            } else {
                base.RenderChildControls(html);
            }
            return;
        }

        /// <summary>
        /// Renders a read only paragraph showing the value.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderReadOnlyValue(HtmlWriter html) {
            string value = this.GetReadOnlyValue();
            string comparativeValue = this.GetComparativeValue();
            if (null == comparativeValue || comparativeValue == value) {
                if (!string.IsNullOrEmpty(value)) {
                    html.AppendMultilineRichTextUnsafe(value, this.ViewField.AutomaticHyperlinkDetection);
                }
            } else {
                if (!string.IsNullOrEmpty(comparativeValue)) {
                    html.AppendOpeningTag("span", "diffrm");
                    html.AppendMultilineRichTextUnsafe(comparativeValue, this.ViewField.AutomaticHyperlinkDetection, "</span></p><p><span class=\"diffrm\">");
                    html.AppendClosingTag("span");
                }
                if (!string.IsNullOrEmpty(comparativeValue) && !string.IsNullOrEmpty(value)) {
                    html.Append(' ');
                }
                if (!string.IsNullOrEmpty(value)) {
                    html.AppendOpeningTag("span", "diffnew");
                    html.AppendMultilineRichTextUnsafe(value, this.ViewField.AutomaticHyperlinkDetection, "</span></p><p><span class=\"diffnew\">");
                    html.AppendClosingTag("span");
                }
            }
            return;
        }

        /// <summary>
        /// Replaces placeholders by HTML tags.
        /// </summary>
        /// <param name="value">value to replace placeholders for</param>
        /// <returns>value with replaced placeholders</returns>
        private static string ReplacePlaceholdersByTags(string value) {
            return value.Replace(".    .", ">").Replace(".   .", "<");
        }

        /// <summary>
        /// Replaces HTML tags by placeholders.
        /// </summary>
        /// <param name="value">value to replace HTML tags for</param>
        /// <param name="htmlTag">html tag to replace by placeholders</param>
        /// <returns>value with replaced HTML tags</returns>
        private static string ReplaceTagsByPlaceholders(string value, string htmlTag) {
            return System.Text.RegularExpressions.Regex.Replace(value, "<(/?)" + htmlTag + "( [^>]*)?>", ".   .$1" + htmlTag + "$2.    .", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Removes all leading and tailing line breaks in HTML
        /// value.
        /// </summary>
        /// <param name="value">HTML value to remove leading and
        /// tailing line breaks from</param>
        /// <returns>HTML value without leading and tailing line
        /// breaks</returns>
        private static string TrimLineBreaks(string value) {
            string cleanedValue = value;
            while (cleanedValue.StartsWith("<br />")) {
                cleanedValue = cleanedValue.Substring(6);
            }
            while (cleanedValue.EndsWith("<br />")) {
                cleanedValue = cleanedValue.Substring(0, cleanedValue.Length - 6);
            }
            return cleanedValue;
        }

    }

}