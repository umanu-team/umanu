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

namespace Framework.Presentation.Web {

    using Framework.Presentation;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Represents a web app manifest as described in
    /// https://w3c.github.io/manifest/. Web app manifests are used
    /// to turn web applications into progessive web apps (PWAs).
    /// </summary>
    public sealed class WebAppManifest : PresentableObject {

        /// <summary>
        /// Expected background color of web application and splash
        /// screen.
        /// </summary>
        public string BackgroundColor {
            get { return this.backgroundColor.Value; }
            set { this.backgroundColor.Value = value; }
        }
        private readonly PresentableFieldForString backgroundColor;

        /// <summary>
        /// Description of purpose of web application.
        /// </summary>
        public string Description {
            get { return this.description.Value; }
            set { this.description.Value = value; }
        }
        private readonly PresentableFieldForString description;

        /// <summary>
        /// Specifies the reading direction for the localizable
        /// members of the manifest, e.g. &quot;ltr&quot; or
        /// &quot;rtl&quot;.
        /// </summary>
        public string Dir {
            get { return this.dir.Value; }
            set { this.dir.Value = value; }
        }
        private readonly PresentableFieldForString dir;

        /// <summary>
        /// Preferred display mode for web application, e.g.
        /// &quot;fullscreen&quot;, &quot;standalone&quot;,
        /// &quot;minimal-ui&quot; or &quot;browser&quot;.
        /// </summary>
        public string Display {
            get { return this.display.Value; }
            set { this.display.Value = value; }
        }
        private readonly PresentableFieldForString display;

        /// <summary>
        /// Iconic representations of web application in various
        /// contexts.
        /// </summary>
        public PresentableFieldForPresentableObjectCollection<WebAppManifestImage> Icons { get; private set; }

        /// <summary>
        /// Primary language for the localizable members of the
        /// manifes, e.g. &quot;de&quot;.
        /// </summary>
        public string Lang {
            get { return this.lang.Value; }
            set { this.lang.Value = value; }
        }
        private readonly PresentableFieldForString lang;

        /// <summary>
        /// Name of web application as it is usually displayed to the user.
        /// </summary>
        public string Name {
            get { return this.name.Value; }
            set { this.name.Value = value; }
        }
        private readonly PresentableFieldForString name;

        /// <summary>
        /// Default screen orientation for web application as defined
        /// in https://www.w3.org/TR/screen-orientation/.
        /// </summary>
        public string Orientation {
            get { return this.orientation.Value; }
            set { this.orientation.Value = value; }
        }
        private readonly PresentableFieldForString orientation;

        /// <summary>
        /// Navigation scope of application context. Usually this is
        /// the root URL of the application.
        /// </summary>
        public string Scope {
            get { return this.scope.Value; }
            set { this.scope.Value = value; }
        }

        /// <summary>
        /// Screenshots representing the app in common usage scenarios.
        /// </summary>
        public PresentableFieldForPresentableObjectCollection<WebAppManifestImage> Screenshots { get; private set; }
        private readonly PresentableFieldForString scope;

        /// <summary>
        /// Short version of the name of web application, intended to
        /// be used where there is not engough space to display the
        /// full name.
        /// </summary>
        public string ShortName {
            get { return this.shortName.Value; }
            set { this.shortName.Value = value; }
        }
        private readonly PresentableFieldForString shortName;

        /// <summary>
        /// URL that should be loaded when the user launches the web
        /// application.
        /// </summary>
        public string StartUrl {
            get { return this.startUrl.Value; }
            set { this.startUrl.Value = value; }
        }
        private readonly PresentableFieldForString startUrl;

        /// <summary>
        /// Default theme color for application context.
        /// </summary>
        public string ThemeColor {
            get { return this.themeColor.Value; }
            set { this.themeColor.Value = value; }
        }
        private readonly PresentableFieldForString themeColor;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="name">name of web application as it is
        /// usually displayed to the user</param>
        /// <param name="shortName">short version of name of web
        /// application, intended to be used where there is not
        /// engough space to display the full name</param>
        /// <param name="description">description of purpose of web
        /// application</param>
        /// <param name="startUrl">URL that should be loaded when the
        /// user launches the web application</param>
        /// <param name="scope">navigation scope of application
        /// context - usually this is the root URL of the application</param>
        public WebAppManifest(string name, string shortName, string description, string startUrl, string scope)
            : base() {
            this.backgroundColor = new PresentableFieldForString(this, nameof(this.BackgroundColor));
            this.AddPresentableField(this.backgroundColor);
            this.description = new PresentableFieldForString(this, nameof(this.Description), description);
            this.AddPresentableField(this.description);
            this.dir = new PresentableFieldForString(this, nameof(this.Dir));
            this.AddPresentableField(this.dir);
            this.display = new PresentableFieldForString(this, nameof(this.Display), "standalone");
            this.AddPresentableField(this.display);
            this.Icons = new PresentableFieldForPresentableObjectCollection<WebAppManifestImage>(this, nameof(this.Icons));
            this.AddPresentableField(this.Icons);
            this.lang = new PresentableFieldForString(this, nameof(this.Lang));
            this.AddPresentableField(this.lang);
            this.name = new PresentableFieldForString(this, nameof(this.Name), name);
            this.AddPresentableField(this.name);
            this.orientation = new PresentableFieldForString(this, nameof(this.Orientation), "natural");
            this.AddPresentableField(this.orientation);
            this.scope = new PresentableFieldForString(this, nameof(this.Scope), scope);
            this.AddPresentableField(this.scope);
            this.Screenshots = new PresentableFieldForPresentableObjectCollection<WebAppManifestImage>(this, nameof(this.Screenshots));
            this.AddPresentableField(this.Screenshots);
            this.shortName = new PresentableFieldForString(this, nameof(this.ShortName), shortName);
            this.AddPresentableField(this.shortName);
            this.startUrl = new PresentableFieldForString(this, nameof(this.StartUrl), startUrl);
            this.AddPresentableField(this.startUrl);
            this.themeColor = new PresentableFieldForString(this, nameof(this.ThemeColor));
            this.AddPresentableField(this.themeColor);
        }

    }

}