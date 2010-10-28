#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Decoded information of the ChangeDescription field of a <see cref="StudyHistory"/> 
	/// record whose type is "WebEdited"
	/// </summary>
	public class WebEditStudyHistoryChangeDescription
	{
		#region Public Properties

		/// <summary>
		/// Type of the edit operation occured on the study.
		/// </summary>
		[XmlElement("EditType")]
		public EditType EditType { get; set; }

		/// <summary>
		/// Reason that the study is being editted
		/// </summary>
		[XmlElement("Reason")]
		public string Reason { get; set; }

		/// <summary>
		/// List of <see cref="BaseImageLevelUpdateCommand"/> that were executed on the study.
		/// </summary>
		[XmlArrayItem("Command", Type = typeof (AbstractProperty<BaseImageLevelUpdateCommand>))]
		public List<BaseImageLevelUpdateCommand> UpdateCommands { get; set; }

		[XmlElement("UserId")]
		public string UserId { get; set; }

		[XmlElement("TimeStamp")]
		public DateTime? TimeStamp { get; set; }

		#endregion
	}
}