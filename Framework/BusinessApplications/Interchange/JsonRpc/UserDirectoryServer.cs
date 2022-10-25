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

    /// <summary>
    /// Server for JSON-RPC remote user directory.
    /// </summary>
    internal sealed class UserDirectoryServer {

        /// <summary>
        /// User directory to serve.
        /// </summary>
        public UserDirectory UserDirectory { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="userDirectory">user directory to serve</param>
        public UserDirectoryServer(UserDirectory userDirectory) {
            this.UserDirectory = userDirectory;
        }

        /// <summary>
        /// Adds a user to the directory setting a given password.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string AddNoCache(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForPersistentUserAndPassword;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                this.UserDirectory.Add(parameters.User, parameters.Password);
                jsonResponse = new Response(request).ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Finds all matching users from this directory.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string Find(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                var result = this.UserDirectory.Find(parameters.FilterCriteria, parameters.SortCriteria, parameters.StartPosition, parameters.MaxResults);
                var users = new ResultForListOfPersistentUser();
                foreach (IUser user in result) {
                    users.Values.Add(new PersistentUser(user));
                }
                var response = new Response(request);
                response.Result = users;
                jsonResponse = response.ToString(2);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Finds all distinct values of a specific property.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string FindDistinctValues(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForFilterCriteriaAndSortCriteriaAndFieldName;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                var result = this.UserDirectory.FindDistinctValues(parameters.FilterCriteria, parameters.SortCriteria, parameters.FieldName);
                var response = new Response(request);
                response.Result = new ResultForListOfString(result);
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Processes a JSON-RPC request.
        /// </summary>
        /// <param name="request">JSON-RPC request to process</param>
        /// <returns>response as JSON-RPC string or null on error</returns>
        public string ProcessRequest(Request request) {
            string jsonResponse;
            if (null == request) {
                jsonResponse = null;
            } else if ("userdirectory.addnocache" == request.Method) {
                jsonResponse = this.AddNoCache(request);
            } else if ("userdirectory.find" == request.Method) {
                jsonResponse = this.Find(request);
            } else if ("userdirectory.finddistinctvalues" == request.Method) {
                jsonResponse = this.FindDistinctValues(request);
            } else if ("userdirectory.removenocache" == request.Method) {
                jsonResponse = this.RemoveNoCache(request);
            } else {
                jsonResponse = new ErrorResponse(request, ErrorCode.MethodNotFound).ToString(0);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Removes a specific user from the directory.
        /// </summary>
        /// <param name="request">JSON-RPC request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        public string RemoveNoCache(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForPersistentUser;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                var result = this.UserDirectory.Remove(parameters.User);
                var response = new Response(request);
                response.Result = new ResultForBool(result);
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

    }

}