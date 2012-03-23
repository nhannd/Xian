#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel
{
	//TODO (CR Sept 2010): restore data contract references to this later (e.g. next time we are releasing migration services).
	//Make sure and analyze everthing that might be affected first.
	public static class DicomNamespace
	{
		public const string Value = "http://www.clearcanvas.ca/dicom";
	}

	[KnownType(typeof(DicomServerApplicationEntity))]
	[KnownType(typeof(StreamingServerApplicationEntity))]
	//[DataContract(Namespace = DicomNamespace.Value)]
    public class ApplicationEntity : IApplicationEntity, IEquatable<ApplicationEntity>
	{
		public ApplicationEntity()
		{}

		public ApplicationEntity(string aeTitle)
		{
			AETitle = aeTitle;
		}

		public ApplicationEntity(string aeTitle, string name, string description, string location)
			: this(aeTitle)
		{
			Description = description;
			Name = name;
			Location = location;
		}

		#region IApplicationEntity Members

		[DataMember(IsRequired = true)]
		public string AETitle { get; set; }

		[DataMember(IsRequired = false)]
		public string Name { get; set; }

		[DataMember(IsRequired = false)]
		public string Description { get; set; }

		[DataMember(IsRequired = false)]
		public string Location { get; set; }

	    // TODO (CR Mar 2012): see if we can get rid of this later.
        public virtual bool IsStreaming { get { return false; } }

        public override string ToString()
        {
            return String.Format("Name: {0}\nAE: {1}\nDescription: {2}\nLocation:{3}",
                                 Name, AETitle, Description, Location);
        }

		#endregion

        public override bool Equals(object obj)
        {
            if (obj is ApplicationEntity)
                return Equals((ApplicationEntity)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 0x3568932;
            hash ^= AETitle.GetHashCode();
            hash ^= Name.GetHashCode();
            hash ^= Description.GetHashCode();
            hash ^= Location.GetHashCode();
            return hash;
        }

        #region IEquatable<ApplicationEntity> Members

        public bool Equals(ApplicationEntity other)
        {
            return (AETitle ?? "" ) == (other.AETitle ?? "") &&
                   (Name ?? "") == (other.Name ?? "") &&
                   (Description ?? "") == (other.Description ?? "") &&
                   (Location ?? "") == (other.Location ?? "") &&
                   IsStreaming == IsStreaming;
        }

        #endregion
    }
}
