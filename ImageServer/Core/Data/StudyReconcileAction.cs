#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents action used for study reconciliation
	/// </summary>
	public enum StudyReconcileAction
	{
		[EnumInfo(ShortDescription = "Discard", LongDescription = "Discarded conflicting images")]
		Discard,

		[EnumInfo(ShortDescription = "Merge", LongDescription = "Merged study and conflicting images")]
		Merge,

		[EnumInfo(ShortDescription = "Split Studies", LongDescription = "Created new study from conflicting images")]
		CreateNewStudy,

		[EnumInfo(ShortDescription = "Process As Is", LongDescription = "Processed the images normally")]
		ProcessAsIs
	}
}