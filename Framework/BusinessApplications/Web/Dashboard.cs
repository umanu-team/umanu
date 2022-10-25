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

    using Presentation.Web;
    using System.Web;
    using Widgets;

    /// <summary>
    /// Web control for dashboard.
    /// </summary>
    public sealed class Dashboard : CascadedControl {

        /// <summary>
        /// Business application to process.
        /// </summary>
        private IBusinessApplication businessApplication;

        /// <summary>
        /// View of dashboard to be rendered.
        /// </summary>
        private DashboardView dashboardView;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="dashboardView">view of dashboard to be
        /// rendered</param>
        public Dashboard(IBusinessApplication businessApplication, DashboardView dashboardView)
            : base("div") {
            this.businessApplication = businessApplication;
            this.dashboardView = dashboardView;
        }

        /// <summary>
        /// Creates all child controls. This is called prior to
        /// HandleEvents().
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(HttpRequest httpRequest) {
            ulong positionId = 0;
            foreach (var viewWidget in this.dashboardView.ViewWidgets) {
                this.Controls.Add(viewWidget.GetControl(this.businessApplication, positionId));
                positionId++;
            }
            base.CreateChildControls(httpRequest);
            return;
        }

    }

}