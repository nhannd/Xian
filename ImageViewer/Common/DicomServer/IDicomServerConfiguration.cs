﻿#region License

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
    //public interface IDicomServerConfigurationCallback
    //{
    //    [OperationContract]
    //    void ConfigurationChanged();
    //}

    [ServiceContract(SessionMode = SessionMode.Allowed,
        ConfigurationName = "IDicomServerConfiguration",
        //CallbackContract = typeof(IDicomServerConfigurationCallback),
        Namespace = DicomServerNamespace.Value)]
    public interface IDicomServerConfiguration
    {
        [OperationContract]
        GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request);

        [OperationContract]
        UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request);
    }
}
