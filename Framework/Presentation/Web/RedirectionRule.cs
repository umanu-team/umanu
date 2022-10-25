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

    using System;
    using Framework.Presentation.Exceptions;

    /// <summary>
    /// Represents a URL redirection.
    /// </summary>
    public class RedirectionRule {

        /// <summary>
        /// True if no further rules need to be checked if this one
        /// matches in a chain of redirection rules, false otherwise.
        /// </summary>
        public bool IsLastRule { get; set; }

        /// <summary>
        /// Type of rule.
        /// </summary>
        public RedirectionRuleType RuleType { get; set; }

        /// <summary>
        /// Absolute path of source URL to match.
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// Target URL for redirecting matching requests to.
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public RedirectionRule() {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="sourceUrl">source URL to match</param>
        /// <param name="ruleType">type of rule</param>
        /// <param name="targetUrl">target URL for redirecting
        /// matching requests to</param>
        public RedirectionRule(string sourceUrl, RedirectionRuleType ruleType, string targetUrl)
            : this() {
            this.IsLastRule = true;
            this.SourceUrl = sourceUrl;
            this.RuleType = ruleType;
            this.TargetUrl = targetUrl;
        }

        /// <summary>
        /// Matches a URL against the condition of this redirection. 
        /// </summary>
        /// <param name="url">absolute path of URL to match against
        /// this redirection</param>
        /// <returns>input URL if it does not match the condition of
        /// this redirection, new target URL otherwise</returns>
        public string Match(string url) {
            string targetUrl = url;
            if (RedirectionRuleType.Equals == this.RuleType) {
                if (url == this.SourceUrl) {
                    targetUrl = this.TargetUrl;
                }
            } else if (RedirectionRuleType.StartsWith == this.RuleType) {
                if (url.StartsWith(this.SourceUrl, StringComparison.Ordinal)) {
                    targetUrl = this.TargetUrl;
                }
            } else if (RedirectionRuleType.Contains == this.RuleType) {
                if (url.Contains(this.SourceUrl)) {
                    targetUrl = this.TargetUrl;
                }
            } else if (RedirectionRuleType.EndsWith == this.RuleType) {
                if (url.EndsWith(this.SourceUrl, StringComparison.Ordinal)) {
                    targetUrl = this.TargetUrl;
                }
            } else if (RedirectionRuleType.RegularExpression == this.RuleType) {
                targetUrl = System.Text.RegularExpressions.Regex.Replace(url, this.SourceUrl, this.TargetUrl);
            } else {
                throw new PresentationException("Redirection source URL type \"" + this.RuleType + "\" is unknown.");
            }
            return targetUrl;
        }

    }

}