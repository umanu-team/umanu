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

namespace Framework.Presentation.Converters {

    using Forms;
    using Framework.Model;
    using Framework.Presentation.Web;
    using Persistence;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Converter for presentable objects to RSS format.
    /// </summary>
    public class RssWriter {

        /// <summary>
        /// Absolute URL of RSS feed (e.g. https://.../feed.rss).
        /// </summary>
        public string AbsoluteUrl { get; private set; }

        /// <summary>
        /// Dictionary of XML tag name and key chain of additional
        /// fields to include in RSS items.
        /// </summary>
        public IDictionary<string, string[]> AdditionalItemFields { get; private set; }

        /// <summary>
        /// Additional namespace to use in XML file.
        /// </summary>
        public KeyValuePair AdditionalNamespace { get; set; }

        /// <summary>
        /// Number of providable objects returned per page.
        /// </summary>
        public ulong? ItemsPerPage { get; set; }

        /// <summary>
        /// Delegate for resolval of link URL for an item.
        /// </summary>
        public OnClickUrlDelegate OnClickUrlDelegate { get; private set; }

        /// <summary>
        /// Data provider to use for option providers.
        /// </summary>
        public IOptionDataProvider OptionDataProvider { get; private set; }

        /// <summary>
        /// Providable objects to be converted.
        /// </summary>
        public IEnumerable<IProvidableObject> ProvidableObjects { get; private set; }

        /// <summary>
        /// Index of the first item in the current set of providable
        /// objects.
        /// </summary>
        public ulong? StartIndex { get; set; }

        /// <summary>
        /// Number of items if no items were skipped or topped.
        /// </summary>
        public ulong? TotalResults { get; set; }

        /// <summary>
        /// List view to be applied for field mapping.
        /// </summary>
        public IListTableView View { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="absoluteUrl">absolute URL of RSS feed
        /// (e.g. https://.../feed.rss)</param>
        /// <param name="view">list view to be applied for field
        /// mapping</param>
        /// <param name="providableObjects">providable objects to
        /// be converted</param>
        /// <param name="optionDataProvider">data provider to use for
        /// option providers</param>
        /// <param name="onClickUrlDelegate">delegate for resolval of
        /// link URL for an item</param>
        public RssWriter(string absoluteUrl, IListTableView view, IEnumerable<IProvidableObject> providableObjects, IOptionDataProvider optionDataProvider, OnClickUrlDelegate onClickUrlDelegate) {
            this.AbsoluteUrl = absoluteUrl;
            this.AdditionalItemFields = new Dictionary<string, string[]>();
            this.OnClickUrlDelegate = onClickUrlDelegate;
            this.OptionDataProvider = optionDataProvider;
            this.ProvidableObjects = providableObjects;
            this.View = view;
        }

        /// <summary>
        /// Writes the header of the RSS.
        /// </summary>
        /// <param name="rssBuilder">RSS builder to write to</param>
        private void WriteRssHead(StringBuilder rssBuilder) {
            rssBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><rss version=\"2.0\"");
            if (this.ItemsPerPage.HasValue || this.StartIndex.HasValue || this.TotalResults.HasValue) {
                rssBuilder.Append(" xmlns:opensearch=\"http://a9.com/-/spec/opensearch/1.1/\"");
            }
            if (null != this.AdditionalNamespace) {
                rssBuilder.Append(" xmlns:");
                rssBuilder.Append(this.AdditionalNamespace.KeyField);
                rssBuilder.Append("=\"");
                rssBuilder.Append(this.AdditionalNamespace.Value);
                rssBuilder.Append("\"");
            }
            rssBuilder.Append("><channel><title>");
            rssBuilder.Append(this.View.Title);
            rssBuilder.Append("</title><description>");
            rssBuilder.Append(this.View.Description);
            rssBuilder.Append("</description><link>");
            rssBuilder.Append(HttpUtility.UrlEncode(this.AbsoluteUrl));
            rssBuilder.Append("</link>");
            if (this.TotalResults.HasValue) {
                rssBuilder.Append("<opensearch:totalResults>");
                rssBuilder.Append(this.TotalResults.Value.ToString(CultureInfo.InvariantCulture));
                rssBuilder.Append("</opensearch:totalResults>");
            }
            if (this.StartIndex.HasValue) {
                rssBuilder.Append("<opensearch:startIndex>");
                rssBuilder.Append(this.StartIndex.Value.ToString(CultureInfo.InvariantCulture));
                rssBuilder.Append("</opensearch:startIndex>");
            }
            if (this.ItemsPerPage.HasValue) {
                rssBuilder.Append("<opensearch:itemsPerPage>");
                rssBuilder.Append(this.ItemsPerPage.Value.ToString(CultureInfo.InvariantCulture));
                rssBuilder.Append("</opensearch:itemsPerPage>");
            }
            return;
        }

        /// <summary>
        /// Writes the footer of the RSS.
        /// </summary>
        /// <param name="rssBuilder">RSS builder to write to</param>
        private void WriteRssFoot(StringBuilder rssBuilder) {
            rssBuilder.Append("</channel></rss>");
            return;
        }

        /// <summary>
        /// Writes an RSS item.
        /// </summary>
        /// <param name="rssBuilder">RSS builder to write to</param>
        /// <param name="item">item to write data for</param>
        private void WriteRssItem(StringBuilder rssBuilder, IProvidableObject item) {
            rssBuilder.Append("<item>");
            var title = item.GetTitle();
            if (!string.IsNullOrEmpty(title)) {
                rssBuilder.Append("<title>");
                rssBuilder.Append(title);
                rssBuilder.Append("</title>");
            }
            var link = this.OnClickUrlDelegate(item);
            if (!string.IsNullOrEmpty(link)) {
                rssBuilder.Append("<link>");
                rssBuilder.Append(link);
                rssBuilder.Append("</link>");
            }
            if (!item.IsNew) {
                if (Guid.Empty != item.Id) {
                    rssBuilder.Append("<guid isPermaLink=\"false\">");
                    rssBuilder.Append(item.Id.ToString());
                    rssBuilder.Append("</guid>");
                }
                rssBuilder.Append("<pubDate>");
                rssBuilder.Append(item.CreatedAt.ToString("R"));
                rssBuilder.Append("</pubDate>");
            }
            rssBuilder.Append("<description><![CDATA[");
            bool isFirstValue = true;
            bool hasOneViewFieldOnly = 1 == this.View.ViewFields.Count;
            foreach (var viewField in this.View.ViewFields) {
                if (viewField.IsVisible && (!(viewField is ViewFieldForFile) || viewField is ViewFieldForImageFile) && (!(viewField is ViewFieldForMultipleFiles) || viewField is ViewFieldForMultipleImageFiles)) {
                    IPresentableField presentableField;
                    if (viewField is ViewFieldForEditableValue viewFieldForEditableValue) {
                        presentableField = item.FindPresentableField(viewFieldForEditableValue.Key);
                    } else {
                        presentableField = null;
                    }
                    if (null != presentableField) {
                        string value = viewField.GetReadOnlyValueFor(presentableField, item, this.OptionDataProvider);
                        if (!string.IsNullOrEmpty(value)) {
                            if (isFirstValue) {
                                isFirstValue = false;
                            } else {
                                rssBuilder.Append("<br />");
                            }
                            if (!hasOneViewFieldOnly) {
                                rssBuilder.Append("<b>");
                                rssBuilder.Append(viewField.Title);
                                rssBuilder.Append(":</b> ");
                            }
                            this.WriteRssItemValue(rssBuilder, item, viewField, presentableField, value);
                        }
                    }
                }
            }
            rssBuilder.Append("]]></description>");
            this.WriteAdditionalItemFields(rssBuilder, item);
            rssBuilder.Append("</item>");
            return;
        }

        /// <summary>
        /// Writes additional fields to RSS item.
        /// </summary>
        /// <param name="rssBuilder">RSS builder to write to</param>
        /// <param name="item">item to write data for</param>
        private void WriteAdditionalItemFields(StringBuilder rssBuilder, IProvidableObject item) {
            foreach (var additionalItemField in this.AdditionalItemFields) {
                var presentableField = item.FindPresentableField(additionalItemField.Value);
                if (presentableField is IPresentableFieldForElement presentableFieldForElement) {
                    rssBuilder.Append('<');
                    rssBuilder.Append(additionalItemField.Key);
                    rssBuilder.Append('>');
                    if (presentableFieldForElement.ValueAsObject is ImageFile imageFile) {
                        RssWriter.WriteRssImageValue(rssBuilder, imageFile);
                    } else {
                        rssBuilder.Append(presentableFieldForElement.ValueAsString);
                    }
                    rssBuilder.Append("</");
                    rssBuilder.Append(additionalItemField.Key);
                    rssBuilder.Append('>');
                } else if (presentableField is IPresentableFieldForCollection presentableFieldForCollection) {
                    using (var objectEnumerator = presentableFieldForCollection.GetValuesAsObject().GetEnumerator()) {
                        using (var stringEnumerator = presentableFieldForCollection.GetValuesAsString().GetEnumerator()) {
                            while (objectEnumerator.MoveNext() && stringEnumerator.MoveNext()) {
                                rssBuilder.Append('<');
                                rssBuilder.Append(additionalItemField.Key);
                                rssBuilder.Append('>');
                                if (objectEnumerator.Current is ImageFile imageFile) {
                                    RssWriter.WriteRssImageValue(rssBuilder, imageFile);
                                } else {
                                    rssBuilder.Append(stringEnumerator.Current);
                                }
                                rssBuilder.Append("</");
                                rssBuilder.Append(additionalItemField.Key);
                                rssBuilder.Append('>');
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Writes an email value of an RSS item.
        /// </summary>
        /// <param name="rssBuilder">RSS builder to write to</param>
        /// <param name="emailAddress">email address to write data for</param>
        private static void WriteRssEmailValue(StringBuilder rssBuilder, string emailAddress) {
            rssBuilder.Append("<a href=\"mailto:");
            rssBuilder.Append(emailAddress);
            rssBuilder.Append("\">");
            rssBuilder.Append(emailAddress);
            rssBuilder.Append("</a>");
            return;
        }

        /// <summary>
        /// Writes an image value of an RSS item.
        /// </summary>
        /// <param name="rssBuilder">RSS builder to write to</param>
        /// <param name="imageFile">image file to write data for</param>
        private static void WriteRssImageValue(StringBuilder rssBuilder, ImageFile imageFile) {
            rssBuilder.Append("<img src=\"data:");
            rssBuilder.Append(imageFile.MimeType);
            rssBuilder.Append(";base64,");
            rssBuilder.Append(Convert.ToBase64String(imageFile.Bytes));
            rssBuilder.Append("\" />");
            return;
        }

        /// <summary>
        /// Writes the actual value of an RSS item.
        /// </summary>
        /// <param name="rssBuilder">RSS builder to write to</param>
        /// <param name="item">item to write data for</param>
        /// <param name="viewField">view field to write value for</param>
        /// <param name="presentableField">presentable field to
        /// write value for</param>
        /// <param name="value">value to be written</param>
        private void WriteRssItemValue(StringBuilder rssBuilder, IProvidableObject item, ViewField viewField, IPresentableField presentableField, string value) {
            if (viewField is ViewFieldForEmailAddress) {
                RssWriter.WriteRssEmailValue(rssBuilder, value);
            } else if (viewField is ViewFieldForImageFile viewFieldForImageFile && presentableField is IPresentableFieldForElement presentableFieldForImage && presentableFieldForImage.ValueAsObject is ImageFile imageFile) {
                RssWriter.WriteRssImageValue(rssBuilder, imageFile);
            } else if (viewField is ViewFieldForMultipleEmailAddresses viewFieldForMultipleEmailAddresses && presentableField is IPresentableFieldForCollection presentableFieldForEmailCollection) {
                bool isFirstEmailAddress = true;
                foreach (var emailAddress in viewFieldForMultipleEmailAddresses.GetReadOnlyValuesFor(presentableFieldForEmailCollection, item, this.OptionDataProvider)) {
                    if (!string.IsNullOrEmpty(emailAddress)) {
                        if (isFirstEmailAddress) {
                            isFirstEmailAddress = false;
                        } else {
                            rssBuilder.Append(WebFieldForCollection.GetValueSeparator(viewFieldForMultipleEmailAddresses.GetValueSeparator(FieldRenderMode.ListTable)));
                        }
                        RssWriter.WriteRssEmailValue(rssBuilder, emailAddress);
                    }
                }
            } else if (viewField is ViewFieldForMultipleImageFiles && presentableField is IPresentableFieldForCollection presentableFieldForImageCollection) {
                foreach (var valueAsObject in presentableFieldForImageCollection.GetValuesAsObject()) {
                    if (valueAsObject is ImageFile valueAsImageFile) {
                        RssWriter.WriteRssImageValue(rssBuilder, valueAsImageFile);
                    }
                }
            } else if (viewField is ViewFieldForMultipleUrls viewFieldForMultipleUrls && presentableField is IPresentableFieldForCollection presentableFieldForUrlCollection) {
                bool isFirstUrl = true;
                foreach (var url in viewFieldForMultipleUrls.GetReadOnlyValuesFor(presentableFieldForUrlCollection, item, this.OptionDataProvider)) {
                    if (!string.IsNullOrEmpty(url)) {
                        if (isFirstUrl) {
                            isFirstUrl = false;
                        } else {
                            rssBuilder.Append(WebFieldForCollection.GetValueSeparator(viewFieldForMultipleUrls.GetValueSeparator(FieldRenderMode.ListTable)));
                        }
                        RssWriter.WriteRssUrlValue(rssBuilder, url);
                    }
                }
            } else if (viewField is ViewFieldForUrl) {
                RssWriter.WriteRssUrlValue(rssBuilder, value);
            } else {
                rssBuilder.Append(value);
            }
            return;
        }

        /// <summary>
        /// Writes an URL value of an RSS item.
        /// </summary>
        /// <param name="rssBuilder">RSS builder to write to</param>
        /// <param name="url">URL value to write data for</param>
        private static void WriteRssUrlValue(StringBuilder rssBuilder, string url) {
            rssBuilder.Append("<a href=\"");
            rssBuilder.Append(url);
            rssBuilder.Append("\" rel=\"noopener\" target=\"_blank\">");
            rssBuilder.Append(url);
            rssBuilder.Append("</a>");
            return;
        }

        /// <summary>
        /// Returns a RSS file that represents the current object.
        /// </summary>
        /// <param name="name">name of file</param>
        /// <returns>RSS file that represents the objects</returns>
        public File WriteFile(string name) {
            return new File(name, "application/rss+xml", Encoding.UTF8.GetBytes(this.WriteString()));
        }

        /// <summary>
        /// Returns a RSS string that represents the current object.
        /// </summary>
        /// <returns>RSS string that represents the objects</returns>
        public string WriteString() {
            var rssBuilder = new StringBuilder();
            this.WriteRssHead(rssBuilder);
            foreach (var item in this.ProvidableObjects) {
                this.WriteRssItem(rssBuilder, item);
            }
            this.WriteRssFoot(rssBuilder);
            return rssBuilder.ToString();
        }

    }

}