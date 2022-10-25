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

    /// <summary>
    /// Pane for combining view fields or panes of multiple objects
    /// in a container - the user can switch between the objects.
    /// </summary>
    public abstract class ViewCollectionPane : ViewPane {

        /// <summary>
        /// True to add the first section of empty panes
        /// automatically, false otherwise.
        /// </summary>
        public bool AutoAddFirstSection {
            get { return this.autoAddFirstSection.Value; }
            set { this.autoAddFirstSection.Value = value; }
        }
        private readonly PersistentFieldForBool autoAddFirstSection =
            new PersistentFieldForBool(nameof(AutoAddFirstSection));

        /// <summary>
        /// Confirmation message to display on removal of objects.
        /// </summary>
        public string ConfirmationMessageForRemoval {
            get { return this.confirmationMessageForRemoval.Value; }
            set { this.confirmationMessageForRemoval.Value = value; }
        }
        private readonly PersistentFieldForString confirmationMessageForRemoval =
            new PersistentFieldForString(nameof(ConfirmationMessageForRemoval));

        /// <summary>
        /// True if the user is allowed to add new objects, false
        /// otherwise.
        /// </summary>
        public bool HasButtonForAddingNewObjects {
            get { return this.hasButtonForAddingNewObjects.Value; }
            set { this.hasButtonForAddingNewObjects.Value = value; }
        }
        private readonly PersistentFieldForBool hasButtonForAddingNewObjects =
            new PersistentFieldForBool(nameof(HasButtonForAddingNewObjects));

        /// <summary>
        /// True if user is allowed to remove objects, false otherwise.
        /// </summary>
        public bool HasButtonsForRemovingObjects {
            get { return this.hasButtonsForRemovingObjects.Value; }
            set { this.hasButtonsForRemovingObjects.Value = value; }
        }
        private readonly PersistentFieldForBool hasButtonsForRemovingObjects =
            new PersistentFieldForBool(nameof(HasButtonsForRemovingObjects));

        /// <summary>
        /// Indicates whether view collection pane is sortable.
        /// </summary>
        public bool IsSortable {
            get { return this.isSortable.Value; }
            set { this.isSortable.Value = value; }
        }
        private readonly PersistentFieldForBool isSortable =
            new PersistentFieldForBool(nameof(IsSortable), true);

        /// <summary>
        /// Placeholder text to be shown if no section exists.
        /// </summary>
        public string Placeholder {
            get { return this.placeholder.Value; }
            set { this.placeholder.Value = value; }
        }
        private readonly PersistentFieldForString placeholder =
            new PersistentFieldForString(nameof(Placeholder));

        /// <summary>
        /// Internal key of field containing the title.
        /// </summary>
        public string TitleField {
            get { return this.titleField.Value; }
            set { this.titleField.Value = value; }
        }
        private readonly PersistentFieldForString titleField =
            new PersistentFieldForString(nameof(TitleField));

        /// <summary>
        /// Internal chain of keys of field containing the title.
        /// </summary>
        public IEnumerable<string> TitleFieldChain {
            get { return Model.KeyChain.FromKey(this.TitleField); }
            set { this.TitleField = Model.KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewCollectionPane()
            : base() {
            this.RegisterPersistentField(this.autoAddFirstSection);
            this.RegisterPersistentField(this.confirmationMessageForRemoval);
            this.RegisterPersistentField(this.hasButtonForAddingNewObjects);
            this.RegisterPersistentField(this.hasButtonsForRemovingObjects);
            this.RegisterPersistentField(this.isSortable);
            this.RegisterPersistentField(this.placeholder);
            this.RegisterPersistentField(this.titleField);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="hasButtonsForAddingAndRemovingObjects">true if user
        /// is allowed to add and remove objects, false otherwise</param>
        protected void Initialize(bool hasButtonsForAddingAndRemovingObjects) {
            this.AutoAddFirstSection = hasButtonsForAddingAndRemovingObjects;
            this.ConfirmationMessageForRemoval = Resources.WouldYouReallyLikeToDeleteTheTab;
            this.HasButtonForAddingNewObjects = hasButtonsForAddingAndRemovingObjects;
            this.HasButtonsForRemovingObjects = hasButtonsForAddingAndRemovingObjects;
            return;
        }

        /// <summary>
        /// Converts this view collection pane to a view pane with
        /// title.
        /// </summary>
        /// <returns>view collection pane converted to a view pane
        /// with title</returns>
        public abstract ViewPaneWithTitle ToViewPaneWithTitle();

    }

}