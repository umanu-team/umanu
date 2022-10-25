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

namespace Framework.BusinessApplications.Widgets {

    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using Persistence;
    using Persistence.Fields;
    using Presentation.Buttons;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Base class of view widgets.
    /// </summary>
    public class ViewWidget : PersistentObject {

        /// <summary>
        /// Indicates whether widget in supposed to be displayed in
        /// full width.
        /// </summary>
        public bool IsInFullWidth {
            get { return this.isInFullWidth.Value; }
            set { this.isInFullWidth.Value = value; }
        }
        private readonly PersistentFieldForBool isInFullWidth =
            new PersistentFieldForBool(nameof(IsInFullWidth), false);

        /// <summary>
        /// Indicates whether wigdet is supposed to be displayed in
        /// print view only.
        /// </summary>
        public bool IsVisibleInPrintViewOnly {
            get { return this.isVisibleInPrintViewOnly.Value; }
            set { this.isVisibleInPrintViewOnly.Value = value; }
        }
        private readonly PersistentFieldForBool isVisibleInPrintViewOnly =
            new PersistentFieldForBool(nameof(IsVisibleInPrintViewOnly), false);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewWidget()
            : base() {
            this.RegisterPersistentField(this.isInFullWidth);
            this.RegisterPersistentField(this.isVisibleInPrintViewOnly);
        }

        /// <summary>
        /// Gets the buttons to be displayed for widget.
        /// </summary>
        /// <returns>buttons to be displayed for widget</returns>
        public virtual IEnumerable<ActionButton> GetButtons() {
            yield break;
        }

        /// <summary>
        /// Gets the control for view widget.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        /// <returns>control for view widget</returns>
        public Control GetControl(IBusinessApplication businessApplication, ulong positionId) {
            var webWidget = this.GetControlWithoutWidgetCss(businessApplication, positionId);
            if (this.IsInFullWidth) {
                webWidget.CssClasses.Add("fullwidth");
            }
            if (this.IsVisibleInPrintViewOnly) {
                webWidget.CssClasses.Add("print");
            }
            return webWidget;
        }

        /// <summary>
        /// Gets the control for view widget.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        /// <returns>control for view widget</returns>
        public virtual Control GetControlWithoutWidgetCss(IBusinessApplication businessApplication, ulong positionId) {
            throw new NotImplementedException("Method GetControlWithoutWidgetCss() has to be implemented in each derived class of ViewWidget.");
        }

        /// <summary>
        /// Gets the child controllers for widget.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of parent page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        /// <returns>child controllers for view widget</returns>
        public virtual IEnumerable<IHttpController> GetChildControllers(IBusinessApplication businessApplication, string absoluteUrl, ulong? positionId) {
            string positionUrl = ViewWidget.GetPositionUrlFor(absoluteUrl, positionId);
            var buttons = BusinessPageController.FilterButtonsForCurrentUser(this.GetButtons(), businessApplication.PersistenceMechanism.UserDirectory.CurrentUser);
            foreach (var button in buttons) {
                foreach (var childController in button.GetChildControllers(null, businessApplication, positionUrl)) {
                    yield return childController;
                }
            }
        }

        /// <summary>
        /// Gets the position URL for a combination of absolute URL
        /// and position ID.
        /// </summary>
        /// <param name="absoluteUrl">absolute URL of parent page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start and end with a slash</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        /// <returns>position URL for a combination of absolute URL
        /// and position ID</returns>
        public static string GetPositionUrlFor(string absoluteUrl, ulong? positionId) {
            string positionUrl;
            if (positionId.HasValue) {
                positionUrl = absoluteUrl + positionId.Value.ToString(CultureInfo.InvariantCulture) + "/";
            } else {
                positionUrl = absoluteUrl;
            }
            return positionUrl;
        }

    }

}