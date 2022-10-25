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
    using Framework.BusinessApplications.Widgets;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Button of action bar for creating new items.
    /// </summary>
    /// <typeparam name="T">type of providable object data controller
    /// is responsible for</typeparam>
    public class NewButton<T> : ClientSideButton where T : class, IProvidableObject {

        /// <summary>
        /// Confirmation message to display on click.
        /// </summary>
        public string ConfirmationMessage { get; set; }

        /// <summary>
        /// Type of element to add button for creating new elements
        /// for - it must be T or be derived from T.
        /// </summary>
        public Type ElementType { get; private set; }

        /// <summary>
        /// Data controller to use for form.
        /// </summary>
        public FormDataController<T> FormDataController { get; private set; }

        /// <summary>
        /// Hashed key of type to create element for on button click.
        /// </summary>
        public string HashedType {
            get {
                return "n" + this.ElementType.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="formDataController">data controller to use
        /// for form</param>
        public NewButton(string title, FormDataController<T> formDataController)
            : this(title, formDataController, typeof(T)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="formDataController">data controller to use
        /// for form</param>
        /// <param name="elementType">type of element to add button
        /// for creating new elements for - it must be of type T or
        /// be derived from type T</param>
        public NewButton(string title, FormDataController<T> formDataController, Type elementType)
            : base(title) {
            this.ElementType = elementType;
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
            yield return new FormPageController<T>(businessApplication, absoluteUrl, this.FormDataController, "edit.html", FormType.New);
        }

        /// <summary>
        /// Gets the client action to execute on click - it may be
        /// null or empty.
        /// </summary>
        /// <param name="positionId">ID of position of parent widget
        /// or null</param>
        /// <returns>client action to execute on click - it may be
        /// null or empty</returns>
        public override string GetOnClientClick(ulong? positionId) {
            string positionUrl = ViewWidget.GetPositionUrlFor("./", positionId);
            string js = "document.location='" + positionUrl + this.HashedType + "/edit.html'";
            if (!string.IsNullOrEmpty(this.ConfirmationMessage)) {
                js = "if(window.confirm('" + System.Web.HttpUtility.HtmlEncode(this.ConfirmationMessage) + "')){" + js + "}";
            }
            return "javascript:" + js;
        }

        /// <summary>
        /// Indicates whether a string represents a hashed type.
        /// </summary>
        /// <param name="s">string to check format of</param>
        /// <returns>true if string represents a hashed type, false
        /// otherwise</returns>
        public static bool IsHashedType(string s) {
            return !string.IsNullOrEmpty(s) && s.StartsWith("n", StringComparison.Ordinal);
        }

    }

}