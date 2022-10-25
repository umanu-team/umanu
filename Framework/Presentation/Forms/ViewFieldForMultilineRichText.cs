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

    using Persistence.Fields;

    /// <summary>
    /// Field for multiline rich text to be presented in a view.
    /// </summary>
    public class ViewFieldForMultilineRichText : ViewFieldForElement {

        /// <summary>
        /// Indicates whether hyperlinks are supposed to be
        /// autodetected or a button for inserting hyperlinks
        /// manually should be available instead.
        /// </summary>
        public AutomaticHyperlinkDetection AutomaticHyperlinkDetection {
            get { return (AutomaticHyperlinkDetection)this.automaticHyperlinkDetection.Value; }
            set { this.automaticHyperlinkDetection.Value = (int)value; }
        }
        private readonly PersistentFieldForInt automaticHyperlinkDetection =
            new PersistentFieldForInt(nameof(AutomaticHyperlinkDetection), (int)AutomaticHyperlinkDetection.IsEnabled);

        /// <summary>
        /// Indicates whether alignment buttons are supposed to be
        /// available.
        /// </summary>
        public bool HasAlignmentButtons {
            get { return this.hasAlignmentButtons.Value; }
            set { this.hasAlignmentButtons.Value = value; }
        }
        private readonly PersistentFieldForBool hasAlignmentButtons =
            new PersistentFieldForBool(nameof(HasAlignmentButtons), false);

        /// <summary>
        /// Indicates whether bold button is supposed to be
        /// available.
        /// </summary>
        public bool HasBoldButton {
            get { return this.hasBoldButton.Value; }
            set { this.hasBoldButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasBoldButton =
            new PersistentFieldForBool(nameof(HasBoldButton), true);

        /// <summary>
        /// Indicates whether bullets button is supposed to be
        /// available.
        /// </summary>
        public bool HasBulletsButton {
            get { return this.hasBulletsButton.Value; }
            set { this.hasBulletsButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasBulletsButton =
            new PersistentFieldForBool(nameof(HasBulletsButton), true);

        /// <summary>
        /// Indicates whether font size button is supposed to be
        /// available.
        /// </summary>
        public bool HasFontSizeButton {
            get { return this.hasFontSizeButton.Value; }
            set { this.hasFontSizeButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasFontSizeButton =
            new PersistentFieldForBool(nameof(HasFontSizeButton), false);

        /// <summary>
        /// Indicates whether image button is supposed to be
        /// available.
        /// </summary>
        public bool HasImageButton {
            get { return this.hasImageButton.Value; }
            set { this.hasImageButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasImageButton =
            new PersistentFieldForBool(nameof(HasImageButton), true);

        /// <summary>
        /// Indicates whether indent buttons are supposed to be
        /// available.
        /// </summary>
        public bool HasIndentButtons {
            get { return this.hasIndentButtons.Value; }
            set { this.hasIndentButtons.Value = value; }
        }
        private readonly PersistentFieldForBool hasIndentButtons =
            new PersistentFieldForBool(nameof(HasIndentButtons), true);

        /// <summary>
        /// Indicates whether italic button is supposed to be
        /// available.
        /// </summary>
        public bool HasItalicButton {
            get { return this.hasItalicButton.Value; }
            set { this.hasItalicButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasItalicButton =
            new PersistentFieldForBool(nameof(HasItalicButton), true);

        /// <summary>
        /// Indicates whether numbering button is supposed to be
        /// available.
        /// </summary>
        public bool HasNumberingButton {
            get { return this.hasNumberingButton.Value; }
            set { this.hasNumberingButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasNumberingButton =
            new PersistentFieldForBool(nameof(HasNumberingButton), true);

        /// <summary>
        /// Indicates whether remove format button is supposed to be
        /// available.
        /// </summary>
        public bool HasRemoveFormatButton {
            get { return this.hasRemoveFormatButton.Value; }
            set { this.hasRemoveFormatButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasRemoveFormatButton =
            new PersistentFieldForBool(nameof(HasRemoveFormatButton), true);

        /// <summary>
        /// Indicates whether strikethrough button is supposed to be
        /// available.
        /// </summary>
        public bool HasStrikethroughButton {
            get { return this.hasStrikethroughButton.Value; }
            set { this.hasStrikethroughButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasStrikethroughButton =
            new PersistentFieldForBool(nameof(HasStrikethroughButton), true);

        /// <summary>
        /// Indicates whether subscript button is supposed to be
        /// available.
        /// </summary>
        public bool HasSubscriptButton {
            get { return this.hasSubscriptButton.Value; }
            set { this.hasSubscriptButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasSubscriptButton =
            new PersistentFieldForBool(nameof(HasSubscriptButton), false);

        /// <summary>
        /// Indicates whether superscript button is supposed to be
        /// available.
        /// </summary>
        public bool HasSuperscriptButton {
            get { return this.hasSuperscriptButton.Value; }
            set { this.hasSuperscriptButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasSuperscriptButton =
            new PersistentFieldForBool(nameof(HasSuperscriptButton), false);

        /// <summary>
        /// Indicates whether table buttons are supposed to be
        /// available.
        /// </summary>
        public bool HasTableButtons {
            get { return this.hasTableButtons.Value; }
            set { this.hasTableButtons.Value = value; }
        }
        private readonly PersistentFieldForBool hasTableButtons =
            new PersistentFieldForBool(nameof(HasTableButtons), true);

        /// <summary>
        /// Indicates whether text color button is supposed to be
        /// available.
        /// </summary>
        public bool HasTextColorButton {
            get { return this.hasTextColorButton.Value; }
            set { this.hasTextColorButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasTextColorButton =
            new PersistentFieldForBool(nameof(HasTextColorButton), false);

        /// <summary>
        /// Indicates whether toggle full window button is supposed
        /// to be available.
        /// </summary>
        public bool HasToggleFullWindowButton {
            get { return this.hasToggleFullWindownButton.Value; }
            set { this.hasToggleFullWindownButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasToggleFullWindownButton =
            new PersistentFieldForBool(nameof(HasToggleFullWindowButton), true);

        /// <summary>
        /// Indicates whether underline button is supposed to be
        /// available.
        /// </summary>
        public bool HasUnderlineButton {
            get { return this.hasUnderlineButton.Value; }
            set { this.hasUnderlineButton.Value = value; }
        }
        private readonly PersistentFieldForBool hasUnderlineButton =
            new PersistentFieldForBool(nameof(HasUnderlineButton), true);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewFieldForMultilineRichText()
            : base() {
            this.RegisterPersistentField(this.automaticHyperlinkDetection);
            this.RegisterPersistentField(this.hasAlignmentButtons);
            this.RegisterPersistentField(this.hasBoldButton);
            this.RegisterPersistentField(this.hasBulletsButton);
            this.RegisterPersistentField(this.hasFontSizeButton);
            this.RegisterPersistentField(this.hasImageButton);
            this.RegisterPersistentField(this.hasIndentButtons);
            this.RegisterPersistentField(this.hasItalicButton);
            this.RegisterPersistentField(this.hasNumberingButton);
            this.RegisterPersistentField(this.hasRemoveFormatButton);
            this.RegisterPersistentField(this.hasStrikethroughButton);
            this.RegisterPersistentField(this.hasSubscriptButton);
            this.RegisterPersistentField(this.hasSuperscriptButton);
            this.RegisterPersistentField(this.hasTableButtons);
            this.RegisterPersistentField(this.hasTextColorButton);
            this.RegisterPersistentField(this.hasToggleFullWindownButton);
            this.RegisterPersistentField(this.hasUnderlineButton);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        private ViewFieldForMultilineRichText(string title, Mandatoriness mandatoriness)
            : this() {
            this.Mandatoriness = mandatoriness;
            this.Title = title;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        public ViewFieldForMultilineRichText(string title, string key, Mandatoriness mandatoriness)
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
        public ViewFieldForMultilineRichText(string title, string[] keyChain, Mandatoriness mandatoriness)
            : this(title, mandatoriness) {
            this.KeyChain = keyChain;
        }

        /// <summary>
        /// Creates a presentable field that can hold the value of
        /// view field.
        /// </summary>
        /// <param name="parentPresentableObject">parent presentable
        /// object of created field</param>
        /// <returns>presentable field that can hold the value of
        /// view field</returns>
        public override IPresentableFieldForElement CreatePresentableField(IPresentableObject parentPresentableObject) {
            return new PresentableFieldForString(parentPresentableObject, this.Key);
        }

    }

}