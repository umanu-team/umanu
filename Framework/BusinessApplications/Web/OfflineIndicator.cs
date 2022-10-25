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

    using Presentation.Web;
    using Properties;
    using System.Collections.Generic;

    /// <summary>
    ///  Web control for offline status indication.
    /// </summary>
    public sealed class OfflineIndicator : Control {

        /// <summary>
        /// Enumerable of indicators for online/offline status.
        /// </summary>
        private IEnumerable<IOfflineCapable> onlineStatusIndicators;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="onlineStatusIndicators">enumerable of
        /// indicators for online/offline status</param>
        public OfflineIndicator(IEnumerable<IOfflineCapable> onlineStatusIndicators)
            : base("span") {
            this.onlineStatusIndicators = onlineStatusIndicators;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            yield return new KeyValuePair<string, string>("data-offline", Resources.OfflineMessage);
        }

        /// <summary>
        /// Gets a value indicating whether control is supposed to be
        /// rendered.
        /// </summary>
        /// <returns>true if control is supposed to be rendered,
        /// false otherwise</returns>
        protected override bool GetIsVisible() {
            bool isOnline = true;
            foreach (var onlineStatusIndicator in this.onlineStatusIndicators) {
                if (false == onlineStatusIndicator.IsOnline) {
                    isOnline = false;
                    break;
                }
            }
            return !isOnline;
        }

    }

}