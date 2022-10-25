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
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Builder for JSON-RPC strings.
    /// </summary>
    internal sealed class JsonRpcBuilder {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public JsonRpcBuilder() {
            // nothing to do
        }

        /// <summary>
        /// Appends the JSON string representation of a field to a
        /// string builder.
        /// </summary>
        /// <param name="field">field to add to string builder</param>
        /// <param name="depth">depth of sub objects to resolve
        /// whereas 0 will not resolve any sub objects</param>
        /// <param name="jsonBuilder">string builder to append JSON
        /// string representation of field to</param>
        private static void AppendFieldForListOfPersistentObjectsToStringBuilder(PersistentFieldForPersistentObjectCollection field, ulong depth, Presentation.Converters.JsonBuilder jsonBuilder) {
            jsonBuilder.AppendKey(field.Key);
            jsonBuilder.AppendArrayStart();
            bool isFirstValue = true;
            foreach (var childObject in field.GetValuesAsPersistentObject()) {
                if (isFirstValue) {
                    isFirstValue = false;
                } else {
                    jsonBuilder.AppendSeparator();
                }
                if (null == childObject) {
                    jsonBuilder.AppendValue(null, false);
                } else {
                    ulong? childDepth;
                    if (depth > 0) {
                        childDepth = depth - 1;
                    } else {
                        childDepth = null;
                    }
                    JsonRpcBuilder.AppendPersistentObjectToStringBuilder(childObject, childDepth, jsonBuilder);
                }
            }
            jsonBuilder.AppendArrayEnd();
            return;
        }

        /// <summary>
        /// Appends the JSON string representation of a field to a
        /// string builder.
        /// </summary>
        /// <param name="field">field to add to string builder</param>
        /// <param name="jsonBuilder">string builder to append JSON
        /// string representation of field to</param>
        private static void AppendFieldForCollectionToStringBuilder(PersistentFieldForCollection field, Presentation.Converters.JsonBuilder jsonBuilder) {
            jsonBuilder.AppendKey(field.Key);
            jsonBuilder.AppendArrayStart();
            bool hasStringValues = JsonRpcBuilder.HasStringValue(field);
            bool isFirstValue = true;
            using (var stringEnumerator = field.GetValuesAsString().GetEnumerator()) {
                using (var objectEnumerator = field.GetValuesAsObject().GetEnumerator()) {
                    while (stringEnumerator.MoveNext() && objectEnumerator.MoveNext()) {
                        if (isFirstValue) {
                            isFirstValue = false;
                        } else {
                            jsonBuilder.AppendSeparator();
                        }
                        string value;
                        if (null == objectEnumerator.Current) {
                            value = null;
                        } else {
                            if (TypeOf.AllowedGroups == field.ContentBaseType) {
                                value = ((AllowedGroups)objectEnumerator.Current)?.Id.ToString("N");
                            } else if (TypeOf.DateTime == field.ContentBaseType || TypeOf.NullableDateTime == field.ContentBaseType) {
                                value = ((DateTime)objectEnumerator.Current).Ticks.ToString(CultureInfo.InvariantCulture);
                            } else if (TypeOf.IUser == field.ContentBaseType) {
                                value = ((IUser)objectEnumerator.Current)?.Id.ToString("N");
                            } else {
                                value = stringEnumerator.Current;
                            }
                        }
                        jsonBuilder.AppendValue(value, hasStringValues);
                    }
                }
            }
            jsonBuilder.AppendArrayEnd();
            return;
        }

        /// <summary>
        /// Appends the JSON string representation of a field to a
        /// string builder.
        /// </summary>
        /// <param name="field">field to add to string builder</param>
        /// <param name="jsonBuilder">string builder to append JSON
        /// string representation of field to</param>
        private static void AppendFieldForElementToStringBuilder(PersistentFieldForElement field, Presentation.Converters.JsonBuilder jsonBuilder) {
            jsonBuilder.AppendKey(field.Key);
            bool hasStringValue = JsonRpcBuilder.HasStringValue(field);
            string value;
            if (null == field.ValueAsObject) {
                value = null;
            } else {
                if (TypeOf.AllowedGroups == field.ContentBaseType) {
                    value = ((AllowedGroups)field.ValueAsObject)?.Id.ToString("N");
                } else if (TypeOf.DateTime == field.ContentBaseType || TypeOf.NullableDateTime == field.ContentBaseType) {
                    value = ((DateTime)field.ValueAsObject).Ticks.ToString(CultureInfo.InvariantCulture);
                } else if (TypeOf.IUser == field.ContentBaseType) {
                    value = ((IUser)field.ValueAsObject)?.Id.ToString("N");
                } else {
                    value = field.ValueAsString;
                }
            }
            jsonBuilder.AppendValue(value, hasStringValue);
            return;
        }

        /// <summary>
        /// Appends the JSON string representation of a field to a
        /// string builder.
        /// </summary>
        /// <param name="field">field to add to string builder</param>
        /// <param name="depth">depth of sub objects to resolve
        /// whereas 0 will not resolve any sub objects</param>
        /// <param name="jsonBuilder">string builder to append JSON
        /// string representation of field to</param>
        private static void AppendFieldForPersistentObjectToStringBuilder(PersistentFieldForPersistentObject field, ulong depth, Presentation.Converters.JsonBuilder jsonBuilder) {
            jsonBuilder.AppendKey(field.Key);
            if (null == field.ValueAsObject) {
                jsonBuilder.AppendValue(null, false);
            } else {
                ulong? childDepth;
                if (depth > 0) {
                    childDepth = depth - 1;
                } else {
                    childDepth = null;
                }
                JsonRpcBuilder.AppendPersistentObjectToStringBuilder(field.ValueAsPersistentObject, childDepth, jsonBuilder);
            }
            return;
        }

        /// <summary>
        /// Appends the JSON string representation of a persistent
        /// object to a string builder.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// create JSON string for</param>
        /// <param name="depth">depth of sub objects to resolve
        /// whereas 0 will not resolve any sub objects</param>
        /// <param name="jsonBuilder">string builder to append JSON
        /// string representation of persistent object to</param>
        private static void AppendPersistentObjectToStringBuilder(PersistentObject persistentObject, ulong? depth, Presentation.Converters.JsonBuilder jsonBuilder) {
            jsonBuilder.AppendObjectStart();
            jsonBuilder.AppendKey("Type");
            jsonBuilder.AppendValue(persistentObject.Type.AssemblyQualifiedName, true);
            if (persistentObject.IsRemoved) {
                jsonBuilder.AppendSeparator();
                jsonBuilder.AppendKey("IsRemoved");
                jsonBuilder.AppendValue(bool.TrueString, false);
            }
            if (depth.HasValue) {
                foreach (var field in persistentObject.PersistentFieldsForElements) {
                    jsonBuilder.AppendSeparator();
                    JsonRpcBuilder.AppendFieldForElementToStringBuilder(field, jsonBuilder);
                }
                foreach (var field in persistentObject.PersistentFieldsForPersistentObjects) {
                    jsonBuilder.AppendSeparator();
                    JsonRpcBuilder.AppendFieldForPersistentObjectToStringBuilder(field, depth.Value, jsonBuilder);
                }
                foreach (var field in persistentObject.PersistentFieldsForCollectionsOfElements) {
                    jsonBuilder.AppendSeparator();
                    JsonRpcBuilder.AppendFieldForCollectionToStringBuilder(field, jsonBuilder);
                }
                foreach (var field in persistentObject.PersistentFieldsForCollectionsOfPersistentObjects) {
                    jsonBuilder.AppendSeparator();
                    JsonRpcBuilder.AppendFieldForListOfPersistentObjectsToStringBuilder(field, depth.Value, jsonBuilder);
                }
            } else {
                jsonBuilder.AppendSeparator();
                PersistentFieldForElement field = persistentObject.GetPersistentFieldForElement(nameof(PersistentObject.Id));
                JsonRpcBuilder.AppendFieldForElementToStringBuilder(field, jsonBuilder);
            }
            jsonBuilder.AppendObjectEnd();
            return;
        }

        /// <summary>
        /// Determines whether a field has a value of JSON string
        /// type.
        /// </summary>
        /// <param name="field">field to check</param>
        /// <returns>true if field has JSON string value, false
        /// otherwise</returns>
        private static bool HasStringValue(IPresentableField field) {
            return TypeOf.AllowedGroups == field.ContentBaseType
                || TypeOf.Char == field.ContentBaseType
                || TypeOf.Guid == field.ContentBaseType
                || TypeOf.IUser == field.ContentBaseType
                || TypeOf.NullableChar == field.ContentBaseType
                || TypeOf.NullableGuid == field.ContentBaseType
                || TypeOf.String == field.ContentBaseType;
        }

        /// <summary>
        /// Converts a persistent object into a JSON string.
        /// </summary>
        /// <param name="persistentObject">persistent object to
        /// create JSON string for</param>
        /// <param name="depth">depth of sub objects to resolve
        /// whereas 0 will not resolve any sub objects</param>
        /// <returns>persistent object as JSON string</returns>
        public string ToString(PersistentObject persistentObject, ulong depth) {
            var jsonBuilder = new Presentation.Converters.JsonBuilder();
            jsonBuilder.CharactersToBeEscaped.Add(new KeyValuePair<char, string>('[', "\\["));
            jsonBuilder.CharactersToBeEscaped.Add(new KeyValuePair<char, string>(']', "\\]"));
            jsonBuilder.CharactersToBeEscaped.Add(new KeyValuePair<char, string>('{', "\\{"));
            jsonBuilder.CharactersToBeEscaped.Add(new KeyValuePair<char, string>('}', "\\}"));
            JsonRpcBuilder.AppendPersistentObjectToStringBuilder(persistentObject, depth, jsonBuilder);
            return jsonBuilder.ToString();
        }

    }

}