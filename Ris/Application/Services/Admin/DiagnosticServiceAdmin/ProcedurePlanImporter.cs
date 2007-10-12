#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using System.Security.Permissions;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticServiceAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name="Procedure Plan Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ProcedurePlanImporter : DataImporterBase
    {
        private IUpdateContext _updateContext;
        private IDiagnosticServiceBroker _dsBroker;
        private IRequestedProcedureTypeBroker _rptBroker;
        private IModalityProcedureStepTypeBroker _sptBroker;
        private IModalityBroker _modalityBroker;

        private List<DiagnosticService> _diagnosticServices;
        private List<RequestedProcedureType> _rpTypes;
        private List<ModalityProcedureStepType> _spTypes;
        private List<Modality> _modalities;

        public ProcedurePlanImporter()
        {
        }

        public override bool SupportsCsv
        {
            get { return true; }
        }


        /// <summary>
        /// Import procedure plans from CSV format.
        /// </summary>
        /// <param name="lines">
        /// Each string in the list must contain 8 CSV fields, as follows:
        ///     0 - Diagnostic Service ID
        ///     1 - Diagnostic Service Name
        ///     2 - Requested Procedure Type ID
        ///     3 - Requested Procedure Type Name
        ///     4 - Modality Procedure Step Type ID
        ///     5 - Modality Procedure Step Type Name
        ///     6 - Default Modality ID
        ///     7 - Default Modality Name
        /// </param>
        /// <param name="context"></param>
        public override void ImportCsv(List<string> lines, IUpdateContext context)
        {
            _updateContext = context;
            _dsBroker = _updateContext.GetBroker<IDiagnosticServiceBroker>();
            _rptBroker = _updateContext.GetBroker<IRequestedProcedureTypeBroker>();
            _sptBroker = _updateContext.GetBroker<IModalityProcedureStepTypeBroker>();
            _modalityBroker = _updateContext.GetBroker<IModalityBroker>();

            _diagnosticServices = new List<DiagnosticService>();
            _rpTypes = new List<RequestedProcedureType>();
            _spTypes = new List<ModalityProcedureStepType>();
            _modalities = new List<Modality>();

            foreach (string line in lines)
            {
                // expect 8 fields in the row
                string[] row = ParseCsv(line, 8);


                string dsId = row[0];
                string dsName = row[1];
                string rptId = row[2];
                string rptName = row[3];
                string sptId = row[4];
                string sptName = row[5];
                string modId = row[6];
                string modName = row[7];

                Modality modality = GetModality(modId, modName);
                DiagnosticService ds = GetDiagnosticService(dsId, dsName);
                RequestedProcedureType rpt = GetRequestedProcedureType(rptId, rptName);
                ModalityProcedureStepType spt = GetModalityProcedureStepType(sptId, sptName, modality);

                if (!ds.RequestedProcedureTypes.Contains(rpt))
                {
                    ds.AddRequestedProcedureType(rpt);
                }

                if (!rpt.ModalityProcedureStepTypes.Contains(spt))
                {
                    rpt.AddModalityProcedureStepType(spt);
                }
            }
        }
        
        private DiagnosticService GetDiagnosticService(string id, string name)
        {
            // first check if we have it in memory
            DiagnosticService ds = CollectionUtils.SelectFirst<DiagnosticService>(_diagnosticServices,
                delegate(DiagnosticService s) { return s.Id == id; });

            // if not, check the database
            if (ds == null)
            {
                DiagnosticServiceSearchCriteria criteria = new DiagnosticServiceSearchCriteria();
                criteria.Id.EqualTo(id);

                ds = CollectionUtils.FirstElement<DiagnosticService>(_dsBroker.Find(criteria));

                // if not, create a transient instance
                if (ds == null)
                {
                    ds = new DiagnosticService(id, name);
                    _updateContext.Lock(ds, DirtyState.New);
                }

                _diagnosticServices.Add(ds);
            }

            // validate the name
            if (ds.Name != name)
                throw new ImportException(SR.ExceptionImportEntityNameIdMismatch);

            return ds;
        }

        private RequestedProcedureType GetRequestedProcedureType(string id, string name)
        {
            // first check if we have it in memory
            RequestedProcedureType rpType = CollectionUtils.SelectFirst<RequestedProcedureType>(_rpTypes,
                delegate(RequestedProcedureType rp) { return rp.Id == id; });

            // if not, check the database
            if (rpType == null)
            {
                RequestedProcedureTypeSearchCriteria criteria = new RequestedProcedureTypeSearchCriteria();
                criteria.Id.EqualTo(id);

                rpType = CollectionUtils.FirstElement<RequestedProcedureType>(_rptBroker.Find(criteria));

                // if not, create a transient instance
                if (rpType == null)
                {
                    rpType = new RequestedProcedureType(id, name);
                    _updateContext.Lock(rpType, DirtyState.New);
                }

                _rpTypes.Add(rpType);
            }

            // validate the name
            if (rpType.Name != name)
                throw new ImportException(SR.ExceptionImportEntityNameIdMismatch);

            return rpType;
        }

        private ModalityProcedureStepType GetModalityProcedureStepType(string id, string name, Modality modality)
        {
            // first check if we have it in memory
            ModalityProcedureStepType spType = CollectionUtils.SelectFirst<ModalityProcedureStepType>(_spTypes,
                delegate(ModalityProcedureStepType sp) { return sp.Id == id; });

            // if not, check the database
            if (spType == null)
            {
                ModalityProcedureStepTypeSearchCriteria criteria = new ModalityProcedureStepTypeSearchCriteria();
                criteria.Id.EqualTo(id);

                spType = CollectionUtils.FirstElement<ModalityProcedureStepType>(_sptBroker.Find(criteria));

                // if not, create a transient instance
                if (spType == null)
                {
                    spType = new ModalityProcedureStepType(id, name, modality);
                    _updateContext.Lock(spType, DirtyState.New);
                }

                _spTypes.Add(spType);
            }

            // validate the name
            if (spType.Name != name)
                throw new ImportException(SR.ExceptionImportEntityNameIdMismatch);

            if (!spType.DefaultModality.Equals(modality))
            {
                string message = string.Format("{0} {1} has default modality {2} {3} which does not match modality {4} {5}",
                    spType.Id, spType.Name, spType.DefaultModality.Id, spType.DefaultModality.Name, modality.Id, modality.Name);
                throw new ImportException(message);
            }

            return spType;
        }

        private Modality GetModality(string id, string name)
        {
            // first check if we have it in memory
            Modality modality = CollectionUtils.SelectFirst<Modality>(_modalities,
                delegate(Modality sp) { return sp.Id == id; });

            // if not, check the database
            if (modality == null)
            {
                ModalitySearchCriteria criteria = new ModalitySearchCriteria();
                criteria.Id.EqualTo(id);

                modality = CollectionUtils.FirstElement<Modality>(_modalityBroker.Find(criteria));

                // if not, create a transient instance
                if (modality == null)
                {
                    modality = new Modality(id, name);
                    _updateContext.Lock(modality, DirtyState.New);
                }

                _modalities.Add(modality);
            }

            // validate the name
            if (modality.Name != name)
                throw new ImportException(SR.ExceptionImportEntityNameIdMismatch);

            return modality;
        }

    }
}
