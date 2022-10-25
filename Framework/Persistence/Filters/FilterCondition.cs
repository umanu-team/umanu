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

namespace Framework.Persistence.Filters {
    using Framework.Model;
    using System;

    /// <summary>
    /// Single filter condition to be applied.
    /// </summary>
    public sealed class FilterCondition {

        /// <summary>
        /// Base type of value of filter criteria.
        /// </summary>
        public Type ContentBaseType { get; private set; }

        /// <summary>
        /// Name of field to use for comparison.
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Name chain of field to use for comparison.
        /// </summary>
        public string[] FieldNameChain {
            get { return KeyChain.FromKey(this.FieldName); }
        }

        /// <summary>
        /// Name chain of parent field to use for comparison.
        /// </summary>
        public string[] FieldNameChainOfParent {
            get { return KeyChain.RemoveLastLinkOf(this.FieldNameChain); }
        }

        /// <summary>
        /// Name of parent field to use for comparison.
        /// </summary>
        public string FieldNameOfParent {
            get { return KeyChain.ToKey(this.FieldNameChainOfParent); }
        }

        /// <summary>
        /// Logical connective to previous filter.
        /// </summary>
        public FilterConnective FilterConnective { get; private set; }

        /// <summary>
        /// Determines whether the property "ValueOrOtherFieldName"
        /// is another value or the name of another field.
        /// </summary>
        public FilterTarget FilterTarget { get; private set; }

        /// <summary>
        /// Indicates whether filter condition is for IS NULL
        /// comparison.
        /// </summary>
        public bool IsForEqualsNull {
            get { return null == this.ValueOrOtherFieldName && RelationalOperator.IsEqualTo == this.RelationalOperator && FilterTarget.IsOtherField != this.FilterTarget; }
        }

        /// <summary>
        /// Relational operator to use for comparison.
        /// </summary>
        public RelationalOperator RelationalOperator { get; private set; }

        /// <summary>
        /// Value or name of other field to use for comparison.
        /// </summary>
        public object ValueOrOtherFieldName { get; private set; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to use for comparison</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        /// <param name="filterConnective">logical connective to
        /// previous filter</param>
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        public FilterCondition(string fieldName, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, FilterConnective filterConnective, Type contentBaseType)
            : base() {
            this.FieldName = fieldName;
            this.RelationalOperator = relationalOperator;
            this.ValueOrOtherFieldName = valueOrOtherFieldName;
            this.FilterTarget = filterTarget;
            this.FilterConnective = filterConnective;
            this.ContentBaseType = contentBaseType;
        }

    }

}