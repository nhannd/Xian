#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Audit;
using DicomEventResult = ClearCanvas.Dicom.Audit.EventResult;

namespace ClearCanvas.ImageViewer.Common.Auditing
{
	/// <summary>
	/// Represents the result of a particular auditable event.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventResult"/> has a 1-to-1 mapping with a <see cref="EventIdentificationContentsEventOutcomeIndicator"/>,
	/// but <see cref="EventResult"/> uses <see cref="AuditHelper"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public class EventResult
	{
		/// <summary>
		/// The auditable event completed successfully.
		/// </summary>
		public static DicomEventResult Success
		{
			get { return DicomEventResult.Success; }
		}

		/// <summary>
		/// The auditable event finished with minor errors.
		/// </summary>
		public static DicomEventResult MinorFailure
		{
			get { return DicomEventResult.MinorFailure; }
		}

		/// <summary>
		/// The auditable event finished with major errors.
		/// </summary>
		public static DicomEventResult MajorFailure
		{
			get { return DicomEventResult.MajorFailure; }
		}

		/// <summary>
		/// The auditable event finished with serious errors.
		/// </summary>
		public static DicomEventResult SeriousFailure
		{
			get { return DicomEventResult.SeriousFailure; }
		}

		private readonly DicomEventResult _result;

		private EventResult(DicomEventResult result)
		{
			_result = result;
		}

		public static implicit operator DicomEventResult(EventResult result)
		{
			return result._result;
		}

		public static implicit operator EventResult(DicomEventResult result)
		{
			return new EventResult(result);
		}
	}
}