#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
		WebEdit  
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