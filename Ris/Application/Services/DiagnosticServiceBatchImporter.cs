using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services
{
    internal class DiagnosticServiceBatchImporter
    {
        internal static void Import(IUpdateContext context, IList<string[]> data)
        {
            DiagnosticServiceBatchImporter importer = new DiagnosticServiceBatchImporter(context);
            importer.DoImport(data);
        }


        private IUpdateContext _updateContext;
        private IDiagnosticServiceBroker _dsBroker;
        private IRequestedProcedureTypeBroker _rptBroker;
        private IModalityProcedureStepTypeBroker _sptBroker;
        private IModalityBroker _modalityBroker;

        private List<DiagnosticService> _diagnosticServices = new List<DiagnosticService>();
        private List<RequestedProcedureType> _rpTypes = new List<RequestedProcedureType>();
        private List<ModalityProcedureStepType> _spTypes = new List<ModalityProcedureStepType>();
        private List<Modality> _modalities = new List<Modality>();

        private DiagnosticServiceBatchImporter(IUpdateContext updateContext)
        {
            _updateContext = updateContext;
            _dsBroker = _updateContext.GetBroker<IDiagnosticServiceBroker>();
            _rptBroker = _updateContext.GetBroker<IRequestedProcedureTypeBroker>();
            _sptBroker = _updateContext.GetBroker<IModalityProcedureStepTypeBroker>();
            _modalityBroker = _updateContext.GetBroker<IModalityBroker>();
        }

        private void DoImport(IList<string[]> data)
        {
            foreach (string[] row in data)
            {
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
                throw new ImportException("Scheduled Procedure Step Type default modality mismatch");

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
