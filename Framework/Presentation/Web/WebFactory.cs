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

    using Framework.Presentation.Exceptions;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Builds a web control for view field.
    /// </summary>
    /// <param name="viewField">view field to build control for</param>
    /// <param name="topmostParentPresentableObject">topmost
    /// presentable parent object to build web control row for</param>
    /// <param name="comparisonDate">point in time to compare
    /// data of read-only fields to or null to not compare data</param>
    /// <param name="clientFieldIdPrefix">prefix to add to ID of
    /// field on client side</param>
    /// <param name="clientFieldIdSuffix">suffix to add to ID of
    /// field on client side</param>
    /// <param name="postBackState">post back state of the parent
    /// form</param>
    /// <param name="fileBaseDirectory">base directory for files</param>
    /// <returns>web control for view field</returns>
    public delegate WebField BuildReadOnlyWebFieldDelegate(ViewField viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory);

    /// <summary>
    /// Builds a web control for view field.
    /// </summary>
    /// <param name="presentableFieldForCollection">presentable
    /// field to build control for</param>
    /// <param name="viewField">view field to build control for</param>
    /// <param name="topmostParentPresentableObject">topmost
    /// presentable parent object to build web control row for</param>
    /// <param name="comparisonDate">point in time to compare
    /// data of read-only fields to or null to not compare data</param>
    /// <param name="clientFieldIdPrefix">prefix to add to ID of
    /// field on client side</param>
    /// <param name="clientFieldIdSuffix">suffix to add to ID of
    /// field on client side</param>
    /// <param name="postBackState">post back state of the parent
    /// form</param>
    /// <param name="fileBaseDirectory">base directory for files</param>
    /// <returns>web control for view field</returns>
    public delegate WebFieldForEditableValue BuildWebFieldForCollectionDelegate(IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory);

    /// <summary>
    /// Builds a web control for view field.
    /// </summary>
    /// <param name="presentableFieldForElement">presentable
    /// field to build control for</param>
    /// <param name="viewField">view field to build control for</param>
    /// <param name="topmostParentPresentableObject">topmost
    /// presentable parent object to build web control row for</param>
    /// <param name="comparisonDate">point in time to compare
    /// data of read-only fields to or null to not compare data</param>
    /// <param name="clientFieldIdPrefix">prefix to add to ID of
    /// field on client side</param>
    /// <param name="clientFieldIdSuffix">suffix to add to ID of
    /// field on client side</param>
    /// <param name="postBackState">post back state of the parent
    /// form</param>
    /// <param name="fileBaseDirectory">base directory for files</param>
    /// <returns>web control for view field</returns>
    public delegate WebFieldForEditableValue BuildWebFieldForElementDelegate(IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory);

    /// <summary>
    /// Factory for creating controls to be used in forms.
    /// </summary>
    public class WebFactory {

        /// <summary>
        /// True to ignore fields that are contained in view but not
        /// in presentable object, false to throw an exception if a
        /// field is missing in presentable object.
        /// </summary>
        public bool IgnoreMissingFields { get; private set; }

        /// <summary>
        /// Mappings for fields for collections.
        /// </summary>
        private Stack<KeyValuePair<Type, BuildWebFieldForCollectionDelegate>> MappingsForFieldsForCollections {
            get {
                if (null == this.mappingsForFieldsForCollections) {
                    this.mappingsForFieldsForCollections = new Stack<KeyValuePair<Type, BuildWebFieldForCollectionDelegate>>();
                    this.RegisterDefaultMappingsForFieldsForCollections();
                }
                return this.mappingsForFieldsForCollections;
            }
        }
        private Stack<KeyValuePair<Type, BuildWebFieldForCollectionDelegate>> mappingsForFieldsForCollections = null;

        /// <summary>
        /// Mappings for fields for elements.
        /// </summary>
        private Stack<KeyValuePair<Type, BuildWebFieldForElementDelegate>> MappingsForFieldsForElements {
            get {
                if (null == this.mappingsForFieldsForElements) {
                    this.mappingsForFieldsForElements = new Stack<KeyValuePair<Type, BuildWebFieldForElementDelegate>>();
                    this.RegisterDefaultMappingsForFieldsForElements();
                }
                return this.mappingsForFieldsForElements;
            }
        }
        private Stack<KeyValuePair<Type, BuildWebFieldForElementDelegate>> mappingsForFieldsForElements = null;

        /// <summary>
        /// Mappings for read-only fields.
        /// </summary>
        private Stack<KeyValuePair<Type, BuildReadOnlyWebFieldDelegate>> MappingsForReadOnlyFields {
            get {
                if (null == this.mappingsForReadOnlyFields) {
                    this.mappingsForReadOnlyFields = new Stack<KeyValuePair<Type, BuildReadOnlyWebFieldDelegate>>();
                    this.RegisterDefaultMappingsForReadOnlyFields();
                }
                return this.mappingsForReadOnlyFields;
            }
        }
        private Stack<KeyValuePair<Type, BuildReadOnlyWebFieldDelegate>> mappingsForReadOnlyFields = null;

        /// <summary>
        /// Data provider to use for option providers.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; private set; }

        /// <summary>
        /// Render mode of fields, e.g. for forms or for list tables.
        /// </summary>
        public FieldRenderMode RenderMode { get; private set; }

        /// <summary>
        /// Rich-text-editor settings.
        /// </summary>
        public IRichTextEditorSettings RichTextEditorSettings { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="renderMode">render mode of fields, e.g. for
        /// forms or for list tables</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="buildingRule">rules for building controls
        /// of forms</param>
        /// <param name="richTextEditorSettings">rich-text-editor
        /// settings</param>
        public WebFactory(FieldRenderMode renderMode, IOptionDataProvider optionDataProvider, BuildingRule buildingRule, IRichTextEditorSettings richTextEditorSettings) {
            this.IgnoreMissingFields = BuildingRule.IgnoreMissingFields == buildingRule;
            this.OptionDataProvider = optionDataProvider;
            this.RenderMode = renderMode;
            this.RichTextEditorSettings = richTextEditorSettings;
        }

        /// <summary>
        /// Builds the control for a field of an element view.
        /// </summary>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to ID of
        /// field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to ID of
        /// field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for element</returns>
        public WebField BuildFieldFor(ViewField viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            WebField webField = null;
            var viewFieldType = viewField.Type;
            foreach (var fieldMapping in this.MappingsForReadOnlyFields) {
                if (fieldMapping.Key == viewFieldType || viewFieldType.IsSubclassOf(fieldMapping.Key)) {
                    webField = fieldMapping.Value(viewField, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
                    break;
                }
            }
            if (null == webField) {
                throw new PresentationException("View field cannot be rendered because the form factory cannot build form fields of type \"" + viewField.Type + "\" for presentable fields.");
            }
            return webField;
        }

        /// <summary>
        /// Builds the control for an editable field of an element
        /// view.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// build control for</param>
        /// <param name="viewField">view field to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to ID of
        /// field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to ID of
        /// field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for element</returns>
        public WebFieldForEditableValue BuildFieldFor(IPresentableField presentableField, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            WebFieldForEditableValue webField = null;
            if (presentableField is IPresentableFieldWithOptionDataProvider presentableFieldWithOptionDataProvider) {
                presentableFieldWithOptionDataProvider.OptionDataProvider = this.OptionDataProvider;
            }
            var viewFieldType = viewField.Type;
            if (presentableField.IsForSingleElement) {
                if (presentableField is IPresentableFieldForIUser presentableFieldForIUser) {
                    presentableFieldForIUser.UserDirectory = this.OptionDataProvider.UserDirectory;
                }
                foreach (var fieldMapping in this.MappingsForFieldsForElements) {
                    if (fieldMapping.Key == viewFieldType || viewFieldType.IsSubclassOf(fieldMapping.Key)) {
                        webField = fieldMapping.Value(presentableField as IPresentableFieldForElement, viewField, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
                        break;
                    }
                }
                if (null == webField) {
                    throw new PresentationException("View field with key \"" + viewField.Key + "\" cannot be rendered because the form factory cannot build form fields of type \"" + viewField.Type + "\" for presentable fields storing a single value.");
                }
            } else { // !presentableField.IsForSingleElement
                if (presentableField is IPresentableFieldForIUserCollection presentableFieldForIUserCollection) {
                    presentableFieldForIUserCollection.UserDirectory = this.OptionDataProvider.UserDirectory;
                }
                foreach (var fieldMapping in this.MappingsForFieldsForCollections) {
                    if (fieldMapping.Key == viewFieldType || viewFieldType.IsSubclassOf(fieldMapping.Key)) {
                        webField = fieldMapping.Value(presentableField as IPresentableFieldForCollection, viewField, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
                        break;
                    }
                }
                if (null == webField) {
                    throw new PresentationException("View field with key \"" + viewField.Key + "\" cannot be rendered because the form factory cannot build form fields of type \"" + viewField.Type + "\" for presentable fields storing multiple values.");
                }
            }
            return webField;
        }

        /// <summary>
        /// Builds the form control for a pane of an element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewPane">view pane to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for pane</returns>
        public virtual FormPane BuildPaneFor(IPresentableObject presentableObject, ViewPane viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            FormPane formPane = null;
            if (viewPane is ViewPaneForFields viewPaneForFields) {
                formPane = this.BuildPaneFor(presentableObject, viewPaneForFields, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
            } else if (viewPane is ViewPaneForPanes viewPaneForPanes) {
                formPane = this.BuildPaneFor(presentableObject, viewPaneForPanes, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
            } else if (viewPane is ViewGroupedPane viewGroupedPane) {
                formPane = this.BuildPaneFor(presentableObject, viewGroupedPane, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
            } else if (viewPane is ViewCollectionPaneForFields viewCollectionPaneForFields) {
                formPane = this.BuildPaneFor(presentableObject, viewCollectionPaneForFields, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
            } else if (viewPane is ViewCollectionPaneForPanes viewCollectionPaneForPanes) {
                formPane = this.BuildPaneFor(presentableObject, viewCollectionPaneForPanes, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
            } else {
                throw new PresentationException("Form pane for unknown view pane of type \"" + viewPane.GetType() + "\" cannot be created.");
            }
            return formPane;
        }

        /// <summary>
        /// Builds the form control for a pane of an element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewPane">view pane to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for pane</returns>
        private FormPaneForFields BuildPaneFor(IPresentableObject presentableObject, ViewPaneForFields viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            var formPane = new FormPaneForFields(FormPaneType.StandAlone, presentableObject, viewPane, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, this);
            this.SetCssClassesForPane(formPane, viewPane.HasTwoColumnsInWideWindows);
            return formPane;
        }

        /// <summary>
        /// Builds the form control for a pane of an element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewPane">view pane to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for pane</returns>
        private FormPaneForPanes BuildPaneFor(IPresentableObject presentableObject, ViewPaneForPanes viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            var formPane = new FormPaneForPanes(FormPaneType.StandAlone, presentableObject, viewPane, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, this);
            this.SetCssClassesForPane(formPane);
            return formPane;
        }

        /// <summary>
        /// Builds the form control for a pane of an element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewPane">view pane to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for pane</returns>
        private FormGroupedPane BuildPaneFor(IPresentableObject presentableObject, ViewGroupedPane viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            var formPane = new FormGroupedPane(presentableObject, viewPane, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, this);
            this.SetCssClassesForPane(formPane, viewPane.SectionGroupType);
            return formPane;
        }

        /// <summary>
        /// Builds the form control for a pane of an element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewPane">view pane to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for pane</returns>
        private FormCollectionPaneForFields BuildPaneFor(IPresentableObject presentableObject, ViewCollectionPaneForFields viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            var formPane = new FormCollectionPaneForFields(presentableObject, viewPane, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, this);
            this.SetCssClassesForPane(formPane, viewPane.SectionGroupType);
            return formPane;
        }

        /// <summary>
        /// Builds the form control for a pane of an element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewPane">view pane to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for pane</returns>
        private FormCollectionPaneForPanes BuildPaneFor(IPresentableObject presentableObject, ViewCollectionPaneForPanes viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            var formPane = new FormCollectionPaneForPanes(presentableObject, viewPane, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, this);
            this.SetCssClassesForPane(formPane, SectionGroupType.Tabs);
            return formPane;
        }

        /// <summary>
        /// Builds the form control for a section of a pane of an
        /// element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewSection">view section to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <param name="isNewFromPostBack">true if this is new from
        /// post-back, false otherwise</param>
        /// <returns>new form control for section</returns>
        public virtual FormPaneWithTitle BuildPaneSectionFor(IPresentableObject presentableObject, ViewPaneWithTitle viewSection, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory, bool isNewFromPostBack) {
            FormPaneWithTitle formSection;
            if (viewSection is ViewPaneForFields viewPaneForFields) {
                formSection = this.BuildPaneSectionFor(presentableObject, viewPaneForFields, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
            } else if (viewSection is ViewPaneForPanes viewPaneForPanes) {
                formSection = this.BuildPaneSectionFor(presentableObject, viewPaneForPanes, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory);
            } else {
                throw new PresentationException("Form pane for unknown view pane of type \"" + viewSection.GetType() + "\" cannot be created.");
            }
            formSection.IsNewFromPostBack = isNewFromPostBack;
            return formSection;
        }

        /// <summary>
        /// Builds the form control for a section of a pane of an
        /// element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewSection">view section to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for section</returns>
        private FormPaneForFields BuildPaneSectionFor(IPresentableObject presentableObject, ViewPaneForFields viewSection, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            var formSectionForFields = new FormPaneForFields(FormPaneType.Section, presentableObject, viewSection, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, this);
            this.SetCssClassesForPaneSection(formSectionForFields);
            return formSectionForFields;
        }

        /// <summary>
        /// Builds the form control for a section of a pane of an
        /// element view.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// build control for</param>
        /// <param name="viewSection">view section to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="postBackState">post back state of the parent
        /// form</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for section</returns>
        private FormPaneForPanes BuildPaneSectionFor(IPresentableObject presentableObject, ViewPaneForPanes viewSection, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
            var formSectionForPanes = new FormPaneForPanes(FormPaneType.Section, presentableObject, viewSection, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, this);
            this.SetCssClassesForPaneSection(formSectionForPanes);
            return formSectionForPanes;
        }

        /// <summary>
        /// Builds the form control for a section placeholder of a
        /// pane of an element view.
        /// </summary>
        /// <param name="placeholderText">placeholder text to be shown</param>
        /// <returns>new form control for section paceholder</returns>
        public virtual FormPanePlaceholder BuildPaneSectionPlaceholder(string placeholderText) {
            var sectionPlaceholder = new FormPanePlaceholder(placeholderText);
            this.SetCssClassesForPaneSectionPlaceholder(sectionPlaceholder);
            return sectionPlaceholder;
        }

        /// <summary>
        /// Builds the form control for a section of a pane of an
        /// element view.
        /// </summary>
        /// <param name="placeholder">sample presentable object to
        /// build template control for</param>
        /// <param name="viewSection">view section to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for section</returns>
        public virtual FormPaneWithTitle BuildPaneSectionTemplateFor(IPresentableObject placeholder, ViewPaneWithTitle viewSection, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, string fileBaseDirectory) {
            FormPaneWithTitle formSection;
            if (viewSection is ViewPaneForFields viewPaneForFields) {
                formSection = this.BuildPaneSectionTemplateFor(placeholder, viewPaneForFields, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, fileBaseDirectory);
            } else if (viewSection is ViewPaneForPanes viewPaneForPanes) {
                formSection = this.BuildPaneSectionTemplateFor(placeholder, viewPaneForPanes, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, fileBaseDirectory);
            } else {
                throw new PresentationException("Form pane section for unknown view pane of type \"" + viewSection.GetType() + "\" cannot be created.");
            }
            formSection.IsNewFromPostBack = true;
            return formSection;
        }

        /// <summary>
        /// Builds the form control for a section of a pane of an
        /// element view.
        /// </summary>
        /// <param name="placeholder">sample presentable object to
        /// build template control for</param>
        /// <param name="viewSection">view section to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for section</returns>
        private FormPaneForFields BuildPaneSectionTemplateFor(IPresentableObject placeholder, ViewPaneForFields viewSection, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, string fileBaseDirectory) {
            var formSectionForFields = new FormPaneForFields(FormPaneType.Section, placeholder, viewSection, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, PostBackState.NoPostBack, fileBaseDirectory, this);
            this.SetCssClassesForPaneSectionTemplate(formSectionForFields);
            return formSectionForFields;
        }

        /// <summary>
        /// Builds the form control for a section template of a pane
        /// of an element view.
        /// </summary>
        /// <param name="placeholder">sample presentable object to
        /// build template control for</param>
        /// <param name="viewSection">view section to build control for</param>
        /// <param name="topmostParentPresentableObject">topmost
        /// presentable parent object to build form for</param>
        /// <param name="comparisonDate">point in time to compare
        /// data of read-only fields to or null to not compare data</param>
        /// <param name="clientFieldIdPrefix">prefix to add to the ID
        /// of each field on client side</param>
        /// <param name="clientFieldIdSuffix">suffix to add to the ID
        /// of each field on client side</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new form control for section template</returns>
        private FormPaneForPanes BuildPaneSectionTemplateFor(IPresentableObject placeholder, ViewPaneForPanes viewSection, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, string fileBaseDirectory) {
            var formSectionForPanes = new FormPaneForPanes(FormPaneType.Section, placeholder, viewSection, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, PostBackState.NoPostBack, fileBaseDirectory, this);
            this.SetCssClassesForPaneSectionTemplate(formSectionForPanes);
            return formSectionForPanes;
        }

        /// <summary>
        /// Builds the control for a table row of a list table.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// view/edit</param>
        /// <param name="view">view to render form for</param>
        /// <param name="fileBaseDirectory">base directory for files</param>
        /// <returns>new list table control for element</returns>
        public virtual ListTableRow BuildRowFor(IPresentableObject presentableObject, IListTableView view, string fileBaseDirectory) {
            return new ListTableRow(presentableObject, view, fileBaseDirectory, this);
        }

        /// <summary>
        /// Registers all default mappings for fields for
        /// collections.
        /// </summary>
        protected virtual void RegisterDefaultMappingsForFieldsForCollections() {
            // last registered mapping will be checked at first
            this.RegisterFieldMapping<ViewFieldForMultipleSingleLineTexts>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleSingleLineTexts(presentableFieldForCollection, (ViewFieldForMultipleSingleLineTexts)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultipleUrls>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleUrls(presentableFieldForCollection, (ViewFieldForMultipleUrls)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultipleStringLookups>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleStringLookups(presentableFieldForCollection, (ViewFieldForMultipleStringLookups)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultiplePresentableObjectLookups>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultiplePresentableObjectLookups(presentableFieldForCollection, (ViewFieldForMultiplePresentableObjectLookups)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultiplePhoneNumbers>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultiplePhoneNumbers(presentableFieldForCollection, (ViewFieldForMultiplePhoneNumbers)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultipleNumbers>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleNumbers(presentableFieldForCollection, (ViewFieldForMultipleNumbers)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultipleFiles>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleFiles(presentableFieldForCollection, (ViewFieldForMultipleFiles)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) { FileBaseDirectory = fileBaseDirectory };
            });
            this.RegisterFieldMapping<ViewFieldForMultipleImageFiles>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleImageFiles(presentableFieldForCollection, (ViewFieldForMultipleImageFiles)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) { FileBaseDirectory = fileBaseDirectory };
            });
            this.RegisterFieldMapping<ViewFieldForMultipleEmailAddresses>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleEmailAddresses(presentableFieldForCollection, (ViewFieldForMultipleEmailAddresses)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultipleColors>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleColors(presentableFieldForCollection, (ViewFieldForMultipleColors)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultipleChoices>(delegate (IPresentableFieldForCollection presentableFieldForCollection, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultipleChoices(presentableFieldForCollection, (ViewFieldForMultipleChoices)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            return;
        }

        /// <summary>
        /// Registers all default mappings for fields for elements.
        /// </summary>
        protected virtual void RegisterDefaultMappingsForFieldsForElements() {
            // last registered mapping will be checked at first
            this.RegisterFieldMapping<ViewFieldForSingleLineText>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForSingleLineText(presentableFieldForElement, (ViewFieldForSingleLineText)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForNumber>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForNumber(presentableFieldForElement, (ViewFieldForNumber)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForUrl>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForUrl(presentableFieldForElement, (ViewFieldForUrl)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForStringLookup>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForStringLookup(presentableFieldForElement, (ViewFieldForStringLookup)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForPresentableObjectLookup>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForPresentableObjectLookup(presentableFieldForElement, (ViewFieldForPresentableObjectLookup)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForPhoneNumber>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForPhoneNumber(presentableFieldForElement, (ViewFieldForPhoneNumber)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForPassword>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForPassword(presentableFieldForElement, (ViewFieldForPassword)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForNumberWithUnitChoice>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForNumberWithUnitChoice(presentableFieldForElement, (ViewFieldForNumberWithUnitChoice)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForNumberWithUnit>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForNumberWithUnit(presentableFieldForElement, (ViewFieldForNumberWithUnit)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultilineText>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultilineText(presentableFieldForElement, (ViewFieldForMultilineText)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForMultilineRichText>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForMultilineRichText(presentableFieldForElement, (ViewFieldForMultilineRichText)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, this.RichTextEditorSettings);
            });
            this.RegisterFieldMapping<ViewFieldForFile>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForFile(presentableFieldForElement, (ViewFieldForFile)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) { FileBaseDirectory = fileBaseDirectory };
            });
            this.RegisterFieldMapping<ViewFieldForVideoFile>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForVideoFile(presentableFieldForElement, (ViewFieldForVideoFile)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) { FileBaseDirectory = fileBaseDirectory };
            });
            this.RegisterFieldMapping<ViewFieldForImageFile>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForImageFile(presentableFieldForElement, (ViewFieldForImageFile)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState) { FileBaseDirectory = fileBaseDirectory };
            });
            this.RegisterFieldMapping<ViewFieldForEmailAddress>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForEmailAddress(presentableFieldForElement, (ViewFieldForEmailAddress)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForDimensions>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForDimensions(presentableFieldForElement, (ViewFieldForDimensions)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForDateTime>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForUtcDateTime(presentableFieldForElement, (ViewFieldForDateTime)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForColor>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForColor(presentableFieldForElement, (ViewFieldForColor)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForChoice>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForChoice(presentableFieldForElement, (ViewFieldForChoice)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            this.RegisterFieldMapping<ViewFieldForBool>(delegate (IPresentableFieldForElement presentableFieldForElement, ViewFieldForEditableValue viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForBool(presentableFieldForElement, (ViewFieldForBool)viewField, this.RenderMode, topmostParentPresentableObject, this.OptionDataProvider, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState);
            });
            return;
        }

        /// <summary>
        /// Registers all default mappings for read-only fields.
        /// </summary>
        protected virtual void RegisterDefaultMappingsForReadOnlyFields() {
            // last registered mapping will be checked at first
            this.RegisterFieldMapping<ViewFieldForTitle>(delegate (ViewField viewField, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory) {
                return new WebFieldForTitle((ViewFieldForTitle)viewField, this.RenderMode, topmostParentPresentableObject);
            });
            return;
        }

        /// <summary>
        /// Registers a new mapping for a combination of type of read
        /// only view field and delegate for creation of matching web
        /// field.
        /// </summary>
        /// <param name="buildReadOnlyWebFieldDelegate">delegate for
        /// creation of web field</param>
        /// <typeparam name="T">type of read-only view field</typeparam>
        public void RegisterFieldMapping<T>(BuildReadOnlyWebFieldDelegate buildReadOnlyWebFieldDelegate) where T : ViewField {
            this.MappingsForReadOnlyFields.Push(new KeyValuePair<Type, BuildReadOnlyWebFieldDelegate>(typeof(T), buildReadOnlyWebFieldDelegate));
            return;
        }

        /// <summary>
        /// Registers a new mapping for a combination of type of view
        /// field for collection and delegate for creation of matching
        /// web field.
        /// </summary>
        /// <param name="buildWebFieldForCollectionDelegate">delegate for
        /// creation of web field</param>
        /// <typeparam name="T">type of view field for element</typeparam>
        public void RegisterFieldMapping<T>(BuildWebFieldForCollectionDelegate buildWebFieldForCollectionDelegate) where T : ViewFieldForEditableValue {
            this.MappingsForFieldsForCollections.Push(new KeyValuePair<Type, BuildWebFieldForCollectionDelegate>(typeof(T), buildWebFieldForCollectionDelegate));
            return;
        }

        /// <summary>
        /// Registers a new mapping for a combination of type of view
        /// field for element and delegate for creation of matching
        /// web field.
        /// </summary>
        /// <param name="buildWebFieldForElementDelegate">delegate for
        /// creation of web field</param>
        /// <typeparam name="T">type of view field for element</typeparam>
        public void RegisterFieldMapping<T>(BuildWebFieldForElementDelegate buildWebFieldForElementDelegate) where T : ViewFieldForEditableValue {
            this.MappingsForFieldsForElements.Push(new KeyValuePair<Type, BuildWebFieldForElementDelegate>(typeof(T), buildWebFieldForElementDelegate));
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a card pane.
        /// </summary>
        /// <param name="cardPane">card pane to set default css classes
        /// to</param>
        public virtual void SetCssClassesForCardPane(CardPane cardPane) {
            cardPane.CssClasses.Add("cardpane");
            cardPane.CssClassForDescriptionPane = "descriptionpane";
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a form .
        /// </summary>
        /// <param name="form">form to set default css classes to</param>
        public virtual void SetCssClassesForForm(Form form) {
            form.CssClassForDescriptionPane = "descriptionpane";
            form.CssClassForErrorPane = "formerror";
            form.CssClassForUpdateStatus = "updatestatus";
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a gallery.
        /// </summary>
        /// <param name="gallery">gallery to set default css classes
        /// to</param>
        public virtual void SetCssClassesForGallery(Gallery gallery) {
            gallery.CssClasses.Add("gallery");
            gallery.CssClassForDescriptionPane = "descriptionpane";
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a list table.
        /// </summary>
        /// <param name="listTable">list table to set default css
        /// classes to</param>
        public virtual void SetCssClassesForListTable(ListTable listTable) {
            listTable.CssClassForDescriptionPane = "descriptionpane";
            listTable.CssClassForTable = "datatable";
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a form pane which is not
        /// a group of sections.
        /// </summary>
        /// <param name="formPane">form pane to set default css
        /// classes for</param>
        protected virtual void SetCssClassesForPane(FormPaneWithTitle formPane) {
            formPane.CssClassForPaneError = "paneerror";
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a form pane for fields.
        /// </summary>
        /// <param name="formPane">form pane to set default css
        /// classes for</param>
        /// <param name="hasTwoColumnsInWideWindows">indicates
        /// whether fields are supposed to be shown in two columns in
        /// wide windows</param>
        protected virtual void SetCssClassesForPane(FormPaneForFields formPane, bool hasTwoColumnsInWideWindows) {
            this.SetCssClassesForPaneSection(formPane);
            if (hasTwoColumnsInWideWindows) {
                formPane.CssClasses.Add("twocolumns");
            }
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a form pane which is a
        /// group of sections.
        /// </summary>
        /// <param name="formPane">form pane to set default css
        /// classes for</param>
        /// <param name="sectionGroupType">type of section group</param>
        protected virtual void SetCssClassesForPane(FormPane formPane, SectionGroupType sectionGroupType) {
            if (SectionGroupType.Table == sectionGroupType) {
                formPane.CssClasses.Add("tablepane");
            } else if (SectionGroupType.Tabs == sectionGroupType) {
                formPane.CssClasses.Add("tabs");
            } else {
                throw new PresentationException("View pane with unknown section group type \""
                    + sectionGroupType.ToString() + "\" cannot be rendered.");
            }
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a section of a grouped
        /// pane.
        /// </summary>
        /// <param name="formSection">form section to set default css
        /// classes for</param>
        protected virtual void SetCssClassesForPaneSection(FormPaneWithTitle formSection) {
            formSection.CssClassForPaneError = "paneerror";
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a placeholder section of a
        /// grouped pane.
        /// </summary>
        /// <param name="formSection">form section to set default css
        /// classes for</param>
        protected virtual void SetCssClassesForPaneSectionPlaceholder(FormPanePlaceholder formSection) {
            formSection.CssClasses.Add("placeholder");
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a template section of a
        /// grouped pane.
        /// </summary>
        /// <param name="formSection">form section to set default css
        /// classes for</param>
        protected virtual void SetCssClassesForPaneSectionTemplate(FormPaneWithTitle formSection) {
            formSection.CssClasses.Add("template");
            return;
        }

        /// <summary>
        /// Sets the default CSS classes for a tile pane.
        /// </summary>
        /// <param name="tilePane">tile pane to set default css classes
        /// to</param>
        public virtual void SetCssClassesForTilePane(TilePane tilePane) {
            tilePane.CssClasses.Add("tilepane");
            tilePane.CssClassForDescriptionPane = "descriptionpane";
            return;
        }

    }

}