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

namespace Framework.BusinessApplications {

    using Framework.Model;
    using Framework.Persistence;
    using Framework.Persistence.Fields;
    using Framework.Presentation;

    /// <summary>
    /// Represents an info page.
    /// </summary>
    public class InfoPage : PersistentObject, IProvidableObject {

        /// <summary>
        /// Text of page.
        /// </summary>
        public MultilingualString Text {
            get { return this.text.Value; }
            set { this.text.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<MultilingualString> text =
            new PersistentFieldForPersistentObject<MultilingualString>(nameof(Text), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// Title of page.
        /// </summary>
        public MultilingualString Title {
            get { return this.title.Value; }
            set { this.title.Value = value; }
        }
        private readonly PersistentFieldForPersistentObject<MultilingualString> title =
            new PersistentFieldForPersistentObject<MultilingualString>(nameof(Title), CascadedRemovalBehavior.RemoveValuesForcibly);

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public InfoPage()
            : base() {
            this.RegisterPersistentField(this.text);
            this.RegisterPersistentField(this.title);
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of page</param>
        /// <param name="text">text of page</param>
        public InfoPage(MultilingualString title, MultilingualString text)
            : this() {
            this.Title = title;
            this.Text = text;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">title of page</param>
        /// <param name="text">text of page</param>
        public InfoPage(string title, string text)
            : this(new MultilingualString(title), new MultilingualString(text)) {
            // nothing to do
        }

        /// <summary>
        /// Gets the title of providable object.
        /// </summary>
        /// <returns>title of providable object</returns>
        public string GetTitle() {
            return this.Title?.ToString();
        }

    }

}