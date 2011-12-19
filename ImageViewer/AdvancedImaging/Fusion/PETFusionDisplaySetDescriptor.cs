#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	public class PETFusionDisplaySetDescriptor : DicomDisplaySetDescriptor, IFusionDisplaySetDescriptor
	{
		[CloneCopyReference]
		private readonly ISeriesIdentifier _petSeries;

		private readonly bool _attenuationCorrection;
		private readonly string _fusionSeriesInstanceUid;

		public PETFusionDisplaySetDescriptor(ISeriesIdentifier baseSeries, ISeriesIdentifier ptSeries, bool attenuationCorrection)
			: base(baseSeries, null)
		{
			_petSeries = ptSeries;
			_attenuationCorrection = attenuationCorrection;
			_fusionSeriesInstanceUid = DicomUid.GenerateUid().UID;

		    IsComposite = true;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected PETFusionDisplaySetDescriptor(PETFusionDisplaySetDescriptor source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		public ISeriesIdentifier PETSeries
		{
			get { return _petSeries; }
		}

		ISeriesIdentifier IFusionDisplaySetDescriptor.OverlaySeries
		{
			get { return PETSeries; }
		}

		public bool AttenuationCorrection
		{
			get { return _attenuationCorrection; }
		}

		protected override string GetName()
		{
			return string.Format(SR.FormatPETFusionDisplaySet, GetSeriesDisplaySetName(base.SourceSeries), GetSeriesDisplaySetName(this.PETSeries),
			                     GetAttentuationCorrectionLabel()
				);
		}

		protected override string GetDescription()
		{
			return string.Format(SR.FormatPETFusionDisplaySet, GetSeriesDisplaySetDescription(base.SourceSeries), GetSeriesDisplaySetDescription(this.PETSeries),
			                     GetAttentuationCorrectionLabel()
				);
		}

		protected override string GetUid()
		{
			return _fusionSeriesInstanceUid;
		}

		private string GetAttentuationCorrectionLabel()
		{
			return AttenuationCorrection ? SR.LabelAttenuationCorrection : SR.LabelNoAttentuationCorrection;
		}

		private static string GetSeriesDisplaySetName(ISeriesIdentifier series)
		{
			if (string.IsNullOrEmpty(series.SeriesDescription))
				return string.Format("{0}", series.SeriesNumber);
			else
				return string.Format("{0} - {1}", series.SeriesNumber, series.SeriesDescription);
		}

		private static string GetSeriesDisplaySetDescription(ISeriesIdentifier series)
		{
			if (string.IsNullOrEmpty(series.SeriesDescription))
				return string.Format("{0}", series.SeriesNumber);
			else
				return string.Format("{0}", series.SeriesDescription);
		}
	}
}