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

    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System;

    /// <summary>
    /// Button of action bar for action field.
    /// </summary>
    public abstract class ActionFieldButton : ActionFormButton<ProvidableObject> {

        /// <summary>
        /// Key to be used for presentable field.
        /// </summary>
        private const string key = "Key";

        /// <summary>
        /// Presentable field to be used for form action.
        /// </summary>
        private IPresentableField presentableField;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="urlName">name of the url for the action form
        /// </param>
        public ActionFieldButton(string title, string urlName)
            : base(title, urlName) {
            // nothing to do
        }

        /// <summary>
        /// Gets the providable object to be used for form.
        /// </summary>
        /// <returns>providable object to be used for form</returns>
        public sealed override ProvidableObject GetElement() {
            var providableObject = new ProvidableObject();
            this.presentableField = this.GetPresentableField(providableObject, ActionFieldButton.key);
            providableObject.AddPresentableField(this.presentableField);
            providableObject.Title = this.Title;
            return providableObject;
        }

        /// <summary>
        /// Gets the form view for providable object.
        /// </summary>
        /// <param name="element">providable object to get form view
        /// for</param>
        /// <returns>form view for providable object</returns>
        public sealed override FormView GetFormView(ProvidableObject element) {
            var formView = new FormView();
            var viewPane = new ViewPaneForFields(this.Title);
            var viewField = this.GetViewField(ActionFieldButton.key);
            viewPane.ViewFields.Add(viewField);
            formView.ViewPanes.Add(viewPane);
            return formView;
        }

        /// <summary>
        /// Gets the presentable field to be used for form action.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of presentable field</param>
        /// <param name="key">key to be used for presentable field</param>
        /// <returns>presentable field to be used for form action</returns>
        protected abstract IPresentableField GetPresentableField(IProvidableObject parentPresentableObject, string key);

        /// <summary>
        /// Gets the view field to be used for form action.
        /// </summary>
        /// <param name="key">key to be used for presentable field</param>
        /// <returns>view field to be used for form action</returns>
        protected abstract ViewField GetViewField(string key);

        /// <summary>
        /// Processes the entered form data.
        /// </summary>
        /// <param name="fieldValue">entered field data</param>
        public abstract void ProcessFieldData(object fieldValue);

        /// <summary>
        /// Processes the entered form data.
        /// </summary>
        /// <param name="element">providable object containing the
        /// entered form data</param>
        public sealed override void ProcessFormData(ProvidableObject element) {
            if (this.presentableField is IPresentableFieldForElement presentableFieldForElement) {
                this.ProcessFieldData(presentableFieldForElement.ValueAsObject);
            } else if (this.presentableField is IPresentableFieldForCollection presentableFieldForCollection) {
                this.ProcessFieldData(presentableFieldForCollection.GetValuesAsObject());
            } else {
                throw new NotSupportedException("Presentable fields of type \"" + this.presentableField.GetType() + "\" are not supported.");
            }
            return;
        }

    }

}