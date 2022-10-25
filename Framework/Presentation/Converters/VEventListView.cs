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

    using Model;
    using Persistence;
    using Persistence.Fields;

    /// <summary>
    /// Represents a view for event lists.
    /// </summary>
    public class VEventListView : PersistentObject {

        /// <summary>
        /// Key of alarms field.
        /// </summary>
        public string AlarmsKey {
            get { return this.alarmsKey.Value; }
            set { this.alarmsKey.Value = value; }
        }
        private readonly PersistentFieldForString alarmsKey =
            new PersistentFieldForString(nameof(AlarmsKey));

        /// <summary>
        /// Key chain of alarms field.
        /// </summary>
        public string[] AlarmsKeyChain {
            get { return KeyChain.FromKey(this.AlarmsKey); }
            set { this.AlarmsKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of attachments field.
        /// </summary>
        public string AttachmentsKey {
            get { return this.attachmentsKey.Value; }
            set { this.attachmentsKey.Value = value; }
        }
        private readonly PersistentFieldForString attachmentsKey =
            new PersistentFieldForString(nameof(AttachmentsKey));

        /// <summary>
        /// Key chain of attachments field.
        /// </summary>
        public string[] AttachmentsKeyChain {
            get { return KeyChain.FromKey(this.AttachmentsKey); }
            set { this.AttachmentsKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key chain of categories field.
        /// </summary>
        public string CategoriesKey {
            get { return this.categoriesKey.Value; }
            set { this.categoriesKey.Value = value; }
        }
        private readonly PersistentFieldForString categoriesKey =
            new PersistentFieldForString(nameof(CategoriesKey));

        /// <summary>
        /// Key chain of categories field.
        /// </summary>
        public string[] CategoriesKeyChain {
            get { return KeyChain.FromKey(this.CategoriesKey); }
            set { this.CategoriesKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of classification field.
        /// </summary>
        public string ClassificationKey {
            get { return this.classificationKey.Value; }
            set { this.classificationKey.Value = value; }
        }
        private readonly PersistentFieldForString classificationKey =
            new PersistentFieldForString(nameof(ClassificationKey));

        /// <summary>
        /// Key chain of classification field.
        /// </summary>
        public string[] ClassificationKeyChain {
            get { return KeyChain.FromKey(this.ClassificationKey); }
            set { this.ClassificationKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of comments field.
        /// </summary>
        public string CommentsKey {
            get { return this.commentsKey.Value; }
            set { this.commentsKey.Value = value; }
        }
        private readonly PersistentFieldForString commentsKey =
            new PersistentFieldForString(nameof(CommentsKey));

        /// <summary>
        /// Key chain of comments field.
        /// </summary>
        public string[] CommentsKeyChain {
            get { return KeyChain.FromKey(this.CommentsKey); }
            set { this.CommentsKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of contacts field.
        /// </summary>
        public string ContactsKey {
            get { return this.contactsKey.Value; }
            set { this.contactsKey.Value = value; }
        }
        private readonly PersistentFieldForString contactsKey =
            new PersistentFieldForString(nameof(ContactsKey));

        /// <summary>
        /// Key chain of contacts field.
        /// </summary>
        public string[] ContactsKeyChain {
            get { return KeyChain.FromKey(ContactsKey); }
            set { this.ContactsKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of created at field.
        /// </summary>
        public string CreatedAtKey {
            get { return this.createdAtKey.Value; }
            set { this.createdAtKey.Value = value; }
        }
        private readonly PersistentFieldForString createdAtKey =
            new PersistentFieldForString(nameof(CreatedAtKey), nameof(PersistentObject.CreatedAt));

        /// <summary>
        /// Key chain of created at field.
        /// </summary>
        public string[] CreatedAtKeyChain {
            get { return KeyChain.FromKey(this.CreatedAtKey); }
            set { this.CreatedAtKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of description field.
        /// </summary>
        public string DescriptionKey {
            get { return this.descriptionKey.Value; }
            set { this.descriptionKey.Value = value; }
        }
        private readonly PersistentFieldForString descriptionKey =
            new PersistentFieldForString(nameof(DescriptionKey));

        /// <summary>
        /// Key chain of description field.
        /// </summary>
        public string[] DescriptionKeyChain {
            get { return KeyChain.FromKey(this.DescriptionKey); }
            set { this.DescriptionKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of end date/time field.
        /// </summary>
        public string EndDateTimeKey {
            get { return this.endDateTimeKey.Value; }
            set { this.endDateTimeKey.Value = value; }
        }
        private readonly PersistentFieldForString endDateTimeKey =
            new PersistentFieldForString(nameof(EndDateTimeKey));

        /// <summary>
        /// Key chain of end date/time field.
        /// </summary>
        public string[] EndDateTimeKeyChain {
            get { return KeyChain.FromKey(this.EndDateTimeKey); }
            set { this.EndDateTimeKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of exception date/times field.
        /// </summary>
        public string ExceptionDateTimesKey {
            get { return this.exceptionDateTimesKey.Value; }
            set { this.exceptionDateTimesKey.Value = value; }
        }
        private readonly PersistentFieldForString exceptionDateTimesKey =
            new PersistentFieldForString(nameof(ExceptionDateTimesKey));

        /// <summary>
        /// Key chain of exception date/times field.
        /// </summary>
        public string[] ExceptionDateTimesKeyChain {
            get { return KeyChain.FromKey(this.ExceptionDateTimesKey); }
            set { this.ExceptionDateTimesKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of ID field.
        /// </summary>
        public string IdKey {
            get { return this.idKey.Value; }
            set { this.idKey.Value = value; }
        }
        private readonly PersistentFieldForString idKey =
            new PersistentFieldForString(nameof(IdKey), nameof(PersistentObject.Id));

        /// <summary>
        /// Key chain of ID field.
        /// </summary>
        public string[] IdKeyChain {
            get { return KeyChain.FromKey(this.IdKey); }
            set { this.IdKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of location latitude field.
        /// </summary>
        public string LocationLatitudeKey {
            get { return this.locationLatitudeKey.Value; }
            set { this.locationLatitudeKey.Value = value; }
        }
        private readonly PersistentFieldForString locationLatitudeKey =
            new PersistentFieldForString(nameof(LocationLatitudeKey));

        /// <summary>
        /// Key chain of location latitude field.
        /// </summary>
        public string[] LocationLatitudeKeyChain {
            get { return KeyChain.FromKey(this.LocationLatitudeKey); }
            set { this.LocationLatitudeKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of location longitude field.
        /// </summary>
        public string LocationLongitudeFieldKey {
            get { return this.locationLongitudeFieldKey.Value; }
            set { this.locationLongitudeFieldKey.Value = value; }
        }
        private readonly PersistentFieldForString locationLongitudeFieldKey =
            new PersistentFieldForString(nameof(LocationLongitudeFieldKey));

        /// <summary>
        /// Key chain of location longitude field.
        /// </summary>
        public string[] LocationLongitudeFieldKeyChain {
            get { return KeyChain.FromKey(this.LocationLongitudeFieldKey); }
            set { this.LocationLongitudeFieldKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of location title field.
        /// </summary>
        public string LocationTitleKey {
            get { return this.locationTitleKey.Value; }
            set { this.locationTitleKey.Value = value; }
        }
        private readonly PersistentFieldForString locationTitleKey =
            new PersistentFieldForString(nameof(LocationTitleKey));

        /// <summary>
        /// Key chain of location title field.
        /// </summary>
        public string[] LocationTitleKeyChain {
            get { return KeyChain.FromKey(this.LocationTitleKey); }
            set { this.LocationTitleKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of modified at field.
        /// </summary>
        public string ModifiedAtKey {
            get { return this.modifiedAtKey.Value; }
            set { this.modifiedAtKey.Value = value; }
        }
        private readonly PersistentFieldForString modifiedAtKey =
            new PersistentFieldForString(nameof(ModifiedAtKey), nameof(PersistentObject.ModifiedAt));

        /// <summary>
        /// Key chain of modified at field.
        /// </summary>
        public string[] ModifiedAtKeyChain {
            get { return KeyChain.FromKey(this.ModifiedAtKey); }
            set { this.ModifiedAtKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of organizer field.
        /// </summary>
        public string OrganizerKey {
            get { return this.organizerKey.Value; }
            set { this.organizerKey.Value = value; }
        }
        private readonly PersistentFieldForString organizerKey =
            new PersistentFieldForString(nameof(OrganizerKey));

        /// <summary>
        /// Key chain of organizer field.
        /// </summary>
        public string[] OrganizerKeyChain {
            get { return KeyChain.FromKey(this.OrganizerKey); }
            set { this.OrganizerKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of priory field.
        /// </summary>
        public string PriorityKey {
            get { return this.priorityKey.Value; }
            set { this.priorityKey.Value = value; }
        }
        private readonly PersistentFieldForString priorityKey =
            new PersistentFieldForString(nameof(PriorityKey));

        /// <summary>
        /// Key chain of priory field.
        /// </summary>
        public string[] PriorityKeyChain {
            get { return KeyChain.FromKey(this.PriorityKey); }
            set { this.PriorityKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of recurrence date/times field.
        /// </summary>
        public string RecurrenceDateTimesKey {
            get { return this.recurrenceDateTimesKey.Value; }
            set { this.recurrenceDateTimesKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceDateTimesKey =
            new PersistentFieldForString(nameof(RecurrenceDateTimesKey));

        /// <summary>
        /// Key chain of recurrence date/times field.
        /// </summary>
        public string[] RecurrenceDateTimesKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceDateTimesKey); }
            set { this.RecurrenceDateTimesKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of recurrence ID field.
        /// </summary>
        public string RecurrenceIdKey {
            get { return this.recurrenceIdKey.Value; }
            set { this.recurrenceIdKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceIdKey =
            new PersistentFieldForString(nameof(RecurrenceIdKey));

        /// <summary>
        /// Key chain of recurrence ID field.
        /// </summary>
        public string[] RecurrenceIdKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceIdKey); }
            set { this.RecurrenceIdKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by day field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleByDayKey {
            get { return this.recurrenceRuleByDayKey.Value; }
            set { this.recurrenceRuleByDayKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleByDayKey =
            new PersistentFieldForString(nameof(RecurrenceRuleByDayKey));

        /// <summary>
        /// Key chain of by day field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleByDayKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleByDayKey); }
            set { this.RecurrenceRuleByDayKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by hour field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleByHourKey {
            get { return this.recurrenceRuleByHourKey.Value; }
            set { this.recurrenceRuleByHourKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleByHourKey =
            new PersistentFieldForString(nameof(RecurrenceRuleByHourKey));

        /// <summary>
        /// Key chain of by hour field of by day field of
        /// recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleByHourKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleByHourKey); }
            set { this.RecurrenceRuleByHourKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by minute field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleByMinuteKey {
            get { return this.recurrenceRuleByMinuteKey.Value; }
            set { this.recurrenceRuleByMinuteKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleByMinuteKey =
            new PersistentFieldForString(nameof(RecurrenceRuleByMinuteKey));

        /// <summary>
        /// Key chain of by minute field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleByMinuteKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleByMinuteKey); }
            set { this.RecurrenceRuleByMinuteKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by month field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleByMonthKey {
            get { return this.recurrenceRuleByMonthKey.Value; }
            set { this.recurrenceRuleByMonthKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleByMonthKey =
            new PersistentFieldForString(nameof(RecurrenceRuleByMonthKey));

        /// <summary>
        /// Key chain of by month field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleByMonthKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleByMonthKey); }
            set { this.RecurrenceRuleByMonthKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by month day field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleByMonthDayKey {
            get { return this.recurrenceRuleByMonthDayKey.Value; }
            set { this.recurrenceRuleByMonthDayKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleByMonthDayKey =
            new PersistentFieldForString(nameof(RecurrenceRuleByMonthDayKey));

        /// <summary>
        /// Key chain of by month day field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleByMonthDayKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleByMonthDayKey); }
            set { this.RecurrenceRuleByMonthDayKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by second field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleBySecondKey {
            get { return this.recurrenceRuleBySecondKey.Value; }
            set { this.recurrenceRuleBySecondKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleBySecondKey =
            new PersistentFieldForString(nameof(RecurrenceRuleBySecondKey));

        /// <summary>
        /// Key chain of by second field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleBySecondKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleBySecondKey); }
            set { this.RecurrenceRuleBySecondKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by set-pos field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleBySetPosKey {
            get { return this.recurrenceRuleBySetPosKey.Value; }
            set { this.recurrenceRuleBySetPosKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleBySetPosKey =
            new PersistentFieldForString(nameof(RecurrenceRuleBySetPosKey));

        /// <summary>
        /// Key chain of by set-pos field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleBySetPosKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleBySetPosKey); }
            set { this.RecurrenceRuleBySetPosKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by week number field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleByWeekNumberKey {
            get { return this.recurrenceRuleByWeekNumberKey.Value; }
            set { this.recurrenceRuleByWeekNumberKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleByWeekNumberKey =
            new PersistentFieldForString(nameof(RecurrenceRuleByWeekNumberKey));

        /// <summary>
        /// Key chain of by week number field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleByWeekNumberKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleByWeekNumberKey); }
            set { this.RecurrenceRuleByWeekNumberKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of by year day field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleByYearDayKey {
            get { return this.recurrenceRuleByYearDayKey.Value; }
            set { this.recurrenceRuleByYearDayKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleByYearDayKey =
            new PersistentFieldForString(nameof(RecurrenceRuleByYearDayKey));

        /// <summary>
        /// Key chain of by year day field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleByYearDayKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleByYearDayKey); }
            set { this.RecurrenceRuleByYearDayKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of count field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleCountKey {
            get { return this.recurrenceRuleCountKey.Value; }
            set { this.recurrenceRuleCountKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleCountKey =
            new PersistentFieldForString(nameof(RecurrenceRuleCountKey));

        /// <summary>
        /// Key chain of count field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleCountKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleCountKey); }
            set { this.RecurrenceRuleCountKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of end date/time field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleEndDateTimeKey {
            get { return this.recurrenceRuleEndDateTimeKey.Value; }
            set { this.recurrenceRuleEndDateTimeKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleEndDateTimeKey =
            new PersistentFieldForString(nameof(RecurrenceRuleEndDateTimeKey));

        /// <summary>
        /// Key chain of end date/time field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleEndDateTimeKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleEndDateTimeKey); }
            set { this.RecurrenceRuleEndDateTimeKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of frequency field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleFrequencyKey {
            get { return this.recurrenceRuleFrequencyKey.Value; }
            set { this.recurrenceRuleFrequencyKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleFrequencyKey =
            new PersistentFieldForString(nameof(RecurrenceRuleFrequencyKey));

        /// <summary>
        /// Key chain of frequency field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleFrequencyKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleFrequencyKey); }
            set { this.RecurrenceRuleFrequencyKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of interval field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleIntervalKey {
            get { return this.recurrenceRuleIntervalKey.Value; }
            set { this.recurrenceRuleIntervalKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleIntervalKey =
            new PersistentFieldForString(nameof(RecurrenceRuleIntervalKey));

        /// <summary>
        /// Key chain of interval field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleIntervalKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleIntervalKey); }
            set { this.RecurrenceRuleIntervalKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of week start field of recurrence rule.
        /// </summary>
        public string RecurrenceRuleWeekStartKey {
            get { return this.recurrenceRuleWeekStartKey.Value; }
            set { this.recurrenceRuleWeekStartKey.Value = value; }
        }
        private readonly PersistentFieldForString recurrenceRuleWeekStartKey =
            new PersistentFieldForString(nameof(RecurrenceRuleWeekStartKey));

        /// <summary>
        /// Key chain of week start field of recurrence rule.
        /// </summary>
        public string[] RecurrenceRuleWeekStartKeyChain {
            get { return KeyChain.FromKey(this.RecurrenceRuleWeekStartKey); }
            set { this.RecurrenceRuleWeekStartKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of resources field.
        /// </summary>
        public string ResourcesKey {
            get { return this.resourcesKey.Value; }
            set { this.resourcesKey.Value = value; }
        }
        private readonly PersistentFieldForString resourcesKey =
            new PersistentFieldForString(nameof(ResourcesKey));

        /// <summary>
        /// Key chain of resources field.
        /// </summary>
        public string[] ResourcesKeyChain {
            get { return KeyChain.FromKey(this.ResourcesKey); }
            set { this.ResourcesKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of sequence field.
        /// </summary>
        public string SequenceKey {
            get { return this.sequenceKey.Value; }
            set { this.sequenceKey.Value = value; }
        }
        private readonly PersistentFieldForString sequenceKey =
            new PersistentFieldForString(nameof(SequenceKey));

        /// <summary>
        /// Key chain of sequence field.
        /// </summary>
        public string[] SequenceKeyChain {
            get { return KeyChain.FromKey(this.SequenceKey); }
            set { this.SequenceKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of start date/time field.
        /// </summary>
        public string StartDateTimeKey {
            get { return this.startDateTimeKey.Value; }
            set { this.startDateTimeKey.Value = value; }
        }
        private readonly PersistentFieldForString startDateTimeKey =
            new PersistentFieldForString(nameof(StartDateTimeKey));

        /// <summary>
        /// Key chain of start date/time field.
        /// </summary>
        public string[] StartDateTimeKeyChain {
            get { return KeyChain.FromKey(this.StartDateTimeKey); }
            set { this.StartDateTimeKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of status field.
        /// </summary>
        public string StatusKey {
            get { return this.statusKey.Value; }
            set { this.statusKey.Value = value; }
        }
        private readonly PersistentFieldForString statusKey =
            new PersistentFieldForString(nameof(StatusKey));

        /// <summary>
        /// Key chain of status field.
        /// </summary>
        public string[] StatusKeyChain {
            get { return KeyChain.FromKey(this.StatusKey); }
            set { this.StatusKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of time transparency field.
        /// </summary>
        public string TimeTransparencyKey {
            get { return this.timeTransparencyKey.Value; }
            set { this.timeTransparencyKey.Value = value; }
        }
        private readonly PersistentFieldForString timeTransparencyKey =
            new PersistentFieldForString(nameof(TimeTransparencyKey));

        /// <summary>
        /// Key chain of time transparency field.
        /// </summary>
        public string[] TimeTransparencyKeyChain {
            get { return KeyChain.FromKey(this.TimeTransparencyKey); }
            set { this.TimeTransparencyKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of title field.
        /// </summary>
        public string TitleKey {
            get { return this.titleKey.Value; }
            set { this.titleKey.Value = value; }
        }
        private readonly PersistentFieldForString titleKey =
            new PersistentFieldForString(nameof(TitleKey));

        /// <summary>
        /// Key chain of title field.
        /// </summary>
        public string[] TitleKeyChain {
            get { return KeyChain.FromKey(this.TitleKey); }
            set { this.TitleKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Key of web site field.
        /// </summary>
        public string WebSiteKey {
            get { return this.webSiteKey.Value; }
            set { this.webSiteKey.Value = value; }
        }
        private readonly PersistentFieldForString webSiteKey =
            new PersistentFieldForString(nameof(WebSiteKey));

        /// <summary>
        /// Key chain of web site field.
        /// </summary>
        public string[] WebSiteKeyChain {
            get { return KeyChain.FromKey(this.WebSiteKey); }
            set { this.WebSiteKey = KeyChain.ToKey(value); }
        }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        public VEventListView()
            : base() {
            this.RegisterPersistentField(this.alarmsKey);
            this.RegisterPersistentField(this.attachmentsKey);
            this.RegisterPersistentField(this.categoriesKey);
            this.RegisterPersistentField(this.classificationKey);
            this.RegisterPersistentField(this.commentsKey);
            this.RegisterPersistentField(this.contactsKey);
            this.RegisterPersistentField(this.createdAtKey);
            this.RegisterPersistentField(this.descriptionKey);
            this.RegisterPersistentField(this.endDateTimeKey);
            this.RegisterPersistentField(this.exceptionDateTimesKey);
            this.RegisterPersistentField(this.idKey);
            this.RegisterPersistentField(this.locationLatitudeKey);
            this.RegisterPersistentField(this.locationLongitudeFieldKey);
            this.RegisterPersistentField(this.locationTitleKey);
            this.RegisterPersistentField(this.modifiedAtKey);
            this.RegisterPersistentField(this.organizerKey);
            this.RegisterPersistentField(this.priorityKey);
            this.RegisterPersistentField(this.recurrenceDateTimesKey);
            this.RegisterPersistentField(this.recurrenceIdKey);
            this.RegisterPersistentField(this.recurrenceRuleByDayKey);
            this.RegisterPersistentField(this.recurrenceRuleByHourKey);
            this.RegisterPersistentField(this.recurrenceRuleByMinuteKey);
            this.RegisterPersistentField(this.recurrenceRuleByMonthKey);
            this.RegisterPersistentField(this.recurrenceRuleByMonthDayKey);
            this.RegisterPersistentField(this.recurrenceRuleBySecondKey);
            this.RegisterPersistentField(this.recurrenceRuleBySetPosKey);
            this.RegisterPersistentField(this.recurrenceRuleByWeekNumberKey);
            this.RegisterPersistentField(this.recurrenceRuleByYearDayKey);
            this.RegisterPersistentField(this.recurrenceRuleCountKey);
            this.RegisterPersistentField(this.recurrenceRuleEndDateTimeKey);
            this.RegisterPersistentField(this.recurrenceRuleFrequencyKey);
            this.RegisterPersistentField(this.recurrenceRuleIntervalKey);
            this.RegisterPersistentField(this.recurrenceRuleWeekStartKey);
            this.RegisterPersistentField(this.resourcesKey);
            this.RegisterPersistentField(this.sequenceKey);
            this.RegisterPersistentField(this.startDateTimeKey);
            this.RegisterPersistentField(this.statusKey);
            this.RegisterPersistentField(this.timeTransparencyKey);
            this.RegisterPersistentField(this.titleKey);
            this.RegisterPersistentField(this.webSiteKey);
        }

    }

}