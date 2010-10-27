#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Defines the interface of a handler that can process WADO requests.
    /// </summary>
    interface IObjectStreamingHandler
    {
        WADOResponse Process(WADORequestTypeHandlerContext context);
    }

    
}
