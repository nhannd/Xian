#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Helpers;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Represents a command that can be executed on an <see cref="DicomFile"/>
	/// </summary>
	/// <remarks>
	/// This class is serializable.
	/// </remarks>
	[XmlRoot("SetTag")]
	public class SetTagCommand : BaseImageLevelUpdateCommand
	{
	    #region Private Fields

	    #endregion

		#region Constructors
		/// <summary>
		/// **** For serialization purpose. ****
		/// </summary>
		public SetTagCommand()
			: base("SetTag")
		{
		}

        /// <summary>
        /// Creates an instance of <see cref="SetTagCommand"/> that can be used to update the specified dicom tag with the specified value
        /// </summary>
        /// <remarks>
        /// <see cref="BaseImageLevelUpdateCommand.File"/> must be set prior to <see cref="BaseImageLevelUpdateCommand.OnExecute"></see>
        /// </remarks>
        public SetTagCommand(DicomAttribute attribute, string newValue)
            : this(attribute.Tag.TagValue, String.Empty, newValue)
        {
        }


		/// <summary>
		/// Creates an instance of <see cref="SetTagCommand"/> that can be used to update the specified dicom tag with the specified value
		/// </summary>
		/// <remarks>
		/// <see cref="BaseImageLevelUpdateCommand.File"/> must be set prior to <see cref="BaseImageLevelUpdateCommand.OnExecute"></see>
		/// </remarks>
		public SetTagCommand(uint tag, string originalValue, string value)
			: this()
		{
		    var dicomTag = DicomTagDictionary.GetDicomTag(tag);
            UpdateEntry.TagPath = new DicomTagPath { Tag = dicomTag };

            if (!string.IsNullOrEmpty(value))
            {
                int maxLength = dicomTag.VR.Equals(DicomVr.PNvr) ? 64 : (int)dicomTag.VR.MaximumLength;
                if (value.Length > maxLength)
                    value = value.Substring(0, maxLength);
            }

		    UpdateEntry.Value = value;
            UpdateEntry.OriginalValue = originalValue;
		}

        /// <summary>
        /// Creates an instance of <see cref="SetTagCommand"/> that can be used to update the specified dicom tag with the specified value
        /// </summary>
        /// <remarks>
        /// <see cref="BaseImageLevelUpdateCommand.File"/> must be set prior to <see cref="BaseImageLevelUpdateCommand.OnExecute"></see>
        /// </remarks>
        public SetTagCommand(uint tag, string newValue)
            : this(tag,String.Empty, newValue)
        {
            
        }

        /// <summary>
        /// Creates an instance of <see cref="SetTagCommand"/> that can be used to set the value of the specified dicom tag 
        /// in the <see cref="DicomFile"/>
        /// </summary>
        /// <remarks>
        /// <see cref="BaseImageLevelUpdateCommand.File"/> must be set prior to <see cref="BaseImageLevelUpdateCommand.OnExecute"></see>
        /// </remarks>
        public SetTagCommand(DicomFile file, uint tag, string originalValue, string value)
            : this(tag, originalValue, value)
        {
            File = file;
            
        }
		#endregion

		#region Public Properties

	    /// <summary>
		/// Gets the name of the Dicom tag affected by this command.
		/// **** For XML serialization purpose. ****
		/// </summary>
		[XmlAttribute(AttributeName = "TagName")]
		public string TagName
		{
			get { return UpdateEntry.TagPath.Tag.Name; }

			// Leave 'set' to public. It's used for serialization
            set {
				// NO-OP
			}
		}

        /// <summary>
        /// Gets/Sets the original value of the tag to be updated.
        /// **** For XML serialization purpose. ****
        /// </summary>
        [XmlAttribute(AttributeName = "OriginalValue")]
        public string OriginalValue
	    {
            get { return UpdateEntry.OriginalValue; }
            set
            {
                UpdateEntry.OriginalValue = value;
            }
	    }

		/// <summary>
		/// Gets or sets the Dicom tag value to be used by this command when updating the dicom file.
		/// </summary>
		[XmlAttribute(AttributeName = "Value")]
		public string Value
		{
			get
			{
				if (UpdateEntry == null)
					return null;

				return UpdateEntry.Value != null ? UpdateEntry.Value.ToString() : null;
			}
			set
			{
				UpdateEntry.Value = value;
			}
		}

		[XmlAttribute(AttributeName = "TagPath")]
		public string TagPath
		{
			get { return UpdateEntry.TagPath.HexString(); }
			set
			{
				DicomTagPathConverter converter = new DicomTagPathConverter();
				UpdateEntry.TagPath = (DicomTagPath)converter.ConvertFromString(value);
			}
		}

		[XmlIgnore]
		public DicomTag Tag
		{
			get
			{
				return UpdateEntry.TagPath.Tag;
			}
		}

	    #endregion

		#region IImageLevelCommand Members

		public override bool Apply(DicomFile file)
		{
			if (UpdateEntry != null)
			{
				DicomAttribute attr = FindAttribute(file.DataSet, UpdateEntry);
				if (attr != null)
				{
				    UpdateEntry.OriginalValue = attr.ToString();
					try
					{
					    var desiredValue = UpdateEntry.GetStringValue();
                        attr.SetStringValue(desiredValue);

                        //Make sure the data is not garbled when stored into file
                        var encodedValue = attr.GetEncodedString(file.TransferSyntax, file.DataSet.SpecificCharacterSet);
                        if (encodedValue != null)
                        {
                            var diff = Diff(encodedValue.Trim(), desiredValue.Trim());

                            if (diff >= 0)
                            {
                                string instanceNumber = file.DataSet[DicomTags.InstanceNumber].ToString();
                                string instanceUid = file.DataSet[DicomTags.SopInstanceUid].ToString();
                                char badChar = diff >= desiredValue.Length ? desiredValue[desiredValue.Length - 1] : desiredValue[diff];
                                var error = string.Format("SOP {4}\n\nCannot set {0} to {1}. Character {2} is not valid in character set {3}.",
                                                    UpdateEntry.TagPath.Tag.Name, desiredValue, badChar, file.DataSet.SpecificCharacterSet,
                                                    string.Format("#{0} [{1}]", instanceNumber, instanceUid)
                                                    );

                                Platform.Log(LogLevel.Error, error);
                                throw new InvalidDicomValueException(error);
                            }
                        }
					    
					   
					}
					catch (DicomDataException)
					{
                        //TODO: Why do we ignore it?
						Platform.Log(LogLevel.Warn, "Unexpected exception when updating tag {0} to value {1}, leaving current value: {2}",
						             UpdateEntry.TagPath, UpdateEntry.GetStringValue(),
						             attr.ToString());
						UpdateEntry.Value = attr.ToString();
					}
				}
			}

			return true;
		}



		public override string ToString()
		{
			return String.Format("Set {0}={1} [Original={2}]", UpdateEntry.TagPath.Tag, UpdateEntry.Value, UpdateEntry.OriginalValue?? "N/A or TBD");
		}
		#endregion

		protected static DicomAttribute FindAttribute(DicomAttributeCollection collection, ImageLevelUpdateEntry entry)
		{
			if (collection == null)
				return null;

			if (entry.TagPath.Parents != null)
			{
				foreach (DicomTag tag in entry.TagPath.Parents)
				{
					DicomAttribute sq = collection[tag] as DicomAttributeSQ;
					if (sq == null)
					{
						throw new Exception(String.Format("Invalid tag value: {0}({1}) is not a SQ VR", tag, tag.Name));
					}
					if (sq.IsEmpty)
					{
						DicomSequenceItem item = new DicomSequenceItem();
						sq.AddSequenceItem(item);
					}

					DicomSequenceItem[] items = sq.Values as DicomSequenceItem[];
					Platform.CheckForNullReference(items, "items");
					collection = items[0];
				}
			}

			return collection[entry.TagPath.Tag];
		}

        private int Diff(string s1, string s2)
        {
            if (s1.Equals(s2))
                return -1;

            int index = 0;
            for (; index < s1.Length && index < s2.Length; index++)
            {
                var  ch = s1[index];
                if (!s2[index].Equals(ch))
                    break;
            }

            return index;
        }
	}
}