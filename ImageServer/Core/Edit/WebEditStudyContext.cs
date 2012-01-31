#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Type of study edit operation
	/// </summary>
	public enum EditType
	{
		/// <summary>
		/// User edited the study via the Web GUI
		/// </summary>
		[EnumInfo(ShortDescription="Web Edit", LongDescription="Edited using the Web GUI")]
		WebEdit,

        /// <summary>
        /// User edited the study via the Web GUI
        /// </summary>
        [EnumInfo(ShortDescription = "Web Service Edit", LongDescription = "Automatic edit caused by web service call")]
        WebServiceEdit  

	}

	/// <summary>
	/// Represents the context of the Web Edit Study operation.
	/// </summary>
	public class WebEditStudyContext
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the value indicating how the edit operation was triggered.
		/// </summary>
		public EditType EditType { get; set; }

		/// <summary>
		/// List of command executed on the images.
		/// </summary>
		public List<BaseImageLevelUpdateCommand> EditCommands { get; set; }

		/// <summary>
		/// Gets or sets the reference to the <see cref="StudyEditor"/>
		/// </summary>
		public StudyEditor WorkQueueProcessor { get; set; }

		/// <summary>
		/// Gets or sets the reference to the <see cref="ServerCommandProcessor"/> currently used.
		/// </summary>
		/// <remarks>
		/// Different <see cref="ServerCommandProcessor"/> may be used per images/series.
		/// </remarks>
		public ServerCommandProcessor CommandProcessor { get; set; }

		/// <summary>
		/// Gets or sets the original (prior to update) <see cref="StudyStorageLocation"/> object.
		/// </summary>
		/// <remarks>
		/// This property is a snapshot of the study location before the edit is executed. 
		/// Once the study has been updated, this object may contain invalid information.
		/// </remarks>
		public StudyStorageLocation OriginalStudyStorageLocation { get; set; }

		/// <summary>
		/// Gets or sets the new (updated) <see cref="StudyStorageLocation"/> object.
		/// </summary>
		/// <remarks>
		/// This property may be null if the study hasn't been updated or hasn't been determined. 
		/// Depending on what is modified, it may have the same or different data 
		/// compared with <see cref="OriginalStudyStorageLocation"/>.
		/// </remarks>
		public StudyStorageLocation NewStudystorageLocation { get; set; }

		/// <summary>
		/// Gets or sets the original <see cref="Study"/>
		/// </summary>
		/// <remarks>
		/// This property is a snapshot of the study before the edit is executed. 
		/// Once the study has been updated, this object may contain invalid information.
		/// </remarks>
		public Study OriginalStudy { get; set; }

		/// <summary>
		/// Gets or sets the reference to the original <see cref="Patient"/> before the study is updated.
		/// </summary>
		/// <remarks>
		/// This property is a snapshot of the patient before the edit is executed. 
		/// Once the study has been updated, this object may contain invalid information.
		/// </remarks>
		public Patient OrginalPatient { get; set; }

		/// <summary>
		/// Gets or sets the id of the user who requested the edit.
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the reason that the study is being editted.
		/// </summary>
		public string Reason { get; set; }

		#endregion
	}
}