#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common.ServiceModel
{
    /// <summary>
    /// Defines the interface of an alert service
    /// </summary>
    [ImageServerService]
    [Authentication(false)]
    [ServiceContract]
    public interface IAlertService
    {
        /// <summary>
        /// Generates an alert record based on the specified <see cref="Alert"/>
        /// </summary>
        /// <param name="alert"></param>
        [OperationContract]
        void GenerateAlert(Alert alert);
    }
}