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

    using Framework.Presentation;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Adapter for presentable objects as form.
    /// </summary>
    public sealed class FormAdapter : IForm {

        /// <summary>
        /// Custom error message for current instance of form.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// True if all postback values of form are valid, false
        /// otherwise.
        /// </summary>
        public bool? HasValidValue {
            get { return null; }
        }

        /// <summary>
        /// Presentable object to view/edit.
        /// </summary>
        public IPresentableObject PresentableObject { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// view/edit</param>
        public FormAdapter(IPresentableObject presentableObject)
            : base() {
            this.PresentableObject = presentableObject;
        }

    }

}