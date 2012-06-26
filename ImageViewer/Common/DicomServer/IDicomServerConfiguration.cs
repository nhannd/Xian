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
    // TODO (CR Jun 2012): Remove this? It's really not used anymore, although the "data contracts" remain useful.
    [ServiceContract(SessionMode = SessionMode.Allowed,
        ConfigurationName = "IDicomServerConfiguration",
        Namespace = DicomServerNamespace.Value)]
    public interface IDicomServerConfiguration
    {
        [OperationContract]
        GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request);

        [OperationContract]
        UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request);

        [OperationContract]
        GetDicomServerExtendedConfigurationResult GetExtendedConfiguration(GetDicomServerExtendedConfigurationRequest request);

        [OperationContract]
        UpdateDicomServerExtendedConfigurationResult UpdateExtendedConfiguration(UpdateDicomServerExtendedConfigurationRequest request);
    }
}
