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

    using System;

    /// <summary>
    /// Control for info article to be used on web pages.
    /// </summary>
    public class InfoArticle : Control {

        /// <summary>
        /// Text to be displayed.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Indicates whether text is supposed to be rendered as
        /// plain text or as rich text.
        /// </summary>
        public TextRenderMode TextRenderMode { get; private set; }

        /// <summary>
        /// Title to be displayed.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Name of HTML tag to surround title with.
        /// </summary>
        public string TitleTagName { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title to be displayed</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="textRenderMode">indicates whether text is
        /// supposed to be rendered as plain text or as rich text</param>
        public InfoArticle(string title, string text, TextRenderMode textRenderMode)
            : base("article") {
            this.Title = title;
            this.Text = text;
            this.TextRenderMode = textRenderMode;
            this.TitleTagName = "h1";
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            if (!string.IsNullOrEmpty(this.Title)) {
                html.AppendOpeningTag(this.TitleTagName);
                html.AppendHtmlEncoded(this.Title);
                html.AppendClosingTag(this.TitleTagName);
            }
            if (!string.IsNullOrEmpty(this.Text)) {
                if (TextRenderMode.PlainText == this.TextRenderMode) {
                    html.AppendOpeningTag("p");
                    html.AppendMultilinePlainText(this.Text, AutomaticHyperlinkDetection.IsEnabled, "</p><p>");
                    html.AppendClosingTag("p");
                } else { // Rich Text
                    html.AppendOpeningTag("div", "rt");
                    if (TextRenderMode.RichTextWithAutomaticHyperlinkDetection == this.TextRenderMode) {
                        html.AppendMultilineRichTextUnsafe(this.Text, AutomaticHyperlinkDetection.IsEnabled);
                    } else if (TextRenderMode.RichTextWithoutAutomaticHyperlinkDetection == this.TextRenderMode) {
                        html.AppendMultilineRichTextUnsafe(this.Text, AutomaticHyperlinkDetection.IsDisabled);
                    } else {
                        throw new NotSupportedException("Text render mode \"" + this.TextRenderMode.ToString() + "\" is not supported.");
                    }
                    html.AppendClosingTag("div");
                }
            }
            return;
        }

    }

}