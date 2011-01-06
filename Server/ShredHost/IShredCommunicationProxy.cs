#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Server.ShredHost
{
    [ServiceContract]
    public interface IShredCommunication
    {
        /// <summary>
        /// Requests the shred stop processing. All
        /// resources should be deallocated.
        /// </summary>
        [OperationContract]
        void Stop();
        /// <summary>
        /// Requests the shred pause processing
        /// This is different from Stop in the sense
        /// that resources are not deallocated.
        /// </summary>
        [OperationContract]
        void Pause();
        /// <summary>
        /// Unpause a paused shred.
        /// </summary>
        [OperationContract]
        void Unpause();
    }
}
