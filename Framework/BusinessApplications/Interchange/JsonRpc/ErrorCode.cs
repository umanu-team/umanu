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

    using System.Runtime.Serialization;

    /// <summary>
    /// Pre-defined error messages for JSON-RPC.
    /// </summary>
    internal enum ErrorCode {

        /// <summary>
        /// Internal JSON-RPC error.
        /// </summary>
        [EnumMemberAttribute]
        InternalError = -32603,

        /// <summary>
        /// Invalid method parameter(s).
        /// </summary>
        [EnumMemberAttribute]
        InvalidParams = -32602,

        /// <summary>
        /// The JSON sent is not a valid Request object.
        /// </summary>
        [EnumMemberAttribute]
        InvalidRequest = -32600,

        /// <summary>
        /// The method does not exist / is not available.
        /// </summary>
        [EnumMemberAttribute]
        MethodNotFound = -32601,

        /// <summary>
        /// Invalid JSON was received by the server. An error
        /// occurred on the server while parsing the JSON text.
        /// </summary>
        [EnumMemberAttribute]
        ParseError = -32700,

        /// <summary>
        /// Server error.
        /// </summary>
        [EnumMemberAttribute]
        ServerError = -32000

    }

}