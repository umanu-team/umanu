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
    using Framework.Persistence.Exceptions;
    using Framework.Presentation;
    using Framework.Presentation.Forms;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Delegate for translation of filter value.
    /// </summary>
    /// <param name="fieldName">name of field to translate value
    /// for</param>
    /// <param name="value">value to be translated</param>
    /// <returns>translated value</returns>
    public delegate object TranslateValueDelegate(string fieldName, object value);

    /// <summary>
    /// Filter criteria for selection of objects.
    /// </summary>
    public sealed class FilterCriteria : IComparable<FilterCriteria>, IEquatable<FilterCriteria> {

        /// <summary>
        /// Logical connective to previous filter.
        /// </summary>
        public FilterConnective Connective { get; private set; }

        /// <summary>
        /// Base type of value of filter criteria.
        /// </summary>
        public Type ContentBaseType { get; private set; }

        /// <summary>
        /// Empty filter that matches all objects.
        /// </summary>
        public static readonly FilterCriteria Empty = new FilterCriteria();

        /// <summary>
        /// Name of field to use for comparison.
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Name chain of field to use for comparison.
        /// </summary>
        public string[] FieldNameChain {
            get { return KeyChain.FromKey(this.FieldName); }
            set { this.FieldName = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Determins whether the target of the comparison is another
        /// value or another field.
        /// </summary>
        public FilterTarget FilterTarget { get; private set; }

        /// <summary>
        /// True if this filter contains comparisons to NULL.
        /// </summary>
        public bool HasNullValues {
            get {
                return null == this.ValueOrOtherFieldName
                    || (this.HasSubFilterCriteria && this.SubFilterCriteria.HasNullValues);
            }
        }

        /// <summary>
        /// True if this filter has a sub filter chain to apply,
        /// false if this filter itself has a criterion to apply.
        /// </summary>
        public bool HasSubFilterCriteria {
            get {
                return null != this.SubFilterCriteria;
            }
        }

        /// <summary>
        /// True if filter is empty, false otherwise.
        /// </summary>
        public bool IsEmpty {
            get {
                return string.IsNullOrEmpty(this.FieldName)
                    && null == this.SubFilterCriteria;
            }
        }

        /// <summary>
        /// True if this is the last filter to apply in filter chain,
        /// false otherwise.
        /// </summary>
        public bool IsLastFilterOfChain {
            get { return null == this.Next; }
        }

        /// <summary>
        /// Last filter of filter chain.
        /// </summary>
        private FilterCriteria LastFilterOfChain {
            get {
                var filterChain = this;
                while (!filterChain.IsLastFilterOfChain) {
                    filterChain = filterChain.Next;
                }
                return filterChain;
            }
        }

        /// <summary>
        /// Next filter to apply in filter chain.
        /// </summary>
        public FilterCriteria Next { get; private set; }

        /// <summary>
        /// Relational operator to use for comparison.
        /// </summary>
        public RelationalOperator RelationalOperator { get; private set; }

        /// <summary>
        /// Sub filter chain to apply.
        /// </summary>
        public FilterCriteria SubFilterCriteria { get; private set; }

        /// <summary>
        /// Value to compare value of the field to or name of other
        /// field whose value to compare value of the field to.
        /// </summary>
        public object ValueOrOtherFieldName { get; private set; }

        #region Constructors

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        private FilterCriteria() {
            this.Connective = FilterConnective.None;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="subFilterChain">sub filter chain to apply</param>
        public FilterCriteria(FilterCriteria subFilterChain)
            : this() {
            this.SubFilterCriteria = subFilterChain.Copy();
        }

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
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        private FilterCriteria(string fieldName, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, Type contentBaseType)
            : this() {
            if (null == contentBaseType) {
                this.ContentBaseType = valueOrOtherFieldName?.GetType();
            } else {
                this.ContentBaseType = contentBaseType;
            }
            this.FilterTarget = filterTarget;
            this.FieldName = fieldName;
            this.RelationalOperator = relationalOperator;
            this.ValueOrOtherFieldName = valueOrOtherFieldName;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to use for comparison</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        private FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, Type contentBaseType)
            : this() {
            if (null == contentBaseType) {
                this.ContentBaseType = valueOrOtherFieldName?.GetType();
            } else {
                this.ContentBaseType = contentBaseType;
            }
            this.FilterTarget = filterTarget;
            this.FieldNameChain = fieldNameChain;
            this.RelationalOperator = relationalOperator;
            this.ValueOrOtherFieldName = valueOrOtherFieldName;
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, bool value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherBoolValue, TypeOf.Bool) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, bool value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherBoolValue, TypeOf.Bool) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, byte value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Byte) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, byte value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Byte) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, char value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherTextValue, TypeOf.Char) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, char value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherTextValue, TypeOf.Char) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, DateTime value)
            : this(fieldName, relationalOperator, UtcDateTime.ConvertToUniversalTime(value).Ticks, FilterTarget.IsOtherNumericValue, TypeOf.Long) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, DateTime value)
            : this(fieldNameChain, relationalOperator, UtcDateTime.ConvertToUniversalTime(value).Ticks, FilterTarget.IsOtherNumericValue, TypeOf.Long) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, decimal value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Decimal) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, decimal value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Decimal) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, double value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Double) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, double value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Double) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, float value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Float) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, float value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Float) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, Guid value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherGuidValue, TypeOf.Guid) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, Guid value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherGuidValue, TypeOf.Guid) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, int value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Int) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, int value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Int) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, IUser value)
            : this(fieldName, relationalOperator, null == value ? null : (object)value.Id, FilterTarget.IsOtherTextValue, TypeOf.Guid) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, IUser value)
            : this(fieldNameChain, relationalOperator, null == value ? null : (object)value.Id, FilterTarget.IsOtherTextValue, TypeOf.Guid) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, long value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Long) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, long value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Long) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, sbyte value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.SByte) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name of field chain to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, sbyte value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.SByte) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, short value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Short) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name of field chain to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, short value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Short) {
            // nothing to do
        }

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
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, string valueOrOtherFieldName, FilterTarget filterTarget)
            : this(fieldName, relationalOperator, FilterCriteria.CastToFilterTarget(valueOrOtherFieldName, filterTarget), filterTarget, null) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name of field chain to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to use for comparison</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, string valueOrOtherFieldName, FilterTarget filterTarget)
            : this(fieldNameChain, relationalOperator, FilterCriteria.CastToFilterTarget(valueOrOtherFieldName, filterTarget), filterTarget, null) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, uint value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UInt) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, uint value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UInt) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, ulong value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.ULong) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, ulong value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.ULong) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string fieldName, RelationalOperator relationalOperator, ushort value)
            : this(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UShort) {
            // nothing to do
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        public FilterCriteria(string[] fieldNameChain, RelationalOperator relationalOperator, ushort value)
            : this(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UShort) {
            // nothing to do
        }

        #endregion

        #region And

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="subFilterChain">sub filter chain to
        /// AND-concatenate</param>
        /// <returns>new filter chain which is the specified filter
        /// chain AND-concatenated to this filter chain</returns>
        public FilterCriteria And(FilterCriteria subFilterChain) {
            FilterCriteria filterChain;
            if (this.IsEmpty) {
                filterChain = subFilterChain.Copy();
            } else {
                filterChain = this.Copy();
                if (!subFilterChain.IsEmpty) {
                    var newFilter = new FilterCriteria(subFilterChain.Copy()) {
                        Connective = FilterConnective.And
                    };
                    filterChain.LastFilterOfChain.Next = newFilter;
                }
            }
            return filterChain;
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
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
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        private FilterCriteria And(string fieldName, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, Type contentBaseType) {
            FilterCriteria filterChain;
            var newFilter = new FilterCriteria(fieldName, relationalOperator, valueOrOtherFieldName, filterTarget, contentBaseType);
            if (this.IsEmpty) {
                filterChain = newFilter;
            } else {
                newFilter.Connective = FilterConnective.And;
                filterChain = this.Copy();
                filterChain.LastFilterOfChain.Next = newFilter;
            }
            return filterChain;
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to use for comparison</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        private FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, Type contentBaseType) {
            FilterCriteria filterChain;
            var newFilter = new FilterCriteria(fieldNameChain, relationalOperator, valueOrOtherFieldName, filterTarget, contentBaseType);
            if (this.IsEmpty) {
                filterChain = newFilter;
            } else {
                newFilter.Connective = FilterConnective.And;
                filterChain = this.Copy();
                filterChain.LastFilterOfChain.Next = newFilter;
            }
            return filterChain;
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, bool value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherBoolValue, TypeOf.Bool);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, bool value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherBoolValue, TypeOf.Bool);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, byte value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Byte);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, byte value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Byte);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, char value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherTextValue, TypeOf.Char);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name of field chain to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, char value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherTextValue, TypeOf.Char);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, DateTime value) {
            return this.And(fieldName, relationalOperator, UtcDateTime.ConvertToUniversalTime(value).Ticks, FilterTarget.IsOtherNumericValue, TypeOf.Long);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, DateTime value) {
            return this.And(fieldNameChain, relationalOperator, UtcDateTime.ConvertToUniversalTime(value).Ticks, FilterTarget.IsOtherNumericValue, TypeOf.Long);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, decimal value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Decimal);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name of field chain to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, decimal value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Decimal);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, double value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Double);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, double value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Double);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, float value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Float);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, float value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Float);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, Guid value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherGuidValue, TypeOf.Guid);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, Guid value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherGuidValue, TypeOf.Guid);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, int value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Int);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, int value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Int);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, IUser value) {
            return this.And(fieldName, relationalOperator, null == value ? null : (object)value.Id, FilterTarget.IsOtherTextValue, TypeOf.Guid);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, IUser value) {
            return this.And(fieldNameChain, relationalOperator, null == value ? null : (object)value.Id, FilterTarget.IsOtherTextValue, TypeOf.Guid);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, long value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Long);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, long value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Long);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, sbyte value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.SByte);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, sbyte value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.SByte);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, short value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Short);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, short value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Short);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
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
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, string valueOrOtherFieldName, FilterTarget filterTarget) {
            return this.And(fieldName, relationalOperator, FilterCriteria.CastToFilterTarget(valueOrOtherFieldName, filterTarget), filterTarget, null);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to use for comparison</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, string valueOrOtherFieldName, FilterTarget filterTarget) {
            return this.And(fieldNameChain, relationalOperator, FilterCriteria.CastToFilterTarget(valueOrOtherFieldName, filterTarget), filterTarget, null);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, uint value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UInt);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, uint value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UInt);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, ulong value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.ULong);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, ulong value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.ULong);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string fieldName, RelationalOperator relationalOperator, ushort value) {
            return this.And(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UShort);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// AND-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion AND-concatenated to this filter chain</returns>
        public FilterCriteria And(string[] fieldNameChain, RelationalOperator relationalOperator, ushort value) {
            return this.And(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UShort);
        }

        #endregion

        /// <summary>
        /// Makes sure the value type matches the filter target.
        /// </summary>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to match to filter target</param>
        /// <param name="filterTarget">filter target to match value
        /// to</param>
        /// <returns>value matching the filter target</returns>
        private static object CastToFilterTarget(string valueOrOtherFieldName, FilterTarget filterTarget) {
            object value;
            if (FilterTarget.IsOtherBoolValue == filterTarget) {
                value = bool.Parse(valueOrOtherFieldName);
            } else if (FilterTarget.IsOtherGuidValue == filterTarget) {
                value = Guid.Parse(valueOrOtherFieldName);
            } else if (FilterTarget.IsOtherNumericValue == filterTarget) {
                if (byte.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out byte byteValue)) {
                    value = byteValue;
                } else if (sbyte.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out sbyte sbyteValue)) {
                    value = sbyteValue;
                } else if (ushort.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out ushort ushortValue)) {
                    value = ushortValue;
                } else if (short.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out short shortValue)) {
                    value = shortValue;
                } else if (uint.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out uint uintValue)) {
                    value = uintValue;
                } else if (int.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue)) {
                    value = intValue;
                } else if (ulong.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out ulong ulongValue)) {
                    value = ulongValue;
                } else if (long.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out long longValue)) {
                    value = longValue;
                } else if (decimal.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue)) {
                    value = decimalValue;
                } else if (float.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue)) {
                    value = floatValue;
                } else if (double.TryParse(valueOrOtherFieldName, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue)) {
                    value = doubleValue;
                } else {
                    throw new FilterException("Cannot convert \"" +
                        valueOrOtherFieldName + "\" to match numeric filter target.");
                }
            } else {
                value = valueOrOtherFieldName;
            }
            return value;
        }

        /// <summary>
        /// Compares the current instance with another object of the
        /// same type by key.
        /// </summary>
        /// <param name="other">An object to compare with this
        /// instance.</param>
        /// <returns>A signed integer that indicates the relative
        /// order of the comparands: Less than zero if this instance
        /// is less than the other instance. Equal to zero if this
        /// instance is equal to the other instance. Greater than
        /// zero if this instance is greater than the other instance.</returns>
        public int CompareTo(FilterCriteria other) {
            return this.ToString().CompareTo(other.ToString());
        }

        /// <summary>
        /// AND-concatenates two filter criteria.
        /// </summary>
        /// <param name="first">first filter criteria to concatenate</param>
        /// <param name="second">second filter criteria to
        /// concatenate</param>
        /// <returns>AND-concatenation of provided filter criteria</returns>
        public static FilterCriteria Concat(FilterCriteria first, FilterCriteria second) {
            FilterCriteria concated;
            if (null == second || second.IsEmpty) {
                concated = first;
            } else if (null == first || first.IsEmpty) {
                concated = second;
            } else {
                concated = new FilterCriteria(first).And(second);
            }
            return concated;
        }

        /// <summary>
        /// Deep copies the state of this instance into another
        /// instance of this type, as far as all child objects
        /// implement ICopyable&lt;T&gt;.
        /// </summary>
        /// <returns>deep copy of this instance</returns>
        public FilterCriteria Copy() {
            var source = this;
            var target = new FilterCriteria();
            var filterCriteria = target;
            do {
                target.Connective = source.Connective;
                target.ContentBaseType = source.ContentBaseType;
                target.FieldName = source.FieldName;
                target.FilterTarget = source.FilterTarget;
                target.RelationalOperator = source.RelationalOperator;
                target.ValueOrOtherFieldName = source.ValueOrOtherFieldName;
                if (!source.IsLastFilterOfChain) {
                    target.Next = new FilterCriteria();
                }
                if (source.HasSubFilterCriteria) {
                    target.SubFilterCriteria = source.SubFilterCriteria.Copy();
                }
                source = source.Next;
                target = target.Next;
            } while (null != target);
            return filterCriteria;
        }

        /// <summary>
        /// Determines whether the current object is equal to another
        /// object of the same type.
        /// </summary>
        /// <param name="other">object to compare this object to</param>
        /// <returns>true if both objects are equal, false otherwise</returns>
        public bool Equals(FilterCriteria other) {
            bool isEqual;
            if (null == other) {
                isEqual = false;
            } else {
                isEqual = this.ToString().Equals(other.ToString(), StringComparison.Ordinal);
            }
            return isEqual;
        }

        /// <summary>
        /// Removes all items from a list that are not matching the
        /// filter criteria.
        /// </summary>
        /// <param name="list">list to be filtered</param>
        /// <typeparam name="T">type of list items</typeparam>
        public void Filter<T>(IList<T> list) where T : IPresentableObject {
            for (var i = list.Count - 1; i > -1; i--) {
                if (!this.IsMatch(list[i])) {
                    list.RemoveAt(i);
                }
            }
            return;
        }

        /// <summary>
        /// Creates a new instance of filter criteria for a given
        /// default string filter.
        /// </summary>
        /// <param name="filter">default string filter to parse</param>
        /// <returns>new filter criteria from filter</returns>
        public static FilterCriteria FromStringFilter(string filter) {
            return new StringFilterParser(filter).ToFilterCriteria();
        }

        /// <summary>
        /// Gets all OR-connected AND-connected filter chain links.
        /// </summary>
        /// <returns>OR-connected AND-connected filter chain links</returns>
        private List<List<FilterCriteria>> GetOrConnectedAndConnectedFilterChainLinks() {
            var orConnectedAndConnectedFilterChainLinks = new List<List<FilterCriteria>>();
            var andConnectedFilterChainLinks = new List<FilterCriteria> {
                this
            };
            orConnectedAndConnectedFilterChainLinks.Add(andConnectedFilterChainLinks);
            var filterChainLink = this;
            while (!filterChainLink.IsLastFilterOfChain) {
                filterChainLink = filterChainLink.Next;
                if (FilterConnective.And == filterChainLink.Connective) {
                    andConnectedFilterChainLinks.Add(filterChainLink);
                } else if (FilterConnective.Or == filterChainLink.Connective) {
                    andConnectedFilterChainLinks = new List<FilterCriteria> {
                        filterChainLink
                    };
                    orConnectedAndConnectedFilterChainLinks.Add(andConnectedFilterChainLinks);
                }
            }
            return orConnectedAndConnectedFilterChainLinks;
        }

        #region isMatch

        /// <summary>
        /// Indicates whether filter criteria match a specific
        /// object.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// apply filter to</param>
        /// <returns>true if filter criteria match the object, false
        /// otherwise</returns>
        public bool IsMatch(IPresentableObject presentableObject) {
            bool isMatch;
            if (this.IsEmpty) {
                isMatch = true;
            } else if (this.HasSubFilterCriteria) {
                isMatch = this.SubFilterCriteria.IsMatch(presentableObject);
            } else {
                var presentableField = presentableObject.FindPresentableField(this.FieldName);
                if (presentableField is IPresentableFieldForElement presentableFieldForElement) {
                    isMatch = this.IsMatch(presentableObject, presentableFieldForElement);
                } else if (presentableField is IPresentableFieldForCollection presentableFieldForCollection) {
                    isMatch = this.IsMatch(presentableObject, presentableFieldForCollection);
                } else if (null == presentableField) {
                    throw new KeyNotFoundException("Presentable field with key \"" + this.FieldName + "\" cannot be found.");
                } else {
                    throw new FieldTypeException("Fields of types other than " + nameof(IPresentableFieldForElement) + " and " + nameof(IPresentableFieldForCollection) + " are not supported.");
                }
            }
            if (!this.IsLastFilterOfChain) {
                if (FilterConnective.And == this.Connective) {
                    isMatch = isMatch && this.Next.IsMatch(presentableObject);
                } else if (FilterConnective.Or == this.Connective) {
                    isMatch = isMatch || this.Next.IsMatch(presentableObject);
                }
            }
            return isMatch;
        }

        /// <summary>
        /// Indicates whether filter criteria match a specific field
        /// of an object.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// apply filter to</param>
        /// <param name="presentableFieldForCollection">presentable
        /// field to apply filter to</param>
        /// <returns>true if filter criteria match the field, false
        /// otherwise</returns>
        private bool IsMatch(IPresentableObject presentableObject, IPresentableFieldForCollection presentableFieldForCollection) {
            bool isMatch;
            if (FilterTarget.IsOtherField == this.FilterTarget) {
                var otherPresentableField = presentableObject.FindPresentableField(this.ValueOrOtherFieldName.ToString());
                if (otherPresentableField is IPresentableFieldForElement otherPresentableFieldForElement) {
                    isMatch = false;
                    foreach (var firstValue in presentableFieldForCollection.GetValuesAsObject()) {
                        if (this.IsMatch(firstValue, otherPresentableFieldForElement.ValueAsObject)) {
                            isMatch = true;
                            break;
                        }
                    }
                } else if (otherPresentableField is IPresentableFieldForCollection otherPresentableFieldForCollection) {
                    isMatch = false;
                    foreach (var firstValue in presentableFieldForCollection.GetValuesAsObject()) {
                        foreach (var otherValue in otherPresentableFieldForCollection.GetValuesAsObject()) {
                            if (this.IsMatch(firstValue, otherValue)) {
                                isMatch = true;
                                break;
                            }
                        }
                        if (isMatch) {
                            break;
                        }
                    }
                } else if (null == otherPresentableField) {
                    throw new KeyNotFoundException("Presentable field with key \"" + this.ValueOrOtherFieldName.ToString() + "\" cannot be found as other field.");
                } else {
                    throw new FieldTypeException("Fields of types other than " + nameof(IPresentableFieldForElement) + " and " + nameof(IPresentableFieldForCollection) + " are not supported as other field.");
                }
            } else {
                isMatch = false;
                foreach (var firstValue in presentableFieldForCollection.GetValuesAsObject()) {
                    if (this.IsMatch(firstValue, this.ValueOrOtherFieldName)) {
                        isMatch = true;
                        break;
                    }
                }
            }
            return isMatch;
        }

        /// <summary>
        /// Indicates whether filter criteria match a specific field
        /// of an object.
        /// </summary>
        /// <param name="presentableObject">presentable object to
        /// apply filter to</param>
        /// <param name="presentableFieldForElement">presentable
        /// field to apply filter to</param>
        /// <returns>true if filter criteria match the field, false
        /// otherwise</returns>
        private bool IsMatch(IPresentableObject presentableObject, IPresentableFieldForElement presentableFieldForElement) {
            bool isMatch;
            if (FilterTarget.IsOtherField == this.FilterTarget) {
                var otherPresentableField = presentableObject.FindPresentableField(this.ValueOrOtherFieldName.ToString());
                if (otherPresentableField is IPresentableFieldForElement otherPresentableFieldForElement) {
                    isMatch = this.IsMatch(presentableFieldForElement.ValueAsObject, otherPresentableFieldForElement.ValueAsObject);
                } else if (otherPresentableField is IPresentableFieldForCollection otherPresentableFieldForCollection) {
                    isMatch = false;
                    foreach (var otherValue in otherPresentableFieldForCollection.GetValuesAsObject()) {
                        if (this.IsMatch(presentableFieldForElement.ValueAsObject, otherValue)) {
                            isMatch = true;
                            break;
                        }
                    }
                } else if (null == otherPresentableField) {
                    throw new KeyNotFoundException("Presentable field with key \"" + this.ValueOrOtherFieldName.ToString() + "\" cannot be found as other field.");
                } else {
                    throw new FieldTypeException("Fields of types other than " + nameof(IPresentableFieldForElement) + " and " + nameof(IPresentableFieldForCollection) + " are not supported as other field.");
                }
            } else {
                isMatch = this.IsMatch(presentableFieldForElement.ValueAsObject, this.ValueOrOtherFieldName);
            }
            return isMatch;
        }

        /// <summary>
        /// Indicates whether two values are matching by relational
        /// operator.
        /// </summary>
        /// <param name="firstValue">value to compare other value to</param>
        /// <param name="otherValue">value to compare first value to</param>
        /// <returns>true if values are matching by relational
        /// operator, false otherwise</returns>
        private bool IsMatch(object firstValue, object otherValue) {
            bool isMatch;
            var firstString = firstValue?.ToString();
            var otherString = otherValue?.ToString();
            if (RelationalOperator.IsEqualTo == this.RelationalOperator) {
                if (string.IsNullOrEmpty(firstString) || string.IsNullOrEmpty(otherString)) {
                    isMatch = firstValue == otherValue;
                } else {
                    isMatch = firstString.Equals(otherString, StringComparison.OrdinalIgnoreCase);
                }
            } else if (RelationalOperator.IsNotEqualTo == this.RelationalOperator) {
                if (string.IsNullOrEmpty(firstString) || string.IsNullOrEmpty(otherString)) {
                    isMatch = firstValue != otherValue;
                } else {
                    isMatch = !firstString.Equals(otherString, StringComparison.OrdinalIgnoreCase);
                }
            } else if (RelationalOperator.Contains == this.RelationalOperator) {
                isMatch = true == firstString?.ToLowerInvariant().Contains(otherString.ToLowerInvariant());
            } else if (RelationalOperator.EndsWith == this.RelationalOperator) {
                isMatch = true == firstString.ToLowerInvariant()?.EndsWith(otherString.ToLowerInvariant());
            } else if (RelationalOperator.StartsWith == this.RelationalOperator) {
                isMatch = true == firstString.ToLowerInvariant()?.StartsWith(otherString.ToLowerInvariant());
            } else {
                if (FilterTarget.IsOtherBoolValue == this.FilterTarget) {
                    if (bool.TryParse(firstString, out bool firstBool)) {
                        if (firstBool) {
                            firstString = "1";
                        } else {
                            firstString = "0";
                        }
                    }
                    if (bool.TryParse(otherString, out bool otherBool)) {
                        if (otherBool) {
                            otherString = "1";
                        } else {
                            otherString = "0";
                        }
                    }
                }
                if (ulong.TryParse(firstString, out ulong firstUInt64) && ulong.TryParse(otherString, out ulong otherUInt64)) {
                    if (RelationalOperator.IsGreaterThan == this.RelationalOperator) {
                        isMatch = firstUInt64 > otherUInt64;
                    } else if (RelationalOperator.IsGreaterThanOrEqualTo == this.RelationalOperator) {
                        isMatch = firstUInt64 >= otherUInt64;
                    } else if (RelationalOperator.IsLessThan == this.RelationalOperator) {
                        isMatch = firstUInt64 < otherUInt64;
                    } else if (RelationalOperator.IsLessThanOrEqualTo == this.RelationalOperator) {
                        isMatch = firstUInt64 <= otherUInt64;
                    } else {
                        throw new FilterException("Relational operator " + this.RelationalOperator.ToString() + " is not supported.");
                    }
                } else if (decimal.TryParse(firstString, out decimal firstDecimal) && decimal.TryParse(otherString, out decimal otherDecimal)) {
                    if (RelationalOperator.IsGreaterThan == this.RelationalOperator) {
                        isMatch = firstDecimal > otherDecimal;
                    } else if (RelationalOperator.IsGreaterThanOrEqualTo == this.RelationalOperator) {
                        isMatch = firstDecimal >= otherDecimal;
                    } else if (RelationalOperator.IsLessThan == this.RelationalOperator) {
                        isMatch = firstDecimal < otherDecimal;
                    } else if (RelationalOperator.IsLessThanOrEqualTo == this.RelationalOperator) {
                        isMatch = firstDecimal <= otherDecimal;
                    } else {
                        throw new FilterException("Relational operator " + this.RelationalOperator.ToString() + " is not supported.");
                    }
                } else {
                    if (RelationalOperator.IsGreaterThan == this.RelationalOperator) {
                        isMatch = firstString?.CompareTo(otherString) > 0;
                    } else if (RelationalOperator.IsGreaterThanOrEqualTo == this.RelationalOperator) {
                        isMatch = firstString?.CompareTo(otherString) >= 0;
                    } else if (RelationalOperator.IsLessThan == this.RelationalOperator) {
                        isMatch = firstString?.CompareTo(otherString) < 0;
                    } else if (RelationalOperator.IsLessThanOrEqualTo == this.RelationalOperator) {
                        isMatch = firstString?.CompareTo(otherString) <= 0;
                    } else {
                        throw new FilterException("Relational operator " + this.RelationalOperator.ToString() + " is not supported.");
                    }
                }
            }
            return isMatch;
        }

        #endregion

        #region Or

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="subFilterChain">sub filter chain to
        /// OR-concatenate</param>
        /// <returns>new filter chain which is the specified filter
        /// chain OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(FilterCriteria subFilterChain) {
            FilterCriteria filterChain;
            if (this.IsEmpty) {
                filterChain = subFilterChain.Copy();
            } else {
                filterChain = this.Copy();
                if (!subFilterChain.IsEmpty) {
                    var newFilter = new FilterCriteria(subFilterChain.Copy()) {
                        Connective = FilterConnective.Or
                    };
                    filterChain.LastFilterOfChain.Next = newFilter;
                }
            }
            return filterChain;
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
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
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        private FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, Type contentBaseType) {
            FilterCriteria filterChain;
            var newFilter = new FilterCriteria(fieldName, relationalOperator, valueOrOtherFieldName, filterTarget, contentBaseType);
            if (this.IsEmpty) {
                filterChain = newFilter;
            } else {
                newFilter.Connective = FilterConnective.Or;
                filterChain = this.Copy();
                filterChain.LastFilterOfChain.Next = newFilter;
            }
            return filterChain;
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to use for comparison</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        /// <param name="contentBaseType">base type of value of
        /// filter criteria</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        private FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, object valueOrOtherFieldName, FilterTarget filterTarget, Type contentBaseType) {
            FilterCriteria filterChain;
            var newFilter = new FilterCriteria(fieldNameChain, relationalOperator, valueOrOtherFieldName, filterTarget, contentBaseType);
            if (this.IsEmpty) {
                filterChain = newFilter;
            } else {
                newFilter.Connective = FilterConnective.Or;
                filterChain = this.Copy();
                filterChain.LastFilterOfChain.Next = newFilter;
            }
            return filterChain;
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, bool value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherBoolValue, TypeOf.Bool);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, bool value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherBoolValue, TypeOf.Bool);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, byte value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Byte);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, byte value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Byte);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, char value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherTextValue, TypeOf.Char);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, char value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherTextValue, TypeOf.Char);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, DateTime value) {
            return this.Or(fieldName, relationalOperator, UtcDateTime.ConvertToUniversalTime(value).Ticks, FilterTarget.IsOtherNumericValue, TypeOf.Long);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, DateTime value) {
            return this.Or(fieldNameChain, relationalOperator, UtcDateTime.ConvertToUniversalTime(value).Ticks, FilterTarget.IsOtherNumericValue, TypeOf.Long);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, decimal value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Decimal);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, decimal value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Decimal);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, double value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Double);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, double value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Double);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, float value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Float);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, float value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Float);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, Guid value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherGuidValue, TypeOf.Guid);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, Guid value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherGuidValue, TypeOf.Guid);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, int value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Int);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, int value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Int);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, IUser value) {
            return this.Or(fieldName, relationalOperator, null == value ? null : (object)value.Id, FilterTarget.IsOtherTextValue, TypeOf.Guid);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, IUser value) {
            return this.Or(fieldNameChain, relationalOperator, null == value ? null : (object)value.Id, FilterTarget.IsOtherTextValue, TypeOf.Guid);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, long value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Long);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, long value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Long);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, sbyte value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.SByte);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, sbyte value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.SByte);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, short value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Short);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, short value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.Short);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
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
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, string valueOrOtherFieldName, FilterTarget filterTarget) {
            return this.Or(fieldName, relationalOperator, FilterCriteria.CastToFilterTarget(valueOrOtherFieldName, filterTarget), filterTarget, null);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="valueOrOtherFieldName">value or name of
        /// other field to use for comparison</param>
        /// <param name="filterTarget">determines whether the
        /// parameter "valueOrOtherFieldName" is another value or the
        /// name of another field</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, string valueOrOtherFieldName, FilterTarget filterTarget) {
            return this.Or(fieldNameChain, relationalOperator, FilterCriteria.CastToFilterTarget(valueOrOtherFieldName, filterTarget), filterTarget, null);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, uint value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UInt);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, uint value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UInt);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, ulong value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.ULong);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, ulong value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.ULong);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldName">name of field to use for
        /// comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string fieldName, RelationalOperator relationalOperator, ushort value) {
            return this.Or(fieldName, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UShort);
        }

        /// <summary>
        /// Returns a new filter chain which is the specified filter
        /// OR-concatenated to this filter chain.
        /// </summary>
        /// <param name="fieldNameChain">name chain of field to use
        /// for comparison</param>
        /// <param name="relationalOperator">relational operator to
        /// use for comparison</param>
        /// <param name="value">value to use for comparison</param>
        /// <returns>new filter chain which is the specified filter
        /// criterion OR-concatenated to this filter chain</returns>
        public FilterCriteria Or(string[] fieldNameChain, RelationalOperator relationalOperator, ushort value) {
            return this.Or(fieldNameChain, relationalOperator, value, FilterTarget.IsOtherNumericValue, TypeOf.UShort);
        }

        #endregion

        /// <summary>
        /// Optimizes filter criteria by reordering them to avoid
        /// unnecessary subqueries.
        /// </summary>
        /// <returns>reordered filter criteria</returns>
        internal FilterCriteria Sort() {
            FilterCriteria sortedFilterCriteria;
            if (this.IsEmpty) {
                sortedFilterCriteria = FilterCriteria.Empty;
            } else {
                sortedFilterCriteria = this.Copy();
                var orConnectedAndConnectedFilterChainLinks = sortedFilterCriteria.GetOrConnectedAndConnectedFilterChainLinks();
                orConnectedAndConnectedFilterChainLinks.Sort(delegate (List<FilterCriteria> x, List<FilterCriteria> y) {
                    int result;
                    bool hasXMultipleFilterChainLinksOrSubFilterCriteria = 1 != x.Count || x[0].HasSubFilterCriteria;
                    bool hasYMultipleFilterChainLinksOrSubFilterCriteria = 1 != y.Count || y[0].HasSubFilterCriteria;
                    if (hasXMultipleFilterChainLinksOrSubFilterCriteria && hasYMultipleFilterChainLinksOrSubFilterCriteria) {
                        result = 0;
                    } else if (hasXMultipleFilterChainLinksOrSubFilterCriteria && !hasYMultipleFilterChainLinksOrSubFilterCriteria) {
                        result = 1;
                    } else if (!hasXMultipleFilterChainLinksOrSubFilterCriteria && hasYMultipleFilterChainLinksOrSubFilterCriteria) {
                        result = -1;
                    } else {
                        result = x[0].FieldName.CompareTo(y[0].FieldName);
                    }
                    return result;
                });
                FilterCriteria filterChainLink = null;
                for (int i = 0; i < orConnectedAndConnectedFilterChainLinks.Count; i++) {
                    var andConnectedFilterChainLinks = orConnectedAndConnectedFilterChainLinks[i];
                    andConnectedFilterChainLinks.Sort(delegate (FilterCriteria x, FilterCriteria y) {
                        int result;
                        if (x.HasSubFilterCriteria && y.HasSubFilterCriteria) {
                            result = 0;
                        } else if (x.HasSubFilterCriteria && !y.HasSubFilterCriteria) {
                            result = 1;
                        } else if (!x.HasSubFilterCriteria && y.HasSubFilterCriteria) {
                            result = -1;
                        } else {
                            result = x.FieldName.CompareTo(y.FieldName);
                        }
                        return result;
                    });
                    for (int j = 0; j < andConnectedFilterChainLinks.Count; j++) {
                        var andConnectedFilterChainLink = andConnectedFilterChainLinks[j];
                        if (andConnectedFilterChainLink.HasSubFilterCriteria) {
                            andConnectedFilterChainLink.SubFilterCriteria.Sort();
                        }
                        if (0 == j) {
                            if (0 == i) {
                                andConnectedFilterChainLink.Connective = FilterConnective.None;
                            } else {
                                andConnectedFilterChainLink.Connective = FilterConnective.Or;
                            }
                        } else {
                            andConnectedFilterChainLink.Connective = FilterConnective.And;
                        }
                        if (null == filterChainLink) {
                            filterChainLink = sortedFilterCriteria = andConnectedFilterChainLink;
                        } else {
                            filterChainLink = filterChainLink.Next = andConnectedFilterChainLink;
                        }
                        andConnectedFilterChainLink.Next = null;
                    }
                }
            }
            return sortedFilterCriteria;
        }

        /// <summary>
        /// Returns the filter chain as a default string filter.
        /// </summary>
        /// <returns>filter chain as default string filter</returns>
        public override string ToString() {
            return new StringFilterBuilder(this).ToFilter();
        }

        /// <summary>
        /// Translates the field names of filter criteria.
        /// </summary>
        /// <param name="dictionary">dictionary of translations to
        /// apply</param>
        /// <returns>copy of filter criteria with translated field
        /// names</returns>
        public FilterCriteria TranslateFieldNames(IDictionary<string, string> dictionary) {
            var filterCriteria = new FilterCriteria {
                Connective = this.Connective,
                ContentBaseType = this.ContentBaseType
            };
            if (null != this.FieldName) {
                if (dictionary.TryGetValue(this.FieldName, out string translatedFieldName)) {
                    filterCriteria.FieldName = translatedFieldName;
                } else {
                    filterCriteria.FieldName = this.FieldName;
                }
            }
            filterCriteria.FilterTarget = this.FilterTarget;
            if (null != this.Next) {
                filterCriteria.Next = this.Next.TranslateFieldNames(dictionary);
            }
            filterCriteria.RelationalOperator = this.RelationalOperator;
            if (this.HasSubFilterCriteria) {
                filterCriteria.SubFilterCriteria = this.SubFilterCriteria.TranslateFieldNames(dictionary);
            }
            if (null != this.ValueOrOtherFieldName) {
                if (FilterTarget.IsOtherField == this.FilterTarget && dictionary.TryGetValue(this.ValueOrOtherFieldName as string, out string translatedValueOrOtherFieldName)) {
                    filterCriteria.ValueOrOtherFieldName = translatedValueOrOtherFieldName;
                } else {
                    filterCriteria.ValueOrOtherFieldName = this.ValueOrOtherFieldName;
                }
            }
            return filterCriteria;
        }

        /// <summary>
        /// Translates the values of filter criteria.
        /// </summary>
        /// <param name="translateValueDelegate">delegate for
        /// translations to apply</param>
        /// <returns>copy of filter criteria with translated values</returns>
        public FilterCriteria TranslateValues(TranslateValueDelegate translateValueDelegate) {
            var filterCriteria = new FilterCriteria {
                Connective = this.Connective,
                ContentBaseType = this.ContentBaseType,
                FieldName = this.FieldName,
                FilterTarget = this.FilterTarget
            };
            if (null != this.Next) {
                filterCriteria.Next = this.Next.TranslateValues(translateValueDelegate);
            }
            filterCriteria.RelationalOperator = this.RelationalOperator;
            if (this.HasSubFilterCriteria) {
                filterCriteria.SubFilterCriteria = this.SubFilterCriteria.TranslateValues(translateValueDelegate);
            }
            if (null != this.ValueOrOtherFieldName) {
                if (FilterTarget.IsOtherField == this.FilterTarget) {
                    filterCriteria.ValueOrOtherFieldName = this.ValueOrOtherFieldName;
                } else {
                    filterCriteria.ValueOrOtherFieldName = translateValueDelegate(this.FieldName, this.ValueOrOtherFieldName);
                }
            }
            return filterCriteria;
        }

        /// <summary>
        /// Tries to split simple filter criteria into smaller filter
        /// criteria of a shorter chain size.
        /// </summary>
        /// <param name="chainSize">new chain size of filter criteria</param>
        /// <param name="splitFilterCriteriaList">list of split
        /// filter criteria of shorter chain size</param>
        /// <returns>true if filter criteria could be split, false otherwise</returns>
        public bool TrySplitIntoChainsOf(ulong chainSize, out IList<FilterCriteria> splitFilterCriteriaList) {
            if (chainSize < 1) {
                throw new ArgumentException("Chain size has to be > 0.", nameof(chainSize));
            }
            splitFilterCriteriaList = new List<FilterCriteria>();
            var subFilterChain = this.Copy();
            splitFilterCriteriaList.Add(subFilterChain);
            ulong count = 1;
            while (!subFilterChain.IsLastFilterOfChain) {
                var nextSubFilterChain = subFilterChain.Next;
                if (FilterConnective.Or == nextSubFilterChain.Connective && !subFilterChain.HasSubFilterCriteria) {
                    if (count == chainSize) {
                        subFilterChain.Next = null;
                        nextSubFilterChain.Connective = FilterConnective.None;
                        splitFilterCriteriaList.Add(nextSubFilterChain);
                        count = 1;
                    } else {
                        count++;
                    }
                    subFilterChain = nextSubFilterChain;
                } else {
                    splitFilterCriteriaList.Clear();
                    splitFilterCriteriaList.Add(this);
                    break;
                }
            }
            for (int i = splitFilterCriteriaList.Count - 1; i > -1; i--) {
                if (splitFilterCriteriaList[i].IsEmpty && splitFilterCriteriaList[i].IsLastFilterOfChain) {
                    splitFilterCriteriaList.RemoveAt(i);
                }
            }
            return splitFilterCriteriaList.Count > 1;
        }

    }

}