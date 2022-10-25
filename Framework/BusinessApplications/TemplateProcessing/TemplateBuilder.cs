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

namespace Framework.BusinessApplications.TemplateProcessing {

    using System;
    using System.Text;

    /// <summary>
    /// Template engine to be used for filling in values into
    /// templates. To do so a subset of {{ mustache }} tags can be
    /// used: https://mustache.github.io/
    /// </summary>
    public class TemplateBuilder {

        /// <summary>
        /// Dictionary of keys and values to be filled in into
        /// template.
        /// </summary>
        public TemplateDictionary Dictionary { get; private set; }

        /// <summary>
        /// Template with placeholders. The following tags are
        /// supported:<br />
        /// 
        /// Variables - A variable like {{foo}} will be replaced by
        /// the value with the key &quot;foo&quot; in the current
        /// context. If there is none, the parent context will be
        /// browsed from the bottom to the top. If still there is
        /// none, the variable will just not be rendered.<br />
        /// 
        /// Sections - A list section is introduced with {{#bar}} and
        /// ends with {{/bar}}. It may contain variables or further
        /// sections. If a value for the key &quot;bar&quot; exists
        /// and is either false or an empty enumerable the contents
        /// of the section will not be rendered. Otherwise, if the
        /// value is true the section will be endered once. If the
        /// value is a non-empty enumerable the contents of the
        /// section will be rendered once per list item and the
        /// context will be set to the current item for each
        /// iteration.
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public TemplateBuilder() {
            this.Dictionary = new TemplateDictionary();
        }

        /// <summary>
        /// Processes a section of the template.
        /// </summary>
        /// <param name="template">section of template to be
        /// processed</param>
        /// <param name="templateDictionary">dictionary of keys and
        /// values to be filled in into template section</param>
        /// <returns>precessed section of template</returns>
        private static string ProcessTemplateSection(string template, TemplateDictionary templateDictionary) {
            string processedTemplate = template;
            int tagStart;
            do {
                tagStart = processedTemplate.IndexOf("{{") + 2;
                if (tagStart > 1) { // tag start found
                    var tagEnd = processedTemplate.IndexOf("}}", tagStart);
                    if (tagEnd > -1) { // tag end found
                        var tagKey = processedTemplate.Substring(tagStart, tagEnd - tagStart);
                        if (tagKey.StartsWith("#", StringComparison.Ordinal)) { // tag is for list
                            tagKey = tagKey.Substring(1);
                            var closingTagStart = processedTemplate.IndexOf("{{/" + tagKey + "}}", tagEnd + 2);
                            if (closingTagStart > -1) { // end tag found
                                var closingTagEnd = closingTagStart + 1 + tagKey.Length;
                                var templateSectionBuilder = new StringBuilder();
                                templateSectionBuilder.Append(processedTemplate.Substring(0, tagStart - 2));
                                if (templateDictionary.Contains(tagKey)) {
                                    var templateItem = templateDictionary[tagKey];
                                    if (templateItem is TemplateKeyValuePair templateKeyValuePair && bool.TryParse(templateKeyValuePair.Value, out bool isValueTrue) && isValueTrue) {
                                        templateSectionBuilder.Append(processedTemplate.Substring(tagEnd + 2, closingTagStart - tagEnd - 2));
                                    } else if (templateItem is TemplateKeyValuesPair templateKeyValuesPair) {
                                        foreach (var childTemplateDictionary in templateKeyValuesPair.Values) {
                                            templateSectionBuilder.Append(TemplateBuilder.ProcessTemplateSection(processedTemplate.Substring(tagEnd + 2, closingTagStart - tagEnd - 2), templateDictionary.MergeWith(childTemplateDictionary)));
                                        }
                                    }
                                }
                                templateSectionBuilder.Append(processedTemplate.Substring(closingTagEnd + 4));
                                processedTemplate = templateSectionBuilder.ToString();
                            } else { // no end tag found
                                throw new FormatException("Matching closing tag could not be found for opening tag {{#" + tagKey + "}}.");
                            }
                        } else { // tag is for variable
                            string tagValue;
                            if (templateDictionary.Contains(tagKey)) {
                                tagValue = (templateDictionary[tagKey] as TemplateKeyValuePair)?.Value;
                            } else {
                                tagValue = string.Empty;
                            }
                            processedTemplate = processedTemplate.Substring(0, tagStart - 2) + tagValue + processedTemplate.Substring(tagEnd + 2);
                        }
                    } else { // no tag end found
                        throw new FormatException("End of tag could not be found at character position " + tagStart + ".");
                    }
                }
            } while (tagStart > 1);
            return processedTemplate;
        }

        /// <summary>
        /// Processes the template by filling in the values.
        /// </summary>
        /// <returns>processed template with inserted values</returns>
        public override string ToString() {
            return TemplateBuilder.ProcessTemplateSection(this.Template, this.Dictionary);
        }

    }

}