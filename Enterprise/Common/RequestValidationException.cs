#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using System.Threading;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Used by a service to indicate to the client that a request was rejected because it was invalid.  The client will likely
    /// display the contained message to the end user.  Therefore, the message should be appropriate for the end-user.
    /// </summary>
    [Serializable]
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message)
            :base(message)
        {
        }

        public RequestValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
