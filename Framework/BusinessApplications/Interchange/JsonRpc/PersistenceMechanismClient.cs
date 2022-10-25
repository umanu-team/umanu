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

    using Diagnostics;
    using Framework.Persistence;
    using Framework.Persistence.Directories;
    using Framework.Persistence.Exceptions;
    using Framework.Persistence.Fields;
    using Framework.Persistence.Filters;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Client for JSON-RPC remote persistence mechanism.
    /// </summary>
    public sealed class PersistenceMechanismClient : PersistenceMechanism, IOfflineCapable {

        /// <summary>
        /// Cache for names of internal containers and related
        /// assembly qualified types.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> containerNameCache;

        /// <summary>
        /// Cache for container name caches with http endpoint as
        /// key.
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> containerNameCaches = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        /// <summary>
        /// Credentials to use for authentication on HTTP endpoint.
        /// </summary>
        public ICredentials HttpCredentials { get; set; }

        /// <summary>
        /// Url of JSON-RPC endpoint.
        /// </summary>
        public string HttpEndpoint { get; private set; }

        /// <summary>
        /// Web proxy to use for HTTP endpoint.
        /// </summary>
        public IWebProxy HttpProxy { get; set; }

        /// <summary>
        /// Indicates whether persistence mechanism is fault tolerant
        /// on attempts to read data from offline endpoints. However,
        /// attempts to write data into offline endpoints will fail
        /// anyway.
        /// </summary>
        public bool IsFaultTolerantAgainstOfflineEndpoint { get; set; }

        /// <summary>
        /// Indicates whether persistence mechanism is online. This
        /// information is available as soon as an attempt was made
        /// to access the endpoint. To force recolval, please call
        /// the GetIsOnline() or the Ping() method.
        /// </summary>
        public bool? IsOnline { get; private set; }

        /// <summary>
        /// JSON-RPC parser.
        /// </summary>
        private readonly JsonRpcParser jsonRpcParser;

        /// <summary>
        /// Number of retries on error.
        /// </summary>
        public byte RetriesOnError { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpEndpoint">url of JSON-RPC endpoint</param>
        /// <param name="currentUser">current user that is logged on</param>
        public PersistenceMechanismClient(string httpEndpoint, IUser currentUser)
            : base(new UserDirectoryClient(currentUser), SecurityModel.IgnorePermissions) {
            this.containerNameCache = PersistenceMechanismClient.containerNameCaches.GetOrAdd(httpEndpoint, delegate (string key) {
                return new ConcurrentDictionary<string, string>();
            });
            this.HttpEndpoint = httpEndpoint;
            this.RetriesOnError = 1;
            (this.UserDirectory as UserDirectoryClient).ProcessJsonRpcRequestWithFaultTolerance = this.ProcessJsonRpcRequestWithFaultTolerance;
            this.jsonRpcParser = new JsonRpcParser(this, this.ObjectCache);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="httpEndpoint">url of JSON-RPC endpoint</param>
        /// <param name="currentUser">current user that is logged on</param>
        /// <param name="versioningRepository">repository for
        /// versioning</param>
        public PersistenceMechanismClient(string httpEndpoint, IUser currentUser, PersistenceMechanism versioningRepository)
            : this(httpEndpoint, currentUser) {
            this.VersioningRepository = versioningRepository;
        }

        /// <summary>
        /// Adds a container for storing persistent objects of a
        /// specific type to persistence mechanism (NOT SUPPORTED).
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to create container for</param>
        protected override void AddContainer(PersistentObject sampleInstance) {
            throw new NotSupportedException("Adding containers is not supported in remote persistence mechanisms.");
        }

        /// <summary>
        /// Adds an object to the persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to add</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was added to persistence
        /// mechanism successfully, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        protected internal override bool AddObject(PersistentObject persistentObject, string internalContainerName, IList<PersistentObject> potentialBrokenReferences) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request("addobject") {
                Parameters = new ParametersForInternalContainerNameAndPersistentObject(internalContainerName, persistentObject)
            };
            Response response = this.ProcessJsonRpcRequest(request, 2); // TODO: check potential broken references
            var result = response.Result as ResultForBool;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type bool.");
            }
            JsonRpcParser.SetRetrievedCompletelyFor(persistentObject);
            return result.Value;
        }

        /// <summary>
        /// Cleans up this persistence mechanism.
        /// </summary>
        public override void CleanUp() {
            var request = new Request("cleanup");
            this.ProcessJsonRpcRequest(request, 3);
            return;
        }

        /// <summary>
        /// Determines whether the container contains a specific
        /// object - including containers of sub types. Permissions
        /// are ignored.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="id">specific ID to look for</param>
        /// <returns>true if specific ID is contained, false
        /// otherwise</returns>
        internal override bool ContainsID(string internalContainerName, Guid id) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request("containsid") {
                Parameters = new ParametersForInternalContainerNameAndID(internalContainerName, id)
            };
            Response response = this.ProcessJsonRpcRequest(request, 3);
            var result = response.Result as ResultForBool;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type bool.");
            }
            return result.Value;
        }

        /// <summary>
        /// Checks the existence of references to a persistent object
        /// regardless of permissions.
        /// </summary>
        /// <param name="persistentObject">persistent object to check
        /// existence of references to for</param>
        /// <returns>true if references to persistent object exist,
        /// false otherwise</returns>
        internal override bool ContainsReferencingPersistentObjectsTo(PersistentObject persistentObject) {
            throw new NotSupportedException("Checking the existence of references to persistent objects is not supported for remote persistence mechanisms.");
        }

        /// <summary>
        /// Counts objects of a certain type in persistence mechanism
        /// - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <returns>pairs of group and number of objects of group</returns>
        /// <typeparam name="T">type of persistent objects to count</typeparam>
        internal override int CountObjects<T>(string internalContainerName, FilterCriteria filterCriteria) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request("countobjects") {
                Parameters = new ParametersForInternalContainerNameAndGenericTypeAndFilterCriteria(internalContainerName, typeof(T), filterCriteria)
            };
            Response response = this.ProcessJsonRpcRequest(request, 3);
            var result = response.Result as ResultForInt;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type int.");
            }
            return result.Value;
        }

        /// <summary>
        /// Counts objects of a certain type in persistence mechanism
        /// - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="groupBy">field names to group table rows by</param>
        /// <returns>pairs of group and number of objects of group</returns>
        /// <typeparam name="T">type of persistent objects to count</typeparam>
        internal override IDictionary<string[], int> CountObjects<T>(string internalContainerName, FilterCriteria filterCriteria, string[] groupBy) {
            throw new NotSupportedException("Counting grouped objects is not supported for remote persistence mechanisms."); // remote call could be implemented easily if required
        }

        /// <summary>
        /// Copys the permission independent state of a source
        /// instance into this one.
        /// </summary>
        /// <param name="source">source instance to copy permission
        /// independent state from</param>
        private void CopyPartiallyFrom(PersistenceMechanismClient source) {
            base.CopyPartiallyFrom(source);
            this.HttpCredentials = source.HttpCredentials;
            this.HttpProxy = source.HttpProxy;
            this.IsFaultTolerantAgainstOfflineEndpoint = source.IsFaultTolerantAgainstOfflineEndpoint;
            this.RetriesOnError = source.RetriesOnError;
            return;
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to apply permissions. This
        /// method does not cache the copied persistence mechanism.
        /// </summary>
        /// <returns>copy of this persistence mechanism that applies
        /// all permissions</returns>
        protected override PersistenceMechanism CopyWithCurrentUserPrivilegesNoCache() {
            var copy = new PersistenceMechanismClient(this.HttpEndpoint, this.UserDirectory.CurrentUser, this.VersioningRepository);
            copy.CopyPartiallyFrom(this);
            return copy;
        }

        /// <summary>
        /// Copys this persistence mechanism and sets the security
        /// model of the copied instance to ignore permissions. This
        /// method does not cache the copied elevated persistence
        /// mechanism.
        /// </summary>
        /// <returns>copy of this persistence mechanism that ignores
        /// all permissions</returns>
        protected internal override PersistenceMechanism CopyWithElevatedPrivilegesNoCache() {
            var copy = new PersistenceMechanismClient(this.HttpEndpoint, this.UserDirectory.CurrentUser, this.VersioningRepository);
            copy.CopyPartiallyFrom(this);
            return copy;
        }

        /// <summary>
        /// Copys this persistence mechanism but replaces the user
        /// directory. This method does not cache the copied
        /// persistence mechanism.
        /// </summary>
        /// <param name="userDirectory">user directory to associate
        /// to copied persistence mechanism</param>
        /// <returns>copy of this persistence mechanism with replaced
        /// user directory</returns>
        public override PersistenceMechanism CopyWithReplacedUserDirectoryNoCache(UserDirectory userDirectory) {
            throw new NotSupportedException("Copying the persistence mechanism with replaced user directory is not supported for remote persistence mechanisms.");
        }

        /// <summary>
        /// Gets all matching persistent objects from persistence
        /// mechanism - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="fullTextQuery">full-text query to select
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" ist the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>all matching objects from persistence mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal override ReadOnlyCollection<T> Find<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            ReadOnlyCollection<T> matches;
            var request = new Request("find") {
                Parameters = new ParametersForInternalContainerNameAndGenericTypeAndFullTextQueryAndFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults(internalContainerName, typeof(T), fullTextQuery, filterCriteria, sortCriteria, startPosition, maxResults)
            };
            Response response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            if (null == response) {
                matches = new List<T>(0).AsReadOnly();
            } else {
                var result = response.Result as ResultForListOfPersistentObject;
                if (null == result) {
                    throw new PersistenceException("JSON-RPC error: Result is not of expected type IList<PersistentObject>.");
                }
                var values = new List<T>(result.Values.Count);
                foreach (var value in result.Values) {
                    values.Add(value as T);
                }
                matches = values.AsReadOnly();
            }
            return matches;
        }

        /// <summary>
        /// Finds the average values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="fieldNames">specific set of properties to
        /// find average values for</param>
        /// <returns>average values of specific properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// average values for</typeparam>
        internal override ReadOnlyCollection<object> FindAverageValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames) {
            ReadOnlyCollection<object> averageValues;
            var request = new Request("findaveragevalues") {
                Parameters = new ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndFieldNames(internalContainerName, typeof(T), filterCriteria, fieldNames)
            };
            Response response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            if (null == response) {
                averageValues = new List<object>(0).AsReadOnly();
            } else {
                var result = response.Result as ResultForObject;
                if (null == result) {
                    throw new PersistenceException("JSON-RPC error: Result is not of expected type object.");
                }
                averageValues = (ReadOnlyCollection<object>)result.Value;
            }
            return averageValues;
        }

        /// <summary>
        /// Finds the complement set of all matching persistent
        /// objects from this container - including containers of sub
        /// types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="fullTextQuery">full-text query to select
        /// objects for</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort objects by</param>
        /// <param name="startPosition">index of first position in
        /// results to return - "0" ist the lowest index: "0" would
        /// return all results, whereas "5" would skip the five first
        /// results (this is useful for paging)</param>
        /// <param name="maxResults">maximum number of results to
        /// return</param>
        /// <returns>matching persistent objects from persistence
        /// mechanism</returns>
        /// <typeparam name="T">type of persistent object to find</typeparam>
        internal override ReadOnlyCollection<T> FindComplement<T>(string internalContainerName, string fullTextQuery, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, ulong startPosition, ulong maxResults) {
            ReadOnlyCollection<T> matches;
            var request = new Request("findcomplement") {
                Parameters = new ParametersForInternalContainerNameAndGenericTypeAndFullTextQueryAndFilterCriteriaAndSortCriteriaAndStartPositionAndMaxResults(internalContainerName, typeof(T), fullTextQuery, filterCriteria, sortCriteria, startPosition, maxResults)
            };
            Response response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            if (null == response) {
                matches = new List<T>(0).AsReadOnly();
            } else {
                var result = response.Result as ResultForListOfPersistentObject;
                if (null == result) {
                    throw new PersistenceException("JSON-RPC error: Result is not of expected type IList<PersistentObject>.");
                }
                var values = new List<T>(result.Values.Count);
                foreach (var value in result.Values) {
                    values.Add(value as T);
                }
                matches = values.AsReadOnly();
            }
            return matches;
        }

        /// <summary>
        /// Finds all distinct combinations of a specific set of
        /// properties - including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="sortCriteria">criteria to sort the distinct
        /// result combinations by</param>
        /// <param name="fieldNames">specific set of properties to
        /// find all distinct combinations for</param>
        /// <returns>all distinct combinations of a specific set of
        /// properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// distinct values for</typeparam>
        internal override ReadOnlyCollection<object[]> FindDistinctValues<T>(string internalContainerName, FilterCriteria filterCriteria, SortCriterionCollection sortCriteria, string[] fieldNames) {
            ReadOnlyCollection<object[]> distinctValues;
            var request = new Request("finddistinctvalues") {
                Parameters = new ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndSortCriteriaAndFieldNames(internalContainerName, typeof(T), filterCriteria, sortCriteria, fieldNames)
            };
            Response response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            if (null == response) {
                distinctValues = new List<object[]>(0).AsReadOnly();
            } else {
                var result = response.Result as ResultForObject;
                if (null == result) {
                    throw new PersistenceException("JSON-RPC error: Result is not of expected type object.");
                }
                distinctValues = (ReadOnlyCollection<object[]>)result.Value;
            }
            return distinctValues;
        }

        /// <summary>
        /// Finds the sum of values of specific properties -
        /// including containers of sub types.
        /// </summary>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="filterCriteria">filter criteria to select
        /// objects for</param>
        /// <param name="fieldNames">specific set of properties to
        /// find sums of values for</param>
        /// <returns>sums of values of specific properties</returns>
        /// <typeparam name="T">type of persistent object to find
        /// sums of values for</typeparam>
        internal override ReadOnlyCollection<object> FindSumsOfValues<T>(string internalContainerName, FilterCriteria filterCriteria, string[] fieldNames) {
            ReadOnlyCollection<object> sumsOfValues;
            var request = new Request("findsumsofvalues") {
                Parameters = new ParametersForInternalContainerNameAndGenericTypeAndFilterCriteriaAndFieldNames(internalContainerName, typeof(T), filterCriteria, fieldNames)
            };
            Response response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            if (null == response) {
                sumsOfValues = new List<object>(0).AsReadOnly();
            } else {
                var result = response.Result as ResultForObject;
                if (null == result) {
                    throw new PersistenceException("JSON-RPC error: Result is not of expected type object.");
                }
                sumsOfValues = (ReadOnlyCollection<object>)result.Value;
            }
            return sumsOfValues;
        }

        /// <summary>
        /// Gets the full .NET type name of a container in this
        /// persistence mechanism.
        /// </summary>
        /// <param name="internalContainerName">internal name of a
        /// container in this persistence mechanism</param>
        /// <returns>full .NET type name of a container in this
        /// persistence mechanism or an empty string if container
        /// does not exist</returns>
        protected internal override string GetAssemblyQualifiedTypeNameOfContainer(string internalContainerName) {
            return this.containerNameCache.GetOrAdd(internalContainerName, delegate (string key) {
                string assemblyQualifiedTypeName;
                var request = new Request("getassemblyqualifiedtypenameofcontainer") {
                    Parameters = new ParametersForInternalContainerName(internalContainerName)
                };
                Response response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
                if (null == response) {
                    assemblyQualifiedTypeName = " ";
                } else {
                    var result = response.Result as ResultForString;
                    if (null == result) {
                        throw new PersistenceException("JSON-RPC error: Result is not of expected type string.");
                    }
                    assemblyQualifiedTypeName = result.Value;
                }
                return assemblyQualifiedTypeName;
            });
        }

        /// <summary>
        /// Gets info objects for all containers (NOT SUPPORTED).
        /// </summary>
        /// <returns>info objects for all containers</returns>
        protected override ICollection<ContainerInfo> GetContainerInfos() {
            throw new NotSupportedException("Getting container infos of remote persistence mechanisms is not supported.");
        }

        /// <summary>
        /// Gets the unique ID of this persistence mechanism.
        /// </summary>
        /// <returns>unique ID of this persistence mechanism</returns>
        protected override string GetId() {
            return this.HttpEndpoint;
        }

        /// <summary>
        /// Gets the internal name of a container in this
        /// persistence mechanism (e.g. the name of the related SQL
        /// database table).
        /// </summary>
        /// <param name="assemblyQualifiedTypeName">assembly qualifoed type name
        /// of persistent object to get internal name for</param>
        /// <returns>internal name of a container in this persistence
        /// mechanism or an empty string if container does not exist</returns>
        protected internal override string GetInternalNameOfContainer(string assemblyQualifiedTypeName) {
            string internalContainerName = null;
            foreach (var keyValuePair in this.containerNameCache) {
                if (keyValuePair.Value == assemblyQualifiedTypeName) {
                    internalContainerName = keyValuePair.Key;
                    break;
                }
            }
            if (string.IsNullOrEmpty(internalContainerName)) {
                var request = new Request("getinternalnameofcontainer") {
                    Parameters = new ParametersForAssemblyQualifiedTypeName(assemblyQualifiedTypeName)
                };
                Response response = this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
                if (null == response) {
                    internalContainerName = " ";
                } else {
                    var result = response.Result as ResultForString;
                    if (null == result) {
                        throw new PersistenceException("JSON-RPC error: Result is not of expected type string.");
                    }
                    internalContainerName = result.Value;
                    if (!string.IsNullOrEmpty(internalContainerName)) {
                        this.containerNameCache.TryAdd(internalContainerName, assemblyQualifiedTypeName);
                    }
                }
            }
            return internalContainerName;
        }

        /// <summary>
        /// Indicates whether persistence mechanism is online.
        /// </summary>
        /// <returns>true if persistence mechanism is online, false
        /// otherwise</returns>
        public bool GetIsOnline() {
            if (!this.IsOnline.HasValue) {
                this.Ping();
            }
            return this.IsOnline.Value;
        }

        /// <summary>
        /// Gets all references to a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to get
        /// references to for</param>
        /// <returns>all references to persistent object</returns>
        internal override ReadOnlyCollection<PersistentObject> GetReferencingPersistentObjectsTo(PersistentObject persistentObject) {
            throw new NotSupportedException("Getting references to persistent objects of is not supported for remote persistence mechanisms.");
        }

        /// <summary>
        /// Creates all required system containers in an empty
        /// persistence mechanism (NOT SUPPORTED).
        /// </summary>
        protected override void InitializePersistenceMechanism() {
            throw new NotSupportedException("Initialization of remote persistence mechanisms is not supported.");
        }

        /// <summary>
        /// Invokes an action on remote server.
        /// </summary>
        /// <param name="actionName">case sensitive name of action to
        /// be invoked</param>
        public void InvokeAction(string actionName) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request(actionName);
            this.ProcessJsonRpcRequest(request, 3);
            return;
        }

        /// <summary>
        /// Determines whether an object with a specific ID is
        /// deleted. Permissions are ignored.
        /// </summary>
        /// <param name="id">specific ID to look for</param>
        /// <returns>true if object with specific ID is deleted,
        /// false otherwise</returns>
        public override bool IsIdDeleted(Guid id) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request("isiddeleted") {
                Parameters = new ParametersForId(id)
            };
            Response response = this.ProcessJsonRpcRequest(request, 3);
            var result = response.Result as ResultForBool;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type bool.");
            }
            return result.Value;
        }

        /// <summary>
        /// Pings to set IsOnline property.
        /// </summary>
        public void Ping() {
            var request = new Request("ping");
            this.ProcessJsonRpcRequestWithFaultTolerance(request, 3);
            return;
        }

        /// <summary>
        /// Preloads the state of multiple persistent objects from
        /// persistence mechanism.
        /// </summary>
        /// <param name="sampleObject">sample object of common base
        /// class of objects to be preloaded</param>
        /// <param name="persistentObjects">persistent objects to
        /// preload</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <param name="keyChains">key chains of properties to be
        /// preloaded</param>
        internal override void PreloadObjects(PersistentObject sampleObject, IEnumerable<PersistentObject> persistentObjects, string internalContainerName, IEnumerable<string[]> keyChains) {
            throw new NotSupportedException("Partial retrieval of objects is not supported in remote persistence mechanisms.");
        }

        /// <summary>
        /// Processes a JSON-RPC request.
        /// </summary>
        /// <param name="request">JSON-RPC request</param>
        /// <param name="depth">depth of sub objects to resolve
        /// whereas 0 will not resolve any sub objects</param>
        /// <returns>JSON-RPC response</returns>
        private Response ProcessJsonRpcRequest(Request request, uint depth) {
            return this.ProcessJsonRpcRequest(request, depth, Guid.Empty);
        }

        /// <summary>
        /// Processes a JSON-RPC request.
        /// </summary>
        /// <param name="request">JSON-RPC request</param>
        /// <param name="depth">depth of sub objects to resolve
        /// whereas 0 will not resolve any sub objects</param>
        /// <param name="idToForceRetrievalFor">ID of object to force
        /// (re)retrieval for or Guid.Empty</param>
        /// <returns>JSON-RPC response</returns>
        private Response ProcessJsonRpcRequest(Request request, uint depth, Guid idToForceRetrievalFor) {
            PersistentObject responseObject = null;
            byte retry = 0;
            bool isDone = false;
            while (!isDone) {
                try {
                    var webRequest = WebRequest.Create(this.HttpEndpoint) as HttpWebRequest;
                    if (null == this.HttpCredentials) {
                        webRequest.UseDefaultCredentials = true;
                    } else {
                        webRequest.UseDefaultCredentials = false;
                        webRequest.Credentials = this.HttpCredentials;
                    }
                    webRequest.Proxy = this.HttpProxy;
                    string jsonRequest = request.ToString(depth);
                    byte[] buffer = Encoding.UTF8.GetBytes(jsonRequest);
                    webRequest.ContentLength = buffer.LongLength;
                    webRequest.ContentType = "application/json";
                    webRequest.KeepAlive = false;
                    webRequest.Method = "POST";
                    using (var webRequestStream = webRequest.GetRequestStream()) {
                        webRequestStream.Write(buffer, 0, buffer.Length);
                    }
                    using (var webResponse = webRequest.GetResponse()) {
                        this.IsOnline = false != this.IsOnline;
                        using (var webResponseStream = webResponse.GetResponseStream()) {
                            var responseReader = new StreamReader(webResponseStream, Encoding.UTF8);
                            string jsonResponse = responseReader.ReadToEnd();
                            responseObject = this.jsonRpcParser.ToPersistentObject(jsonResponse, idToForceRetrievalFor);
                        }
                    }
                    isDone = true;
#if DEBUG
                } catch (WebException exception) {
                    if (WebExceptionStatus.ProtocolError == exception.Status) {
                        throw new PersistenceException(exception.Message, exception);
                    } else {
#else
                } catch (WebException) {
#endif
                        if (retry < this.RetriesOnError) {
                            this.Log?.WriteEntry("Persistence mechanism client failed to process a JSON request for method \"" + request.Method + "\" and will repeat it.", LogLevel.Warning);
                            Thread.Sleep(retry * 1000);
                            retry++;
                        } else {
                            isDone = true;
                            this.IsOnline = false;
                            throw;
                        }
#if DEBUG
                    }
#endif
                }
            }
            var response = responseObject as Response;
            if (null == response) {
                var errorResponse = responseObject as ErrorResponse;
                string errorMessage = "JSON-RPC error ";
                if (null == errorResponse) {
                    errorMessage += "- invalid response";
                } else {
                    errorMessage += (int)errorResponse.ErrorCode;
                    if (!string.IsNullOrEmpty(errorResponse.ErrorMessage)) {
                        errorMessage += "; " + errorResponse.ErrorMessage;
                    }
                    if (!string.IsNullOrEmpty(errorResponse.ErrorData)) {
                        errorMessage += ": " + errorResponse.ErrorData;
                    }
                }
                throw new PersistenceException(errorMessage);
            }
            return response;
        }

        /// <summary>
        /// Processes a JSON-RPC request with fault tolerance.
        /// </summary>
        /// <param name="request">JSON-RPC request</param>
        /// <param name="depth">depth of sub objects to resolve
        /// whereas 0 will not resolve any sub objects</param>
        /// <returns>JSON-RPC response or null on error</returns>
        private Response ProcessJsonRpcRequestWithFaultTolerance(Request request, uint depth) {
            Response response;
            if (this.IsFaultTolerantAgainstOfflineEndpoint) {
                if (false == this.IsOnline) {
                    response = null;
                } else {
                    try {
                        response = this.ProcessJsonRpcRequest(request, depth);
                    } catch (WebException) {
                        response = null;
                    }
                }
            } else {
                response = this.ProcessJsonRpcRequest(request, depth);
            }
            return response;
        }

        /// <summary>
        /// Deletes the container for storing persistent objects of
        /// a type and all its persistent objects in persistence
        /// mechanism. Containers of subclasses are not affected
        /// (NOT SUPPORTED).
        /// </summary>
        /// <param name="internalContainerName">internal name of
        /// container in this persistence mechanism to remove</param>
        protected override void RemoveContainer(string internalContainerName) {
            throw new NotSupportedException("Removal of containers in remote persistence mechanisms is not supported.");
        }

        /// <summary>
        /// Removes a specific object from persistence mechanism and
        /// stores its id for replication scenarios.
        /// </summary>
        /// <param name="persistentObject">object to remove</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was not removed because
        /// it is referenced by other objects, false otherwise or if
        /// object was not contained in persistence mechanism</returns>
        protected internal override bool? RemoveObject(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly, string internalContainerName) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request("removeobject") {
                Parameters = new ParametersForInternalContainerNameAndIsToBeRemovedIfNotReferencedOnlyAndPersistentObject(internalContainerName, isToBeRemovedIfNotReferencedOnly, persistentObject)
            };
            Response response = this.ProcessJsonRpcRequest(request, 1);
            var result = response.Result as ResultForNullableBool;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type nullable bool.");
            }
            return result.Value;
        }

        /// <summary>
        /// Removes a specific object and all child objects to be
        /// removed cascadedly from persistence mechanism
        /// immediately.
        /// </summary>
        /// <param name="persistentObject">object to remove
        /// cascadedly</param>
        /// <param name="isToBeRemovedIfNotReferencedOnly">true if
        /// object is to be removed if not referenced by other
        /// objects only, false otherwise</param>
        /// <returns>true if object was removed successfully from
        /// persistence mechanism, null if it was not removed because
        /// it is referenced by other objects, false otherwise or if
        /// object was not contained in persistence mechanism</returns>
        internal override bool? RemoveObjectCascadedly(PersistentObject persistentObject, bool isToBeRemovedIfNotReferencedOnly) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request("removeobjectcascadedly") {
                Parameters = new ParametersForPersistentObjectAndIsToBeRemovedIfNotReferencedOnly(persistentObject, isToBeRemovedIfNotReferencedOnly)
            };
            Response response = this.ProcessJsonRpcRequest(request, 1);
            var result = response.Result as ResultForNullableBool;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type nullable bool.");
            }
            return result.Value;
        }

        /// <summary>
        /// Renames a container for storing persistent objects of a
        /// type.
        /// </summary>
        /// <param name="oldName">start of old name of container</param>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to rename container for</param>
        protected override bool RenameContainer(string oldName, PersistentObject sampleInstance) {
            throw new NotSupportedException("Renaming of containers in remote persistence mechanisms is not supported.");
        }

        /// <summary>
        /// Retrieves the state of all fields for elements of
        /// persistent object from persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// retrieve fields for elements for</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveFieldsForElements(PersistentObject persistentObject, string internalContainerName) {
            this.RetrieveObject(persistentObject, internalContainerName);
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// elements from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveFieldForCollectionOfElements(PersistentFieldForCollection persistentField, string internalContainerName) {
            this.RetrieveObject(persistentField.ParentPersistentObject, internalContainerName);
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a persistent object
        /// from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveFieldForPersistentObject(PersistentFieldForPersistentObject persistentField, string internalContainerName) {
            this.RetrieveObject(persistentField.ParentPersistentObject, internalContainerName);
            return;
        }

        /// <summary>
        /// Retrieves the state of a field for a collection of
        /// persistent objects from persistence mechanism.
        /// </summary>
        /// <param name="persistentField">persistent field to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveFieldForCollectionOfPersistentObjects(PersistentFieldForPersistentObjectCollection persistentField, string internalContainerName) {
            this.RetrieveObject(persistentField.ParentPersistentObject, internalContainerName);
            return;
        }

        /// <summary>
        /// Retrieves the state of a persistent object from
        /// persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// retrieve</param>
        /// <param name="internalContainerName">internal name of the
        /// related container</param>
        internal override void RetrieveObject(PersistentObject persistentObject, string internalContainerName) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request("retrieveobject") {
                Parameters = new ParametersForInternalContainerNameAndPersistentObject(internalContainerName, persistentObject)
            };
            Response response = this.ProcessJsonRpcRequest(request, 1, persistentObject.Id);
            var result = response.Result as ResultForPersistentObject;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type PersistentObject.");
            }
            persistentObject.SetIsChangedToFalse();
            return;
        }

        /// <summary>
        /// Updates the container for storing persistent objects of a
        /// specific type in persistence mechanism (NOT SUPPORTED).
        /// </summary>
        /// <param name="sampleInstance">sample instance of type of
        /// persistent object to update container for</param>
        protected override void UpdateContainer(PersistentObject sampleInstance) {
            throw new NotSupportedException("Updating containers in remote persistence mechanisms is not supported.");
        }

        /// <summary>
        /// Updates a persistent object in persistence mechanism.
        /// </summary>
        /// <param name="persistentObject">persistent object to update</param>
        /// <param name="internalContainerName">internal name of the related
        /// container</param>
        /// <param name="potentialBrokenReferences">list of objects
        /// which might have broken references pointing to them if
        /// they remain new after the add/update transaction is
        /// finished</param>
        /// <returns>true if object was updated in persistence
        /// mechanism successfully, false otherwise or if object was
        /// not contained in persistence mechanism</returns>
        protected internal override bool UpdateObject(PersistentObject persistentObject, string internalContainerName, IList<PersistentObject> potentialBrokenReferences) {
            if (false == this.IsOnline) {
                throw new PersistenceException("Referential integrity might be inconsistent because remote persistence mechnism was not online on previous data retrieval.");
            }
            var request = new Request("updateobject") {
                Parameters = new ParametersForInternalContainerNameAndPersistentObject(internalContainerName, persistentObject)
            };
            Response response = this.ProcessJsonRpcRequest(request, 2); // TODO: check potential broken references
            var result = response.Result as ResultForBool;
            if (null == result) {
                throw new PersistenceException("JSON-RPC error: Result is not of expected type bool.");
            }
            return result.Value;
        }

        /// <summary>
        /// Updates the value for created at of a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// update value for created at for</param>
        /// <param name="createdAt">new date/time value to be set for
        /// created at</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        internal override bool UpdateObjectCreatedAt(PersistentObject persistentObject, DateTime createdAt) {
            throw new NotSupportedException("Updating values for created at in remote persistence mechanisms is not supported.");
        }

        /// <summary>
        /// Updates the value for created by of a persistent object.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// update value for created by for</param>
        /// <param name="createdBy">new user value to be set for
        /// created by</param>
        /// <returns>true if object was successfully updated in
        /// container, false otherwise or if object was not contained
        /// in container</returns>
        internal override bool UpdateObjectCreatedBy(PersistentObject persistentObject, IUser createdBy) {
            throw new NotSupportedException("Updating values for created by in remote persistence mechanisms is not supported.");
        }

    }

}