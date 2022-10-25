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

    using Framework.BusinessApplications;
    using Framework.BusinessApplications.Buttons;
    using Framework.Model;
    using Framework.Presentation;
    using Framework.Presentation.Buttons;
    using Framework.Presentation.Forms;
    using Framework.Presentation.Web;
    using Framework.Presentation.Web.Controllers;
    using Framework.Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// HTTP controller for responding info pages with edit form.
    /// </summary>
    public class EditableInfoPageController : InfoPageController {

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="businessApplication">business application to
        /// process</param>
        /// <param name="absoluteUrl">absolute URL of page
        /// - it may not be empty, not contain any special charaters
        /// except for dashes and has to start with a slash</param>
        /// <param name="infoPage">title and text of page</param>
        /// <param name="textRenderMode">indicates whether text is
        /// supposed to be rendered as plain text or as rich text</param>
        public EditableInfoPageController(IBusinessApplication businessApplication, string absoluteUrl, InfoPage infoPage, TextRenderMode textRenderMode)
            : base(businessApplication, absoluteUrl, infoPage, textRenderMode) {
            // nothing to do
        }

        /// <summary>
        /// Fills the page with a form.
        /// </summary>
        protected void CreateFormPage() {
            this.Page.Title = this.InfoPage.Title?.ToString();
            var form = new Form(this.InfoPage, this.GetFormView(), this.BusinessApplication.FileBaseDirectory, this.BusinessApplication.GetWebFactory(FieldRenderMode.Form));
            var actionBar = new ActionBar<InfoPage>(form);
            var saveButton = new SaveDelegateButton<InfoPage>(Resources.Save, null, delegate {
                if (this.InfoPage.IsNew) {
                    if (false == this.InfoPage.Title?.IsNew) {
                        this.InfoPage.Title.UpdateCascadedly();
                    }
                    if (false == this.InfoPage.Text?.IsNew) {
                        this.InfoPage.Text.UpdateCascadedly();
                    }
                } else {
                    this.InfoPage.UpdateCascadedly();
                }
                return;
            }) {
                RedirectionTarget = this.AbsoluteUrl
            };
            actionBar.AddButton(saveButton);
            var cancelButton = new CancelButton(Resources.Cancel) {
                RedirectionTarget = this.AbsoluteUrl
            };
            actionBar.AddButton(cancelButton);
            this.AddActionBarToPage(actionBar);
            this.Page.ContentSection.AddChildControl(form);
            return;
        }

        /// <summary>
        /// Gets the buttons to be displayed on info page.
        /// </summary>
        /// <returns>buttons to be displayed on info page</returns>
        public override IEnumerable<ActionButton> GetButtons() {
            foreach (var button in base.GetButtons()) {
                yield return button;
            }
            if (false == this.InfoPage.Title?.IsNew || false == this.InfoPage.Text?.IsNew) {
                var editButton = new EditButton<InfoPage>(Resources.Edit, null);
                if (null != this.InfoPage.AllowedGroups) {
                    editButton.AllowedGroupsForReading.AddRange(this.InfoPage.AllowedGroups.ForWriting);
                }
                if (null != this.InfoPage.Title?.AllowedGroups) {
                    editButton.AllowedGroupsForReading.AddRangeIfNotContained(this.InfoPage.Title.AllowedGroups.ForWriting);
                }
                if (null != this.InfoPage.Text?.AllowedGroups) {
                    editButton.AllowedGroupsForReading.AddRangeIfNotContained(this.InfoPage.Text.AllowedGroups.ForWriting);
                }
                editButton.TargetUrl = this.AbsoluteUrl + "edit.html";
                yield return editButton;
            }
        }

        /// <summary>
        /// Gets a form view for multilingual strings.
        /// </summary>
        /// <returns>form view for multilingual strings</returns>
        protected FormView GetFormView() {
            var formView = new FormView();
            if (false == this.InfoPage.Title?.IsNew) {
                var viewPaneForTitle = new ViewPaneForPanes(Resources.Title, nameof(this.InfoPage.Title));
                var viewPaneForInvariantTitle = new ViewPaneForFields();
                viewPaneForInvariantTitle.ViewFields.Add(new ViewFieldForSingleLineText(Resources.InvariantTitle, nameof(MultilingualString.InvariantValue), Mandatoriness.Required));
                viewPaneForTitle.ViewPanes.Add(viewPaneForInvariantTitle);
                var viewPaneForTranslatedTitles = new ViewCollectionPaneForFields(nameof(KeyValuePair.KeyField), nameof(MultilingualString.Translations), SectionGroupType.Tabs, true) {
                    AutoAddFirstSection = false,
                    Placeholder = Resources.PleaseUseThePlusButtonToAddTranslations
                };
                var titleKeyField = new ViewFieldForStringChoice(Resources.Language, nameof(KeyValuePair.KeyField), Mandatoriness.Required) {
                    OptionProvider = new CultureOptionProvider(CultureTypes.NeutralCultures)
                };
                viewPaneForTranslatedTitles.ViewFields.Add(titleKeyField);
                viewPaneForTranslatedTitles.ViewFields.Add(new ViewFieldForSingleLineText(Resources.InvariantText, nameof(KeyValuePair.Value), Mandatoriness.Required));
                viewPaneForTitle.ViewPanes.Add(viewPaneForTranslatedTitles);
                formView.ViewPanes.Add(viewPaneForTitle);
            }
            if (false == this.InfoPage.Text?.IsNew) {
                var viewPaneForText = new ViewPaneForPanes(Resources.Text, nameof(this.InfoPage.Text));
                var viewPaneForInvariantText = new ViewPaneForFields();
                viewPaneForInvariantText.ViewFields.Add(EditableInfoPageController.GetViewFieldForMultilineText(Resources.InvariantText, nameof(MultilingualString.InvariantValue), Mandatoriness.Required, this.TextRenderMode));
                viewPaneForText.ViewPanes.Add(viewPaneForInvariantText);
                var viewPaneForTranslatedTexts = new ViewCollectionPaneForFields(nameof(KeyValuePair.KeyField), nameof(MultilingualString.Translations), SectionGroupType.Tabs, true) {
                    AutoAddFirstSection = false,
                    Placeholder = Resources.PleaseUseThePlusButtonToAddTranslations
                };
                var textKeyField = new ViewFieldForStringChoice(Resources.Language, nameof(KeyValuePair.KeyField), Mandatoriness.Required) {
                    OptionProvider = new CultureOptionProvider(CultureTypes.NeutralCultures)
                };
                viewPaneForTranslatedTexts.ViewFields.Add(textKeyField);
                viewPaneForTranslatedTexts.ViewFields.Add(EditableInfoPageController.GetViewFieldForMultilineText(Resources.TranslatedText, nameof(KeyValuePair.Value), Mandatoriness.Required, this.TextRenderMode));
                viewPaneForText.ViewPanes.Add(viewPaneForTranslatedTexts);
                formView.ViewPanes.Add(viewPaneForText);
            }
            return formView;
        }

        /// <summary>
        /// Gets a view field for multiline text for a specific text
        /// render mode.
        /// </summary>
        /// <param name="title">title of field</param>
        /// <param name="key">internal key of this field</param>
        /// <param name="mandatoriness">value indicating whether a
        /// value in this field is required</param>
        /// <param name="textRenderMode">render mode to be applied
        /// for text</param>
        /// <returns>view field for multiline text for specific text
        /// render mode</returns>
        private static ViewField GetViewFieldForMultilineText(string title, string key, Mandatoriness mandatoriness, TextRenderMode textRenderMode) {
            ViewField multilineTextViewField;
            if (TextRenderMode.PlainText == textRenderMode) {
                multilineTextViewField = new ViewFieldForMultilineText(title, key, mandatoriness);
            } else {
                var multilineRichTextViewField = new ViewFieldForMultilineRichText(title, key, mandatoriness);
                if (TextRenderMode.RichTextWithAutomaticHyperlinkDetection == textRenderMode) {
                    multilineRichTextViewField.AutomaticHyperlinkDetection = AutomaticHyperlinkDetection.IsEnabled;
                } else if (TextRenderMode.RichTextWithoutAutomaticHyperlinkDetection == textRenderMode) {
                    multilineRichTextViewField.AutomaticHyperlinkDetection = AutomaticHyperlinkDetection.IsDisabled;
                }
                multilineTextViewField = multilineRichTextViewField;
            }
            return multilineTextViewField;
        }

        /// <summary>
        /// Processes a web request - can be called from method
        /// Application_PostAuthenticateRequest of Global.asax.
        /// </summary>
        /// <param name="httpRequest">http request to process</param>
        /// <param name="httpResponse">http response for web request</param>
        /// <returns>true if request was processed, false if
        /// controller is not responsible</returns>
        public override bool ProcessRequest(HttpRequest httpRequest, HttpResponse httpResponse) {
            bool isProcessed = base.ProcessRequest(httpRequest, httpResponse);
            if (httpRequest.Url.AbsolutePath.Equals(this.AbsoluteUrl + "edit.html", StringComparison.OrdinalIgnoreCase)) {
                if (httpRequest.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) || httpRequest.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase)) {
                    this.CreateFormPage();
                    this.ProcessPreProcessedRequest(httpRequest, httpResponse);
                } else {
                    OptionsController.RejectRequest(httpResponse);
                }
                isProcessed = true;
            }
            return isProcessed;
        }

    }

}