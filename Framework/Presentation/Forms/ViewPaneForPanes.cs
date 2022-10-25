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
    /// Pane for combining view panes of one object in a static
    /// container - this is used for grouping view panes visually.
    /// </summary>
    public class ViewPaneForPanes : ViewPaneWithTitle {

        /// <summary>
        /// List of view panes contained in this section.
        /// </summary>
        public PersistentFieldForPersistentObjectCollection<ViewPane> ViewPanes { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewPaneForPanes()
            : base() {
            this.ViewPanes = new PersistentFieldForPersistentObjectCollection<ViewPane>(nameof(ViewPanes), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);
            this.RegisterPersistentField(this.ViewPanes);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of this pane</param>
        public ViewPaneForPanes(string title)
            : this() {
            this.Title = title;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of this pane</param>
        /// <param name="key">internal key of field containing child
        /// object to view - if the key is null or empty the parent
        /// object will be viewed</param>
        public ViewPaneForPanes(string title, string key)
            : this(title) {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display title of this pane</param>
        /// <param name="keyChain">internal key chain of field
        /// containing child object to view - if the key chain is
        /// null or empty the parent object will be viewed</param>
        public ViewPaneForPanes(string title, string[] keyChain)
            : this(title) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Finds the view fields for a specific key chain.
        /// </summary>
        /// <param name="keyChain">key chain to find view fields for</param>
        /// <returns>view fields for specific key chain</returns>
        public override ReadOnlyCollection<ViewFieldForEditableValue> FindViewFields(string[] keyChain) {
            return ViewPane.FindViewFields(keyChain, this.ViewPanes);
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
            return base.GetViewFieldsCascadedly(keyChainPath, isParentVisible, this.ViewPanes);
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
            return ViewPane.IsReadOnlyFor(viewObject, this.ViewPanes);
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
            return ViewPane.IsValidValue(viewObject, this.ViewPanes, validityCheck, topmostPresentableObject, optionDataProvider);
        }

        /// <summary>
        /// Sets all desired and required fields of view pane to the
        /// specified manditoriness.
        /// </summary>
        /// <param name="manditoriness">manditoriness to set for all
        /// desired and required fields of view pane</param>
        public override void SetMandatoriness(Mandatoriness manditoriness) {
            ViewPane.SetMandatoriness(manditoriness, this.ViewPanes);
            return;
        }

        /// <summary>
        /// Sets all fields of view pane to be read-only.
        /// </summary>
        public override void SetReadOnly() {
            ViewPane.SetReadOnly(this.ViewPanes);
            return;
        }

    }

}