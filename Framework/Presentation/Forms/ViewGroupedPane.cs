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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Pane for combining view fields or panes for one object in a
    /// dynamic container - the user can switch between which of the
    /// contained sections of view panes to display.
    /// </summary>
    public class ViewGroupedPane : ViewPane {

        /// <summary>
        /// Type of group to dispay.
        /// </summary>
        public SectionGroupType SectionGroupType {
            get { return (SectionGroupType)this.sectionGroupType.Value; }
            set { this.sectionGroupType.Value = (int)value; }
        }
        private readonly PersistentFieldForInt sectionGroupType =
            new PersistentFieldForInt(nameof(SectionGroupType), 0);

        /// <summary>
        /// List of sections of this grouped pane.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<ViewPaneWithTitle> Sections { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewGroupedPane()
            : base() {
            this.RegisterPersistentField(this.sectionGroupType);
            this.Sections = new PersistentFieldForPersistentObjectCollection<ViewPaneWithTitle>(nameof(Sections), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.Sections);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="key">internal key of field containing child
        /// object to view - if the key is null or empty the parent
        /// object will be viewed</param>
        public ViewGroupedPane(string key)
            : this() {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="keyChain">internal key chain of field
        /// containing child object to view - if the key chain is
        /// null or empty the parent object will be viewed</param>
        public ViewGroupedPane(string[] keyChain)
            : this() {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Finds the view fields for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find view fields for</param>
        /// <returns>view fields for specific key chain</returns>
        public override ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string[] keyChain) {
            return ViewPane.FindViewFields(keyChain, this.Sections);
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
            return base.GetViewFieldsCascadedly(keyChainPath, isParentVisible, this.Sections);
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
            IPresentableObject viewObject = this.FindPresentableChildObject(presentableObject);
            return ViewPane.IsReadOnlyFor(viewObject, this.Sections);
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
            IPresentableObject viewObject = this.FindPresentableChildObject(presentableObject);
            return ViewPane.IsValidValue(viewObject, this.Sections, validityCheck, topmostPresentableObject, optionDataProvider);
        }

        /// <summary>
        /// Sets all desired and required fields of view pane to the
        /// specified manditoriness.
        /// </summary>
        /// <param name="manditoriness">manditoriness to set for all
        /// desired and required fields of view pane</param>
        public override void SetMandatoriness(Mandatoriness manditoriness) {
            ViewPane.SetMandatoriness(manditoriness, this.Sections);
            return;
        }

        /// <summary>
        /// Sets all fields of view pane to be read-only.
        /// </summary>
        public override void SetReadOnly() {
            ViewPane.SetReadOnly(this.Sections);
            return;
        }

    }

}