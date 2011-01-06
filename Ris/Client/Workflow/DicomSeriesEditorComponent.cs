#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomSeriesEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class DicomSeriesEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DicomSeriesEditorComponent class.
	/// </summary>
	[AssociateView(typeof(DicomSeriesEditorComponentViewExtensionPoint))]
	public class DicomSeriesEditorComponent : ApplicationComponent
	{
		private readonly DicomSeriesDetail _dicomSeries;
		private bool _isNew;

		public DicomSeriesEditorComponent(DicomSeriesDetail dicomSeries, bool isNew)
		{
			_dicomSeries = dicomSeries;
			_isNew = isNew;
		}

		#region Presentation Model

		public string StudyInstanceUID
		{
			get { return _dicomSeries.StudyInstanceUID; }
		}

		public string SeriesInstanceUID
		{
			get { return _dicomSeries.SeriesInstanceUID; }
		}

		[ValidateNotNull]
		public string SeriesNumber
		{
			get { return _dicomSeries.SeriesNumber; }
			set
			{
				_dicomSeries.SeriesNumber = value;
				this.Modified = true;
			}
		}

		public bool IsSeriesNumberReadOnly
		{
			get { return !_isNew; }
		}

		public string SeriesDescription
		{
			get { return _dicomSeries.SeriesDescription; }
			set
			{
				_dicomSeries.SeriesDescription = value;
				this.Modified = true;
			}
		}

		[ValidateGreaterThan(0)]
		public int NumberOfSeriesRelatedInstances
		{
			get { return _dicomSeries.NumberOfSeriesRelatedInstances; }
			set
			{
				_dicomSeries.NumberOfSeriesRelatedInstances = value;
				this.Modified = true;
			}
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
