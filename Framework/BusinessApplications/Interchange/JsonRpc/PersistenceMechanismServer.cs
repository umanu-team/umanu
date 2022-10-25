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

    using Framework.Diagnostics;
    using Framework.Persistence;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Server for JSON-RPC remote persistence mechanism.
    /// </summary>
    public sealed class PersistenceMechanismServer {

        /// <summary>
        /// Dictionary of actions that can be called.
        /// </summary>
        public Dictionary<string, Action> Actions { get; private set; }

        /// <summary>
        /// JSON-RPC parser.
        /// </summary>
        private readonly JsonRpcParser jsonRpcParser;

        /// <summary>
        /// Log to use for logging.
        /// </summary>
        public ILog Log { get; private set; }

        /// <summary>
        /// Persistence mechanism to serve.
        /// </summary>
        public PersistenceMechanism PersistenceMechanism { get; private set; }

        /// <summary>
        /// User directory server.
        /// </summary>
        private readonly UserDirectoryServer userDirectoryServer;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to serve</param>
        public PersistenceMechanismServer(PersistenceMechanism persistenceMechanism) {
            this.Actions = new Dictionary<string, Action>();
            this.jsonRpcParser = new JsonRpcParser(persistenceMechanism, new PersistentObjectCache());
            this.PersistenceMechanism = persistenceMechanism;
            this.userDirectoryServer = new UserDirectoryServer(persistenceMechanism.UserDirectory);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to serve</param>
        /// <param name="log">log to use</param>
        public PersistenceMechanismServer(PersistenceMechanism persistenceMechanism, ILog log)
                : this(persistenceMechanism) {
            this.Log = log;
        }

        /// <summary>
        /// Adds an object to the persistence mechanism.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string AddObject(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndPersistentObject;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                bool result = this.PersistenceMechanism.AddObject(parameters.PersistentObject, parameters.InternalContainerName, new List<PersistentObject>()); // TODO: check potential broken references
                var response = new Response(request) {
                    Result = new ResultForBool(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Cleans up the persistence mechanism.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string CleanUp(Request request) {
            this.PersistenceMechanism.CleanUp();
            return new Response(request).ToString(0);
        }

        /// <summary>
        /// Determines whether the container contains a specific
        /// object - including containers of sub types. Permissions
        /// are ignored.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string ContainsID(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndID;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                bool result = this.PersistenceMechanism.ContainsID(parameters.InternalContainerName, parameters.ObjectID);
                var response = new Response(request) {
                    Result = new ResultForBool(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Counts all objects of a certain type in persistence
        /// mechanism - including containers of sub types.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string CountObjects(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndGenericTypeAndFilterCriteria;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                MethodInfo method = this.PersistenceMechanism.GetType().GetMethod(nameof(PersistenceMechanism.CountObjects), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method = method.MakeGenericMethod(parameters.GenericType);
                var result = (int)method.Invoke(this.PersistenceMechanism, new object[] { parameters.InternalContainerName, parameters.FilterCriteria });
                var response = new Response(request) {
                    Result = new ResultForInt(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Gets the all matching persistent objects from persistence
        /// mechanism - including containers of sub types.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string Find(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndGenericTypeAndFullTextQueryAndFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                MethodInfo method = this.PersistenceMechanism.GetType().GetMethod(nameof(PersistenceMechanism.Find), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method = method.MakeGenericMethod(parameters.GenericType);
                var results = method.Invoke(this.PersistenceMechanism, new object[] { parameters.InternalContainerName, parameters.FullTextQuery, parameters.FilterCriteria, parameters.SortCriteria, parameters.StartPosition, parameters.MaxResults }) as System.Collections.IEnumerable;
                var response = new Response(request);
                var responseResult = new ResultForListOfPersistentObject();
                foreach (var result in results) {
                    responseResult.Values.Add(result as PersistentObject);
                }
                response.Result = responseResult;
                jsonResponse = response.ToString(2);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string FindAverageValues(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndFieldNames;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                MethodInfo method = this.PersistenceMechanism.GetType().GetMethod(nameof(PersistenceMechanism.FindAverageValues), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method = method.MakeGenericMethod(parameters.GenericType);
                object result = method.Invoke(this.PersistenceMechanism, new object[] { parameters.InternalContainerName, parameters.FilterCriteria, parameters.FieldNames });
                var response = new Response(request) {
                    Result = new ResultForObject(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string FindComplement(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndGenericTypeAndFullTextQueryAndFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                MethodInfo method = this.PersistenceMechanism.GetType().GetMethod(nameof(PersistenceMechanism.FindComplement), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method = method.MakeGenericMethod(parameters.GenericType);
                var results = method.Invoke(this.PersistenceMechanism, new object[] { parameters.InternalContainerName, parameters.FullTextQuery, parameters.FilterCriteria, parameters.SortCriteria, parameters.StartPosition, parameters.MaxResults }) as System.Collections.IEnumerable;
                var response = new Response(request);
                var responseResult = new ResultForListOfPersistentObject();
                foreach (var result in results) {
                    responseResult.Values.Add(result as PersistentObject);
                }
                response.Result = responseResult;
                jsonResponse = response.ToString(2);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Finds all distinct combinations of a specific set of
        /// properties - including containers of sub types.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string FindDistinctValues(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndSortCriteriaAndFieldNames;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                MethodInfo method = this.PersistenceMechanism.GetType().GetMethod(nameof(PersistenceMechanism.FindDistinctValues), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method = method.MakeGenericMethod(parameters.GenericType);
                object result = method.Invoke(this.PersistenceMechanism, new object[] { parameters.InternalContainerName, parameters.FilterCriteria, parameters.SortCriteria, parameters.FieldNames.ToArray() });
                var response = new Response(request) {
                    Result = new ResultForObject(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string FindSumsOfValues(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndFieldNames;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                MethodInfo method = this.PersistenceMechanism.GetType().GetMethod(nameof(PersistenceMechanism.FindSumsOfValues), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method = method.MakeGenericMethod(parameters.GenericType);
                object result = method.Invoke(this.PersistenceMechanism, new object[] { parameters.InternalContainerName, parameters.FilterCriteria, parameters.FieldNames });
                var response = new Response(request) {
                    Result = new ResultForObject(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Gets the full .NET type name of a container in this
        /// persistence mechanism.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string GetAssemblyQualifiedTypeNameOfContainer(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerName;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                string result = this.PersistenceMechanism.GetAssemblyQualifiedTypeNameOfContainer(parameters.InternalContainerName);
                var response = new Response(request) {
                    Result = new ResultForString(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Gets the internal name of a container in this
        /// persistence mechanism (e.g. the name of the related SQL
        /// database table).
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string GetInternalNameOfContainer(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForAssemblyQualifiedTypeName;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                string result = this.PersistenceMechanism.GetInternalNameOfContainer(parameters.AssemblyQualifiedTypeName);
                var response = new Response(request) {
                    Result = new ResultForString(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Determines whether an object with a specific ID is
        /// deleted. Permissions are ignored.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string IsIdDeleted(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForId;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                bool result = this.PersistenceMechanism.IsIdDeleted(parameters.ObjectId);
                var response = new Response(request) {
                    Result = new ResultForBool(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Responds to a ping request.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string Ping(Request request) {
            return new Response(request).ToString(0);
        }

        /// <summary>
        /// Processes a JSON-RPC request.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed;
            string jsonRequest;
            using (var httpRequestStream = httpRequest.InputStream) {
                var httpRequestStreamReader = new StreamReader(httpRequestStream, Encoding.UTF8);
                jsonRequest = httpRequestStreamReader.ReadToEnd();
            }
            string jsonResponse = this.ProcessRequest(jsonRequest);
            if (string.IsNullOrEmpty(jsonResponse)) {
                isProcessed = false;
            } else {
                httpResponse.Clear();
                httpResponse.AppendHeader("Cache-Control", "no-store");
                httpResponse.ContentType = "application/json";
                byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
                httpResponse.BinaryWrite(buffer);
                isProcessed = true;
            }
            return isProcessed;
        }

        /// <summary>
        /// Processes a JSON-RPC request.
        /// </summary>
        /// <param name="jsonRequest">request to process as JSON-RPC
        /// string</param>
        /// <returns>response as JSON-RPC string or null on error</returns>
        public string ProcessRequest(string jsonRequest) {
            this.Log?.WriteEntry("Persistence mechanism server received the following JSON request: " + jsonRequest, LogLevel.Debug);
            Request request = this.jsonRpcParser.ToPersistentObject(jsonRequest, Guid.Empty) as Request;
            return this.ProcessRequest(request);
        }

        /// <summary>
        /// Processes a JSON-RPC request.
        /// </summary>
        /// <param name="request">JSON-RPC request to process</param>
        /// <returns>response as JSON-RPC string or null on error</returns>
        private string ProcessRequest(Request request) {
            string jsonResponse;
            if (null == request) {
                this.Log?.WriteEntry("Persistence mechanism server will not process JSON request because it is of unknown type.", LogLevel.Debug);
                jsonResponse = null;
            } else {
                this.Log?.WriteEntry("Persistence mechanism server will try to process JSON request using method \"" + request.Method + "\".", LogLevel.Debug);
                if (request.Method.StartsWith("userdirectory.", StringComparison.Ordinal)) {
                    jsonResponse = this.userDirectoryServer.ProcessRequest(request);
                } else if ("addobject" == request.Method) {
                    jsonResponse = this.AddObject(request);
                } else if ("cleanup" == request.Method) {
                    jsonResponse = this.CleanUp(request);
                } else if ("containsid" == request.Method) {
                    jsonResponse = this.ContainsID(request);
                } else if ("countobjects" == request.Method) {
                    jsonResponse = this.CountObjects(request);
                } else if ("find" == request.Method) {
                    jsonResponse = this.Find(request);
                } else if ("findaveragevalues" == request.Method) {
                    jsonResponse = this.FindAverageValues(request);
                } else if ("findcomplement" == request.Method) {
                    jsonResponse = this.FindComplement(request);
                } else if ("finddistinctvalues" == request.Method) {
                    jsonResponse = this.FindDistinctValues(request);
                } else if ("findsumsofvalues" == request.Method) {
                    jsonResponse = this.FindSumsOfValues(request);
                } else if ("getassemblyqualifiedtypenameofcontainer" == request.Method) {
                    jsonResponse = this.GetAssemblyQualifiedTypeNameOfContainer(request);
                } else if ("getinternalnameofcontainer" == request.Method) {
                    jsonResponse = this.GetInternalNameOfContainer(request);
                } else if ("isiddeleted" == request.Method) {
                    jsonResponse = this.IsIdDeleted(request);
                } else if ("ping" == request.Method) {
                    jsonResponse = this.Ping(request);
                } else if ("removeobject" == request.Method) {
                    jsonResponse = this.RemoveObject(request);
                } else if ("removeobjectcascadedly" == request.Method) {
                    jsonResponse = this.RemoveObjectCascadedly(request);
                } else if ("retrieveobject" == request.Method) {
                    jsonResponse = this.RetrieveObject(request);
                } else if ("updateobject" == request.Method) {
                    jsonResponse = this.UpdateObject(request);
                } else {
                    if (this.Actions.TryGetValue(request.Method, out Action action) && null != action) {
                        action.Invoke();
                        jsonResponse = new Response(request).ToString(1);
                    } else {
                        this.Log?.WriteEntry("Persistence mechanism server cannot process request because requested method \"" + request.Method + "\" is unknown.", LogLevel.Warning);
                        jsonResponse = new ErrorResponse(request, ErrorCode.MethodNotFound).ToString(0);
                    }
                }
            }
            return jsonResponse;
        }

        /// <summary>
        /// Removes a specific object from persistence mechanism and
        /// stores its id for replication scenarios.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string RemoveObject(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndIsToBeRemovedIfNotReferencedOnlyAndPersistentObject;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                if (null != parameters.PersistentObject) {
                    parameters.PersistentObject.Retrieve(); // avoids deadlock in transactions
                }
                bool? result = this.PersistenceMechanism.RemoveObject(parameters.PersistentObject, parameters.IsToBeRemovedIfNotReferencedOnly, parameters.InternalContainerName);
                var response = new Response(request) {
                    Result = new ResultForNullableBool(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Removes a specific object and all child objects to be
        /// removed cascadedly from persistence mechanism
        /// immediately.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string RemoveObjectCascadedly(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForPersistentObjectAndIsToBeRemovedIfNotReferencedOnly;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                if (null != parameters.PersistentObject) {
                    parameters.PersistentObject.Retrieve(); // avoids deadlock in transactions
                }
                bool? result = this.PersistenceMechanism.RemoveObjectCascadedly(parameters.PersistentObject, parameters.IsToBeRemovedIfNotReferencedOnly);
                var response = new Response(request) {
                    Result = new ResultForNullableBool(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Retrieves the state of a persistent object from
        /// persistence mechanism.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string RetrieveObject(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndPersistentObject;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                this.PersistenceMechanism.RetrieveObject(parameters.PersistentObject, parameters.InternalContainerName);
                var response = new Response(request) {
                    Result = new ResultForPersistentObject(parameters.PersistentObject)
                };
                jsonResponse = response.ToString(2);
            }
            return jsonResponse;
        }

        /// <summary>
        /// Updates a persistent object in persistence mechanism.
        /// </summary>
        /// <param name="request">request to process</param>
        /// <returns>response as JSON-RPC string</returns>
        private string UpdateObject(Request request) {
            string jsonResponse;
            var parameters = request.Parameters as ParametersForInternalContainerNameAndPersistentObject;
            if (null == parameters) {
                jsonResponse = new ErrorResponse(request, ErrorCode.InvalidParams).ToString(0);
            } else {
                bool result = this.PersistenceMechanism.UpdateObject(parameters.PersistentObject, parameters.InternalContainerName, new List<PersistentObject>()); // TODO: check for potential broken references
                var response = new Response(request) {
                    Result = new ResultForBool(result)
                };
                jsonResponse = response.ToString(1);
            }
            return jsonResponse;
        }

    }

}