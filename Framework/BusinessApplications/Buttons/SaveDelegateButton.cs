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

namespace Framework.BusinessApplications.Buttons {

    using Framework.Presentation;
    using Framework.Presentation.Forms;

    /// <summary>
    /// Action button of action bar for saving items.
    /// </summary>
    /// <typeparam name="T">type of objects to save</typeparam>
    public class SaveDelegateButton<T> : SaveButton<T> where T : class, IProvidableObject {

        /// <summary>
        /// Delegate to be called on save.
        /// </summary>
        public OnSaveDelegate OnSaveDelegate { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="dataProvider">data provider for current
        /// presentable object</param>
        /// <param name="onSaveDelegate">delegate to be called on
        /// save</param>
        public SaveDelegateButton(string title, DataProvider<T> dataProvider, OnSaveDelegate onSaveDelegate)
            : base(title, dataProvider) {
            this.OnSaveDelegate = onSaveDelegate;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="title">display caption of button</param>
        /// <param name="dataProvider">data provider for current
        /// presentable object</param>
        /// <param name="onSaveDelegate">delegate to be called on
        /// save</param>
        ///<param name="formId">client ID of target form - if this is
        ///null or empty the last form of the page will be picked</param>
        public SaveDelegateButton(string title, DataProvider<T> dataProvider, OnSaveDelegate onSaveDelegate, string formId)
            : this(title, dataProvider, onSaveDelegate) {
            this.FormId = formId;
        }

        /// <summary>
        /// Server-side action to execute on click.
        /// </summary>
        /// <param name="form">current form or null</param>
        /// <param name="promptInput">prompt input of user</param>
        public override void OnClick(IForm form, string promptInput) {
            if (true == form.HasValidValue) {
                var providableObject = form.PresentableObject as T;
                this.OnSaveDelegate(providableObject, promptInput);
            }
            return;
        }

    }

}