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

namespace Framework.Presentation.Converters {

    using Forms;
    using Framework.Model;
    using Framework.Persistence;
    using Framework.Presentation.Web.Controllers;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Gets the JSON values for a view field.
    /// </summary>
    /// <param name="presentableFieldForCollection">presentable
    /// field to get JSON values for</param>
    /// <param name="viewField">view field get JSON values for</param>
    /// <param name="topmostParentPresentableObject">topmost
    /// presentable parent object to get JSON values for</param>
    /// <param name="fileBaseDirectory">base directory for files</param>
    /// <param name="isStringValue">true if value is of type
    /// string, false otherwise</param>
    /// <returns>JSON values for view field</returns>
    public delegate IEnumerable<string> GetJsonValueForFieldForCollectionDelegate(IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, string fileBaseDirectory, ref bool isStringValue);

    /// <summary>
    /// Gets the JSON value for a view field.
    /// </summary>
    /// <param name="presentableFieldForElement">presentable
    /// field to get JSON value for</param>
    /// <param name="viewField">view field get JSON value for</param>
    /// <param name="topmostParentPresentableObject">topmost
    /// presentable parent object to get JSON value for</param>
    /// <param name="fileBaseDirectory">base directory for files</param>
    /// <param name="isStringValue">true if value is of type
    /// string, false otherwise</param>
    /// <returns>JSON value for view field</returns>
    public delegate string GetJsonValueForFieldForElementDelegate(IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, string fileBaseDirectory, ref bool isStringValue);

    /// <summary>
    /// Gets the JSON value for a view field.
    /// </summary>
    /// <param name="viewField">view field get JSON value for</param>
    /// <param name="topmostParentPresentableObject">topmost
    /// presentable parent object to get JSON value for</param>
    /// <param name="fileBaseDirectory">base directory for files</param>
    /// <param name="isStringValue">true if value is of type
    /// string, false otherwise</param>
    /// <returns>JSON value for view field</returns>
    public delegate string GetJsonValueForReadOnlyFieldDelegate(ViewField viewField, IPresentableObject topmostParentPresentableObject, string fileBaseDirectory, ref bool isStringValue);

    /// <summary>
    /// Converter for presentable objects to JSON format.
    /// </summary>
    public class JsonWriter {

        /// <summary>
        /// URL of base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; private set; }

        /// <summary>
        /// Indicates whether titles of view fields are supposed to
        /// be used as JSON keys over keys of views fields.
        /// </summary>
        public bool IsPreferringTitlesOverKeys { get; set; }

        /// <summary>
        /// Indicates whether exceptions are supposed to be thrown on
        /// missing fields.
        /// </summary>
        public bool IsThrowingExceptionsOnMissingFields { get; set; }

        /// <summary>
        /// Indicates whether fields with null/empty values are
        /// supposed to be written at all.
        /// </summary>
        public bool IsWritingEmptyValues { get; set; }

        /// <summary>
        /// Indicates whether IDs of new obejcts are supposed to be
        /// written.
        /// </summary>
        public bool IsWritingIdsOfNewObjects { get; set; }

        /// <summary>
        /// Type of conversion to be applied for keys.
        /// </summary>
        public KeyConversion KeyConversion { get; set; }

        /// <summary>
        /// Mappings for fields for collections.
        /// </summary>
        private Stack<KeyValuePair<Type, GetJsonValueForFieldForCollectionDelegate>> MappingsForFieldsForCollections {
            get {
                if (null == this.mappingsForFieldsForCollections) {
                    this.mappingsForFieldsForCollections = new Stack<KeyValuePair<Type, GetJsonValueForFieldForCollectionDelegate>>();
                    this.RegisterDefaultMappingsForFieldsForCollections();
                }
                return this.mappingsForFieldsForCollections;
            }
        }
        private Stack<KeyValuePair<Type, GetJsonValueForFieldForCollectionDelegate>> mappingsForFieldsForCollections = null;

        /// <summary>
        /// Mappings for fields for elements.
        /// </summary>
        private Stack<KeyValuePair<Type, GetJsonValueForFieldForElementDelegate>> MappingsForFieldsForElements {
            get {
                if (null == this.mappingsForFieldsForElements) {
                    this.mappingsForFieldsForElements = new Stack<KeyValuePair<Type, GetJsonValueForFieldForElementDelegate>>();
                    this.RegisterDefaultMappingsForFieldsForElements();
                }
                return this.mappingsForFieldsForElements;
            }
        }
        private Stack<KeyValuePair<Type, GetJsonValueForFieldForElementDelegate>> mappingsForFieldsForElements = null;

        /// <summary>
        /// Mappings for read-only fields.
        /// </summary>
        private Stack<KeyValuePair<Type, GetJsonValueForReadOnlyFieldDelegate>> MappingsForReadOnlyFields {
            get {
                if (null == this.mappingsForReadOnlyFields) {
                    this.mappingsForReadOnlyFields = new Stack<KeyValuePair<Type, GetJsonValueForReadOnlyFieldDelegate>>();
                    this.RegisterDefaultMappingsForReadOnlyFields();
                }
                return this.mappingsForReadOnlyFields;
            }
        }
        private Stack<KeyValuePair<Type, GetJsonValueForReadOnlyFieldDelegate>> mappingsForReadOnlyFields = null;

        /// <summary>
        /// Data provider to use for option providers.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; private set; }

        /// <summary>
        /// View fields to be applied for field mapping ordered by
        /// key chain.
        /// </summary>
        public IEnumerable<ViewField> ViewFields { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="formView">form view to be applied for field
        /// mapping</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        public JsonWriter(FormView formView, IOptionDataProvider optionDataProvider, string fileBaseDirectory)
            : this(formView.GetViewFieldsCascadedly(), optionDataProvider, fileBaseDirectory) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="viewFields">view fields to be applied for
        /// field mapping</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        public JsonWriter(IEnumerable<ViewField> viewFields, IOptionDataProvider optionDataProvider, string fileBaseDirectory)
            : base() {
            this.FileBaseDirectory = fileBaseDirectory;
            this.IsThrowingExceptionsOnMissingFields = true;
            this.IsPreferringTitlesOverKeys = false;
            this.IsWritingEmptyValues = true;
            this.IsWritingIdsOfNewObjects = false;
            this.OptionDataProvider = optionDataProvider;
            var viewFieldList = new List<ViewField>(viewFields);
            ViewField.SortByKeyChain(viewFieldList);
            this.ViewFields = viewFieldList;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="listView">list view to be applied for field
        /// mapping</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="fileBaseDirectory">URL of base directory for
        /// files</param>
        public JsonWriter(IListTableView listView, IOptionDataProvider optionDataProvider, string fileBaseDirectory)
            : this(listView.ViewFields, optionDataProvider, fileBaseDirectory) {
            // nothing to do
        }

        /// <summary>
        /// Writes the view fields of a presentable object into JSON
        /// builder.
        /// </summary>
        /// <param name="presentableObject">presentable object to be
        /// written into JSON builder</param>
        /// <param name="keyChainPath">key chain path to topmost
        /// presentable object</param>
        /// <param name="topmostPresentableObject">topmost
        /// presentable object to be written into JSON builder</param>
        /// <param name="childViewFields">view fields to write</param>
        /// <param name="jsonBuilder">JSON builder to write
        /// presentable object data into</param>
        private void AppendChildViewFieldsToJsonBuilder(IPresentableObject presentableObject, string[] keyChainPath, IPresentableObject topmostPresentableObject, Dictionary<string, List<ViewFieldForEditableValue>> childViewFields, JsonBuilder jsonBuilder) {
            foreach (var childViewField in childViewFields) {
                var presentableField = presentableObject.FindPresentableField(childViewField.Key);
                if (null == presentableField) {
                    if (this.IsThrowingExceptionsOnMissingFields) {
                        throw new KeyNotFoundException("Presentable field for view field with key \"" + childViewField.Key + "\" cannot be found.");
                    }
                } else {
                    jsonBuilder.AppendSeparator();
                    jsonBuilder.AppendKey(childViewField.Key);
                    var childKeyChainPath = KeyChain.Concat(keyChainPath, childViewField.Key);
                    if (presentableField.IsForSingleElement) {
                        var childPresentableObject = (presentableField as IPresentableFieldForElement).ValueAsObject as IPresentableObject;
                        this.AppendObjectToJsonBuilder(childPresentableObject, childKeyChainPath, topmostPresentableObject, childViewField.Value, jsonBuilder);
                    } else {
                        jsonBuilder.AppendArrayStart();
                        bool isFirstChildPresentableObject = true;
                        foreach (IPresentableObject childPresentableObject in (presentableField as IPresentableFieldForCollection).GetValuesAsObject()) {
                            if (isFirstChildPresentableObject) {
                                isFirstChildPresentableObject = false;
                            } else {
                                jsonBuilder.AppendSeparator();
                            }
                            this.AppendObjectToJsonBuilder(childPresentableObject, childKeyChainPath, topmostPresentableObject, childViewField.Value, jsonBuilder);
                        }
                        jsonBuilder.AppendArrayEnd();
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Writes the default ID field of a presentable object into
        /// JSON builder.
        /// </summary>
        /// <param name="presentableObject">presentable object to be
        /// written into JSON builder</param>
        /// <param name="jsonBuilder">JSON builder to write
        /// presentable object data into</param>
        private void AppendIdFieldToJsonBuilder(IPresentableObject presentableObject, JsonBuilder jsonBuilder) {
            jsonBuilder.AppendKey(nameof(presentableObject.Id));
            if (presentableObject.IsNew && !this.IsWritingIdsOfNewObjects) {
                jsonBuilder.AppendValue(null, false);
            } else {
                jsonBuilder.AppendValue(presentableObject.Id.ToString("N"), true);
            }
            return;
        }

        /// <summary>
        /// Writes a presentable object into JSON builder.
        /// </summary>
        /// <param name="presentableObject">presentable object to be
        /// written into JSON builder</param>
        /// <param name="keyChainPath">key chain path to topmost
        /// presentable object</param>
        /// <param name="topmostPresentableObject">topmost
        /// presentable object to be written into JSON builder</param>
        /// <param name="viewFields">view fields to write</param>
        /// <param name="jsonBuilder">JSON builder to write
        /// presentable object data into</param>
        private void AppendObjectToJsonBuilder(IPresentableObject presentableObject, string[] keyChainPath, IPresentableObject topmostPresentableObject, IEnumerable<ViewField> viewFields, JsonBuilder jsonBuilder) {
            if (null == presentableObject) {
                jsonBuilder.AppendValue(null, false);
            } else {
                jsonBuilder.AppendObjectStart();
                this.AppendIdFieldToJsonBuilder(presentableObject, jsonBuilder);
                this.AppendViewFieldsToJsonBuilder(presentableObject, keyChainPath, topmostPresentableObject, viewFields, jsonBuilder);
                jsonBuilder.AppendObjectEnd();
            }
            return;
        }

        /// <summary>
        /// Writes the view fields of a presentable object into JSON
        /// builder.
        /// </summary>
        /// <param name="presentableObject">presentable object to be
        /// written into JSON builder</param>
        /// <param name="keyChainPath">key chain path to topmost
        /// presentable object</param>
        /// <param name="topmostPresentableObject">topmost
        /// presentable object to be written into JSON builder</param>
        /// <param name="viewFields">view fields to write</param>
        /// <param name="jsonBuilder">JSON builder to write
        /// presentable object data into</param>
        private void AppendViewFieldsToJsonBuilder(IPresentableObject presentableObject, string[] keyChainPath, IPresentableObject topmostPresentableObject, IEnumerable<ViewField> viewFields, JsonBuilder jsonBuilder) {
            var childViewFields = new Dictionary<string, List<ViewFieldForEditableValue>>();
            foreach (var viewField in viewFields) {
                if (viewField.IsVisible) {
                    if (viewField is ViewFieldForEditableValue viewFieldForEditableValue && !this.IsIdField(viewFieldForEditableValue)) {
                        var keyChain = KeyChain.RemoveLeadingLinksOf(viewFieldForEditableValue.KeyChain, keyChainPath.LongLength);
                        if (keyChain.LongLength < 2L) {
                            var presentableField = presentableObject.FindPresentableField(keyChain);
                            if (null == presentableField) {
                                if (this.IsThrowingExceptionsOnMissingFields) {
                                    throw new KeyNotFoundException("Presentable field for view field with key \"" + KeyChain.ToKey(keyChain) + "\" cannot be found.");
                                }
                            } else {
                                bool isStringValue = JsonWriter.HasStringValue(presentableField);
                                if (viewFieldForEditableValue is ViewFieldForCollection viewFieldForCollection) {
                                    if (presentableField is IPresentableFieldForCollection presentableFieldForCollection) {
                                        this.AppendViewFieldToJsonBuilder(presentableFieldForCollection, topmostPresentableObject, viewFieldForCollection, jsonBuilder, isStringValue);
                                    }
                                } else {
                                    if (presentableField is IPresentableFieldForElement presentableFieldForElement) {
                                        this.AppendViewFieldToJsonBuilder(presentableFieldForElement, topmostPresentableObject, viewFieldForEditableValue, jsonBuilder, isStringValue);
                                    }
                                }
                            }
                        } else {
                            var key = keyChain[0];
                            if (!childViewFields.ContainsKey(key)) {
                                childViewFields.Add(key, new List<ViewFieldForEditableValue>());
                            }
                            childViewFields[key].Add(viewFieldForEditableValue);
                        }
                    } else {
                        this.AppendViewFieldToJsonBuilder(topmostPresentableObject, jsonBuilder, viewField, true);
                    }
                }
            }
            this.AppendChildViewFieldsToJsonBuilder(presentableObject, keyChainPath, topmostPresentableObject, childViewFields, jsonBuilder);
            return;
        }

        /// <summary>
        /// Writes one view field into JSON builder.
        /// </summary>
        /// <param name="presentableFieldForElement">presentable
        /// field to be written into JDON builder</param>
        /// <param name="topmostPresentableObject">topmost
        /// presentable object to be written into JSON builder</param>
        /// <param name="viewFieldForEditableValue">view field to
        /// write</param>
        /// <param name="jsonBuilder">JSON builder to write
        /// presentable object data into</param>
        /// <param name="isStringValue">true if value is of type
        /// string, false otherwise</param>
        private void AppendViewFieldToJsonBuilder(IPresentableFieldForElement presentableFieldForElement, IPresentableObject topmostPresentableObject, ViewFieldForEditableValue viewFieldForEditableValue, JsonBuilder jsonBuilder, bool isStringValue) {
            string value = null;
            bool hasMapping = false;
            var viewFieldType = viewFieldForEditableValue.Type;
            foreach (var fieldMapping in this.MappingsForFieldsForElements) {
                if (fieldMapping.Key == viewFieldType || viewFieldType.IsSubclassOf(fieldMapping.Key)) {
                    value = fieldMapping.Value(presentableFieldForElement, viewFieldForEditableValue, topmostPresentableObject, this.FileBaseDirectory, ref isStringValue);
                    hasMapping = true;
                    break;
                }
            }
            if (!hasMapping) {
                if (isStringValue && TypeOf.String == presentableFieldForElement.ContentBaseType) {
                    value = viewFieldForEditableValue.GetReadOnlyValueFor(presentableFieldForElement, topmostPresentableObject, this.OptionDataProvider);
                } else if (isStringValue && TypeOf.DateTime == presentableFieldForElement.ContentBaseType || TypeOf.NullableDateTime == presentableFieldForElement.ContentBaseType) {
                    var dateTime = presentableFieldForElement.ValueAsObject as DateTime?;
                    if (dateTime.HasValue) {
                        value = UtcDateTime.FormatAsIso8601Value(dateTime.Value, DateTimeType.DateAndTime);
                    }
                } else if (isStringValue && typeof(IPresentableObject).IsAssignableFrom(presentableFieldForElement.ContentBaseType)) {
                    value = (presentableFieldForElement.ValueAsObject as IPresentableObject)?.Id.ToString("N");
                } else {
                    value = presentableFieldForElement.ValueAsString;
                }
            }
            if (this.IsWritingEmptyValues || !string.IsNullOrEmpty(value)) {
                jsonBuilder.AppendSeparator();
                if (this.IsPreferringTitlesOverKeys && !string.IsNullOrEmpty(viewFieldForEditableValue.Title)) {
                    jsonBuilder.AppendKey(viewFieldForEditableValue.Title);
                } else {
                    jsonBuilder.AppendKey(presentableFieldForElement.Key);
                }
                jsonBuilder.AppendValue(value, isStringValue);
            }
            return;
        }

        /// <summary>
        /// Writes one view field into JSON builder.
        /// </summary>
        /// <param name="presentableFieldForCollection">presentable
        /// field to be written into JDON builder</param>
        /// <param name="topmostPresentableObject">topmost
        /// presentable object to be written into JSON builder</param>
        /// <param name="viewFieldForCollection">view field to write</param>
        /// <param name="jsonBuilder">JSON builder to write
        /// presentable object data into</param>
        /// <param name="isStringValue">true if value is of type
        /// string, false otherwise</param>
        private void AppendViewFieldToJsonBuilder(IPresentableFieldForCollection presentableFieldForCollection, IPresentableObject topmostPresentableObject, ViewFieldForCollection viewFieldForCollection, JsonBuilder jsonBuilder, bool isStringValue) {
            var values = new List<string>();
            bool hasMapping = false;
            var viewFieldType = viewFieldForCollection.Type;
            foreach (var fieldMapping in this.MappingsForFieldsForCollections) {
                if (fieldMapping.Key == viewFieldType || viewFieldType.IsSubclassOf(fieldMapping.Key)) {
                    values.AddRange(fieldMapping.Value(presentableFieldForCollection, viewFieldForCollection, topmostPresentableObject, this.FileBaseDirectory, ref isStringValue));
                    hasMapping = true;
                    break;
                }
            }
            if (!hasMapping) {
                if (isStringValue && TypeOf.String == presentableFieldForCollection.ContentBaseType) {
                    values.AddRange(viewFieldForCollection.GetReadOnlyValuesFor(presentableFieldForCollection, topmostPresentableObject, this.OptionDataProvider));
                } else if (isStringValue && TypeOf.DateTime == presentableFieldForCollection.ContentBaseType || TypeOf.NullableDateTime == presentableFieldForCollection.ContentBaseType) {
                    foreach (var valueAsObject in presentableFieldForCollection.GetValuesAsObject()) {
                        var dateTime = valueAsObject as DateTime?;
                        if (dateTime.HasValue) {
                            values.Add(UtcDateTime.FormatAsIso8601Value(dateTime.Value, DateTimeType.DateAndTime));
                        } else {
                            values.Add(null);
                        }
                    }
                } else if (isStringValue && typeof(IPresentableObject).IsAssignableFrom(presentableFieldForCollection.ContentBaseType)) {
                    foreach (var valueAsObject in presentableFieldForCollection.GetValuesAsObject()) {
                        values.Add((valueAsObject as IPresentableObject)?.Id.ToString("N"));
                    }
                } else {
                    values.AddRange(presentableFieldForCollection.GetValuesAsString());
                }
            }
            jsonBuilder.AppendSeparator();
            if (this.IsPreferringTitlesOverKeys && !string.IsNullOrEmpty(viewFieldForCollection.Title)) {
                jsonBuilder.AppendKey(viewFieldForCollection.Title);
            } else {
                jsonBuilder.AppendKey(presentableFieldForCollection.Key);
            }
            jsonBuilder.AppendArrayStart();
            bool isSeparatorRequired = false;
            foreach (var value in values) {
                if (this.IsWritingEmptyValues || !string.IsNullOrEmpty(value)) {
                    if (isSeparatorRequired) {
                        jsonBuilder.AppendSeparator();
                    } else {
                        isSeparatorRequired = true;
                    }
                    jsonBuilder.AppendValue(value, isStringValue);
                }
                isSeparatorRequired = true;
            }
            jsonBuilder.AppendArrayEnd();
            return;
        }

        /// <summary>
        /// Writes one view field into JSON builder.
        /// </summary>
        /// <param name="topmostPresentableObject">topmost
        /// presentable object to be written into JSON builder</param>
        /// <param name="viewField">view field to write</param>
        /// <param name="jsonBuilder">JSON builder to write
        /// presentable object data into</param>
        /// <param name="isStringValue">true if value is of type
        /// string, false otherwise</param>
        private void AppendViewFieldToJsonBuilder(IPresentableObject topmostPresentableObject, JsonBuilder jsonBuilder, ViewField viewField, bool isStringValue) {
            var viewFieldType = viewField.Type;
            foreach (var fieldMapping in this.MappingsForReadOnlyFields) {
                if (fieldMapping.Key == viewFieldType || viewFieldType.IsSubclassOf(fieldMapping.Key)) {
                    var value = fieldMapping.Value(viewField, topmostPresentableObject, this.FileBaseDirectory, ref isStringValue);
                    if (this.IsWritingEmptyValues || !string.IsNullOrEmpty(value)) {
                        jsonBuilder.AppendSeparator();
                        jsonBuilder.AppendKeyValuePair(viewField.Title, value, isStringValue);
                    }
                    break;
                }
            }
            return;
        }

        /// <summary>
        /// Gets a JSON builder for presentable object to be
        /// converted to JSON.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// be converted</param>
        /// <returns>JSON builder for presentable object to be
        /// converted to JSON</returns>
        private JsonBuilder CreateJsonBuilderFor(IPresentableObject presentableObject) {
            var jsonBuilder = new JsonBuilder {
                KeyConversion = this.KeyConversion
            };
            this.AppendObjectToJsonBuilder(presentableObject, new string[0], presentableObject, this.ViewFields, jsonBuilder);
            return jsonBuilder;
        }

        /// <summary>
        /// Gets a JSON builder for presentable objects to be
        /// converted to JSON.
        /// </summary>
        /// <param name="presentableObjects">presentable objects to
        /// be converted</param>
        /// <returns>JSON builder for presentable objects to be
        /// converted to JSON</returns>
        private JsonBuilder CreateJsonBuilderFor(IEnumerable<IPresentableObject> presentableObjects) {
            var jsonBuilder = new JsonBuilder {
                KeyConversion = this.KeyConversion
            };
            jsonBuilder.AppendArrayStart();
            bool isFirstObject = true;
            foreach (var presentableObject in presentableObjects) {
                if (isFirstObject) {
                    isFirstObject = false;
                } else {
                    jsonBuilder.AppendSeparator();
                }
                this.AppendObjectToJsonBuilder(presentableObject, new string[0], presentableObject, this.ViewFields, jsonBuilder);
            }
            jsonBuilder.AppendArrayEnd();
            return jsonBuilder;
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
                || TypeOf.DateTime == field.ContentBaseType
                || TypeOf.Guid == field.ContentBaseType
                || TypeOf.IUser == field.ContentBaseType
                || TypeOf.NullableChar == field.ContentBaseType
                || TypeOf.NullableDateTime == field.ContentBaseType
                || TypeOf.NullableGuid == field.ContentBaseType
                || TypeOf.PersistentObject == field.ContentBaseType
                || TypeOf.String == field.ContentBaseType
                || typeof(IPresentableObject).IsAssignableFrom(field.ContentBaseType);
        }

        /// <summary>
        /// Indicates whether a view field is a default field.
        /// </summary>
        /// <param name="viewField">view field to be checked</param>
        /// <returns>true if view field is default field, false
        /// otherwise</returns>
        private bool IsIdField(ViewFieldForEditableValue viewField) {
            var key = viewField.KeyChain[viewField.KeyChain.LongLength - 1];
            return nameof(IPresentableObject.Id) == key;
        }

        /// <summary>
        /// Registers all default mappings for fields for
        /// collections.
        /// </summary>
        protected virtual void RegisterDefaultMappingsForFieldsForCollections() {
            // last registered mapping will be checked at first
            this.RegisterFieldMapping<ViewFieldForMultipleFiles>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, string fileBaseDirectory, ref bool isStringValue) {
                var values = new List<string>();
                foreach (var valueAsObject in presentableFieldForCollection.GetValuesAsObject()) {
                    if (valueAsObject is File file) {
                        values.Add(FileController.GetUrlOf(fileBaseDirectory, file.Id, file.Name));
                    } else {
                        values.Add(null);
                    }
                }
                return values;
            });
            return;
        }

        /// <summary>
        /// Registers all default mappings for fields for elements.
        /// </summary>
        protected virtual void RegisterDefaultMappingsForFieldsForElements() {
            // last registered mapping will be checked at first
            this.RegisterFieldMapping<ViewFieldForFile>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, string fileBaseDirectory, ref bool isStringValue) {
                string value;
                if (presentableFieldForElement.ValueAsObject is File file) {
                    value = FileController.GetUrlOf(fileBaseDirectory, file);
                } else {
                    value = null;
                }
                return value;
            });
            return;
        }

        /// <summary>
        /// Registers all default mappings for read-only fields.
        /// </summary>
        protected virtual void RegisterDefaultMappingsForReadOnlyFields() {
            // last registered mapping will be checked at first
            this.RegisterFieldMapping<ViewFieldForTitle>(delegate (ViewField viewField, IPresentableObject topmostParentPresentableObject, string fileBaseDirectory, ref bool isStringValue) {
                return viewField.GetReadOnlyValueFor(null, topmostParentPresentableObject, this.OptionDataProvider);
            });
            return;
        }

        /// <summary>
        /// Registers a new mapping for a combination of type of view
        /// field for collection and delegate for getting JSON value.
        /// </summary>
        /// <param name="getJsonValueForFieldForCollectionDelegate">delegate for
        /// getting JSON value</param>
        /// <typeparam name="T">type of view field for element</typeparam>
        public void RegisterFieldMapping<T>(GetJsonValueForFieldForCollectionDelegate getJsonValueForFieldForCollectionDelegate) where T : ViewFieldForEditableValue {
            this.MappingsForFieldsForCollections.Push(new KeyValuePair<Type, GetJsonValueForFieldForCollectionDelegate>(typeof(T), getJsonValueForFieldForCollectionDelegate));
            return;
        }

        /// <summary>
        /// Registers a new mapping for a combination of type of view
        /// field for element and delegate for getting JSON value.
        /// </summary>
        /// <param name="getJsonValueForFieldForElementDelegate">delegate for
        /// getting JSON value</param>
        /// <typeparam name="T">type of view field for element</typeparam>
        public void RegisterFieldMapping<T>(GetJsonValueForFieldForElementDelegate getJsonValueForFieldForElementDelegate) where T : ViewFieldForEditableValue {
            this.MappingsForFieldsForElements.Push(new KeyValuePair<Type, GetJsonValueForFieldForElementDelegate>(typeof(T), getJsonValueForFieldForElementDelegate));
            return;
        }

        /// <summary>
        /// Registers a new mapping for a combination of type of read
        /// only view field and delegate for getting JSON value.
        /// </summary>
        /// <param name="getJsonValueForReadOnlyFieldDelegate">delegate for
        /// getting JSON value</param>
        /// <typeparam name="T">type of read-only view field</typeparam>
        public void RegisterFieldMapping<T>(GetJsonValueForReadOnlyFieldDelegate getJsonValueForReadOnlyFieldDelegate) where T : ViewField {
            this.MappingsForReadOnlyFields.Push(new KeyValuePair<Type, GetJsonValueForReadOnlyFieldDelegate>(typeof(T), getJsonValueForReadOnlyFieldDelegate));
            return;
        }

        /// <summary>
        /// Returns an JSON file that represents the current object.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="presentableObject">presentable object to be
        /// converted</param>
        /// <returns>JSON file that represents the object</returns>
        public File WriteFile(string name, IPresentableObject presentableObject) {
            return this.CreateJsonBuilderFor(presentableObject).ToFile(name);
        }

        /// <summary>
        /// Returns an JSON file that represents the current object.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <param name="presentableObjects">presentable objects to
        /// be converted</param>
        /// <returns>JSON file that represents the objects</returns>
        public File WriteFile(string name, IEnumerable<IPresentableObject> presentableObjects) {
            return this.CreateJsonBuilderFor(presentableObjects).ToFile(name);
        }

        /// <summary>
        /// Returns an JSON string that represents the current
        /// object.
        /// </summary>
        /// <param name="presentableObject">presentable object to be
        /// converted</param>
        /// <returns>JSON string that represents the current object</returns>
        public string WriteString(IPresentableObject presentableObject) {
            return this.CreateJsonBuilderFor(presentableObject).ToString();
        }

        /// <summary>
        /// Returns an JSON string that represents the current
        /// object.
        /// </summary>
        /// <param name="presentableObjects">presentable objects to
        /// be converted</param>
        /// <returns>JSON string that represents the current object</returns>
        public string WriteString(IEnumerable<IPresentableObject> presentableObjects) {
            return this.CreateJsonBuilderFor(presentableObjects).ToString();
        }

    }

}