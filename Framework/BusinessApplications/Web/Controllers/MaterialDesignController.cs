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

namespace Framework.BusinessApplications.Web.Controllers {

    using Persistence;
    using Presentation.Web.Controllers;
    using System;

    /// <summary>
    /// HTTP controller for responding local CSS files for material
    /// design.
    /// </summary>
    public sealed class MaterialDesignController : LocalFileController {

        /// <summary>
        /// Primary color to set.
        /// </summary>
        public PrimaryColor PrimaryColor { get; set; }

        /// <summary>
        /// Placholder string for primary color 100 in CSS file.
        /// </summary>
        private const string primaryColor100Placehodler = "#cfd8dc";

        /// <summary>
        /// Placholder string for primary color 500 in CSS file.
        /// </summary>
        private const string primaryColor500Placehodler = "#607d8b";

        /// <summary>
        /// Placholder string for primary color 700 in CSS file.
        /// </summary>
        private const string primaryColor700Placehodler = "#455a64";

        /// <summary>
        /// Secondary color to set.
        /// </summary>
        public SecondaryColor SecondaryColor { get; set; }

        /// <summary>
        /// Placholder string for secondary color in CSS file.
        /// </summary>
        private const string secondaryColorPlacehodler = "#00acc1";

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="styleSheetFile">name of css file</param>
        /// <param name="primaryColor">primary color to set</param>
        /// <param name="secondaryColor">secondary color to set</param>
        public MaterialDesignController(string styleSheetFile, PrimaryColor primaryColor, SecondaryColor secondaryColor)
            : base("/" + styleSheetFile, CacheControl.MustRevalidate) {
            this.PrimaryColor = primaryColor;
            this.SecondaryColor = secondaryColor;
        }

        /// <summary>
        /// Finds the file for a specific URL.
        /// </summary>
        /// <param name="url">URL of requested file</param>
        /// <returns>file for URL or null</returns>
        protected override File FindFile(Uri url) {
            var file = base.FindFile(url);
            if (null != file) {
                string css = System.Text.Encoding.UTF8.GetString(file.Bytes);
                if (PrimaryColor.BlueGrey != this.PrimaryColor) {
                    css = css.Replace(MaterialDesignController.primaryColor500Placehodler, '#' + ((int)this.PrimaryColor).ToString("x6"));
                    if (PrimaryColor.Red == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#ffcdd2");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#d32f2f");
                    } else if (PrimaryColor.Pink == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#f8bbd0");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#c2185b");
                    } else if (PrimaryColor.Purple == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#e1bee7");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#7b1fa2");
                    } else if (PrimaryColor.DeepPurple == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#d1c4e9");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#512da8");
                    } else if (PrimaryColor.Indigo == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#c5cae9");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#303f9f");
                    } else if (PrimaryColor.Blue == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#bbdefb");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#1976d2");
                    } else if (PrimaryColor.LightBlue == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#b3e5fc");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#0288d1");
                    } else if (PrimaryColor.Cyan == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#b2ebf2");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#0097a7");
                    } else if (PrimaryColor.Teal == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#b2dfdb");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#00796b");
                    } else if (PrimaryColor.Green == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#c8e6c9");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#388e3c");
                    } else if (PrimaryColor.LightGreen == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#dcedc8");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#689f38");
                    } else if (PrimaryColor.Lime == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#f0f4c3");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#afb42b");
                    } else if (PrimaryColor.Yellow == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#fff9c4");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#fbc02d");
                    } else if (PrimaryColor.Amber == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#ffecb3");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#ffa000");
                    } else if (PrimaryColor.Orange == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#ffe0b2");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#f57c00");
                    } else if (PrimaryColor.DeepOrange == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#ffccbc");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#e64a19");
                    } else if (PrimaryColor.Brown == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#d7ccc8");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#5d4037");
                    } else if (PrimaryColor.Grey == this.PrimaryColor) {
                        css = css.Replace(MaterialDesignController.primaryColor100Placehodler, "#f5f5f5");
                        css = css.Replace(MaterialDesignController.primaryColor700Placehodler, "#616161");
                    } else {
                        throw new ArgumentException("Primary color \"" + this.PrimaryColor + "\" is unknown.");
                    }
                }
                if (SecondaryColor.Cyan != this.SecondaryColor) {
                    if (SecondaryColor.Logo == this.SecondaryColor) {
                        css = css.Replace("body.logo", "body");
                    }
                    css = css.Replace(MaterialDesignController.secondaryColorPlacehodler, '#' + ((int)this.SecondaryColor).ToString("x6"));
                }
                file.Bytes = System.Text.Encoding.UTF8.GetBytes(css);
            }
            return file;
        }

    }

}