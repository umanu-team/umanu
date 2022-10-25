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

    using Framework.BusinessApplications.DataControllers;
    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using Presentation;
    using Presentation.Buttons;
    using Presentation.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// View widget for view form.
    /// </summary>
    /// <typeparam name="T">type of providable object</typeparam>
    public class ViewWidgetForForm<T> : ViewWidget where T : class, IProvidableObject {

        /// <summary>
        /// Element to show in form.
        /// </summary>
        public T Element {
            get {
                if (null == this.element) {
                    this.element = this.GetElement();
                }
                return this.element;
            }
        }
        private T element;

        /// <summary>
        /// ID of element to show in form or hashed type in case form
        /// is for new elements.
        /// </summary>
        public string ElementId {
            get {
                return this.elementId;
            }
            set {
                this.elementId = value;
                this.ClearCache();
            }
        }
        private string elementId;

        /// <summary>
        /// Data controller for forms.
        /// </summary>
        public FormDataController<T> FormDataController {
            get {
                return this.formDataController;
            }
            set {
                this.formDataController = value;
                this.ClearCache();
            }
        }
        private FormDataController<T> formDataController;

        /// <summary>
        /// Indicates what kind of form is to be created.
        /// </summary>
        public FormType FormType {
            get {
                return this.formType;
            }
            set {
                this.formType = value;
                this.ClearCache();
            }
        }
        private FormType formType;

        /// <summary>
        /// View to apply to form.
        /// </summary>
        public FormView FormView {
            get {
                if (null == this.formView) {
                    this.formView = this.GetFormView();
                }
                return this.formView;
            }
        }
        private FormView formView;

        /// <summary>
        /// Title of element.
        /// </summary>
        public string Title {
            get {
                if (null == this.title) {
                    this.title = this.GetTitle();
                }
                return this.title;
            }
        }
        private string title;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewWidgetForForm()
            : base() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="elementId">ID of element to show in form or
        /// hashed type in case form is for new elements</param>
        /// <param name="formDataController">data controller to use
        /// for form</param>
        /// <param name="formType">indicates what kind of form is to
        /// be created</param>
        public ViewWidgetForForm(string elementId, FormDataController<T> formDataController, FormType formType)
            : this() {
            this.ElementId = elementId;
            this.FormDataController = formDataController;
            this.FormType = formType;
        }

        /// <summary>
        /// Clears the internal cache.
        /// </summary>
        private void ClearCache() {
            this.element = null;
            this.formView = null;
            this.title = null;
            return;
        }

        /// <summary>
        /// Gets the buttons to be displayed for widget.
        /// </summary>
        /// <returns>buttons to be displayed for widget</returns>
        public override IEnumerable<ActionButton> GetButtons() {
            IEnumerable<ActionButton> buttons;
            if (FormType.New == this.FormType) {
                buttons = this.FormDataController.GetNewFormButtons(this.Element);
            } else if (FormType.Edit == this.FormType) {
                buttons = this.FormDataController.GetEditFormButtons(this.Element);
            } else if (FormType.View == this.FormType) {
                buttons = this.FormDataController.GetViewFormButtons(this.Element);
            } else {
                buttons = null;
            }
            return buttons;
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
        public override IEnumerable<IHttpController> GetChildControllers(IBusinessApplication businessApplication, string absoluteUrl, ulong? positionId) {
            yield return new FileAttachmentController<T>(businessApplication, absoluteUrl, this.FormDataController.DataProvider);
            string positionUrl = ViewWidget.GetPositionUrlFor(absoluteUrl, positionId);
            if (FormType.New != this.FormType) {
                yield return new JsonFormController<T>(businessApplication, positionUrl, this.FormDataController, this.FormType.ToString().ToLowerInvariant() + ".json", this.FormType);
            }
            var buttons = BusinessPageController.FilterButtonsForCurrentUser(this.GetButtons(), businessApplication.PersistenceMechanism.UserDirectory.CurrentUser);
            foreach (var button in buttons) {
                foreach (var childController in button.GetChildControllers(this.Element, businessApplication, positionUrl)) {
                    yield return childController;
                }
            }
            string absoluteElementPageUrl = absoluteUrl + this.ElementId + "/";
            if (FormType.View == this.FormType) {
                ulong viewFormWidgetPositionId = 0;
                var viewFormWidgets = this.FormDataController.GetViewFormWidgets(this.Element);
                foreach (var viewFormWidget in viewFormWidgets) {
                    foreach (var childController in viewFormWidget.GetChildControllers(businessApplication, absoluteElementPageUrl, viewFormWidgetPositionId)) {
                        yield return childController;
                    }
                    viewFormWidgetPositionId++;
                }
            } else {
                yield return new LookupController(absoluteElementPageUrl, this.Element, this.FormView, businessApplication.GetWebFactory(FieldRenderMode.Form).OptionDataProvider);
            }
            var redirectionController = new RedirectionController();
            redirectionController.RedirectionRules.Add(new RedirectionRule(absoluteElementPageUrl, RedirectionRuleType.Equals, absoluteElementPageUrl + "view.html"));
            yield return redirectionController;
        }

        /// <summary>
        /// Gets the control for view widget.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        /// <returns>control for view widget</returns>
        public override Control GetControlWithoutWidgetCss(IBusinessApplication businessApplication, ulong positionId) {
            return new Form(this.Element, this.FormView, businessApplication.FileBaseDirectory, businessApplication.GetWebFactory(Framework.Presentation.Forms.FieldRenderMode.Form));
        }

        /// <summary>
        /// Gets the element to show in form.
        /// </summary>
        /// <returns>element to show in form</returns>
        private T GetElement() {
            T element;
            if (FormType.New == this.FormType) {
                element = this.FormDataController.DataProvider.Create(this.FormDataController.GetTypeOfNewElement(this.ElementId));
            } else if (FormType.Edit == this.FormType || FormType.View == this.FormType) {
                element = this.FormDataController.DataProvider.FindOne(this.ElementId);
            } else {
                element = null;
            }
            return element;
        }

        /// <summary>
        /// Gets the view to apply to form.
        /// </summary>
        /// <returns>view to apply to form</returns>
        private FormView GetFormView() {
            FormView formView;
            if (FormType.New == this.FormType) {
                formView = this.FormDataController.GetNewFormView(this.FormDataController.GetTypeOfNewElement(this.ElementId));
            } else if (FormType.Edit == this.FormType) {
                formView = this.FormDataController.GetEditFormView(this.Element);
            } else if (FormType.View == this.FormType) {
                formView = this.FormDataController.GetViewFormView(this.Element);
            } else {
                formView = null;
            }
            return formView;
        }

        /// <summary>
        /// Gets the title of element.
        /// </summary>
        /// <returns>title of element</returns>
        private string GetTitle() {
            string title;
            if (FormType.New == this.FormType) {
                title = this.FormDataController.GetTitleOfNewElement(this.ElementId);
            } else {
                title = this.Element?.GetTitle();
            }
            return title;
        }

    }

}