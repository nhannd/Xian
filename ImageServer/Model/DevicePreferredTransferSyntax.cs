#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    public partial class DevicePreferredTransferSyntax : ServerEntity
    {

     
       
        public ServerSopClass GetServerSopClass()
        {
            return ServerSopClass.Load(ServerSopClassKey);
        }

        public ServerTransferSyntax GetServerTransferSyntax()
        {
            return ServerTransferSyntax.Load(ServerTransferSyntaxKey);
        }
    }
}
