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
    /// Represents a persistent user directory with cookie based
    /// authentication. This authentication method is less secure but
    /// may be more convenient than basic authentication.
    /// </summary>
    public sealed class PersistentUserDirectoryWithCookieAuthentication : PersistentUserDirectory {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpContext">HTTP context to use as current
        /// user source - may be null if resolval of current user is
        /// not necessary</param>
        public PersistentUserDirectoryWithCookieAuthentication(HttpContext httpContext)
            : base(httpContext) {
            // nothing to do
        }

        /// <summary>
        /// Gets the credental of cookie in HTTP context.
        /// </summary>
        /// <returns>credental of cookie in HTTP context</returns>
        private NetworkCredential GetCookieCredential() {
            NetworkCredential cookieCredential = null;
            if (null != this.HttpContext) {
                string authorization = this.HttpContext.Request.Cookies.Get("authorization")?.Value;
                if (!string.IsNullOrEmpty(authorization)) {
                    authorization = Encoding.UTF8.GetString(Convert.FromBase64String(authorization));
                    int index = authorization.IndexOf(':');
                    if (index > 0 && authorization.Length > index + 1) {
                        cookieCredential = new NetworkCredential();
                        cookieCredential.UserName = authorization.Substring(0, index);
                        cookieCredential.Password = authorization.Substring(index + 1);
                    }
                }
            }
            return cookieCredential;
        }

        /// <summary>
        /// Gets the log-on info of a user.
        /// </summary>
        /// <param name="userName">user name of user to get log-on
        /// info for</param>
        /// <returns>log-on info of user or null</returns>
        internal LogOnInfo GetLogOnInfo(string userName) {
            LogOnInfo logOnInfo = null;
            var filterCriteria = new FilterCriteria(nameof(PersistentUser.UserName), RelationalOperator.IsEqualTo, userName, FilterTarget.IsOtherTextValue);
            var persistentUser = this.Users.FindOne(filterCriteria, SortCriterionCollection.Empty);
            if (null != persistentUser) {
                PersistentUserDirectory.LogOnInfos.TryGetValue(persistentUser.Id, out logOnInfo);
            }
            return logOnInfo;
        }

        /// <summary>
        /// Refreshes the CurrentUser property by (re)loading the
        /// user from user source.
        /// </summary>
        /// <returns>current user that just logged on</returns>
        public override void LogOn() {
            this.SetCurrentUser(UserDirectory.AnonymousUser); // to avoid infinite loops
            var cookieCredential = this.GetCookieCredential();
            if (null != cookieCredential) {
                var cookieToken = CookieToken.FindOne(cookieCredential.UserName, cookieCredential.Password, this.Users.ParentPersistenceMechanism);
                if (null != cookieToken) {
                    this.SetCurrentUser(cookieToken.AssociatedUser);
                }
            }
            return;
        }

        /// <summary>
        /// Refreshes the CurrentUser property and sets or refreshes
        /// authorization cookie.
        /// </summary>
        /// <param name="httpCredential">credential of user to log on</param>
        /// <param name="httpResponse">http response to set cookie to</param>
        /// <returns>current user that just logged on</returns>
        internal IUser LogOn(NetworkCredential httpCredential, HttpResponse httpResponse) {
            var user = this.LogOn(httpCredential);
            if (UserDirectory.AnonymousUser != user) {
                var cookieToken = CookieToken.Create(user.UserName, this.Users.ParentPersistenceMechanism);
                var cookieValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.UserName + ":" + cookieToken.Identifier));
                var cookie = new HttpCookie("authorization", cookieValue);
                cookie.Expires = cookieToken.ExpirationDate;
                cookie.HttpOnly = true;
#if !DEBUG
                cookie.Secure = true;
#endif
                httpResponse.AppendCookie(cookie);
            }
            return user;
        }

        /// <summary>
        /// Removes the authorization cookie.
        /// </summary>
        /// <param name="httpResponse">http response to unset cookie in</param>
        internal void LogOut(HttpResponse httpResponse) {
            var cookieCredential = this.GetCookieCredential();
            if (null != cookieCredential) {
                var cookieToken = CookieToken.FindOne(cookieCredential.UserName, cookieCredential.Password, this.Users.ParentPersistenceMechanism);
                cookieToken?.RemoveCascadedly();
            }
            var cookie = new HttpCookie("authorization", "expired");
            cookie.Expires = new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc);
            cookie.HttpOnly = true;
#if !DEBUG
                cookie.Secure = true;
#endif
            httpResponse.AppendCookie(cookie);
            return;
        }

    }

}