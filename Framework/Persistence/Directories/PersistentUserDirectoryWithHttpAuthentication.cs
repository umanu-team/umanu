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

namespace Framework.Persistence.Directories {

    using Framework.Persistence.Filters;
    using System;
    using System.Net;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Represents a persistent user directory with HTTP
    /// authentication. This authentication method is more secure but
    /// may be less convenient than cookie authentication.
    /// </summary>
    public sealed class PersistentUserDirectoryWithHttpAuthentication : PersistentUserDirectory {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpContext">HTTP context to use as current
        /// user source - may be null if resolval of current user is
        /// not necessary</param>
        public PersistentUserDirectoryWithHttpAuthentication(HttpContext httpContext) :
            base(httpContext) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpContext">HTTP context to use as current
        /// user source - may be null if resolval of current user is
        /// not necessary</param>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to use as user store</param>
        public PersistentUserDirectoryWithHttpAuthentication(HttpContext httpContext, PersistenceMechanism persistenceMechanism)
            : this(httpContext) {
            this.PersistenceMechanism = persistenceMechanism;
        }

        /// <summary>
        /// Gets the credental of HTTP context.
        /// </summary>
        /// <returns>credental of HTTP context</returns>
        private NetworkCredential GetHttpCredential() {
            NetworkCredential httpCredential = null;
            if (null != this.HttpContext) {
                string authorization = this.HttpContext.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("basic", StringComparison.InvariantCultureIgnoreCase) && authorization.Length > 5) {
                    authorization = authorization.Substring(5).Trim();
                    authorization = Encoding.UTF8.GetString(Convert.FromBase64String(authorization));
                    int index = authorization.IndexOf(':');
                    if (index > 0 && authorization.Length > index + 1) {
                        httpCredential = new NetworkCredential {
                            UserName = authorization.Substring(0, index),
                            Password = authorization.Substring(index + 1)
                        };
                    }
                }
            }
            return httpCredential;
        }

        /// <summary>
        /// Gets the log-on info of current user.
        /// </summary>
        /// <returns>log-on info of current user or null</returns>
        internal LogOnInfo GetLogOnInfo() {
            LogOnInfo logOnInfo = null;
            var httpCredential = this.GetHttpCredential();
            if (null != httpCredential) {
                var filterCriteria = new FilterCriteria(nameof(PersistentUser.UserName), RelationalOperator.IsEqualTo, httpCredential.UserName, FilterTarget.IsOtherTextValue);
                var persistentUser = this.Users.FindOne(filterCriteria, SortCriterionCollection.Empty);
                if (null != persistentUser) {
                    PersistentUserDirectory.LogOnInfos.TryGetValue(persistentUser.Id, out logOnInfo);
                }
            }
            return logOnInfo;
        }

        /// <summary>
        /// Refreshes the CurrentUser property by (re)loading the
        /// user from user source.
        /// </summary>
        /// <returns>current user that just logged on</returns>
        public override void LogOn() {
            this.LogOn(this.GetHttpCredential());
            return;
        }

    }

}