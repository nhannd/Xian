#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyLoaders.Local
{
    [ExtensionOf(typeof(ServiceNodeServiceProviderExtensionPoint))]
    internal class StudyLoaderServiceProvider : ServiceNodeServiceProvider
    {
        private bool IsLocalServiceNode
        {
            get
            {
                var dicomServiceNode = Context.ServiceNode as IDicomServiceNode;
                return dicomServiceNode != null && dicomServiceNode.IsLocal && StudyStore.IsSupported;
            }
        }

        public override bool IsSupported(System.Type type)
        {
            return type == typeof (IStudyLoader) && IsLocalServiceNode;
        }

        public override object GetService(System.Type type)
        {
            return IsSupported(type) ? new LocalStoreStudyLoader() : null;
        }
    }

    //TODO (Marmot):Move once IStudyLoader is moved to Common?

    [ExtensionOf(typeof(StudyLoaderExtensionPoint))]
    public class LocalStoreStudyLoader : StudyLoader
    {
        private IEnumerator<ISopInstance> _sops;

        public LocalStoreStudyLoader() : base("DICOM_LOCAL")
        {
     
        }

        protected override int OnStart(StudyLoaderArgs studyLoaderArgs)
        {
            _sops = null;

            EventResult result = EventResult.Success;
            var loadedInstances = new AuditedInstances();
            try
            {
                using (var context = new DataAccessContext())
                {
                    IStudy study = context.GetStudyBroker().GetStudy(studyLoaderArgs.StudyInstanceUid);
                    if (study == null)
                    {
                        result = EventResult.MajorFailure;
                        loadedInstances.AddInstance(studyLoaderArgs.StudyInstanceUid);
                        throw new NotFoundLoadStudyException(studyLoaderArgs.StudyInstanceUid);
                    }
                    loadedInstances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                    _sops = study.GetSopInstances().GetEnumerator();
                    return study.NumberOfStudyRelatedInstances;
                }
            }
            finally
            {
                AuditHelper.LogOpenStudies(new[] { AuditHelper.LocalAETitle }, loadedInstances, EventSource.CurrentUser, result);
            }
        }

        protected override SopDataSource LoadNextSopDataSource()
        {
            if (_sops == null)
                return null;

            if (!_sops.MoveNext())
            {
                _sops = null;
                return null;
            }

            return new LocalStoreSopDataSource(_sops.Current);
        }
    }
}