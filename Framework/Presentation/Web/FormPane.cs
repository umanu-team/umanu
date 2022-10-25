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

    using Framework.Persistence;
    using Framework.Persistence.Filters;
    using Framework.Presentation.Forms;
    using Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class of controls for rendering panes for combining
    /// fields, view panes or collections of objects visually.
    /// </summary>
    public abstract class FormPane : CascadedControl {

        /// <summary>
        /// HTML attributes of tag of control.
        /// </summary>
        public Dictionary<string, string> Attributes { get; private set; }

        /// <summary>
        /// Suffix to add to the ID of each field on client side.
        /// </summary>
        public string ClientFieldIdPrefix { get; private set; }

        /// <summary>
        /// Suffix to add to the ID of each field on client side.
        /// </summary>
        public string ClientFieldIdSuffix { get; private set; }

        /// <summary>
        /// Point in time to compare form data of read-only fields
        /// to or null to not compare any data.
        /// </summary>
        public DateTime? ComparisonDate { get; private set; }

        /// <summary>
        /// Base directory for files.
        /// </summary>
        public string FileBaseDirectory { get; private set; }

        /// <summary>
        /// Factory for building form controls.
        /// </summary>
        public WebFactory FormFactory { get; private set; }

        /// <summary>
        /// True if this pane and all child panes have read-only
        /// fields only, false if at least one field is writable. Be
        /// aware: This property is supposed to be set during
        /// "CreateChildControls" and will be set to null in any
        /// earlier state.
        /// </summary>
        public bool? HasReadOnlyFieldsOnly { get; protected set; }

        /// <summary>
        /// True if all postback values of this pane are valid, false
        /// otherwise. Be aware: This property is supposed to be set
        /// during "CreateChildControls" and will be set to null in
        /// any earlier state.
        /// </summary>
        public bool? HasValidValue { get; protected set; }

        /// <summary>
        /// Indicates whether form pane is empty.
        /// </summary>
        public bool IsEmpty {
            get {
                bool isEmpty = true;
                foreach (var control in this.Controls) {
                    var formPane = control as FormPane;
                    if (null == formPane || !formPane.IsEmpty) {
                        isEmpty = false;
                        break;
                    }
                }
                return isEmpty;
            }
        }

        /// <summary>
        /// Internal key of field containing the child objects(s) to
        /// use.
        /// </summary>
        protected string Key { get; private set; }

        /// <summary>
        /// Post back state of the parent form.
        /// </summary>
        public PostBackState PostBackState { get; private set; }

        /// <summary>
        /// Presentable field of the presentable objects(s) to render
        /// form pane for or null if key is invalid or presentable
        /// object(s) is/are not set.
        /// </summary>
        private IPresentableField presentableFieldToRenderPaneFor;

        /// <summary>
        /// Presentable object to edit.
        /// </summary>
        public IPresentableObject PresentableObject { get; private set; }

        /// <summary>
        /// Presentable object to render form pane for or null if key
        /// is invalid or no presentable object is set.
        /// </summary>
        private IPresentableObject presentableObjectToRenderPaneFor;

        /// <summary>
        /// Topmost presentable parent object to build form for.
        /// </summary>
        public IPresentableObject TopmostParentPresentableObject { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="tagName">HTML tag name of this control</param>
        /// <param name="presentableObject">presentable object to
        /// show</param>
        /// <param name="key">internal key of field containing the
        /// child objects(s) to use</param>
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
        protected FormPane(string tagName, IPresentableObject presentableObject, string key, IPresentableObject topmostParentPresentableObject, DateTime? comparisonDate, string clientFieldIdPrefix, string clientFieldIdSuffix, PostBackState postBackState, string fileBaseDirectory, WebFactory formFactory)
            : base(tagName) {
            this.Attributes = new Dictionary<string, string>();
            this.ClientFieldIdPrefix = clientFieldIdPrefix;
            if (!string.IsNullOrEmpty(key)) {
                if (!string.IsNullOrEmpty(this.ClientFieldIdPrefix)) {
                    this.ClientFieldIdPrefix += '.';
                }
                this.ClientFieldIdPrefix += key;
            }
            this.ClientFieldIdSuffix = clientFieldIdSuffix;
            this.ComparisonDate = comparisonDate;
            this.FileBaseDirectory = fileBaseDirectory;
            this.FormFactory = formFactory;
            this.Key = key;
            this.PostBackState = postBackState;
            this.PresentableObject = presentableObject;
            this.TopmostParentPresentableObject = topmostParentPresentableObject;
        }

        /// <summary>
        /// Gets the HTML attributes of tag of control.
        /// </summary>
        /// <returns>HTML attributes of tag of control</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetAttributes() {
            foreach (var attribute in base.GetAttributes()) {
                yield return attribute;
            }
            foreach (var attribute in this.Attributes) {
                yield return attribute;
            }
        }

        /// <summary>
        /// Gets the presentable field of the presentable objects(s)
        /// to render form pane for or null if key is invalid or
        /// presentable object(s) is/are not set.
        /// </summary>
        protected IPresentableField GetPresentableFieldToRenderPaneFor() {
            if (null == this.presentableFieldToRenderPaneFor) {
                if (null == this.PresentableObject) {
                    if (!this.FormFactory.IgnoreMissingFields) {
                        throw new Framework.Presentation.Exceptions.PresentationException("Presentable object may not be null.");
                    }
                } else {
                    this.presentableFieldToRenderPaneFor = this.PresentableObject.FindPresentableField(this.Key);
                    if (null == this.presentableFieldToRenderPaneFor && !this.FormFactory.IgnoreMissingFields && this.IsFieldMissing(this.PresentableObject, KeyChain.FromKey(this.Key))) {
                        throw new Framework.Presentation.Exceptions.KeyNotFoundException("Presentable field for view field with key \"" + this.Key + "\" cannot be found.");
                    }
                }
            }
            return this.presentableFieldToRenderPaneFor;
        }

        /// <summary>
        /// Gets the presentable object to render form pane for or
        /// null if key is invalid or no presentable object is set.
        /// </summary>
        protected IPresentableObject GetPresentableObjectToRenderPaneFor() {
            if (null == this.presentableObjectToRenderPaneFor) {
                if (string.IsNullOrEmpty(this.Key)) {
                    this.presentableObjectToRenderPaneFor = this.PresentableObject;
                } else {
                    var presentableFieldToRenderPaneFor = this.GetPresentableFieldToRenderPaneFor();
                    if (presentableFieldToRenderPaneFor.IsForSingleElement) {
                        this.presentableObjectToRenderPaneFor = (presentableFieldToRenderPaneFor as IPresentableFieldForElement).ValueAsObject as IPresentableObject;
                    } else if (!this.FormFactory.IgnoreMissingFields) {
                        throw new Framework.Presentation.Exceptions.PresentationException("Presentable field for view field with key \"" + this.Key + "\" was expected to be for a single element, but it is for a collection.");
                    }
                }
            }
            return this.presentableObjectToRenderPaneFor;
        }

        /// <summary>
        /// Determines whether a field is missing in persistence
        /// mechanism regadless of permissions.
        /// </summary>
        /// <param name="presentableObject">presentable object which
        /// is supposed to have the field</param>
        /// <param name="keyChain">key chain of field to look for</param>
        /// <returns>true if field is missing regardless of
        /// permissions, false otherwise</returns>
        protected bool IsFieldMissing(IPresentableObject presentableObject, string[] keyChain) {
            IPresentableObject elevatedPresentableObject = null;
            if (presentableObject is PersistentObject) {
                var containsContainerMethod = this.FormFactory.OptionDataProvider.GetType().GetMethod(nameof(IOptionDataProvider.ContainsContainer));
                containsContainerMethod = containsContainerMethod.MakeGenericMethod(presentableObject.GetType());
                if ((bool)containsContainerMethod.Invoke(this.FormFactory.OptionDataProvider, new object[0])) {
                    var findOneMethod = this.FormFactory.OptionDataProvider.GetType().GetMethod(nameof(IOptionDataProvider.FindOne));
                    findOneMethod = findOneMethod.MakeGenericMethod(presentableObject.GetType());
                    var elevatedOptionDataProvider = this.FormFactory.OptionDataProvider.CopyWithElevatedPrivileges();
                    var filterCriteria = new FilterCriteria(nameof(IPresentableObject.Id), RelationalOperator.IsEqualTo, presentableObject.Id);
                    elevatedPresentableObject = findOneMethod.Invoke(elevatedOptionDataProvider, new object[] { filterCriteria, SortCriterionCollection.Empty }) as IPresentableObject;
                }
            }
            return null == elevatedPresentableObject || null == elevatedPresentableObject.FindPresentableField(keyChain);
        }

        /// <summary>
        /// Renders all child controls.
        /// </summary>
        /// <param name="html">HTML response</param>
        protected override void RenderChildControls(HtmlWriter html) {
            foreach (var control in this.Controls) {
                var formPane = control as FormPane;
                if (null == formPane || !formPane.IsEmpty) {
                    control.Render(html);
                }
            }
            return;
        }

        /// <summary>
        /// Set whether writable fields are contained in any child
        /// pane.
        /// </summary>
        /// <param name="formPanes">enumerable of all child form
        /// panes</param>
        protected void SetHasReadOnlyFieldsOnly(IEnumerable<FormPane> formPanes) {
            this.HasReadOnlyFieldsOnly = true;
            foreach (var formPane in formPanes) {
                if (false == formPane.HasReadOnlyFieldsOnly) {
                    this.HasReadOnlyFieldsOnly = false;
                    break;
                }
            }
            return;
        }

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        internal abstract void SetHasValidValue();

        /// <summary>
        /// Set validity state of post back values.
        /// </summary>
        /// <param name="formPanes">enumerable of all child pane
        /// objects</param>
        protected void SetHasValidValue(IEnumerable<FormPane> formPanes) {
            this.HasValidValue = true;
            foreach (var formPane in formPanes) {
                if (RemovalType.False == formPane.PresentableObject.RemoveOnUpdate) {
                    formPane.SetHasValidValue();
                    if (null == formPane.HasValidValue) {
                        this.HasValidValue = null;
                        break;
                    } else if (false == formPane.HasValidValue) {
                        this.HasValidValue = false;
                    }
                }
            }
            return;
        }

    }

}