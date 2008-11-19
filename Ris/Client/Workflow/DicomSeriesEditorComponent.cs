#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

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

		public DicomSeriesEditorComponent(DicomSeriesDetail dicomSeries)
		{
			_dicomSeries = dicomSeries;
		}

		#region Presentation Model

		public string SeriesNumber
		{
			get { return _dicomSeries.SeriesNumber; }
			set
			{
				_dicomSeries.SeriesNumber = value;
				this.Modified = true;
			}
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
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
