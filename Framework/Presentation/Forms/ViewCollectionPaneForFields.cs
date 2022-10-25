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

    using Framework.Persistence.Fields;
    using Properties;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Pane for combining view fields of multiple objects in a
    /// container - the user can switch between the objects.
    /// </summary>
    public class ViewCollectionPaneForFields : ViewCollectionPane {

        /// <summary>
        /// Type of group to dispay.
        /// </summary>
        public SectionGroupType SectionGroupType {
            get { return (SectionGroupType)this.sectionGroupType.Value; }
            set {
                this.sectionGroupType.Value = (int)value;
                if (SectionGroupType.Table == value) {
                    this.IsSortable = false;
                }
            }
        }
        private readonly PersistentFieldForInt sectionGroupType =
            new PersistentFieldForInt(nameof(SectionGroupType), 0);

        /// <summary>
        /// List of view fields contained in this pane.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<ViewField> ViewFields { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewCollectionPaneForFields()
            : base() {
            this.RegisterPersistentField(this.sectionGroupType);
            this.ViewFields = new PersistentFieldForPersistentObjectCollection<ViewField>(nameof(ViewFields), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.ViewFields);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="sectionGroupType">type of group to dispay</param>
        /// <param name="hasButtonsForAddingAndRemovingObjects">true
        /// if user is allowed to add and remove objects, false otherwise</param>
        private ViewCollectionPaneForFields(SectionGroupType sectionGroupType, bool hasButtonsForAddingAndRemovingObjects)
            : this() {
            this.SectionGroupType = sectionGroupType;
            this.Initialize(hasButtonsForAddingAndRemovingObjects);
            if (SectionGroupType.Table == this.SectionGroupType) {
                this.ConfirmationMessageForRemoval = Resources.WouldYouReallyLikeToDeleteTheRow;
            }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="titleField">internal key of field containing
        /// the title</param>
        /// <param name="key">internal key of field containing the
        /// collection of child objects to view - this may not be
        /// null or empty</param>
        /// <param name="sectionGroupType">type of group to dispay</param>
        /// <param name="hasButtonsForAddingAndRemovingObjects">true
        /// if user is allowed to add and remove objects, false otherwise</param>
        public ViewCollectionPaneForFields(string titleField, string key, SectionGroupType sectionGroupType, bool hasButtonsForAddingAndRemovingObjects)
            : this(sectionGroupType, hasButtonsForAddingAndRemovingObjects) {
            this.Key = key;
            this.TitleField = titleField;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="titleFieldChain">internal chain of keys of
        /// field containing the title</param>
        /// <param name="key">internal key of field containing the
        /// collection of child objects to view - this may not be
        /// null or empty</param>
        /// <param name="sectionGroupType">type of group to dispay</param>
        /// <param name="hasButtonsForAddingAndRemovingObjects">true
        /// if user is allowed to add and remove objects, false otherwise</param>
        public ViewCollectionPaneForFields(IEnumerable<string> titleFieldChain, string key, SectionGroupType sectionGroupType, bool hasButtonsForAddingAndRemovingObjects)
            : this(sectionGroupType, hasButtonsForAddingAndRemovingObjects) {
            this.Key = key;
            this.TitleFieldChain = titleFieldChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="titleField">internal key of field containing
        /// the title</param>
        /// <param name="keyChain">internal key chain of field
        /// containing the collection of child objects to view - this
        /// may not be null or empty</param>
        /// <param name="sectionGroupType">type of group to dispay</param>
        /// <param name="hasButtonsForAddingAndRemovingObjects">true
        /// if user is allowed to add and remove objects, false otherwise</param>
        public ViewCollectionPaneForFields(string titleField, string[] keyChain, SectionGroupType sectionGroupType, bool hasButtonsForAddingAndRemovingObjects)
            : this(sectionGroupType, hasButtonsForAddingAndRemovingObjects) {
            this.KeyChain = keyChain;
            this.TitleField = titleField;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="titleFieldChain">internal chain of keys of
        /// field containing the title</param>
        /// <param name="keyChain">internal key chain of field
        /// containing the collection of child objects to view - this
        /// may not be null or empty</param>
        /// <param name="sectionGroupType">type of group to dispay</param>
        /// <param name="hasButtonsForAddingAndRemovingObjects">true
        /// if user is allowed to add and remove objects, false otherwise</param>
        public ViewCollectionPaneForFields(IEnumerable<string> titleFieldChain, string[] keyChain, SectionGroupType sectionGroupType, bool hasButtonsForAddingAndRemovingObjects)
            : this(sectionGroupType, hasButtonsForAddingAndRemovingObjects) {
            this.KeyChain = keyChain;
            this.TitleFieldChain = titleFieldChain;
        }

        /// <summary>
        /// Finds the view fields for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find view fields for</param>
        /// <returns>view fields for specific key chain</returns>
        public override ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string[] keyChain) {
            return ViewPane.FindViewFields(keyChain, this.ViewFields);
        }

        /// <summary>
        /// Gets all view fields for a presentable object and all
        /// child objects as a flattened enumerable. All key chains
        /// are converted to be relative to topmost presenable
        /// object.
        /// </summary>
        /// <param name="keyChainPath">key chain path to topmost
        /// presentable object</param>
        /// <param name="isParentVisible">true if parent is visible,
        /// false otherwise</param>
        /// <returns>flattened view fields for presentable object
        /// with converted key chains</returns>
        internal override IEnumerable<ViewField> GetViewFieldsCascadedly(string[] keyChainPath, bool isParentVisible) {
            return base.GetViewFieldsCascadedly(keyChainPath, isParentVisible, this.ViewFields);
        }

        /// <summary>
        /// Determines whether this view pane is read-only for a
        /// specific presentable object.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// check</param>
        /// <returns>true if this form view is read-only for the
        /// given presentable object, false otherwise</returns>
        internal override bool IsReadOnlyFor(IPresentableObject presentableObject) {
            bool isReadOnly = true;
            foreach (var viewObject in this.FindPresentableChildObjects(presentableObject, this.HasButtonForAddingNewObjects)) {
                if (!ViewPane.IsReadOnlyFor(viewObject, this.ViewFields)) {
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
        /// <param name="topmostPresentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>true if the presentable object is valid, false
        /// otherwise</returns>
        public override bool IsValidValue(IPresentableObject presentableObject, ValidityCheck validityCheck, IPresentableObject topmostPresentableObject, IOptionDataProvider optionDataProvider) {
            bool isValidValue = true;
            foreach (var viewObject in this.FindPresentableChildObjects(presentableObject, false)) {
                if (!ViewPane.IsValidValue(viewObject, this.ViewFields, validityCheck, topmostPresentableObject, optionDataProvider)) {
                    isValidValue = false;
                    break;
                }
            }
            return isValidValue;
        }

        /// <summary>
        /// Sets all desired and required fields of view pane to the
        /// specified manditoriness.
        /// </summary>
        /// <param name="manditoriness">manditoriness to set for all
        /// desired and required fields of view pane</param>
        public override void SetMandatoriness(Mandatoriness manditoriness) {
            ViewPane.SetMandatoriness(manditoriness, this.ViewFields);
            return;
        }

        /// <summary>
        /// Sets all fields of view pane to be read-only.
        /// </summary>
        public override void SetReadOnly() {
            ViewPane.SetReadOnly(this.ViewFields);
            return;
        }

        /// <summary>
        /// Converts this view collection pane to a view pane with
        /// title.
        /// </summary>
        /// <returns>view collection pane converted to a view pane
        /// with title</returns>
        public override ViewPaneWithTitle ToViewPaneWithTitle() {
            ViewPaneForFields viewPane = new ViewPaneForFields();
            viewPane.Key = null;
            viewPane.Title = null;
            viewPane.ViewFields.AddRange(this.ViewFields);
            return viewPane;
        }

    }

}