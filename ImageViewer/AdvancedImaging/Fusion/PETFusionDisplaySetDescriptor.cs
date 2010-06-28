#region License

// Copyright (c) 2010, ClearCanvas Inc.
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