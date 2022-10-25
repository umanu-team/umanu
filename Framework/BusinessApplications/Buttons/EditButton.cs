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

namespace Framework.BusinessApplications.Buttons {

    using Framework.BusinessApplications.DataControllers;
    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web.Controllers;
    using System.Collections.Generic;

    /// <summary>
    /// Button of action bar for link to edit forms.
    /// </summary>
    /// <typeparam name="T">type of providable object data controller
    /// is responsible for</typeparam>
    public class EditButton<T> : LinkButton where T : class, IProvidableObject {

        /// <summary>
        /// Data controller to use for form.
        /// </summary>
        public FormDataController<T> FormDataController { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="formDataController">data controller to use
        /// for form</param>
        public EditButton(string title, FormDataController<T> formDataController)
            : base(title, "edit.html") {
            this.FormDataController = formDataController;
        }

        /// <summary>
        /// Gets the child controllers for action button.
        /// </summary>
        /// <param name="element">object to get child controllers for</param>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page</param>
        /// <returns>child controllers for form</returns>
        public override IEnumerable<IHttpController> GetChildControllers(IProvidableObject element, IBusinessApplication businessApplication, string absoluteUrl) {
            foreach (var childController in base.GetChildControllers(element, businessApplication, absoluteUrl)) {
                yield return childController;
            }
            if (null != this.FormDataController) {
                yield return new FormPageController<T>(businessApplication, absoluteUrl, this.FormDataController, "edit.html", FormType.Edit);
            }
        }

    }

}