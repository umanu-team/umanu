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

namespace Framework.BusinessApplications.Interchange.JsonRpc {

    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Filters;
    using System.Collections.Generic;

    /// <summary>
    /// Client for JSON-RPC remote user directory.
    /// </summary>
    public sealed class UserDirectoryClient : UserDirectory {

        /// <summary>
        /// Gets the current user that is logged on.
        /// </summary>
        public override IUser CurrentUser {
            get { return this.currentUser; }
        }
        private IUser currentUser;

        /// <summary>
        /// Indicates whether user directory supports sorting of
        /// values within directory requests.
        /// </summary>
        protected override bool IsCapableToSortWithinRequests {
            get { return true; }
        }

        /// <summary>
        /// Delegate to be called for processing JSON-RPC requests.
        /// </summary>
        internal ProcessJsonRpcRequestDelegate ProcessJsonRpcRequestWithFaultTolerance { get; set; }

        /// <summary>
        /// Type of delegate to be called for processing JSON-RPC
        /// requests.
        /// </summary>
        /// <param name="request">JSON-RPC request</param>
        /// <param name="depth">depth of sub objects to resolve
        /// whereas 0 will not resolve any sub objects</param>
        /// <returns>JSON-RPC response</returns>
        internal delegate Response ProcessJsonRpcRequestDelegate(Request request, uint depth);

        /// <summary>
        /// Instantitates a new instance.
        /// </summary>
        /// <param name="currentUser">current user that is logged on</param>
        internal UserDirectoryClient(IUser currentUser)
            : base() {
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Adds a user to the directory setting a given password.
        /// </summary>
        /// <param name="user">user to add to the directory</param>
        /// <param name="password">password to set for new user</param>
        protected override void AddNoCache(IUser user, string password) {
            var request = new Request("userdirectory.addnocache");
            var persistentUser = (PersistentUser)user;
            request.Parameters = new ParametersForPersistentUserAndPassword(persistentUser, password);
            this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            return;
        }

        /// <summary>
        /// Finds all matching users from this directory.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <param name="sortCriteria">criteria to sort users by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" is the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to return</param>
        /// <param name="propertiesToPreLoad">properties to load
        /// prior to the actual retrieval of users - this can also be
        /// used to load values of additional properties that
        /// would not be loaded at all otherwise (optional parameter)</param>
        /// <returns>all matching users from this directory</returns>
        protected override IEnumerable<IUser> FindNoCache(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults, params string[] propertiesToPreLoad) {
            IEnumerable<IUser> users;
            var request = new Request("userdirectory.find");
            request.Parameters = new ParametersForFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults(filterCriteria, sortCriteria, startPosition, maxResults);
            var response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            if (null == response) {
                users = new IUser[0];
            } else {
                var result = response.Result as ResultForListOfPersistentUser;
                if (null == result) {
                    throw new PersistenceException("JSON-RPC error: Result is not of expected type List<IUser>.");
                }
                users = result.Values;
            }
            return users;
        }

        /// <summary>
        /// Finds all distinct values of a specific property.
        /// </summary>
        /// <param name="filterCriteria">filter criteria to select
        /// users for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result values by</param>
        /// <param name="fieldName">specific property to find
        /// distinct values for</param>
        /// <returns>all distinct values of a specific property</returns>
        public override IEnumerable<string> FindDistinctValues(FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string fieldName) {
            IEnumerable<string> distinctValues;
            var request = new Request("userdirectory.finddistinctvalues");
            request.Parameters = new ParametersForFilterCriteriaAndSortCriteriaAndFieldName(filterCriteria, sortCriteria, fieldName);
            var response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            if (null == response) {
                distinctValues = new string[0];
            } else {
                var result = response.Result as ResultForListOfString;
                if (null == result) {
                    throw new PersistenceException("JSON-RPC error: Result is not of expected type List<string>.");
                }
                distinctValues = result.Values.AsReadOnly();
            }
            return distinctValues;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="user">specific user to remove from directory</param>
        /// <returns>true if user was successfully removed from
        /// directory, false otherwise or if user was not contained
        /// in directory</returns>
        protected override bool RemoveNoCache(IUser user) {
            var request = new Request("userdirectory.removenocache");
            var persistentUser = (PersistentUser)user;
            request.Parameters = new ParametersForPersistentUser(persistentUser);
            var response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            var result = response.Result as ResultForBool;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type bool.");
            }
            return result.Value;
        }

    }

}