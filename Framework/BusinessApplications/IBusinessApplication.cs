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

namespace Framework.BusinessApplications {

    using Framework.BusinessApplications.Web.Controllers;
    using Framework.Persistence;
    using Framework.Presentation.Web;
    using Presentation.Forms;
    using System.Collections.Generic;
    using System.Web;

    /// <summary>
    /// Interface of business applications.
    /// </summary>
    public interface IBusinessApplication {

        /// <summary>
        /// URL  of base directory for files.
        /// </summary>
        string FileBaseDirectory { get; }

        /// <summary>
        /// List of indicators for online/offline status.
        /// </summary>
        IList<IOfflineCapable> OnlineStatusIndicators { get; }

        /// <summary>
        /// Settings to apply to web pages.
        /// </summary>
        BusinessPageSettings PageSettings { get; }

        /// <summary>
        /// Persistence mechanism to get data from.
        /// </summary>
        PersistenceMechanism PersistenceMechanism { get; }

        /// <summary>
        /// Primary color of application.
        /// </summary>
        PrimaryColor PrimaryColor { get; }

        /// <summary>
        /// Root url of application.
        /// </summary>
        string RootUrl { get; }

        /// <summary>
        /// Secondary color of application.
        /// </summary>
        SecondaryColor SecondaryColor { get; }

        /// <summary>
        /// Display title of business application.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Absolute URL of web app manifest.
        /// </summary>
        string WebAppManifestUrl { get; }

        /// <summary>
        /// Determines whether current user is authorized to access
        /// this application.
        /// </summary>
        /// <returns>true if current user is authorized to access
        /// this application, false otherwise</returns>
        bool CurrentUserIsAuthorized();

        /// <summary>
        /// Gets the global action buttons to display.
        /// </summary>
        /// <returns>global action buttons to display</returns>
        IEnumerable<Presentation.Buttons.LinkButton> GetGlobalActionButtons();

        /// <summary>
        /// Gets the navigation items to show in menu of business
        /// application.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <returns>navigation items to show in menu of business
        /// application</returns>
        IEnumerable<NavigationItem> GetNavigationItems(HttpRequest httpRequest);

        /// <summary>
        /// Gets the web factory to use.
        /// </summary>
        /// <param name="renderMode">render mode of fields, e.g. for
        /// forms or for list tables</param>
        /// <returns>web factory to use</returns>
        WebFactory GetWebFactory(FieldRenderMode renderMode);

        /// <summary>
        /// Initializes this business application on each access of
        /// root path - this can be used for initialization of
        /// permission groups in database.
        /// </summary>
        void InitializeRoot();

    }

}