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

    using Framework.Presentation.Buttons;
    using Framework.Presentation.Web;
    using System.Collections.Generic;

    /// <summary>
    /// Control for button of action bar with client-side on-click
    /// action.
    /// </summary>
    internal class ClientSideWebButton : Control {

        /// <summary>
        /// Action button to display.
        /// </summary>
        public ClientSideButton ActionButton { get; private set; }

        /// <summary>
        /// ID of position of parent widget or null.
        /// </summary>
        public ulong? PositionId { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="actionButton">action button to display</param>
        /// <param name="positionId">ID of position of parent widget
        /// or null</param>
        public ClientSideWebButton(ClientSideButton actionButton, ulong? positionId)
            : base("div") {
            this.ActionButton = actionButton;
            this.PositionId = positionId;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            var onClientClick = this.ActionButton.GetOnClientClick(this.PositionId);
            if (!string.IsNullOrEmpty(onClientClick)) {
                yield return new KeyValuePair<string, string>("onclick", onClientClick);
            }
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            html.AppendHtmlEncoded(this.ActionButton.Title);
            return;
        }

    }

}