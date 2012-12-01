#region License

//MWL Support for Clear Canvas RIS
//Copyright (C)  2012 Archibald Archibaldovitch

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Globalization;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Shreds.Mwl
{
    public enum DicomDataElementType
    {
        Type1,
        Type1C,
        Type2,
        Type2C,
        Type3
    }

    public interface IAttributeConverter
    {
        uint Tag { get; }
        DicomAttribute Extract(DicomMessage message);

        void GenerateSearchCriteria(string attributeValue, MwlWorklistItemSearchCriteria inputCriteria);
        void GenerateResponseMessage(MwlWorklistItem item, DicomMessage inputMessage);
    }

    public class AttributeConverter : IAttributeConverter
    {
        public AttributeConverter(uint tag, DicomDataElementType type, int maximumLength,
                                  Func<MwlWorklistItem, string> attributeGetter)
        {
            Tag = tag;
            DicomDataElementType = type;
            _attributeValueGetter = attributeGetter;
            _maxLength = maximumLength;
        }

        public AttributeConverter(uint tag, DicomDataElementType type, Func<MwlWorklistItem, string> attributeExtractor)
            : this(tag, type, 0, attributeExtractor)
        {
        }

        public AttributeConverter(uint tag, DicomDataElementType type)
            : this(tag, type, 0, null)
        {
        }

        public uint Tag { get; private set; }

        public virtual void GenerateSearchCriteria(string attributeValue, MwlWorklistItemSearchCriteria inputCriteria)
        {
        }

        public virtual void GenerateResponseMessage(MwlWorklistItem item, DicomMessage inputMessage)
        {
            var attributeValue = GetAttributeValue(item);
            attributeValue = string.IsNullOrEmpty(attributeValue) ? String.Empty : Truncate(attributeValue);

            bool shouldAdd;
            switch (DicomDataElementType)
            {
                case DicomDataElementType.Type1:
                case DicomDataElementType.Type2:
                    shouldAdd = true;
                    break;
                case DicomDataElementType.Type1C:
                case DicomDataElementType.Type2C:
                    shouldAdd = ShouldAddToReply(item);
                    break;
                default:
                    shouldAdd = !string.IsNullOrEmpty(attributeValue);
                    break;
            }

            if (!shouldAdd)
                return;

            var dicomAttribute = Extract(inputMessage);
            if (dicomAttribute is DicomAttributeSQ)
                return;

            dicomAttribute.SetStringValue(attributeValue);
        }

        public virtual DicomAttribute Extract(DicomMessage message)
        {
            return message.DataSet[DicomTagDictionary.GetDicomTag(Tag).TagValue];
        }

        public static string GeneratetDicomPersonName(PersonName name)
        {
            var builder = new StringBuilder();

            // standard name
            builder.Append(name.FamilyName);
            builder.Append(DicomNameDelimiter);
            builder.Append(name.GivenName);

            //middle name
            if (!string.IsNullOrEmpty(name.MiddleName))
            {
                builder.Append(DicomNameDelimiter);
                builder.Append(name.MiddleName);
            }

            //prefix
            if (!string.IsNullOrEmpty(name.Prefix))
            {
                builder.Append(DicomNameDelimiter);
                builder.Append(name.Prefix);
            }

            //suffix
            if (!string.IsNullOrEmpty(name.Suffix))
            {
                builder.Append(DicomNameDelimiter);
                builder.Append(name.Suffix);
            }

            return builder.ToString();
        }

        protected bool SwapDicomWildcardWithSqlWildcard(ref string attributeValue)
        {
            if (attributeValue.EndsWith(DicomWildcard.ToString(CultureInfo.InvariantCulture)))
            {
                attributeValue = attributeValue.Replace(DicomWildcard, SqlWildcard);
                return true;
            }

            return false;
        }


        protected DicomDataElementType DicomDataElementType { get; private set; }

        protected virtual bool ShouldAddToReply(MwlWorklistItem item)
        {
            return false;
        }

        protected string GetAttributeValue(MwlWorklistItem item)
        {
            return _attributeValueGetter == null ? null : _attributeValueGetter(item);
        }

        private string Truncate(string attributeValue)
        {
            if (_maxLength == 0 || string.IsNullOrEmpty(attributeValue))
                return attributeValue;
            return attributeValue.Length < _maxLength ? attributeValue : attributeValue.Substring(0, _maxLength);
        }

        private readonly int _maxLength;

        private readonly Func<MwlWorklistItem, string> _attributeValueGetter;

        protected const char DicomWildcard = '*';
        protected const char SqlWildcard = '%';

        protected const char DicomNameDelimiter = '^';

        protected const char DicomDateRangeDelimiter = '-';
        public const string DicomTimeFormat = "HHmmss";
    }

    public class SequenceConverter : AttributeConverter
    {
        public SequenceConverter(uint sequenceTag, uint tag, DicomDataElementType type,
                                 Func<MwlWorklistItem, string> attributeExtractor)
            : base(tag, type, attributeExtractor)
        {
            SequenceTag = sequenceTag;
        }

        public SequenceConverter(uint sequenceTag, uint tag, DicomDataElementType type, int maximumLength,
                                 Func<MwlWorklistItem, string> attributeGetter)
            : base(tag, type, maximumLength, attributeGetter)
        {
            SequenceTag = sequenceTag;
        }

        public SequenceConverter(uint sequenceTag, uint tag, DicomDataElementType type)
            : base(tag, type)
        {
            SequenceTag = sequenceTag;
        }

        public SequenceConverter(uint tag, DicomDataElementType type, Func<MwlWorklistItem, string> attributeExtractor)
            : this(tag, tag, type, attributeExtractor)
        {
        }

        public SequenceConverter(uint tag, DicomDataElementType type, int maximumLength,
                                 Func<MwlWorklistItem, string> attributeGetter)
            : this(tag, tag, type, maximumLength, attributeGetter)
        {
        }

        public SequenceConverter(uint tag, DicomDataElementType type)
            : this(tag, tag, type)
        {
        }

        public uint SequenceTag { get; private set; }

        public override DicomAttribute Extract(DicomMessage message)
        {
            return GetSequence(SequenceTag, message)[DicomTagDictionary.GetDicomTag(Tag).TagValue];
        }

        private static DicomSequenceItem GetSequence(uint sequenceTag, DicomMessageBase message)
        {
            if (!message.DataSet.Contains(sequenceTag))
            {
                var newSeq = new DicomAttributeSQ(sequenceTag);
                message.DataSet[sequenceTag] = newSeq;
                newSeq.AddSequenceItem(new DicomSequenceItem());
            }
            var sequence = (DicomAttributeSQ) message.DataSet[sequenceTag];
            if (sequence[0] == null)
                sequence.AddSequenceItem(new DicomSequenceItem());
            return sequence[0];
        }
    }

    public class ScheduledProcedureStepStartDateConverter : SequenceConverter
    {
        public ScheduledProcedureStepStartDateConverter()
            : base(
                DicomTags.ScheduledProcedureStepSequence, DicomTags.ScheduledProcedureStepStartDate,
                DicomDataElementType.Type1,
                item => item.Time == null ? null : DateParser.ToDicomString(item.Time.Value))
        {
        }

        public override void GenerateSearchCriteria(string attributeValue, MwlWorklistItemSearchCriteria inputCriteria)
        {
            //default time period
            if (string.IsNullOrEmpty(attributeValue))
            {
                var now = DateTime.Now;
                var min = now.AddHours(-MwlSettings.Default.DefaultScheduledProcedureStepStartDateMin);
                var max = now.AddHours(MwlSettings.Default.DefaultScheduledProcedureStepDateMax);
                inputCriteria.ProcedureStep.Scheduling.StartTime.Between(min, max);
            }
            else
            {
                // time range
                if (attributeValue.Contains(DicomDateRangeDelimiter.ToString(CultureInfo.InvariantCulture)))
                {
                    var parts = attributeValue.Split(DicomDateRangeDelimiter);
                    var min = DateParser.Parse(parts[0]);
                    var max = DateParser.Parse(parts[1]);
                    if (max != null)
                        max = max.Value.AddDays(1);

                    if (max == null)
                        inputCriteria.ProcedureStep.Scheduling.StartTime.MoreThanOrEqualTo(min);
                    else if (min == null)
                        inputCriteria.ProcedureStep.Scheduling.StartTime.LessThan(max);
                    else
                        inputCriteria.ProcedureStep.Scheduling.StartTime.Between(min, max);
                }
                else
                {
                    //24 hour range
                    //ToDo what if min is null
                    var min = DateParser.Parse(attributeValue);
                    var max = min.Value.AddDays(1);
                    inputCriteria.ProcedureStep.Scheduling.StartTime.Between(min, max);
                }
            }
        }
    }


    public class ScheduledProcedureStepModalityConverter : SequenceConverter
    {
        public ScheduledProcedureStepModalityConverter()
            : base(DicomTags.ScheduledProcedureStepSequence, DicomTags.Modality, DicomDataElementType.Type1, 16,
                   item => item.DicomModality == null ? null : item.DicomModality.Code)
        {
        }

        public override void GenerateSearchCriteria(string attributeValue, MwlWorklistItemSearchCriteria inputCriteria)
        {
            if (string.IsNullOrEmpty(attributeValue))
                return;

            using (var scope = new PersistenceScope(PersistenceContextType.Read))
            {
                var dicomModality =
                    PersistenceScope.CurrentContext.GetBroker<IEnumBroker>().Find<DicomModalityEnum>(attributeValue);
                if (dicomModality == null)
                {
                    scope.Complete();
                    return;
                }
                inputCriteria.Modality.DicomModality.EqualTo(dicomModality);
                scope.Complete();
            }
        }
    }


    public class ScheduledProcedureStepDescriptionConverter : SequenceConverter
    {
        public ScheduledProcedureStepDescriptionConverter()
            : base(
                DicomTags.ScheduledProcedureStepSequence, DicomTags.ScheduledProcedureStepDescription,
                DicomDataElementType.Type1C, 64,
                item => item.ProcedureName)
        {
        }


        protected override bool ShouldAddToReply(MwlWorklistItem item)
        {
            return !string.IsNullOrEmpty(GetAttributeValue(item));
        }
    }


    public class RequestedProcedureDescriptionConverter : AttributeConverter
    {
        public RequestedProcedureDescriptionConverter()
            : base(
                DicomTags.RequestedProcedureDescription, DicomDataElementType.Type1C, 64,
                item => item.ProcedureName)
        {
        }

        protected override bool ShouldAddToReply(MwlWorklistItem item)
        {
            return !string.IsNullOrEmpty(GetAttributeValue(item));
        }
    }

    public class PatientsNameConverter : AttributeConverter
    {
        public PatientsNameConverter()
            : base(
                DicomTags.PatientsName, DicomDataElementType.Type1, item => GeneratetDicomPersonName(item.PatientName))
        {
        }

        public override void GenerateSearchCriteria(string attributeValue, MwlWorklistItemSearchCriteria inputCriteria)
        {
            if (string.IsNullOrEmpty(attributeValue))
                return;

            var parts = attributeValue.Split(DicomNameDelimiter);

            //family name
            if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
            {
                if (SwapDicomWildcardWithSqlWildcard(ref parts[0]))
                    inputCriteria.PatientProfile.Name.FamilyName.Like(parts[0]);
                else
                    inputCriteria.PatientProfile.Name.FamilyName.EqualTo(parts[0]);
            }

            // given name
            if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
            {
                if (SwapDicomWildcardWithSqlWildcard(ref parts[1]))
                    inputCriteria.PatientProfile.Name.GivenName.Like(parts[1]);
                else
                    inputCriteria.PatientProfile.Name.GivenName.EqualTo(parts[1]);
            }
        }
    }

    public class PatientIdConverter : AttributeConverter
    {
        public PatientIdConverter()
            : base(
                DicomTags.PatientId, DicomDataElementType.Type1,
                item => item.Mrn.Id)
        {
        }

        public override void GenerateSearchCriteria(string attributeValue, MwlWorklistItemSearchCriteria inputCriteria)
        {
            if (string.IsNullOrEmpty(attributeValue))
                return;

            if (SwapDicomWildcardWithSqlWildcard(ref attributeValue))
                inputCriteria.PatientProfile.Mrn.Id.Like(attributeValue);
            else
                inputCriteria.PatientProfile.Mrn.Id.EqualTo(attributeValue);
        }
    }
}