﻿/*********************************************************************
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

    /// <summary>
    /// Control for info pane to be used on top of web pages.
    /// </summary>
    public class InfoPane : Control {

        /// <summary>
        /// Text to be displayed.
        /// </summary>
        protected string Text { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="text">text to be displayed</param>
        public InfoPane(string text)
            : base("article") {
            this.Text = text;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendOpeningTag("p");
            html.AppendMultilinePlainText(this.Text, AutomaticHyperlinkDetection.IsEnabled, "</p><p>");
            html.AppendClosingTag("p");
            return;
        }

    }

}