#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Web.Common;
using ClearCanvas.ImageViewer.Web.Common.Entities;
using System;

namespace ClearCanvas.ImageViewer.Web.Common
{

    [DataContract]
    [Flags]
    public enum LoadStudyOptions
    {        
        /// <summary>
        ///  Load the entire study, no priors
        /// </summary>
        [EnumMember]
        EntireStudy,

        /// <summary>
        /// Load key images only. No effect if EntireStudy is set
        /// </summary>
        [EnumMember]
        KeyImagesOnly,

        /// <summary>
        /// Include priors. When used in conjuciton with KeyImagesOnly, only prior key images are loaded
        /// </summary>
        [EnumMember]
        IncludePriors
    }

	[DataContract(Namespace = ViewerNamespace.Value)]
	public class StartViewerApplicationRequest : StartApplicationRequest
	{
		[DataMember(IsRequired = true)]
		public string AeTitle { get; set; }

		[DataMember(IsRequired = false)]
		public string[] StudyInstanceUid { get; set; }

		[DataMember(IsRequired = false)]
		public string[] AccessionNumber { get; set; }

		[DataMember(IsRequired = false)]
		public string[] PatientId { get; set; }

        [DataMember(IsRequired = false)]
        public LoadStudyOptions LoadStudyOptions { get; set; }

        [DataMember(IsRequired = false)]
        public string ApplicationName { get; set; }
	}

	[DataContract(Namespace = ViewerNamespace.Value)]
	public class ViewerApplication : Application
	{
        [DataMember(IsRequired = true)]
        public string VersionString { get; set; }

		[DataMember(IsRequired = true)]
		public Viewer Viewer { get; set; }
	}
}