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
    using Framework.Persistence.Fields;
    using Framework.Persistence.Text;
    using Persistence.Directories;
    using Presentation.Forms;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;

    /// <summary>
    /// Parser for JSON-RPC strings.
    /// </summary>
    internal sealed class JsonRpcParser : StringParser {

        /// <summary>
        /// Indicates whether persistence mechanism is remote.
        /// </summary>
        private readonly bool isPersistenceMechanismRemote;

        /// <summary>
        /// Cache for persistent objects.
        /// </summary>
        private readonly KeyedCollection<Guid, PersistentObject> objectCache;

        /// <summary>
        /// Persistence mechanism to set as parent persistence
        /// mechanism of paresed objects.
        /// </summary>
        public PersistenceMechanism PersistenceMechanism { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="persistenceMechanism">persistence mechanism
        /// to set as parent persistence mechanism of paresed objects</param>
        /// <param name="objectCache">cache for persistent objects</param>
        public JsonRpcParser(PersistenceMechanism persistenceMechanism, KeyedCollection<Guid, PersistentObject> objectCache)
            : base() {
            this.ArrayDelimiters.Add(new StringDelimiter('[', ',', ']'));
            this.EscapeCharacters.Add('\\');
            this.isPersistenceMechanismRemote = persistenceMechanism is PersistenceMechanismClient;
            this.NonParsableDataDelimiters.Add(new StringDelimiter('\"', char.MinValue, '\"'));
            this.ObjectDelimiters.Add(new StringDelimiter('{', ',', '}'));
            this.PersistenceMechanism = persistenceMechanism;
            this.objectCache = objectCache;
        }

        /// <summary>
        /// Creates a persistent object for a JSON object.
        /// </summary>
        /// <param name="splitStrings">JSON object as split string of
        /// object properties</param>
        /// <param name="idToForceRetrievalFor">ID of object to force
        /// (re)retrieval for or Guid.Empty</param>
        /// <returns>new persistent object for JSON object</returns>
        private PersistentObject CreatePersistentObjectForJsonObject(StringSplitCollection splitStrings, Guid idToForceRetrievalFor) {
            Guid? id = JsonRpcParser.GetIdOfJsonObject(splitStrings);
            PersistentObject persistentObject;
            if (id.HasValue) {
                if (this.objectCache.Contains(id.Value)) {
                    persistentObject = this.objectCache[id.Value];
                } else {
                    Type type = JsonRpcParser.GetTypeOfJsonObject(splitStrings);
                    persistentObject = this.PersistenceMechanism.CreateInstance(type);
                    var parentPersistentContainer = this.PersistenceMechanism.FindContainer(persistentObject.Type);
                    if (this.isPersistenceMechanismRemote ||
                        (this.PersistenceMechanism.ContainsContainer(persistentObject) && parentPersistentContainer.Contains(id.Value))) {
                        persistentObject.ParentPersistentContainer = parentPersistentContainer;
                    }
                    persistentObject.Id = id.Value;
                    if (Guid.Empty != persistentObject.Id && typeof(Request) != persistentObject.Type && typeof(Response) != persistentObject.Type && !persistentObject.Type.IsSubclassOf(typeof(Parameter)) && !persistentObject.Type.IsSubclassOf(typeof(Result))) {
                        this.objectCache.Add(persistentObject);
                    }
                }
                bool isRetrievalForced;
                if (persistentObject.Id == idToForceRetrievalFor) {
                    isRetrievalForced = true;
                    idToForceRetrievalFor = Guid.Empty;
                } else {
                    isRetrievalForced = false;
                }
                if (isRetrievalForced || !persistentObject.IsRetrievedPartially) {
                    bool isRemoved = false;
                    foreach (var property in splitStrings) {
                        string[] splitProperty = property.Split(new char[] { ':' }, 2, System.StringSplitOptions.None);
                        if (2 == splitProperty.Length) {
                            string key = splitProperty[0];
                            string value = splitProperty[1];
                            string valueToUpper = value.ToUpperInvariant();
                            if ("\"IsRemoved\"" == key) {
                                isRemoved = ("TRUE" == valueToUpper);
                            } else if ("\"Type\"" != key && "\"" + nameof(PersistentObject.Id) + "\"" != key) {
                                if (!persistentObject.IsRetrievedPartially) {
                                    JsonRpcParser.SetRetrievedCompletelyFor(persistentObject);
                                    foreach (var fieldForList in persistentObject.PersistentFieldsForCollectionsOfElements) {
                                        fieldForList.Clear();
                                    }
                                    foreach (var fieldForList in persistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                                        fieldForList.Clear();
                                    }
                                }
                                if (key.Length > 2 && key.StartsWith("\"", StringComparison.Ordinal) && key.EndsWith("\"", StringComparison.Ordinal)) {
                                    key = key.Substring(1, key.Length - 2);
                                    if ("NULL" == valueToUpper) { // null
                                        var persistentField = persistentObject.GetPersistentFieldForElement(key);
                                        if (null != persistentField) {
                                            persistentField.ValueAsObject = null;
                                        }
                                    } else if ("TRUE" == valueToUpper || "FALSE" == valueToUpper) { // bool
                                        bool boolValue = ("TRUE" == valueToUpper);
                                        persistentObject.GetPersistentFieldForElement(key).ValueAsObject = boolValue;
                                    } else if (value.Length > 1 && value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal)) { // string, DateTime, IUser or Guid
                                        value = value.Substring(1, value.Length - 2);
                                        value = JsonRpcParser.UnescapeString(value);
                                        PersistentFieldForElement field = persistentObject.GetPersistentFieldForElement(key);
                                        if (TypeOf.IUser == field.ContentBaseType) {
                                            var fieldForUser = field as IPresentableFieldForIUser;
                                            fieldForUser.UserDirectory = this.PersistenceMechanism.UserDirectory;
                                            var guidValue = Guid.Parse(value);
                                            var userValue = this.PersistenceMechanism.UserDirectory.FindOne(guidValue);
                                            if (null == userValue && UserDirectory.AnonymousUser.Id == guidValue) {
                                                userValue = UserDirectory.AnonymousUser;
                                            }
                                            fieldForUser.Value = userValue;
                                        } else {
                                            field.ValueAsString = value;
                                        }
                                    } else { // number, array or object
                                        StringSplitCollection childSplitStrings = this.TrySplit(value, StringSplitOptions.None);
                                        if (StringSplitType.None == childSplitStrings.SplitType) { // number
                                            PersistentFieldForElement field = persistentObject.GetPersistentFieldForElement(key);
                                            if (TypeOf.DateTime == field.ContentBaseType || TypeOf.NullableDateTime == field.ContentBaseType) {
                                                field.ValueAsObject = new DateTime(long.Parse(value, CultureInfo.InvariantCulture), DateTimeKind.Utc);
                                            } else {
                                                field.ValueAsString = value;
                                            }
                                        } else if (StringSplitType.Array == childSplitStrings.SplitType) { // array
                                            this.SetFieldForListOfPersistentObject(childSplitStrings, key, persistentObject, idToForceRetrievalFor);
                                        } else if (StringSplitType.Object == childSplitStrings.SplitType) { // object
                                            PersistentObject childObject = this.CreatePersistentObjectForJsonObject(childSplitStrings, idToForceRetrievalFor);
                                            var persistentFieldForPersistentObject = persistentObject.GetPersistentFieldForPersistentObject(key);
                                            persistentFieldForPersistentObject.InitialValue = childObject;
                                            persistentFieldForPersistentObject.ValueAsObject = childObject;
                                        } else {
                                            throw new FormatException("JSON value \"" + value + "\" of property \"" + property + "\" is malformed.");
                                        }
                                    }
                                } else {
                                    throw new FormatException("JSON key \"" + key + "\" of property \"" + property + "\" is malformed.");
                                }
                            }
                        } else {
                            throw new FormatException("JSON property \"" + property + "\" is malformed.");
                        }
                        persistentObject.IsRemoved = isRemoved;
                    }
                }
            } else {
                throw new FormatException("JSON object has no ID.");
            }
            return persistentObject;
        }

        /// <summary>
        /// Gets the ID of a JSON object.
        /// </summary>
        /// <param name="splitStrings">JSON object as split string of
        /// object properties</param>
        /// <returns>ID of JSON object or null</returns>
        private static Guid? GetIdOfJsonObject(StringSplitCollection splitStrings) {
            Guid? id = null;
            foreach (var property in splitStrings) {
                string[] splitProperty = property.Split(new char[] { ':' }, 2, System.StringSplitOptions.None);
                if (2 == splitProperty.Length) {
                    string key = splitProperty[0];
                    string value = splitProperty[1];
                    if ("\"" + nameof(PersistentObject.Id) + "\"" == key && value.Length > 2 && value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal)) {
                        value = value.Substring(1, value.Length - 2);
                        id = Guid.Parse(value);
                        break;
                    }
                }
            }
            return id;
        }

        /// <summary>
        /// Gets the corresponding .NET type of a JSON object.
        /// </summary>
        /// <param name="splitStrings">JSON object as split string of
        /// object properties</param>
        /// <returns>corresponding .NET type of JSON object</returns>
        private static Type GetTypeOfJsonObject(StringSplitCollection splitStrings) {
            Type type = null;
            foreach (var property in splitStrings) {
                string[] splitProperty = property.Split(new char[] { ':' }, 2, System.StringSplitOptions.None);
                if (2 == splitProperty.Length) {
                    string key = splitProperty[0];
                    string value = splitProperty[1];
                    if ("\"Type\"" == key && value.Length > 2 && value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal)) {
                        value = value.Substring(1, value.Length - 2);
                        value = JsonRpcParser.UnescapeString(value);
                        type = Type.GetType(value);
                        break;
                    }
                }
            }
            return type;
        }

        /// <summary>
        /// Adds an array propterty to a node.
        /// </summary>
        /// <param name="splitStrings">array as split JSON string</param>
        /// <param name="key">key of node property to add</param>
        /// <param name="persistentObject">persistent object to add
        /// array property to</param>
        /// <param name="idToForceRetrievalFor">ID of object to force
        /// (re)retrieval for or Guid.Empty</param>
        private void SetFieldForListOfPersistentObject(StringSplitCollection splitStrings, string key, PersistentObject persistentObject, Guid idToForceRetrievalFor) {
            if (splitStrings.Count > 0 && (1 != splitStrings.Count || !string.IsNullOrEmpty(splitStrings[0]))) {
                string sampleValue = null;
                foreach (string value in splitStrings) {
                    if ("NULL" != value.ToUpperInvariant()) {
                        sampleValue = value;
                    }
                }
                string sampleValueToUpper;
                if (null == sampleValue) {
                    sampleValueToUpper = null;
                } else {
                    sampleValueToUpper = sampleValue.ToUpperInvariant();
                }
                if (null == sampleValue || (sampleValue.Length > 1 && sampleValue.StartsWith("\"", StringComparison.Ordinal) && sampleValue.EndsWith("\"", StringComparison.Ordinal))) { // strings, DateTimes, IUsers or Guids
                    PersistentFieldForCollection field = persistentObject.GetPersistentFieldForCollectionOfElements(key);
                    if (TypeOf.IUser == field.ContentBaseType) {
                        var fieldForUsers = field as IPresentableFieldForIUserCollection;
                        if (null != fieldForUsers) {
                            fieldForUsers.UserDirectory = this.PersistenceMechanism.UserDirectory;
                        }
                    }
                    foreach (string splitString in splitStrings) {
                        if ("NULL" == splitString.ToUpperInvariant()) {
                            field.AddObject(null);
                        } else if (splitString.Length > 1 && splitString.StartsWith("\"", StringComparison.Ordinal) && splitString.EndsWith("\"", StringComparison.Ordinal)) {
                            string value = splitString.Substring(1, splitString.Length - 2);
                            value = JsonRpcParser.UnescapeString(value);
                            if (TypeOf.IUser == field.ContentBaseType) {
                                var guidValue = Guid.Parse(value);
                                var userValue = this.PersistenceMechanism.UserDirectory.FindOne(guidValue);
                                if (null == userValue && UserDirectory.AnonymousUser.Id == guidValue) {
                                    userValue = UserDirectory.AnonymousUser;
                                }
                                if (null != userValue) {
                                    field.AddObject(userValue);
                                }
                            } else {
                                field.AddString(value);
                            }
                        } else {
                            throw new FormatException("JSON value \"" + splitString + "\" of key \"" + key + "\" is malformed - string value expected.");
                        }
                    }
                } else if ("TRUE" == sampleValueToUpper || "FALSE" == sampleValueToUpper) { // bool values
                    PersistentFieldForCollection field = persistentObject.GetPersistentFieldForCollectionOfElements(key);
                    foreach (string splitString in splitStrings) {
                        string valueToUpper = splitString.ToUpperInvariant();
                        if ("TRUE" == valueToUpper) {
                            field.AddObject(true);
                        } else if ("FALSE" == valueToUpper) {
                            field.AddObject(false);
                        } else {
                            throw new FormatException("JSON value \"" + splitString + "\" of key \"" + key + "\" is malformed - boolean value expected.");
                        }
                    }
                } else { // numbers, arrays or objects
                    StringSplitType splitType = this.TrySplit(sampleValue, StringSplitOptions.None).SplitType;
                    if (StringSplitType.None == splitType) { // assume numbers
                        PersistentFieldForCollection field = persistentObject.GetPersistentFieldForCollectionOfElements(key);
                        foreach (string splitString in splitStrings) {
                            field.AddString(splitString);
                        }
                    } else if (StringSplitType.Array == splitType) { // arrays
                        throw new NotSupportedException("JSON value \"" + sampleValue + "\" of key \"" + key + "\" cannot be processed - arrays in arrays are not supported.");
                    } else if (StringSplitType.Object == splitType) { // objects
                        PersistentFieldForPersistentObjectCollection field = persistentObject.GetPersistentFieldForCollectionOfPersistentObjects(key);
                        foreach (string splitString in splitStrings) {
                            if ("NULL" == splitString.ToUpperInvariant()) {
                                field.InitialValues.Add(null);
                                field.AddObject(null);
                            } else {
                                StringSplitCollection childSplitStrings = this.TrySplit(splitString, StringSplitOptions.None);
                                if (StringSplitType.Object == childSplitStrings.SplitType) {
                                    PersistentObject childObject = this.CreatePersistentObjectForJsonObject(childSplitStrings, idToForceRetrievalFor);
                                    field.InitialValues.Add(childObject);
                                    field.AddObject(childObject);
                                } else {
                                    throw new FormatException("JSON value \"" + splitString + "\" of key \"" + key + "\" is malformed - object expected.");
                                }
                            }
                        }
                    } else {
                        throw new FormatException("JSON value \"" + sampleValue + "\" of key \"" + key + "\" is malformed.");
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Converts a JSON string into a persistent object.
        /// </summary>
        /// <param name="jsonString">JSON string to be converted</param>
        /// <param name="idToForceRetrievalFor">ID of object to force
        /// (re)retrieval for or Guid.Empty</param>
        /// <returns>persistent object of type T</returns>
        public PersistentObject ToPersistentObject(string jsonString, Guid idToForceRetrievalFor) {
            PersistentObject persistentObject;
            if (string.IsNullOrEmpty(jsonString)) {
                persistentObject = null;
            } else {
                var splitStrings = this.TrySplit(jsonString, System.StringSplitOptions.None);
                if (StringSplitType.Object == splitStrings.SplitType) {
                    persistentObject = this.CreatePersistentObjectForJsonObject(splitStrings, idToForceRetrievalFor);
                } else {
                    throw new FormatException("JSON string is malformed: " + jsonString);
                }
            }
            return persistentObject;
        }

        /// <summary>
        /// Unescapes a JSON string value.
        /// </summary>
        /// <param name="value">JSON string value to unescape</param>
        /// <returns>escaped JSON string value</returns>
        public static string UnescapeString(string value) {
            return value.Replace("\\}", "}")
                .Replace("\\{", "{")
                .Replace("\\]", "]")
                .Replace("\\[", "[")
                .Replace("\\\u001F", "\u001F")
                .Replace("\\\u001E", "\u001E")
                .Replace("\\\u001D", "\u001D")
                .Replace("\\\u001C", "\u001C")
                .Replace("\\\u001B", "\u001B")
                .Replace("\\\u001A", "\u001A")
                .Replace("\\\u0019", "\u0019")
                .Replace("\\\u0018", "\u0018")
                .Replace("\\\u0017", "\u0017")
                .Replace("\\\u0016", "\u0016")
                .Replace("\\\u0015", "\u0015")
                .Replace("\\\u0014", "\u0014")
                .Replace("\\\u0013", "\u0013")
                .Replace("\\\u0012", "\u0012")
                .Replace("\\\u0011", "\u0011")
                .Replace("\\\u0010", "\u0010")
                .Replace("\\\u000F", "\u000F")
                .Replace("\\\u000E", "\u000E")
                .Replace("\\\u000D", "\u000D")
                .Replace("\\\u000C", "\u000C")
                .Replace("\\\u000B", "\u000B")
                .Replace("\\\u000A", "\u000A")
                .Replace("\\\u0009", "\u0009")
                .Replace("\\\u0008", "\u0008")
                .Replace("\\\u0007", "\u0007")
                .Replace("\\\u0006", "\u0006")
                .Replace("\\\u0005", "\u0005")
                .Replace("\\\u0004", "\u0004")
                .Replace("\\\u0003", "\u0003")
                .Replace("\\\u0002", "\u0002")
                .Replace("\\\u0001", "\u0001")
                .Replace("\\\u0000", "\u0000")
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t")
                .Replace("\\\"", "\"")
                .Replace("\\\\", "\\");
        }

        /// <summary>
        /// Sets the IsRetrievedCompletely property of a persistent
        /// object to true.
        /// </summary>
        /// <param name="persistentObject">persistent object to set
        /// retrieval state for</param>
        internal static void SetRetrievedCompletelyFor(PersistentObject persistentObject) {
            foreach (var persistentField in persistentObject.GetPersistentFields()) {
                persistentField.IsRetrieved = true;
            }
        }

    }

}