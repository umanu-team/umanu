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

    using Framework.BusinessApplications.DataProviders;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Presentation.Forms;
    using Framework.Properties;
    using System.Globalization;

    /// <summary>
    /// Master/Detail data controller for Active Directory
    /// localization settings.
    /// </summary>
    internal sealed class ActiveDirectoryLocalizationSettingsDataController : LocalizationSettingsDataController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to get data from</param>
        public ActiveDirectoryLocalizationSettingsDataController(PersistenceMechanism persistenceMechanism)
            : base(new ActiveDirectoryLocalizationSettingsDataProvider(persistenceMechanism)) {
            // nothing to do
        }

        /// <summary>
        /// Edit form for existing items.
        /// </summary>
        /// <param name="element">object to get form view
        /// for</param>
        public override FormView GetEditFormView(LocalizationSettings element) {
            var formView = new FormView {
                HasModificationInfo = false
            };
            var viewPane = new ViewPaneForFields(Resources.ChangeLanguage);
            var preferredLanguageField = new ViewFieldForStringChoice(Resources.PreferredLanguage, nameof(LocalizationSettings.CultureName), Mandatoriness.Required) {
                OptionProvider = new CultureOptionProvider(CultureTypes.SpecificCultures)
            };
            viewPane.ViewFields.Add(preferredLanguageField);
            var viewFieldForPassword = new ViewFieldForPassword(Resources.Password, nameof(LocalizationSettings.Password), Mandatoriness.Required) {
                DescriptionForEditMode = Resources.PleaseConfirmTheChangeOfYourPreferredLanguageByEnteringYourWindowsPassword,
                OnClientKeyDown = "javascript:if('13'==((event.which)?event.which:event.keyCode))$('.actionbar > :first').click();"
            };
            viewPane.ViewFields.Add(viewFieldForPassword);
            formView.ViewPanes.Add(viewPane);
            return formView;
        }

    }

}