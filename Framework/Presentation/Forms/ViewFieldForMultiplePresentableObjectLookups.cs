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

    /// <summary>
    /// Field for collection of presentable object lookup values to
    /// be presented in a view.
    /// </summary>
    public class ViewFieldForMultiplePresentableObjectLookups : ViewFieldForMultipleLookups, IClickableViewFieldWithLookupProvider {

        /// <summary>
        /// Provider for suggested values.
        /// </summary>
        public PresentableObjectLookupProvider LookupProvider {
            get { return this.lookupProvider.Value; }
            set { this.lookupProvider.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<PresentableObjectLookupProvider> lookupProvider =
            new PersistentFieldForPersistentObject<PresentableObjectLookupProvider>(nameof(LookupProvider), CascadedRemovalBehavior.RemoveValuesIfTheyAreNotReferenced);

        /// <summary>
        /// Delegate to get URL links for presentable objects. The
        /// assigned delegate won't be persisted.
        /// </summary>
        public OnClickUrlDelegate OnClickUrlDelegate { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForMultiplePresentableObjectLookups()
            : base() {
            this.RegisterPersistentField(this.lookupProvider);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForMultiplePresentableObjectLookups(string title, string key, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.Key = key;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="keyChain">internal key chain of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForMultiplePresentableObjectLookups(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForMultiplePresentableObjectLookups(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

        /// <summary>
        /// Gets the provider for suggested values.
        /// </summary>
        public sealed override LookupProvider GetLookupProvider() {
            return this.LookupProvider;
        }

        /// <summary>
        /// Gets the read-only value of a presentable field.
        /// </summary>
        /// <param name="presentableField">presentable field to get
        /// read-only value of</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to get read-only value of</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>read-only value of presentable field</returns>
        public override IEnumerable<string> GetReadOnlyValuesFor(IPresentableFieldForCollection presentableField, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            foreach (var key in presentableField.GetValuesAsObject()) {
                yield return this.LookupProvider.FindValueForKey(key as IPresentableObject, presentableObject, optionDataProvider);
            }
        }

        /// <summary>
        /// Returns true if the specified keys are valid, false
        /// otherwise.
        /// </summary>
        /// <param name="presentableField">presentable field to be
        /// validated</param>
        /// <param name="validityCheck">type of validity check to
        /// apply</param>
        /// <param name="presentableObject">topmost presentable
        /// parent object to build form for</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <returns>null if the specified value is valid, error
        /// message otherwise</returns>
        public override string Validate(IPresentableFieldForCollection presentableField, ValidityCheck validityCheck, IPresentableObject presentableObject, IOptionDataProvider optionDataProvider) {
            var viewFieldForElement = new ViewFieldForPresentableObjectLookup(this.Key, this.Title, Mandatoriness.Optional) {
                DescriptionForEditMode = this.DescriptionForEditMode,
                DescriptionForViewMode = this.DescriptionForViewMode,
                LookupProvider = this.LookupProvider,
                Placeholder = this.Placeholder
            };
            return base.Validate(presentableField, validityCheck, presentableObject, optionDataProvider, viewFieldForElement);
        }

    }

}