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

namespace Framework.Presentation.Web {

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web;

    /// <summary>
    /// Resolves providable objects based on query string.
    /// </summary>
    /// <typeparam name="T">type of providable objects to be resolved</typeparam>
    public abstract class QueryResolver<T> where T : class, IProvidableObject {

        /// <summary>
        /// Collection of names and corresponding values of
        /// parameters of query string.
        /// </summary>
        private readonly NameValueCollection queryString;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpRequest">HTTP request to get parameters
        /// from</param>
        public QueryResolver(HttpRequest httpRequest)
            : base() {
            if (httpRequest.Form.Count > 0) {
                this.queryString = httpRequest.Form;
            } else {
                this.queryString = httpRequest.QueryString;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="url">URL sent by client to get parameters
        /// from</param>
        public QueryResolver(Uri url)
            : base() {
            this.queryString = HttpUtility.ParseQueryString(url.Query);
        }

        /// <summary>
        /// Builds a query string with all relevant parameters.
        /// </summary>
        /// <returns>query string with all relevant parameters</returns>
        public abstract string BuildQueryString();

        /// <summary>
        /// Finds the matching result for query.
        /// </summary>
        /// <returns>matching result for query</returns>
        public abstract QueryResult<T> Execute();

        /// <summary>
        /// Finds the matching subset of providable objects for
        /// query.
        /// </summary>
        /// <returns>matching subset of providable objects for query</returns>
        public abstract ICollection<T> FindProvidableObjects();

        /// <summary>
        /// Gets the value of a boolean parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected bool? GetBoolParameter(string parameterName) {
            bool? boolParameter;
            var stringParameter = this.GetStringParameter(parameterName);
            if (!string.IsNullOrEmpty(stringParameter) && bool.TryParse(stringParameter, out bool parsedStringParameter)) {
                boolParameter = parsedStringParameter;
            } else {
                boolParameter = null;
            }
            return boolParameter;
        }

        /// <summary>
        /// Gets the value of a date/time parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected DateTime? GetDateTimeParameter(string parameterName) {
            DateTime? dateTimeParameter;
            var stringParameter = this.GetStringParameter(parameterName);
            if (!string.IsNullOrEmpty(stringParameter) && DateTime.TryParse(stringParameter, out DateTime parsedStringParameter)) {
                dateTimeParameter = parsedStringParameter;
            } else {
                dateTimeParameter = null;
            }
            return dateTimeParameter;
        }

        /// <summary>
        /// Gets the value of a decimal parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected decimal? GetDecimalParameter(string parameterName) {
            decimal? decimalParameter;
            var stringParameter = this.GetStringParameter(parameterName);
            if (!string.IsNullOrEmpty(stringParameter) && decimal.TryParse(stringParameter, out decimal parsedStringParameter)) {
                decimalParameter = parsedStringParameter;
            } else {
                decimalParameter = null;
            }
            return decimalParameter;
        }

        /// <summary>
        /// Gets the value of a GUID parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected Guid? GetGuidParameter(string parameterName) {
            Guid? guidParameter;
            var stringParameter = this.GetStringParameter(parameterName);
            if (!string.IsNullOrEmpty(stringParameter) && Guid.TryParse(stringParameter, out Guid parsedStringParameter)) {
                guidParameter = parsedStringParameter;
            } else {
                guidParameter = null;
            }
            return guidParameter;
        }

        /// <summary>
        /// Gets the value of an integer parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected int? GetIntParameter(string parameterName) {
            int? intParameter;
            var stringParameter = this.GetStringParameter(parameterName);
            if (!string.IsNullOrEmpty(stringParameter) && int.TryParse(stringParameter, out int parsedStringParameter)) {
                intParameter = parsedStringParameter;
            } else {
                intParameter = null;
            }
            return intParameter;
        }

        /// <summary>
        /// Gets the value of a long parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected long? GetLongParameter(string parameterName) {
            long? longParameter;
            var stringParameter = this.GetStringParameter(parameterName);
            if (!string.IsNullOrEmpty(stringParameter) && long.TryParse(stringParameter, out long parsedStringParameter)) {
                longParameter = parsedStringParameter;
            } else {
                longParameter = null;
            }
            return longParameter;
        }

        /// <summary>
        /// Gets the value of a string parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected string GetStringParameter(string parameterName) {
            return this.queryString[parameterName];
        }

        /// <summary>
        /// Gets the value of an unsigned integer parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected uint? GetUIntParameter(string parameterName) {
            uint? uintParameter;
            var stringParameter = this.GetStringParameter(parameterName);
            if (!string.IsNullOrEmpty(stringParameter) && uint.TryParse(stringParameter, out uint parsedStringParameter)) {
                uintParameter = parsedStringParameter;
            } else {
                uintParameter = null;
            }
            return uintParameter;
        }

        /// <summary>
        /// Gets the value of an unsigned long parameter of query.
        /// </summary>
        /// <param name="parameterName">name of parameter to be
        /// resolved</param>
        /// <returns>value of parameter of query or null</returns>
        protected ulong? GetULongParameter(string parameterName) {
            ulong? ulongParameter;
            var stringParameter = this.GetStringParameter(parameterName);
            if (!string.IsNullOrEmpty(stringParameter) && ulong.TryParse(stringParameter, out ulong parsedStringParameter)) {
                ulongParameter = parsedStringParameter;
            } else {
                ulongParameter = null;
            }
            return ulongParameter;
        }

        /// <summary>
        /// Rounds an integer value to an unsigned long value.
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns>integer value rounded to unsigned long value</returns>
        protected static ulong RoundToULong(int intValue) {
            ulong ulongValue;
            if (intValue < 0) {
                ulongValue = ulong.MinValue;
            } else {
                ulongValue = (ulong)intValue;
            }
            return ulongValue;
        }

    }

}