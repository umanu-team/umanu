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

    using Framework.Persistence.Fields;

    /// <summary>
    /// JSON-RPC error response.
    /// </summary>
    internal sealed class ErrorResponse : Message {

        /// <summary>
        /// Pre-defined error code.
        /// </summary>
        public ErrorCode ErrorCode {
            get {
                return (ErrorCode)this.errorCode.Value;
            }
            private set {
                this.errorCode.Value = (int)value;
                if (ErrorCode.InternalError == value) {
                    this.ErrorMessage = "Internal error";
                } else if (ErrorCode.InvalidParams == value) {
                    this.ErrorMessage = "Invalid params";
                } else if (ErrorCode.InvalidRequest == value) {
                    this.ErrorMessage = "Invalid Request";
                } else if (ErrorCode.MethodNotFound == value) {
                    this.ErrorMessage = "Method not found";
                } else if (ErrorCode.ParseError == value) {
                    this.ErrorMessage = "Internal error";
                } else if (ErrorCode.ServerError == value) {
                    this.ErrorMessage = "Server error";
                }
            }
        }
        private readonly PersistentFieldForInt errorCode =
            new PersistentFieldForInt(ErrorCodeField);

        /// <summary>
        /// Name of persistent field "ErrorCode".
        /// </summary>
        public const string ErrorCodeField = "code";

        /// <summary>
        /// Additional information about the error.
        /// </summary>
        public string ErrorData {
            get { return this.errorData.Value; }
            set { this.errorData.Value = value; }
        }
        private readonly PersistentFieldForString errorData =
            new PersistentFieldForString(ErrorDataField);

        /// <summary>
        /// Name of persistent field "ErrorData".
        /// </summary>
        public const string ErrorDataField = "data";

        /// <summary>
        /// Pre-defined error message.
        /// </summary>
        public string ErrorMessage {
            get { return this.errorMessage.Value; }
            private set { this.errorMessage.Value = value; }
        }
        private readonly PersistentFieldForString errorMessage =
            new PersistentFieldForString(ErrorMessageField);

        /// <summary>
        /// Name of persistent field "ErrorMessage".
        /// </summary>
        public const string ErrorMessageField = "message";

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ErrorResponse()
            : base() {
            this.RegisterPersistentField(this.errorCode);
            this.RegisterPersistentField(this.errorData);
            this.RegisterPersistentField(this.errorMessage);
        }


        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="request">request this response is for</param>
        /// <param name="errorCode">error code</param>
        public ErrorResponse(Request request, ErrorCode errorCode)
            : this() {
            this.ErrorCode = errorCode;
            this.Id = request.Id;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="request">request this response is for</param>
        /// <param name="errorCode">error code</param>
        /// <param name="errorData">additional information about the
        /// error</param>
        public ErrorResponse(Request request, ErrorCode errorCode, string errorData)
            : this(request, errorCode) {
            this.ErrorData = errorData;
        }
    }

}