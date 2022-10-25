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
    using Framework.Properties;

    /// <summary>
    /// Editable field to be presented in a view.
    /// </summary>
    public abstract class ViewFieldForEditableValue : ViewField {

        /// <summary>
        /// Description of this field that will be shown
        /// when it is rendered in edit mode.
        /// </summary>
        public string DescriptionForEditMode {
            get { return this.descriptionForEditMode.Value; }
            set { this.descriptionForEditMode.Value = value; }
        }
        private readonly PersistentFieldForString descriptionForEditMode =
            new PersistentFieldForString(nameof(DescriptionForEditMode));

        /// <summary>
        /// Description of this field that will be shown
        /// when it is rendered in view mode (read-only).
        /// </summary>
        public string DescriptionForViewMode {
            get { return this.descriptionForViewMode.Value; }
            set { this.descriptionForViewMode.Value = value; }
        }
        private readonly PersistentFieldForString descriptionForViewMode =
            new PersistentFieldForString(nameof(DescriptionForViewMode));

        /// <summary>
        /// Value indicating whether to focus this field as the first
        /// one.
        /// </summary>
        public bool IsAutofocused {
            get { return this.isAutofocused.Value; }
            set { this.isAutofocused.Value = value; }
        }
        private readonly PersistentFieldForBool isAutofocused =
            new PersistentFieldForBool(nameof(IsAutofocused), false);

        /// <summary>
        /// Gets a value indicating whether this field is read-only.
        /// If this property is not set to read-only but the
        /// connected presentable data field is, this field will be
        /// rendered as read-only anyway.
        /// </summary>
        public bool IsReadOnly {
            get { return this.isReadOnly.Value; }
            set { this.isReadOnly.Value = value; }
        }
        private readonly PersistentFieldForBool isReadOnly =
            new PersistentFieldForBool(nameof(IsReadOnly), true);

        /// <summary>
        /// Internal key of this field.
        /// </summary>
        public string Key {
            get { return this.key.Value; }
            set { this.key.Value = value; }
        }
        private readonly PersistentFieldForString key =
            new PersistentFieldForString("FieldKey");

        /// <summary>
        /// Internal key chain of this field.
        /// </summary>
        public string[] KeyChain {
            get { return Model.KeyChain.FromKey(this.Key); }
            set { this.Key = Model.KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Gets a value indicating whether it is required to enter a
        /// value in this field.
        /// </summary>
        public Mandatoriness Mandatoriness {
            get { return (Mandatoriness)this.mandatoriness.Value; }
            set { this.mandatoriness.Value = (int)value; }
        }
        private readonly PersistentFieldForInt mandatoriness =
            new PersistentFieldForInt(nameof(Mandatoriness), (int)Mandatoriness.Optional);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForEditableValue()
            : base() {
            this.IsReadOnly = false;
            this.descriptionForEditMode.PreviousKeys.Add("Description"); // TODO: remove after every application has been migrated
            this.RegisterPersistentField(this.descriptionForEditMode);
            this.RegisterPersistentField(this.descriptionForViewMode);
            this.RegisterPersistentField(this.isAutofocused);
            this.RegisterPersistentField(this.isReadOnly);
            this.RegisterPersistentField(this.key);
            this.RegisterPersistentField(this.mandatoriness);
        }

        /// <summary>
        /// Gets the default error message.
        /// </summary>
        /// <returns>default error message</returns>
        public virtual string GetDefaultErrorMessage() {
            var errorMessage = Resources.PleaseEnterAValidValueForThisField;
            errorMessage += " " + this.GetInfoMessageAboutManditoriness();
            return errorMessage;
        }

        /// <summary>
        /// Gets an info message about manditoriness of field.
        /// </summary>
        /// <returns>info message about manditoriness of field</returns>
        protected string GetInfoMessageAboutManditoriness() {
            string infoMessage;
            if (Mandatoriness.Required == this.Mandatoriness) {
                infoMessage = Resources.ThisIsAMandatoryField;
            } else if (Mandatoriness.Desired == this.Mandatoriness) {
                infoMessage = Resources.AlternativelyYouCanLeaveThisFieldBlankForNow;
            } else {
                infoMessage = Resources.AlternativelyYouCanLeaveThisFieldBlank;
            }

            return infoMessage;
        }

    }

}