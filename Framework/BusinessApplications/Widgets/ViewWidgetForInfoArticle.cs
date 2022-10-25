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

namespace Framework.BusinessApplications.Widgets {

    using Framework.Persistence.Fields;
    using Framework.Presentation.Web;

    /// <summary>
    /// View widget for info text.
    /// </summary>
    public class ViewWidgetForInfoArticle : ViewWidget {

        /// <summary>
        /// Text to be displayed.
        /// </summary>
        public string Text {
            get { return this.text.Value; }
            set { this.text.Value = value; }
        }
        private readonly PersistentFieldForString text =
            new PersistentFieldForString(nameof(Text));

        /// <summary>
        /// Indicates whether text is supposed to be rendered as
        /// plain text or as rich text.
        /// </summary>
        public TextRenderMode TextRenderMode {
            get { return (TextRenderMode)this.textRenderMode.Value; }
            set { this.textRenderMode.Value = (int)value; }
        }
        private readonly PersistentFieldForInt textRenderMode =
            new PersistentFieldForInt(nameof(TextRenderMode));

        /// <summary>
        /// Title to be displayed.
        /// </summary>
        public string Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForString title =
            new PersistentFieldForString(nameof(Title));

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public ViewWidgetForInfoArticle()
            : base() {
            this.RegisterPersistentField(this.text);
            this.RegisterPersistentField(this.textRenderMode);
            this.RegisterPersistentField(this.title);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title to be displayed</param>
        /// <param name="text">text to be displayed</param>
        /// <param name="textRenderMode">indicates whether text is
        /// supposed to be rendered as plain text or as rich text</param>
        public ViewWidgetForInfoArticle(string title, string text, TextRenderMode textRenderMode)
            : this() {
            this.Text = text;
            this.TextRenderMode = textRenderMode;
            this.Title = title;
        }

        /// <summary>
        /// Gets the control for view widget.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="positionId">ID of position of widget on
        ///  parent page</param>
        /// <returns>control for view widget</returns>
        public override Control GetControlWithoutWidgetCss(IBusinessApplication businessApplication, ulong positionId) {
            return new InfoArticle(this.Title, this.Text, this.TextRenderMode);
        }

    }

}