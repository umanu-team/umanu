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
    /// Control for rendering panes for combining view fields or
    /// panes of multiple objects in a container - the user can
    /// switch between the objects.
    /// </summary>
    /// <typeparam name="T">type of view collection panes contained in this
    /// collection pane</typeparam>
    public abstract class FormCollectionPane<T> : FormPane where T : ViewCollectionPane {

        /// <summary>
        /// Child form sections.
        /// </summary>
        protected IList<FormPaneWithTitle> FormSections { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this field is read-only.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// True if this pane is supposed to be rendered, false
        /// otherwise. This property is supposed to be set in
        /// CreateChildControls().
        /// </summary>
        protected bool IsVisible { get; set; }

        /// <summary>
        /// View pane to create form pane for.
        /// </summary>
        public T ViewPane { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// show</param>
        /// <param name="viewPane">view pane to render form pane for</param>
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
        /// <param name="formFactory">factory for building form
        /// controls</param>
        public FormCollectionPane(IPresentableObject presentableObject, T viewPane, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory, WebFactory formFactory)
            : base("div", presentableObject, viewPane.Key, topmostParentPresentableObject, comparisonDate, clientFieldIdPrefix, clientFieldIdSuffix, postBackState, fileBaseDirectory, formFactory) {
            var presentableFieldToRenderPaneFor = this.GetPresentableFieldToRenderPaneFor();
            this.IsReadOnly = null == presentableFieldToRenderPaneFor || presentableFieldToRenderPaneFor.IsReadOnly || viewPane.IsReadOnlyFor(presentableObject);
            this.ViewPane = viewPane;
        }

        /// <summary>
        /// Adds a form section for a presentable object, if a
        /// section for it is not contained in the list of sections
        /// already.
        /// </summary>
        /// <param name="presentableObject">presentable object to add
        /// section for</param>
        /// <param name="viewSection">view to use for new section</param>
        /// <param name="idSuffix">ID suffix of new section</param>
        /// <param name="isNewFromPostBack">true if this is new from
        /// post-back, false otherwise</param>
        private void AddSectionForPresentableObject(IPresentableObject presentableObject, ViewPaneWithTitle viewSection, string idSuffix, bool isNewFromPostBack) {
            bool presentableObjectIsContainedInSections = false;
            foreach (var existingSection in this.FormSections) {
                if (existingSection.PresentableObject.Id == presentableObject.Id) {
                    presentableObjectIsContainedInSections = true;
                    break;
                }
            }
            if (!presentableObjectIsContainedInSections) {
                var section = this.FormFactory.BuildPaneSectionFor(presentableObject, viewSection, this.TopmostParentPresentableObject, this.ComparisonDate, this.ClientFieldIdPrefix, this.ClientFieldIdSuffix + idSuffix, this.PostBackState, this.FileBaseDirectory, isNewFromPostBack);
                this.AddTitleFieldToSection(presentableObject, section, idSuffix);
                section.ForceRenderingOfObjectId = true;
                this.Controls.Add(section);
                this.FormSections.Add(section);
            }
            return;
        }

        /// <summary>
        /// Adds a form section template.
        /// </summary>
        /// <param name="placeholder">placeholder object to add
        /// section for</param>
        /// <param name="viewSection">view to use for new section</param>
        private void AddSectionTemplate(IPresentableObject placeholder, ViewPaneWithTitle viewSection) {
            var section = this.FormFactory.BuildPaneSectionTemplateFor(placeholder, viewSection, this.TopmostParentPresentableObject, this.ComparisonDate, this.ClientFieldIdPrefix, this.ClientFieldIdSuffix, this.FileBaseDirectory);
            this.AddTitleFieldToSection(placeholder, section, string.Empty);
            section.ForceRenderingOfObjectId = true;
            this.Controls.Add(section);
            return;
        }

        /// <summary>
        /// Adds the title field to a new section.
        /// </summary>
        /// <param name="presentableObject">object to get title from</param>
        /// <param name="section">new section to add title field to</param>
        /// <param name="idSuffix">ID suffix of new section</param>
        private void AddTitleFieldToSection(IPresentableObject presentableObject, FormPaneWithTitle section, string idSuffix) {
            if (!string.IsNullOrEmpty(this.ViewPane.TitleField)) {
                var viewFieldForTitle = this.ViewPane.FindOneViewField(this.ViewPane.TitleField);
                if (this.ViewPane.HasButtonForAddingNewObjects && null == viewFieldForTitle) {
                    throw new PresentationException("Title field of view pane is not contained in view fields of view pane.");
                } else {
                    var presentableFieldForTitle = presentableObject.FindPresentableField(this.ViewPane.TitleField) as IPresentableFieldForElement;
                    if (null == presentableFieldForTitle) {
                        if (!this.FormFactory.IgnoreMissingFields) {
                            throw new Framework.Presentation.Exceptions.KeyNotFoundException("Presentable field for view field with key \"" + this.ViewPane.TitleField + "\" cannot be found.");
                        }
                    } else {
                        if (null == viewFieldForTitle) {
                            if (!string.IsNullOrEmpty(presentableFieldForTitle.ValueAsString)) {
                                section.Attributes.Add("data-title", System.Web.HttpUtility.HtmlEncode(presentableFieldForTitle.ValueAsString));
                            }
                        } else if (viewFieldForTitle.IsReadOnly || presentableFieldForTitle.IsReadOnly) {
                            var title = viewFieldForTitle.GetReadOnlyValueFor(presentableFieldForTitle, this.TopmostParentPresentableObject, this.FormFactory.OptionDataProvider);
                            if (!string.IsNullOrEmpty(title)) {
                                section.Attributes.Add("data-title", System.Web.HttpUtility.HtmlEncode(title));
                            }
                        } else {
                            string titleInputName;
                            if (string.IsNullOrEmpty(this.ClientFieldIdPrefix)) {
                                titleInputName = string.Empty;
                            } else {
                                titleInputName = this.ClientFieldIdPrefix + '.';
                            }
                            titleInputName += this.ViewPane.TitleField + this.ClientFieldIdSuffix + idSuffix;
                            section.Attributes.Add("data-title-input-name", titleInputName);
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Creates all child controls.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        public override void CreateChildControls(System.Web.HttpRequest httpRequest) {
            var presentableField = this.GetPresentableFieldToRenderPaneFor();
            var presentableFieldForCollection = presentableField as IPresentableFieldForCollection;
            if (null == presentableFieldForCollection) {
                if (null != presentableField) {
                    throw new PresentationException("Form collection pane for presentable field of type " + presentableField.GetType() + " with key \"" + presentableField.Key + "\" cannot be rendered because it is not a presentable field for collections.");
                }
            } else {
                var viewSection = this.ViewPane.ToViewPaneWithTitle();
                if (this.ViewPane.AutoAddFirstSection) {
                    this.Attributes.Add("data-add-first-section", "1");
                } else {
                    this.CreateChildControlsForSectionPlaceholder();
                }
                if (!this.IsReadOnly) {
                    if (this.ViewPane.HasButtonsForRemovingObjects) {
                        this.Attributes.Add("data-remove-button", "1");
                        string confirmationMessageText = this.ViewPane.ConfirmationMessageForRemoval;
                        if (!string.IsNullOrEmpty(confirmationMessageText)) {
                            this.Attributes.Add("data-remove-confirmation", System.Web.HttpUtility.HtmlEncode(confirmationMessageText));
                        }
                    }
                    if (this.ViewPane.IsSortable && !presentableFieldForCollection.IsReadOnly) {
                        this.Attributes.Add("data-is-sortable", "1");
                    }
                }
                this.CreateChildControlsForSectionTemplate(presentableFieldForCollection, viewSection);
                this.FormSections = new List<FormPaneWithTitle>();
                this.CreateChildControlsForPostBackSections(httpRequest, presentableFieldForCollection, viewSection);
                this.CreateChildControlsForNonPostBackSections(presentableFieldForCollection, viewSection);
                base.CreateChildControls(httpRequest);
                this.SetHasReadOnlyFieldsOnly(this.FormSections);
                if ((this.IsReadOnly || !this.ViewPane.HasButtonForAddingNewObjects) && this.FormSections.Count < 1) {
                    this.IsVisible = false;
                } else {
                    this.IsVisible = true;
                }
            }
            return;
        }

        /// <summary>
        /// Creates the child controls for a section placeholder if
        /// necessary.
        /// </summary>
        private void CreateChildControlsForSectionPlaceholder() {
            string placeholderText = this.ViewPane.Placeholder;
            if (!string.IsNullOrEmpty(placeholderText)) {
                var sectionPlaceholder = this.FormFactory.BuildPaneSectionPlaceholder(placeholderText);
                this.Controls.Add(sectionPlaceholder);
            }
            return;
        }

        /// <summary>
        /// Creates the child controls for the section template if
        /// necessary.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// create section for</param>
        /// <param name="viewSection">view to use for new section</param>
        private void CreateChildControlsForSectionTemplate(IPresentableFieldForCollection presentableField, ViewPaneWithTitle viewSection) {
            if (this.ViewPane.HasButtonForAddingNewObjects && !this.IsReadOnly) {
                var placeholder = presentableField.NewItemAsObject() as IPresentableObject;
                this.AddSectionTemplate(placeholder, viewSection);
            }
            return;
        }

        /// <summary>
        /// Creates the child controls for all post-back sections if
        /// applicable.
        /// </summary>
        /// <param name="httpRequest">HTTP request sent by client</param>
        /// <param name="presentableField">presentable field to
        /// create sections for</param>
        /// <param name="viewSection">view to use for new sections</param>
        private void CreateChildControlsForPostBackSections(System.Web.HttpRequest httpRequest, IPresentableFieldForCollection presentableField, ViewPaneWithTitle viewSection) {
            if (PostBackState.ValidPostBack == this.PostBackState) {
                string postBackId;
                int i = 0;
                int highestIndexOfPreparedObjects = -1;
                var sortSequence = new Dictionary<Guid, long>();
                do {
                    postBackId = httpRequest.Form[this.ClientFieldIdPrefix + this.ClientFieldIdSuffix + "_" + i];
                    if (!string.IsNullOrEmpty(postBackId)) {
                        string[] postBackIds = postBackId.Split(new char[] { ',' }, StringSplitOptions.None);
                        if (postBackIds.LongLength > 1) {
                            postBackId = postBackIds[0];
                            foreach (string id in postBackIds) {
                                if (id.StartsWith("R-")) {
                                    postBackId = id;
                                    break;
                                } else if ("N" == id) {
                                    postBackId = id;
                                }
                            }
                        }
                        bool isNewFromPostBack;
                        IPresentableObject postBackObject = null;
                        int previousIndex = -1;
                        if (this.ViewPane.HasButtonForAddingNewObjects && "N" == postBackId) { // initial new object
                            isNewFromPostBack = true;
                            postBackObject = presentableField.NewItemAsObject() as IPresentableObject;
                            previousIndex = presentableField.Count;
                            presentableField.AddObject(postBackObject);
                        } else if (this.ViewPane.HasButtonForAddingNewObjects && this.ViewPane.HasButtonsForRemovingObjects && "R-N" == postBackId) { // removal of new object
                            isNewFromPostBack = true;
                            postBackObject = presentableField.NewItemAsObject() as IPresentableObject;
                            postBackObject.RemoveOnUpdate = RemovalType.RemoveCascadedly;
                            previousIndex = presentableField.Count;
                        } else if ("P" == postBackId) { // round-tripped new object
                            isNewFromPostBack = false;
                            int indexOfPreparedObject = 0;
                            int j = 0;
                            foreach (var currentObject in presentableField.GetValuesAsObject()) {
                                var current = currentObject as IPresentableObject;
                                if (current.IsNew) {
                                    indexOfPreparedObject++;
                                    if (indexOfPreparedObject > highestIndexOfPreparedObjects) {
                                        postBackObject = current;
                                        previousIndex = j;
                                        highestIndexOfPreparedObjects = indexOfPreparedObject;
                                        break;
                                    }
                                }
                                j++;
                            }
                        } else {
                            isNewFromPostBack = false;
                            bool isRemoval = postBackId.StartsWith("R-");
                            if (isRemoval) {
                                postBackId = postBackId.Substring(2);
                            }
                            if (Guid.TryParse(postBackId, out Guid postBackGuid)) {
                                int j = 0;
                                foreach (var currentObject in presentableField.GetValuesAsObject()) {
                                    var current = currentObject as IPresentableObject;
                                    if (current.Id == postBackGuid) {
                                        if (this.ViewPane.HasButtonsForRemovingObjects && isRemoval) { // removal of existing object
                                            current.RemoveOnUpdate = RemovalType.RemoveCascadedly;
                                        } else { // potential update of object
                                            previousIndex = j;
                                        }
                                        postBackObject = current;
                                        break;
                                    }
                                    j++;
                                }
                            }
                        }
                        if (null != postBackObject) {
                            var order = httpRequest.Form[this.ClientFieldIdPrefix + this.ClientFieldIdSuffix + "_" + i + "::"];
                            if (!string.IsNullOrEmpty(order) && long.TryParse(order, out long parsedOrder)) {
                                sortSequence[postBackObject.Id] = parsedOrder;
                            }
                            this.AddSectionForPresentableObject(postBackObject, viewSection, "_" + i, isNewFromPostBack);
                            if (i < previousIndex) {
                                presentableField.Swap(i, previousIndex);
                            }
                        }
                    }
                    i++;
                } while (!string.IsNullOrEmpty(postBackId));
                if (sortSequence.Count > 0) {
                    presentableField.SortObjects(delegate (object x, object y) {
                        int result;
                        var xId = ((IPresentableObject)x).Id;
                        var yId = ((IPresentableObject)y).Id;
                        bool isXContainedInSortSequence = sortSequence.ContainsKey(xId);
                        bool isYContainedInSortSequence = sortSequence.ContainsKey(yId);
                        if (isXContainedInSortSequence && isYContainedInSortSequence) {
                            result = sortSequence[xId].CompareTo(sortSequence[yId]);
                        } else if (isXContainedInSortSequence) {
                            result = 1;
                        } else if (isYContainedInSortSequence) {
                            result = -1;
                        } else {
                            return 1;
                        }
                        return result;
                    });
                }
            }
            return;
        }

        /// <summary>
        /// Creates the child controls for all sections that belong
        /// to the presentable field, but were not included in the
        /// post-back.
        /// </summary>
        /// <param name="presentableField">presentable field to
        /// create sections for</param>
        /// <param name="viewSection">view to use for new sections</param>
        private void CreateChildControlsForNonPostBackSections(IPresentableFieldForCollection presentableField, ViewPaneWithTitle viewSection) {
            var i = this.FormSections.Count;
            foreach (var currentObject in presentableField.GetValuesAsObject()) {
                var current = currentObject as IPresentableObject;
                this.AddSectionForPresentableObject(current, viewSection, "_" + i, false);
                i++;
            }
            return;
        }

        /// <summary>
        /// Gets a value indicating whether control is supposed to be
        /// rendered.
        /// </summary>
        /// <returns>true if control is supposed to be rendered,
        /// false otherwise</returns>
        protected override bool GetIsVisible() {
            return this.IsVisible;
        }

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        internal sealed override void SetHasValidValue() {
            this.SetHasValidValue(this.FormSections);
            return;
        }

    }

}