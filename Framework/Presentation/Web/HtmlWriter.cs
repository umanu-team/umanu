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

    using Framework.Presentation.Forms;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Builder class for HTML output.
    /// </summary>
    public sealed class HtmlWriter : XmlWriter {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpResponse">http response</param>
        public HtmlWriter(HttpResponse httpResponse)
            : base(httpResponse) {
            // nothing to do
        }

        /// <summary>
        /// Appends a hidden HTML input tag at the end.
        /// </summary>
        /// <param name="name">name of hidden input</param>
        /// <param name="value">value of hidden input</param>
        public void AppendHiddenInputTag(string name, string value) {
            var attributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("type", "hidden"),
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("value", value)
            };
            this.AppendSelfClosingTag("input", attributes);
            return;
        }

        /// <summary>
        /// Appends a copy of a multiline plain text value to the end
        /// as HTML encoded string. HTML tags for e-mail addresses,
        /// hyperlinks and date/time values are inserted automatically.
        /// </summary>
        /// <param name="value">multiline text value to append</param>
        /// <param name="automaticHyperlinkDetection">indicates
        /// whether hyperlinks should be detected and embedded in
        /// anchor tags automatically</param>
        /// <param name="newParagraphHtml">HTML markup for new
        /// paragraph, e.g. &quot;&lt;/p&gt;&lt;p&gt;&quot;</param>
        public void AppendMultilinePlainText(string value, AutomaticHyperlinkDetection automaticHyperlinkDetection, string newParagraphHtml) {
            this.Append(HtmlWriter.ConvertMultilinePlainText(value, automaticHyperlinkDetection, newParagraphHtml));
            return;
        }

        /// <summary>
        /// Appends a copy of a multiline plain text value to the end
        /// as HTML encoded string. HTML tags for e-mail addresses,
        /// hyperlinks and date/time values are inserted automatically.
        /// The multiline plain text to be appended must be trimed
        /// and all duplicate whitespaces have to be removed.
        /// </summary>
        /// <param name="value">multiline text value to append</param>
        /// <param name="automaticHyperlinkDetection">indicates
        /// whether hyperlinks should be detected and embedded in
        /// anchor tags automatically</param>
        /// <param name="newParagraphHtml">HTML markup for new
        /// paragraph, e.g. &quot;&lt;/p&gt;&lt;p&gt;&quot;</param>
        public void AppendMultilinePlainTextUnsafe(string value, AutomaticHyperlinkDetection automaticHyperlinkDetection, string newParagraphHtml) {
            this.Append(HtmlWriter.ConvertMultilinePlainTextUnsafe(value, automaticHyperlinkDetection, newParagraphHtml));
            return;
        }

        /// <summary>
        /// Appends a copy of a multiline rich text value to the end
        /// as HTML string. HTML tags for e-mail addresses,
        /// hyperlinks and date/time values are inserted automatically.
        /// </summary>
        /// <param name="value">multiline text value to append</param>
        /// <param name="automaticHyperlinkDetection">indicates
        /// whether hyperlinks should be detected and embedded in
        /// anchor tags automatically</param>
        /// <param name="newParagraphHtml">HTML markup for new
        /// paragraph, e.g. &quot;&lt;/p&gt;&lt;p&gt;&quot;</param>
        public void AppendMultilineRichText(string value, AutomaticHyperlinkDetection automaticHyperlinkDetection, string newParagraphHtml) {
            if (!string.IsNullOrEmpty(value)) {
                string text = value.Replace("\r", string.Empty);
                text = text.Replace("\n", string.Empty);
                text = text.Trim();
                text = Regex.ForMultipleSpaces.Replace(text, " ");
                this.AppendMultilineRichTextUnsafe(text, automaticHyperlinkDetection, newParagraphHtml);
            }
            return;
        }

        /// <summary>
        /// Appends a copy of a multiline rich text value to the end
        /// as HTML string. HTML tags for e-mail addresses,
        /// hyperlinks and date/time values are inserted automatically.
        /// The multiline rich text to be appended must be trimed
        /// and all duplicate whitespaces have to be removed.
        /// </summary>
        /// <param name="value">multiline text value to append</param>
        /// <param name="automaticHyperlinkDetection">indicates
        /// whether hyperlinks should be detected and embedded in
        /// anchor tags automatically</param>
        public void AppendMultilineRichTextUnsafe(string value, AutomaticHyperlinkDetection automaticHyperlinkDetection) {
            this.AppendMultilineRichTextUnsafe(value, automaticHyperlinkDetection, "</p><p>");
            return;
        }

        /// <summary>
        /// Appends a copy of a multiline rich text value to the end
        /// as HTML string. HTML tags for e-mail addresses,
        /// hyperlinks and date/time values are inserted automatically.
        /// The multiline rich text to be appended must be trimed
        /// and all duplicate whitespaces have to be removed.
        /// </summary>
        /// <param name="value">multiline text value to append</param>
        /// <param name="automaticHyperlinkDetection">indicates
        /// whether hyperlinks should be detected and embedded in
        /// anchor tags automatically</param>
        /// <param name="newParagraphHtml">HTML markup for new
        /// paragraph, e.g. &quot;&lt;/p&gt;&lt;p&gt;&quot;</param>
        public void AppendMultilineRichTextUnsafe(string value, AutomaticHyperlinkDetection automaticHyperlinkDetection, string newParagraphHtml) {
            if (!string.IsNullOrEmpty(value)) {
                string text = XmlUtility.ReplaceEmailAddressesInText(value, "<a href=\"mailto:$1\">$1</a>");
                if (AutomaticHyperlinkDetection.IsEnabled == automaticHyperlinkDetection) {
                    text = Regex.ForHyperlinkInText.Replace(text, "<a href=\"$1\" rel=\"noopener\" target=\"_blank\">$1</a>");
                } else {
                    text = Regex.ForHyperlinkTagInText.Replace(text, "<a href=\"$2\" rel=\"noopener\" target=\"_blank\">");
                }
                text = text.Replace("</p><p>", newParagraphHtml);
                this.Append(text);
            }
            return;
        }

        /// <summary>
        /// Appends an opening tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        /// <param name="cssClass">CSS class to set</param>
        public void AppendOpeningTag(string tagName, string cssClass) {
            this.AppendTagWithCssClass(tagName, cssClass, false);
            return;
        }

        /// <summary>
        /// Appends an opening tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        /// <param name="cssClasses">enumerable of CSS classes to set</param>
        public void AppendOpeningTag(string tagName, IEnumerable<string> cssClasses) {
            this.AppendTagWithCssClasses(tagName, cssClasses, false);
            return;
        }

        /// <summary>
        /// Appends a self-closing tag at the end.
        /// </summary>
        /// <param name="tagName">name of self-closing tag</param>
        /// <param name="cssClass">CSS class to set</param>
        public void AppendTag(string tagName, string cssClass) {
            this.AppendTagWithCssClass(tagName, cssClass, true);
            return;
        }

        /// <summary>
        /// Appends a self-closing tag at the end.
        /// </summary>
        /// <param name="tagName">name of self-closing tag</param>
        /// <param name="cssClasses">enumerable of CSS classes to set</param>
        public void AppendTag(string tagName, IEnumerable<string> cssClasses) {
            this.AppendTagWithCssClasses(tagName, cssClasses, true);
            return;
        }

        /// <summary>
        /// Appends a tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        /// <param name="cssClass">CSS class to set</param>
        /// <param name="isSelfClosing">true to close opening tag, so
        /// no closing tag is needed</param>
        private void AppendTagWithCssClass(string tagName, string cssClass, bool isSelfClosing) {
            var attributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("class", cssClass)
            };
            this.AppendTagWithAttributes(tagName, attributes, isSelfClosing);
            return;
        }

        /// <summary>
        /// Appends a tag at the end.
        /// </summary>
        /// <param name="tagName">name of tag</param>
        /// <param name="cssClasses">enumerable of CSS classes to set</param>
        /// <param name="isSelfClosing">true to close opening tag, so
        /// no closing tag is needed</param>
        private void AppendTagWithCssClasses(string tagName, IEnumerable<string> cssClasses, bool isSelfClosing) {
            var attributes = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("class", string.Join(" ", cssClasses))
            };
            this.AppendTagWithAttributes(tagName, attributes, isSelfClosing);
            return;
        }

        /// <summary>
        /// Converts a copy of a multiline plain text value to HTML.
        /// HTML tags for e-mail addresses, hyperlinks and date/time
        /// values are inserted automatically.
        /// </summary>
        /// <param name="value">multiline text value to convert</param>
        /// <param name="automaticHyperlinkDetection">indicates
        /// whether hyperlinks should be detected and embedded in
        /// anchor tags automatically</param>
        /// <param name="newParagraphHtml">HTML markup for new
        /// paragraph, e.g. &quot;&lt;/p&gt;&lt;p&gt;&quot;</param>
        /// <returns>copy of multiline plain text converted to HTML
        /// </returns>
        public static string ConvertMultilinePlainText(string value, AutomaticHyperlinkDetection automaticHyperlinkDetection, string newParagraphHtml) {
            string text;
            if (string.IsNullOrEmpty(value)) {
                text = value;
            } else {
                text = value.Trim();
                text = Regex.ForMultipleSpaces.Replace(text, " ");
                text = HtmlWriter.ConvertMultilinePlainTextUnsafe(text, automaticHyperlinkDetection, newParagraphHtml);
            }
            return text;
        }

        /// <summary>
        /// Converts a copy of a multiline plain text value to HTML.
        /// HTML tags for e-mail addresses, hyperlinks and date/time
        /// values are inserted automatically.
        /// The multiline plain text to be converted must be trimed
        /// and all duplicate whitespaces have to be removed.
        /// </summary>
        /// <param name="value">multiline text value to be converted
        /// </param>
        /// <param name="automaticHyperlinkDetection">indicates
        /// whether hyperlinks should be detected and embedded in
        /// anchor tags automatically</param>
        /// <param name="newParagraphHtml">HTML markup for new
        /// paragraph, e.g. &quot;&lt;/p&gt;&lt;p&gt;&quot;</param>
        /// <returns>copy of multiline plain text converted to HTML
        /// </returns>
        public static string ConvertMultilinePlainTextUnsafe(string value, AutomaticHyperlinkDetection automaticHyperlinkDetection, string newParagraphHtml) {
            string text;
            if (string.IsNullOrEmpty(value)) {
                text = value;
            } else {
                text = XmlUtility.ReplaceEmailAddressesInText(value, HtmlWriter.ReplaceTagsByPlaceholders("<a href=\"mailto:$1\">$1</a>"));
                if (AutomaticHyperlinkDetection.IsEnabled == automaticHyperlinkDetection) {
                    text = Regex.ForHyperlinkInText.Replace(text, HtmlWriter.ReplaceTagsByPlaceholders("<a href=\"$1\" rel=\"noopener\" target=\"_blank\">$1</a>"));
                }
                text = System.Web.HttpUtility.HtmlEncode(text);
                text = HtmlWriter.ReplacePlaceholdersByTags(text);
                text = text.Replace("\r\n", "\n");
                text = text.Replace("\r", "\n");
                text = text.Replace("\n\n", newParagraphHtml);
                text = text.Replace("\n", "<br />");
            }
            return text;
        }

        /// <summary>
        /// Replaces placeholders by HTML tags.
        /// </summary>
        /// <param name="value">value to replace placeholders for</param>
        /// <returns>value with replaced placeholders</returns>
        private static string ReplacePlaceholdersByTags(string value) {
            return value.Replace(".    .", ">").Replace(".   .", "<").Replace(".  .", "\"");
        }

        /// <summary>
        /// Replaces HTML tags by placeholders.
        /// </summary>
        /// <param name="value">value to replace HTML tags for</param>
        /// <returns>value with replaced HTML tags</returns>
        public static string ReplaceTagsByPlaceholders(string value) {
            return value.Replace(">", ".    .").Replace("<", ".   .").Replace("\"", ".  .");
        }

    }

}