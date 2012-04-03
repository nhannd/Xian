#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    //TODO (Marmot): Add callback contract?
    [ServiceContract(SessionMode = SessionMode.Allowed,
        ConfigurationName = "IDicomServerConfiguration",
        Namespace = DicomServerNamespace.Value)]
    public interface IDicomServerConfiguration
    {
        [OperationContract]
        GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request);

        [OperationContract]
        UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request);
    }
}
