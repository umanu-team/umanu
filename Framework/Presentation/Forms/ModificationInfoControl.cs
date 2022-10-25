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

namespace Framework.Presentation.Forms {

    using Framework.Presentation.Web;
    using Model;
    using Properties;

    /// <summary>
    /// Renders a paragraph for created at, created by, modified
    /// at and modified by if applicable.
    /// </summary>
    public sealed class ModificationInfoControl : Control {

        /// <summary>
        /// Presentable object to render modification info for.
        /// </summary>
        public IPresentableObject PresentableObject { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ModificationInfoControl(IPresentableObject presentableObject)
            : base("p") {
            this.PresentableObject = presentableObject;
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            string createdAt = Time.RenderHtmlTag(this.PresentableObject.CreatedAt, DateTimeType.DateAndTime);
            string createdBy;
            if (null == this.PresentableObject.CreatedBy) {
                createdBy = "?";
            } else {
                createdBy = System.Web.HttpUtility.HtmlEncode(this.PresentableObject.CreatedBy.DisplayName);
            }
            string modifiedAt = Time.RenderHtmlTag(this.PresentableObject.ModifiedAt, DateTimeType.DateAndTime);
            string modifiedBy;
            if (null == this.PresentableObject.ModifiedBy) {
                modifiedBy = "?";
            } else {
                modifiedBy = System.Web.HttpUtility.HtmlEncode(this.PresentableObject.ModifiedBy.DisplayName);
            }
            html.Append(string.Format(Resources.CreatedAtByModifiedAtBy, createdAt, createdBy, modifiedAt, modifiedBy));
            return;
        }

    }

}