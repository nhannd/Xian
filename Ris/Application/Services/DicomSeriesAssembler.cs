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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Services
{
	public class DicomSeriesAssembler
	{
		class DicomSeriesSynchronizeHelper : CollectionSynchronizeHelper<DicomSeries, DicomSeriesDetail>
        {
            private readonly DicomSeriesAssembler _assembler;
            private readonly ModalityPerformedProcedureStep _mpps;
            private readonly IPersistenceContext _context;

            public DicomSeriesSynchronizeHelper(DicomSeriesAssembler assembler, ModalityPerformedProcedureStep mpps, IPersistenceContext context)
                : base(true, true)
            {
            	_assembler = assembler;
                _mpps = mpps;
                _context = context;
            }

            protected override bool CompareItems(DicomSeries domainItem, DicomSeriesDetail sourceItem)
            {
                return Equals(domainItem.GetRef(), sourceItem.DicomSeriesRef);
            }

            protected override void AddItem(DicomSeriesDetail sourceItem, ICollection<DicomSeries> domainList)
            {
                DicomSeries item = _assembler.CreateDicomSeries(sourceItem, _mpps);
                _context.Lock(item, DirtyState.New);
                _context.SynchState();
                sourceItem.DicomSeriesRef = item.GetRef();
                domainList.Add(item);
            }

            protected override void UpdateItem(DicomSeries domainItem, DicomSeriesDetail sourceItem, ICollection<DicomSeries> domainList)
            {
                _assembler.UpdateDicomSeries(domainItem, sourceItem);
            }

            protected override void RemoveItem(DicomSeries domainItem, ICollection<DicomSeries> domainList)
            {
				domainList.Remove(domainItem);
            }
        }

        public void SynchronizeDicomSeries(ModalityPerformedProcedureStep mpps, IList<DicomSeriesDetail> sourceList, IPersistenceContext context)
        {
			DicomSeriesSynchronizeHelper synchronizer = new DicomSeriesSynchronizeHelper(this, mpps, context);
			synchronizer.Synchronize(mpps.DicomSeries, sourceList);
        }

		public List<DicomSeriesDetail> GetDicomSeriesDetails(IEnumerable<DicomSeries> sourceList)
		{
			List<DicomSeriesDetail> dicomSeries = CollectionUtils.Map<DicomSeries, DicomSeriesDetail>(
				sourceList,
				delegate(DicomSeries series) { return CreateDicomSeriesDetail(series); });

			return dicomSeries;
		}

		#region Private Helpers

		private DicomSeries CreateDicomSeries(DicomSeriesDetail detail, ModalityPerformedProcedureStep mpps)
        {
			DicomSeries newSeries = new DicomSeries();
			newSeries.ModalityPerformedProcedureStep = mpps;
			UpdateDicomSeries(newSeries, detail);
			return newSeries;
        }

		private void UpdateDicomSeries(DicomSeries domainItem, DicomSeriesDetail sourceItem)
        {
            domainItem.StudyInstanceUID = sourceItem.StudyInstanceUID;
        	domainItem.SeriesInstanceUID = sourceItem.SeriesInstanceUID;
            domainItem.SeriesDescription = sourceItem.SeriesDescription;
            domainItem.SeriesNumber = sourceItem.SeriesNumber;
            domainItem.NumberOfSeriesRelatedInstances = sourceItem.NumberOfSeriesRelatedInstances;
        }

		private DicomSeriesDetail CreateDicomSeriesDetail(DicomSeries dicomSeries)
		{
			DicomSeriesDetail detail = new DicomSeriesDetail();

			detail.ModalityPerformedProcedureStepRef = dicomSeries.ModalityPerformedProcedureStep.GetRef();
			detail.DicomSeriesRef = dicomSeries.GetRef();
			detail.StudyInstanceUID = dicomSeries.StudyInstanceUID;
			detail.SeriesInstanceUID = dicomSeries.SeriesInstanceUID;
			detail.SeriesDescription = dicomSeries.SeriesDescription;
			detail.SeriesNumber = dicomSeries.SeriesNumber;
			detail.NumberOfSeriesRelatedInstances = dicomSeries.NumberOfSeriesRelatedInstances;

			return detail;
		}

		#endregion
	}
}
