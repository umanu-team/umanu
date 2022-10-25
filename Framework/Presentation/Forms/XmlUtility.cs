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

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Helper class for XML.
    /// </summary>
    public static class XmlUtility {

        /// <summary>
        /// Gets the evaluator callback delegate for HTML attribute
        /// matches.
        /// </summary>
        /// <param name="toleratedAttributes">attributes to be kept</param>
        /// <returns>evaluator callback delegate for HTML attribute
        /// matches</returns>
        private static System.Text.RegularExpressions.MatchEvaluator GetAttributeMatchEvaluator(IEnumerable<string> toleratedAttributes) {
            return delegate (System.Text.RegularExpressions.Match match) {
                var replacementBuilder = new StringBuilder();
                replacementBuilder.Append('<');
                replacementBuilder.Append(match.Groups[1].Value);
                foreach (var toleratedAttribute in toleratedAttributes) {
                    string pattern = " " + toleratedAttribute + "\\s*?=\\s*?(\".*?\")";
                    var attributeMatch = System.Text.RegularExpressions.Regex.Match(match.Value, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (2 == attributeMatch.Groups.Count) {
                        string value = attributeMatch.Groups[1].Value;
                        if (("href" != toleratedAttribute || (value.Length > 2 && Regex.ForStandAloneHyperlink.IsMatch(value.Substring(1, value.Length - 2))))
                            && ("src" != toleratedAttribute || value.StartsWith("\"data:", StringComparison.OrdinalIgnoreCase))) {
                            replacementBuilder.Append(' ');
                            replacementBuilder.Append(toleratedAttribute);
                            replacementBuilder.Append('=');
                            replacementBuilder.Append(value);
                        }
                    }
                }
                replacementBuilder.Append('>');
                return replacementBuilder.ToString();
            };
        }

        /// <summary>
        /// Indicates whether a text value contains XML tags.
        /// </summary>
        /// <param name="value">value to be checked for XML tags</param>
        /// <returns>true if text value contains XML tags, false
        /// otherwise</returns>
        public static bool IsXml(string value) {
            return Regex.ForXmlTag.IsMatch(value);
        }

        /// <summary>
        /// Removes attributes of all instances of a specific XML
        /// tag.
        /// </summary>
        /// <param name="value">text to remove XML attributes in</param>
        /// <param name="xmlTag">XML tag to remove XML attributes
        /// for</param>
        /// <returns>text with removed attributes for XML tag</returns>
        public static string RemoveAttributes(string value, string xmlTag) {
            return XmlUtility.RemoveAttributes(value, xmlTag, new string[0]);
        }

        /// <summary>
        /// Removes attributes of all instances of a specific XML
        /// tag.
        /// </summary>
        /// <param name="value">text to remove XML attributes in</param>
        /// <param name="xmlTag">XML tag to remove XML attributes
        /// for</param>
        /// <param name="toleratedAttributes">attributes to be kept</param>
        /// <returns>text with removed attributes for XML tag</returns>
        public static string RemoveAttributes(string value, string xmlTag, IEnumerable<string> toleratedAttributes) {
            string pattern = "<(" + xmlTag + ")(?: [^>]*)?>";
            return System.Text.RegularExpressions.Regex.Replace(value, pattern, XmlUtility.GetAttributeMatchEvaluator(toleratedAttributes), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Removed all empty tags in XML value.
        /// </summary>
        /// <param name="value">XML value to remove empty tags from</param>
        /// <returns>XML value without empty tags</returns>
        public static string RemoveEmptyTags(string value) {
            string cleanedValue = value;
            string previousValue;
            var regex = new System.Text.RegularExpressions.Regex("<(?:[^/>]*?)>(?:\\s*?)((?:<br />)*?)(?:\\s*?)</(?:[^>]*?)>");
            do {
                previousValue = cleanedValue;
                cleanedValue = regex.Replace(cleanedValue, "$1");
            } while (cleanedValue != previousValue);
            return cleanedValue;
        }

        /// <summary>
        /// Removed all empty tags in XML value.
        /// </summary>
        /// <param name="value">XML value to remove empty tags from</param>
        /// <param name="toleratedEmptyTags">tags to be kept although
        /// they are empty</param>
        /// <returns>XML value without empty tags</returns>
        public static string RemoveEmptyTags(string value, IEnumerable<string> toleratedEmptyTags) {
            string preparedValue = value;
            foreach (var toleratedEmptyTag in toleratedEmptyTags) {
                preparedValue = System.Text.RegularExpressions.Regex.Replace(preparedValue, "<" + toleratedEmptyTag + "( [^>]*)?></" + toleratedEmptyTag + ">", "<" + toleratedEmptyTag + "$1>&nbsp;</" + toleratedEmptyTag + ">");
            }
            return XmlUtility.RemoveEmptyTags(preparedValue);
        }

        /// <summary>
        /// Removes XML tags from text.
        /// </summary>
        /// <param name="text">text to remove XML tags from</param>
        /// <returns>text without XML tags</returns>
        public static string RemoveTags(string text) {
            return XmlUtility.RemoveTags(text, " ");
        }

        /// <summary>
        /// Removes XML tags from text.
        /// </summary>
        /// <param name="text">text to remove XML tags from</param>
        /// <param name="newLine">characters to replace tags for new
        /// lines with, for example &quot; &quot; or
        /// Environment.NewLine</param>
        /// <returns>text without XML tags</returns>
        public static string RemoveTags(string text, string newLine) {
            string plainText = text;
            if (!string.IsNullOrEmpty(plainText)) {
                plainText = plainText.Replace("</article>", newLine);
                plainText = plainText.Replace("<br>", newLine);
                plainText = plainText.Replace("<br/>", newLine);
                plainText = plainText.Replace("<br />", newLine);
                plainText = plainText.Replace("</div>", newLine);
                plainText = plainText.Replace("</p>", newLine);
                plainText = Regex.ForXmlTag.Replace(plainText, string.Empty);
                plainText = HttpUtility.HtmlDecode(plainText);
                plainText = Regex.ForMultipleSpaces.Replace(plainText, " ");
            }
            return plainText;
        }

        /// <summary>
        /// Replaces email addresses in a text.
        /// </summary>
        /// <param name="input">text to replace email addresses in -
        /// may be html</param>
        /// <param name="replacement">text to replace email address
        /// by - placeholder $1 can be used for email address</param>
        /// <returns>text with replaced email addresses</returns>
        public static string ReplaceEmailAddressesInText(string input, string replacement) {
            return XmlUtility.ReplaceEmailAddressesInText(input, replacement, delegate (string emailAddress) {
                return emailAddress; // do not process email address
            });
        }

        /// <summary>
        /// Replaces email addresses in a text.
        /// </summary>
        /// <param name="input">text to replace email addresses in -
        /// may be html</param>
        /// <param name="replacement">text to replace email address
        /// by - placeholder $1 can be used for email address</param>
        /// <param name="processEmailAddressDelegate">delegate for
        /// processing email addresses</param>
        /// <returns>text with replaced email addresses</returns>
        public static string ReplaceEmailAddressesInText(string input, string replacement, ProcessEmailAddressDelegate processEmailAddressDelegate) {
            var whiteSpaceCharacters = new List<char> { ' ', '\n', '\r', '\t', '"', '(', ')', ',', ':', ';', '[', ']' };
            var detectionState = EmailAddressDetectionState.InWord;
            var positions = new Stack<int>();
            int wordStartPos = 0;
            int count = 0;
            foreach (char c in input) {
                if (EmailAddressDetectionState.WhiteSpace == detectionState) {
                    if (whiteSpaceCharacters.Contains(c)) {
                        detectionState = EmailAddressDetectionState.WhiteSpace;
                    } else if ('@' == c) {
                        detectionState = EmailAddressDetectionState.InInvalidEmailAddress;
                    } else if ('<' == c) {
                        detectionState = EmailAddressDetectionState.InHtmlTag;
                    } else {
                        detectionState = EmailAddressDetectionState.InWord;
                        wordStartPos = count;
                    }
                } else if (EmailAddressDetectionState.InWord == detectionState) {
                    if (whiteSpaceCharacters.Contains(c)) {
                        detectionState = EmailAddressDetectionState.WhiteSpace;
                    } else if ('@' == c) {
                        detectionState = EmailAddressDetectionState.InWordWithAtCharacter;
                    } else if ('<' == c) {
                        detectionState = EmailAddressDetectionState.InHtmlTag;
                    }
                } else if (EmailAddressDetectionState.InWordWithAtCharacter == detectionState) {
                    if (whiteSpaceCharacters.Contains(c)) {
                        detectionState = EmailAddressDetectionState.WhiteSpace;
                    } else if ('@' == c) {
                        detectionState = EmailAddressDetectionState.InInvalidEmailAddress;
                    } else if ('<' == c) {
                        detectionState = EmailAddressDetectionState.InHtmlTag;
                    } else {
                        detectionState = EmailAddressDetectionState.InEmailAddress;
                    }
                } else if (EmailAddressDetectionState.InEmailAddress == detectionState) {
                    if (whiteSpaceCharacters.Contains(c)) {
                        detectionState = EmailAddressDetectionState.WhiteSpace;
                        positions.Push(wordStartPos);
                        positions.Push(count);
                    } else if ('@' == c) {
                        detectionState = EmailAddressDetectionState.InInvalidEmailAddress;
                    } else if ('<' == c) {
                        detectionState = EmailAddressDetectionState.InHtmlTag;
                        positions.Push(wordStartPos);
                        positions.Push(count);
                    }
                } else if (EmailAddressDetectionState.InInvalidEmailAddress == detectionState) {
                    if (whiteSpaceCharacters.Contains(c)) {
                        detectionState = EmailAddressDetectionState.WhiteSpace;
                    } else if ('<' == c) {
                        detectionState = EmailAddressDetectionState.InHtmlTag;
                    }
                } else if (EmailAddressDetectionState.InHtmlTag == detectionState) {
                    if ('>' == c) {
                        detectionState = EmailAddressDetectionState.WhiteSpace;
                    }
                }
                count++;
            }
            if (detectionState == EmailAddressDetectionState.InEmailAddress) {
                positions.Push(wordStartPos);
                positions.Push(count);
            }
            string text = input;
            while (positions.Count > 0) {
                int endPos = positions.Pop();
                int startPos = positions.Pop();
                string emailAddress = text.Substring(startPos, endPos - startPos);
                emailAddress = processEmailAddressDelegate(emailAddress);
                text = text.Substring(0, startPos) + replacement.Replace("$1", emailAddress) + text.Substring(endPos);
            }
            return text;
        }

    }

}