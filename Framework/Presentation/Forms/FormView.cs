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

namespace Framework.Presentation.Forms {

    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Model;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a form view for a single element.
    /// </summary>
    public class FormView : PersistentObject {

        /// <summary>
        /// Description that will be shown for the form when it is in
        /// edit mode.
        /// </summary>
        public string DescriptionForEditMode {
            get { return this.descriptionForEditMode.Value; }
            set { this.descriptionForEditMode.Value = value; }
        }
        private readonly PersistentFieldForString descriptionForEditMode =
            new PersistentFieldForString(nameof(DescriptionForEditMode));

        /// <summary>
        /// Description that will be shown for the form when it is in
        /// view mode.
        /// </summary>
        public string DescriptionForViewMode {
            get { return this.descriptionForViewMode.Value; }
            set { this.descriptionForViewMode.Value = value; }
        }
        private readonly PersistentFieldForString descriptionForViewMode =
            new PersistentFieldForString(nameof(DescriptionForViewMode));

        /// <summary>
        /// Indicates whether autocompletion is enabled for form.
        /// </summary>
        public bool HasAutocompletionEnabled {
            get { return this.hasAutocompletionEnabled.Value; }
            set { this.hasAutocompletionEnabled.Value = value; }
        }
        private readonly PersistentFieldForBool hasAutocompletionEnabled =
            new PersistentFieldForBool(nameof(HasAutocompletionEnabled), false);

        /// <summary>
        /// True if this form has a paragraph for created at, created
        /// by, modified at and modified by.
        /// </summary>
        public bool HasModificationInfo {
            get { return this.hasModificationInfo.Value; }
            set { this.hasModificationInfo.Value = value; }
        }
        private readonly PersistentFieldForBool hasModificationInfo =
            new PersistentFieldForBool(nameof(HasModificationInfo), true);

        /// <summary>
        /// List of view panes contained in this view.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<ViewPane> ViewPanes { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public FormView()
            : base() {
            this.RegisterPersistentField(this.descriptionForEditMode);
            this.descriptionForViewMode.PreviousKeys.Add("Description"); // TODO: Remove this after all applications have been migrated
            this.RegisterPersistentField(this.descriptionForViewMode);
            this.RegisterPersistentField(this.hasAutocompletionEnabled);
            this.RegisterPersistentField(this.hasModificationInfo);
            this.ViewPanes = new PersistentFieldForPersistentObjectCollection<ViewPane>(nameof(this.ViewPanes), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.ViewPanes);
        }

        /// <summary>
        /// Copies the state of another instance of this type
        /// into this instance.
        /// </summary>
        /// <param name="source">source instance to copy state from</param>
        /// <param name="copyBehaviorForAllowedGroups">determines how
        /// to copy allowed groups</param>
        /// <param name="copyBehaviorForAggregations">determines how
        /// to copy child objects that are not "owned" by their
        /// parent object</param>
        /// <param name="copyBehaviorForCompositions">determines how
        /// to copy child objects that are "owned" by their parent
        /// object</param>
        public virtual void CopyFrom(FormView source, CopyBehaviorForAllowedGroups copyBehaviorForAllowedGroups, CopyBehaviorForAggregations copyBehaviorForAggregations, CopyBehaviorForCompositions copyBehaviorForCompositions) {
            base.CopyFrom(source, copyBehaviorForAllowedGroups, copyBehaviorForAggregations, copyBehaviorForCompositions);
            foreach (var sourceViewField in source.GetViewFieldsCascadedly()) {
                if (sourceViewField is IClickableViewFieldWithLookupProvider clickableSourceViewFieldWithLookupProvider && null != clickableSourceViewFieldWithLookupProvider.OnClickUrlDelegate) {
                    var clickableTargetViewFieldWithLookupProvider = this.FindOneViewField(clickableSourceViewFieldWithLookupProvider.KeyChain) as IClickableViewFieldWithLookupProvider;
                    if (null != clickableTargetViewFieldWithLookupProvider) {
                        clickableTargetViewFieldWithLookupProvider.OnClickUrlDelegate = clickableSourceViewFieldWithLookupProvider.OnClickUrlDelegate;
                    }
                }
                if (sourceViewField is IClickableViewFieldWithOptionProvider clickableSourceViewFieldWithOptionProvider && null != clickableSourceViewFieldWithOptionProvider.OnClickUrlDelegate) {
                    var clickableTargetViewFieldWithOptionProvider = this.FindOneViewField(clickableSourceViewFieldWithOptionProvider.KeyChain) as IClickableViewFieldWithOptionProvider;
                    if (null != clickableTargetViewFieldWithOptionProvider) {
                        clickableTargetViewFieldWithOptionProvider.OnClickUrlDelegate = clickableSourceViewFieldWithOptionProvider.OnClickUrlDelegate;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Finds the first view field for a specific key.
        /// </summary>
        /// <param name="key">key to find first view field for</param>
        /// <returns>first view field for specific key or null</returns>
        public ViewFieldForEditableValue FindOneViewField(string key) {
            return this.FindOneViewField(KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds the first view field for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find first view field
        /// for</param>
        /// <returns>first view field for specific key chain or null</returns>
        public ViewFieldForEditableValue FindOneViewField(string[] keyChain) {
            ViewFieldForEditableValue viewField;
            var viewFields = this.FindViewFields(keyChain);
            if (viewFields.Count > 0) {
                viewField = viewFields[0];
            } else {
                viewField = null;
            }
            return viewField;
        }

        /// <summary>
        /// Finds the view fields for a specific key.
        /// </summary>
        /// <param name="key">key to find view fields for</param>
        /// <returns>view fields for specific key</returns>
        public ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string key) {
            return this.FindViewFields(KeyChain.FromKey(key));
        }

        /// <summary>
        /// Finds the view fields for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find view fields for</param>
        /// <returns>view fields for specific key chain</returns>
        public ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string[] keyChain) {
            return ViewPane.FindViewFields(keyChain, this.ViewPanes);
        }

        /// <summary>
        /// Gets all view fields for a presentable object and all
        /// child objects as a flattened list. All key chains are
        /// converted to be relative to topmost presenable object.
        /// </summary>
        /// <returns>flattened view fields for presentable object
        /// with converted key chains</returns>
        public ReadOnlyCollection<ViewField> GetViewFieldsCascadedly() {
            var viewFields = new List<ViewField>();
            foreach (var viewPane in this.ViewPanes) {
                viewFields.AddRange(viewPane.GetViewFieldsCascadedly(new string[0], true));
            }
            return viewFields.AsReadOnly();
        }

        /// <summary>
        /// Determines whether this form view is read-only for a
        /// specific presentable object.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// check</param>
        /// <returns>true if this form view is read-only for the
        /// given presentable object, false otherwise</returns>
        public bool IsReadOnlyFor(IPresentableObject presentableObject) {
            bool isReadOnly = true;
            foreach (var viewPane in this.ViewPanes) {
                if (!viewPane.IsReadOnlyFor(presentableObject)) {
                    isReadOnly = false;
                    break;
                }
            }
            return isReadOnly;
        }

        /// <summary>
        /// Returns true if the specified presentable object is
        /// valid, false otherwise.
        /// </summary>
        /// <param name="presentableObject">presentable object to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>true if the specified key is valid, false
        /// otherwise</returns>
        public bool IsValidValue(IPresentableObject presentableObject, ValidityCheck validityCheck, IOptionDataProvider optionDataProvider) {
            bool isValidValue = true;
            foreach (var viewPane in this.ViewPanes) {
                if (!viewPane.IsValidValue(presentableObject, validityCheck, presentableObject, optionDataProvider)) {
                    isValidValue = false;
                    break;
                }
            }
            return isValidValue;
        }

        /// <summary>
        /// Sets all desired and required fields of view to the
        /// specified manditoriness.
        /// </summary>
        /// <param name="manditoriness">manditoriness to set for all
        /// desired and required fields of view</param>
        public void SetMandatoriness(Mandatoriness manditoriness) {
            foreach (var viewPane in this.ViewPanes) {
                viewPane.SetMandatoriness(manditoriness);
            }
            return;
        }

        /// <summary>
        /// Sets all fields of view to be read-only.
        /// </summary>
        public void SetReadOnly() {
            foreach (var viewPane in this.ViewPanes) {
                viewPane.SetReadOnly();
            }
            return;
        }

    }

}