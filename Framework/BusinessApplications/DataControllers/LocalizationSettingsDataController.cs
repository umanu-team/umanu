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

namespace Framework.BusinessApplications.DataControllers {

    using Framework.BusinessApplications.Buttons;
    using Framework.BusinessApplications.DataProviders;
    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Master/Detail data controller for localization settings.
    /// </summary>
    internal class LocalizationSettingsDataController : FormDataController<LocalizationSettings> {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        public LocalizationSettingsDataController(PersistenceMechanism persistenceMechanism)
            : this(new LocalizationSettingsDataProvider(persistenceMechanism)) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="dataProvider">provider of data</param>
        protected LocalizationSettingsDataController(DataProvider<LocalizationSettings> dataProvider)
            : base(dataProvider) {
            // nothing to do
        }

        /// <summary>
        /// Buttons to show on edit form of existing items.
        /// </summary>
        /// <param name="element">object to get buttons for</param>
        public override IEnumerable<ActionButton> GetEditFormButtons(LocalizationSettings element) {
            var saveButton = new SaveButton<LocalizationSettings>(Resources.Save, this.DataProvider);
            saveButton.AllowedGroupsForReading.AddRange(element.AllowedGroups.ForWriting);
            saveButton.RedirectionTarget = "./";
            yield return saveButton;
            var cancelButton = new CancelButton(Resources.Cancel);
            var allUsers = new Group("All users");
            allUsers.Members.Add(UserDirectory.AnonymousUser);
            cancelButton.AllowedGroupsForReading.Add(allUsers);
            cancelButton.RedirectionTarget = null;
            yield return cancelButton;
        }

        /// <summary>
        /// Edit form for existing items.
        /// </summary>
        /// <param name="element">object to get form view
        /// for</param>
        public override FormView GetEditFormView(LocalizationSettings element) {
            var formView = new FormView();
            var languageViewPane = new ViewPaneForFields(Resources.ChangeLanguage);
            var cultureField = new ViewFieldForStringChoice(Resources.PreferredCulture, nameof(LocalizationSettings.CultureName), Mandatoriness.Required) {
                OptionProvider = new CultureOptionProvider(CultureTypes.SpecificCultures)
            };
            languageViewPane.ViewFields.Add(cultureField);
            formView.ViewPanes.Add(languageViewPane);
            var passwordViewPane = new ViewPaneForFields(Resources.ChangePassword);
            var viewFieldForPassword = LoginController.CreateViewFieldForPassword(Resources.Password, nameof(LocalizationSettings.Password), Mandatoriness.Optional);
            viewFieldForPassword.DescriptionForEditMode = Resources.PleaseEnterANewPasswordToChangeYourPasswordOrLeaveThisFieldEmptyToKeepYourPassword;
            viewFieldForPassword.OnClientKeyDown = "javascript:if('13'==((event.which)?event.which:event.keyCode))$('.actionbar > :first').click();";
            passwordViewPane.ViewFields.Add(viewFieldForPassword);
            formView.ViewPanes.Add(passwordViewPane);
            return formView;
        }

        /// <summary>
        /// Gets the display title of a specific hashed type.
        /// </summary>
        /// <param name="hashedType">hashed type to get display title
        /// for</param>
        /// <returns>display title of specific hashed type</returns>
        public override string GetTitleOfNewElement(string hashedType) {
            return null;
        }

        /// <summary>
        /// Gets the actual type of a specific hashed type.
        /// </summary>
        /// <param name="hashedType">hashed type to get actual type
        /// for</param>
        /// <returns>actual type of specific hashed type</returns>
        public override Type GetTypeOfNewElement(string hashedType) {
            return null;
        }

    }

}